using System;
using System.Linq;
using System.Collections.Generic;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEditor;

// this code from unity technologies tree view sample
// http://files.unity3d.com/mads/TreeViewExamples.zip

namespace charcolle.Utility.EasyAssetBundle.v1 {

    internal class EasyAssetBundleAssetListView : TreeViewWithTreeModel<EditorAssetInfo> {

        const float kRowHeights = 20f;
        const float kToggleWidth = 18f;
        public bool showControls = true;
        //[SerializeField]
        //private IAssetBundleBuildConfig Config;
        //[SerializeField]
        //private string BuildRootPath;

        // All columns
        enum Columns {
            Icon,
            Checker,
            FileName,
            AssetBundleName,
            AssetPath,
            Extension,
            Version,
            Size,
            Date,
            UnityVersion,
        }

        public enum SortOption {
            Icon,
            Checker,
            FileName,
            AssetBundleName,
            AssetPath,
            Extension,
            Version,
            Size,
            Date,
            UnityVersion,
        }

        // Sort options per column
        SortOption[] m_SortOptions =
        {
            SortOption.Icon,
            SortOption.Checker,
            SortOption.FileName,
            SortOption.AssetBundleName,
            SortOption.AssetPath,
            SortOption.Extension,
            SortOption.Version,
            SortOption.Size,
            SortOption.Date,
            SortOption.UnityVersion,
        };

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

        public EasyAssetBundleAssetListView( TreeViewState state, MultiColumnHeader multicolumnHeader, TreeModel<EditorAssetInfo> model ) : base( state, multicolumnHeader, model ) {
            // Custom setup
            rowHeight = kRowHeights;
            columnIndexForTreeFoldouts = 0;
            showAlternatingRowBackgrounds = true;
            showBorder = false;
            customFoldoutYOffset = ( kRowHeights - EditorGUIUtility.singleLineHeight ) * 0.5f; // center foldout in the row since we also center content. See RowGUI
            extraSpaceBeforeIconAndLabel = kToggleWidth;
            multicolumnHeader.sortingChanged += OnSortingChanged;

            Reload();
        }

        public void Initialize( IAssetBundleBuildConfig config, string buildRootPath ) {
            //Config = config;
            //BuildRootPath = buildRootPath;
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
            SortByMultipleColumns();
            TreeToList( root, rows );
            Repaint();
        }

        void SortByMultipleColumns() {
            var sortedColumns = multiColumnHeader.state.sortedColumns;

            if( sortedColumns.Length == 0 )
                return;

            var myTypes = rootItem.children.Cast<TreeViewItem<EditorAssetInfo>>();
            var orderedQuery = InitialOrder( myTypes, sortedColumns );
            for( int i = 1; i < sortedColumns.Length; i++ ) {
                SortOption sortOption = m_SortOptions[ sortedColumns[ i ] ];
                bool ascending = multiColumnHeader.IsSortedAscending( sortedColumns[ i ] );

                switch( sortOption ) {
                    case SortOption.Checker:
                        orderedQuery = orderedQuery.ThenBy( l => l.data.IsAvailable, ascending );
                        break;
                    case SortOption.FileName:
                        orderedQuery = orderedQuery.ThenBy( l => l.data.Name, ascending );
                        break;
                    case SortOption.AssetBundleName:
                        orderedQuery = orderedQuery.ThenBy( l => l.data.AssetBundleName, ascending );
                        break;
                    case SortOption.AssetPath:
                        orderedQuery = orderedQuery.ThenBy( l => l.data.AssetPath, ascending );
                        break;
                    case SortOption.Extension:
                        orderedQuery = orderedQuery.ThenBy( l => l.data.Extension, ascending );
                        break;
                    case SortOption.Version:
                        orderedQuery = orderedQuery.ThenBy( l => l.data.Version, ascending );
                        break;
                    case SortOption.Size:
                        orderedQuery = orderedQuery.ThenBy( l => l.data.Size, ascending );
                        break;
                    case SortOption.Date:
                        orderedQuery = orderedQuery.ThenBy( l => l.data.Date, ascending );
                        break;
                    case SortOption.UnityVersion:
                        orderedQuery = orderedQuery.ThenBy( l => l.data.UnityVersion, ascending );
                        break;
                }
            }

            rootItem.children = orderedQuery.Cast<TreeViewItem>().ToList();
        }

