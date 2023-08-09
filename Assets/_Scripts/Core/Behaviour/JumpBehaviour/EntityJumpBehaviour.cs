using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Animancer;

using SevenGame.Utility;

namespace SeleneGame.Core {

    [System.Serializable]
    public abstract class EntityJumpBehaviour : CompositeBehaviour {
        public const float JUMP_COOLDOWN = 0.4f;

        public ScaledTimeInterval cooldownTimer;
        private float _jumpCooldown = JUMP_COOLDOWN;

        
        public virtual bool CanJump => cooldownTimer.isDone;
        public virtual Vector3 DefaultDirection => -Entity.gravityDown;


        private AnimationClip _jumpAnimation;

        public AnimationClip JumpAnimation {
            get {
                if ( _jumpAnimation == null )
                    _jumpAnimation = Entity.Character?.Data.GetAnimation("Jump");
                return _jumpAnimation;
            }
        }


        protected internal void SetJumpCooldown(float additionalCooldown) {
            _jumpCooldown = JUMP_COOLDOWN + Mathf.Max(additionalCooldown, 0);
        }

        protected internal override void HandleInput(Player contoller) {;}

        protected internal virtual void Jump(Vector3 direction) {
            if ( !CanJump ) return;

            AnimancerState jumpState = Entity.Animancer.Layers[0].Play(JumpAnimation, 0.3f);
            jumpState.Events.OnEnd = () => {
                jumpState.Stop();
            };

            float jumpHeight = Entity.Character.Data.jumpHeight * Entity.JumpMultiplier;

            Debug.Log($"Entity {Entity.name} jumped with force : {jumpHeight}");


            Entity.Inertia = Vector3.ProjectOnPlane(Entity.Inertia, direction) + (jumpHeight * direction);
            cooldownTimer.SetDuration(_jumpCooldown);

        }

        protected internal void Jump() {
            Jump(DefaultDirection);
        }


        public abstract class Builder<TJumpBehaviour> : Builder<TJumpBehaviour, EntityJumpBehaviour> where TJumpBehaviour : EntityJumpBehaviour {

            protected readonly float additionalCooldown;

            public Builder(float additionalCooldown = 0f) {
                this.additionalCooldown = additionalCooldown;
            }

            public override TJumpBehaviour Build(Entity entity, EntityBehaviour parentBehaviour, EntityJumpBehaviour previousBehaviour) {
                return base.Build(entity, parentBehaviour, previousBehaviour);
            }

            protected override void ConfigureBehaviour(Context context) {
                base.ConfigureBehaviour(context);
                context.Behaviour.SetJumpCooldown(0);
            }

        }
    }

}
