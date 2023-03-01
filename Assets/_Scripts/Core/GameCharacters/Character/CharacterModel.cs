using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SevenGame.Utility;

namespace SeleneGame.Core {
    

    public abstract class CharacterModel : CostumeModel<CharacterCostume> {

        [SerializeField] [ReadOnly] protected Entity _entity;

        public Entity entity => _entity;

        public CharacterModel(Entity entity, CharacterCostume costume) : base(costume) {
            _entity = entity;
        }
    }

}
