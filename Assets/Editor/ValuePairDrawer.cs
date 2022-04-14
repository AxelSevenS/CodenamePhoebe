using UnityEngine;
using UnityEditor;

namespace SeleneGame.EditorUI {

    [CustomPropertyDrawer( typeof( SeleneGame.Core.ValuePair ), true )]
    public class ValuePairDrawer : PropertyDrawer {

        public override void OnGUI( Rect position, SerializedProperty property, GUIContent label ) {
            EditorGUI.BeginProperty( position, label, property );

            float halfWidth = position.width/2f;
            Rect leftRect = new Rect(position.x - 2f, position.y, halfWidth, position.height);
            Rect rightRect = new Rect(position.x + halfWidth, position.y, halfWidth, position.height);

            EditorGUI.PropertyField( leftRect, property.FindPropertyRelative( "valueOne" ), GUIContent.none );
            EditorGUI.PropertyField( rightRect, property.FindPropertyRelative( "valueTwo" ), GUIContent.none );

            EditorGUI.EndProperty();
        }
    }
}