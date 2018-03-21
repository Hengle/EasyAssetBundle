using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.IMGUI.Controls;

using FileHelper = charcolle.Utility.EasyAssetBundle.v1.FileHelper;
using GUIHelper  = charcolle.Utility.EasyAssetBundle.v1.GUIHelper;

namespace charcolle.Utility.EasyAssetBundle.v1 {

    public static class TreeHelper {

        //=======================================================
        // public
        //=======================================================

        public static List<EditorAssetInfo> GetFileTreeViewItems( string assetPath ) {
            var data = new List<EditorAssetInfo>();

            var dirInfo = getAssetBundleDirectoryInfo( assetPath );
            if( dirInfo != null ) {
                var root = EditorAssetInfo.Root;
                data.Add( root );
                data.AddRange( buildTreeViewItemRecursive( dirInfo, root, -1 ) );
            }
            return data;
        }

        public static Texture2D GetTexture( string extension, string assetPath ) {
            switch( extension ) {
                case "":
                    return GUIHelper.Textures.FolderIcon;
                case ".ai":
                case ".png":
                case ".bmp":
                case ".eps":
                case ".ico":
                case ".icon":
                case ".jpeg":
                case ".jpg":
                case ".tex":
                case ".tga":
                case ".psd": {
                        if( AssetDatabase.LoadAssetAtPath<Sprite>( assetPath ) != null )
                            return GUIHelper.Textures.SpriteIcon;
                        return GUIHelper.Textures.TextureIcon;
                    }
                case ".aac":
                case ".aif":
                case ".aiff":
                case ".au":
                case ".mid":
                case ".midi":
                case ".mp3":
                case ".mpa":
                case ".ra":
                case ".ram":
                case ".wma":
                case ".wav":
                case ".wave":
                case ".ogg":
                    return GUIHelper.Textures.AudioClipIcon;
                case ".csv":
                case ".txt":
                case ".json":
                    return GUIHelper.Textures.TextAssetIcon;
                case ".prefab":
                    return GUIHelper.Textures.PrefabNormalIcon;
                case ".fbx":
                case ".mesh":
                case ".obj":
                    return GUIHelper.Textures.PrefabModelIcon;
                case ".asset":
                    return GUIHelper.Textures.ScriptableObjectIcon;
                case ".controller":
                    return GUIHelper.Textures.AnimatorIcon;
                case ".anim":
                    return GUIHelper.Textures.AnimationClipIcon;
                case ".mat":
                    return GUIHelper.Textures.MaterialIcon;
                case ".unity":
                    return GUIHelper.Textures.SceneIcon;
                case ".guiskin":
                    return GUIHelper.Textures.GUISkinIcon;
                case ".ttf":
                case ".otf":
                case ".fon":
                case ".fnt":
                    return GUIHelper.Textures.FontIcon;
                case ".cs":
                    return GUIHelper.Textures.ScriptIcon;
                case ".shader":
                    return GUIHelper.Textures.ShaderIcon;
                case ".spriteatlas":
                    return GUIHelper.Textures.SpriteAtlasIcon;
                default:
                    return null;
            }
        }

        //=======================================================
        // private
        //=======================================================
        private static int idCounter = 0;

        private static DirectoryInfo getAssetBundleDirectoryInfo( string assetPath ) {
            idCounter = 0;
            var path = FileHelper.AssetPathToSystemPath( assetPath );
            if( !Directory.Exists( path ) ) {
                Debug.LogError( "not found: " + path );
                return null;
            }

            return new DirectoryInfo( path );
        }

        private static List<EditorAssetInfo> buildTreeViewItemRecursive( DirectoryInfo dirInfo, EditorAssetInfo parent, int depth ) {
            var items = new List<EditorAssetInfo>();
            var myRoot = getTreeViewItem( FileHelper.SystemPathToAssetPath( dirInfo.FullName ), depth );
            myRoot.Initialize( FileHelper.SystemPathToAssetPath( dirInfo.FullName ) );
            if( depth != -1 )
                items.Add( myRoot );

            var fileInfos = dirInfo.GetFiles( "*.*" ).ToList();
            fileInfos.RemoveAll( f => Path.GetExtension( f.Name ).Equals( ".meta" ) );
            for( int i = 0; i < fileInfos.Count; i++ ) {
                var assetPath = FileHelper.SystemPathToAssetPath( fileInfos[ i ].FullName );
                var item = getTreeViewItem( assetPath, depth + 1 );
                item.Initialize( assetPath );
                items.Add( item );
            }
            //myRoot.children = items;

            foreach( DirectoryInfo di in dirInfo.GetDirectories() )
                items.AddRange( buildTreeViewItemRecursive( di, myRoot, depth + 1 ) );

            return items;
        }

        private static EditorAssetInfo getTreeViewItem( string path, int depth ) {
            return new EditorAssetInfo( Path.GetFileNameWithoutExtension( path ), depth, ++idCounter );
        }

    }

    internal static class MyExtensionMethods {
        public static IOrderedEnumerable<T> Order<T, TKey>( this IEnumerable<T> source, Func<T, TKey> selector, bool ascending ) {
            if( ascending ) {
                return source.OrderBy( selector );
            } else {
                return source.OrderByDescending( selector );
            }
        }

        public static IOrderedEnumerable<T> ThenBy<T, TKey>( this IOrderedEnumerable<T> source, Func<T, TKey> selector, bool ascending ) {
            if( ascending ) {
                return source.ThenBy( selector );
            } else {
                return source.ThenByDescending( selector );
            }
        }
    }

}