using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SeleneGame.Core;

namespace SeleneGame.States {
    
    public class VehicleState : State{

        public override int id => 7;
        protected override Vector3 GetCameraPosition() => new Vector3(1f, 1f, -3.5f);
        protected override Vector3 GetEntityUp(){
            if ( Vector3.Dot(entity.groundOrientation * -entity.gravityDown, -entity.gravityDown) > 0.75f)
                return entity.groundOrientation * -entity.gravityDown;
            return -entity.gravityDown;
        }

        protected override bool canJump => (coyoteTimer > 0f && jumpCount > 0 && useGravity) && entity.jumpCooldown == 0;
        protected override bool canEvade => (evadeCount > 0f && entity.evadeTimer == 0f);
        
        protected override bool canTurn => (entity.evadeTimer < entity.data.evadeCooldown);
        protected override bool useGravity => true;

        public override bool masked => false;

        private float moveAmount = 0f;
        private float turnDirection = 0f;

        private bool landed;
        public float coyoteTimer = 0f;

        protected override void StateEnable(){

            entity.groundData.started += OnEntityLand;
        }
        protected override void StateDisable(){

            entity.groundData.started -= OnEntityLand;
        }

        protected override void StateFixedUpdate(){

            Gravity(entity.gravityForce, entity.gravityDown);

            coyoteTimer = Mathf.Max( Mathf.MoveTowards( coyoteTimer, 0f, Time.deltaTime ), (System.Convert.ToSingle(entity.onGround) * 0.4f) );
            
            if ( entity.groundData.currentValue ){
                evadeCount = 1;
                if( entity.jumpCooldown == 0 )
                    jumpCount = 1;
            }


            // When the Entity is sliding
            if (entity.sliding)
                entity._rb.velocity += entity.groundOrientation * entity.evadeDirection *entity.data.baseSpeed * entity.inertiaMultiplier * Time.deltaTime;



            //  ---------------------------- When the Entity is Focusing
            if ( entity.onGround && entity.focusing)
                entity.gravityDown = Vector3.Lerp( entity.gravityDown, -entity.groundHit.normal, 0.1f );
            
            entity.SetRotation(-entity.gravityDown);

            
            
            // Handling Tank Movement Logic.
            float turningSpeed = (-(Mathf.Max(entity.moveSpeed, .1f)/entity.data.baseSpeed) + 2.5f)/1.25f;
            
            entity.relativeForward = Quaternion.AngleAxis(180f * turningSpeed * turnDirection * Time.deltaTime, Vector3.up) * entity.relativeForward;
            entity.rotationForward = Vector3.Lerp(entity.rotationForward, entity.relativeForward, 0.7f).normalized;

            entity.moveDirection = entity.absoluteForward * moveAmount;
            entity.GroundedMove(entity.moveDirection * Time.deltaTime * entity.moveSpeed);
            
            RotateEntity(entity.rotationForward);

        }

        protected override void UpdateMoveSpeed(){
            float newSpeed = entity.walkSpeed != Entity.WalkSpeed.idle ? entity.data.baseSpeed : 0f;
            if (entity.walkSpeed != Entity.WalkSpeed.run) 
                newSpeed *= entity.walkSpeed == Entity.WalkSpeed.sprint ? /* entity.data.sprintSpeed */1f : entity.data.slowSpeed;

            newSpeed = newSpeed * speedMultiplier;
            
            entity.moveSpeed = Mathf.MoveTowards(entity.moveSpeed, newSpeed, entity.data.moveIncrement * Time.deltaTime);
        }


        public override void HandleInput(){
            
            moveAmount = entity.moveInputData.currentValue.z;
            turnDirection = entity.moveInputData.currentValue.x;
            
            // Jump if the Jump key is pressed.
            if (entity.jumpInputData.currentValue){
                Jump();
            }
        }

        private void OnEntityLand(float timer){
            entity.StartWalkAnim();
        }
    }
}
