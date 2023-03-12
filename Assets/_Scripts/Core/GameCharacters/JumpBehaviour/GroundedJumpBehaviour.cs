using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SevenGame.Utility;

namespace SeleneGame.Core {
    [System.Serializable]
    public class GroundedJumpBehaviour : JumpBehaviour {


        // public int jumpCount = 1;

        private float fallGravityMultiplier = 1f;

        public int jumpCount = 1;
        [SerializeField] private TimeUntil jumpCooldownTimer;

        private float fallInertia = 0f;



        public override bool canJump => base.canJump && entity.onGround.falseTimer < 0.2f && jumpCount > 0 && jumpCooldownTimer.isDone;



        protected internal override void HandleInput(PlayerEntityController contoller) {
            fallGravityMultiplier = contoller.jumpInput ? 0.75f : 1f;
        } 


        protected internal override void Jump(Vector3 direction) {

            base.Jump(direction);

            jumpCooldownTimer.SetTime(0.2f);
            jumpCount--;
        }



        private void FixedUpdate() {

            if (entity.onGround) {
                jumpCount = 1;
                fallInertia = 0f;
                return;
            }

            const float fallingMultiplier = 20f;
            
            float floatingMultiplier = fallGravityMultiplier * entity.gravityMultiplier;
            fallInertia = Mathf.Lerp( fallInertia, floatingMultiplier * entity.fallVelocity >= 0f ? 0f : fallingMultiplier, 3f * GameUtility.timeDelta );

            entity.Displace( fallInertia * entity.gravityDown );

            // entity.rigidbody.AddForce( fallInertia * entity.gravityDown, ForceMode.Acceleration );
        }
    }
}
