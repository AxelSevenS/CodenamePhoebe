using UnityEngine;
using UnityEditor;
using System;

namespace SeleneGame.Core.UI {

    [CustomPropertyDrawer( typeof( GameEvent ), true )]
    public class GameEventDrawer : PropertyDrawer {

        private float lineSpace = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;


        public override void OnGUI( Rect position, SerializedProperty property, GUIContent label ) {

            EditorGUI.BeginProperty( position, label, property );

            Rect rectType = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);


            SerializedProperty propCondition = property.FindPropertyRelative( "condition" );
            EditorGUI.PropertyField( rectType, propCondition, GUIContent.none );
                

            rectType.y += EditorGUI.GetPropertyHeight(propCondition);


            if (propCondition.FindPropertyRelative("conditionType").enumValueIndex != 2) {
                SerializedProperty propSubConditions = property.FindPropertyRelative( "subConditions" );
                EditorGUI.PropertyField( rectType, propSubConditions );
                    

                rectType.y += EditorGUI.GetPropertyHeight(propSubConditions);
            }


            SerializedProperty propEventType = property.FindPropertyRelative( "eventType" );
            EditorGUI.PropertyField( rectType, propEventType, GUIContent.none );


            rectType.y += lineSpace;


            switch (propEventType.enumValueIndex) {
                case 0:
                    DisplaySetFlagInfo( rectType, property );
                    break;
                case 1:
                    DisplayRemoveFlagInfo( rectType, property );
                    break;
                case 2:
                case 3:
                case 4:
                    DisplayLineInfo( rectType, property );
                    break;
                // case 5:
                //     DisplayEndDialogueInfo( rectType, property );
                //     break;
                case 6:
                    DisplaySetCharacterCostumeInfo( rectType, property );
                    break;
                case 7:
                    DisplaySetWeaponCostumeInfo( rectType, property );
                    break;
                // case 8:
                //     DisplayDestroyInfo( rectType, property );
                //     break;
            }

            EditorGUI.EndProperty();
        }


        private void DisplaySetFlagInfo(Rect rectType, SerializedProperty property) {
            float thirdWidth = rectType.width / 3f;
            Rect setFlagRow = new Rect(rectType.x, rectType.y, thirdWidth, rectType.height);
            SerializedProperty propEditedFlagType = property.FindPropertyRelative( "editedFlagType" );
            EditorGUI.PropertyField( setFlagRow, propEditedFlagType, GUIContent.none );

            setFlagRow.x += thirdWidth;
            SerializedProperty propEditedFlagName = property.FindPropertyRelative( "editedFlagName" );
            EditorGUI.PropertyField( setFlagRow, propEditedFlagName, GUIContent.none );

            setFlagRow.x += thirdWidth;
            SerializedProperty propEditedFlagValue = property.FindPropertyRelative( "editedFlagValue" );
            EditorGUI.PropertyField( setFlagRow, propEditedFlagValue, GUIContent.none );
        }

        private void DisplayRemoveFlagInfo(Rect rectType, SerializedProperty property) {
            float halfWidth = rectType.width / 2f;
            Rect removeFlagRow = new Rect(rectType.x, rectType.y, halfWidth, rectType.height);
            SerializedProperty propEditedFlagType = property.FindPropertyRelative( "editedFlagType" );
            EditorGUI.PropertyField( removeFlagRow, propEditedFlagType, GUIContent.none );

            removeFlagRow.x += halfWidth;
            SerializedProperty propEditedFlagName = property.FindPropertyRelative( "editedFlagName" );
            EditorGUI.PropertyField( removeFlagRow, propEditedFlagName, GUIContent.none );
        }

        private void DisplayLineInfo(Rect rectType, SerializedProperty property) {
            SerializedProperty propTargetLine = property.FindPropertyRelative( "targetLine" );
            EditorGUI.PropertyField( rectType, propTargetLine, GUIContent.none );
        }

        // private void DisplayEndDialogueInfo(Rect rectType, SerializedProperty property) {
        //     // throw new NotImplementedException();
        // }

        // private void DisplayDestroyInfo(Rect rectType, SerializedProperty property) {
        //     // throw new NotImplementedException();
        // }

        private void DisplaySetCharacterCostumeInfo(Rect rectType, SerializedProperty property) {
            SerializedProperty propTargetCharacterId = property.FindPropertyRelative( "targetCharacterId" );
            EditorGUI.PropertyField( rectType, propTargetCharacterId, GUIContent.none );

            rectType.y += lineSpace;

            SerializedProperty propTargetCharacterCostume = property.FindPropertyRelative( "targetCharacterCostume" );
            EditorGUI.PropertyField( rectType, propTargetCharacterCostume, GUIContent.none );
        }

        private void DisplaySetWeaponCostumeInfo(Rect rectType, SerializedProperty property) {
            SerializedProperty propTargetWeaponId = property.FindPropertyRelative( "targetWeaponId" );
            EditorGUI.PropertyField( rectType, propTargetWeaponId, GUIContent.none );

            rectType.y += lineSpace;

            SerializedProperty propTargetWeaponCostume = property.FindPropertyRelative( "targetWeaponCostume" );
            EditorGUI.PropertyField( rectType, propTargetWeaponCostume, GUIContent.none );
        }



        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            
            SerializedProperty propCondition = property.FindPropertyRelative( "condition" );

            float height = EditorGUI.GetPropertyHeight(propCondition) + lineSpace;

            if (propCondition.FindPropertyRelative("conditionType").enumValueIndex != 2) {
                SerializedProperty propSubConditions = property.FindPropertyRelative( "subConditions" );
                height += EditorGUI.GetPropertyHeight(propSubConditions);
            }

            int eventType = property.FindPropertyRelative( "eventType" ).enumValueIndex;
            switch (eventType) {
                case 0:
                case 1:
                case 2:
                case 3:
                case 4:
                    height += lineSpace;
                    break;
                case 6:
                case 7:
                    height += 2f * lineSpace;
                    break;
            }

            return height;
        }
    }

}