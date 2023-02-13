using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace SeleneGame.Core {

    [CustomPropertyDrawer(typeof(Weapon), true)]
    public class WeaponDrawer : CostumableDrawer<Weapon, WeaponCostume, WeaponModel> {

        public override void SetValue(SerializedProperty property, int typeIndex) {



            ((Weapon)property.managedReferenceValue)?.Dispose();

            if (typeIndex <= -1) {
                property.managedReferenceValue = null;
                return;
            }

            WeaponInventory inventory = (WeaponInventory)GetParent(property);

            ArmedEntity entityRef = 
                (targetCostumable as Weapon)?.entity ??
                inventory?.entity ??
                (ArmedEntity)GetParent(property) ?? 
                ((MonoBehaviour)property.serializedObject.targetObject).GetComponent<ArmedEntity>();

            Type type = Weapon._types[typeIndex];
            property.managedReferenceValue = Weapon.CreateInstance(type, entityRef);
        }

        // private static string[] weaponOptions;
        // int weaponTypeIndex = 0;


        // static WeaponDrawer() {
            
        //     weaponOptions = new string[Weapon._types.Count + 1];
        //     weaponOptions[0] = "None";
        //     for (int i = 0; i < Weapon._types.Count; i++) {
        //         weaponOptions[i + 1] = Weapon._types[i].Name;
        //     }
            
        // }

        // public override void PreWork(Rect position, SerializedProperty property, GUIContent label) {
        //     base.PreWork(position, property, label);

        //     // WeaponInventory parent = (WeaponInventory)GetParent(property);

        //     // if (parent == null) {
        //     //     EditorGUI.PropertyField(position, property, label);
        //     //     return;
        //     // }



        //     if (targetCostumable == null)
        //         weaponTypeIndex = 0;
        //     else 
        //         weaponTypeIndex = Weapon._types.IndexOf( targetCostumable.GetType() ) + 1;

        //     Rect valueRect = new Rect(position.x, position.y, position.width/* /2f */, position.height);
        //     Rect buttonRect = new Rect(position.x + position.width/2f, position.y, position.width/2f, EditorGUIUtility.singleLineHeight);
            

        //     EditorGUI.BeginChangeCheck();

        //     weaponTypeIndex = EditorGUI.Popup(
        //         buttonRect,
        //         weaponTypeIndex,
        //         weaponOptions);

        //     if (EditorGUI.EndChangeCheck()) {
        //         ((Weapon)property.managedReferenceValue)?.Dispose();
        //         property.managedReferenceValue = null;
        //         if (weaponTypeIndex != 0) {
        //             property.managedReferenceValue = Weapon.CreateInstance(Weapon._types[weaponTypeIndex - 1], parent.entity);
        //         }
        //         // if (weaponTypeIndex == 0) {
        //         //     parent.Set( parent.IndexOf(property.managedReferenceValue as Weapon), null );
        //         // } else if (Weapon._types[weaponTypeIndex - 1] != null) {
        //         //     parent.Set( parent.IndexOf(property.managedReferenceValue as Weapon), Weapon._types[weaponTypeIndex - 1] );
        //         // }
        //     }
        // }
    }
}
