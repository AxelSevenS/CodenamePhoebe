using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SeleneGame.Core;
using SeleneGame.Entities;
using SevenGame.Utility;

namespace SeleneGame.States {
    
    public class SwimmingState : State{

        public override StateType stateType => StateType.waterState;
        protected override Vector3 GetCameraPosition() {
            // if (entity is ArmedEntity armed)
            //     return armed.weapons.current.cameraPosition;
                
            return Global.cameraDefaultPosition;
        }

        public override void OnEnter(Entity entity){
            base.OnEnter(entity);

            entity.gravityDown = Vector3.down;
            
            entity.jumpCount = 1;
        }

        public override void StateUpdate(){


            if (!entity.inWater || entity.CanSink()){
                entity.SetState(entity.defaultState);
            }

            if ( entity.controller.evadeInput.started && entity is ArmedEntity armed )
                armed.Evade(armed.absoluteForward);


        }

        public override void StateFixedUpdate(){

            entity.SetRotation(-entity.gravityDown);

            entity.rigidbody.velocity = Vector3.Dot(entity.rigidbody.velocity.normalized, entity.gravityDown) > 0f ? entity.rigidbody.velocity / 1.1f : entity.rigidbody.velocity;

            // if (entity is ArmedEntity armed && armed.EvadeUpdate(out _, out float evadeCurve)){
                
            //     armed.Move( GameUtility.timeDelta * evadeCurve * armed.data.evadeSpeed * armed.evadeDirection );
                
            // }


            if (entity.moveDirection.sqrMagnitude > 0f){

                entity.absoluteForward = entity.moveDirection.normalized;
                Vector3 newUp = Vector3.Cross(entity.absoluteForward, Vector3.Cross(entity.absoluteForward, entity.gravityDown));
                // if ( entity is ArmedEntity && (armed.evadeTimer < armed.data.evadeCooldown) ){
                //     armed.RotateTowardsAbsolute(armed.absoluteForward, newUp);
                // }

                entity.Move(entity.moveDirection * GameUtility.timeDelta * entity.moveSpeed);

                entity.RotateTowardsAbsolute(entity.absoluteForward, newUp);
            }



        }

        public override void HandleInput(){
            
            entity.controller.RawInputToCameraRelativeMovement(out Quaternion cameraRotation, out Vector3 cameraRelativeMovement);
            float verticalInput = (entity.controller.jumpInput ? 1f: 0f) - (entity.controller.crouchInput ? 1f: 0f);
            entity.moveDirection.SetVal( cameraRelativeMovement + (cameraRotation * Vector3.up * verticalInput) );
            
            if (entity.controller.jumpInput && entity.isOnWaterSurface && entity.jumpCount != 0 && entity.jumpCooldownTimer.isDone ){
                entity.Jump( -entity.gravityDown );
            }

            float newSpeed = entity.isIdle ? 0f : entity.data.baseSpeed;

            float speedDelta = newSpeed > entity.moveSpeed ? 1f : 0.65f;
            entity.moveSpeed = Mathf.MoveTowards(entity.moveSpeed, newSpeed, speedDelta * entity.data.acceleration * GameUtility.timeDelta);
        }
    }
}