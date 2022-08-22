using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SevenGame.Utility;
using SeleneGame.Core;
using SeleneGame.Entities;

namespace SeleneGame.Weapons {
    
    [CreateAssetMenu(fileName = "Eris", menuName = "Weapon/Eris")]
    public sealed class ErisWeapon : Weapon{

        // [SerializeField] private BoolData gravitySlide = new BoolData();
        
        public override void WeaponUpdate( Entity entity ){
            base.WeaponUpdate( entity );

            // gravitySlide.SetVal(entity.sliding && entity.onGround);

            // // Stick to Surface when sliding in Light Mode. (Light Anchoring)
            // if ( gravitySlide ){
            //     // entity.inertiaMultiplier = Mathf.Min( Mathf.Max( entity.inertiaMultiplier, 12f ), 14f );
            //     entity.gravityDown = Vector3.Lerp( entity.gravityDown, -entity.groundHit.normal, 0.1f );
            // }
            

        }
    }
}