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



            // If the weapon has a reference to an ArmedEntity, use that; (This should always be the case as long as the weapon is not null)
            ArmedEntity entityRef = (targetCostumable as Weapon)?.armedEntity;
            
            if (entityRef == null) {

                if (property.managedReferenceValue != null)
                    Debug.LogWarning($"Weapon {property.managedReferenceValue} has nulled-out Entity Reference. Not good.");

                
                // If the weapon is in a WeaponInventory, get the entity from the inventory
                WeaponInventory inventory = (WeaponInventory)GetParent(property);
                entityRef = inventory?.entity;

                // If the weapon is on a MonoBehaviour, get the entity from the MonoBehaviour
                entityRef ??= ((MonoBehaviour)property.serializedObject.targetObject).GetComponent<ArmedEntity>();
            }


            if (entityRef == null) {
                Debug.LogError($"No ArmedEntity found for Weapon {property.managedReferenceValue}");
                // return;
            }
            

            Type type = Weapon._types[typeIndex];
            property.managedReferenceValue = Weapon.CreateInstance(type, entityRef);
        }
    }
}
