using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SeleneGame.Core;

namespace SeleneGame.States {
    
    public class SwimmingState : State{

        public override int id => 1;
        protected override Vector3 GetCameraPosition() => entity.currentWeapon.cameraPosition;

        public override bool masked => false;

        protected override void StateAwake(){
            entity.evadeCount = 1;
            entity.jumpCount = 1;
        }

        protected override void StateUpdate(){

            if (entity.currentWeapon.weightModifier > 1f || !entity.inWater){
                entity.SetState("Walking");
            }

            if (entity.evadeInput.started)
                entity.Evade(entity.absoluteForward);

        }

        protected override void StateFixedUpdate(){

            entity.SetRotation(-entity.gravityDown);

            entity._rb.velocity = Vector3.Dot(entity._rb.velocity.normalized, entity.gravityDown) > 0f ? entity._rb.velocity / 1.1f : entity._rb.velocity;

            if ( entity.EvadeUpdate(out float evadeCurve) )
                entity.Move( Global.timeDelta * evadeCurve * entity.data.evadeSpeed * entity.evadeDirection );

            if (entity.moveDirection.magnitude > 0f){

                entity.absoluteForward = entity.moveDirection.normalized;
                if ( (entity.evadeTimer < entity.data.evadeCooldown) ){
                    Vector3 up = Vector3.Cross(entity.absoluteForward, Vector3.Cross(entity.absoluteForward, entity.gravityDown));
                    entity.RotateTowardsAbsolute(entity.absoluteForward, up);
                }

                entity.Move(entity.moveDirection * Global.timeDelta * entity.moveSpeed);
            }


        }

        protected override void UpdateMoveSpeed(){
            float newSpeed = entity.walkSpeed != Entity.WalkSpeed.idle ? entity.data.baseSpeed : 0f;
            if (entity.walkSpeed != Entity.WalkSpeed.run) 
                newSpeed *= entity.walkSpeed == Entity.WalkSpeed.sprint ? /* entity.data.sprintSpeed */1f : entity.data.slowSpeed;

            newSpeed = newSpeed * speedMultiplier * entity.data.swimSpeed;
            
            entity.moveSpeed = Mathf.MoveTowards(entity.moveSpeed, newSpeed, entity.data.moveIncrement * Global.timeDelta);
        }

        public override void HandleInput(){
            
            entity.moveDirection = entity.rotation * entity.lookRotation * entity.moveInput;
            
            if (entity.jumpInput && entity.isOnWaterSurface && entity.jumpCount != 0 && entity.jumpCooldown == 0f ){
                entity.Jump( -entity.gravityDown );
            }
        }
    }
}