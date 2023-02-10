using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SeleneGame.Core {
    
    public sealed class UnarmedWeapon : Weapon {

        public override string internalName => "Unarmed";

        public override string displayName => "None";

        public override string description => "Fighting with bare hands? Gutsy move, but you'll have to make do with what you have";

        public UnarmedWeapon(Entity entity, WeaponCostume costume = null) : base(entity, costume) {
        }

        public override WeaponCostume GetBaseCostume() => WeaponCostume.GetAsset("Unarmed_Base");

    }
}