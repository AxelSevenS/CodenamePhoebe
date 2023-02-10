using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SevenGame.Utility;
using System;

namespace SeleneGame.Core {

    [System.Serializable]
    public class ListWeaponInventory : WeaponInventory {

        [SerializeReference] private Weapon[] items;
        
        [SerializeField] private int currentIndex;



        public override int Count => items.Length;

        public override Weapon current => Get(currentIndex) ?? defaultWeapon;



        public ListWeaponInventory(ArmedEntity entity, int size) : base(entity) {
            currentIndex = 0;

            items = new Weapon[size];
        }


        public override Weapon Get(int index) => items[index] ?? defaultWeapon;

        public override void Set(int index, Type weaponType, WeaponCostume costume = null){
            try {
                // weapon = Weapon.Initialize(weapon, entity, costume);
                items[index]?.Dispose();
                items[index] = Weapon.CreateInstance(weaponType, entity, costume);
            } catch (System.Exception e) {
                Debug.LogError($"Error setting weapon at index {index} in WeaponInventory : {e.Message}.");
            }
        }

        public override void Remove(int index) {
            try {
                items[index]?.Dispose();
                items[index] = null;
            } catch (System.Exception e) {
                Debug.LogError($"Error removing weapon at index {index} in WeaponInventory : {e.Message}.");
            }
        }

        public override void Switch(int index){
            if (index == currentIndex) return;

            foreach ( Weapon weapon in items )
                weapon.OnUnequip();

            if ( items[index] != null ) {
                currentIndex = index;
            } else {
                Debug.LogError($"Error switching to weapon at index {index} in WeaponInventory; Switching to Default Weapon");
            }

            current.OnEquip();
        }



        public override IEnumerator GetEnumerator() => items.GetEnumerator();
    }
}
