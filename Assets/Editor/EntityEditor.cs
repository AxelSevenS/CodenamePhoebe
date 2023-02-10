using System;
using System.Linq;
using System.Collections.Generic;

using UnityEngine;
using UnityEditor;

using System.Reflection;

namespace SeleneGame.Core {

    [CustomEditor(typeof(Entity), true)]
    public class EntityEditor : Editor {

        private Entity targetEntity;
        private SerializedProperty soSelectedCharacter;
        private SerializedProperty soSelectedCharacterCostume;
        private SerializedProperty soAnimator;
        private SerializedProperty soAnimancer;
        private SerializedProperty soRigidbody;
        private SerializedProperty soPhysicsComponent;
        private bool foldout = true;


        private static List<Type> characterTypes = new List<Type>();
        // private static List<ConstructorInfo> characterConstructors = new List<ConstructorInfo>();
        private static string[] characterOptions;
        int characterTypeIndex = 0;

        CharacterCostume selectedCharacterCostume = null;


        [UnityEditor.Callbacks.DidReloadScripts]
        private static void PopulateOptions() {

            characterTypes = new();
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies()) {
                foreach (var type in assembly.GetTypes()) {
                    if (typeof(Character).IsAssignableFrom(type) && !type.IsAbstract) {
                        characterTypes.Add(type);
                    }
                }
            }
            
            characterOptions = new string[characterTypes.Count + 1];
            characterOptions[0] = "None";
            for (int i = 0; i < characterTypes.Count; i++) {
                characterOptions[i + 1] = characterTypes[i].Name;
            }
            
        }


        private void OnEnable() {

            soSelectedCharacter = serializedObject.FindProperty("m_selectedCharacter");
            soSelectedCharacterCostume = serializedObject.FindProperty("m_selectedCharacterCostume");
            soAnimator = serializedObject.FindProperty("_animator");
            soAnimancer = serializedObject.FindProperty("_animancer");
            soRigidbody = serializedObject.FindProperty("_rigidbody");
            soPhysicsComponent = serializedObject.FindProperty("_physicsComponent");

            targetEntity = (Entity)target;

            UpdateCharacterInfo();
        }

        private void UpdateCharacterInfo() {
            if (targetEntity.character == null) {
                characterTypeIndex = 0;
                selectedCharacterCostume = null;
            } else {
                characterTypeIndex = characterTypes.IndexOf(targetEntity.character.GetType()) + 1;
                selectedCharacterCostume = targetEntity.character?.model?.costume;
            }
        }

        public override void OnInspectorGUI(){


            EditorGUI.BeginChangeCheck();

            characterTypeIndex = EditorGUILayout.Popup(
                "Character: ",
                characterTypeIndex,
                characterOptions);

            if (EditorGUI.EndChangeCheck()) {
                if (characterTypeIndex == 0) {
                    targetEntity.SetCharacter(null);
                } else if (characterTypes[characterTypeIndex - 1] != null) {
                    targetEntity.SetCharacter( characterTypes[characterTypeIndex - 1] );
                }
                UpdateCharacterInfo();
            }



            EditorGUI.BeginChangeCheck();
            
            selectedCharacterCostume = EditorGUILayout.ObjectField(new GUIContent("Set Character Costume", "Use this to Switch the Character Costume of the Entity"), selectedCharacterCostume, typeof(CharacterCostume), false) as CharacterCostume;

            if ( EditorGUI.EndChangeCheck() ) {
                targetEntity.SetCostume( selectedCharacterCostume );
                UpdateCharacterInfo();
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
            


		
			SerializedProperty prop = serializedObject.GetIterator();
			if (prop.NextVisible(true)) {
				while (prop.NextVisible(false)) {
                    EditorGUILayout.PropertyField(serializedObject.FindProperty(prop.name), true);
                }
			}
		
			serializedObject.ApplyModifiedProperties();
            // DrawDefaultInspector();
        }
    }
}