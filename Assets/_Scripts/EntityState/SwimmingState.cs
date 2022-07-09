using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SeleneGame.Core;
using SeleneGame.Entities;
using SeleneGame.Utility;

namespace SeleneGame.States {
    
    public class SwimmingState : State{

        public override StateType stateType => StateType.waterState;
        protected override Vector3 GetCameraPosition() {
            // if (entity is ArmedEntity armed)
            //     return armed.weapons.current.cameraPosition;
                
            return Player.current.defaultCameraPosition;
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

            if ( entity.evadeInput.started && entity is ArmedEntity armed )
                armed.Evade(armed.absoluteForward);


        }

        public override void StateFixedUpdate(){

            entity.SetRotation(-entity.gravityDown);

            entity.rb.velocity = Vector3.Dot(entity.rb.velocity.normalized, entity.gravityDown) > 0f ? entity.rb.velocity / 1.1f : entity.rb.velocity;

            // if (entity is ArmedEntity armed && armed.EvadeUpdate(out _, out float evadeCurve)){
                
            //     armed.Move( GameUtility.timeDelta * evadeCurve * armed.data.evadeSpeed * armed.evadeDirection );
                
            // }


            if (entity.moveDirection.magnitude > 0f){

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
            
            
            entity.moveDirection.SetVal( entity.rotation * entity.cameraRotation * entity.moveInput );
            
            if (entity.jumpInput && entity.isOnWaterSurface && entity.jumpCount != 0 && entity.jumpCooldown == 0f ){
                entity.Jump( -entity.gravityDown );
            }

            float newSpeed = entity.walkSpeed != Entity.WalkSpeed.idle ? entity.data.baseSpeed : 0f;
            if (entity.walkSpeed != Entity.WalkSpeed.run) 
                newSpeed *= entity.walkSpeed == Entity.WalkSpeed.sprint ? /* entity.data.sprintMultiplier */1f : entity.data.slowMultiplier;
            newSpeed = newSpeed * entity.data.swimMultiplier;

            float speedDelta = newSpeed > entity.moveSpeed ? 1f : 0.65f;
            entity.moveSpeed = Mathf.MoveTowards(entity.moveSpeed, newSpeed, speedDelta * entity.data.moveIncrement * GameUtility.timeDelta);
        }
    }
}