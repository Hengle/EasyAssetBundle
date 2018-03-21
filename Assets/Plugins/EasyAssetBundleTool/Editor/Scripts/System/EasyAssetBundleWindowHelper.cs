using System.Linq;
using System.Collections.Generic;
using UnityEngine;

using SplitterState = charcolle.Utility.EasyAssetBundle.v1.SplitterState;
using FileHelper    = charcolle.Utility.EasyAssetBundle.v1.FileHelper;

namespace charcolle.Utility.EasyAssetBundle.v1 {

    internal static class WindowHelper {

        internal const string WIN_TITLE = "EasyAssetBundle";

        internal static SplitterState HorizontalStateMain;
        internal static SplitterState HorizontalStateSub;

        internal readonly static string[] MENU_BUILDERMENU      = new string[] { "Process Settings", "Label Setting" };
        internal readonly static string[] MENU_BUILDERASSETMENU = new string[] { "Menu", "", "Select Files", "Select DifferentUnityVersion" };
        internal readonly static string[] MENU_ASSETLISTMENU    = new string[] { "Menu", "", "Delete Selection", "Delete Invalid", "Set Version 1", "Check Duplicate" };

        internal static List<EasyAssetBundleBuilderData> Data = new List<EasyAssetBundleBuilderData>();

        internal static void Initialize( Rect win ) {
            HorizontalStateMain = new SplitterState( new float[] { win.width * 0.3f, win.width * 0.7f },
                                                              new int[] { 300, 300 }, new int[] { 400, 1000 } );
            HorizontalStateSub = new SplitterState( new float[] { win.width * 0.20f, win.width * 0.80f },
                                                              new int[] { 250, 250 }, new int[] { 500, 1000 } );

            Data = FileHelper.LoadAssetBundleConfigs();
        }

        internal static void OnGUIFirst() {
            GUI.skin.label.richText = true;
            GUI.skin.button.richText = true;
        }

        internal static void OnGUIEnd() {
            GUI.skin.label.richText = false;
            GUI.skin.button.richText = false;
        }

        //=======================================================
        // public
        //=======================================================

        internal static string[] BuilderDataList {
            get {
                if( BuilderDataExists )
                    return Data.Select( s => s.name ).ToArray();
                return null;
            }
        }

        internal static bool BuilderDataExists {
            get {
                if( Data == null || Data.Count == 0 )
                    return false;

                return true;
            }
        }

    }

}