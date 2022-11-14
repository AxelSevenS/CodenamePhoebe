using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SeleneGame.Core;

using SevenGame.Utility;

namespace SeleneGame.Content {

    [System.Serializable]
    public class MaskedWeaponInventory : WeaponInventory {

        private WeaponIndex _currentIndex;

        [SerializeReference] [ReadOnly] public Weapon primaryWeapon;
        [SerializeReference] [ReadOnly] public Weapon secondaryWeapon;
        [SerializeReference] [ReadOnly] public Weapon tertiaryWeapon;

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


        public override Weapon current => Get((int)_currentIndex) ?? defaultWeapon;

        public MaskedWeaponInventory(ArmedEntity entity) : base(entity) {
            Switch((int)WeaponIndex.primary);
        }

        public override void Set(int index, Weapon weapon, WeaponCostume costume = null){
            Debug.Log(weapon);
            switch (index) {
                case 0:
                    primaryWeapon = weapon;
                    primaryWeapon?.Initialize(entity, costume);
                    break;
                case 1:
                    secondaryWeapon = weapon;
                    secondaryWeapon?.Initialize(entity, costume);
                    break;
                case 2:
                    tertiaryWeapon = weapon;
                    tertiaryWeapon?.Initialize(entity, costume);
                    break;
                default:
                    throw new System.IndexOutOfRangeException($"Index {index} is out of range for MaskedWeaponInventory");
            }

        }

        public override void Remove(int index) {
            try {
                Set(index, weapon: null);
            } catch (System.Exception e) {
                Debug.LogError($"Error removing weapon at index {index} in WeaponInventory : {e.Message}.");
            }
        }

        public override void Switch(int index){
            if (index == (int)_currentIndex) return;

            current.Hide();
            _currentIndex = (WeaponIndex)index;
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


        private enum WeaponIndex { primary = 0, secondary = 1, tertiary = 2 }
    }
}
