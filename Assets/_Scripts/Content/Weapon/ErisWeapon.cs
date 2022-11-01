using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SevenGame.Utility;
using SeleneGame.Core;
using SeleneGame.Entities;

namespace SeleneGame.Weapons {
    
    [CreateAssetMenu(fileName = "Eris", menuName = "Weapon/Eris")]
    public sealed class ErisWeapon : Weapon {

        private GameObject model;

        // [SerializeField] private BoolData gravitySlide = new BoolData();

        protected override void LoadModel() {
            if (costume.model != null) {
                model = Instantiate(costume.model, entity.transform);
            }
        }
        protected override void UnloadModel() {
            model = GameUtility.SafeDestroy(model);
        }


        public override void Display() {
            model?.SetActive(true);
        }

        public override void Hide() {
            model?.SetActive(false);
        }


        
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