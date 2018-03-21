using System.Collections;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

namespace charcolle.EasyAssetBundle.Sample {

    public static class EasyAssetBundleSampleManager {

        #region varies

        /// <summary>
        /// this is your custom assetbundleList data.
        /// </summary>
        private static SampleAssetBundleInfoData ResourceList;

        private static Dictionary<string, AssetBundle> CurrentLoadAssetBundle = new Dictionary<string, AssetBundle>();

        private static string AssetBundleURL {
            get; set;
        }

        private static string AssetBundleCachePath {
            get; set;
        }

        #endregion varies

        #region initialize

        public async static Task Initialize( SampleAssetBundleInfoData data, string url, string cachePath ) {
            CurrentLoadAssetBundle = new Dictionary<string, AssetBundle>();
            ResourceList = data;
            AssetBundleURL = url;
            AssetBundleCachePath = cachePath;

            await Task.Run( () => {
                ResourceList.CheckCacheFileExists( AssetBundleCachePath );
            } );
        }

        public static void SetURL( string url ) {
            AssetBundleURL = url;
        }

        #endregion initialize

        public static EasyAssetBundleLoadOperator GetEasyAssetBundleLoadOperator( int retry = 3 ) {
            return new EasyAssetBundleLoadOperator( AssetBundleURL, AssetBundleCachePath, retry );
        }

        public async static Task<T> LoadResource<T>( string resourceName ) where T : UnityEngine.Object {
            if( ResourceList == null ) {
                Debug.LogError( "You must initialize EasyAssetBundleSampleManager before loading assetbundle." );
                return default( T );
            }

            var assetInfo = ResourceList.GetAssetInfo( resourceName );

            if( assetInfo != null ) {
                AssetBundle assetBundle;
                var assetBundleName = assetInfo.AssetBundleName;
                CurrentLoadAssetBundle.TryGetValue( assetBundleName, out assetBundle );
                if( assetBundle == null ) {
                    assetBundle = await LoadAssetBundle( assetBundleName, assetInfo.IsExist );
                    if( assetBundle == null )
                        return default( T );
                    CurrentLoadAssetBundle.Add( assetBundleName, assetBundle );
                }

                return LoadAssetFromBundle<T>( assetBundle, resourceName );
            }
            return default( T );
        }

        public async static Task<AssetBundle> LoadAssetBundle( string assetBundleName, bool IsExist = false ) {
            var loadOperator = GetEasyAssetBundleLoadOperator();
            if( !IsExist )
                await loadOperator.DownLoadAssetBundle( assetBundleName );
            return await loadOperator.LoadAssetBundleFromCache( assetBundleName );
        }

        public static T LoadAssetFromBundle<T>( AssetBundle assetBundle, string resourceName ) where T : UnityEngine.Object {
            if( assetBundle == null )
                return null;
            return assetBundle.LoadAsset<T>( resourceName );
        }
    }
}