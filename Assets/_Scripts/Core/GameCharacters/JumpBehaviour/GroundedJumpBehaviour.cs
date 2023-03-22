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

        [SerializeField] private float fallInertia = 0f;



        public override bool canJump => base.canJump && entity.onGround.falseTimer < 0.2f && jumpCount > 0;


        public GroundedJumpBehaviour(Entity entity) : base(entity) {;}


        protected internal override void HandleInput(PlayerEntityController contoller) {
            fallGravityMultiplier = contoller.jumpInput ? 0f : 1f;
        } 


        protected internal override void Jump(Vector3 direction) {

            base.Jump(direction);

            jumpCount--;
        }

        public override void Update() {

            if (entity.onGround) {
                jumpCount = 1;
                fallInertia = 0f;
                return;
            }


            const float fallingMultiplier = 0.01f;
            const float maxInertia = 25f;


            fallInertia = Mathf.Lerp( fallInertia, entity.fallVelocity >= 0f ? 0f : fallingMultiplier * fallGravityMultiplier, 7f * GameUtility.timeDelta );
            
            // Add down inertia to the entity, only if it doesn't make it go faster than maxInertia.
            float addedInertiaA = Mathf.Min( -entity.fallVelocity + (fallInertia * entity.gravityMultiplier), maxInertia ) - Mathf.Min( -entity.fallVelocity, maxInertia );

            entity.inertia += addedInertiaA * entity.gravityDown;
        }
    }
}
