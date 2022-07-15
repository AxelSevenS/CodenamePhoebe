using UnityEngine;
using UnityEditor;

namespace SevenGame.Utility {

    [CustomPropertyDrawer( typeof( Map ), true )]
    public class MapDrawer : PropertyDrawer {

        public override void OnGUI( Rect position, SerializedProperty property, GUIContent label ) {
            EditorGUI.BeginProperty( position, label, property );

            EditorGUI.PropertyField( position, property.FindPropertyRelative( "pairs" ), label, includeChildren:true );

            EditorGUI.EndProperty();
        }
        
        public override float GetPropertyHeight (SerializedProperty property, GUIContent label) {

            SerializedProperty pairList = property.FindPropertyRelative( "pairs" );
    
            if ( !pairList.isExpanded ) return EditorGUIUtility.singleLineHeight;

            int count = pairList.CountInProperty();
            return (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing) * (System.Math.Max(count, 3) + 0.5f);

        }
    }
}