        IOrderedEnumerable<TreeViewItem<EditorAssetInfo>> InitialOrder( IEnumerable<TreeViewItem<EditorAssetInfo>> myTypes, int[] history ) {
            SortOption sortOption = m_SortOptions[ history[ 0 ] ];
            bool ascending = multiColumnHeader.IsSortedAscending( history[ 0 ] );
            switch( sortOption ) {
                case SortOption.Checker:
                    return myTypes.Order( l => l.data.IsAvailable, ascending );
                case SortOption.FileName:
                    return myTypes.Order( l => l.data.Name, ascending );
                case SortOption.AssetBundleName:
                    return myTypes.Order( l => l.data.AssetBundleName, ascending );
                case SortOption.AssetPath:
                    return myTypes.Order( l => l.data.AssetPath, ascending );
                case SortOption.Extension:
                    return myTypes.Order( l => l.data.Extension, ascending );
                case SortOption.Version:
                    return myTypes.Order( l => l.data.Version, ascending );
                case SortOption.Size:
                    return myTypes.Order( l => l.data.Size, ascending );
                case SortOption.Date:
                    return myTypes.Order( l => l.data.Date, ascending );
                case SortOption.UnityVersion:
                    return myTypes.Order( l => l.data.UnityVersion, ascending );
                default:
                    Assert.IsTrue( false, "Unhandled enum" );
                    break;
            }

            // default
            return myTypes.Order( l => l.data.name, ascending );
        }

        protected override void RowGUI( RowGUIArgs args ) {
            var item = ( TreeViewItem<EditorAssetInfo> )args.item;

            for( int i = 0; i < args.GetNumVisibleColumns(); ++i )
                CellGUI( args.GetCellRect( i ), item, ( Columns )args.GetColumn( i ), ref args );
        }

