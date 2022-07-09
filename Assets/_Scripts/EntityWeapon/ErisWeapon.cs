using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SeleneGame.Core;
using SeleneGame.Entities;

namespace SeleneGame.Weapons {
    
    public class ErisWeapon : Weapon{

        public override WeaponType weaponType => WeaponType.heavySword;

        [SerializeField] private BoolData gravitySlide = new BoolData();

        protected override float GetWeightModifier() => 1.4f;
        protected override Vector3 GetJumpDirection() => -entity.gravityDown;
        
        public override void WeaponUpdate(){

            if (!isEquipped) return;

            gravitySlide.SetVal(entity.sliding && entity.onGround);

            // Stick to Surface when sliding in Light Mode. (Light Anchoring)
            if ( gravitySlide ){
                // entity.inertiaMultiplier = Mathf.Min( Mathf.Max( entity.inertiaMultiplier, 12f ), 14f );
                entity.gravityDown = Vector3.Lerp( entity.gravityDown, -entity.groundHit.normal, 0.1f );
            }
            

        }
    }
}