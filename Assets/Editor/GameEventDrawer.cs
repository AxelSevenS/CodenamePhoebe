using UnityEngine;
using UnityEditor;

namespace SeleneGame.Core.UI {

    [CustomPropertyDrawer( typeof( GameEvent ), true )]
    public class GameEventDrawer : PropertyDrawer {

        public override void OnGUI( Rect position, SerializedProperty property, GUIContent label ) {

            GameEvent gameEvent = PropertyDrawerUtility.GetTargetObject(property) as GameEvent;


            EditorGUI.BeginProperty( position, label, property );

            Rect rectType = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);


            SerializedProperty propCondition = property.FindPropertyRelative( "condition" );
            EditorGUI.PropertyField( rectType, propCondition, GUIContent.none );
                

            rectType.y += EditorGUI.GetPropertyHeight(propCondition);


            if (propCondition.FindPropertyRelative("conditionType").intValue != 2) {
                SerializedProperty propSubConditions = property.FindPropertyRelative( "subConditions" );
                EditorGUI.PropertyField( rectType, propSubConditions );
                    

                rectType.y += EditorGUI.GetPropertyHeight(propSubConditions);
            }

            gameEvent.eventType = (GameEvent.EventType)EditorGUI.EnumPopup(rectType, GUIContent.none, gameEvent.eventType, gameEvent.DisplayEventType);


            rectType.y += PropertyDrawerUtility.lineSpace;


            switch (gameEvent.eventType) {
                case GameEvent.EventType.SetFlag:
                    DisplaySetFlagInfo( rectType, property );
                    break;
                case GameEvent.EventType.RemoveFlag:
                    DisplayRemoveFlagInfo( rectType, property );
                    break;
                case GameEvent.EventType.StartDialogue:
                case GameEvent.EventType.StartAlert:
                case GameEvent.EventType.SkipToLine:
                    DisplayLineInfo( rectType, property );
                    break;
                case GameEvent.EventType.SetCharacterCostume:
                    DisplaySetCharacterCostumeInfo( rectType, property );
                    break;
                case GameEvent.EventType.SetWeaponCostume:
                    DisplaySetWeaponCostumeInfo( rectType, property );
                    break;
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

        private void DisplaySetCharacterCostumeInfo(Rect rectType, SerializedProperty property) {
            SerializedProperty propTargetCharacterId = property.FindPropertyRelative( "targetCharacterId" );
            EditorGUI.PropertyField( rectType, propTargetCharacterId, GUIContent.none );

            rectType.y += PropertyDrawerUtility.lineSpace;

            SerializedProperty propTargetCharacterCostume = property.FindPropertyRelative( "targetCharacterCostume" );
            EditorGUI.PropertyField( rectType, propTargetCharacterCostume, GUIContent.none );
        }

        private void DisplaySetWeaponCostumeInfo(Rect rectType, SerializedProperty property) {
            SerializedProperty propTargetWeaponId = property.FindPropertyRelative( "targetWeaponId" );
            EditorGUI.PropertyField( rectType, propTargetWeaponId, GUIContent.none );

            rectType.y += PropertyDrawerUtility.lineSpace;

            SerializedProperty propTargetWeaponCostume = property.FindPropertyRelative( "targetWeaponCostume" );
            EditorGUI.PropertyField( rectType, propTargetWeaponCostume, GUIContent.none );
        }



        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            
            SerializedProperty propCondition = property.FindPropertyRelative( "condition" );

            float height = EditorGUI.GetPropertyHeight(propCondition) + PropertyDrawerUtility.lineSpace;

            if (propCondition.FindPropertyRelative("conditionType").intValue != 2) {
                SerializedProperty propSubConditions = property.FindPropertyRelative( "subConditions" );
                height += EditorGUI.GetPropertyHeight(propSubConditions);
            }

            GameEvent.EventType eventType = (GameEvent.EventType)property.FindPropertyRelative( "eventType" ).intValue;
            switch (eventType) {
                case GameEvent.EventType.SetFlag:
                case GameEvent.EventType.RemoveFlag:
                case GameEvent.EventType.StartDialogue:
                case GameEvent.EventType.StartAlert:
                case GameEvent.EventType.SkipToLine:
                    height += PropertyDrawerUtility.lineSpace;
                    break;
                case GameEvent.EventType.SetCharacterCostume:
                case GameEvent.EventType.SetWeaponCostume:
                    height += 2f * PropertyDrawerUtility.lineSpace;
                    break;
                case GameEvent.EventType.EndDialogue:
                case GameEvent.EventType.Destroy:
                    break;
            }

            return height;
        }

    }

}