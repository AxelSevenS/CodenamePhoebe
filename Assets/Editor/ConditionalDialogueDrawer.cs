using UnityEngine;
using UnityEditor;

namespace SeleneGame.Core.UI {

    [CustomPropertyDrawer( typeof( ConditionalDialogue ), true )]
    public class ConditionalDialogueDrawer : PropertyDrawer {

        public override void OnGUI( Rect position, SerializedProperty property, GUIContent label ) {

            EditorGUI.BeginProperty( position, label, property );

            Rect rectType = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);


            SerializedProperty propConditions = property.FindPropertyRelative( "conditions" );
            EditorGUI.PropertyField( rectType, propConditions, GUIContent.none );
                
            rectType.y += EditorGUI.GetPropertyHeight(propConditions) + EditorGUIUtility.standardVerticalSpacing;

            
            SerializedProperty propDialogueSource = property.FindPropertyRelative( "dialogueSource" );
            EditorGUI.PropertyField( rectType, propDialogueSource, GUIContent.none );
                
            rectType.y += EditorGUI.GetPropertyHeight(propDialogueSource);


            EditorGUI.EndProperty();
        }


        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            
            SerializedProperty propConditions = property.FindPropertyRelative( "conditions" );
            float height = EditorGUI.GetPropertyHeight(propConditions) + EditorGUIUtility.standardVerticalSpacing;

            SerializedProperty propDialogueSource = property.FindPropertyRelative( "dialogueSource" );
            height += EditorGUI.GetPropertyHeight(propDialogueSource);

            return height;
        }

    }

}