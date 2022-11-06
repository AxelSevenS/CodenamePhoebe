using System;

using UnityEngine;

using SevenGame.Utility;

namespace SeleneGame.Core {
    
    public abstract class Weapon : Costumable<Weapon, WeaponCostume> {



        [Header("Weapon Info")]

        [SerializeField] [NotFlagEnum] private WeaponType _weaponType;
        public float weightModifier = 1f;


        [Header("Weapon Data")]

        [SerializeField] [ReadOnly] protected ArmedEntity entity;



        public WeaponType weaponType => _weaponType;
        public float WeightModifier => weightModifier;



        public void Initialize( ArmedEntity entity, WeaponCostume costume = null) {
            if (this.entity != null)
                throw new InvalidOperationException("Weapon already initialized");

            this.entity = entity;
            SetCostume( costume ?? baseCostume );
        }
        

        protected internal virtual void OnEquip(){
            Display();
        }
        protected internal virtual void OnUnequip(){
            Hide();
        }


        public abstract void Display();
        public abstract void Hide();


        protected internal virtual void Update( ){;}
        protected internal virtual void FixedUpdate( ){;}
        


        [Flags]
        public enum WeaponType : byte {
            OneHanded = 1 << 0,
            TwoHanded = 1 << 1,
            Staff = 1 << 2,
            DoubleOneHanded = 1 << 3,
            Sparring = 1 << 4
        };

    }
}