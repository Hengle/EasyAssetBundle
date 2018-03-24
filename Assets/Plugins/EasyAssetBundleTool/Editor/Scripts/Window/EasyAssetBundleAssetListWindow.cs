using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEditor.IMGUI.Controls;

using SplitterGUI  = charcolle.Utility.EasyAssetBundle.v1.SplitterGUI;
using WindowHelper = charcolle.Utility.EasyAssetBundle.v1.WindowHelper;
using GUIHelper    = charcolle.Utility.EasyAssetBundle.v1.GUIHelper;
using UndoHelper   = charcolle.Utility.EasyAssetBundle.v1.UndoHelper;

namespace charcolle.Utility.EasyAssetBundle.v1 {

    internal class EasyAssetBundleAssetListWindow : EditorWindow {

        [SerializeField]
        internal static EasyAssetBundleAssetListWindow win;

        internal static void Open() {
            win = GetWindow<EasyAssetBundleAssetListWindow>( true );
            win.titleContent.text = "AssetBundleList";

            Initialize();
            Undo.undoRedoPerformed -= Initialize;
            Undo.undoRedoPerformed += Initialize;
        }

        static void Initialize() {
            win.TreeViewInitialize();
        }

        private void TreeViewInitialize() {
            if( EasyAssetBundleWindow.win == null || EasyAssetBundleWindow.win.SelectedData == null || EasyAssetBundleWindow.win.CurrentConfig == null )
                return;

            if( treeViewState == null )
                treeViewState = new TreeViewState();

            var headerState = EasyAssetBundleAssetListView.CreateDefaultMultiColumnHeaderState( multiColumnTreeViewRect.width );
            if( MultiColumnHeaderState.CanOverwriteSerializedFields( MultiColumnHeaderState, headerState ) )
                MultiColumnHeaderState.OverwriteSerializedFields( MultiColumnHeaderState, headerState );
            MultiColumnHeaderState = headerState;

            var multiColumnHeader = new EasyAssetBundleMultiColumnHeader( headerState );
            multiColumnHeader.ResizeToFit();

            var treeModel = new TreeModel<EditorAssetInfo>( EasyAssetBundleWindow.win.SelectedData.VersionAssets );

            treeView = new EasyAssetBundleAssetListView( treeViewState, multiColumnHeader, treeModel );
            treeView.Initialize( (IAssetBundleBuildConfig)EasyAssetBundleWindow.win.CurrentConfig, EasyAssetBundleWindow.win.CurrentConfig.AssetBundleBuildRootPath );
            treeView.showControls = false;
            treeView.ExpandAll();

            SaveAssetBundleListName = EasyAssetBundleWindow.win.SelectedData.CurrentAssetBundleListName;
            allFileSize = EasyAssetBundleWindow.win.SelectedData.VersionAssets.Sum( a => a.Size );
            allFileNum = EasyAssetBundleWindow.win.SelectedData.VersionAssets.Count - 1;
        }

        private void OnGUI() {
            if( EasyAssetBundleWindow.win == null )
                Close();
            if( win == null )
                Open();
            if( treeView == null )
                TreeViewInitialize();

            EditorGUI.BeginChangeCheck();
            WindowHelper.OnGUIFirst();

            Draw();

            WindowHelper.OnGUIEnd();
        }

        #region drawer
        //=======================================================
        // drawer
        //=======================================================
        [SerializeField]
        private TreeViewState treeViewState;
        [SerializeField]
        private MultiColumnHeaderState MultiColumnHeaderState;
        private EasyAssetBundleAssetListView treeView;
        private int AssetBundleFileNum;

        private float allFileSize;
        private int allFileNum;
        private void Draw() {
            if( EasyAssetBundleWindow.win == null || EasyAssetBundleWindow.win.SelectedData == null )
                return;

            EditorGUILayout.BeginVertical();
            {
                Header();
                TreeView();
            }
            EditorGUILayout.EndVertical();
        }

