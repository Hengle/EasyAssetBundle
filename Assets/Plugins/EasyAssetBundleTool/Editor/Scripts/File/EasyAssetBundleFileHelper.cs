using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

using Object = UnityEngine.Object;

namespace charcolle.Utility.EasyAssetBundle.v1 {

    internal static class FileHelper {

        private const string SEARCH_ROOT           = "EasyAssetBundleFileHelper";
        private const string SEARCH_CONFIG         = "EasyAssetBundleBuilderData";

        private const string RELATIVEPATH_SAVEDATA = "Editor/Data/";
        private const string RELATIVEPATH_TEMPLATE = "Editor/Template/";

        //=======================================================
        // path
        //=======================================================

        internal static string RootPath {
            get {
                var assembly = Assembly.GetExecutingAssembly().GetName().Name;
                var search = assembly.Contains( "Assembly" ) ? SEARCH_ROOT : assembly;

                var guid = getAssetGUID( search );

                if( string.IsNullOrEmpty( guid ) ) {
                    Debug.LogError( "fatal error." );
                    return null;
                }

                var filePath   = Path.GetDirectoryName( AssetDatabase.GUIDToAssetPath( guid ) );
                var scriptPath = Path.GetDirectoryName( filePath );
                var editorPath = Path.GetDirectoryName( scriptPath );
                var rootPath   = Path.GetDirectoryName( editorPath );

                return pathSlashFix( rootPath );
            }
        }

        internal static string DataPath {
            get {
                return pathSlashFix( Path.Combine( RootPath, RELATIVEPATH_SAVEDATA ) );
            }
        }

        internal static string TemplatePath {
            get {
                return pathSlashFix( Path.Combine( RootPath, RELATIVEPATH_TEMPLATE + "EasyAssetBundleBuilderScript.template" ) );
            }
        }

        //=======================================================
        // Data Load
        //=======================================================

        internal static List<EasyAssetBundleBuilderData> LoadAssetBundleConfigs() {
            return FindAssetsByType<EasyAssetBundleBuilderData>( SEARCH_CONFIG );
        }

        internal static void CreateNewAssetBundleBuildConfig( Type builderType, string builderTypeName, string BuildConfigName ) {
            if( !Directory.Exists( DataPath ) )
                Directory.CreateDirectory( DataPath );
            var savePath = pathSlashFix( Path.Combine( DataPath, BuildConfigName + ".asset" ) );
            if( File.Exists( AssetPathToSystemPath( savePath ) ) ) {
                Debug.LogError( "EasyAssetBundle: File already exists. :" + savePath );
                return;
            }
            var saveData = ScriptableObject.CreateInstance( builderType ) as EasyAssetBundleBuilderData;
            saveData.Config = new EasyAssetBundleBuildConfig( builderTypeName );
            AssetDatabase.CreateAsset( saveData, savePath );
            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();
        }

        private readonly static string REPLACE_CLASSNAME = "#CLASS#";
        internal static void CreateNewBuilderScript( string scriptName ) {
            var script = "";
            try {
                StreamReader sr = new StreamReader( TemplatePath, Encoding.UTF8 );
                script = sr.ReadToEnd();
                sr.Close();
            } catch( Exception ex ) {
                Debug.LogError( "EasyAssetBundle: Cannot read the script template. : " + ex );
                return;
            }

            var savePath = EditorUtility.SaveFilePanel( "Create EasyAssetBundleBuilder Script", Application.dataPath, scriptName, "cs" );
            if( !string.IsNullOrEmpty( savePath ) ) {
                if( File.Exists( savePath ) ) {
                    Debug.LogWarning( "EasyAssetBundle: File already exists. :" + savePath );
                    return;
                }

                if( !savePath.Contains( "/Editor/" ) ) {
                    Debug.LogWarning( "EasyAssetBundle: You must create a converter script at Editor Folder." );
                    return;
                }

                script = script.Replace( REPLACE_CLASSNAME, scriptName );
                try {
                    StreamWriter sw = new StreamWriter( savePath, false, Encoding.UTF8 );
                    sw.Write( script );
                    sw.Close();
                    AssetDatabase.Refresh();
                }
                catch( Exception ex ) {
                    Debug.LogError( "SpreadSheetConvertTool: Cannot save the script. : " + ex );
                }
            }

        }

        internal static void OpenInEditor( string scriptName, int scriptLine = 0 ) {
            var searchFilter = "t:Script " + scriptName;
            var guids = AssetDatabase.FindAssets( searchFilter );

            for( int i = 0; i < guids.Length; i++ ) {
                var path = AssetDatabase.GUIDToAssetPath( guids[ i ] );
                string fileName = Path.GetFileNameWithoutExtension( path );
                if( fileName.Equals( scriptName ) ) {
                    MonoScript script = AssetDatabase.LoadAssetAtPath( path, typeof( MonoScript ) ) as MonoScript;
                    if( script != null ) {
                        if( !AssetDatabase.OpenAsset( script, scriptLine ) ) {
                            Debug.LogWarning( "Couldn't open script : " + scriptName );
                        }
                        break;
                    } else {
                        Debug.LogError( script );
                    }
                    break;
                }
            }
        }

