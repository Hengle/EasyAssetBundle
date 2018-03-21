using System;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEditor;

using BuildTargetUtility = charcolle.Utility.EasyAssetBundle.v1.BuildTargetUtility;
using GUIHelper          = charcolle.Utility.EasyAssetBundle.v1.GUIHelper;
using FileHelper         = charcolle.Utility.EasyAssetBundle.v1.FileHelper;
using UndoHelper         = charcolle.Utility.EasyAssetBundle.v1.UndoHelper;

namespace charcolle.Utility.EasyAssetBundle.v1 {

    [Serializable]
    internal class EasyAssetBundleLabelConfig {

        [SerializeField]
        internal List<DirectoryLabelSetting> LabelSetting = new List<DirectoryLabelSetting>();
        [SerializeField]
        internal List<FileNameCkeckerSetting> FileNameChecker = new List<FileNameCkeckerSetting>();
        [SerializeField]
        internal AssetBundleLabelType DefaultLabelType;
        [SerializeField]
        internal string DefaultNGPattern = "[!\"#$%&'()\\*\\+\\.,\\/:;<=>?@\\[\\\\]^`{|}~]+";

        [SerializeField]
        internal string BuildRootPath;

        internal EasyAssetBundleLabelConfig() { }

        internal EasyAssetBundleLabelConfig( EasyAssetBundleLabelConfig copy ) {
            for( int i = 0; i < copy.LabelSetting.Count; i++ )
                LabelSetting.Add( new DirectoryLabelSetting( copy.LabelSetting[i] ) );

            for( int i = 0; i < copy.FileNameChecker.Count; i++ )
                FileNameChecker.Add( new FileNameCkeckerSetting( copy.FileNameChecker[ i ] ) );

            DefaultLabelType = copy.DefaultLabelType;
            DefaultNGPattern = copy.DefaultNGPattern;
        }

        internal void Initialize( string buildRootPath ) {
            BuildRootPath = buildRootPath;
        }

        #region method
        //=======================================================
        // method
        //=======================================================

        internal string GetAssetLabel( string path, string extension, string buildPath, string guid ) {

            for( int i = 0; i < LabelSetting.Count; i++ ) {
                if( LabelSetting[ i ].CheckLabel( buildPath ) )
                    return LabelSetting[ i ].AssetBundleName;
            }

            switch( DefaultLabelType ) {
                case AssetBundleLabelType.FileName:
                    return Path.GetFileNameWithoutExtension( path ).ToLower();
                case AssetBundleLabelType.RelativePath:
                    return string.IsNullOrEmpty( extension ) ? buildPath.ToLower() : buildPath.ToLower().Replace( extension, "" );
                case AssetBundleLabelType.GuidFileName:
                    return guid;
                case AssetBundleLabelType.GuidRelativePath:
                    return FileHelper.pathSlashFix( Path.Combine( Path.GetDirectoryName( buildPath ).ToLower(), guid ) );
            }
            return null;
        }

        internal bool CheckDirectoryLabel( string path, string buildPath ) {
            for( int i = 0; i < LabelSetting.Count; i++ ) {
                if( LabelSetting[ i ].CheckLabel( buildPath ) )
                    return true;
            }
            return false;
        }

        internal bool CheckFileAvailable( string path, string fileName, string extension, string buildPath ) {
            if( NGExtensions( extension ) )
                return false;

            try {
                var regex = new Regex( DefaultNGPattern, RegexOptions.Compiled );
                if( regex.IsMatch( fileName ) )
                    return false;
            } catch {
                return false;
            }

            for( int i = 0; i < FileNameChecker.Count; i++ ) {
                if( FileNameChecker[ i ].IsMatch( buildPath ) )
                    return FileNameChecker[ i ].CheckAvailable( fileName, extension );
            }
            return true;
        }

        private bool NGExtensions( string extension ) {
            if( extension.Equals( ".cs" ) || extension.Equals( ".js" ) || extension.Equals( ".dll" ) )
                return true;
            return false;
        }

        #endregion

        #region drawer

