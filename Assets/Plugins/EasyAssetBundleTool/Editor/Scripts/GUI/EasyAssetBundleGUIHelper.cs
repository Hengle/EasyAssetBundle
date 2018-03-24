using UnityEngine;
using UnityEditor;

namespace charcolle.Utility.EasyAssetBundle.v1 {

    internal static class GUIHelper {

        internal static class Styles {

            static Styles() {
                NoSpace = new GUIStyle();
                NoSpace.border = new RectOffset( 0, 3, 0, 0 );
                NoSpace.margin = new RectOffset( 0, 0, 0, 0 );
                NoSpace.padding = new RectOffset( 1, 1, 1, 1 );

                NoSpaceBox = new GUIStyle( GUI.skin.box );
                NoSpaceBox.margin = new RectOffset( 0, 0, 0, 0 );
                NoSpaceBox.padding = new RectOffset( 1, 1, 1, 1 );

                NoOverflowButton = new GUIStyle( GUI.skin.button );
                NoOverflowButton.wordWrap = true;
                NoOverflowButton.alignment = TextAnchor.MiddleLeft;

                LabelWordWrap = new GUIStyle( GUI.skin.label );
                LabelWordWrap.wordWrap = true;
                LabelWordWrap.richText = true;

                TextFieldWordWrap = new GUIStyle( GUI.skin.textField );
                TextFieldWordWrap.wordWrap = true;

                LeftArea = new GUIStyle( "WindowBackground" );
                LeftArea.margin = new RectOffset( 0, 0, 0, 0 );
                LeftArea.padding = new RectOffset( 0, 0, 0, 0 );

                MenuList = new GUIStyle( "RL Background" );

                PlusButton = new GUIStyle( "OL Plus" );

                MinusButton = new GUIStyle( "OL Minus" );

                SearchField = new GUIStyle( "SearchTextField" );
                SearchFieldCancel = new GUIStyle( "SearchCancelButton" );
                SearchFieldToolBar = new GUIStyle( "ToolbarSeachTextField" );
                SearchFieldCancelToolBar = new GUIStyle( "ToolbarSeachCancelButton" );
            }

            public static GUIStyle NoSpace {
                get;
                private set;
            }

            public static GUIStyle NoSpaceBox {
                get;
                private set;
            }

            public static GUIStyle LabelWordWrap {
                get;
                private set;
            }

            public static GUIStyle TextFieldWordWrap {
                get;
                private set;
            }

            public static GUIStyle NoOverflowButton {
                get;
                private set;
            }

            public static GUIStyle LeftArea {
                get;
                private set;
            }

            public static GUIStyle MenuList {
                get;
                private set;
            }

            public static GUIStyle PlusButton {
                get;
                private set;
            }

            public static GUIStyle MinusButton {
                get;
                private set;
            }

            public static GUIStyle SceneButton {
                get;
                private set;
            }

            public static GUIStyle SearchField {
                get;
                private set;
            }

            public static GUIStyle SearchFieldCancel {
                get;
                private set;
            }

            public static GUIStyle SearchFieldToolBar {
                get;
                private set;
            }

            public static GUIStyle SearchFieldCancelToolBar {
                get;
                private set;
            }

        }

        internal static class Textures {

            static Textures() {
                ConfigAvailable      = EditorGUIUtility.Load( "greenLight" ) as Texture2D;
                ConfigInvalid        = EditorGUIUtility.Load( "redLight" ) as Texture2D;
                ConfigWarning        = EditorGUIUtility.Load( "orangeLight" ) as Texture2D;

                CheckMarkOK          = EditorGUIUtility.Load( "vcs_check" ) as Texture2D;
                CheckMaarkNG         = EditorGUIUtility.Load( "vcs_delete" ) as Texture2D;

                VersionNew = EditorGUIUtility.Load( "d_AS Badge New" ) as Texture2D;
                VersionUpdate = EditorGUIUtility.Load( "Refresh" ) as Texture2D;

                FolderIcon           = EditorGUIUtility.Load( "Folder Icon" ) as Texture2D;
                SceneIcon            = EditorGUIUtility.Load( "SceneAsset Icon" ) as Texture2D;
                TextureIcon          = EditorGUIUtility.Load( "Texture Icon" ) as Texture2D;
                SpriteIcon           = EditorGUIUtility.Load( "Sprite Icon" ) as Texture2D;
                AudioClipIcon        = EditorGUIUtility.Load( "AudioClip Icon" ) as Texture2D;
                TextAssetIcon        = EditorGUIUtility.Load( "TextAsset Icon" ) as Texture2D;
                PrefabNormalIcon     = EditorGUIUtility.Load( "PrefabNormal Icon" ) as Texture2D;
                PrefabModelIcon      = EditorGUIUtility.Load( "PrefabModel Icon" ) as Texture2D;
                ScriptableObjectIcon = EditorGUIUtility.Load( "ScriptableObject Icon" ) as Texture2D;
                AnimatorIcon         = EditorGUIUtility.Load( "AnimatorController Icon" ) as Texture2D;
                AnimationClipIcon    = EditorGUIUtility.Load( "AnimationClip Icon" ) as Texture2D;
                MaterialIcon         = EditorGUIUtility.Load( "Material Icon" ) as Texture2D;
                GUISkinIcon          = EditorGUIUtility.Load( "GUISkin Icon" ) as Texture2D;
                FontIcon             = EditorGUIUtility.Load( "Font Icon" ) as Texture2D;
                ScriptIcon           = EditorGUIUtility.Load( "cs Script Icon" ) as Texture2D;
                ShaderIcon           = EditorGUIUtility.Load( "Shader Icon" ) as Texture2D;
                SpriteAtlasIcon      = EditorGUIUtility.Load( "SpriteAtlas Icon" ) as Texture2D;
            }

