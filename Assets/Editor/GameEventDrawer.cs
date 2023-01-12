using UnityEngine;
using UnityEditor;

namespace SeleneGame.Core.UI {

    [CustomPropertyDrawer( typeof( GameEvent ), true )]
    public class GameEventDrawer : PropertyDrawer {

        public override void OnGUI( Rect position, SerializedProperty property, GUIContent label ) {

            GameEvent gameEvent = PropertyDrawerUtility.GetTargetObject(property) as GameEvent;
            Rect rectType = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);


            EditorGUI.BeginProperty( position, label, property );


            SerializedProperty propConditions = property.FindPropertyRelative( "conditions" );
            EditorGUI.PropertyField( rectType, propConditions, GUIContent.none );
                
            rectType.y += EditorGUI.GetPropertyHeight(propConditions) + EditorGUIUtility.standardVerticalSpacing;



                
            SerializedProperty propEventType = property.FindPropertyRelative( "eventType" );
            gameEvent.eventType = (GameEvent.EventType)EditorGUI.EnumPopup(rectType, GUIContent.none, gameEvent.eventType, gameEvent.DisplayEventType);
            
            rectType.y += EditorGUI.GetPropertyHeight(propEventType) + EditorGUIUtility.standardVerticalSpacing;

            switch (gameEvent.eventType) {
                case GameEvent.EventType.SetFlag:
                    DisplaySetFlagInfo( ref rectType, property );
                    break;
                case GameEvent.EventType.RemoveFlag:
                    DisplayRemoveFlagInfo( ref rectType, property );
                    break;
                case GameEvent.EventType.StartDialogue:
                case GameEvent.EventType.StartAlert:
                case GameEvent.EventType.SkipToLine:
                    DisplayDialogueSourceInfo( ref rectType, property );
                    break;
                case GameEvent.EventType.SetCharacterCostume:
                    DisplaySetCharacterCostumeInfo( ref rectType, property );
                    break;
                case GameEvent.EventType.SetWeaponCostume:
                    DisplaySetWeaponCostumeInfo( ref rectType, property );
                    break;
            }


            EditorGUI.EndProperty();
        }


        private void DisplaySetFlagInfo(ref Rect rectType, SerializedProperty property) {
            
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

            rectType.y += Mathf.Max(EditorGUI.GetPropertyHeight(propEditedFlagType), EditorGUI.GetPropertyHeight(propEditedFlagName), EditorGUI.GetPropertyHeight(propEditedFlagValue));
        }



        private void DisplayRemoveFlagInfo(ref Rect rectType, SerializedProperty property) {
            float halfWidth = rectType.width / 2f;
            Rect removeFlagRow = new Rect(rectType.x, rectType.y, halfWidth, rectType.height);
            SerializedProperty propEditedFlagType = property.FindPropertyRelative( "editedFlagType" );
            EditorGUI.PropertyField( removeFlagRow, propEditedFlagType, GUIContent.none );

            removeFlagRow.x += halfWidth;
            SerializedProperty propEditedFlagName = property.FindPropertyRelative( "editedFlagName" );
            EditorGUI.PropertyField( removeFlagRow, propEditedFlagName, GUIContent.none );

            rectType.y += Mathf.Max(EditorGUI.GetPropertyHeight(propEditedFlagType), EditorGUI.GetPropertyHeight(propEditedFlagName));
        }



        private void DisplayDialogueSourceInfo(ref Rect rectType, SerializedProperty property) {
            SerializedProperty propTargetLine = property.FindPropertyRelative( "dialogueSource" );
            EditorGUI.PropertyField( rectType, propTargetLine, GUIContent.none );

            rectType.y += EditorGUI.GetPropertyHeight(propTargetLine);
        }



        private void DisplaySetCharacterCostumeInfo(ref Rect rectType, SerializedProperty property) {
            SerializedProperty propTargetCharacterId = property.FindPropertyRelative( "targetCharacterId" );
            EditorGUI.PropertyField( rectType, propTargetCharacterId, GUIContent.none );

            rectType.y += EditorGUI.GetPropertyHeight(propTargetCharacterId) + EditorGUIUtility.standardVerticalSpacing;


            SerializedProperty propTargetCharacterCostume = property.FindPropertyRelative( "targetCharacterCostume" );
            EditorGUI.PropertyField( rectType, propTargetCharacterCostume, GUIContent.none );

            rectType.y += EditorGUI.GetPropertyHeight(propTargetCharacterCostume);
        }



