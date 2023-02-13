using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.AddressableAssets;

using SevenGame.Utility;

namespace SeleneGame.Core {

    // [CreateAssetMenu(fileName = "new Character Costume", menuName = "Costume/Character")]
    public abstract class CharacterCostume : SpeakerCostume<CharacterCostume> {


        public abstract CostumeModel<CharacterCostume> LoadModel(Entity entity);
    }

    public abstract class CharacterModel : CostumeModel<CharacterCostume> {

        [ReadOnly] public Entity entity;

        public CharacterModel(Entity entity, CharacterCostume costume) : base(costume) {
            this.entity = entity;
        }
    }

}