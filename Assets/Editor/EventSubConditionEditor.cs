using UnityEngine;
using UnityEditor;
using System;

namespace SeleneGame.Core.UI {

    [CustomPropertyDrawer( typeof( EventSubCondition ), true )]
    public class EventSubConditionDrawer : PropertyDrawer {


        public override void OnGUI( Rect position, SerializedProperty property, GUIContent label ) {

            EditorGUI.BeginProperty( position, label, property );

            Rect rectType = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);

            SerializedProperty propBinaryOperation = property.FindPropertyRelative( "binaryOperation" );
            EditorGUI.PropertyField( rectType, propBinaryOperation, GUIContent.none );

            rectType.y += PropertyDrawerUtility.lineSpace;

            SerializedProperty propCondition = property.FindPropertyRelative( "condition" );
            EditorGUI.PropertyField( rectType, propCondition, GUIContent.none );


            EditorGUI.EndProperty();
        }


        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            return PropertyDrawerUtility.lineSpace + EditorGUI.GetPropertyHeight(property.FindPropertyRelative( "condition" ));
        }
    }

}