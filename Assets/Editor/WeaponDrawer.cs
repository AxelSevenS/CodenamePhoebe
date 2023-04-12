using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace SeleneGame.Core {

    [CustomPropertyDrawer(typeof(Weapon), true)]
    public class WeaponDrawer : CostumableDrawer<Weapon, WeaponData, WeaponCostume, WeaponModel> {

        public override void SetValue(SerializedProperty property, WeaponData data) {

            ((Weapon)property.managedReferenceValue)?.Dispose();


            // If the weapon has a reference to an ArmedEntity, use that; (This should always be the case as long as the weapon is not null)
            ArmedEntity entityRef = (targetCostumable as Weapon)?.armedEntity;
            WeaponInventory inventory = entityRef?.weapons ??(WeaponInventory)GetParent(property);
            
            if (entityRef == null) {

                if (targetCostumable != null)
                    Debug.LogWarning($"Weapon {targetCostumable} has nulled-out Entity Reference.");

                
                // If the weapon is in a WeaponInventory, get the entity from the inventory
                entityRef = inventory?.entity;

                // If the weapon is on a MonoBehaviour, get the entity from the MonoBehaviour
                entityRef ??= ((MonoBehaviour)property.serializedObject.targetObject).GetComponent<ArmedEntity>();
            }


            if (entityRef == null) {
                Debug.LogError($"No ArmedEntity found for Weapon {targetCostumable}");
                // return;
            }

            property.managedReferenceValue = data?.GetWeapon( entityRef );
            property.serializedObject.ApplyModifiedProperties();

            inventory?.UpdateDisplay();
            
        }
    }
}