        //=======================================================
        // drawer
        //=======================================================
        [SerializeField]
        private bool Fold;
        [SerializeField]
        private string SearchText;
        [SerializeField]
        private int SelectedMenu;
        private Vector2 ScrollView;
        internal void OnGUI() {
            var buildPathExists = !string.IsNullOrEmpty( BuildRootPath );
            if( !buildPathExists )
                EditorGUILayout.HelpBox( "set up buildroot path", MessageType.Warning );

            EditorGUI.BeginDisabledGroup( !buildPathExists );
            EditorGUILayout.BeginVertical();
            {
                GUILayout.Space( 7 );

                EditorGUILayout.BeginHorizontal();
                {
                    Undo.IncrementCurrentGroup();
                    UndoHelper.BuilderDataUndo( "Change default label type" );
                    GUILayout.Label( new GUIContent( "Default label type", "used for naming assetlabel" ) );
                    DefaultLabelType = ( AssetBundleLabelType )EditorGUILayout.EnumPopup( DefaultLabelType );
                }
                EditorGUILayout.EndHorizontal();

                GUILayout.Space( 15 );

                Undo.IncrementCurrentGroup();
                UndoHelper.BuilderDataUndo( "Change NG filename pattern" );
                GUILayout.Label( "NG FileName Pattern" );
                DefaultNGPattern = EditorGUILayout.TextField( DefaultNGPattern );

                GUILayout.Space( 10 );

                EditorGUILayout.BeginVertical( EditorStyles.helpBox );
                {
                    GUILayout.Space( 5 );

                    var menu = GUILayout.Toolbar( SelectedMenu, new string[] { "FileName Checker", "Directory Label" }, GUILayout.Height( 20 ) );
                    if( SelectedMenu != menu ) {
                        UndoHelper.BuilderDataUndo( "Change label menu" );
                        EditorGUIUtility.keyboardControl = 0;
                    }
                    SelectedMenu = menu;

                    EditorGUILayout.BeginHorizontal();
                    {
                        GUI.backgroundColor = Color.yellow;
                        switch( SelectedMenu ) {
                            case 0:
                                if( GUILayout.Button( "+", GUILayout.Width( 25 ) ) ) {
                                    UndoHelper.BuilderDataUndo( "Add FilenameChecker Setting" );
                                    FileNameChecker.Add( new FileNameCkeckerSetting() );
                                    EditorGUIUtility.keyboardControl = 0;
                                }
                                break;
                            case 1:
                                if( GUILayout.Button( "+", GUILayout.Width( 25 ) ) ) {
                                    UndoHelper.BuilderDataUndo( "Add DirectoryLabel Setting" );
                                    LabelSetting.Add( new DirectoryLabelSetting() );
                                    EditorGUIUtility.keyboardControl = 0;
                                }
                                break;
                        }

                        GUI.backgroundColor = Color.white;

                        Undo.IncrementCurrentGroup();
                        UndoHelper.BuilderDataUndo( "Change setting search text" );
                        SearchText = EditorGUILayout.TextField( SearchText, GUIHelper.Styles.SearchField, GUILayout.ExpandWidth( true ) );
                        if( GUILayout.Button( "", GUIHelper.Styles.SearchFieldCancel ) ) {
                            SearchText = "";
                            EditorGUIUtility.keyboardControl = 0;
                        }
                    }
                    EditorGUILayout.EndHorizontal();

                    GUILayout.Space( 5 );

                    ScrollView = EditorGUILayout.BeginScrollView( ScrollView );
                    {
                        switch( SelectedMenu ) {
                            case 0:
                                FileNameCheckerView();
                                break;
                            case 1:
                                LabelSettingView();
                                break;
                        }
                    }
                    EditorGUILayout.EndScrollView();

                    GUILayout.FlexibleSpace();
                }
                EditorGUILayout.EndVertical();


            }
            EditorGUILayout.EndVertical();

            EditorGUI.EndDisabledGroup();
        }

        private void LabelSettingView() {
            EditorGUILayout.BeginVertical();
            {
                for( int i = 0; i < LabelSetting.Count; i++ ) {
                    if( string.IsNullOrEmpty( SearchText ) || LabelSetting[i].DirectoryName.Contains( SearchText ) )
                        LabelSettingDrawer( LabelSetting[ i ] );
                }
            }
            EditorGUILayout.EndVertical();
        }

        private void LabelSettingDrawer( DirectoryLabelSetting setting ) {
            var rect = EditorGUILayout.BeginVertical( GUIHelper.Styles.NoSpaceBox );
            {
                GUI.backgroundColor = setting.IsActive ? Color.green : Color.grey;
                if( GUILayout.Button( "", EditorStyles.toolbarButton, GUILayout.ExpandWidth( true ) ) ) {
                    UndoHelper.BuilderDataUndo( "Change DirectoryLabel activation" );
                    setting.IsActive = !setting.IsActive;
                    EditorGUIUtility.keyboardControl = 0;
                }
                GUI.backgroundColor = Color.white;

                EditorGUILayout.BeginHorizontal();
                {
                    Undo.IncrementCurrentGroup();
                    UndoHelper.BuilderDataUndo( "Change DirectoryLabel Setting" );
                    GUILayout.Label( "Directory", GUILayout.Width( 70 ) );
                    setting.DirectoryName = EditorGUILayout.TextField( setting.DirectoryName );
                    var path = FileHelper.GetDraggedObject( Event.current, GUILayoutUtility.GetLastRect() );
                    if( !string.IsNullOrEmpty( path ) )
                        setting.DirectoryName = FileHelper.AssetPathToBuildPath( BuildRootPath, path );
                }
                EditorGUILayout.EndHorizontal();

                if( setting.IsActive ) {
                    EditorGUILayout.BeginHorizontal();
                    {
                        Undo.IncrementCurrentGroup();
                        UndoHelper.BuilderDataUndo( "Change DirectoryLabel Setting" );
                        GUILayout.Label( "Label", GUILayout.Width( 70 ) );
                        setting.AssetBundleName = EditorGUILayout.TextField( setting.AssetBundleName );
                    }
                    EditorGUILayout.EndHorizontal();

                    setting.Type = ( DirectoryLabelNameType )EditorGUILayout.EnumPopup( setting.Type );
                    GUILayout.Space( 5 );
                }
            }
            EditorGUILayout.EndVertical();

            GUILayout.Space( 3 );

            if( IsContextClick( Event.current, rect ) ) {
                var menu = new GenericMenu();
                menu.AddItem( new GUIContent( "Delete" ), false, () => {
                    UndoHelper.BuilderDataUndo( "Delete DirectoryLabel Setting" );
                    LabelSetting.Remove( setting );
                } );
                menu.ShowAsContext();
            }
        }

