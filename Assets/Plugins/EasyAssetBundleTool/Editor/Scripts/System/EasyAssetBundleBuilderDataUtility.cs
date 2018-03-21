using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;

namespace charcolle.Utility.EasyAssetBundle.v1 {

    internal static class BuilderDataEditorUtility {

        internal static List<string> BuilderDataTypeMenu = new List<string>();
        internal static List<Type> BuilderDataType       = new List<Type>();

        internal static void GetBuilderDataSubClass() {
            var types = Assembly
                        .GetAssembly( typeof( EasyAssetBundleBuilderData ) )
                        .GetTypes()
                        .Where( t => {
                            return t.IsSubclassOf( typeof( EasyAssetBundleBuilderData ) ) && !t.IsAbstract;
                        } );

            BuilderDataTypeMenu = new List<string>();
            BuilderDataType     = new List<Type>();

            BuilderDataTypeMenu = types.Select( t => t.Name ).ToList();
            BuilderDataType     = types.ToList();
        }

        internal static string[] BuilderDataMenu {
            get {
                return BuilderDataTypeMenu.ToArray();
            }
        }

        internal static string ToBold( this string str ) {
            return string.Format( "<b>{0}</b>", str );
        }

        internal static string ToMiddleBold( this string str ) {
            return string.Format( "<size=12><b>{0}</b></size>", str );
        }

    }

}