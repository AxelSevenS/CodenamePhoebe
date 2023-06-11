using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SevenGame.Utility;

namespace SeleneGame.Core {
    [System.Serializable]
    public class GroundedJumpBehaviour : JumpBehaviour {


        [SerializeField] private float maxInertia = 28f;
        [SerializeField] private float fallAcceleration = 1.25f;
        [SerializeField] private int maxJumpCount = 1;

        public int jumpCount = 1;


        private float fallInertia = 0f;
        private float fallGravityMultiplier = 1f;



        public override bool canJump => base.canJump && entity.onGround.falseTimer < 0.2f && jumpCount > 0;


        // public GroundedJumpBehaviour(Entity entity, float maxInertia = 28f, float fallAcceleration = 1.25f, int maxJumpCount = 1) : base(entity) {
        //     this.maxJumpCount = maxJumpCount;
        //     this.maxInertia = maxInertia;
        //     this.fallAcceleration = fallAcceleration;
        //     jumpCount = maxJumpCount;
        // }


        protected internal override void HandleInput(Player contoller) {
            fallGravityMultiplier = contoller.jumpInput ? 0.15f : 1f;
        } 


        protected internal override void Jump(Vector3 direction) {

            base.Jump(direction);

            jumpCount--;
        }

        private void Update() {

            if (entity.onGround) {
                jumpCount = maxJumpCount;
                fallInertia = 0f;
                return;
            }

            const float fallThreshold = 10f;

            float targetInertia = fallGravityMultiplier * entity.gravityMultiplier;
            fallInertia = Mathf.MoveTowards( fallInertia, entity.fallVelocity >= fallThreshold ? 0f : targetInertia, fallAcceleration * GameUtility.timeDelta );
            
            // Add down inertia to the entity, only if it doesn't make it go faster than maxInertia.
            float addedInertia = Mathf.Min( -entity.fallVelocity + fallInertia, maxInertia ) - Mathf.Min( -entity.fallVelocity, maxInertia );

            entity.inertia += addedInertia * entity.gravityDown;
        }
    }
}
