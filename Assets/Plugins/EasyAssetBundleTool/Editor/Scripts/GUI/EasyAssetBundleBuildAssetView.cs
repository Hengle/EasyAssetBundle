using System;
using System.Collections.Generic;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using UnityEditor;

using UndoHelper = charcolle.Utility.EasyAssetBundle.v1.UndoHelper;

// this code from unity technologies tree view sample
// http://files.unity3d.com/mads/TreeViewExamples.zip

namespace charcolle.Utility.EasyAssetBundle.v1 {

    internal class EasyAssetBundleBuildAssetView : TreeViewWithTreeModel<EditorAssetInfo> {

        const float kRowHeights = 20f;
        const float kToggleWidth = 18f;
        public bool showControls = true;
        [SerializeField]
        private IAssetBundleBuildConfig Config;
        [SerializeField]
        private string BuildRootPath;

        // All columns
        enum Columns {
            Icon,
            Checker,
            FileName,
            Extension,
            AssetBundleName,
            AssetPath,
            Guid,
            Version,
            UnityVersion,
        }

        public static void TreeToList( TreeViewItem root, IList<TreeViewItem> result ) {
            if( root == null )
                throw new NullReferenceException( "root" );
            if( result == null )
                throw new NullReferenceException( "result" );

            result.Clear();

            if( root.children == null )
                return;

            Stack<TreeViewItem> stack = new Stack<TreeViewItem>();
            for( int i = root.children.Count - 1; i >= 0; i-- )
                stack.Push( root.children[ i ] );

            while( stack.Count > 0 ) {
                TreeViewItem current = stack.Pop();
                result.Add( current );

                if( current.hasChildren && current.children[ 0 ] != null ) {
                    for( int i = current.children.Count - 1; i >= 0; i-- ) {
                        stack.Push( current.children[ i ] );
                    }
                }
            }
        }

        public EasyAssetBundleBuildAssetView( TreeViewState state, MultiColumnHeader multicolumnHeader, TreeModel<EditorAssetInfo> model ) : base( state, multicolumnHeader, model ) {
            //Assert.AreEqual( m_SortOptions.Length, Enum.GetValues( typeof( Columns ) ).Length, "Ensure number of sort options are in sync with number of MyColumns enum values" );

            // Custom setup
            rowHeight = kRowHeights;
            columnIndexForTreeFoldouts = 2;
            showAlternatingRowBackgrounds = true;
            showBorder = false;
            customFoldoutYOffset = ( kRowHeights - EditorGUIUtility.singleLineHeight ) * 0.5f; // center foldout in the row since we also center content. See RowGUI
            extraSpaceBeforeIconAndLabel = kToggleWidth;
            multicolumnHeader.sortingChanged += OnSortingChanged;

            Reload();
        }

        public void Initialize( IAssetBundleBuildConfig config, string rootPath ) {
            Config = config;
            BuildRootPath = rootPath;
        }

        // Note we We only build the visible rows, only the backend has the full tree information.
        // The treeview only creates info for the row list.
        protected override IList<TreeViewItem> BuildRows( TreeViewItem root ) {
            var rows = base.BuildRows( root );
            SortIfNeeded( root, rows );
            return rows;
        }

        void OnSortingChanged( MultiColumnHeader multiColumnHeader ) {
            SortIfNeeded( rootItem, GetRows() );
        }

        void SortIfNeeded( TreeViewItem root, IList<TreeViewItem> rows ) {
            if( rows.Count <= 1 )
                return;

            if( multiColumnHeader.sortedColumnIndex == -1 ) {
                return; // No column to sort for (just use the order the data are in)
            }

            // Sort the roots of the existing tree items
            //SortByMultipleColumns();
            TreeToList( root, rows );
            Repaint();
        }


        protected override void RowGUI( RowGUIArgs args ) {
            var item = ( TreeViewItem<EditorAssetInfo> )args.item;

            for( int i = 0; i < args.GetNumVisibleColumns(); ++i )
                CellGUI( args.GetCellRect( i ), item, ( Columns )args.GetColumn( i ), ref args );
        }

