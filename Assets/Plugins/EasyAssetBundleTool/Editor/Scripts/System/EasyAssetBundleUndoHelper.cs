using UnityEngine;
using UnityEditor;

namespace charcolle.Utility.EasyAssetBundle.v1 {

    internal static class UndoHelper {

        private static int UndoGroupId;

        internal static void Initialize() {
            UndoGroupId = Undo.GetCurrentGroup();
        }

        internal static void OnDisable() {
            Undo.CollapseUndoOperations( UndoGroupId );
        }

        internal static void BuilderDataUndo( string text ) {
            if( EasyAssetBundleWindow.win != null && EasyAssetBundleWindow.win.SelectedData != null )
                Undo.RecordObject( EasyAssetBundleWindow.win.SelectedData, text );
        }

        internal static void MainWindowUndo( string text ) {
            if( EasyAssetBundleWindow.win != null )
                Undo.RecordObject( EasyAssetBundleWindow.win, text );
        }

        internal static void AssetListWindowUndo( string text ) {
            if( EasyAssetBundleAssetListWindow.win != null )
                Undo.RecordObject( EasyAssetBundleAssetListWindow.win, text );
        }

        internal static void PopupWindowUndo( EditorWindow win, string text ) {
            if( win != null )
                Undo.RecordObject( win, text );
        }

    }

}