using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Threading.Tasks;

using charcolle.EasyAssetBundle.Sample;

public class SampleAssetBundleInfoData : ScriptableObject, ISerializationCallbackReceiver {

    public List<SampleAssetInfo> Info = new List<SampleAssetInfo>();
    public Dictionary<string, SampleAssetInfo> Resource = new Dictionary<string, SampleAssetInfo>();

    public void OnAfterDeserialize() {
        if( Info == null )
            return;

        Resource = new Dictionary<string, SampleAssetInfo>();
        for( int i = 0; i < Info.Count; i++ ) {
            try {
                Resource.Add( Info[ i ].Name, Info[ i ] );
            } catch ( Exception ex ) {
                Debug.LogError( "error : " + ex );
            }
        }
    }

    public void OnBeforeSerialize() { }

    public void CheckCacheFileExists( string cachePath ) {
        for( int i = 0; i < Info.Count; i++ ) {
            Info[ i ].LocalPath = cachePath + Info[ i ].Name;
            Info[ i ].IsExist = File.Exists( Info[i].LocalPath );
            Debug.Log( Info[ i ].LocalPath + " " + Info[i].IsExist );
        }
    }

    public SampleAssetInfo GetAssetInfo( string name ) {
        SampleAssetInfo info;
        Resource.TryGetValue( name, out info );
        if( info == null )
            Debug.LogError( "This file is not registered. : " + name );
        return info;
    }

}
