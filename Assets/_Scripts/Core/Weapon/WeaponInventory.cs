using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace SeleneGame.Core {

    [System.Serializable]
    public abstract class WeaponInventory : IEnumerable {

        protected ArmedEntity entity;
        

        public abstract int Count { get; }
        public Weapon this[int index] => Get(index);
        public abstract Weapon current { get; }



        public abstract Weapon Get(int index);

        public abstract void Set(int index, Weapon weapon, WeaponCostume costume = null);
        public void Set(int index, string weaponName, WeaponCostume costume = null) {
            Weapon.GetAsync(weaponName, (weapon) => {
                Set(index, weapon, costume);
            });
        }
        public void SetToDefault(int index){
            try {
                Weapon.GetDefaultAsync((weapon) => {
                    Set(index, weapon);
                });
            } catch (System.Exception e) {
                Debug.LogError($"Error setting weapon at index {index} in WeaponInventory : {e.Message}.");
            }
        }

        public void Replace(Weapon oldWeapon, Weapon newWeapon, WeaponCostume costume = null){
            int index = IndexOf(oldWeapon);
            if (index == -1) return;

            Set(index, newWeapon, costume);
        }

        public int IndexOf(Weapon weapon){
            for (int i = 0; i < Count; i++) {
                if (this[i] == weapon) return i;
            }
            return -1;
        }

        public abstract void Remove(int index);

        public abstract void Switch(int index);

        public abstract IEnumerator GetEnumerator();
    }
}
