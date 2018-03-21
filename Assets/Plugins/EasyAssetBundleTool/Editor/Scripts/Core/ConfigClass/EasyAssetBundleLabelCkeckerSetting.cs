using UnityEngine;
using System;
using System.IO;
using System.Text.RegularExpressions;

namespace charcolle.Utility.EasyAssetBundle.v1 {

    [Serializable]
    internal class FileNameCkeckerSetting {

        [SerializeField]
        internal string DirectoryName;
        [SerializeField]
        internal DirectoryLabelNameType DirectoryType;
        [SerializeField]
        internal string RegexPattern;
        [SerializeField]
        internal LabelCheckerFileType FileType;
        [SerializeField]
        internal LabelCheckerType CheckerType;
        [SerializeField]
        internal bool IsActive;

        internal FileNameCkeckerSetting() { }

        internal FileNameCkeckerSetting( FileNameCkeckerSetting copy ) {
            DirectoryName = copy.DirectoryName;
            DirectoryType = copy.DirectoryType;
            RegexPattern  = copy.RegexPattern;
            FileType      = copy.FileType;
            IsActive      = copy.IsActive;
        }

        public bool IsMatch( string buildPath ) {
            if( !IsActive || string.IsNullOrEmpty( DirectoryName ) )
                return false;

            switch( DirectoryType ) {
                case DirectoryLabelNameType.DirectoryName:
                    var dir = Path.GetFileName( Path.GetDirectoryName( buildPath ) );
                    if( dir.Equals( DirectoryName ) )
                        return true;
                    return false;
                case DirectoryLabelNameType.RelativePath:
                    if( buildPath.Contains( DirectoryName ) )
                        return true;
                    return false;
                default:
                    return false;
            }
        }

        public bool CheckAvailable( string fileName, string extension ) {
            if( !IsActive )
                return true;

            try {
                var regex = new Regex( RegexPattern, RegexOptions.Compiled );
                switch( FileType ) {
                    case LabelCheckerFileType.FileName:
                        return checkerType( regex.IsMatch( fileName ) );
                    case LabelCheckerFileType.Extension:
                        return checkerType( regex.IsMatch( extension ) );
                }
            } catch { }
            return false;
        }

        private bool checkerType( bool match ) {
            switch( CheckerType ) {
                case LabelCheckerType.Ban:
                    return !match;
                case LabelCheckerType.Limit:
                    return match;
            }
            return true;
        }

    }

}