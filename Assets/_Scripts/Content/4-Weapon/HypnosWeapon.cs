using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SeleneGame.Core;


namespace SeleneGame.Content {

    public class HypnosWeapon : Weapon {

        public override string internalName => "Hypnos";

        public override string displayName => "Hypnos";

        public override string description => "";

        public HypnosWeapon(ArmedEntity entity, WeaponCostume costume = null) : base(entity, costume) {
        }

        public override WeaponCostume GetBaseCostume() => WeaponCostume.GetAsset("Hypnos_Base");

    }
}
