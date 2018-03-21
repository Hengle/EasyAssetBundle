using System;

namespace charcolle.EasyAssetBundle.Sample {

    [Serializable]
    public class SampleAssetInfo {

        public string Name;
        public string Path;
        public string Extension;
        public string AssetBundleName;
        public bool IsAssetBundle;
        public int Version;

        public bool IsExist;
        public string LocalPath;

    }

}