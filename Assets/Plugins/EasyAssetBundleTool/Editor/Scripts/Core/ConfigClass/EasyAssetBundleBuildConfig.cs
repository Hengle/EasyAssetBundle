using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using BuildTargetUtility = charcolle.Utility.EasyAssetBundle.v1.BuildTargetUtility;
using GUIHelper          = charcolle.Utility.EasyAssetBundle.v1.GUIHelper;
using FileHelper         = charcolle.Utility.EasyAssetBundle.v1.FileHelper;
using UndoHelper = charcolle.Utility.EasyAssetBundle.v1.UndoHelper;

/// <summary>
/// http://lv100hp9999.com/wp1/2016/07/06/unity%E8%A4%87%E6%95%B0%E9%81%B8%E6%8A%9E%E3%83%81%E3%82%A7%E3%83%83%E3%82%AF%E3%83%AA%E3%82%B9%E3%83%88%E3%81%AEeditor%E3%82%B9%E3%82%AF%E3%83%AA%E3%83%97%E3%83%88%E3%81%AE%E3%82%B5%E3%83%B3/
/// </summary>
namespace charcolle.Utility.EasyAssetBundle.v1 {

    [Serializable]
    public class EasyAssetBundleBuildConfig : IAssetBundleBuildConfig {

        [SerializeField]
        internal EasyAssetBundleLabelConfig AssetLabelConfig;
        [SerializeField]
        internal EasyAssetBundleExportConfig ExportConfig;
        [SerializeField]
        internal EasyAssetBundleListConfig AssetBundleListConfig;
        [SerializeField]
        internal EasyAssetBundleUploadConfig UploadConfig;

        [SerializeField]
        internal BuildAssetBundleOptions Options;
        [SerializeField]
        internal string AssetBundleBuildRootPath;
        [SerializeField]
        internal int Platform;
        [SerializeField]
        internal string ScriptName;

        internal EasyAssetBundleBuildConfig ( string scriptName ) {
            Platform              = 0;
            ExportConfig          = new EasyAssetBundleExportConfig();
            UploadConfig          = new EasyAssetBundleUploadConfig();
            AssetLabelConfig      = new EasyAssetBundleLabelConfig();
            AssetBundleListConfig = new EasyAssetBundleListConfig();
            ScriptName            = scriptName;
        }

        internal EasyAssetBundleBuildConfig( EasyAssetBundleBuildConfig copy ) {
            AssetLabelConfig         = new EasyAssetBundleLabelConfig( copy.AssetLabelConfig );
            ExportConfig             = new EasyAssetBundleExportConfig( copy.ExportConfig );
            AssetBundleListConfig    = new EasyAssetBundleListConfig( copy.AssetBundleListConfig );
            UploadConfig             = copy.UploadConfig;
            Options                  = copy.Options;
            AssetBundleBuildRootPath = copy.AssetBundleBuildRootPath;
            Platform                 = copy.Platform;

            AssetLabelConfig.Initialize( AssetBundleBuildRootPath );
        }

        internal void Initialize() {
            ExportConfig.Initialize();
            UploadConfig.Initialize();
            AssetLabelConfig.Initialize( AssetBundleBuildRootPath );
            AssetBundleListConfig.Initialize();
        }

        #region interface
        //=======================================================
        // interface
        //=======================================================

        public bool IsConfigAvailable {
            get {
                return !string.IsNullOrEmpty( AssetBundleBuildRootPath ) && ExportConfig.IsConfigAvailable;
            }
        }

        public bool IsUseAssetBundleList {
            get {
                return AssetBundleListConfig.IsConfigAvailable;
            }
        }

        public string GetAssetBundleListSavePath( string fileName ) {
            return FileHelper.pathSlashFix( Path.Combine( AssetBundleListConfig.AssetBundleListTextPath, fileName ) );
        }

        public string GetAssetLabel( EditorAssetInfo asset, string buildRootPath ) {
            var buildPath = FileHelper.AssetPathToBuildPath( buildRootPath, asset.AssetPath );
            if( string.IsNullOrEmpty( buildPath ) )
                return null;

            return AssetLabelConfig.GetAssetLabel( asset.AssetPath, asset.Extension, buildPath, asset.Guid );
        }

