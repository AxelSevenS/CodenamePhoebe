using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace SeleneGame.Core {

    [System.Serializable]
    public abstract class WeaponInventory : IEnumerable {

        [SerializeField] protected ArmedEntity entity;
        

        public abstract int Count { get; }
        public Weapon this[int index] => Get(index);
        public abstract Weapon current { get; }



        public abstract Weapon Get(int index);

        public abstract void Set(int index, Weapon weapon, WeaponCostume costume = null);
        public void Set(int index, string weaponName, WeaponCostume costume = null) {
            Weapon.GetInstanceAsync(weaponName, (weapon) => {
                Set(index, weapon, costume);
            });
        }
        public void SetToDefault(int index){
            try {
                Weapon.GetDefaultInstanceAsync((weapon) => {
                    Set(index, weapon);
                });
            } catch (System.Exception e) {
                Debug.LogError($"Error setting weapon at index {index} in WeaponInventory : {e.Message}.");
            }
        }

        public void Replace(Weapon oldWeapon, Weapon newWeapon, WeaponCostume costume = null){
            try {
                int index = IndexOf(oldWeapon);
                Set(index, newWeapon, costume);
            } catch (System.ArgumentOutOfRangeException ) {
                
            }
        }

        public int IndexOf(Weapon weapon){
            for (int i = 0; i < Count; i++) {
                if (this[i] == weapon) return i;
            }
            
            throw new System.ArgumentOutOfRangeException();
        }

        public abstract void Remove(int index);

        public abstract void Switch(int index);

        public abstract IEnumerator GetEnumerator();
    }
}
