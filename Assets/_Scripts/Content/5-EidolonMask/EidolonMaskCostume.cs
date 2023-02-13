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

        [SerializeReference] [HideInInspector] private EidolonMask _mask;


        public EidolonMask mask => _mask;


        public EidolonMaskModel(EidolonMask mask, EidolonMaskCostume costume) : base(costume) {
            _mask = mask;
        }
    }

}