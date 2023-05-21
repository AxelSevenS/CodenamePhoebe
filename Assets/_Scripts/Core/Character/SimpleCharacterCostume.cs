using System.Collections;
using System.Collections.Generic;
using SevenGame.Utility;
using UnityEngine;

namespace SeleneGame.Core {

    [CreateAssetMenu(fileName = "new Character Costume", menuName = "Costume/Simple Character")]
    public sealed class SimpleCharacterCostume : CharacterCostume {

        public GameObject model;

        public override CharacterModel LoadModel(Entity entity) {
            return new SimpleCharacterModel(entity, this);
        }
    }

    public sealed class SimpleCharacterModel : CharacterModel {

        [ReadOnly] public GameObject model;

        public override Transform mainTransform => model.transform;

        public SimpleCharacterModel(Entity entity, SimpleCharacterCostume costume) : base(entity, costume) {
            if (entity != null && costume?.model != null) {
                model = GameObject.Instantiate(costume.model, entity.transform, false);
                _costumeData = model.GetComponent<ModelProperties>();
                entity.animator?.Rebind();
            }
        }


        public override void RotateTowards(Quaternion newRotation, float speed = 12) {
            if (mainTransform == null) return;

            mainTransform.rotation = Quaternion.Slerp(mainTransform.rotation, newRotation, speed * GameUtility.timeDelta);
        }


        public override void Unload() {
            model = GameUtility.SafeDestroy(model);
        }

        public override void Display() {
            model?.SetActive(true);
        }

        public override void Hide() {
            model?.SetActive(false);
        }
    }
}
