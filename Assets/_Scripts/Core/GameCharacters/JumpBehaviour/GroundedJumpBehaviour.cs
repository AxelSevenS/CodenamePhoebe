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
            fallGravityMultiplier = contoller.jumpInput ? 0.1f : 1f;
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


            const float maxInertia = 25f;

            float targetInertia = fallGravityMultiplier * entity.gravityMultiplier;
            fallInertia = Mathf.MoveTowards( fallInertia, entity.fallVelocity >= 1f ? 0f : targetInertia, 1f * GameUtility.timeDelta );
            
            // Add down inertia to the entity, only if it doesn't make it go faster than maxInertia.
            float addedInertia = Mathf.Min( -entity.fallVelocity + fallInertia, maxInertia ) - Mathf.Min( -entity.fallVelocity, maxInertia );

            entity.inertia += addedInertia * entity.gravityDown;
        }
    }
}