        internal static void DeleteBuilderData( EasyAssetBundleBuilderData data ) {
            var path = AssetDatabase.GetAssetPath( data );
            AssetDatabase.DeleteAsset( path );
            AssetDatabase.Refresh();
        }

        //=======================================================
        // utility
        //=======================================================

        /// <summary>
        /// get path of dropped file
        /// </summary>
        internal static string GetDraggedObject( Event curEvent, Rect dropArea ) {
            int ctrlID = GUIUtility.GetControlID( FocusType.Passive );
            switch( curEvent.type ) {
                case EventType.DragUpdated:
                case EventType.DragPerform:
                    if( !dropArea.Contains( curEvent.mousePosition ) )
                        break;

                    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                    DragAndDrop.activeControlID = ctrlID;

                    if( curEvent.type == EventType.DragPerform ) {
                        DragAndDrop.AcceptDrag();
                        foreach( var draggedObj in DragAndDrop.objectReferences )
                            return AssetDatabase.GetAssetPath( draggedObj );
                    }
                    curEvent.Use();
                    break;
            }
            return null;
        }

        internal static string AssetPathToSystemPath( string path ) {
            return pathSlashFix( Path.Combine( dataPathWithoutAssets, path ) );
        }

        internal static string SystemPathToAssetPath( string path ) {
            return pathSlashFix( path ).Replace( Application.dataPath, "Assets" );
        }

        internal static string AssetPathToBuildPath( string buildPath, string relativePath ) {
            if( string.IsNullOrEmpty( buildPath ) || !relativePath.Contains( buildPath ) ) {
                Debug.LogWarning( "This directory is not at buildPath. " + buildPath + "\n" + relativePath );
                return "";
            }
            return relativePath.Replace( buildPath + "/", "" );
        }

        internal static string SystemPathToBuildPath( string buildPath, string systemPath ) {
            return null;
        }

        internal static List<FileInfo> AllFilesInDirectory( string directorySystemPath, string searchFilter ) {
            var d = new DirectoryInfo( directorySystemPath );
            var files = d.GetFiles( searchFilter ).ToList();
            foreach( DirectoryInfo di in d.GetDirectories() )
                files.AddRange( AllFilesInDirectory( di.FullName, searchFilter ) );

            return files;
        }

        internal static void CreateParentDirectory( string path ) {
            var dir = Path.GetDirectoryName( path );
            if( Directory.Exists( dir ) )
                return;

            if( !Directory.Exists( dir ) )
                CreateParentDirectory( dir );

            Debug.Log( dir );
            Directory.CreateDirectory( dir );
        }

        internal static string GetLastestFile( string path, SearchOption option = SearchOption.TopDirectoryOnly ) {
            var files = Directory.GetFiles( path, "*", option );
            if( files == null || files.Length == 0 )
                return null;

            var filePath = files.OrderByDescending( f => File.GetLastWriteTime( f ) ).First();
            return filePath;
        }

        private const string forwardSlash = "/";
        private const string backSlash = "\\";
        internal static string pathSlashFix( string path ) {
            return path.Replace( backSlash, forwardSlash );
        }

        //=======================================================
        // utility
        //=======================================================

        private static T FindAssetByType<T>( string type ) where T : Object {
            var searchFilter = "t:" + type;
            var guid = getAssetGUID( searchFilter );
            if( string.IsNullOrEmpty( guid ) )
                return null;
            var assetPath = AssetDatabase.GUIDToAssetPath( guid );
            return AssetDatabase.LoadAssetAtPath<T>( assetPath );
        }

        private static List<T> FindAssetsByType<T>( string type ) where T : Object {
            var searchFilter = "t:" + type;
            var guids = AssetDatabase.FindAssets( searchFilter );
            if( guids == null || guids.Length == 0 ) {
                return null;
            }
            var list = new List<T>();
            for( int i = 0; i < guids.Length; i++ ) {
                var assetPath = AssetDatabase.GUIDToAssetPath( guids[ i ] );
                list.Add( AssetDatabase.LoadAssetAtPath<T>( assetPath ) );
            }
            return list;
        }

        private static string getAssetGUID( string searchFilter ) {
            var guids = AssetDatabase.FindAssets( searchFilter );
            if( guids == null || guids.Length == 0 ) {
                return null;
            }

            if( guids.Length > 1 ) {
                //Debug.LogWarning( "more than one file was found." );
            }
            return guids[ 0 ];
        }

        private static string dataPathWithoutAssets {
            get {
                return pathSlashFix( Application.dataPath.Replace( "Assets", "" ) );
            }
        }

    }

}