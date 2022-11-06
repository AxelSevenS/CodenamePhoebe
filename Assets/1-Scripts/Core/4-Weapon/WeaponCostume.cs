using System;

using UnityEngine;

using SevenGame.Utility;

namespace SeleneGame.Core {
    
    [CreateAssetMenu(fileName = "new Weapon Costume", menuName = "Costume/Weapon")]
    public class WeaponCostume : Costume<WeaponCostume> {


        public GameObject model;

        public Weapon.WeaponType equippableOn;

    }
}