            public static Texture2D ConfigAvailable {
                get;
                private set;
            }

            public static Texture2D ConfigWarning {
                get;
                private set;
            }

            public static Texture2D ConfigInvalid {
                get;
                private set;
            }

            public static Texture2D CheckMarkOK {
                get;
                private set;
            }

            public static Texture2D CheckMaarkNG {
                get;
                private set;
            }

            public static Texture2D VersionNew {
                get;
                private set;
            }

            public static Texture2D VersionUpdate {
                get;
                private set;
            }

            #region asset icon

            public static Texture2D FolderIcon {
                get;
                private set;
            }

            public static Texture2D SceneIcon {
                get;
                private set;
            }

            public static Texture2D TextureIcon {
                get;
                private set;
            }

            public static Texture2D SpriteIcon {
                get;
                private set;
            }

            public static Texture2D AudioClipIcon {
                get;
                private set;
            }

            public static Texture2D TextAssetIcon {
                get;
                private set;
            }

            public static Texture2D PrefabNormalIcon {
                get;
                private set;
            }

            public static Texture2D PrefabModelIcon {
                get;
                private set;
            }

            public static Texture2D ScriptableObjectIcon {
                get;
                private set;
            }

            public static Texture2D AnimatorIcon {
                get;
                private set;
            }

            public static Texture2D AnimationClipIcon {
                get;
                private set;
            }

            public static Texture2D MaterialIcon {
                get;
                private set;
            }

            public static Texture2D GUISkinIcon {
                get;
                private set;
            }

            public static Texture2D FontIcon {
                get;
                private set;
            }

            public static Texture2D ScriptIcon {
                get;
                private set;
            }

            public static Texture2D ShaderIcon {
                get;
                private set;
            }

            public static Texture2D SpriteAtlasIcon {
                get;
                private set;
            }

            #endregion

        }

        internal class Scope {

            public class DropSettingTextArea : GUI.Scope {

                public DropSettingTextArea( GUIContent label, ref string path ) {
                    EditorGUILayout.BeginVertical();

                    GUILayout.Label( label );
                    EditorGUILayout.BeginHorizontal();
                    {
                        path = EditorGUILayout.TextField( path );
                        var p = FileHelper.GetDraggedObject( Event.current, GUILayoutUtility.GetLastRect() );
                        if( !string.IsNullOrEmpty( p ) ) {
                            EditorGUIUtility.keyboardControl = 0;
                            path = FileHelper.AssetPathToSystemPath( p ) + "/";
                        }
                        if( GUILayout.Button( "Select", GUILayout.Width( 50 ) ) ) {
                            EditorGUIUtility.keyboardControl = 0;
                            var select = EditorUtility.OpenFolderPanel( string.Format( "Select {0} folder", label.text ), Application.dataPath, "" );
                            if( !string.IsNullOrEmpty( select ) ) {
                                path = select;
                            }
                        }
                    }
                    EditorGUILayout.EndHorizontal();
                }

                protected override void CloseScope() {
                    EditorGUILayout.EndVertical();
                }
            }

            public class DropSettingTextAreaInProject : GUI.Scope {

                public DropSettingTextAreaInProject( GUIContent label, ref string path ) {
                    EditorGUILayout.BeginVertical();

                    GUILayout.Label( label );
                    EditorGUILayout.BeginHorizontal();
                    {
                        path = EditorGUILayout.TextField( path );
                        var p = FileHelper.GetDraggedObject( Event.current, GUILayoutUtility.GetLastRect() );
                        if( !string.IsNullOrEmpty( p ) ) {
                            EditorGUIUtility.keyboardControl = 0;
                            path = p + "/";
                        }
                        if( GUILayout.Button( "Select", GUILayout.Width( 50 ) ) ) {
                            EditorGUIUtility.keyboardControl = 0;
                            var select = EditorUtility.OpenFolderPanel( string.Format( "Select {0} folder", label.text ), Application.dataPath, "" );
                            if( !string.IsNullOrEmpty( select ) ) {
                                if( select.Contains( Application.dataPath ) ) {
                                    path = FileHelper.SystemPathToAssetPath( select );
                                } else {
                                    Debug.LogWarning( "EasyAssetBundleConfig: Select the project folder." );
                                }
                            }
                        }
                    }
                    EditorGUILayout.EndHorizontal();
                }

                protected override void CloseScope() {
                    EditorGUILayout.EndVertical();
                }

            }

        }

    }

}