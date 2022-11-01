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
        public float evadeCount = 1f;



        public float evadeTime { get; protected set; }
        public float evadeCurve { get; protected set; }



        public event Action<Vector3> onEvade;

        public virtual void Evade(Vector3 evadeDirection){

            if ( !evadeTimer.isDone ) return;
            
            if (gravityMultiplier > 0f) {

                Vector3 newVelocity = entity.rigidbody.velocity.NullifyInDirection( entity.gravityDown );
                if (!entity.onGround){
                    newVelocity += -entity.gravityDown.normalized * 5f;
                }
                entity.rigidbody.velocity = newVelocity;
            }
            

            currentEvadeDirection = evadeDirection;
            evadeTimer.SetDuration(entity.character.totalEvadeDuration);

            entity.animator.SetTrigger("Evade");


            onEvade?.Invoke(evadeDirection);
        }

        public override void HandleInput(EntityController controller) {

            if ( evadeCount > 0 && controller.evadeInput.started && canEvade )
                Evade( evadeDirection );

            if ( entity is ArmedEntity armed ) {
                if ( KeyInputData.SimultaneousTap( controller.lightAttackInput, controller.heavyAttackInput ) && canParry )
                    armed.Parry();
            }
        }


        public override void StateUpdate() {

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

        public override void StateFixedUpdate() {
            base.StateFixedUpdate();

            if (evading)
                entity.Move( evadeCurve * entity.character.evadeSpeed * evadeDirection );
        }
    }
}
