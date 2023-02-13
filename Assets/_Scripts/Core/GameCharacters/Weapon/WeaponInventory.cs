using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using SevenGame.Utility;
using UnityEngine;

namespace SeleneGame.Core {

    [System.Serializable]
    public abstract class WeaponInventory : IEnumerable, IDisposable {

        [SerializeField] [ReadOnly] protected ArmedEntity _entity;

        [SerializeReference] [ReadOnly] private Weapon _defaultWeapon;
        
        

        public abstract int Count { get; }
        public Weapon this[int index] => Get(index);
        public abstract Weapon current { get; }

        public Weapon defaultWeapon => _defaultWeapon;
        public ArmedEntity entity => _entity;



        public WeaponInventory(ArmedEntity entity) {
            _entity = entity;
            // _defaultWeapon = Weapon.Initialize(Weapon.GetDefaultAsset(), entity);
            _defaultWeapon = new UnarmedWeapon(entity);
        }



        public virtual void Dispose() {
            // foreach (Weapon weapon in this) {
            //     weapon?.Dispose();
            // }
        }

        public abstract Weapon Get(int index);

        public void Set<TWeapon>(int index, WeaponCostume costume = null) where TWeapon : Weapon {
            Set(index, typeof(TWeapon), costume);
        }
        public abstract void Set(int index, Type weapon, WeaponCostume costume = null);


        public void Replace<TOldWeapon, TNewWeapon>(int index, WeaponCostume costume = null) where TOldWeapon : Weapon where TNewWeapon : Weapon {
            Replace(typeof(TOldWeapon), typeof(TNewWeapon), costume);
        }
        public void Replace(Type oldWeaponType, Type newWeaponType, WeaponCostume costume = null) {

            TypeCheck(oldWeaponType);

            if (newWeaponType != null)
                TypeCheck(newWeaponType);

            if (oldWeaponType == newWeaponType) return;
            
            try {
                int index = IndexOf(oldWeaponType);
                Set(index, newWeaponType, costume);
            } catch (System.ArgumentOutOfRangeException) {
                
            }
        }


        public int IndexOf<TWeapon>() where TWeapon : Weapon {
            return IndexOf(typeof(TWeapon));
        }
        public int IndexOf(Type weaponType){

            TypeCheck(weaponType);

            for (int i = 0; i < Count; i++) {
                if (this[i].GetType() == weaponType) return i;
            }
            
            throw new System.ArgumentOutOfRangeException();
        }
        public abstract int IndexOf(Weapon weapon);


        protected void TypeCheck(Type weaponType) {
            if (!typeof(Weapon).IsAssignableFrom(weaponType))
                throw new System.ArgumentException("Type must be a Weapon type.", "weaponType");
        }


        public abstract void Remove(int index);

        public abstract void Switch(int index);

        public abstract IEnumerator GetEnumerator();
    }
}
