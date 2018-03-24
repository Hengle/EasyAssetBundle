using System.Threading.Tasks;
using System;
using UnityEngine;
using UniRx;

namespace charcolle.EasyAssetBundle.Sample {

    public static class EasyAssetBundle {

        public async static Task<T> Load<T>( string resourceName, bool unLoad = false ) where T : UnityEngine.Object {
            return await EasyAssetBundleSampleManager.LoadResource<T>( resourceName, unLoad );
        }

        public static IObservable<T> ObservableLoad<T>( string resourceName ) where T : UnityEngine.Object {
            return Observable.Create<T>( req => {

                return new CompositeDisposable();
            } );
        }

        public async static Task<AssetBundle> LoadAssetBundle( string resourceName ) {
            var assetInfo = GetAssetInfo( resourceName );
            if( assetInfo != null ) {
                var assetBundle = await EasyAssetBundleSampleManager.LoadAssetBundle( assetInfo.AssetBundleName, assetInfo.IsExist );
                return assetBundle;
            }
            return null;
        }

        public static SampleAssetInfo GetAssetInfo( string resourceName ) {
            return EasyAssetBundleSampleManager.GetAssetInfo( resourceName );
        }

    }

}