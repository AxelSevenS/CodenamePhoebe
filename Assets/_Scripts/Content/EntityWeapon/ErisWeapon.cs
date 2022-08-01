using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SevenGame.Utility;
using SeleneGame.Core;
using SeleneGame.Entities;

namespace SeleneGame.Weapons {
    
    public sealed class ErisWeapon : Weapon{

        public override WeaponType weaponType => WeaponType.heavySword;

        // [SerializeField] private BoolData gravitySlide = new BoolData();

        protected override float GetWeightModifier() => 1.4f;
        
        public override void WeaponUpdate(){

            // gravitySlide.SetVal(entity.sliding && entity.onGround);

            // // Stick to Surface when sliding in Light Mode. (Light Anchoring)
            // if ( gravitySlide ){
            //     // entity.inertiaMultiplier = Mathf.Min( Mathf.Max( entity.inertiaMultiplier, 12f ), 14f );
            //     entity.gravityDown = Vector3.Lerp( entity.gravityDown, -entity.groundHit.normal, 0.1f );
            // }
            

        }
    }
}