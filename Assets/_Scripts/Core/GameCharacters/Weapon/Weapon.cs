using System;

using UnityEngine;

using SevenGame.Utility;
using System.Collections.Generic;
using System.Reflection;

namespace SeleneGame.Core {
    
    public abstract class Weapon : Costumable<Weapon, WeaponCostume, WeaponModel> {

        [SerializeField] private ArmedEntity _armedEntity;



        public ArmedEntity armedEntity => _armedEntity;


        public virtual WeaponType weaponType => WeaponType.Sparring;
        public virtual float weight => 1f;
        


        public Weapon(ArmedEntity armedEntity, WeaponCostume costume = null) {
            _armedEntity = armedEntity;
            SetCostume(costume ?? baseCostume);
        }


        public static Weapon CreateInstance<TWeapon>(ArmedEntity entity, WeaponCostume costume = null) where TWeapon : Weapon {
            return CreateInstance(typeof(TWeapon), entity, costume);
        }
        public static Weapon CreateInstance(Type weaponType, ArmedEntity entity, WeaponCostume costume = null) {
            // if (weaponType == null || weaponType == typeof(UnarmedWeapon)) 
            //     return null;

            if (!typeof(Weapon).IsAssignableFrom(weaponType))
                throw new ArgumentException("weaponType must inherit from Weapon", "weaponType");

            ConstructorInfo constructor = weaponType.GetConstructor(new Type[] { typeof(ArmedEntity), typeof(WeaponCostume) });
            if (constructor != null) {
                return (Weapon)constructor.Invoke(new object[] { entity, costume });
            }
            return null;
        }



        public override void SetCostume(WeaponCostume costume) {
            _model?.Dispose();
            _model = (WeaponModel)costume?.LoadModel(_armedEntity, this) ?? null;
        }


        public virtual void OnEquip(){
            // Display();
        }
        public virtual void OnUnequip(){
            // Hide();
        }



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