// using UnityEngine;
// using UnityEditor;

// namespace SeleneGame.EditorUI {

//     [CustomPropertyDrawer( typeof( SeleneGame.Core.WeaponInventory ), true )]
//     public class WeaponInventoryDrawer : PropertyDrawer {

//         public override void OnGUI( Rect position, SerializedProperty property, GUIContent label ) {
//             EditorGUI.BeginProperty( position, label, property );

//             EditorGUI.PropertyField( position, property.FindPropertyRelative( "items" ), label, includeChildren:false );

//             EditorGUI.EndProperty();
//         }
        
//         public override float GetPropertyHeight (SerializedProperty property, GUIContent label) {
//             return EditorGUI.GetPropertyHeight( property.FindPropertyRelative( "items" ) );

//         }
//     }
// }