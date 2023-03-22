using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SeleneGame.Core;

using SevenGame.Utility;

namespace SeleneGame.Content {

    public abstract class EidolonMaskModel : CostumeModel<EidolonMaskCostume> {

        [SerializeField] [ReadOnly] MaskedEntity _entity;
        [SerializeReference] [HideInInspector] private EidolonMask _mask;


        public MaskedEntity entity => _entity;
        public EidolonMask mask => _mask;


        public EidolonMaskModel(MaskedEntity entity, EidolonMask mask, EidolonMaskCostume costume) : base(costume) {
            _entity = entity;
            _mask = mask;
        }
    }

}