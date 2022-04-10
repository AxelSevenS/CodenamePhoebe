using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SeleneGame {

    public class ObjectManager : MonoBehaviour{
        
        public static ObjectManager current;
        
        public List<GameObject> objectList = new List<GameObject>();

        void OnEnable(){
            if (current != null)
                Destroy(current);
            current = this;
        }


    #if UNITY_EDITOR
        void OnDrawGizmosSelected(){
            // Handles.zTest = CompareFunction.LessEqual;

            foreach (GameObject objectVar in objectList){
                Vector3 offset = Vector3.up * ((transform.position.y + objectVar.transform.position.y)/4f);

                Handles.DrawBezier(transform.position, objectVar.transform.position, transform.position + offset, objectVar.transform.position + offset, Color.white, EditorGUIUtility.whiteTexture, 1f);
                // Gizmos.DrawLine(transform.position, objectVar.transform.position);
            }
        }
    #endif

        public void DisableAllObjects(){
            /* foreach (GameObject obj in objectList){
                obj.SetActive(false);
            } */
        }
    }
}