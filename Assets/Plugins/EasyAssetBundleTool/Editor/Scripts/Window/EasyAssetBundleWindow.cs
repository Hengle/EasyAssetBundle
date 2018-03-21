using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEditor.IMGUI.Controls;

using SplitterGUI  = charcolle.Utility.EasyAssetBundle.v1.SplitterGUI;
using WindowHelper = charcolle.Utility.EasyAssetBundle.v1.WindowHelper;
using GUIHelper    = charcolle.Utility.EasyAssetBundle.v1.GUIHelper;
using UndoHelper   = charcolle.Utility.EasyAssetBundle.v1.UndoHelper;

namespace charcolle.Utility.EasyAssetBundle.v1 {

    internal class EasyAssetBundleWindow : EditorWindow {

        [SerializeField]
        internal static EasyAssetBundleWindow win;

        [ MenuItem( "Window/EasyAssetBundle" ) ]
        static void Open() {
            win = GetWindow<EasyAssetBundleWindow>();
            win.titleContent.text = WindowHelper.WIN_TITLE;

            Initialize();
            Undo.undoRedoPerformed -= Initialize;
            Undo.undoRedoPerformed += Initialize;
        }

        internal static void Initialize() {
            UndoHelper.Initialize();
            WindowHelper.Initialize( win.position );
            win.TreeViewInitialize();
        }

        private void TreeViewInitialize() {
            if( SelectedData == null || SelectedData.BuildAssets == null || SelectedData.BuildAssets.Count == 0 || !SelectedData.Config.IsConfigAvailable ) {
                treeView = null;
                return;
            }

            SelectedData.Initialize();
            CurrentConfig = SelectedData.CurrentConfig;
            if( CurrentConfig == null ) {
                treeView = null;
                return;
            }

            if( treeViewState == null )
                treeViewState = new TreeViewState();

            var headerState = EasyAssetBundleBuildAssetView.CreateDefaultMultiColumnHeaderState( multiColumnTreeViewRect.width, IsUseAssetBundleList );
            if( MultiColumnHeaderState.CanOverwriteSerializedFields( MultiColumnHeaderState, headerState ) )
                MultiColumnHeaderState.OverwriteSerializedFields( MultiColumnHeaderState, headerState );
            MultiColumnHeaderState = headerState;

            var multiColumnHeader = new EasyAssetBundleMultiColumnHeader( headerState );

            var treeModel = new TreeModel<EditorAssetInfo>( SelectedData.BuildAssets );

            treeView = new EasyAssetBundleBuildAssetView( treeViewState, multiColumnHeader, treeModel );
            treeView.Initialize( ( IAssetBundleBuildConfig )CurrentConfig, CurrentConfig.AssetBundleBuildRootPath );
            treeView.ExpandAll();
            multiColumnHeader.ResizeToFit();
        }

        private void OnDisable() {
            UndoHelper.OnDisable();
        }

        private void OnGUI() {
            if( win == null )
                Open();

            EditorGUI.BeginChangeCheck();
            WindowHelper.OnGUIFirst();

            Draw();

            WindowHelper.OnGUIEnd();
            if( EditorGUI.EndChangeCheck() && SelectedData != null ) {
                EditorUtility.SetDirty( SelectedData );
            }
        }

        #region drawer

        //=======================================================
        // drawer
        //=======================================================
        [SerializeField]
        private TreeViewState treeViewState;
        [SerializeField]
        private MultiColumnHeaderState MultiColumnHeaderState;
        private EasyAssetBundleBuildAssetView treeView;

        private void Draw() {

            SplitterGUI.BeginHorizontalSplit( WindowHelper.HorizontalStateMain );
            {
                LeftArea();
                RightArea();
            }
            SplitterGUI.EndHorizontalSplit();
        }

        #region left area
        [SerializeField]
        private int selectedBuilderDataIdx = 0;
        [SerializeField]
        private int selectedBuilderMenuIdx = 0;
        [SerializeField]
        private EasyAssetBundlePopupWindow popupWin;
        private void LeftArea() {
            EditorGUILayout.BeginVertical( GUIHelper.Styles.NoSpace );
            {
                HeaderLeft();
                if( WindowHelper.BuilderDataExists ) {
                    switch( selectedBuilderMenuIdx ) {
                        case 0:
                            BuilderMenu();
                            break;
                        case 1:
                            LabelConfig();
                            break;
                    }
                    FooterLeft();
                } else {
                    EditorGUILayout.HelpBox( "No builder data.", MessageType.Warning );
                    selectedBuilderDataIdx = 0;
                }
            }
            EditorGUILayout.EndVertical();
        }

