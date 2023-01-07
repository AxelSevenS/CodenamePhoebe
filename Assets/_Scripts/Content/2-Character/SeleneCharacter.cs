using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using SeleneGame.Core;

namespace SeleneGame.Content {

    [CreateAssetMenu(fileName = "Selene", menuName = "Characters/Selene")]
    public sealed class SeleneCharacter : Character {

        protected override void Setup() {
            base.Setup();

            if ( _entity is ArmedEntity armed ){
                armed.weapons.Set(1, "Hypnos");
                armed.weapons.Set(2, "Eris");

                if ( armed is MaskedEntity masked ) {
                    masked.SetMask( EidolonMask.GetInstance("Erebus") );
                }
            }

        }

    }
}
