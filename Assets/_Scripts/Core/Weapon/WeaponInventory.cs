using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Scribe;
using SevenGame.Utility;
using UnityEngine;

namespace SeleneGame.Core {

    [System.Serializable]
    public abstract class WeaponInventory : IEnumerable, IDisposable {

        [SerializeField] [ReadOnly] protected ArmedEntity _entity;

        [SerializeReference] /* [ReadOnly] */ private Weapon _defaultWeapon;
        
        

        public abstract int Count { get; }
        public Weapon this[int index] => Get(index);
        public abstract Weapon current { get; }

        public Weapon defaultWeapon => _defaultWeapon;
        public ArmedEntity entity => _entity;



        public WeaponInventory(ArmedEntity entity) {
            _entity = entity;
            // _defaultWeapon = AddressablesUtils.GetDefaultAsset<WeaponData>().GetWeapon(entity);
            _defaultWeapon?.Display();
        }


        public virtual void HandleInput(Player playerController) {
            current?.HandleInput(playerController);
        }

        public virtual void LightAttack() {
            current?.LightAttack();
        }

        public virtual void HeavyAttack() {
            current?.HeavyAttack();
        }




        public virtual void Dispose() {
            // foreach (Weapon weapon in this) {
            //     weapon?.Dispose();
            // }
        }

        public abstract Weapon Get(int index);

        // public void Set<TWeapon>(int index, WeaponCostume costume = null) where TWeapon : Weapon {
        //     Set(index, typeof(TWeapon), costume);
        // }
        public abstract void Set(int index, WeaponData data, WeaponCostume costume = null);


        // public void Replace<TOldWeapon, TNewWeapon>(int index, WeaponCostume costume = null) where TOldWeapon : Weapon where TNewWeapon : Weapon {
        //     Replace(typeof(TOldWeapon), typeof(TNewWeapon), costume);
        // }
        public void Replace(Weapon oldWeapon, WeaponData newWeaponData, WeaponCostume costume = null) {

            if (oldWeapon.Data == newWeaponData) return;
            
            try {
                int index = IndexOf(oldWeapon);
                Set(index, newWeaponData, costume);
            } catch (System.ArgumentOutOfRangeException) {
                
            }
        }


        public abstract int IndexOf(Weapon weapon);


        // protected void TypeCheck(Type weaponType) {
        //     if (!typeof(Weapon).IsAssignableFrom(weaponType))
        //         throw new System.ArgumentException("Type must be a Weapon type.", "weaponType");
        // }


        public abstract void Remove(int index);

        public abstract void Switch(int index);

        public abstract IEnumerator GetEnumerator();


        public void UpdateDisplay() {
            _defaultWeapon?.OnUnequip();
            foreach (Weapon weapon in this) {
                weapon?.OnUnequip();
            }
            current?.OnEquip();
        }



        public virtual void Update() {
            foreach (Weapon weapon in this) {
                weapon?.Update();
            }
        }

        public virtual void LateUpdate() {
            foreach (Weapon weapon in this) {
                weapon?.LateUpdate();
            }
        }

        public virtual void FixedUpdate() {
            foreach (Weapon weapon in this) {
                weapon?.FixedUpdate();
            }
        }
    }
}
