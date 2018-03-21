using System.Collections.Generic;

namespace charcolle.Utility.EasyAssetBundle.v1 {

    interface IAssetBundleList {

        string CurrentAssetBundleListName { get; set; }

        List<EditorAssetInfo> LoadAssetBundleList( string path );

        void UpdateAssetBundleList( Dictionary<string, List<EditorAssetInfo>> assetListDic, Dictionary<string, List<EditorAssetInfo>> versionListDic );

        void ConvertAssetBundleList( List<EditorAssetInfo> versionAssets, string savePath );

        void SaveAssetBundleList( List<EditorAssetInfo> versionAsset, string savePath );

    }
}