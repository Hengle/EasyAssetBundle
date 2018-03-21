using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using charcolle.Utility.EasyAssetBundle.v1;
using charcolle.EasyAssetBundle.Sample;

public class EasyAssetBundleSampleBuilder : EasyAssetBundleBuilderData {

    private SampleAssetBundleInfoData Data;

    //=======================================================
    // AssetBundleList
    //=======================================================

    /// <summary>
    /// Convert your custom assetbundlelist to EditorAssetInfo.
    /// Use VersionAssets
    /// </summary>
    public override List<EditorAssetInfo> LoadAssetBundleList( string text ) {
        var versionList = new List<EditorAssetInfo>();

        var assetList = text.Split( "\n"[ 0 ] );
        for( int i = 1; i < assetList.Length; i++ ) {
            var asset = new EditorAssetInfo( "", 0, i );
            var info = assetList[ i ].Split( ","[ 0 ] );
            if( info == null || info.Length < 6 )
                break;

            try {
                asset.Name            = info[ 0 ];
                asset.AssetPath       = info[ 1 ];
                asset.AssetBundleName = info[ 2 ];
                asset.Extension       = info[ 3 ];
                asset.Version         = int.Parse( info[ 4 ] );
                asset.Size            = float.Parse( info[ 5 ] );
                asset.Date            = info[ 6 ];
                asset.UnityVersion    = info[ 7 ];
                asset.name            = asset.AssetPath;
                asset.Initialize( asset.AssetPath );

                versionList.Add( asset );
            }
            catch( Exception ex ) {
                Debug.LogError( ex );
            }
        }
        return versionList;
    }

    /// <summary>
    /// Convert EditorAssetInfo to your custom assetbundleList.
    /// NOTE: versionAssets begin at index:1
    /// </summary>
    public override void SaveAssetBundleList( List<EditorAssetInfo> versionAsset, string savePath ) {
        var sr = new StreamWriter( savePath, false, Encoding.UTF8 );

        var sb = new StringBuilder();
        sb.Append( "FileName,AssetPath,AssetBundle,Extension,Version,Size,Date,UnityVersion" );
        sb.Append( "\n" );
        for( int i = 1; i < versionAsset.Count; i++ ) {
            var asset = versionAsset[ i ];
            sb.Append( asset.Name );
            sb.Append( "," );
            sb.Append( asset.AssetPath );
            sb.Append( "," );
            sb.Append( asset.AssetBundleName );
            sb.Append( "," );
            sb.Append( asset.Extension );
            sb.Append( "," );
            sb.Append( asset.Version );
            sb.Append( "," );
            sb.Append( asset.Size );
            sb.Append( "," );
            sb.Append( asset.Date.ToString() );
            sb.Append( "," );
            sb.Append( asset.UnityVersion );
            sb.Append( "\n" );
        }

        sr.Write( sb.ToString() );
        sr.Close();
    }

    /// <summary>
    /// Convert EditorAssetInfo to your custom assetbundleList.
    /// NOTE: versionAssets begin at index:1
    /// </summary>
    public override void ConvertAssetBundleList( List<EditorAssetInfo> versionAsset, string savePath ) {

        var convertData = ScriptableObject.CreateInstance<SampleAssetBundleInfoData>();

        convertData.Info = new List<SampleAssetInfo>();
        for( int i = 1; i < versionAsset.Count; i++ ) {
            var asset = VersionAssets[ i ];
            var assetinfo = new SampleAssetInfo();
            assetinfo.Name = asset.Name;
            assetinfo.Path = asset.AssetPath;
            assetinfo.Extension = asset.Extension;
            assetinfo.AssetBundleName = asset.AssetBundleName;
            assetinfo.Version = asset.Version;
            assetinfo.IsAssetBundle = !asset.IsAsset;
            convertData.Info.Add( assetinfo );
        }

        AssetDatabase.CreateAsset( convertData, savePath );
    }

    /// <summary>
    /// Use this if you want to customize export-process.
    /// </summary>
    //public override void UpdateAssetBundleList( List<EditorAssetInfo> Assets ) {
    //    //throw new NotImplementedException();
    //    base.UpdateAssetBundleList( Assets );
    //}

    //=======================================================
    // Exporter
    //=======================================================

    /// <summary>
    /// Use this if you want to customize export-process.
    /// </summary>
    //public override void ExportAssetBundle( List<EditorAssetInfo> buildAssets, string exportPath, string cachePath, BuildTarget[] platforms, ExportType type, bool exportWithManifest ) {
    //    throw new NotImplementedException();
    //}

    /// <summary>
    /// Use this if you want to export assetbundleList to specified location.
    /// </summary>
    public override void ExportVersionAssetBundleList() {
        throw new NotImplementedException();
    }

    //=======================================================
    // Uploader
    //=======================================================

    /// <summary>
    /// Use this if you want to upload assetbundlelist to server.
    /// </summary>
    public override void UploadAssetBundleList() {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Use this if you want to upload assetbundlelist to server.
    /// </summary>
    public override void UploadResourceDataFile() {
        throw new NotImplementedException();
    }

}
