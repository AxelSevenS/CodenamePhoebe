// using System;

// using UnityEngine;
// using UnityEditor;


// namespace SeleneGame.Core {

//     [CustomEditor(typeof(Entity), true)]
//     public class EntityEditor : Editor {

//         private Entity targetEntity;
//         private SerializedProperty soHealth;
//         private SerializedProperty soAnimator;
//         private SerializedProperty soAnimancer;
//         private SerializedProperty soRigidbody;
//         private SerializedProperty soPhysicsComponent;


//         private bool foldout = true;


//         private void OnEnable() {
//             soHealth = serializedObject.FindProperty("_health");
//             soAnimator = serializedObject.FindProperty("_animator");
//             soAnimancer = serializedObject.FindProperty("_animancer");
//             soRigidbody = serializedObject.FindProperty("_rigidbody");
//             soPhysicsComponent = serializedObject.FindProperty("_physicsComponent");

//             targetEntity = (Entity)target;
//         }

//         public override void OnInspectorGUI(){

//             foldout = EditorGUILayout.BeginFoldoutHeaderGroup(foldout, "Components");
//             if (foldout) {
//                 EditorGUILayout.PropertyField(soHealth);
//                 EditorGUILayout.PropertyField(soAnimator);
//                 EditorGUILayout.PropertyField(soAnimancer);
//                 EditorGUILayout.PropertyField(soRigidbody);
//                 EditorGUILayout.PropertyField(soPhysicsComponent);
//             }
//             EditorGUILayout.EndFoldoutHeaderGroup();
		
        
// 			SerializedProperty prop = serializedObject.GetIterator();
// 			if (prop.NextVisible(true)) {
// 				while (prop.NextVisible(false)) {
//                     EditorGUILayout.PropertyField(serializedObject.FindProperty(prop.name), true);
//                 }
// 			}
		
// 			serializedObject.ApplyModifiedProperties();
            
//         }
//     }
// }