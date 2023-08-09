using System;

using UnityEngine;

using SevenGame.Utility;

namespace SeleneGame.Core {
    [System.Serializable]
    public sealed class GroundedJumpBehaviour : EntityJumpBehaviour {


        [SerializeField] private float _maxInertia = 28f;
        [SerializeField] private float _fallAcceleration = 1.25f;
        [SerializeField] private int _maxJumpCount = 1;

        private int jumpCount = 1;

        private float fallInertia = 0f;
        private float fallGravityMultiplier = 1f;


        public override bool CanJump => base.CanJump && Entity.onGround.falseTimer < 0.2f && jumpCount > 0;




        protected internal override void HandleInput(Player contoller) {
            if ( contoller.jumpInput ) {
                Jump();
            }
            fallGravityMultiplier = contoller.jumpInput ? 0.15f : 1f;
        } 

        protected internal override void Jump(Vector3 direction) {
            if ( !CanJump ) return;
            
            base.Jump(direction);

            jumpCount = Math.Max( jumpCount - 1, 0 );
        }

        internal void SetMaxInertia(float maxInertia) {
            _maxInertia = maxInertia;
        }
        internal void SetFallAcceleration(float fallAcceleration) {
            _fallAcceleration = fallAcceleration;
        }
        internal void SetMaxJumpCount(int maxJumpCount) {
            _maxJumpCount = maxJumpCount;
        }


        private void Update() {

            if (Entity.onGround) {
                jumpCount = _maxJumpCount;
                fallInertia = 0f;
                return;
            }

            float targetInertia = fallGravityMultiplier * Entity.GravityMultiplier;
            fallInertia = Mathf.MoveTowards( fallInertia, Entity.FallVelocity >= Entity.MAX_FALL_INERTIA ? 0f : targetInertia, _fallAcceleration * GameUtility.timeDelta );
            
            // Add down inertia to the entity, only if it doesn't make it go faster than maxInertia.
            float addedInertia = Mathf.Min( -Entity.FallVelocity + fallInertia, _maxInertia ) - Mathf.Min( -Entity.FallVelocity, _maxInertia );

            Entity.Inertia += addedInertia * Entity.gravityDown;
        }



        public sealed class Builder : Builder<GroundedJumpBehaviour> {

            public static readonly Builder Default = new ();
            
            private readonly int maxJumpCount;
            private readonly float maxInertia;
            private readonly float fallAcceleration;

            public Builder(float additionalCooldown = 0f, int maxJumpCount = 1, float maxInertia = 28f, float fallAcceleration = 1.25f) : base(additionalCooldown) {
                this.maxJumpCount = Math.Max(maxJumpCount, 1);
                this.maxInertia = Mathf.Max(maxInertia, 0);
                this.fallAcceleration = Mathf.Max(fallAcceleration, 0);
            }

            protected override void ConfigureBehaviour(Context context) {
                base.ConfigureBehaviour(context);
                context.Behaviour.SetMaxJumpCount(maxJumpCount);
                context.Behaviour.SetMaxInertia(maxInertia);
                context.Behaviour.SetFallAcceleration(fallAcceleration);
            }
        }
    }
}
