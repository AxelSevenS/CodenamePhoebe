using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using SeleneGame.Core;
using SeleneGame.Entities;
using SeleneGame.Weapons;

namespace SeleneGame.Characters {
    [CreateAssetMenu(fileName = "Selene", menuName = "Characters/Selene")]
    public sealed class SeleneCharacter : Character {

        public override void CharacterCreation(Instance instance) {
            base.CharacterCreation(instance);

            if ( instance.entity is ArmedEntity armed ){
                armed.weapons.Set(1, "Hypnos");
                armed.weapons.Set(2, "Eris");

                if ( armed is MaskedEntity masked ) {
                    GameObject mask = Instantiate( Resources.Load<GameObject>("Prefabs/Characters/MaskEidolon/ErebusEidolon") );
                    
                    MaskMovement maskMovement = mask.GetComponent<MaskMovement>();
                    maskMovement.SetEntity(masked);
                    masked.mask = maskMovement;
                }
            }

        }

    }
}