        void CellGUI( Rect cellRect, TreeViewItem<EditorAssetInfo> item, Columns column, ref RowGUIArgs args ) {
            // Center cell rect vertically (makes it easier to place controls, icons etc in the cells)
            CenterRectUsingSingleLineHeight( ref cellRect );

            if( item.data.IsSelected != args.selected ) {
                item.data.OnSelected();
                item.data.IsSelected = args.selected;
            }

            switch( column ) {
                case Columns.Icon: {
                        if( item.data.Texture != null )
                            GUI.DrawTexture( cellRect, item.data.Texture, ScaleMode.ScaleToFit );
                    }
                    break;
                case Columns.Checker: {
                        GUI.DrawTexture( cellRect, item.data.IsAvailable  ? GUIHelper.Textures.CheckMarkOK : GUIHelper.Textures.CheckMaarkNG, ScaleMode.ScaleToFit );
                    }
                    break;
                case Columns.FileName: {
                        var toggleRect = cellRect;
                        toggleRect.x += GetContentIndent( item );
                        toggleRect.width = kToggleWidth;

                        var build = EditorGUI.Toggle( toggleRect, item.data.IsBuild ); // hide when outside cell rect
                        if( item.data.IsBuild != build ) {
                            UndoHelper.BuilderDataUndo( "Check asset" );
                            setChildren( item, build );
                        }
                        //if( item.data.IsDirectoryLabel )
                        //    SetExpanded( item.id, false );

                        var rect = cellRect;
                        rect.x += GetContentIndent( item ) + 15f;
                        rect.width -= ( GetContentIndent( item ) + 15f );
                        EditorGUI.BeginDisabledGroup( !item.data.IsBuild );
                        DefaultGUI.Label( rect, item.data.Name, args.selected, args.focused );
                        EditorGUI.EndDisabledGroup();
                    }
                    break;
                case Columns.Extension: {
                        EditorGUI.BeginDisabledGroup( !item.data.IsBuild );
                        DefaultGUI.Label( cellRect, item.data.Extension, args.selected, args.focused );
                        EditorGUI.EndDisabledGroup();
                    }
                    break;
                case Columns.AssetBundleName: {
                        if( item.data.IsBuild ) {
                            var assetBundleName = Config.GetAssetLabel( item.data, BuildRootPath );
                            item.data.AssetBundleName = assetBundleName;
                        } else {
                            if( item.parent != null ) {
                                var parent = getAssetInfo( item.parent );
                                item.data.AssetBundleName = parent.data.AssetBundleName;
                            }
                        }
                        if( !item.data.IsAvailable )
                            break;
                        EditorGUI.BeginDisabledGroup( !item.data.IsBuild );
                        DefaultGUI.Label( cellRect, item.data.AssetBundleName, args.selected, args.focused );
                        EditorGUI.EndDisabledGroup();
                    }
                    break;
                case Columns.AssetPath: {
                        EditorGUI.BeginDisabledGroup( !item.data.IsBuild );
                        DefaultGUI.Label( cellRect, item.data.AssetPath, args.selected, args.focused );
                        EditorGUI.EndDisabledGroup();
                    }
                    break;
                case Columns.Guid: {
                        EditorGUI.BeginDisabledGroup( !item.data.IsBuild );
                        DefaultGUI.Label( cellRect, item.data.Guid, args.selected, args.focused );
                        EditorGUI.EndDisabledGroup();
                    }
                    break;
                case Columns.Version: {
                        EditorGUI.BeginDisabledGroup( !item.data.IsBuild );
                        if( Config.IsUseAssetBundleList ) {
                            item.data.Version = item.data.IsBuild ? item.data.OldAssetVersion + 1 : item.data.OldAssetVersion;
                            var version = item.data.Version > 0 ? item.data.Version : 1;
                            DefaultGUI.Label( cellRect, version.ToString(), args.selected, args.focused );

                            var iconRect = cellRect;
                            iconRect.x += 13;
                            iconRect.width = 30;
                            if( item.data.IsBuild )
                                GUI.DrawTexture( iconRect, item.data.OldAssetVersion > 0 ? GUIHelper.Textures.VersionUpdate : GUIHelper.Textures.VersionNew, ScaleMode.ScaleToFit );
                        }
                        EditorGUI.EndDisabledGroup();
                    }
                    break;
                case Columns.UnityVersion: {
                        EditorGUI.BeginDisabledGroup( !item.data.IsBuild );
                        DefaultGUI.Label( cellRect, item.data.UnityVersion, args.selected, args.focused );
                        EditorGUI.EndDisabledGroup();
                    }
                    break;
            }
        }

        //=======================================================
        // header
        //=======================================================

        protected override bool CanRename( TreeViewItem item ) {
            // Only allow rename if we can show the rename overlay with a certain width (label might be clipped by other columns)
            Rect renameRect = GetRenameRect( treeViewRect, 0, item );
            return renameRect.width > 30;
        }

        protected override void RenameEnded( RenameEndedArgs args ) {
            // Set the backend name and reload the tree to reflect the new model
            if( args.acceptedRename ) {
                var element = treeModel.Find( args.itemID );
                element.name = args.newName;
                Reload();
            }
        }

        protected override Rect GetRenameRect( Rect rowRect, int row, TreeViewItem item ) {
            Rect cellRect = GetCellRectForTreeFoldouts( rowRect );
            CenterRectUsingSingleLineHeight( ref cellRect );
            return base.GetRenameRect( cellRect, row, item );
        }

        protected override bool CanMultiSelect( TreeViewItem item ) {
            return true;
        }

