using UnityEngine;
using UnityEditor;
using System;

namespace SeleneGame.Core.UI {

    [CustomPropertyDrawer( typeof( EventSubCondition ), true )]
    public class EventSubConditionDrawer : PropertyDrawer {


        public override void OnGUI( Rect position, SerializedProperty property, GUIContent label ) {

            Rect rectType = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            

            EditorGUI.BeginProperty( position, label, property );


            SerializedProperty propBinaryOperation = property.FindPropertyRelative( "binaryOperation" );
            EditorGUI.PropertyField( rectType, propBinaryOperation, GUIContent.none );

            rectType.y += EditorGUI.GetPropertyHeight(propBinaryOperation) + EditorGUIUtility.standardVerticalSpacing;


            SerializedProperty propCondition = property.FindPropertyRelative( "condition" );
            EditorGUI.PropertyField( rectType, propCondition, GUIContent.none );

            rectType.y += EditorGUI.GetPropertyHeight(propCondition);


            EditorGUI.EndProperty();
        }


        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            return EditorGUI.GetPropertyHeight(property.FindPropertyRelative( "binaryOperation" )) + EditorGUI.GetPropertyHeight(property.FindPropertyRelative( "condition" ));
        }
    }

}