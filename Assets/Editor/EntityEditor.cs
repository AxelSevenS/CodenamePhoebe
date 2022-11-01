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

            Character selectedCharacter = null;
            selectedCharacter = EditorGUILayout.ObjectField("Set Character", selectedCharacter, typeof(Character), false) as Character;

            if ( EditorGUI.EndChangeCheck() )
                targetEntity.SetCharacter( Character.GetInstanceOf(selectedCharacter) );


            EditorGUI.BeginChangeCheck();
            
            CharacterCostume selectedCharacterCostume = null;
            selectedCharacterCostume = EditorGUILayout.ObjectField("Set Character Costume", selectedCharacterCostume, typeof(CharacterCostume), false) as CharacterCostume;

            if ( EditorGUI.EndChangeCheck() )
                targetEntity.SetCostume( selectedCharacterCostume );

            EditorGUILayout.Space(15f);

            DrawDefaultInspector();
        }
    }
}