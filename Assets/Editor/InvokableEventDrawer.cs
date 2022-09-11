using UnityEngine;
using UnityEditor;

namespace SeleneGame.Core {

    [CustomPropertyDrawer( typeof( InvokableEvent ), true )]
    public class InvokableEventDrawer : PropertyDrawer {

        private float lineSpace = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;


        public override void OnGUI( Rect position, SerializedProperty property, GUIContent label ) {
            SerializedProperty propEventType = property.FindPropertyRelative( "eventType" );

            EditorGUI.BeginProperty( position, label, property );

            Rect rectType = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            EditorGUI.PropertyField( rectType, propEventType, GUIContent.none );

            switch (propEventType.enumValueIndex) {
                    // DisplayDialogueInfo(position, property);
                    // break;
                case 0:
                    DisplayEntityCostumeInfo(position, property);
                    break;
                case 1:
                    DisplayPlayerCostumeInfo(position, property);
                    break;
                // case 2:
            }

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            int eventType = property.FindPropertyRelative( "eventType" ).enumValueIndex;

            switch (eventType) {
                    // return GetDialogueInfoHeight(property);
                case 0:
                    return GetEntityCostumeInfoHeight(property);
                case 1:
                    return GetPlayerCostumeInfoHeight(property);
                // case 2:
                default:
                    return GetDropdownHeight(property);
            }

        }

        private float GetDropdownHeight(SerializedProperty property){
            return EditorGUI.GetPropertyHeight( property.FindPropertyRelative( "eventType" ) );
        }


        // private void DisplayDialogueInfo(Rect position, SerializedProperty property) {
        //     SerializedProperty propDialogue = property.FindPropertyRelative( "dialogue" );
        //     Rect rectDialogue = new Rect(position.x, position.y + lineSpace, position.width, EditorGUI.GetPropertyHeight( propDialogue ));
        //     EditorGUI.PropertyField( rectDialogue, propDialogue, GUIContent.none );
        // }
        // private float GetDialogueInfoHeight(SerializedProperty property) {
        //     float propEventTypeHeight = GetDropdownHeight(property);
        //     float propDialogueHeight = EditorGUI.GetPropertyHeight( property.FindPropertyRelative( "dialogue" ) );
        //     return propEventTypeHeight + propDialogueHeight + EditorGUIUtility.standardVerticalSpacing;
        // }


        private void DisplayEntityCostumeInfo(Rect position, SerializedProperty property) {
            SerializedProperty propEntity = property.FindPropertyRelative( "entity" );
            SerializedProperty propEntityCostume = property.FindPropertyRelative( "characterCostume" );
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
            float propEntityCostumeHeight = EditorGUI.GetPropertyHeight( property.FindPropertyRelative( "characterCostume" ) );
            return propEventTypeHeight + propEntityHeight + propEntityCostumeHeight + EditorGUIUtility.standardVerticalSpacing*2;
        }


        private void DisplayPlayerCostumeInfo(Rect position, SerializedProperty property) {
            SerializedProperty propEntityCostume = property.FindPropertyRelative( "characterCostume" );
            float rectEntityCostumeHeight = EditorGUI.GetPropertyHeight( propEntityCostume );
            Rect rectEntityCostume = new Rect(position.x, position.y + lineSpace, position.width, rectEntityCostumeHeight);
            EditorGUI.PropertyField( rectEntityCostume, propEntityCostume, GUIContent.none );
        }
        private float GetPlayerCostumeInfoHeight(SerializedProperty property) {
            float propEventTypeHeight = GetDropdownHeight(property);
            float propEntityCostumeHeight = EditorGUI.GetPropertyHeight( property.FindPropertyRelative( "characterCostume" ) );
            return propEventTypeHeight + propEntityCostumeHeight + EditorGUIUtility.standardVerticalSpacing*2;
        }
    }

}