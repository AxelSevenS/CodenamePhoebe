using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.AddressableAssets;

using SevenGame.Utility;

namespace SeleneGame.Core {

    public abstract class CharacterCostume : SpeakerCostume {

        public abstract CharacterModel LoadModel(Entity entity);
        
    }

}