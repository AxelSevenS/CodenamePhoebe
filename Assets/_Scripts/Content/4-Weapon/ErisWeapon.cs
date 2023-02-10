using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SeleneGame.Core;

namespace SeleneGame.Content {

    public class ErisWeapon : Weapon {

        public override string internalName => "Eris";

        public override string displayName => "Eris";

        public override string description => "";

        public ErisWeapon(Entity entity, WeaponCostume costume = null) : base(entity, costume) {
        }

        public override WeaponCostume GetBaseCostume() => WeaponCostume.GetAsset("Eris_Base");
    }
}
