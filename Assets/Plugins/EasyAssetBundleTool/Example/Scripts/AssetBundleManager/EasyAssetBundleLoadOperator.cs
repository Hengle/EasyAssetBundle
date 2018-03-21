using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using UniRx;

namespace charcolle.EasyAssetBundle.Sample {

    public class EasyAssetBundleLoadOperator {

        private string Url;
        private string CachePath;
        private string AssetBundleName;
        private int RetryMax;

        public EasyAssetBundleLoadOperator( string url, string cachePath, int retry = 3 ) {
            Url       = url;
            CachePath = cachePath;
            RetryMax  = retry;
        }

        public async Task DownLoadAssetBundle( string assetBundleName ) {
            while( retryNum <= RetryMax ) {
                if( await DownloadAssetBundle( assetBundleName ) )
                    return;
                retryNum++;
            }
            Debug.LogError( "Fail to download assetbundle. : " + assetBundleName );
        }

        public async Task<AssetBundle> LoadAssetBundleFromCache( string assetBundleName ) {
            var path = CachePath + assetBundleName;

            var req = AssetBundle.LoadFromFileAsync( path );
            await req;
            return req.assetBundle;
        }

        private int retryNum;
        private async Task<bool> DownloadAssetBundle( string name ) {
            var request = UnityWebRequest.GetAssetBundle( Url );
            await request.SendWebRequest();

            if( !string.IsNullOrEmpty( request.error ) ) {
                if( EasyAssetBundleFileUtility.WriteAllBytes( CachePath + name, request.downloadHandler.data ) ) {
                    return true;
                } else {
                    retryNum = RetryMax;
                    return false;
                }
            } else {
                Debug.LogError( request.error );
            }
            request.Dispose();
            return false;
        }

    }

}