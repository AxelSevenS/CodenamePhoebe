using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SeleneGame.Core;

using SevenGame.Utility;
using System;
using EasySplines;

namespace SeleneGame.Content {

    public abstract class EidolonMaskCostume : SpeakerCostume<EidolonMaskCostume> {

        public abstract CostumeModel<EidolonMaskCostume> LoadModel(EidolonMask mask);
    }

    public abstract class EidolonMaskModel : CostumeModel<EidolonMaskCostume> {

        public readonly EidolonMask mask;
        protected Transform headTransform => mask.maskedEntity["head"]?.transform ?? null;


        public EidolonMaskModel(EidolonMask mask, EidolonMaskCostume costume) : base(costume) {
            this.mask = mask;
        }
    }

}