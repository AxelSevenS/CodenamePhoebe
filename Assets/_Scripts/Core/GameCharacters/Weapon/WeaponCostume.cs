using System;

using UnityEngine;

using SevenGame.Utility;

namespace SeleneGame.Core {
    
    public abstract class WeaponCostume : Costume {

        public Weapon.WeaponType equippableOn;

        public abstract WeaponModel LoadModel(Entity entity, Weapon weapon);

    }
}