        private void FileNameCheckerView() {
            EditorGUILayout.BeginVertical();
            {
                for( int i = 0; i < FileNameChecker.Count; i++ )
                    if( string.IsNullOrEmpty( SearchText ) || FileNameChecker[ i ].DirectoryName.Contains( SearchText ) )
                        FileNameCheckerDrawer( FileNameChecker[ i ] );
            }
            EditorGUILayout.EndVertical();
        }

        private void FileNameCheckerDrawer( FileNameCkeckerSetting setting ) {
            var rect = EditorGUILayout.BeginVertical( GUIHelper.Styles.NoSpaceBox );
            {
                GUI.backgroundColor = setting.IsActive ? Color.green : Color.grey;
                if( GUILayout.Button( "", EditorStyles.toolbarButton, GUILayout.ExpandWidth( true ) ) ) {
                    UndoHelper.BuilderDataUndo( "Change FileNameChecker activation" );
                    setting.IsActive = !setting.IsActive;
                    EditorGUIUtility.keyboardControl = 0;
                }
                GUI.backgroundColor = Color.white;

                EditorGUILayout.BeginHorizontal();
                {
                    GUILayout.Label( "Directory", GUILayout.Width( 70 ) );
                    EditorGUILayout.TextField( setting.DirectoryName );
                    var path = FileHelper.GetDraggedObject( Event.current, GUILayoutUtility.GetLastRect() );
                    if( !string.IsNullOrEmpty( path ) ) {
                        UndoHelper.BuilderDataUndo( "Change FileNameChecker Setting" );
                        setting.DirectoryName = FileHelper.AssetPathToBuildPath( BuildRootPath, path );
                    }
                }
                EditorGUILayout.EndHorizontal();

                if( setting.IsActive ) {
                    EditorGUILayout.BeginHorizontal();
                    {
                        Undo.IncrementCurrentGroup();
                        UndoHelper.BuilderDataUndo( "Change FileNameChecker type" );
                        GUILayout.Label( "Directory type", GUILayout.Width( 70 ) );
                        setting.DirectoryType = ( DirectoryLabelNameType )EditorGUILayout.EnumPopup( setting.DirectoryType );
                    }
                    EditorGUILayout.EndHorizontal();

                    GUILayout.Space( 5 );

                    EditorGUILayout.BeginHorizontal();
                    {
                        Undo.IncrementCurrentGroup();
                        UndoHelper.BuilderDataUndo( "Change FileNameChecker regex" );
                        GUILayout.Label( "Regex", GUILayout.Width( 70 ) );
                        setting.RegexPattern = EditorGUILayout.TextField( setting.RegexPattern );
                    }
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.BeginHorizontal();
                    {
                        Undo.IncrementCurrentGroup();
                        UndoHelper.BuilderDataUndo( "Change FileNameChecker filetype" );
                        GUILayout.Label( "File Type", GUILayout.Width( 70 ) );
                        setting.FileType = ( LabelCheckerFileType )EditorGUILayout.EnumPopup( setting.FileType );
                    }
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.BeginHorizontal();
                    {
                        Undo.IncrementCurrentGroup();
                        UndoHelper.BuilderDataUndo( "Change FileNameChecker checkerType" );
                        GUILayout.Label( "Checker Type", GUILayout.Width( 70 ) );
                        setting.CheckerType = ( LabelCheckerType )EditorGUILayout.EnumPopup( setting.CheckerType );
                    }
                    EditorGUILayout.EndHorizontal();

                    GUILayout.Space( 5 );
                }

            }
            EditorGUILayout.EndVertical();

            GUILayout.Space( 3 );

            if( IsContextClick( Event.current, rect ) ) {
                var menu = new GenericMenu();
                menu.AddItem( new GUIContent( "Delete" ), false, () => {
                    UndoHelper.BuilderDataUndo( "Delete FileNameChecker Setting" );
                    FileNameChecker.Remove( setting );
                } );
                menu.ShowAsContext();
            }
        }

        private bool IsContextClick( Event e, Rect rect ) {
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