using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace SeleneGame.Core {

    // [CustomPropertyDrawer(typeof(Character), true)]
    public class CostumableDrawer<TCostumable, TCostume, TModel> : PropertyDrawer where TCostumable : Costumable<TCostumable, TCostume, TModel> where TCostume : Costume<TCostume> where TModel : CostumeModel<TCostume> {

        private bool foldout = false;
            
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {


            TCostumable targetCostumable = property.managedReferenceValue as TCostumable;
            if (targetCostumable == null) {
                EditorGUI.LabelField(position, label.text, "Null");
                return;
            }


            EditorGUI.BeginProperty( position, label, property );
            
            Rect rectType = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            foldout = EditorGUI.Foldout(rectType, foldout, label, true);

            if (foldout) {
                
                EditorGUI.indentLevel++;
                rectType.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;


                EditorGUI.BeginChangeCheck();

                TCostume selectedCostume = targetCostumable?.model?.costume ?? null;
                selectedCostume = EditorGUI.ObjectField(rectType, new GUIContent("Costume"), selectedCostume, typeof(TCostume), false) as TCostume;

                if ( EditorGUI.EndChangeCheck() ) {
                    targetCostumable.SetCostume( selectedCostume ?? targetCostumable.baseCostume);
                }

                rectType.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                foreach (SerializedProperty prop in property) {
                    
                    // Only draw properties that are visible and direct children of the component.
                    if (prop.name == "m_Script" || prop.depth > 1)
                        continue;

                    // Draw the property.
                    EditorGUI.PropertyField(rectType, prop, true);
                    rectType.y += EditorGUI.GetPropertyHeight(prop, true) + EditorGUIUtility.standardVerticalSpacing;
                }

                
                EditorGUI.indentLevel--;
            }

            EditorGUI.EndProperty();


        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            float height = EditorGUIUtility.singleLineHeight;
            if (foldout) {
                height += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                foreach (SerializedProperty prop in property) {
                    if (prop.name == "m_Script" || prop.depth > 1)
                        continue;
                    height += EditorGUI.GetPropertyHeight(prop, true) + EditorGUIUtility.standardVerticalSpacing;
                }
            }
            return height;
        }
    }
}
