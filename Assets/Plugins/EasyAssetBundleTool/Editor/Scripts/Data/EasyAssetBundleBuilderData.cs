using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using charcolle.Utility.EasyAssetBundle.v1;

public abstract class EasyAssetBundleBuilderData : ScriptableObject, IAssetBundleExporter, IAssetBundleUploader, IAssetBundleList {

    public EasyAssetBundleBuildConfig Config;
    public List<EditorAssetInfo> BuildAssets   = new List<EditorAssetInfo>();
    public List<EditorAssetInfo> VersionAssets = new List<EditorAssetInfo>();

    public void Initialize() {
        Config.Initialize();
    }

    public EasyAssetBundleBuildConfig CurrentConfig {
        get {
            if( !Config.IsConfigAvailable ) {
                //Debug.LogWarning( "EasyAssetBundle: Check if your config is correct." );
                return null;
            }
            return new EasyAssetBundleBuildConfig( Config );
        }
    }

    public void ApplyConfig() {
        if( !Config.IsConfigAvailable ) {
            Debug.LogWarning( "EasyAssetBundle: Check if your config is correct." );
            return;
        }

        for( int i = 1; i < BuildAssets.Count; i++ ) {
            var asset = BuildAssets[ i ];
            asset.IsAvailable      = Config.CheckFileAvailable( asset, Config.AssetBundleBuildRootPath );
            asset.IsDirectoryLabel = Config.CheckDirectoryLabel( asset, Config.AssetBundleBuildRootPath );
            if( asset.IsDirectoryLabel )
                asset.IsBuild          = true;
            asset.OldAssetVersion = 0;
        }
    }

    public void ApplyVersionListConfig() {
        if( !Config.IsConfigAvailable || !Config.IsUseAssetBundleList )
            return;

        var assetDic = new Dictionary<string, int>();
        for( int i = 1; i < BuildAssets.Count; i++ ) {
            var asset = BuildAssets[ i ];
            if( !assetDic.ContainsKey( asset.Name ) ) {
                assetDic.Add( asset.Name, i );
            } else {
                Debug.LogWarning( "EasyAssetBundle: There is the same asset name. Try to change. " + asset.Name );
            }
        }
        for( int i = 1; i < VersionAssets.Count; i++ ) {
            var asset = VersionAssets[ i ];
            asset.IsAvailable = Config.CheckFileAvailable( asset, Config.AssetBundleBuildRootPath );
            asset.IsDirectoryLabel = Config.CheckDirectoryLabel( asset, Config.AssetBundleBuildRootPath );
            int idx = -1;
            assetDic.TryGetValue( asset.Name, out idx );
            if( idx != -1 ) {
                BuildAssets[ idx ].OldAssetVersion = asset.Version;
                BuildAssets[ idx ].OldUnityVersion    = asset.UnityVersion;
            }
        }
    }

    public void LoadNewestAssetBundleList() {
        if( !Config.IsConfigAvailable )
            return;
        if( Config.IsUseAssetBundleList ) {
            LoadAssetBundleListFile( FileHelper.GetLastestFile( Config.AssetBundleListConfig.AssetBundleListTextPath ) );
        }
    }

    //=======================================================
    // property
    //=======================================================

    public string CurrentAssetBundleListName {
        get {
            return Config.AssetBundleListConfig.CurrentAssetBundleListName;
        }
        set {
            Config.AssetBundleListConfig.CurrentAssetBundleListName = value;
        }
    }

    //=======================================================
    // build process
    //=======================================================

    public abstract List<EditorAssetInfo> LoadAssetBundleList( string text );
    public abstract void ConvertAssetBundleList( List<EditorAssetInfo> versionAssets, string savePath );
    public abstract void SaveAssetBundleList( List<EditorAssetInfo> versionAsset, string savePath );

    public abstract void ExportVersionAssetBundleList();
    public abstract void UploadAssetBundleList();
    public abstract void UploadResourceDataFile();

    public virtual void UpdateAssetBundleList( Dictionary<string, List<EditorAssetInfo>> assetListDic, Dictionary<string, List<EditorAssetInfo>> versionListDic ) {
        var date = DateTime.Now.ToString();
        foreach( var asset in assetListDic ) {
            var list = new List<EditorAssetInfo>();
            versionListDic.TryGetValue( asset.Key, out list );
            if( list == null ) {
                versionListDic.Add( asset.Key, asset.Value );
                for( int i = 0; i < asset.Value.Count; i++ ) {
                    asset.Value[ i ].Version = asset.Value[ 0 ].Version;
                    asset.Value[ i ].Date = date;
                }
            } else {
                // add new asset
                if( asset.Value.Count != list.Count ) {
                    for( int i = 0; i < asset.Value.Count; i++ ) {
                        if( asset.Value[ i ].OldAssetVersion == 0 ) {
                            asset.Value[ i ].Version = 0;
                            list.Add( asset.Value[ i ] );
                        }
                    }
                }

                for( int i = 0; i < list.Count; i++ ) {
                    list[ i ].Version = list[ i ].Version + 1;
                    list[ i ].Date    = date;
                    Debug.Log( "update : " + list[ i ].Version );
                }
            }
        }
        VersionAssets.Clear();
        var idcounter = 0;
        foreach( var asset in versionListDic ) {
            for( int i = 0; i < asset.Value.Count; i++ ) {
                var v = new EditorAssetInfo( asset.Value[ i ] );
                v.depth = 0;
                v.id = ++idcounter;
                VersionAssets.Add( v );
            }
        }
        VersionAssets.Insert( 0, EditorAssetInfo.Root );
    }

