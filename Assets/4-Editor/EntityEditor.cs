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
            selectedCharacter = EditorGUILayout.ObjectField(new GUIContent("Set Character", "Use this to Switch the Character of the Entity"), selectedCharacter, typeof(Character), false) as Character;

            if ( EditorGUI.EndChangeCheck() )
                targetEntity.SetCharacter( Character.GetInstanceOf(selectedCharacter) );



            EditorGUI.BeginChangeCheck();
            
            CharacterCostume selectedCharacterCostume = null;
            selectedCharacterCostume = EditorGUILayout.ObjectField(new GUIContent("Set Character Costume", "Use this to Switch the Character Costume of the Entity"), selectedCharacterCostume, typeof(CharacterCostume), false) as CharacterCostume;

            if ( EditorGUI.EndChangeCheck() ) {
                targetEntity.SetCostume( CharacterCostume.GetInstanceOf(selectedCharacterCostume) );
            }

                

            EditorGUILayout.Space(15f);

            DrawDefaultInspector();
        }
    }
}