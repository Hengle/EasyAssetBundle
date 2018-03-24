using System;
using System.IO;

namespace charcolle.EasyAssetBundle.Sample {

    public static class EasyAssetBundleFileUtility {

        public static bool WriteAllBytes( string path, byte[] bytes ) {
            try {
                File.WriteAllBytes( path, bytes );
                return true;
            } catch {
                return false;
            }
        }

        public static bool WriteAllText( string path, string text ) {
            try {
                File.WriteAllText( path, text );
                return true;
            }
            catch {
                return false;
            }
        }

    }

}