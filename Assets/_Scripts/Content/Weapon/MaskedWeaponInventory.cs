using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SevenGame.Utility;
using System;

namespace SeleneGame.Core {

    [System.Serializable]
    public class MaskedWeaponInventory : WeaponInventory {

        [SerializeField] [ReadOnly] private WeaponIndex _currentIndex;

        [SerializeReference] private Weapon primaryWeapon;
        [SerializeReference] private Weapon secondaryWeapon;
        [SerializeReference] private Weapon tertiaryWeapon;

        public override int Count => 3;

        public override Weapon Get(int index) {
            return GetNullable(index) ?? defaultWeapon;
        }

        protected Weapon GetNullable(int index) {
            return index switch {
                (int)WeaponIndex.Primary => primaryWeapon,
                (int)WeaponIndex.Secondary => secondaryWeapon,
                (int)WeaponIndex.Tertiary => tertiaryWeapon,
                _ => null,
            };
        }


        public override Weapon current => Get((int)_currentIndex);
        protected Weapon currentNullable => GetNullable((int)_currentIndex);

        public MaskedWeaponInventory(ArmedEntity entity) : base(entity) {
            Switch((int)WeaponIndex.Primary);
        }

        public override void Set(int index, WeaponData data, Core.WeaponCostume costume = null){
            // weapon = Weapon.Initialize(weapon, entity, costume);
            // try {
                switch (index) {
                    default:
                        // Debug.LogError($"Index {index} is out of range for MaskedWeaponInventory");
                    case (int)WeaponIndex.Primary:
                        SetWeapon(ref primaryWeapon, WeaponIndex.Primary, data, costume);
                        break;
                    case (int)WeaponIndex.Secondary:
                        SetWeapon(ref secondaryWeapon, WeaponIndex.Secondary, data, costume);
                        break;
                    case (int)WeaponIndex.Tertiary:
                        SetWeapon(ref tertiaryWeapon, WeaponIndex.Tertiary, data, costume);
                        break;
                }
            // } catch (System.Exception e) {
            //     Debug.LogError($"Error setting weapon at index {index} in WeaponInventory : {e.GetType()} : {e.Message}.");
            // }

            void SetWeapon(ref Weapon weapon, WeaponIndex index, WeaponData data, Core.WeaponCostume costume = null) {
                if (weapon == null)
                    defaultWeapon?.Hide();
                else
                    weapon.Dispose();
                    
                weapon = data?.CreateWeaponFor(entity, costume);
                if (_currentIndex == index)
                    (weapon ?? defaultWeapon)?.Display();
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

        public override int IndexOf(Weapon weapon) {
            return weapon switch {
                Weapon w when w == primaryWeapon => (int)WeaponIndex.Primary,
                Weapon w when w == secondaryWeapon => (int)WeaponIndex.Secondary,
                Weapon w when w == tertiaryWeapon => (int)WeaponIndex.Tertiary,
                _ => -1,
            };
        }

        public override void Switch(int index){
            if (index == (int)_currentIndex) return;

            current.OnUnequip();
            _currentIndex = (WeaponIndex)index;
            current.OnEquip();
        }

        public override IEnumerator GetEnumerator() => new MaskedWeaponInventoryEnumerator(this);


        public override void Update() {
            primaryWeapon?.Update();
            secondaryWeapon?.Update();
            tertiaryWeapon?.Update();
        }

        public override void LateUpdate() {
            primaryWeapon?.LateUpdate();
            secondaryWeapon?.LateUpdate();
            tertiaryWeapon?.LateUpdate();
        }

        public override void FixedUpdate() {
            primaryWeapon?.FixedUpdate();
            secondaryWeapon?.FixedUpdate();
            tertiaryWeapon?.FixedUpdate();
        }


        private enum WeaponIndex { Primary = 0, Secondary = 1, Tertiary = 2 }

        private class MaskedWeaponInventoryEnumerator : IEnumerator {
            private readonly MaskedWeaponInventory inventory;
            private int index;

            public MaskedWeaponInventoryEnumerator(MaskedWeaponInventory inventory) {
                this.inventory = inventory;
                index = -1;
            }
            
            public object Current => inventory.GetNullable(index);

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
