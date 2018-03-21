using System;
using UnityEngine;
using UnityEditor;

using GUIHelper  = charcolle.Utility.EasyAssetBundle.v1.GUIHelper;
using UndoHelper = charcolle.Utility.EasyAssetBundle.v1.UndoHelper;

namespace charcolle.Utility.EasyAssetBundle.v1 {

    [Serializable]
    internal class EasyAssetBundleListConfig {

        [SerializeField]
        internal bool UseAssetBundleList;
        [SerializeField]
        internal string AssetBundleListTextPath;
        [SerializeField]
        internal string AssetBundleListConvertPath;
        [SerializeField]
        internal string AssetBundleListConvertExtension;

        [SerializeField]
        internal string CurrentAssetBundleListName;

        internal EasyAssetBundleListConfig() {
            AssetBundleListConvertExtension = "asset";
        }

        internal EasyAssetBundleListConfig( EasyAssetBundleListConfig copy ) {
            UseAssetBundleList         = copy.UseAssetBundleList;
            AssetBundleListTextPath    = copy.AssetBundleListTextPath;
            AssetBundleListConvertPath = copy.AssetBundleListConvertPath;
        }

        internal void Initialize() { }

        internal bool IsConfigAvailable {
            get {
                return UseAssetBundleList && !string.IsNullOrEmpty( AssetBundleListTextPath );
            }
        }

        #region drawer
        //=======================================================
        // drawer
        //=======================================================

        [SerializeField]
        private bool Fold;
        internal void OnGUI() {
            GUILayout.Space( 5 );

            EditorGUILayout.BeginVertical( EditorStyles.helpBox );
            {
                GUILayout.Space( 3 );

                EditorGUILayout.BeginHorizontal();
                {
                    var fold = EditorGUILayout.Foldout( Fold, "AssetBundleList Setting" );
                    if( fold != Fold ) {
                        EditorGUIUtility.keyboardControl = 0;
                        UndoHelper.BuilderDataUndo( "Change fold" );
                    }
                    Fold = fold;
                    GUILayout.FlexibleSpace();
                    if( GUILayout.Button( IsConfigAvailable ? GUIHelper.Textures.ConfigAvailable : GUIHelper.Textures.ConfigWarning, GUIStyle.none, new GUILayoutOption[] { GUILayout.Width( 20 ), GUILayout.Height( 20 ) } ) ) {
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
                            EditorGUILayout.BeginHorizontal();
                            {
                                Undo.IncrementCurrentGroup();
                                UndoHelper.BuilderDataUndo( "Change use AssetBundleList" );
                                GUILayout.Label( new GUIContent( "Use AssetBundleList", "used for exporting to ExportAssetBundlePath." ) );
                                UseAssetBundleList = EditorGUILayout.Toggle( UseAssetBundleList );
                            }
                            EditorGUILayout.EndHorizontal();

                            if( UseAssetBundleList ) {
                                GUILayout.Space( 10 );

                                Undo.IncrementCurrentGroup();
                                UndoHelper.BuilderDataUndo( "Change AssetBundleList save path" );
                                using( new GUIHelper.Scope.DropSettingTextArea( new GUIContent( "★AssetBundleList SavePath", "used for loading asset-list." ), ref AssetBundleListTextPath ) ) { };
                                if( GUILayout.Button( "Open AssetList Folder" ) )
                                    EditorUtility.OpenWithDefaultApp( AssetBundleListTextPath );

                                GUILayout.Space( 10 );

                                Undo.IncrementCurrentGroup();
                                UndoHelper.BuilderDataUndo( "Change ConvertFile save path" );
                                using( new GUIHelper.Scope.DropSettingTextAreaInProject( new GUIContent( "ConvertFile SavePath", "Convert file will created at the path." ), ref AssetBundleListConvertPath ) ) { };

                                Undo.IncrementCurrentGroup();
                                UndoHelper.BuilderDataUndo( "Change ConvertFileExtension" );
                                GUILayout.Label( new GUIContent( "ConvertFile Extension", "Default extension of convert file." ) );
                                AssetBundleListConvertExtension = EditorGUILayout.TextField( AssetBundleListConvertExtension );

                            }
                        }

                        GUILayout.Space( 2 );

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

        private void CheckConfig() {
            Debug.Log( "Configのチェック" );
        }

        #endregion

    }

}