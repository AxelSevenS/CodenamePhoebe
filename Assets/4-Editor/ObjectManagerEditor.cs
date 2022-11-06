using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace SeleneGame.Core {

    [CustomEditor(typeof(ObjectManager))]
    public class ObjectManagerEditor : Editor{
        
        public override void OnInspectorGUI(){
            DrawDefaultInspector();
            ObjectManager objectManager = (ObjectManager)target;

            if (GUILayout.Button( "Disable all Objects" )){
                objectManager.DisableAllObjects();
            }
        }

        [DrawGizmo(GizmoType.InSelectionHierarchy | GizmoType.Active)]
        private static void OnDrawGizmosSelected(ObjectManager scr, GizmoType gizmoType) {
            // Handles.zTest = CompareFunction.LessEqual;

            foreach (GameObject obj in scr.objects){
                Vector3 offset = Vector3.up * ((scr.transform.position.y + obj.transform.position.y)/4f);

                Handles.DrawBezier(scr.transform.position, obj.transform.position, scr.transform.position + offset, obj.transform.position + offset, Color.white, EditorGUIUtility.whiteTexture, 1f);
                // Gizmos.DrawLine(transform.position, objectVar.transform.position);
            }
        }
    }
}