using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SevenGame.Utility;

namespace SeleneGame.Core {

    public abstract class HumanoidState : State {


        [Header("Evading")]
        public BoolData evading;
        public Vector3 currentEvadeDirection = Vector3.forward;
        public TimeUntil evadeTimer;
        
        public float evadeTime { get; protected set; }
        public float evadeCurve { get; protected set; }



        public override float gravityMultiplier => base.gravityMultiplier;
        public override Vector3 cameraPosition => base.cameraPosition;


        protected override Vector3 jumpDirection => base.jumpDirection;
        protected override bool canJump => base.canJump;

        protected override Vector3 evadeDirection => base.evadeDirection;
        protected override bool canEvade => base.canEvade && evadeTimer.isDone;

        protected override bool canParry => base.canParry;



        public override void HandleInput(EntityController controller) {

            if ( controller.evadeInput.started )
                Evade(evadeDirection);

            if ( KeyInputData.SimultaneousTap( controller.lightAttackInput, controller.heavyAttackInput ) )
                Parry();
        }


        public override void Jump() {
            base.Jump();
        }
        public override void Evade(Vector3 direction) {
            base.Evade(direction);
        }
        public override void Parry() {
            base.Parry();
        }
        public override void LightAttack() {
            base.LightAttack();
        }
        public override void HeavyAttack() {
            base.HeavyAttack();
        }


        protected override void JumpAction(Vector3 jumpDirection) {
            base.JumpAction(jumpDirection);
        }
        protected override void EvadeAction(Vector3 direction) {
            base.EvadeAction(direction);

            currentEvadeDirection = direction;
            
            if (gravityMultiplier > 0f) {

                Vector3 newVelocity = entity.rigidbody.velocity.NullifyInDirection( entity.gravityDown );
                if (!entity.onGround){
                    newVelocity += -entity.gravityDown.normalized * 5f;
                }
                entity.rigidbody.velocity = newVelocity;
            }
            
            evadeTimer.SetDuration(entity.character.totalEvadeDuration);

            entity.animator.SetTrigger("Evade");
            // onEvade?.Invoke(direction);
        }

        protected override void ParryAction() {
            base.ParryAction();

            if ( entity is ArmedEntity armed ) {
                // entity.animator.SetTrigger("Parry");
                armed.Parry();
            }
        }
        protected override void LightAttackAction() {
            base.LightAttackAction();
        }
        protected override void HeavyAttackAction() {
            base.HeavyAttackAction();
        }



        protected internal override void StateUpdate() {
            base.StateUpdate();

            evading.SetVal( evadeTimer > entity.character.evadeCooldown );

            if ( !entity.isIdle && evadeTimer > entity.character.totalEvadeDuration - 0.15f )
                currentEvadeDirection = entity.absoluteForward;

            if ( !evading ) {
                evadeTime = 0f;
                evadeCurve = 0f;
            } else {
                evadeTime = Mathf.Clamp01( 1 - ( (evadeTimer - entity.character.evadeCooldown) / entity.character.evadeDuration ) );
                evadeCurve = Mathf.Clamp01( EntityManager.current.evadeCurve.Evaluate( evadeTime ) );
            }
        }

        protected internal override void StateFixedUpdate() {
            base.StateFixedUpdate();

            if (evading)
                entity.Move( evadeCurve * entity.character.evadeSpeed * evadeDirection );
        }
    }
}
