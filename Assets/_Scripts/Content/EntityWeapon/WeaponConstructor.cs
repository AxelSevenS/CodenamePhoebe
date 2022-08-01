using System;

using SevenGame.Utility;

namespace SeleneGame.Weapons {

    public static class WeaponConstructor {

        
        public static SeleneGame.Core.Weapon CreateWeapon(Type type){
            return CreateWeapon(type.Name);
        }

        public static SeleneGame.Core.Weapon CreateWeapon(string type){
            switch (type) {
                default: return new SeleneGame.Core.UnarmedWeapon();
                case "ErisWeapon": return new SeleneGame.Weapons.ErisWeapon();
                case "HypnosWeapon": return new SeleneGame.Weapons.HypnosWeapon();
            }
        }
    }
}
