using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SeleneGame.Core;
using SeleneGame.Entities;

namespace SeleneGame.Weapons {
    
    public class ErisWeapon : Weapon{

        [SerializeField] private BoolData gravitySlide = new BoolData();

        protected override float GetSpeedMultiplier() => entity.inWater ? 1.5f : 1f;
        protected override float GetWeightModifier() => 1.4f;
        protected override Vector3 GetJumpDirection() => -entity.gravityDown;
        // protected override Vector3 GetCameraPosition() => entity.inWater ? new Vector3(0, 1f, -3.5f) : new Vector3(1f, 1f, -3.5f);

        // public ErisWeapon(Entity entity) : base(entity){;}
        protected override void WeaponUpdateEquipped(){

            gravitySlide.SetVal(entity.sliding && entity.onGround);

            // Stick to Surface when sliding in Light Mode. (Light Anchoring)
            if ( gravitySlide ){
                // entity.inertiaMultiplier = Mathf.Min( Mathf.Max( entity.inertiaMultiplier, 12f ), 14f );
                entity.gravityDown = Vector3.Lerp( entity.gravityDown, -entity.groundHit.normal, 0.1f );
            }
            

        }
    }
}