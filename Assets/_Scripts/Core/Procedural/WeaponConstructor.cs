using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SeleneGame.Utility {

    public static class WeaponConstructor {
        
        public static SeleneGame.Core.Weapon CreateWeapon(string type){
            switch (type) {
                default: return new SeleneGame.Weapons.UnarmedWeapon();
                case "ErisWeapon": return new SeleneGame.Weapons.ErisWeapon();
                case "HypnosWeapon": return new SeleneGame.Weapons.HypnosWeapon();
            }
        }
    }
}
