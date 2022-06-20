using UnityEngine;
using UnityEditor;
using SeleneGame.Core;

namespace SeleneGame.EditorUI {

    [CustomPropertyDrawer( typeof( InvokableEvent ), true )]
    public class InvokableEventDrawer : PropertyDrawer {

        private float lineSpace = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

        public override void OnGUI( Rect position, SerializedProperty property, GUIContent label ) {
            SerializedProperty propEventType = property.FindPropertyRelative( "eventType" );
            

            EditorGUI.BeginProperty( position, label, property );

            Rect rectType = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            EditorGUI.PropertyField( rectType, propEventType, GUIContent.none );

            switch (propEventType.enumValueIndex) {
                case 0:
                    DisplayConversationInfo(position, property);
                    break;
                case 1:
                    DisplayEntityCostumeInfo(position, property);
                    break;
                case 2:
                    DisplayWeaponCostumeInfo(position, property);
                    break;
            }

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            int eventType = property.FindPropertyRelative( "eventType" ).enumValueIndex;

            switch (eventType) {
                case 0:
                    return GetConversationInfoHeight(property);
                    break;
                case 1:
                    return GetEntityCostumeInfoHeight(property);
                    break;
                case 2:
                    return GetWeaponCostumeInfoHeight(property);
                    break;
                default:
                    return GetDropdownHeight(property);
                    break;
            }

        }

        private float GetDropdownHeight(SerializedProperty property){
            return EditorGUI.GetPropertyHeight( property.FindPropertyRelative( "eventType" ) );
        }


        private void DisplayConversationInfo(Rect position, SerializedProperty property) {
            SerializedProperty propConversation = property.FindPropertyRelative( "conversation" );
            Rect rectConversation = new Rect(position.x, position.y + lineSpace, position.width, EditorGUI.GetPropertyHeight( propConversation ));
            EditorGUI.PropertyField( rectConversation, propConversation, GUIContent.none );
        }
        private float GetConversationInfoHeight(SerializedProperty property) {
            float propEventTypeHeight = GetDropdownHeight(property);
            float propConversationHeight = EditorGUI.GetPropertyHeight( property.FindPropertyRelative( "conversation" ) );
            return propEventTypeHeight + propConversationHeight + EditorGUIUtility.standardVerticalSpacing;
        }


        private void DisplayEntityCostumeInfo(Rect position, SerializedProperty property) {
            SerializedProperty propEntity = property.FindPropertyRelative( "entity" );
            SerializedProperty propEntityCostume = property.FindPropertyRelative( "entityCostume" );
            float rectEntityHeight = EditorGUI.GetPropertyHeight( propEntity );
            float rectEntityCostumeHeight = EditorGUI.GetPropertyHeight( propEntityCostume );
            Rect rectEntity = new Rect(position.x, position.y + lineSpace, position.width, rectEntityHeight);
            Rect rectEntityCostume = new Rect(position.x, position.y + lineSpace + rectEntityHeight + EditorGUIUtility.standardVerticalSpacing, position.width, rectEntityCostumeHeight);
            EditorGUI.PropertyField( rectEntity, propEntity, GUIContent.none );
            EditorGUI.PropertyField( rectEntityCostume, propEntityCostume, GUIContent.none );
        }
        private float GetEntityCostumeInfoHeight(SerializedProperty property) {
            float propEventTypeHeight = GetDropdownHeight(property);
            float propEntityHeight = EditorGUI.GetPropertyHeight( property.FindPropertyRelative( "entity" ) );
            float propEntityCostumeHeight = EditorGUI.GetPropertyHeight( property.FindPropertyRelative( "entityCostume" ) );
            return propEventTypeHeight + propEntityHeight + propEntityCostumeHeight + EditorGUIUtility.standardVerticalSpacing*2;
        }


        private void DisplayWeaponCostumeInfo(Rect position, SerializedProperty property) {
            SerializedProperty propWeapon = property.FindPropertyRelative( "weapon" );
            SerializedProperty propWeaponCostume = property.FindPropertyRelative( "weaponCostume" );
            float rectWeaponHeight = EditorGUI.GetPropertyHeight( propWeapon );
            float rectWeaponCostumeHeight = EditorGUI.GetPropertyHeight( propWeaponCostume );
            Rect rectWeapon = new Rect(position.x, position.y + lineSpace, position.width, rectWeaponHeight);
            Rect rectWeaponCostume = new Rect(position.x, position.y + lineSpace + rectWeaponHeight + EditorGUIUtility.standardVerticalSpacing, position.width, rectWeaponCostumeHeight);
            EditorGUI.PropertyField( rectWeapon, propWeapon, GUIContent.none );
            EditorGUI.PropertyField( rectWeaponCostume, propWeaponCostume, GUIContent.none );
        }
        private float GetWeaponCostumeInfoHeight(SerializedProperty property) {
            float propEventTypeHeight = GetDropdownHeight(property);
            float propWeaponHeight = EditorGUI.GetPropertyHeight( property.FindPropertyRelative( "weapon" ) );
            float propWeaponCostumeHeight = EditorGUI.GetPropertyHeight( property.FindPropertyRelative( "weaponCostume" ) );
            return propEventTypeHeight + propWeaponHeight + propWeaponCostumeHeight + EditorGUIUtility.standardVerticalSpacing*2;
        }
    }

}