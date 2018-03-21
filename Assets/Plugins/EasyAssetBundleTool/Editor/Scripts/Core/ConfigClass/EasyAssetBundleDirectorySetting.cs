using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace charcolle.Utility.EasyAssetBundle.v1 {

    [Serializable]
    internal class DirectoryLabelSetting {

        [SerializeField]
        internal string DirectoryName;
        [SerializeField]
        internal string AssetBundleName;
        [SerializeField]
        internal DirectoryLabelNameType Type;
        [SerializeField]
        internal bool IsActive;

        internal DirectoryLabelSetting() { }

        internal DirectoryLabelSetting( DirectoryLabelSetting copy ) {
            DirectoryName   = copy.DirectoryName;
            AssetBundleName = copy.AssetBundleName;
            Type            = copy.Type;
            IsActive        = copy.IsActive;
        }

        internal bool CheckLabel( string buildPath ) {
            if( !IsActive || string.IsNullOrEmpty( AssetBundleName ) )
                return false;

            switch( Type ) {
                case DirectoryLabelNameType.DirectoryName:
                    var dir = Path.GetFileName( buildPath );
                    if( dir.Equals( DirectoryName ) )
                        return true;
                    return false;
                case DirectoryLabelNameType.RelativePath:
                    if( buildPath.Equals( DirectoryName ) )
                        return true;
                    return false;
                default:
                    return false;
            }
        }

    }

}