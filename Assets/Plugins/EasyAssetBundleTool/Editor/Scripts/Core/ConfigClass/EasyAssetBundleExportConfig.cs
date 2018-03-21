using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using GUIHelper = charcolle.Utility.EasyAssetBundle.v1.GUIHelper;
using UndoHelper = charcolle.Utility.EasyAssetBundle.v1.UndoHelper;

namespace charcolle.Utility.EasyAssetBundle.v1 {

    [Serializable]
    internal class EasyAssetBundleExportConfig {

        [SerializeField]
        internal string AssetBundleCachePath;
        [SerializeField]
        internal string ExportAssetBundlePath;
        [SerializeField]
        internal string ExportAssetListPath;
        [SerializeField]
        internal bool ExportWithManifest;
        [SerializeField]
        internal ExportType ExportType;

        internal void Initialize() {

        }

        internal EasyAssetBundleExportConfig() { }

        internal EasyAssetBundleExportConfig( EasyAssetBundleExportConfig copy ) {
            AssetBundleCachePath  = copy.AssetBundleCachePath;
            ExportAssetBundlePath = copy.ExportAssetBundlePath;
            ExportAssetListPath   = copy.ExportAssetListPath;
            ExportWithManifest    = copy.ExportWithManifest;
            ExportType            = copy.ExportType;
        }

        #region drawer
        //=======================================================
        // Drawer
        //=======================================================
        [ SerializeField]
        private bool Fold;
        internal void OnGUI() {
            GUILayout.Space( 5 );

            EditorGUILayout.BeginVertical( EditorStyles.helpBox );
            {
                GUILayout.Space( 3 );

                EditorGUILayout.BeginHorizontal();
                {
                    var fold = EditorGUILayout.Foldout( Fold, new GUIContent( "Export Setting" ) );
                    if( fold != Fold ) {
                        EditorGUIUtility.keyboardControl = 0;
                        UndoHelper.BuilderDataUndo( "Change fold" );
                    }
                    Fold = fold;
                    GUILayout.FlexibleSpace();
                    if( GUILayout.Button( IsConfigAvailable ? GUIHelper.Textures.ConfigAvailable : GUIHelper.Textures.ConfigInvalid, GUIStyle.none, new GUILayoutOption[] { GUILayout.Width( 20 ), GUILayout.Height( 20 ) } ) ) {
                        CheckConfig();
                    }
                }
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
                {

                    GUILayout.Space( 17 );

                    EditorGUILayout.BeginVertical();
                    {
                        if( Fold ) {
                            Undo.IncrementCurrentGroup();
                            UndoHelper.BuilderDataUndo( "Change CachePath" );
                            using( new GUIHelper.Scope.DropSettingTextArea( new GUIContent( "★AssetBundle CachePath", "used for exporting to ExportAssetBundlePath." ), ref AssetBundleCachePath ) ) { };
                            if( GUILayout.Button( "Open CachePath" ) )
                                EditorUtility.OpenWithDefaultApp( AssetBundleCachePath );

                            GUILayout.Space( 5 );

                            EditorGUILayout.BeginHorizontal();
                            {
                                Undo.IncrementCurrentGroup();
                                UndoHelper.BuilderDataUndo( "Change export with Manifest" );
                                GUILayout.Label( new GUIContent( "Export with Manifest", "used for exporting to ExportAssetBundlePath." ) );
                                ExportWithManifest = EditorGUILayout.Toggle( ExportWithManifest );
                            }
                            EditorGUILayout.EndHorizontal();

                            Undo.IncrementCurrentGroup();
                            UndoHelper.BuilderDataUndo( "Change ExportPath" );
                            using( new GUIHelper.Scope.DropSettingTextArea( new GUIContent( "★Export AssetBundle Path", "finally export assetbundles for this path." ), ref ExportAssetBundlePath ) ) { };
                            if( GUILayout.Button( "Open ExportPath" ) )
                                EditorUtility.OpenWithDefaultApp( ExportAssetBundlePath );

                            Undo.IncrementCurrentGroup();
                            UndoHelper.BuilderDataUndo( "Change ExportType" );
                            GUILayout.Label( "★Export Type" );
                            ExportType = ( ExportType )EditorGUILayout.EnumPopup( ExportType );

                            GUILayout.Space( 2 );
                        }
                    }
                    EditorGUILayout.EndVertical();
                }
                EditorGUILayout.EndHorizontal();

                GUILayout.Space( 5 );

            }
            EditorGUILayout.EndVertical();
        }

        //=======================================================
        // Utility
        //=======================================================

        internal bool IsConfigAvailable {
            get {
                var phase1 = !string.IsNullOrEmpty( AssetBundleCachePath );
                var phase2 = !string.IsNullOrEmpty( ExportAssetBundlePath );
                return phase1 && phase2;
            }
        }

        private void CheckConfig() {
            var message = "";
            if( string.IsNullOrEmpty( AssetBundleCachePath ) )
                message += "You must set up AssetBundleCachePath.\n";
            if( string.IsNullOrEmpty( ExportAssetBundlePath ) )
                message += "You must set up ExportAssetBundlePath.\n";

            if( !string.IsNullOrEmpty( message ) )
                EditorUtility.DisplayDialog( "Export Config Error", message, "ok" );
        }

        #endregion

    }
}