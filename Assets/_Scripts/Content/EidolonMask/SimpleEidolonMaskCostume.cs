using System.Collections;
using System.Collections.Generic;

using SeleneGame.Core;

using UnityEngine;


namespace SeleneGame.Content {

    [CreateAssetMenu(fileName = "new Eidolon Mask Costume", menuName = "Costume/Simple Eidolon Mask")]
    public sealed class SimpleEidolonMaskCostume : EidolonMaskCostume {

        [SerializeField] public GameObject model;

        public override EidolonMaskModel LoadModel(MaskedEntity entity, EidolonMask mask) {
            return new SimpleEidolonMaskModel(entity, mask, this);
        }
    }

}