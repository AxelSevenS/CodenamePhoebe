using UnityEngine;
using UnityEditor;
using System;

namespace SeleneGame.Core.UI {

    [CustomPropertyDrawer( typeof( EventCondition ), true )]
    public class EventConditionDrawer : PropertyDrawer {


        public override void OnGUI( Rect position, SerializedProperty property, GUIContent label ) {

            EditorGUI.BeginProperty( position, label, property );

            Rect rectType = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);


            SerializedProperty propConditionType = property.FindPropertyRelative( "conditionType" );

            if (propConditionType.intValue == 2) {
                Rect flagRowRect = new Rect(rectType.x, rectType.y, rectType.width, rectType.height);
                EditorGUI.PropertyField( flagRowRect, propConditionType, GUIContent.none );

                rectType.y += EditorGUI.GetPropertyHeight(propConditionType);
            } else {
                float thirdWidth = rectType.width / 3f;

                Rect flagRowRect = new Rect(rectType.x, rectType.y, thirdWidth, rectType.height);
                EditorGUI.PropertyField( flagRowRect, propConditionType, GUIContent.none );

                flagRowRect.x += thirdWidth;
                SerializedProperty propFlagType = property.FindPropertyRelative( "flagType" );
                EditorGUI.PropertyField( flagRowRect, propFlagType, GUIContent.none );

                flagRowRect.x += thirdWidth;
                SerializedProperty propFlagName = property.FindPropertyRelative( "flagName" );
                EditorGUI.PropertyField( flagRowRect, propFlagName, GUIContent.none );

                rectType.y += Mathf.Max(EditorGUI.GetPropertyHeight(propConditionType), EditorGUI.GetPropertyHeight(propFlagType), EditorGUI.GetPropertyHeight(propFlagName)) + EditorGUIUtility.standardVerticalSpacing;


                float halfWidth = rectType.width / 2f;

                Rect operationRowRect = new Rect(rectType.x, rectType.y, halfWidth, rectType.height);
                SerializedProperty propOperatorType = property.FindPropertyRelative( "operatorType" );
                EditorGUI.PropertyField( operationRowRect, propOperatorType, GUIContent.none );

                operationRowRect.x += halfWidth;
                SerializedProperty propFlagValue = property.FindPropertyRelative( "flagValue" );
                EditorGUI.PropertyField( operationRowRect, propFlagValue, GUIContent.none );

                rectType.y += Mathf.Max(EditorGUI.GetPropertyHeight(propOperatorType), EditorGUI.GetPropertyHeight(propFlagValue));
            }

            EditorGUI.EndProperty();
        }


        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            SerializedProperty propConditionType = property.FindPropertyRelative( "conditionType" );
            float height = 0;

            if (property.FindPropertyRelative( "conditionType" ).intValue == 2) {
                height += EditorGUI.GetPropertyHeight(propConditionType);
            } else {
                SerializedProperty propFlagType = property.FindPropertyRelative( "flagType" );
                SerializedProperty propFlagName = property.FindPropertyRelative( "flagName" );
                height += Mathf.Max(EditorGUI.GetPropertyHeight(propConditionType), EditorGUI.GetPropertyHeight(propFlagType), EditorGUI.GetPropertyHeight(propFlagName)) + EditorGUIUtility.standardVerticalSpacing;

                SerializedProperty propOperatorType = property.FindPropertyRelative( "operatorType" );
                SerializedProperty propFlagValue = property.FindPropertyRelative( "flagValue" );
                height += Mathf.Max(EditorGUI.GetPropertyHeight(propOperatorType), EditorGUI.GetPropertyHeight(propFlagValue));
            }

            return height;
        }
    }

}