        //=======================================================
        // header
        //=======================================================

        public static MultiColumnHeaderState CreateDefaultMultiColumnHeaderState( float treeViewWidth, bool useVersion = false ) {
            var columns = new List<MultiColumnHeaderState.Column>()
            {
                new MultiColumnHeaderState.Column
                {
                    headerContent = new GUIContent(EditorGUIUtility.FindTexture("FilterByType"), "AssetType"),
                    contextMenuText = "",
                    headerTextAlignment = TextAlignment.Center,
                    canSort = false,
                    sortingArrowAlignment = TextAlignment.Right,
                    width = 30,
                    minWidth = 30,
                    maxWidth = 35,
                    autoResize = false,
                    allowToggleVisibility = true
                },
                new MultiColumnHeaderState.Column
                {
                    headerContent = new GUIContent(EditorGUIUtility.FindTexture("Checkmark"), "Lorem ipsum dolor sit amet, consectetur adipiscing elit. "),
                    contextMenuText = "",
                    headerTextAlignment = TextAlignment.Left,
                    canSort = false,
                    width = 30,
                    minWidth = 30,
                    maxWidth = 35,
                    autoResize = false,
                    allowToggleVisibility = true
                },
                new MultiColumnHeaderState.Column
                {
                    headerContent = new GUIContent("FileName"),
                    headerTextAlignment = TextAlignment.Left,
                    canSort = false,
                    width = 160,
                    minWidth = 150,
                    autoResize = false,
                    allowToggleVisibility = false
                },
                new MultiColumnHeaderState.Column
                {
                    headerContent = new GUIContent("Extension", "In sed porta ante. Nunc et nulla mi."),
                    headerTextAlignment = TextAlignment.Left,
                    canSort = false,
                    width = 70,
                    minWidth = 70,
                    maxWidth = 100,
                    autoResize = true
                },
                new MultiColumnHeaderState.Column
                {
                    headerContent = new GUIContent("AssetBundle", "Maecenas congue non tortor eget vulputate."),
                    headerTextAlignment = TextAlignment.Left,
                    canSort = false,
                    width = 70,
                    minWidth = 75,
                    autoResize = true,
                    allowToggleVisibility = true
                },
                new MultiColumnHeaderState.Column {
                    headerContent = new GUIContent( "AssetPath", "Nam at tellus ultricies ligula vehicula ornare sit amet quis metus." ),
                    headerTextAlignment = TextAlignment.Left,
                    canSort = false,
                    width = 90,
                    minWidth = 60,
                    autoResize = true
                },
                new MultiColumnHeaderState.Column {
                    headerContent = new GUIContent( "Guid", "Nam at tellus ultricies ligula vehicula ornare sit amet quis metus." ),
                    headerTextAlignment = TextAlignment.Left,
                    canSort = false,
                    width = 90,
                    minWidth = 60,
                    autoResize = true
                }
            };
            if( useVersion ) {
                columns.Add( new MultiColumnHeaderState.Column {
                    headerContent = new GUIContent( "Version", "Nam at tellus ultricies ligula vehicula ornare sit amet quis metus." ),
                    headerTextAlignment = TextAlignment.Left,
                    canSort = false,
                    width = 55,
                    minWidth = 55,
                    maxWidth = 60,
                    autoResize = true
                } );
                columns.Add( new MultiColumnHeaderState.Column {
                    headerContent = new GUIContent( "UnityVersion", "Nam at tellus ultricies ligula vehicula ornare sit amet quis metus." ),
                    headerTextAlignment = TextAlignment.Left,
                    canSort = false,
                    width = 70,
                    minWidth = 60,
                    maxWidth = 75,
                    autoResize = true
                } );
            }

            var state = new MultiColumnHeaderState( columns.ToArray() );
            return state;
        }

        //=======================================================
        // utility
        //=======================================================

        private void setChildren( TreeViewItem item, bool val ) {
            if( item == null )
                return;
            var info = getAssetInfo( item );
            parentFlag = 0;
            checkParent( item );
            info.data.IsBuild = parentFlag == 0 ? val : false;

            if( !item.hasChildren )
                return;

            for( int i = 0; i < info.children.Count; i++ ) {
                var child = getAssetInfo( info.children[ i ] );
                setChildren( child, !info.data.IsBuild );
            }
        }

        private int parentFlag = 0;
        private void checkParent( TreeViewItem item ) {
            if( item.parent == null )
                return;

            var parent = getAssetInfo( item.parent );
            if( parent.parent != null )
                checkParent( parent );

            if( parent.data.IsBuild || parent.data.IsDirectoryLabel ) parentFlag++;
        }

        private TreeViewItem<EditorAssetInfo> getAssetInfo( TreeViewItem element ) {
            return ( TreeViewItem<EditorAssetInfo> )element;
        }

    }


}