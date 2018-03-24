using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using Object     = UnityEngine.Object;
using TreeHelper = charcolle.Utility.EasyAssetBundle.v1.TreeHelper;

namespace charcolle.Utility.EasyAssetBundle.v1 {

    [Serializable]
    public class EditorAssetInfo : TreeElement {

        /// <summary>
        /// Whether this is a directory or a file.
        /// </summary>
        public bool IsAsset;
        /// <summary>
        /// Filename of this asset.
        /// </summary>
        public string Name;
        /// <summary>
        /// Path of this asset.
        /// </summary>
        public string AssetPath;
        /// <summary>
        /// Extension of this asset.
        /// </summary>
        public string Extension;
        /// <summary>
        /// Assetbundle name of this asset.
        /// </summary>
        public string AssetBundleName;
        /// <summary>
        /// File-size of assetbundle.
        /// </summary>
        public float Size;
        /// <summary>
        /// Version of this asset.
        /// </summary>
        public int Version;
        /// <summary>
        /// Guid of this asset.
        /// </summary>
        public string Guid;
        /// <summary>
        /// Date of last update this asset.
        /// </summary>
        public string Date;
        /// <summary>
        /// Version of UnityEditor.
        /// </summary>
        public string UnityVersion;
        /// <summary>
        /// key: platform, value: crc
        /// </summary>
        public Dictionary<string, uint> CRC     = new Dictionary<string, uint>();
        /// <summary>
        /// key: platform, value: hash
        /// </summary>
        public Dictionary<string, Hash128> Hash = new Dictionary<string, Hash128>();

        /// <summary>
        /// Editor Only
        /// </summary>
        public bool IsBuild;
        /// <summary>
        /// Editor Only
        /// </summary>
        public bool IsAvailable;
        /// <summary>
        /// Editor Only
        /// </summary>
        public bool IsSelected;
        /// <summary>
        /// Editor Only
        /// </summary>
        public bool IsDirectoryLabel;
        /// <summary>
        /// Editor Only
        /// </summary>
        public Texture2D Texture;
        /// <summary>
        /// Editor Only
        /// </summary>
        public int OldAssetVersion;
        /// <summary>
        /// Editor Only
        /// </summary>
        public string OldUnityVersion;

        public EditorAssetInfo( string name, int depth, int id ) : base( name, depth, id ) {
            Version         = 1;
            OldAssetVersion = 0;
            UnityVersion    = Application.unityVersion;
            OldUnityVersion = Application.unityVersion;
        }

        public EditorAssetInfo( EditorAssetInfo copy ) {
            IsAsset         = copy.IsAsset;
            Name            = copy.Name;
            AssetPath       = copy.AssetPath;
            Extension       = copy.Extension;
            AssetBundleName = copy.AssetBundleName;
            Version         = copy.Version;
            CRC             = copy.CRC;
            Guid            = copy.Guid;
            Hash            = copy.Hash;
            Size            = copy.Size;
            Date            = copy.Date;
            UnityVersion    = copy.UnityVersion;
        }

        public void Initialize( string assetPath ) {
            AssetPath   = assetPath;
            Name         = System.IO.Path.GetFileNameWithoutExtension( AssetPath );
            Extension    = System.IO.Path.GetExtension( AssetPath );
            IsAsset      = !AssetDatabase.IsValidFolder( assetPath );
            Texture      = TreeHelper.GetTexture( Extension, AssetPath );
            Guid         = AssetDatabase.AssetPathToGUID( AssetPath );
        }

        public AssetBundleBuild GetAssetBundleBuild() {
            var build             = new AssetBundleBuild();
            build.assetBundleName = AssetBundleName;
            build.assetNames      = new string[] { Name };
            return build;
        }

        public void OnSelected() {
            EditorGUIUtility.PingObject( AssetDatabase.LoadAssetAtPath( AssetPath, typeof( Object ) ) );
        }

        public static EditorAssetInfo Root {
            get {
                var root = new EditorAssetInfo( "Root", -1, 0 );
                root.IsAvailable = true;
                return root;
            }
        }

    }

}