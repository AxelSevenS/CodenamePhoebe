using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Scribe;

namespace SeleneGame.Core {
    
    public abstract class WeaponData : CostumableData<WeaponCostume> {


        [SerializeField] private Weapon.WeaponType _weaponType = Weapon.WeaponType.Sparring;
        [SerializeField] private float _weight = 1f;


        public Weapon.WeaponType weaponType => _weaponType;
        public float weight => _weight;


        public abstract void HandleInput(Weapon weapon, Player playerController);

        public abstract void LightAttack(Weapon weapon);

        public abstract void HeavyAttack(Weapon weapon);

        public abstract Weapon CreateWeaponFor(ArmedEntity entity, WeaponCostume costume = null);

        public virtual void WeaponUpdate(Weapon weapon) {;}
        public virtual void WeaponLateUpdate(Weapon weapon) {;}
        public virtual void WeaponFixedUpdate(Weapon weapon) {;}

    }
}
