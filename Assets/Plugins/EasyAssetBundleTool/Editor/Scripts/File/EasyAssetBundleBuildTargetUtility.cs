using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using UnityEditor;

namespace charcolle.Utility.EasyAssetBundle.v1 {

    internal static class BuildTargetUtility {

        private static List<BuildTarget> InstalledBuildModules = new List<BuildTarget>();
        private static string[] InstalledBuildModulesMenu;

        static BuildTargetUtility() {
            InstalledBuildModules = new List<BuildTarget>();

            Type ModuleManager = System.Type.GetType( "UnityEditor.Modules.ModuleManager,UnityEditor.dll" );
            MethodInfo IsPlatformSupportLoaded = ModuleManager.GetMethod( "IsPlatformSupportLoaded", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic );
            MethodInfo GetTargetStringFromBuildTarget = ModuleManager.GetMethod( "GetTargetStringFromBuildTarget", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic );
            foreach( BuildTarget target in Enum.GetValues( typeof( BuildTarget ) ) ) {
                if( ( bool )IsPlatformSupportLoaded.Invoke( null, new object[] { ( string )GetTargetStringFromBuildTarget.Invoke( null, new object[] { target } ) } ) )
                    InstalledBuildModules.Add( target );
            }

            Initialize();
        }

        private static void Initialize() {
            InstalledBuildModulesMenu = InstalledBuildModules.Select( e => Enum.GetName( typeof( BuildTarget ), e ) ).ToArray();
        }

        public static string[] GetBuildModulesMenu() {
            return InstalledBuildModulesMenu;
        }

        public static BuildTarget[] GetPlatforms() {
            return InstalledBuildModules.ToArray();
        }

    }



}