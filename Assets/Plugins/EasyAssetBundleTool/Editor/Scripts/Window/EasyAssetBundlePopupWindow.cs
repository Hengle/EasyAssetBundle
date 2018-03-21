using UnityEngine;
using UnityEditor;

using UndoHelper = charcolle.Utility.EasyAssetBundle.v1.UndoHelper;
using FileHelper = charcolle.Utility.EasyAssetBundle.v1.FileHelper;
using GUIHelper  = charcolle.Utility.EasyAssetBundle.v1.GUIHelper;

namespace charcolle.Utility.EasyAssetBundle.v1 {

    internal class EasyAssetBundlePopupWindow : PopupWindowContent {

        [SerializeField]
        private string newBuilderDataName = "New BuilderData";
        [SerializeField]
        private string newBuilderScriptName = "NewBuilderScript";

        [SerializeField]
        private int selectedType;

        public void Initialize() {
            selectedType = 0;
            BuilderDataEditorUtility.GetBuilderDataSubClass();
        }

        public override void OnGUI( Rect rect ) {
            GUI.skin.label.richText = true;
            GUILayout.Space( 5 );

            if( BuilderDataEditorUtility.BuilderDataMenu.Length > 0 ) {
                EditorGUILayout.BeginVertical( EditorStyles.helpBox );
                {
                    GUILayout.Label( new GUIContent( "Create New BuilderData".ToBold(), GUIHelper.Textures.ScriptableObjectIcon ), GUILayout.Height( 20 ) );

                    Undo.IncrementCurrentGroup();
                    UndoHelper.PopupWindowUndo( editorWindow, "Edit BuilderData name" );
                    newBuilderDataName = EditorGUILayout.TextField( newBuilderDataName );

                    var selected = EditorGUILayout.Popup( selectedType, BuilderDataEditorUtility.BuilderDataMenu );
                    if( selectedType != selected ) {
                        UndoHelper.PopupWindowUndo( editorWindow, "Change selected script" );
                        EditorGUIUtility.keyboardControl = 0;
                        selectedType = selected;
                    }

                    GUILayout.Space( 3 );

                    if( GUILayout.Button( "Create" ) ) {
                        FileHelper.CreateNewAssetBundleBuildConfig( BuilderDataEditorUtility.BuilderDataType[ selectedType ], BuilderDataEditorUtility.BuilderDataMenu[ selectedType ], newBuilderDataName );
                        EasyAssetBundleWindow.Initialize();
                        this.editorWindow.Close();
                    }
                }
                EditorGUILayout.EndVertical();
            } else {
                selectedType = 0;
                EditorGUILayout.HelpBox( "There is no BuilderScript.", MessageType.Warning );
            }

            GUILayout.Space( 5 );

            EditorGUILayout.BeginVertical( EditorStyles.helpBox );
            {
                GUILayout.Label( new GUIContent( "Create New BuilderScript".ToBold(), GUIHelper.Textures.ScriptIcon ), GUILayout.Height( 20 ) );

                Undo.IncrementCurrentGroup();
                UndoHelper.PopupWindowUndo( editorWindow, "Edit BuilderScript name" );
                newBuilderScriptName = EditorGUILayout.TextField( newBuilderScriptName );

                GUILayout.Space( 3 );

                if( GUILayout.Button( "Create" ) ) {
                    FileHelper.CreateNewBuilderScript( newBuilderScriptName );
                    this.editorWindow.Close();
                }
            }
            EditorGUILayout.EndVertical();

            GUI.skin.label.richText = false;
        }

        public override Vector2 GetWindowSize() {
            return new Vector2( 250f, 170f );
        }

    }
}