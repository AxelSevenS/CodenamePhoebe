using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace SeleneGame.Core {

    [CustomEditor(typeof(Entity), true)]
    public class EntityEditor : Editor{

        Entity targetEntity;
        SerializedProperty soSelectedCharacter;
        SerializedProperty soSelectedCharacterCostume;


        private void OnEnable(){

            soSelectedCharacter = serializedObject.FindProperty( "m_selectedCharacter" );
            soSelectedCharacterCostume = serializedObject.FindProperty( "m_selectedCharacterCostume" );

            targetEntity = (Entity)target;
        }
        
        public override void OnInspectorGUI(){

            EditorGUI.BeginChangeCheck();

            EditorGUILayout.PropertyField( soSelectedCharacter );

            if ( EditorGUI.EndChangeCheck() )
                targetEntity.LoadCharacter();


            EditorGUI.BeginChangeCheck();

            EditorGUILayout.PropertyField( soSelectedCharacterCostume );

            if ( EditorGUI.EndChangeCheck() )
                targetEntity.LoadCharacterCostume();

                

            DrawDefaultInspector();
        }
    }
}