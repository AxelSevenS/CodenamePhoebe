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


        public virtual Weapon GetWeapon(ArmedEntity entity, WeaponCostume costume = null) {
            return new Weapon(entity, this, costume);
        }

    }
}
