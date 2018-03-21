using UnityEditor;
using System.Collections.Generic;

namespace charcolle.Utility.EasyAssetBundle.v1 {

    interface IAssetBundleExporter {

        void ExportVersionAssetBundleList();

        void ExportAssetBundle( List<EditorAssetInfo> buildAssets, string exportPath, string cachePath, BuildTarget[] platform, ExportType type, bool ignoreManifest );

    }

}