    public virtual void ExportAssetBundle( List<EditorAssetInfo> buildAssets, string exportPath, string cachePath, BuildTarget[] platforms, ExportType type, bool exportWithManifest ) {
        Dictionary<string, EditorAssetInfo> assetDic = new Dictionary<string, EditorAssetInfo>();
        if( type == ExportType.IgnoreDirectoryStructureWithVersion || type == ExportType.KeepDirectoryStructureWithVersion ) {
            for( int i = 0; i < buildAssets.Count; i++ ) {
                EditorAssetInfo asset = null;
                assetDic.TryGetValue( buildAssets[ i ].AssetBundleName, out asset );
                if( asset == null ) {
                    assetDic.Add( Path.GetFileName( buildAssets[ i ].AssetBundleName ), buildAssets[ i ] );
                }
            }
        }

        for( int i = 0; i < platforms.Length; i++ ) {
            var platform = platforms[ i ].ToString();
            var targetPath = FileHelper.pathSlashFix( Path.Combine( cachePath, platform ) );

            // pick up all of the files of the current platform folder
            var files = FileHelper.AllFilesInDirectory( targetPath, "*" );
            if( !exportWithManifest )
                files = files.Where( f => !Path.GetExtension( f.Name ).Equals( ".manifest" ) ).ToList();

            var dstDir = FileHelper.pathSlashFix( Path.Combine( exportPath, platform ) );
            if( !Directory.Exists( dstDir ) )
                Directory.CreateDirectory( dstDir );

            for( int j = 0; j < files.Count; j++ ) {
                if( files[ j ].Name.Equals( platform ) )
                    continue;
                var fileName = Path.GetFileNameWithoutExtension( files[ j ].Name );
                switch( type ) {
                    case ExportType.IgnoreDirectoryStructure: {
                            var src = FileHelper.pathSlashFix( files[ j ].Name );
                            var dst = FileHelper.pathSlashFix( Path.Combine( dstDir, src.Replace( targetPath + "/", "" ) ) );
                            if( File.Exists( dst ) )
                                File.Delete( dst );
                            FileUtil.CopyFileOrDirectory( files[ j ].FullName, dst );
                        }
                        break;
                    case ExportType.KeepDirectoryStructure: {
                            var src = FileHelper.pathSlashFix( files[ j ].FullName );
                            var dst = FileHelper.pathSlashFix( Path.Combine( dstDir, src.Replace( targetPath + "/", "" ) ) );
                            FileHelper.CreateParentDirectory( dst );
                            if( File.Exists( dst ) )
                                File.Delete( dst );
                            FileUtil.CopyFileOrDirectory( src, dst );
                        }
                        break;
                    case ExportType.IgnoreDirectoryStructureWithVersion: {
                            EditorAssetInfo assetInfo;
                            assetDic.TryGetValue( fileName, out assetInfo );
                            if( assetInfo != null ) {
                                var src = FileHelper.pathSlashFix( files[ j ].Name );
                                var dstDirWithVersion = FileHelper.pathSlashFix( Path.Combine( dstDir, assetInfo.Version.ToString() ) );
                                var dst = FileHelper.pathSlashFix( Path.Combine( dstDirWithVersion, src.Replace( targetPath + "/", "" ) ) );
                                FileHelper.CreateParentDirectory( dst );
                                if( File.Exists( dst ) )
                                    File.Delete( dst );
                                FileUtil.CopyFileOrDirectory( files[ j ].FullName, dst );
                            } else {
                                Debug.LogWarning( "EasyAssetBundle: Unknown file. : " + fileName );
                            }
                        }
                        break;
                    case ExportType.KeepDirectoryStructureWithVersion: {
                            EditorAssetInfo assetInfo;
                            assetDic.TryGetValue( fileName, out assetInfo );
                            if( assetInfo != null ) {
                                var src = FileHelper.pathSlashFix( files[ j ].FullName );
                                var dstDirWithVersion = FileHelper.pathSlashFix( Path.Combine( dstDir, assetInfo.Version.ToString() ) );
                                var dst = FileHelper.pathSlashFix( Path.Combine( dstDirWithVersion, src.Replace( targetPath + "/", "" ) ) );
                                FileHelper.CreateParentDirectory( dst );
                                if( File.Exists( dst ) )
                                    File.Delete( dst );
                                FileUtil.CopyFileOrDirectory( src, dst );
                            } else {
                                Debug.LogWarning( "EasyAssetBundle: Unknown file. :" + fileName );
                            }
                        }
                        break;
                }
            }
        }
    }

    //=======================================================
    // process
    //=======================================================

    private void LoadAssetBundleListFile( string assetBundleListPath ) {
        try {
            VersionAssets.Clear();
            VersionAssets.Add( EditorAssetInfo.Root );

            if( string.IsNullOrEmpty( assetBundleListPath ) )
                throw new FileNotFoundException();

            var sr = new StreamReader( assetBundleListPath );
            var text = sr.ReadToEnd();
            sr.Close();
            CurrentAssetBundleListName = FileHelper.pathSlashFix( Path.GetFileName( assetBundleListPath ) );

            var loadVersionList = LoadAssetBundleList( text );
            VersionAssets.AddRange( loadVersionList );
        }
        catch( Exception ex ) {
            Debug.LogError( "EasyAssetBundle: Fail to load AssetBundleList.\n" + ex );
        }
    }

}
