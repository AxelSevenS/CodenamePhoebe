using UnityEngine;
using UnityEditor;
using SeleneGame.Core;

namespace SeleneGame.EditorUI {

    [CustomPropertyDrawer( typeof( InvokableEvent ), true )]
    public class InvokableEventEditor : PropertyDrawer {

        private float lineSpace = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

        public override void OnGUI( Rect position, SerializedProperty property, GUIContent label ) {
            SerializedProperty propEventType = property.FindPropertyRelative( "eventType" );
            
            SerializedProperty propConversation = property.FindPropertyRelative( "conversation" );
            SerializedProperty propEntity = property.FindPropertyRelative( "entity" );
            SerializedProperty propEntityCostume = property.FindPropertyRelative( "entityCostume" );
            SerializedProperty propWeapon = property.FindPropertyRelative( "weapon" );
            SerializedProperty propWeaponCostume = property.FindPropertyRelative( "weaponCostume" );
            

            EditorGUI.BeginProperty( position, label, property );

            Rect rectType = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            EditorGUI.PropertyField( rectType, propEventType, GUIContent.none );

            switch (propEventType.enumValueIndex) {
                case 0:
                    Rect rectConversation = new Rect(position.x, position.y + lineSpace, position.width, EditorGUI.GetPropertyHeight( propConversation ));
                    EditorGUI.PropertyField( rectConversation, propConversation, GUIContent.none );
                    break;
                case 1:
                    float rectEntityHeight = EditorGUI.GetPropertyHeight( propEntity );
                    float rectEntityCostumeHeight = EditorGUI.GetPropertyHeight( propEntityCostume );
                    Rect rectEntity = new Rect(position.x, position.y + lineSpace, position.width, rectEntityHeight);
                    Rect rectEntityCostume = new Rect(position.x, position.y + lineSpace + rectEntityHeight + EditorGUIUtility.standardVerticalSpacing, position.width, rectEntityCostumeHeight);
                    EditorGUI.PropertyField( rectEntity, propEntity, GUIContent.none );
                    EditorGUI.PropertyField( rectEntityCostume, propEntityCostume, GUIContent.none );
                    break;
                case 2:
                    float rectWeaponHeight = EditorGUI.GetPropertyHeight( propWeapon );
                    float rectWeaponCostumeHeight = EditorGUI.GetPropertyHeight( propWeaponCostume );
                    Rect rectWeapon = new Rect(position.x, position.y + lineSpace, position.width, rectWeaponHeight);
                    Rect rectWeaponCostume = new Rect(position.x, position.y + lineSpace + rectWeaponHeight + EditorGUIUtility.standardVerticalSpacing, position.width, rectWeaponCostumeHeight);
                    EditorGUI.PropertyField( rectWeapon, propWeapon, GUIContent.none );
                    EditorGUI.PropertyField( rectWeaponCostume, propWeaponCostume, GUIContent.none );
                    break;
            }

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight (SerializedProperty property, GUIContent label) {
            int eventType = property.FindPropertyRelative( "eventType" ).enumValueIndex;

            float propEventTypeHeight = EditorGUI.GetPropertyHeight( property.FindPropertyRelative( "eventType" ) );

            float propConversationHeight = EditorGUI.GetPropertyHeight( property.FindPropertyRelative( "conversation" ) );
            float propEntityHeight = EditorGUI.GetPropertyHeight( property.FindPropertyRelative( "entity" ) );
            float propEntityCostumeHeight = EditorGUI.GetPropertyHeight( property.FindPropertyRelative( "entityCostume" ) );
            float propWeaponHeight = EditorGUI.GetPropertyHeight( property.FindPropertyRelative( "weapon" ) );
            float propWeaponCostumeHeight = EditorGUI.GetPropertyHeight( property.FindPropertyRelative( "weaponCostume" ) );

            switch (eventType) {
                case 0:
                    return propEventTypeHeight + propConversationHeight + EditorGUIUtility.standardVerticalSpacing;
                case 1:
                    return propEventTypeHeight + propEntityHeight + propEntityCostumeHeight + EditorGUIUtility.standardVerticalSpacing*2;
                case 2:
                    return propEventTypeHeight + propWeaponHeight + propWeaponCostumeHeight + EditorGUIUtility.standardVerticalSpacing*2;
                default:
                    return propEventTypeHeight;
            }

        }
    }

}