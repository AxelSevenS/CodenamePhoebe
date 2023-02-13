using System;

using UnityEngine;

using SevenGame.Utility;

namespace SeleneGame.Core {
    
    public abstract class WeaponCostume : Costume<WeaponCostume> {

        public Weapon.WeaponType equippableOn;

        public abstract CostumeModel<WeaponCostume> LoadModel(Entity entity, Weapon weapon);

    }

    public abstract class WeaponModel : CostumeModel<WeaponCostume> {
        
        [SerializeReference] [HideInInspector] public Weapon weapon;
        
        public WeaponModel(Entity entity, Weapon weapon, WeaponCostume costume) : base(costume) {
            this.weapon = weapon;
        }

    }
}