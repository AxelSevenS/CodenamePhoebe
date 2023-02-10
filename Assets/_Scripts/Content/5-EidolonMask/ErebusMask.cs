using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SeleneGame.Content;

namespace SeleneGame.Masks {

    // [CreateAssetMenu(fileName = "Erebus", menuName = "Masks/Erebus")]
    public class ErebusMask : EidolonMask {
        
        public ErebusMask(MaskedEntity maskedEntity) : base(maskedEntity) {
        }

        public override string internalName => "Erebus";

        public override string displayName => "Erebus";

        public override string description => "";

        public override EidolonMaskCostume GetBaseCostume() => EidolonMaskCostume.GetAsset("Erebus_Base");
    }

}