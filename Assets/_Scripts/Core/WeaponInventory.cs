using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SeleneGame.Entities;
using SeleneGame.Weapons;

namespace SeleneGame.Core {

    [System.Serializable]
    public class WeaponInventory : IEnumerable {
        private ArmedEntity entity;
        [SerializeReference] private List<Weapon> items = new List<Weapon>();
        
        private const int defaultIndex = 0;
        [SerializeField] private int currentIndex;

        public Weapon currentItem { get {
            try {
                return items[currentIndex];
            } catch {
                return items[defaultIndex];
            }
        } }

        public WeaponInventory(ArmedEntity entity){
            this.entity = entity;
            Set( defaultIndex, new UnarmedWeapon());
        }

        public void Set(int index, Weapon weapon){
            weapon.entity = entity;
            weapon.OnAdd();
            try {
                items[index] = weapon;
            } catch {
                items.Add(weapon);
            }
        }

        public void Remove(int index) {
            if (index == defaultIndex) return;

            items[index].OnRemove();
            items[index] = null;
        }

        public void Switch(int index){
            if (index == currentIndex) return;

            foreach ( Weapon weapon in items )
                weapon.Hide();

            try {
                Weapon newItem = items[index];
                currentIndex = index;
            } catch ( KeyNotFoundException ) {
                currentIndex = defaultIndex;
            }

            currentItem.Display();
        }

        public IEnumerator GetEnumerator() => items.GetEnumerator();
    }
}
