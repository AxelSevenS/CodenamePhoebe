using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SevenGame.Utility;

namespace SeleneGame.Core {
    [System.Serializable]
    public class GroundedJumpBehaviour : JumpBehaviour {


        // public int jumpCount = 1;

        private float fallGravityMultiplier = 1f;

        public override bool canJump => base.canJump && entity.onGround.falseTimer < 0.2f;



        public GroundedJumpBehaviour(State entityState) : base(entityState) { }



        protected internal override void HandleInput(PlayerEntityController contoller) {
            fallGravityMultiplier = contoller.jumpInput ? 0.75f : 1f;
        } 

        protected internal override void FixedUpdate() {

            base.FixedUpdate();


            // if (entityState.entity.onGround) {
            //     jumpCount = 1;
            // }

            const float regularFallMultiplier = 3.25f;
            const float fallingMultiplier = 2f;
            
            float floatingMultiplier = regularFallMultiplier * fallGravityMultiplier * entity.gravityMultiplier;
            float multiplier = floatingMultiplier * (entity.fallVelocity >= 0 ? 1f : fallingMultiplier);

            entity.rigidbody.velocity += multiplier * entity.gravityForce * GameUtility.timeDelta;
        }


        protected internal override void Jump(Vector3 direction) {

            base.Jump(direction);

            // jumpCount--;
        }
    }
}
