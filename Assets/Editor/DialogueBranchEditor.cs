using UnityEngine;
using UnityEditor;
using System;

namespace SeleneGame.Core.UI {

    [CustomEditor( typeof( DialogueBranch ), true )]
    public class DialogueBranchEditor : Editor {

        DialogueBranch targetBranch;

        private void OnEnable() {
            targetBranch = (DialogueBranch)target;
        }

        public override void OnInspectorGUI() {

            DrawDefaultInspector();

            if ( targetBranch.highPriorityDialogues.Count > 32 || targetBranch.lowPriorityDialogues.Count > 32) {
                
                Undo.PerformUndo();
                Debug.LogError( "DO NOT put more than 32 dialogue events in a Dialogue Branch. It will break everything" );
                
            }

            EditorGUILayout.HelpBox( "Order High Priority Dialogue Events from Top to Bottom depending on priority.", MessageType.Warning );
            
        }

    }

}