using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SeleneGame.Core {

    [System.Serializable]
    public class WeaponInventory : IEnumerable {
        private ArmedEntity entity;
        [SerializeReference] private Weapon[] items;
        
        private const int defaultIndex = 0;
        [SerializeField] private int currentIndex;

        public int Count => items.Length;

        public Weapon this[int index] => items[index];

        public Weapon current { get {
            try {
                return items[currentIndex];
            } catch {
                currentIndex = defaultIndex;
                return items[currentIndex];
            }
        } }

        public WeaponInventory(ArmedEntity entity, int size){
            currentIndex = defaultIndex;
            
            this.entity = entity;

            items = new Weapon[size];
            for (int i = 0; i < items.Length; i++) {
                Set( i, new UnarmedWeapon());
            }
        }

        public void Set(int index, Weapon weapon){
            try {
                items[index] = weapon;
                items[index].OnAdd(entity);
            } catch (System.Exception e) {
                Debug.LogError($"Error setting weapon at index {index} in WeaponInventory : {e.Message}.");
            }
        }

        public void Set(int index, System.Type weaponType){
            Weapon weapon = (Weapon)System.Activator.CreateInstance(weaponType);
            Set(index, weapon);
        }

        public void Remove(int index) {
            if (index == defaultIndex) return;

            try {
                items[index].OnRemove();
                items[index] = new UnarmedWeapon();
            } catch (System.Exception e) {
                Debug.LogError($"Error removing weapon at index {index} in WeaponInventory : {e.Message}.");
            }
        }

        public void Switch(int index){
            if (index == currentIndex) return;

            foreach ( Weapon weapon in items )
                weapon.Hide();

            try {
                Weapon newItem = items[index];
                currentIndex = index;
            } catch (System.Exception e) {
                Debug.LogError($"Error switching to weapon at index {index} in WeaponInventory : {e.Message}; Switching to Default Weapon");
                currentIndex = defaultIndex;
            }

            current.Display();
        }

        public IEnumerator GetEnumerator() => items.GetEnumerator();
    }
}
