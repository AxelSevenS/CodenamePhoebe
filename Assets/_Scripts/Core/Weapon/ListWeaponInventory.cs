using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SevenGame.Utility;

namespace SeleneGame.Core {

    [System.Serializable]
    public class ListWeaponInventory : WeaponInventory {

        [SerializeReference] [ReadOnly] private Weapon[] items;
        
        private const int defaultIndex = 0;
        [SerializeField] private int currentIndex;



        public override int Count => items.Length;

        public override Weapon current { get {
            try {
                return items[currentIndex];
            } catch {
                currentIndex = defaultIndex;
                return items[currentIndex];
            }
        } }



        public ListWeaponInventory(ArmedEntity entity, int size){
            currentIndex = defaultIndex;
            
            this.entity = entity;

            items = new Weapon[size];
            for (int i = 0; i < items.Length; i++) {
                SetToDefault( i );
            }
        }


        public override Weapon Get(int index) => items[index];

        public override void Set(int index, Weapon weapon, WeaponCostume costume = null){
            try {
                items[index] = weapon;
                items[index].Initialize(entity, costume);
            } catch (System.Exception e) {
                Debug.LogError($"Error setting weapon at index {index} in WeaponInventory : {e.Message}.");
            }
        }

        public override void Remove(int index) {
            if (index == defaultIndex) return;

            try {
                SetToDefault( index );
            } catch (System.Exception e) {
                Debug.LogError($"Error removing weapon at index {index} in WeaponInventory : {e.Message}.");
            }
        }

        public override void Switch(int index){
            if (index == currentIndex) return;

            foreach ( Weapon weapon in items )
                weapon.Hide();

            if ( items[index] != null ) {
                currentIndex = index;
            } else {
                Debug.LogError($"Error switching to weapon at index {index} in WeaponInventory; Switching to Default Weapon");
                currentIndex = defaultIndex;
            }

            current.Display();
        }



        public override IEnumerator GetEnumerator() => items.GetEnumerator();
    }
}