        private string SearchText;
        private string SaveAssetBundleListName;
        private void Header() {
            var data = EasyAssetBundleWindow.win.SelectedData;

            EditorGUILayout.BeginVertical();
            {
                EditorGUILayout.BeginHorizontal( EditorStyles.toolbar, GUILayout.ExpandWidth( true ) );
                {
                    GUILayout.Label( "<b>" + data.CurrentAssetBundleListName + "</b>" );
                }
                EditorGUILayout.EndHorizontal();

                GUILayout.Space( 5 );

                EditorGUILayout.BeginVertical( EditorStyles.helpBox );
                {
                    GUILayout.Label( string.Format( "FileNum: {0}", allFileNum ) );
                    GUILayout.Label( string.Format( "FileSize: {0}", allFileSize ) );
                    EditorGUILayout.BeginHorizontal();
                    {

                    }
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.EndVertical();

                GUILayout.Space( 5 );

                EditorGUILayout.BeginHorizontal( EditorStyles.toolbar, GUILayout.ExpandWidth( true ) );
                {
                    Undo.IncrementCurrentGroup();
                    UndoHelper.AssetListWindowUndo( "Edit AssetList search" );
                    SearchText = EditorGUILayout.TextField( SearchText, GUIHelper.Styles.SearchFieldToolBar, GUILayout.Width( 200 ) );
                    if( GUILayout.Button( "", GUIHelper.Styles.SearchFieldCancelToolBar ) ) {
                        SearchText = "";
                    }
                    GUILayout.FlexibleSpace();

                    treeView.showControls = GUILayout.Toggle( treeView.showControls, "Edit", EditorStyles.toolbarButton, GUILayout.Width( 50 ) );
                    var selectedMenu = EditorGUILayout.Popup( 0, WindowHelper.MENU_ASSETLISTMENU, EditorStyles.toolbarPopup, GUILayout.Width( 70 ) );
                    if( selectedMenu != 0 ) {
                        switch( selectedMenu ) {
                            case 2:
                                UndoHelper.BuilderDataUndo( "Remove checked asset" );
                                data.VersionAssets.RemoveAll( a => a.IsBuild );
                                break;
                            case 3:
                                UndoHelper.BuilderDataUndo( "Remove invalid asset" );
                                data.VersionAssets.RemoveAll( a => !a.IsAvailable );
                                break;
                            case 4:
                                for( int i = 0; i < data.VersionAssets.Count; i++ )
                                    data.VersionAssets[ i ].Version = 1;
                                break;
                            case 5:
                                Debug.Log( "5" );
                                break;
                        }
                        TreeViewInitialize();
                    }
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();
        }

        private void TreeView() {
            var data   = EasyAssetBundleWindow.win.SelectedData;
            var config = data.Config;
            EditorGUILayout.BeginVertical();
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndVertical();

            treeView.searchString = SearchText;
            treeView.OnGUI( GUILayoutUtility.GetLastRect() );
            EditorGUILayout.BeginVertical();
            {
                EditorGUILayout.BeginHorizontal( EditorStyles.toolbar, GUILayout.ExpandWidth( true ) );
                {
                    if( GUILayout.Button( "Check Selection", EditorStyles.toolbarButton, GUILayout.Width( 90 ) ) ) {
                        UndoHelper.BuilderDataUndo( "Check Selection" );
                        for( int i = 0; i < treeView.state.selectedIDs.Count; i++ )
                            data.VersionAssets[ treeView.state.selectedIDs[ i ] ].IsBuild = true;
                    }
                    if( GUILayout.Button( "UnCheck All", EditorStyles.toolbarButton, GUILayout.Width( 75 ) ) ) {
                        UndoHelper.BuilderDataUndo( "Uncheck all AssetList" );
                        for( int i = 0; i < data.VersionAssets.Count; i++ )
                            data.VersionAssets[ i ].IsBuild = false;
                    }
                    GUILayout.FlexibleSpace();
                    Undo.IncrementCurrentGroup();
                    UndoHelper.AssetListWindowUndo( "Edit AssetList name" );
                    SaveAssetBundleListName = EditorGUILayout.TextField( SaveAssetBundleListName, EditorStyles.toolbarTextField, GUILayout.Width( 150 ) );
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                {
                    GUILayout.FlexibleSpace();
                    if( GUILayout.Button( "Convert", new GUILayoutOption[] { GUILayout.Width( 150 ), GUILayout.Height( 25 ) } ) ) {
                        var convertPath = string.IsNullOrEmpty( config.AssetBundleListConfig.AssetBundleListConvertPath ) ? Application.dataPath : config.AssetBundleListConfig.AssetBundleListConvertPath;
                        var savePath = EditorUtility.SaveFilePanel( "Convert AssetBundleList", convertPath, "ConvertFile", config.AssetBundleListConfig.AssetBundleListConvertExtension );
                        if( !string.IsNullOrEmpty( savePath ) ) {
                            savePath = FileHelper.SystemPathToAssetPath( savePath );
                            data.ConvertAssetBundleList( data.VersionAssets, savePath );
                        }
                    }
                    GUI.backgroundColor = Color.yellow;
                    if( GUILayout.Button( "Save", new GUILayoutOption[] { GUILayout.Width( 150 ), GUILayout.Height( 25 ) } ) ) {
                        data.CurrentAssetBundleListName = SaveAssetBundleListName;
                        var processor = new EasyAssetBundleBuildProcessor( data, BuildProgress.SaveVersionFile, BuildProgress.SaveVersionFile );
                        processor.OnBuildProcessEnd += () => {
                            data.LoadNewestAssetBundleList();
                            data.ApplyVersionListConfig();
                        };
                        processor.Build( "AssetBundleList Save Complete" );
                    }
                    GUI.backgroundColor = Color.white;
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();
        }

        Rect multiColumnTreeViewRect {
            get { return new Rect( 0, 30, position.width - 40, position.height - 60 ); }
        }
        #endregion

    }

}