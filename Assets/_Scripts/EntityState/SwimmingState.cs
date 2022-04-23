using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SeleneGame.Core;

namespace SeleneGame.States {
    
    public class SwimmingState : State{

        public override int id => 1;
        protected override Vector3 GetCameraPosition() => entity.currentWeapon.cameraPosition;

        public override bool masked => false;

        protected override void StateEnable(){

            entity.evadeInputData.started += OnEvadeInputStart;
        }
        protected override void StateDisable(){

            entity.evadeInputData.started -= OnEvadeInputStart;
        }

        protected override void StateUpdate(){

            if (entity.currentWeapon.weightModifier > 1f || !entity.inWater){
                entity.SetState("Walking");
            }

        }

        protected override void StateFixedUpdate(){
            if ( entity.EvadeUpdate(out float evadeSpeed) )
                entity.Move( Global.timeDelta * evadeSpeed * entity.evadeDirection );

            entity.SetRotation(-entity.gravityDown);

            if (entity.moveDirection.magnitude > 0f){

                entity.absoluteForward = entity.moveDirection.normalized;
                if ( (entity.evadeTimer < entity.data.evadeCooldown) ){
                    Vector3 up = Vector3.Cross(entity.moveDirection, Vector3.Cross(entity.moveDirection, entity.gravityDown));
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
            
            entity.moveDirection = entity.lookRotationData.currentValue * entity.moveInputData.currentValue;
            
            if (entity.jumpInputData.currentValue && entity.isOnWaterSurface){
                entity.Jump( -entity.gravityDown );
            }
        }

        private void OnEvadeInputStart(float timer){
            entity.Evade(entity.absoluteForward);
        }
    }
}