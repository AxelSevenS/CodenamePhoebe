using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SevenGame.Utility;
using SeleneGame.Core;

namespace SeleneGame.Content {
    
    [CreateAssetMenu(fileName = "Eris", menuName = "Weapon/Eris")]
    public sealed class ErisWeapon : Weapon {


        // public override void Display() {
        //     costume.modelInstance?.SetActive(true);
        // }

        // public override void Hide() {
        //     costume.modelInstance?.SetActive(false);
        // }


        
        protected override void Update( ){
            base.Update();

            // gravitySlide.SetVal(entity.sliding && entity.onGround);

            // // Stick to Surface when sliding in Light Mode. (Light Anchoring)
            // if ( gravitySlide ){
            //     // entity.inertiaMultiplier = Mathf.Min( Mathf.Max( entity.inertiaMultiplier, 12f ), 14f );
            //     entity.gravityDown = Vector3.Lerp( entity.gravityDown, -entity.groundHit.normal, 0.1f );
            // }
            

        }
    }
}