        public bool CheckDirectoryLabel( EditorAssetInfo asset, string buildRootPath ) {
            var buildPath = FileHelper.AssetPathToBuildPath( buildRootPath, asset.AssetPath );
            if( string.IsNullOrEmpty( buildPath ) )
                return false;

            return AssetLabelConfig.CheckDirectoryLabel( asset.AssetPath, buildPath );
        }

        public bool CheckFileAvailable( EditorAssetInfo asset, string buildRootPath ) {
            var buildPath = FileHelper.AssetPathToBuildPath( buildRootPath, asset.AssetPath );
            if( string.IsNullOrEmpty( buildPath ) )
                return false;

            return AssetLabelConfig.CheckFileAvailable( asset.AssetPath, asset.Name, asset.Extension, buildPath );
        }

        public void OnGUI() {
            Drawer();
        }

        #endregion

        #region drawer
        //=======================================================
        // drawer
        //=======================================================

        [SerializeField]
        private bool Fold = true;
        internal void Drawer() {
            GUILayout.Space( 10 );

            EditorGUILayout.BeginVertical();
            {
                if( GUILayout.Button( new GUIContent( ScriptName.ToBold(), GUIHelper.Textures.ScriptIcon ), GUILayout.Height( 20 ) ) )
                    FileHelper.OpenInEditor( ScriptName );
            }
            EditorGUILayout.EndVertical();

            GUILayout.Space( 5 );

            var rect = EditorGUILayout.BeginVertical( EditorStyles.helpBox );
            {
                GUILayout.Space( 3 );

                EditorGUILayout.BeginHorizontal();
                {
                    var fold = EditorGUILayout.Foldout( Fold, "Basic Setting" );
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
                            UndoHelper.BuilderDataUndo( "Change BuildRootPath" );
                            using( new GUIHelper.Scope.DropSettingTextAreaInProject( new GUIContent( "★Root Path of Build AssetBundle", "This use for naming assetbundles" ), ref AssetBundleBuildRootPath ) ) { };
                            AssetLabelConfig.BuildRootPath = AssetBundleBuildRootPath;

                            GUILayout.Space( 3 );

                            Undo.IncrementCurrentGroup();
                            UndoHelper.BuilderDataUndo( "Change BuildOption" );
                            GUILayout.Label( new GUIContent( "★AssetBundle Build Options", "AssetBundle BuildOption" ) );
                            Options = ( BuildAssetBundleOptions )EditorGUILayout.EnumFlagsField( Options );

                            GUILayout.Space( 3 );

                            Undo.IncrementCurrentGroup();
                            UndoHelper.BuilderDataUndo( "Change BuildPlatform" );
                            GUILayout.Label( new GUIContent( "★Build Platform", "Build AssetBundle for selected platform." ) );
                            Platform = EditorGUILayout.MaskField( Platform, BuildTargetUtility.GetBuildModulesMenu() );
                        }

                        GUILayout.Space( 2 );
                    }
                    EditorGUILayout.EndVertical();
                }
                EditorGUILayout.EndHorizontal();

                GUILayout.Space( 5 );

            }
            EditorGUILayout.EndVertical();

            ExportConfig.OnGUI();
            AssetBundleListConfig.OnGUI();
            UploadConfig.OnGUI();

            if( IsContextClick( Event.current, rect ) ) {
                var menu = new GenericMenu();
                menu.AddItem( new GUIContent( "Clear Basic Settings" ), false, () => {
                    AssetBundleBuildRootPath = "";
                } );
                menu.ShowAsContext();
            }
        }

        //=======================================================
        // Utility
        //=======================================================

        internal void CheckConfig() {

        }

        internal bool IsContextClick( Event e, Rect rect ) {
            switch( e.type ) {
                case EventType.ContextClick:
                    if( rect.Contains( e.mousePosition ) ) {
                        e.Use();
                        return true;
                    }
                    break;
            }
            return false;
        }

        #endregion

    }

}