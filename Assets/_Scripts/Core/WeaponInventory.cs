using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SeleneGame.Entities;
using SeleneGame.Weapons;

namespace SeleneGame.Core {

    [System.Serializable]
    public class WeaponInventory : IEnumerable {
        private ArmedEntity entity;
        [SerializeReference] private Weapon[] items;
        
        private const int defaultIndex = 0;
        [SerializeField] private int currentIndex;

        public Weapon this[int index] => items[index];

        public Weapon current { get {
            try {
                return items[currentIndex];
            } catch {
                currentIndex = defaultIndex;
                return items[currentIndex];
            }
        } }

        public WeaponInventory(ArmedEntity entity, byte size){
            this.entity = entity;
            items = new Weapon[size];
            for (int i = 0; i < items.Length; i++) {
                Set( i, new UnarmedWeapon());
            }
        }

        public void Set(int index, Weapon weapon){
            weapon.entity = entity;
            weapon.OnAdd();
            try {
                items[index] = weapon;
            } catch {
                Debug.LogError("WeaponInventory: Index out of range; Set");
            }
        }

        public void Set(int index, System.Type weaponType){
            Weapon weapon = (Weapon)System.Activator.CreateInstance(weaponType);
            weapon.entity = entity;
            weapon.OnAdd();
            try {
                items[index] = weapon;
            } catch {
                Debug.LogError("WeaponInventory: Index out of range; Set");
            }
        }

        public void Remove(int index) {
            if (index == defaultIndex) return;

            try {
                items[index].OnRemove();
                items[index] = new UnarmedWeapon();
            } catch {
                Debug.LogError("WeaponInventory: Index out of range; Remove");
            }
        }

        public void Switch(int index){
            if (index == currentIndex) return;

            foreach ( Weapon weapon in items )
                weapon.Hide();

            try {
                Weapon newItem = items[index];
                currentIndex = index;
            } catch {
                Debug.LogError("WeaponInventory: Index out of range; Switching to default weapon");
                currentIndex = defaultIndex;
            }

            current.Display();
        }

        public IEnumerator GetEnumerator() => items.GetEnumerator();

        public int Length => items.Length;
    }
}