        private void HeaderLeft() {
            EditorGUILayout.BeginVertical();
            {
                if( WindowHelper.BuilderDataExists ) {
                    EditorGUILayout.BeginHorizontal();
                    {
                        var selected = EditorGUILayout.Popup( selectedBuilderDataIdx, WindowHelper.BuilderDataList, EditorStyles.toolbarPopup );
                        if( selected != selectedBuilderDataIdx ) {
                            UndoHelper.MainWindowUndo( "Change BuilderData selection" );
                            selectedBuilderDataIdx = selected;
                            selectedBuilderMenuIdx = 0;
                            EditorGUIUtility.keyboardControl = 0;
                            currentConfig = null;
                            TreeViewInitialize();
                        }

                        GUI.backgroundColor = Color.green;
                        if( GUILayout.Button( "+", EditorStyles.toolbarButton, GUILayout.Width( 40 ) ) ) {
                            popupWin = new EasyAssetBundlePopupWindow();
                            popupWin.Initialize();
                            PopupWindow.Show( Rect.zero, popupWin );
                        }
                        GUI.backgroundColor = Color.white;
                    }
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.BeginHorizontal();
                    {
                        var selected = GUILayout.Toolbar( selectedBuilderMenuIdx, WindowHelper.MENU_BUILDERMENU, EditorStyles.toolbarButton );
                        if( selected != selectedBuilderMenuIdx ) {
                            UndoHelper.MainWindowUndo( "Change Menu selection" );
                            EditorGUIUtility.keyboardControl = 0;
                        }
                        selectedBuilderMenuIdx = selected;
                    }
                    EditorGUILayout.EndHorizontal();
                } else {
                    EditorGUILayout.BeginHorizontal();
                    {
                        GUILayout.Label( "no assetbundle builder" );
                        GUILayout.FlexibleSpace();
                        GUI.backgroundColor = Color.green;
                        if( GUILayout.Button( "+", EditorStyles.toolbarButton, GUILayout.Width( 40 ) ) ) {
                            popupWin = new EasyAssetBundlePopupWindow();
                            popupWin.Initialize();
                            PopupWindow.Show( Rect.zero, popupWin );
                        }
                        GUI.backgroundColor = Color.white;
                    }
                    EditorGUILayout.EndHorizontal();
                }
            }
            EditorGUILayout.EndVertical();
        }

        private Vector2 BuildeMenuScroll;
        private void BuilderMenu() {
            BuildeMenuScroll = EditorGUILayout.BeginScrollView( BuildeMenuScroll );
            {
                SelectedData.Config.OnGUI();
            }
            EditorGUILayout.EndScrollView();
        }

        private Vector2 LabelConfigScroll;
        private void LabelConfig() {
            LabelConfigScroll = EditorGUILayout.BeginScrollView( LabelConfigScroll );
            {
                SelectedData.Config.AssetLabelConfig.OnGUI();
            }
            EditorGUILayout.EndScrollView();
        }

        private void FooterLeft() {
            GUILayout.FlexibleSpace();

            EditorGUILayout.BeginHorizontal();
            {
                GUI.backgroundColor = Color.cyan;
                if( GUILayout.Button( "Apply Setting", GUILayout.Height( 30 ) ) ) {
                    UndoHelper.BuilderDataUndo( "Apply Config" );
                    CurrentConfig = SelectedData.CurrentConfig;
                    SelectedData.ApplyConfig();
                    SelectedData.ApplyVersionListConfig();
                    if( CurrentConfig != null )
                        TreeViewInitialize();
                }
                GUI.backgroundColor = Color.white;
                GUI.backgroundColor = Color.yellow;
                if( GUILayout.Button( "Load Assets", GUILayout.Height( 30 ) ) ) {
                    UndoHelper.BuilderDataUndo( "Load Assets" );
                    CurrentConfig = SelectedData.CurrentConfig;
                    if( CurrentConfig != null ) {
                        SelectedData.BuildAssets = TreeHelper.GetFileTreeViewItems( CurrentConfig.AssetBundleBuildRootPath );
                        SelectedData.ApplyConfig();
                        SelectedData.LoadNewestAssetBundleList();
                        SelectedData.ApplyVersionListConfig();
                        TreeViewInitialize();
                    }
                }
                GUI.backgroundColor = Color.white;
            }
            EditorGUILayout.EndHorizontal();
        }

        #endregion

        #region right area

