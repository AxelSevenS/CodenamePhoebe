using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SeleneGame.Core;

namespace SeleneGame.States {
    
    public class SwimmingState : State{

        public override int id => 1;
        protected override Vector3 GetCameraPosition() => entity.currentWeapon.cameraPosition;
        protected override Vector3 GetEntityUp() => Vector3.Cross(entity.moveDirection, Vector3.Cross(entity.moveDirection, entity.gravityDown));

        protected override bool canJump => entity.isOnWaterSurface && entity.jumpCooldown == 0;
        protected override bool canEvade => entity.evadeTimer == 0f;

        protected override bool canTurn => (entity.evadeTimer < entity.data.evadeCooldown) && !entity.currentWeapon.cannotTurn;
        protected override bool useGravity => false;

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

            if (entity.evading){

                if (entity.evadeTimer > entity.data.evadeCooldown + entity.data.evadeDuration - 0.2f){
                    entity.evadeDirection = entity.absoluteForward;
                }

                entity.Move(entity.evadeDirection * Time.deltaTime * 24f * entity.data.evadeCurve.Evaluate( 1 - ( (entity.evadeTimer - entity.data.evadeCooldown) / entity.data.evadeDuration ) ));

            }

            entity.SetRotation(-entity.gravityDown);

            if (entity.moveDirection.magnitude > 0f){

                entity.absoluteForward = entity.moveDirection.normalized;

                if (canTurn)
                    entity.rotationForward = Vector3.Lerp(entity.rotationForward, entity.relativeForward, 0.7f).normalized;

                entity.Move(entity.moveDirection * Time.deltaTime * entity.moveSpeed);
            }

            RotateEntity(entity.rotationForward);

        }

        protected override void UpdateMoveSpeed(){
            float newSpeed = entity.walkSpeed != Entity.WalkSpeed.idle ? entity.data.baseSpeed : 0f;
            if (entity.walkSpeed != Entity.WalkSpeed.run) 
                newSpeed *= entity.walkSpeed == Entity.WalkSpeed.sprint ? /* entity.data.sprintSpeed */1f : entity.data.slowSpeed;

            newSpeed = newSpeed * speedMultiplier * entity.data.swimSpeed;
            
            entity.moveSpeed = Mathf.MoveTowards(entity.moveSpeed, newSpeed, entity.data.moveIncrement * Time.deltaTime);
        }

        public override void HandleInput(){
            
            entity.moveDirection = entity.lookRotationData.currentValue * entity.moveInputData.currentValue;
            
            if (entity.jumpInputData.currentValue){
                Jump();
            }
        }

        private void OnEvadeInputStart(float timer){
            Evade();
        }
    }
}