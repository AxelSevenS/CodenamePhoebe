using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SeleneGame.Core;

namespace SeleneGame.Weapons {

    [System.Serializable]
    public class MaskedWeaponInventory : WeaponInventory {

        private enum WeaponIndex { primary = 0, secondary = 1, tertiary = 2 }
        private WeaponIndex _currentIndex;

        [SerializeReference] public Weapon primaryWeapon;
        [SerializeReference] public Weapon secondaryWeapon;
        [SerializeReference] public Weapon tertiaryWeapon;

        public override int Count => 3;

        public override Weapon Get(int index) {
            switch (index) {
                case (int)WeaponIndex.primary:
                    return primaryWeapon;
                case (int)WeaponIndex.secondary:
                    return secondaryWeapon;
                case (int)WeaponIndex.tertiary:
                    return tertiaryWeapon;
                default:
                    return null;
            }
        }


        public override Weapon current {
            get {
                try {
                    return this[(int)_currentIndex];
                } catch {
                    _currentIndex = WeaponIndex.primary;
                    return this[(int)_currentIndex];
                }
            }
        }

        public MaskedWeaponInventory(ArmedEntity entity){
            this.entity = entity;
            Weapon defaultWeapon = Weapon.GetDefault();
            Set(0, defaultWeapon);
            Set(1, defaultWeapon);
            Set(2, defaultWeapon);
            Switch((int)WeaponIndex.primary);
        }

        public override void Set(int index, Weapon weapon, WeaponCostume costume = null){
            Debug.Log(weapon);
            switch (index) {
                case 0:
                    primaryWeapon = weapon;
                    primaryWeapon.Initialize(entity, costume);
                    break;
                case 1:
                    secondaryWeapon = weapon;
                    secondaryWeapon.Initialize(entity, costume);
                    break;
                case 2:
                    tertiaryWeapon = weapon;
                    tertiaryWeapon.Initialize(entity, costume);
                    break;
                default:
                    Debug.LogError($"Error setting weapon at index {index} in WeaponInventory : Index out of bounds.");
                    break;
            }

        }

        public override void Remove(int index) {
            if (index > 2 || index < 0) {
                Debug.LogError($"Error removing weapon at index {index} in WeaponInventory : Index out of bounds.");
                return;
            }
            SetToDefault( index );
        }

        public override void Switch(int index){
            if (index == (int)_currentIndex) return;

            switch (index) {
                case 0:
                    _currentIndex = WeaponIndex.primary;
                    secondaryWeapon.Hide();
                    tertiaryWeapon.Hide();
                    break;
                case 1:
                    primaryWeapon.Hide();
                    _currentIndex = WeaponIndex.secondary;
                    tertiaryWeapon.Hide();
                    break;
                case 2:
                    primaryWeapon.Hide();
                    secondaryWeapon.Hide();
                    _currentIndex = WeaponIndex.tertiary;
                    break;
                default:
                    Debug.LogError($"Error switching weapon at index {index} in WeaponInventory : Index out of bounds.");
                    break;
            }
            current.Display();
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
    }
}
