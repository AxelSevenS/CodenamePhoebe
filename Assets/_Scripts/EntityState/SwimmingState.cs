using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SeleneGame.Core;

namespace SeleneGame.States {
    
    public class SwimmingState : State{

        public override int id => 1;
        protected override Vector3 GetCameraPosition() => entity.currentWeapon.cameraPosition;
        protected override Vector3 GetEntityUp(){
            return Vector3.Cross(entity.moveDirection, Vector3.Cross(entity.moveDirection, entity.gravityDown));
        }

        protected override bool canJump => entity.isOnWaterSurface && entity.jumpCooldown == 0;
        protected override bool canEvade => entity.evadeTimer == 0f;

        protected override bool canTurn => (entity.evadeTimer < entity.data.evadeCooldown) && !entity.currentWeapon.cannotTurn;
        protected override bool useGravity => false;

        public override bool masked => false;

        private void OnEnable(){
            entity.evadeInputData.startAction += OnEvadeInputStart;
        }
        private void OnDisable(){
            entity.evadeInputData.startAction -= OnEvadeInputStart;
        }

        public override void StateUpdate(){ 

            if (entity.currentWeapon.weightModifier > 1f || !entity.inWater){
                entity.SetState("Walking");
            }

        }

        public override void StateFixedUpdate(){


            // ---------------------------- When the Entity is Evading.
            if (entity.evading){

                if (entity.evadeTimer > entity.data.evadeCooldown + entity.data.evadeDuration - 0.2f){
                    entity.evadeDirection = entity.absoluteForward;
                }

                entity.Move(entity.evadeDirection * Time.deltaTime * 24f * entity.data.evadeCurve.Evaluate( 1 - ( (entity.evadeTimer - entity.data.evadeCooldown) / entity.data.evadeDuration ) ));

            }
            
            // Jump if the Jump key is pressed.
            if (entity.jumpInputData.currentValue){
                Jump();
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

        public override void HandleInput(){
            
            entity.moveDirection = entity.lookRotationData.currentValue * entity.moveInputData.currentValue;
        }

        private void OnEvadeInputStart(float timer){
            Evade();
        }
    }
}