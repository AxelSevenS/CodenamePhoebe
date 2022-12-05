using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using SeleneGame.Core;

namespace SeleneGame.Content {

    [CreateAssetMenu(fileName = "Selene", menuName = "Characters/Selene")]
    public sealed class SeleneCharacter : Character {

        public override void Initialize(Entity entity, CharacterCostume costume = null) {
            base.Initialize(entity, costume);

            if ( entity is ArmedEntity armed ){
                armed.weapons.Set(1, "Hypnos");
                armed.weapons.Set(2, "Eris");

                if ( armed is MaskedEntity masked ) {
                    masked.SetMask( EidolonMask.GetInstance("Erebus") );
                }
            }

        }

    }
}
