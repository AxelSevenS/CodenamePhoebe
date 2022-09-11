using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace SeleneGame.Core {

    [CustomEditor(typeof(Entity), true)]
    public class EntityEditor : Editor{

        Entity targetEntity;
        SerializedProperty soLoadedCharacter;


        private void OnEnable(){

            soLoadedCharacter = serializedObject.FindProperty( "m_loadedCharacter" );

            targetEntity = (Entity)target;
        }
        
        public override void OnInspectorGUI(){

            EditorGUI.BeginChangeCheck();

            EditorGUILayout.PropertyField( soLoadedCharacter );

            if ( EditorGUI.EndChangeCheck() ){
                targetEntity.LoadCharacter();
            }

            DrawDefaultInspector();
        }
    }
}