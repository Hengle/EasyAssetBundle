﻿using UnityEngine;
using UnityEditor;
using UnityEditor.IMGUI.Controls;

// this code from unity technologies tree view sample
// http://files.unity3d.com/mads/TreeViewExamples.zip

namespace charcolle.Utility.EasyAssetBundle.v1 {

    internal class EasyAssetBundleMultiColumnHeader : MultiColumnHeader {
        Mode m_Mode;

        public enum Mode {
            LargeHeader,
            DefaultHeader,
            MinimumHeaderWithoutSorting
        }

        public EasyAssetBundleMultiColumnHeader( MultiColumnHeaderState state )
            : base( state ) {
            mode = Mode.DefaultHeader;
        }

        public Mode mode {
            get {
                return m_Mode;
            }
            set {
                m_Mode = value;
                switch( m_Mode ) {
                    case Mode.LargeHeader:
                        canSort = true;
                        height = 37f;
                        break;
                    case Mode.DefaultHeader:
                        canSort = true;
                        height = DefaultGUI.defaultHeight;
                        break;
                    case Mode.MinimumHeaderWithoutSorting:
                        canSort = false;
                        height = DefaultGUI.minimumHeight;
                        break;
                }
            }
        }

        protected override void ColumnHeaderGUI( MultiColumnHeaderState.Column column, Rect headerRect, int columnIndex ) {
            // Default column header gui
            base.ColumnHeaderGUI( column, headerRect, columnIndex );

            // Add additional info for large header
            if( mode == Mode.LargeHeader ) {
                // Show example overlay stuff on some of the columns
                if( columnIndex > 2 ) {
                    headerRect.xMax -= 3f;
                    var oldAlignment = EditorStyles.largeLabel.alignment;
                    EditorStyles.largeLabel.alignment = TextAnchor.UpperRight;
                    GUI.Label( headerRect, 36 + columnIndex + "%", EditorStyles.largeLabel );
                    EditorStyles.largeLabel.alignment = oldAlignment;
                }
            }
        }
    }
}