using UnityEngine;
using UnityEditor;

namespace SeleneGame.Core.UI {

    [CustomPropertyDrawer( typeof( ConditionalDialogue ), true )]
    public class ConditionalDialogueDrawer : PropertyDrawer {

        public override void OnGUI( Rect position, SerializedProperty property, GUIContent label ) {

            EditorGUI.BeginProperty( position, label, property );

            Rect rectType = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);


            SerializedProperty propCondition = property.FindPropertyRelative( "condition" );
            EditorGUI.PropertyField( rectType, propCondition, GUIContent.none );
                
            rectType.y += EditorGUI.GetPropertyHeight(propCondition) + EditorGUIUtility.standardVerticalSpacing;



            if (propCondition.FindPropertyRelative("conditionType").intValue != 2) {
                SerializedProperty propSubConditions = property.FindPropertyRelative( "subConditions" );
                EditorGUI.PropertyField( rectType, propSubConditions );
                    
                rectType.y += EditorGUI.GetPropertyHeight(propSubConditions) + EditorGUIUtility.standardVerticalSpacing;
            }

            
            SerializedProperty propDialogueSource = property.FindPropertyRelative( "dialogueSource" );
            EditorGUI.PropertyField( rectType, propDialogueSource, GUIContent.none );
                
            rectType.y += EditorGUI.GetPropertyHeight(propDialogueSource) + EditorGUIUtility.standardVerticalSpacing;


            EditorGUI.EndProperty();
        }


        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            
            SerializedProperty propCondition = property.FindPropertyRelative( "condition" );
            float height = EditorGUI.GetPropertyHeight(propCondition) + EditorGUIUtility.standardVerticalSpacing;

            if (propCondition.FindPropertyRelative("conditionType").intValue != 2) {
                SerializedProperty propSubConditions = property.FindPropertyRelative( "subConditions" );
                height += EditorGUI.GetPropertyHeight(propSubConditions) + EditorGUIUtility.standardVerticalSpacing;
            }

            SerializedProperty propDialogueSource = property.FindPropertyRelative( "dialogueSource" );
            height += EditorGUI.GetPropertyHeight(propDialogueSource) + EditorGUIUtility.standardVerticalSpacing;

            return height;
        }

    }

}