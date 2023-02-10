// using System;
// using System.Collections;
// using System.Collections.Generic;
// using SevenGame.Utility.Editor;
// using UnityEditor;
// using UnityEngine;

// namespace SeleneGame.Core {

//     [CustomPropertyDrawer(typeof(Weapon), true)]
//     public class WeaponDrawer : PropertyDrawer {

//         private bool foldout = false;
            
//         public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {


//             // Weapon targetWeapon = SevenEditorUtility.GetTargetObject(property) as Weapon;
//             Weapon targetWeapon = fieldInfo.GetValue(property.serializedObject.targetObject) as Weapon;
//             Debug.Log($"{label.text}: {targetWeapon}");
//             // if (targetWeapon == null) {
//             //     EditorGUI.LabelField(position, label.text, "Null");
//             //     return;
//             // }


//             EditorGUI.LabelField(position, label.text, "Sex");

//             EditorGUI.BeginProperty( position, label, property );
            
//             Rect rectType = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
//             foldout = EditorGUI.Foldout(rectType, foldout, label, true);

//             if (foldout) {
                
//                 EditorGUI.indentLevel++;
//                 rectType.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;


//                 EditorGUI.BeginChangeCheck();

//                 WeaponCostume selectedWeaponCostume = targetWeapon?.model?.costume ?? null;
//                 selectedWeaponCostume = EditorGUI.ObjectField(rectType, new GUIContent("Costume"), selectedWeaponCostume, typeof(WeaponCostume), false) as WeaponCostume;

//                 if ( EditorGUI.EndChangeCheck() ) {
//                     targetWeapon.SetCostume( selectedWeaponCostume ?? targetWeapon.baseCostume);
//                     // UpdateWeaponInfo();
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
