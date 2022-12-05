using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace SeleneGame.Core {

    [CustomEditor(typeof(Entity), true)]
    public class EntityEditor : Editor{

        private Entity targetEntity;
        private SerializedProperty soSelectedCharacter;
        private SerializedProperty soSelectedCharacterCostume;
        private SerializedProperty soAnimator;
        private SerializedProperty soAnimancer;
        private SerializedProperty soRigidbody;
        private SerializedProperty soPhysicsComponent;
        private bool foldout = true;


        private void OnEnable(){

            soSelectedCharacter = serializedObject.FindProperty( "m_selectedCharacter" );
            soSelectedCharacterCostume = serializedObject.FindProperty( "m_selectedCharacterCostume" );
            soAnimator = serializedObject.FindProperty( "_animator" );
            soAnimancer = serializedObject.FindProperty( "_animancer" );
            soRigidbody = serializedObject.FindProperty( "_rigidbody" );
            soPhysicsComponent = serializedObject.FindProperty( "_physicsComponent" );

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

            foldout = EditorGUILayout.BeginFoldoutHeaderGroup(foldout, "Components");
            if (foldout) {
                EditorGUILayout.PropertyField(soAnimator);
                EditorGUILayout.PropertyField(soAnimancer);
                EditorGUILayout.PropertyField(soRigidbody);
                EditorGUILayout.PropertyField(soPhysicsComponent);
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
            

            DrawDefaultInspector();
        }
    }
}