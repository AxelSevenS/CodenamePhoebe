using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace SeleneGame.Core {

    [System.Serializable]
    public abstract class WeaponInventory : IEnumerable, IDisposable {

        [SerializeField] protected ArmedEntity entity;

        [SerializeField] private Weapon _defaultWeapon;
        
        

        public abstract int Count { get; }
        public Weapon this[int index] => Get(index);
        public abstract Weapon current { get; }

        public Weapon defaultWeapon => _defaultWeapon;



        public WeaponInventory(ArmedEntity entity) {
            this.entity = entity;
            _defaultWeapon = Weapon.GetDefaultInstance();
            _defaultWeapon.Initialize(entity);
        }



        public virtual void Dispose() {
            foreach (Weapon weapon in this) {
                weapon?.Dispose();
            }
        }

        public abstract Weapon Get(int index);

        public abstract void Set(int index, Weapon weapon, WeaponCostume costume = null);
        public void Set(int index, string weaponName, WeaponCostume costume = null) {
            Weapon.GetInstanceAsync(weaponName, (weapon) => {
                Set(index, weapon, costume);
            });
        }

        public void Replace(Weapon oldWeapon, Weapon newWeapon, WeaponCostume costume = null){
            if (oldWeapon == null) return;
            
            try {
                int index = IndexOf(oldWeapon);
                Set(index, newWeapon, costume);
            } catch (System.ArgumentOutOfRangeException) {
                
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
