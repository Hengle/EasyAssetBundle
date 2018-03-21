using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace charcolle.Utility.EasyAssetBundle.v1 {

    internal class EasyAssetBundleBuildProcessor {
        internal BuildProgress Progress;
        internal BuildProgress ProgressEnd;

        private EasyAssetBundleBuilderData buildData;
        private EasyAssetBundleBuildConfig config;
        private List<EditorAssetInfo> assetList;
        private List<EditorAssetInfo> versionAssetList;

        private Dictionary<string, List<EditorAssetInfo>> assetListDic;
        private Dictionary<string, List<EditorAssetInfo>> versionAssetListDic;

        private IAssetBundleList assetListProcess;
        private IAssetBundleExporter assetExportProcess;
        private IAssetBundleUploader assetUploadProcess;
        private BuildTarget[] platform;

        internal event Action OnBuildProcessEnd;

        internal EasyAssetBundleBuildProcessor( EasyAssetBundleBuilderData data, BuildProgress start = BuildProgress.PickUpBuildAssets, BuildProgress end = BuildProgress.UploadAssetBundle ) {
            Progress = start;
            ProgressEnd = end;
            buildData = data;

            config = data.Config;
            assetList = buildData.BuildAssets;
            versionAssetList = buildData.VersionAssets;
            assetListProcess = ( IAssetBundleList )buildData;
            assetExportProcess = ( IAssetBundleExporter )buildData;
            assetUploadProcess = ( IAssetBundleUploader )buildData;
        }

        internal void Build( string endMessage = "Build Complete" ) {
            platform = GetPlatform();
            if( platform == null || platform.Length == 0 )
                throw new ArgumentException( "no platform is selected." );

            var allProgress = Enum.GetValues( typeof( BuildProgress ) ).Length;

            try {
                for( int i = ( int )Progress; i <= ( int )ProgressEnd; i++ ) {
                    Progress = ( BuildProgress )i;

                    EditorUtility.DisplayProgressBar( "EasyAssetBundle", string.Format( "Progress at < {0} >", Progress.ToString() ), ( float )i / ( float )allProgress );
                    buildProcessor( Progress );
                }

                Debug.Log( endMessage );
            }
            catch( Exception ex ) {
                Debug.LogError( "AssetBundle BuildProcess results in failure. \n" + ex );
                Debug.LogError( string.Format( "Progress end at < {0} >.", Progress.ToString() ) );
            }

            EditorUtility.ClearProgressBar();
            if( OnBuildProcessEnd != null )
                OnBuildProcessEnd();
        }

        private List<EditorAssetInfo> buildAssets;

        private void buildProcessor( BuildProgress progress ) {
            switch( progress ) {
                case BuildProgress.PickUpBuildAssets:
                    pickupBuildAssets();
                    break;

                case BuildProgress.NameAssetLabel:
                    NameAssetLabel();
                    break;

                case BuildProgress.BuildAssetBundle:
                    BuildAssetBundle();
                    break;

                case BuildProgress.UnNameAssetLabel:
                    UnNameAssetLabel();
                    break;

                case BuildProgress.ExportAssetBundle:
                    ExportAssetBundle();
                    break;

                case BuildProgress.UpdateVersionFile:
                    CheckVersionFile();
                    UpdateVersionFile();
                    break;

                case BuildProgress.SaveVersionFile:
                    SaveVersionFile();
                    break;

                case BuildProgress.UploadAssetBundle:
                    UploadAssetBundle();
                    break;
            }
        }

        #region build process

        //=======================================================
        // process
        //=======================================================

        /// <summary>
        /// pick up assets for build from EditorAssetInfo.
        /// </summary>
        private void pickupBuildAssets() {
            if( assetList.Count( a => !a.IsAvailable ) > 0 ) {
                if( !EditorUtility.DisplayDialog( "EasyAssetBundle", "Some assets has invalid name.\nAre you sure to continue build?", "ok", "cancel" ) )
                    throw new Exception( "This build is canceled." );
            }

            buildAssets = assetList.Where( a => a.IsBuild ).ToList();
            if( buildAssets == null || buildAssets.Count == 0 )
                throw new ArgumentException( "No assets to buid." );
        }

        private void NameAssetLabel() {
            for( int i = 0; i < buildAssets.Count; i++ ) {
                var asset = AssetImporter.GetAtPath( buildAssets[ i ].AssetPath );
                asset.assetBundleName = buildAssets[ i ].AssetBundleName;
            }
        }

        private string assetBundleCachePath;

        private void BuildAssetBundle() {
            var now = DateTime.Now;
            var dateTime = string.Format( "{0}{1}{2}{3}{4}{5}", now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second );
            assetBundleCachePath = Path.Combine( config.ExportConfig.AssetBundleCachePath, dateTime );

            for( int i = 0; i < platform.Length; i++ ) {
                Debug.Log( string.Format( "Build for {0} ...", platform[ i ].ToString() ) );

                var outputPath = Path.Combine( assetBundleCachePath, platform[ i ].ToString() );
                Directory.CreateDirectory( outputPath );

                BuildPipeline.BuildAssetBundles( outputPath, config.Options, platform[ i ] );
            }
        }

        private void UnNameAssetLabel() {
            for( int i = 0; i < buildAssets.Count; i++ ) {
                var asset = AssetImporter.GetAtPath( buildAssets[ i ].AssetPath );
                asset.assetBundleName = "";
            }

            AssetDatabase.Refresh();
        }

        private void ExportAssetBundle() {
            var exportPath = config.ExportConfig.ExportAssetBundlePath;
            var type = config.ExportConfig.ExportType;
            var exportWithManifest = config.ExportConfig.ExportWithManifest;
            assetExportProcess.ExportAssetBundle( buildAssets, exportPath, assetBundleCachePath, platform, type, exportWithManifest );

            for( int j = 0; j < buildAssets.Count; j++ ) {
                buildAssets[ j ].CRC = new Dictionary<string, uint>();
                buildAssets[ j ].Hash = new Dictionary<string, Hash128>();
                for( int i = 0; i < platform.Length; i++ ) {
                    var outputPath = Path.Combine( assetBundleCachePath, platform[ i ].ToString() );
                    var platformName = platform[ i ].ToString();
                    var targetPath = FileHelper.pathSlashFix( Path.Combine( outputPath, buildAssets[ j ].AssetBundleName ) );
                    if( !File.Exists( targetPath ) )
                        continue;
                    uint crc;
                    Hash128 hash;
                    BuildPipeline.GetCRCForAssetBundle( targetPath, out crc );
                    BuildPipeline.GetHashForAssetBundle( targetPath, out hash );
                    buildAssets[ j ].CRC.Add( platformName, crc );
                    buildAssets[ j ].Hash.Add( platformName, hash );

                    var f = new FileInfo( targetPath );
                    buildAssets[ j ].Size = f.Length / 1024f / 1024f;
                }
            }
        }

        /// <summary>
        /// construct buildAsset-Dic and versionAsset-Dic
        /// </summary>
        private void CheckVersionFile() {
            if( !config.AssetBundleListConfig.IsConfigAvailable )
                return;

            var date = DateTime.Now.ToString();
            assetListDic = new Dictionary<string, List<EditorAssetInfo>>();
            versionAssetListDic = new Dictionary<string, List<EditorAssetInfo>>();

            for( int i = 1; i < assetList.Count; i++ ) {
                var asset = assetList[ i ];
                if( string.IsNullOrEmpty( asset.AssetBundleName ) )
                    continue;
                asset.Date = date;

                var list = new List<EditorAssetInfo>();
                assetListDic.TryGetValue( asset.AssetBundleName, out list );
                if( list == null ) {
                    if( asset.IsBuild ) {
                        var l = new List<EditorAssetInfo>();
                        l.Add( asset );
                        assetListDic.Add( asset.AssetBundleName, l );
                    }
                } else {
                    if( asset.IsAsset )
                        assetListDic[ asset.AssetBundleName ].Add( asset );
                }
            }

            for( int i = 1; i < versionAssetList.Count; i++ ) {
                var asset = versionAssetList[ i ];
                var list = new List<EditorAssetInfo>();
                versionAssetListDic.TryGetValue( asset.AssetBundleName, out list );
                if( list == null ) {
                    var l = new List<EditorAssetInfo>();
                    l.Add( asset );
                    versionAssetListDic.Add( asset.AssetBundleName, l );
                } else {
                    versionAssetListDic[ asset.AssetBundleName ].Add( asset );
                }
            }
        }

        private void UpdateVersionFile() {
            if( !config.AssetBundleListConfig.IsConfigAvailable )
                return;

            assetListProcess.UpdateAssetBundleList( assetListDic, versionAssetListDic );
        }

        private void SaveVersionFile() {
            if( !config.AssetBundleListConfig.IsConfigAvailable )
                return;

            var savePath = buildData.Config.GetAssetBundleListSavePath( buildData.Config.AssetBundleListConfig.CurrentAssetBundleListName );
            if( File.Exists( savePath ) ) {
                if( !EditorUtility.DisplayDialog( "EasyAssetBundle", string.Format( "The AssetBundleList is already exists.\nAre you sure you want to overwrite? \n{0}", savePath ), "ok", "cancel" ) )
                    throw new Exception( "EasyAssetBundle: Cancel saving AssetBundleList." );
                File.Delete( savePath );
            }
            assetListProcess.SaveAssetBundleList( versionAssetList, savePath );
        }

        private void UploadAssetBundle() {
            if( !config.UploadConfig.IsConfigAvailable )
                return;

            assetUploadProcess.UploadAssetBundleList();
        }

        #endregion build process

        //=======================================================
        // utility
        //=======================================================

        private BuildTarget[] GetPlatform() {
            var platforms = BuildTargetUtility.GetPlatforms();
            var selectedPlatform = new List<BuildTarget>();
            for( int i = 0; i < platforms.Length; i++ ) {
                if( ( config.Platform & ( 1 << i ) ) == ( 1 << i ) )
                    selectedPlatform.Add( platforms[ i ] );
            }
            return selectedPlatform.ToArray();
        }
    }
}