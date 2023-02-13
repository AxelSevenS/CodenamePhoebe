using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SeleneGame.Core;

using SevenGame.Utility;
using System;

namespace SeleneGame.Content {

    [System.Serializable]
    public class MaskedWeaponInventory : WeaponInventory {

        private WeaponIndex _currentIndex;

        [SerializeReference] private Weapon primaryWeapon;
        [SerializeReference] private Weapon secondaryWeapon;
        [SerializeReference] private Weapon tertiaryWeapon;

        public override int Count => 3;

        public override Weapon Get(int index) {
            switch (index) {
                case (int)WeaponIndex.primary:
                    return primaryWeapon ?? defaultWeapon;
                case (int)WeaponIndex.secondary:
                    return secondaryWeapon ?? defaultWeapon;
                case (int)WeaponIndex.tertiary:
                    return tertiaryWeapon ?? defaultWeapon;
                default:
                    return defaultWeapon;
            }
        }


        public override Weapon current => Get((int)_currentIndex) ?? defaultWeapon;

        public MaskedWeaponInventory(ArmedEntity entity) : base(entity) {
            Switch((int)WeaponIndex.primary);
        }

        public override void Set(int index, Type weaponType, WeaponCostume costume = null){
            // weapon = Weapon.Initialize(weapon, entity, costume);
            try {
                switch (index) {
                    case (int)WeaponIndex.primary:
                        primaryWeapon?.Dispose();
                        primaryWeapon = Weapon.CreateInstance(weaponType, entity, costume);
                        break;
                    case (int)WeaponIndex.secondary:
                        secondaryWeapon?.Dispose();
                        secondaryWeapon = Weapon.CreateInstance(weaponType, entity, costume);
                        break;
                    case (int)WeaponIndex.tertiary:
                        tertiaryWeapon?.Dispose();
                        tertiaryWeapon = Weapon.CreateInstance(weaponType, entity, costume);
                        break;
                    default:
                        throw new System.IndexOutOfRangeException($"Index {index} is out of range for MaskedWeaponInventory");
                }
            } catch (System.Exception e) {
                Debug.LogError($"Error setting weapon at index {index} in WeaponInventory : {e.Message}.");
            }
        }

        public override void Remove(int index) {
            try {
                Get(index)?.Dispose();
                Set(index, null);
            } catch (System.Exception e) {
                Debug.LogError($"Error removing weapon at index {index} in WeaponInventory : {e.Message}.");
            }
        }

        public override void Switch(int index){
            if (index == (int)_currentIndex) return;

            current.OnUnequip();
            _currentIndex = (WeaponIndex)index;
            current.OnEquip();
        }

        public override IEnumerator GetEnumerator() => new MaskedWeaponInventoryEnumerator(this);

        private class MaskedWeaponInventoryEnumerator : IEnumerator {
            private MaskedWeaponInventory inventory;
            private int index;

            public MaskedWeaponInventoryEnumerator(MaskedWeaponInventory inventory) {
                this.inventory = inventory;
                index = -1;
            }
            
            public object Current => inventory[index];

            public bool MoveNext(){
                index++;
                return index < 3;
            }
            public void Reset(){
                index = -1;
            }
        }


        private enum WeaponIndex { primary = 0, secondary = 1, tertiary = 2 }
    }
}