        private void RightArea() {
            if( !WindowHelper.BuilderDataExists ) {
                EditorGUILayout.BeginVertical();
                EditorGUILayout.HelpBox( "No BuilderData.", MessageType.Warning );
                EditorGUILayout.EndVertical();
                return;
            }

            EditorGUILayout.BeginVertical( GUIHelper.Styles.NoSpaceBox );
            {
                if( treeView != null ) {
                    EditorGUILayout.BeginHorizontal( EditorStyles.toolbar );
                    {
                        GUILayout.FlexibleSpace();
                        if( IsUseAssetBundleList ) {
                            if( GUILayout.Button( "Open AssetList", EditorStyles.toolbarButton, GUILayout.Width( 90 ) ) ) {
                                UndoHelper.BuilderDataUndo( "Apply Config" );
                                SelectedData.LoadNewestAssetBundleList();
                                SelectedData.ApplyVersionListConfig();
                                EasyAssetBundleAssetListWindow.Open();
                            }
                            GUILayout.Space( 5 );
                        }
                        var buildAssetMenu = EditorGUILayout.Popup( 0, WindowHelper.MENU_BUILDERASSETMENU, EditorStyles.toolbarPopup, GUILayout.Width( 90 ) );
                        switch( buildAssetMenu ) {
                            case 2:
                                UndoHelper.BuilderDataUndo( "Select files" );
                                for( int i = 0; i < SelectedData.BuildAssets.Count; i++ )
                                    SelectedData.BuildAssets[ i ].IsBuild = SelectedData.BuildAssets[ i ].IsAsset;
                                break;
                            case 3:
                                UndoHelper.BuilderDataUndo( "Select DirrefentUnityVersion" );
                                if( !IsUseAssetBundleList )
                                    return;
                                for( int i = 0; i < SelectedData.BuildAssets.Count; i++ )
                                    SelectedData.BuildAssets[ i ].IsBuild = !SelectedData.BuildAssets[ i ].UnityVersion.Equals( Application.unityVersion );
                                break;
                        }
                    }
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginVertical();
                    GUILayout.FlexibleSpace();
                    EditorGUILayout.EndVertical();

                    treeView.OnGUI( GUILayoutUtility.GetLastRect() );

                    EditorGUILayout.BeginHorizontal( EditorStyles.toolbar );
                    {
                        if( GUILayout.Button( "Disable All", EditorStyles.toolbarButton, GUILayout.Width( 70 ) ) ) {
                            UndoHelper.BuilderDataUndo( "Disable all" );
                            for( int i = 0; i < SelectedData.BuildAssets.Count; i++ )
                                SelectedData.BuildAssets[ i ].IsBuild = false;
                        }
                        if( GUILayout.Button( "Expand All", EditorStyles.toolbarButton, GUILayout.Width( 70 ) ) ) {
                            treeView.ExpandAll();
                        }
                        GUILayout.FlexibleSpace();
                        if( IsUseAssetBundleList ) {
                            var listName = EditorGUILayout.TextField( SelectedData.CurrentAssetBundleListName, EditorStyles.toolbarTextField, GUILayout.Width( 200 ) );
                            if( listName != SelectedData.CurrentAssetBundleListName ) {
                                UndoHelper.BuilderDataUndo( "Edit AssetList name" );
                                SelectedData.CurrentAssetBundleListName = listName;
                            }
                        }
                    }
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal();
                    {
                        EditorGUILayout.BeginVertical( EditorStyles.helpBox, GUILayout.ExpandWidth( true ) );
                        {
                            GUILayout.Label( string.Format( "Build: {0}", SelectedData.BuildAssets.Count( a => a.IsBuild ) ) );
                            GUILayout.Label( string.Format( "InValid: {0}", SelectedData.BuildAssets.Count( a => !a.IsAvailable ) ) );
                        }
                        EditorGUILayout.EndVertical();

                        GUI.backgroundColor = Color.green;
                        if( GUILayout.Button( "Build", new GUILayoutOption[] { GUILayout.Width( 200 ), GUILayout.ExpandHeight( true ) } ) ) {
                            var processor = new EasyAssetBundleBuildProcessor( SelectedData );
                            processor.OnBuildProcessEnd += () => {
                                if( IsUseAssetBundleList ) {
                                    SelectedData.ApplyConfig();
                                    SelectedData.LoadNewestAssetBundleList();
                                    SelectedData.ApplyVersionListConfig();
                                    TreeViewInitialize();
                                    EasyAssetBundleAssetListWindow.Open();
                                }
                            };
                            processor.Build();
                        }
                        GUI.backgroundColor = Color.white;
                    }
                    EditorGUILayout.EndHorizontal();
                } else {
                    EditorGUILayout.HelpBox( "set build config.", MessageType.Warning );
                    GUILayout.FlexibleSpace();
                }

            }
            EditorGUILayout.EndVertical();
        }

        #endregion

        //=======================================================
        // Utility
        //=======================================================

        internal EasyAssetBundleBuilderData SelectedData {
            get {
                if( WindowHelper.Data == null || WindowHelper.Data.Count == 0 || selectedBuilderDataIdx >= WindowHelper.Data.Count ) {
                    selectedBuilderDataIdx = 0;
                    return null;
                }
                return WindowHelper.Data[ selectedBuilderDataIdx ];
            }
        }

        [SerializeField]
        private EasyAssetBundleBuildConfig currentConfig;
        internal EasyAssetBundleBuildConfig CurrentConfig {
            get {
                return currentConfig;
            }
            set {
                currentConfig = value;
            }
        }

        private bool IsUseAssetBundleList {
            get {
                if( currentConfig == null )
                    return false;
                return currentConfig.IsUseAssetBundleList;
            }
        }

        private Rect multiColumnTreeViewRect {
            get { return new Rect( 0, 30, position.width - 40, position.height - 60 ); }
        }

        #endregion

    }

}