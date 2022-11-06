using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.AddressableAssets;

namespace SeleneGame.Core {

    [CreateAssetMenu(fileName = "new Character Costume", menuName = "Costume/Character")]
    public class CharacterCostume : SpeakerCostume<CharacterCostume> {


        public GameObject model;
        
    }
}