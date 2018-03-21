using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using GUIHelper = charcolle.Utility.EasyAssetBundle.v1.GUIHelper;
using UndoHelper = charcolle.Utility.EasyAssetBundle.v1.UndoHelper;

namespace charcolle.Utility.EasyAssetBundle.v1 {

    [Serializable]
    internal class EasyAssetBundleUploadConfig {

        [SerializeField]
        internal bool UseUploadAssetBundle;

        [SerializeField]
        internal string UploadAssetBundleURL;

        internal void Initialize() {　}

        internal bool IsConfigAvailable {
            get {
                return UseUploadAssetBundle;
            }
        }

        #region drawer
        //=======================================================
        // Drawer
        //=======================================================
        [SerializeField]
        private bool Fold = true;
        internal void OnGUI() {
            GUILayout.Space( 10 );

            EditorGUILayout.BeginVertical( EditorStyles.helpBox );
            {
                GUILayout.Space( 3 );

                EditorGUILayout.BeginHorizontal();
                {
                    var fold = EditorGUILayout.Foldout( Fold, "Upload Setting" );
                    if( fold != Fold ) {
                        EditorGUIUtility.keyboardControl = 0;
                    }
                    Fold = fold;
                    GUILayout.FlexibleSpace();
                    if( GUILayout.Button( IsConfigAvailable ? GUIHelper.Textures.ConfigAvailable : GUIHelper.Textures.ConfigWarning, GUIStyle.none, new GUILayoutOption[] { GUILayout.Width( 20 ), GUILayout.Height( 20 ) } ) ) {
                        CheckConfig();
                    }
                }
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
                {

                    GUILayout.Space( 17 );

                    EditorGUILayout.BeginVertical();
                    {
                        if( Fold ) {

                            EditorGUILayout.BeginHorizontal();
                            {
                                GUILayout.Label( new GUIContent( "Use Upload", "used for upload AssetBundle for server." ) );
                                UseUploadAssetBundle = EditorGUILayout.Toggle( UseUploadAssetBundle );
                            }
                            EditorGUILayout.EndHorizontal();

                            if( UseUploadAssetBundle ) {
                                GUILayout.Label( new GUIContent( "Upload AssetBundle URL", "" ) );
                                EditorGUILayout.TextField( UploadAssetBundleURL );

                            }

                            EditorGUILayout.HelpBox( "not implemented", MessageType.Warning );

                        }

                        GUILayout.Space( 2 );
                    }
                    EditorGUILayout.EndVertical();
                }
                EditorGUILayout.EndHorizontal();

                GUILayout.Space( 5 );

            }
            EditorGUILayout.EndVertical();

        }

        //=======================================================
        // Utility
        //=======================================================

        internal void CheckConfig() {
            Debug.Log( "Configのチェック" );
        }

        #endregion

    }
}