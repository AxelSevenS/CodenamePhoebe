// using UnityEngine;
// using UnityEditor;
// using System.Reflection;
// using System;
// using System.Linq;
// using System.Collections;
// using SeleneGame.Core;

// [CustomPropertyDrawer(typeof(ButtonAttribute))]
// public class ButtonDrawer : PropertyDrawer {
	
// 	private static string[] weaponOptions;
// 	int weaponTypeIndex = 0;


// 	[UnityEditor.Callbacks.DidReloadScripts]
// 	static ButtonDrawer() {
// 		Debug.Log("PD");
// 		weaponOptions = new string[Weapon._types.Count + 1];
// 		weaponOptions[0] = "None";
// 		for (int i = 0; i < Weapon._types.Count; i++) {
// 			weaponOptions[i + 1] = Weapon._types[i].Name;
// 		}
		
// 	}

// 	public override void OnGUI (Rect position, SerializedProperty property, GUIContent label) {
// 		// ButtonAttribute battribute = attribute as ButtonAttribute;
		

// 		EditorGUI.PropertyField(valueRect, property, label, true);
// 	}

// 	public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
// 		return EditorGUI.GetPropertyHeight(property, label, true);
// 	}
// }