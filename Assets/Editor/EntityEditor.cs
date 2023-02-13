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
        private SerializedProperty soAnimator;
        private SerializedProperty soAnimancer;
        private SerializedProperty soRigidbody;
        private SerializedProperty soPhysicsComponent;


        private bool foldout = true;

        // private static string[] characterOptions;
        // int characterTypeIndex = 0;


        // [UnityEditor.Callbacks.DidReloadScripts]
        // private static void PopulateOptions() {
            
        //     characterOptions = new string[Character._types.Count + 1];
        //     characterOptions[0] = "None";
        //     for (int i = 0; i < Character._types.Count; i++) {
        //         characterOptions[i + 1] = Character._types[i].Name;
        //     }
            
        // }


        private void OnEnable() {
            soAnimator = serializedObject.FindProperty("_animator");
            soAnimancer = serializedObject.FindProperty("_animancer");
            soRigidbody = serializedObject.FindProperty("_rigidbody");
            soPhysicsComponent = serializedObject.FindProperty("_physicsComponent");

            targetEntity = (Entity)target;

            // UpdateCharacterInfo();
        }

        // private void UpdateCharacterInfo() {
        //     if (targetEntity.character == null) {
        //         characterTypeIndex = 0;
        //     } else {
        //         characterTypeIndex = Character._types.IndexOf(targetEntity.character.GetType()) + 1;
        //     }
        // }

        public override void OnInspectorGUI(){


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
            
        }
    }
}