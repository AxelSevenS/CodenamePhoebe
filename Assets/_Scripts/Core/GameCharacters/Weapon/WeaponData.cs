using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SeleneGame.Core {
    
    [CreateAssetMenu(fileName = "New Weapon", menuName = "Data/Weapon", order = 0)]
    public class WeaponData : CostumableData<WeaponCostume> {


        public Weapon.WeaponType weaponType = Weapon.WeaponType.Sparring;
        public float weight = 1f;


        public virtual Weapon GetWeapon(ArmedEntity entity, WeaponCostume costume = null) {
            return new Weapon(entity, this, costume);
        }

    }
}
