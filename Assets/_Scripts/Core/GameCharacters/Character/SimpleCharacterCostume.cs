using System.Collections;
using System.Collections.Generic;
using SevenGame.Utility;
using UnityEngine;

namespace SeleneGame.Core {

    [CreateAssetMenu(fileName = "new Character Costume", menuName = "Costume/Simple Character")]
    public sealed class SimpleCharacterCostume : CharacterCostume {

        public GameObject model;

        public override CostumeModel<CharacterCostume> LoadModel(Entity entity) {
            return new SimpleCharacterModel(entity, this);
        }
    }

    public sealed class SimpleCharacterModel : CharacterModel {

        [ReadOnly] public GameObject model;

        public override Transform mainTransform => model.transform;

        public SimpleCharacterModel(Entity entity, SimpleCharacterCostume costume) : base(entity, costume) {
            if (entity != null && costume?.model != null) {
                model = GameObject.Instantiate(costume.model, entity.transform, false);
                _costumeData = model.GetComponent<CostumeData>();
                entity.animator.Rebind();
            }
        }

        public override void Unload() {
            model = GameUtility.SafeDestroy(model);
        }
    }
}
