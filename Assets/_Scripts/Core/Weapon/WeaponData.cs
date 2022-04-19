using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SeleneGame.Core {
    
    [CreateAssetMenu(fileName = "new Weapon Data", menuName = "Data/Weapon")]
    public class WeaponData : UnitData<WeaponCostume> {

        public static string defaultData => "Unarmed";
        
        public enum WeaponType {lightSword, heavySword, spear, swordAndShield, sparring};
        
        public WeaponType weaponType;
    }
}