        void CellGUI( Rect cellRect, TreeViewItem<EditorAssetInfo> item, Columns column, ref RowGUIArgs args ) {
            // Center cell rect vertically (makes it easier to place controls, icons etc in the cells)
            CenterRectUsingSingleLineHeight( ref cellRect );

            //if( item.data.IsSelected != args.selected ) {
            //    item.data.OnSelected();
            //    item.data.IsSelected = args.selected;
            //}

            switch( column ) {
                case Columns.Icon: {
                        if( item.data.Texture != null )
                            GUI.DrawTexture( cellRect, item.data.Texture, ScaleMode.ScaleToFit );
                    }
                    break;

                case Columns.Checker: {
                        GUI.DrawTexture( cellRect, item.data.IsAvailable ? GUIHelper.Textures.CheckMarkOK : GUIHelper.Textures.CheckMaarkNG, ScaleMode.ScaleToFit );
                    }
                    break;

                case Columns.FileName: {
                        var toggleRect = cellRect;
                        toggleRect.width = kToggleWidth;
                        item.data.IsBuild = EditorGUI.Toggle( toggleRect, item.data.IsBuild );

                        var rect = cellRect;
                        rect.x += 15;
                        rect.width -= 15;
                        DefaultGUI.Label( rect, item.data.Name, args.selected, args.focused );
                    }
                    break;

                case Columns.AssetBundleName: {
                        DefaultGUI.Label( cellRect, item.data.AssetBundleName, args.selected, args.focused );
                    }
                    break;

                case Columns.AssetPath: {
                        DefaultGUI.Label( cellRect, item.data.AssetPath, args.selected, args.focused );
                    }
                    break;

                case Columns.Extension: {
                        DefaultGUI.Label( cellRect, item.data.Extension, args.selected, args.focused );
                    }
                    break;

                case Columns.Version: {
                        if( showControls ) {
                            cellRect.xMin += 5f;
                            item.data.Version = EditorGUI.IntField( cellRect, item.data.Version );
                        } else {
                            DefaultGUI.Label( cellRect, item.data.Version.ToString(), args.selected, args.focused );
                        }
                    }
                    break;
                case Columns.Size: {
                        DefaultGUI.Label( cellRect, item.data.Size.ToString(), args.selected, args.focused );
                    }
                    break;
                case Columns.Date: {
                        DefaultGUI.Label( cellRect, item.data.Date, args.selected, args.focused );
                    }
                    break;
                case Columns.UnityVersion: {
                        DefaultGUI.Label( cellRect, item.data.UnityVersion, args.selected, args.focused );
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

        public static MultiColumnHeaderState CreateDefaultMultiColumnHeaderState( float treeViewWidth ) {
            var columns = new[]
            {
                new MultiColumnHeaderState.Column
                {
                    headerContent = new GUIContent(EditorGUIUtility.FindTexture( "FilterByType" ), "Type of the asset."),
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
                    headerContent = new GUIContent(EditorGUIUtility.FindTexture( "Checkmark" ), "Check if filename is available. "),
                    contextMenuText = "",
                    headerTextAlignment = TextAlignment.Center,
                    sortedAscending = true,
                    sortingArrowAlignment = TextAlignment.Right,
                    width = 30,
                    minWidth = 30,
                    maxWidth = 35,
                    autoResize = false,
                    allowToggleVisibility = true
                },
                new MultiColumnHeaderState.Column
                {
                    headerContent = new GUIContent( "FileName" ),
                    headerTextAlignment = TextAlignment.Left,
                    sortedAscending = true,
                    sortingArrowAlignment = TextAlignment.Right,
                    width = 160,
                    minWidth = 150,
                    autoResize = false,
                    allowToggleVisibility = false
                },
                new MultiColumnHeaderState.Column
                {
                    headerContent = new GUIContent("AssetBundle"),
                    headerTextAlignment = TextAlignment.Left,
                    sortedAscending = true,
                    sortingArrowAlignment = TextAlignment.Right,
                    width = 100,
                    minWidth = 70,
                    autoResize = true,
                    allowToggleVisibility = true
                },
                new MultiColumnHeaderState.Column
                {
                    headerContent = new GUIContent("AssetPath"),
                    headerTextAlignment = TextAlignment.Left,
                    sortedAscending = true,
                    sortingArrowAlignment = TextAlignment.Right,
                    width = 70,
                    minWidth = 60,
                    autoResize = true
                },
                new MultiColumnHeaderState.Column
                {
                    headerContent = new GUIContent( "Extension"),
                    headerTextAlignment = TextAlignment.Left,
                    sortedAscending = true,
                    sortingArrowAlignment = TextAlignment.Right,
                    width = 70,
                    minWidth = 65,
                    maxWidth = 100,
                    autoResize = true
                },
                new MultiColumnHeaderState.Column
                {
                    headerContent = new GUIContent("Version"),
                    headerTextAlignment = TextAlignment.Left,
                    sortedAscending = true,
                    sortingArrowAlignment = TextAlignment.Left,
                    width = 50,
                    minWidth = 40,
                    maxWidth = 50,
                },
                new MultiColumnHeaderState.Column
                {
                    headerContent = new GUIContent("Size"),
                    headerTextAlignment = TextAlignment.Left,
                    sortedAscending = true,
                    sortingArrowAlignment = TextAlignment.Left,
                    width = 50,
                    minWidth = 40,
                    maxWidth = 50,
                },
                new MultiColumnHeaderState.Column
                {
                    headerContent = new GUIContent("Date"),
                    headerTextAlignment = TextAlignment.Right,
                    sortedAscending = true,
                    sortingArrowAlignment = TextAlignment.Left,
                    width = 60,
                    minWidth = 40,
                    autoResize = true
                },
                new MultiColumnHeaderState.Column
                {
                    headerContent = new GUIContent("UnityVersion"),
                    headerTextAlignment = TextAlignment.Right,
                    sortedAscending = true,
                    sortingArrowAlignment = TextAlignment.Left,
                    width = 60,
                    minWidth = 40,
                    autoResize = true
                }
            };

            var state = new MultiColumnHeaderState( columns );
            return state;
        }

    }

}