using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SeleneGame.Core;
using SeleneGame.Entities;

namespace SeleneGame.States {
    
    [System.Serializable]
    public class SwimmingState : State{

        public override int id => 1;
        protected override Vector3 GetCameraPosition() {
            if (entity is ArmedEntity armed)
                return armed.currentWeapon.cameraPosition;
                
            return base.GetCameraPosition();
        }

        // public override bool masked => true;

        public override void OnEnter(Entity entity){
            base.OnEnter(entity);
            
            entity.jumpCount = 1;
        }

        public override void StateUpdate(){


            if (!entity.inWater || entity.CanSink()){
                entity.SetState(entity.defaultState);
            }


            if ( entity.evadeInput.started )
                entity.Evade(entity.absoluteForward);


        }

        public override void StateFixedUpdate(){

            entity.SetRotation(-entity.gravityDown);

            entity._rb.velocity = Vector3.Dot(entity._rb.velocity.normalized, entity.gravityDown) > 0f ? entity._rb.velocity / 1.1f : entity._rb.velocity;

            // if (entity is ArmedEntity armed && armed.EvadeUpdate(out _, out float evadeCurve)){
                
            //     armed.Move( Global.timeDelta * evadeCurve * armed.data.evadeSpeed * armed.evadeDirection );
                
            // }


            if (entity.moveDirection.magnitude > 0f){

                entity.absoluteForward = entity.moveDirection.normalized;
                Vector3 newUp = Vector3.Cross(entity.absoluteForward, Vector3.Cross(entity.absoluteForward, entity.gravityDown));
                // if ( entity is ArmedEntity && (armed.evadeTimer < armed.data.evadeCooldown) ){
                //     armed.RotateTowardsAbsolute(armed.absoluteForward, newUp);
                // }

                entity.Move(entity.moveDirection * Global.timeDelta * entity.moveSpeed);

                entity.RotateTowardsAbsolute(entity.absoluteForward, newUp);
            }



        }

        public override float UpdateMoveSpeed(){
            float newSpeed = entity.walkSpeed != Entity.WalkSpeed.idle ? entity.data.baseSpeed : 0f;
            if (entity.walkSpeed != Entity.WalkSpeed.run) 
                newSpeed *= entity.walkSpeed == Entity.WalkSpeed.sprint ? /* entity.data.sprintSpeed */1f : entity.data.slowSpeed;

            newSpeed = newSpeed * entity.data.swimSpeed;
            
            return newSpeed;
        }

        public override void HandleInput(){
            
            entity.moveDirection.SetVal( entity.rotation * entity.lookRotation * entity.moveInput );
            
            if (entity.jumpInput && entity.isOnWaterSurface && entity.jumpCount != 0 && entity.jumpCooldown == 0f ){
                entity.Jump( -entity.gravityDown );
            }
        }
    }
}