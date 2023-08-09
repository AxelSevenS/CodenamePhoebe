using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SeleneGame.Core;

namespace SeleneGame.Content {

    [CreateAssetMenu(fileName = "New Eidolon Mask", menuName = "Data/Eidolon Mask", order = 0)]
    public class EidolonMaskData : CostumableData<EidolonMaskCostume> {

        public EidolonMask GetMask(MaskedEntity entity, EidolonMaskCostume costume = null) {
            return new EidolonMask(entity, this, costume);
        }


        public virtual void MaskUpdate(EidolonMask mask) {;}
        public virtual void MaskLateUpdate(EidolonMask mask) {;}
        public virtual void MaskFixedUpdate(EidolonMask mask) {;}
    }

}