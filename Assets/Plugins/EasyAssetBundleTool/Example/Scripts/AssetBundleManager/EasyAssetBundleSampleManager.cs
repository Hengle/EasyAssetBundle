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

        private static bool IsDebug = false;

        #endregion varies

        #region initialize

        public async static Task Initialize( SampleAssetBundleInfoData data, string url, string cachePath, bool isDebug = false ) {
            IsDebug                = isDebug;
            if( IsDebug ) Debug.Log( "Initializing EasyAssetBundleSampleManager..." );
            CurrentLoadAssetBundle = new Dictionary<string, AssetBundle>();
            ResourceList           = data;
            AssetBundleURL         = url;
            AssetBundleCachePath   = cachePath;

            await Task.Run( () => {
                ResourceList.CheckCacheFileExists( AssetBundleCachePath );
                if( IsDebug ) Debug.Log( "Initialize EasyAssetBundleSampleManager finished." );
            } );
        }

        public static void SetURL( string url ) {
            AssetBundleURL = url;
        }

        #endregion initialize

        public static EasyAssetBundleLoadOperator GetEasyAssetBundleLoadOperator( int retry = 3 ) {
            return new EasyAssetBundleLoadOperator( AssetBundleURL, AssetBundleCachePath, retry, IsDebug );
        }

        public async static Task<T> LoadResource<T>( string resourceName, bool unLoad = false ) where T : UnityEngine.Object {
            if( ResourceList == null ) {
                Debug.LogError( "You must initialize EasyAssetBundleSampleManager before loading assetbundle." );
                return default( T );
            }
            var assetInfo = ResourceList.GetAssetInfo( resourceName );
            if( assetInfo != null ) {
                var assetBundleName = assetInfo.AssetBundleName;
                var assetBundle = await LoadAssetBundle( assetBundleName, assetInfo.IsExist );
                if( assetBundle == null )
                    return default( T );
                assetInfo.IsExist = true;
                var resource = LoadResourceFromBundle<T>( assetBundle, resourceName );
                if( unLoad ) {
                    assetBundle.Unload( false );
                    RemoveAssetBundleDic( assetBundleName );
                }

                return resource;
            }
            return default( T );
        }

        public async static Task<AssetBundle> LoadAssetBundle( string assetBundleName, bool isExist = false ) {
            AssetBundle assetBundle;
            CurrentLoadAssetBundle.TryGetValue( assetBundleName, out assetBundle );
            if( assetBundle == null ) {
                if( IsDebug ) Debug.Log( "Try to load AssetBundle:" + assetBundleName );
                var loadOperator = GetEasyAssetBundleLoadOperator();
                if( !isExist )
                    await loadOperator.DownLoadAssetBundle( assetBundleName );
                assetBundle = await loadOperator.LoadAssetBundleFromCache( assetBundleName );
                if( IsDebug ) Debug.Log( "Success to load AssetBundle:" + assetBundleName );
                AddAssetBundleDic( assetBundleName, assetBundle );
            } else {
                if( IsDebug ) Debug.Log( "Already loaded:" + assetBundleName );
            }
            return assetBundle;
        }

        public static T LoadResourceFromBundle<T>( AssetBundle assetBundle, string resourceName ) where T : UnityEngine.Object {
            if( assetBundle == null || assetBundle.isStreamedSceneAssetBundle )
                return null;
            return assetBundle.LoadAsset<T>( resourceName );
        }

        public static void UnLoadAssetBundle( string assetBundleName ) {
            AssetBundle assetBundle;
            CurrentLoadAssetBundle.TryGetValue( assetBundleName, out assetBundle );
            if( assetBundle != null ) {
                RemoveAssetBundleDic( assetBundleName );
                assetBundle.Unload( true );
            }
        }

        public static SampleAssetInfo GetAssetInfo( string resourceName ) {
            return ResourceList.GetAssetInfo( resourceName );
        }

        public static void AddAssetBundleDic( string assetBundleName, AssetBundle assetBundle ) {
            try {
                CurrentLoadAssetBundle.Add( assetBundleName, assetBundle );
            }
            catch {
                Debug.LogError( "fatal error when adding assetbundle-dic. : " + assetBundleName );
            }
        }

        public static void RemoveAssetBundleDic( string assetBundleName ) {
            try {
                CurrentLoadAssetBundle.Remove( assetBundleName );
            }
            catch {
                Debug.LogError( "fatal error when removing assetbundle-dic. : " + assetBundleName );
            }
        }

    }
}