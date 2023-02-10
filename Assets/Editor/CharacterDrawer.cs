// using System;
// using System.Collections;
// using System.Collections.Generic;
// using UnityEditor;
// using UnityEngine;

// namespace SeleneGame.Core {

//     [CustomPropertyDrawer(typeof(Character), true)]
//     public class CharacterDrawer : PropertyDrawer {

//         private bool foldout = false;
            
//         public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {


//             Character targetCharacter = fieldInfo.GetValue(property.serializedObject.targetObject) as Character;
//             if (targetCharacter == null) {
//                 EditorGUI.LabelField(position, label.text, "Null");
//                 return;
//             }


//             EditorGUI.BeginProperty( position, label, property );
            
//             Rect rectType = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
//             foldout = EditorGUI.Foldout(rectType, foldout, label, true);

//             if (foldout) {
                
//                 EditorGUI.indentLevel++;
//                 rectType.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;


//                 EditorGUI.BeginChangeCheck();

//                 CharacterCostume selectedCharacterCostume = targetCharacter?.model?.costume ?? null;
//                 selectedCharacterCostume = EditorGUI.ObjectField(rectType, new GUIContent("Costume"), selectedCharacterCostume, typeof(CharacterCostume), false) as CharacterCostume;

//                 if ( EditorGUI.EndChangeCheck() ) {
//                     targetCharacter.SetCostume( selectedCharacterCostume ?? targetCharacter.baseCostume);
//                     // UpdateCharacterInfo();
//                 }

//                 rectType.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
//                 foreach (SerializedProperty prop in property) {
                    
//                     // Only draw properties that are visible and direct children of the component.
//                     if (prop.name == "m_Script" || prop.depth > 1)
//                         continue;

//                     // Draw the property.
//                     EditorGUI.PropertyField(rectType, prop, true);
//                     rectType.y += EditorGUI.GetPropertyHeight(prop, true) + EditorGUIUtility.standardVerticalSpacing;
//                 }

                
//                 EditorGUI.indentLevel--;
//             }

//             EditorGUI.EndProperty();


//         }

//         public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
//             float height = EditorGUIUtility.singleLineHeight;
//             if (foldout) {
//                 height += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
//                 foreach (SerializedProperty prop in property) {
//                     if (prop.name == "m_Script" || prop.depth > 1)
//                         continue;
//                     height += EditorGUI.GetPropertyHeight(prop, true) + EditorGUIUtility.standardVerticalSpacing;
//                 }
//             }
//             return height;
//         }
//     }
// }
