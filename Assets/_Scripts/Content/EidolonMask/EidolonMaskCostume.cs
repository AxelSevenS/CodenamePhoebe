using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SeleneGame.Core;

using SevenGame.Utility;
using System;
using EasySplines;

namespace SeleneGame.Content {

    public abstract class EidolonMaskCostume : SpeakerCostume {

        public abstract EidolonMaskModel LoadModel(MaskedEntity entity, EidolonMask mask);
    }

}