        private void DisplaySetWeaponCostumeInfo(ref Rect rectType, SerializedProperty property) {
            SerializedProperty propTargetWeaponId = property.FindPropertyRelative( "targetWeaponId" );
            EditorGUI.PropertyField( rectType, propTargetWeaponId, GUIContent.none );

            rectType.y += EditorGUI.GetPropertyHeight(propTargetWeaponId) + EditorGUIUtility.standardVerticalSpacing;


            SerializedProperty propTargetWeaponCostume = property.FindPropertyRelative( "targetWeaponCostume" );
            EditorGUI.PropertyField( rectType, propTargetWeaponCostume, GUIContent.none );

            rectType.y += EditorGUI.GetPropertyHeight(propTargetWeaponCostume);
        }



        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            
            SerializedProperty propConditions = property.FindPropertyRelative( "conditions" );
            SerializedProperty propEventType = property.FindPropertyRelative( "eventType" );

            float height = EditorGUI.GetPropertyHeight(propConditions) + EditorGUIUtility.standardVerticalSpacing + EditorGUI.GetPropertyHeight(propEventType) + EditorGUIUtility.standardVerticalSpacing;


            GameEvent.EventType eventType = (GameEvent.EventType)propEventType.intValue;
            switch (eventType) {
                case GameEvent.EventType.SetFlag:
                    height += GetSetFlagInfoHeight(property);
                    break;
                case GameEvent.EventType.RemoveFlag:
                    height += GetRemoveFlagInfoHeight(property);
                    break;
                case GameEvent.EventType.StartDialogue:
                case GameEvent.EventType.StartAlert:
                case GameEvent.EventType.SkipToLine:
                    height += GetDialogueSourceInfoHeight(property);
                    break;
                case GameEvent.EventType.SetCharacterCostume:
                    height += GetSetCharacterCostumeInfoHeight(property);
                    break;
                case GameEvent.EventType.SetWeaponCostume:
                    height += GetSetWeaponCostumeInfoHeight(property);
                    break;
            }

            return height;
        }


        private float GetSetFlagInfoHeight(SerializedProperty property) {
            SerializedProperty propEditedFlagType = property.FindPropertyRelative( "editedFlagType" );
            SerializedProperty propEditedFlagName = property.FindPropertyRelative( "editedFlagName" );
            SerializedProperty propEditedFlagValue = property.FindPropertyRelative( "editedFlagValue" );

            return Mathf.Max( EditorGUI.GetPropertyHeight(propEditedFlagType), EditorGUI.GetPropertyHeight(propEditedFlagName), EditorGUI.GetPropertyHeight(propEditedFlagValue) );
        }



        private float GetRemoveFlagInfoHeight(SerializedProperty property) {
            SerializedProperty propEditedFlagType = property.FindPropertyRelative( "editedFlagType" );
            SerializedProperty propEditedFlagName = property.FindPropertyRelative( "editedFlagName" );
            return Mathf.Max( EditorGUI.GetPropertyHeight(propEditedFlagType), EditorGUI.GetPropertyHeight(propEditedFlagName) );
        }



        private float GetDialogueSourceInfoHeight(SerializedProperty property) {
            SerializedProperty propTargetLine = property.FindPropertyRelative( "dialogueSource" );
            return EditorGUI.GetPropertyHeight(propTargetLine);
        }



        private float GetSetCharacterCostumeInfoHeight(SerializedProperty property) {
            SerializedProperty propTargetCharacterId = property.FindPropertyRelative( "targetCharacterId" );
            SerializedProperty propTargetCharacterCostume = property.FindPropertyRelative( "targetCharacterCostume" );
            return EditorGUI.GetPropertyHeight(propTargetCharacterId) + EditorGUI.GetPropertyHeight(propTargetCharacterCostume);
        }

        

        private float GetSetWeaponCostumeInfoHeight(SerializedProperty property) {
            SerializedProperty propTargetWeaponId = property.FindPropertyRelative( "targetWeaponId" );
            SerializedProperty propTargetWeaponCostume = property.FindPropertyRelative( "targetWeaponCostume" );
            return EditorGUI.GetPropertyHeight(propTargetWeaponId) + EditorGUI.GetPropertyHeight(propTargetWeaponCostume);
        }

    }

}