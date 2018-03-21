namespace charcolle.Utility.EasyAssetBundle.v1 {

    public interface IAssetBundleBuildConfig {

        string GetAssetLabel( EditorAssetInfo info, string buildRootPath );

        bool CheckDirectoryLabel( EditorAssetInfo asset, string buildRootPath );

        bool CheckFileAvailable( EditorAssetInfo asset, string buildRootPath );

        string GetAssetBundleListSavePath( string fileName );

        bool IsConfigAvailable { get; }

        bool IsUseAssetBundleList { get; }

    }

}
