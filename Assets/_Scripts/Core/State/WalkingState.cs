using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SevenGame.Utility;

namespace SeleneGame.Core {
    
    public class WalkingState : State {

        private const float entityWaterHoverWeightTreshold = 10f;

        public WalkSpeed walkSpeed;
        private bool crouching;
        private float additionalCameraDistance;


        public override StateType stateType => StateType.groundState;

        public override Vector3 cameraPosition => base.cameraPosition - new Vector3(0,0,additionalCameraDistance);



        public void SetWalkSpeed(WalkSpeed newSpeed) {
            if (walkSpeed != newSpeed){
                // if ((int)walkSpeed < (int)newSpeed && !currentAnimationState.Contains("Run")) SetAnimationState("RunningStart", 0.1f);
                walkSpeed = newSpeed;
            }
        }
        
        private void OnEntityJump(Vector3 jumpDirection){
            // entity.jumpCount--;
        }


        public override void OnEnter(Entity entity){
            base.OnEnter(entity);

            entity.onJump += OnEntityJump;
        }
        public override void OnExit(){
            base.OnExit();

            entity.onJump -= OnEntityJump;
        }
        
        public override void HandleInput(){

            if (entity == null) return;

            if (entity.controller.shiftInput.trueTimer > Global.HOLDTIME){
                entity.gravityDown = Vector3.down;
            }

            entity.controller.RawInputToGroundedMovement(out Quaternion cameraRotation, out Vector3 groundedMovement);

            if ( !(entity is ArmedEntity armed) || !armed.evading)
                entity.moveDirection.SetVal(groundedMovement);

            if (entity.controller.crouchInput)
                SetWalkSpeed(WalkSpeed.crouch);
            else if ( (groundedMovement.sqrMagnitude <= 0.25 || entity.controller.walkInput) && entity.onGround ) 
                SetWalkSpeed(WalkSpeed.walk);
            else if ( entity.controller.evadeInput )
                SetWalkSpeed(WalkSpeed.sprint);
            else 
                SetWalkSpeed(WalkSpeed.run);


            float newSpeed = entity.isIdle ? 0f : entity.character.baseSpeed;
            if (walkSpeed != WalkSpeed.run) 
                newSpeed *= walkSpeed == WalkSpeed.sprint ? entity.character.sprintMultiplier : entity.character.slowMultiplier;


            float speedDelta = newSpeed > entity.moveSpeed ? 1f : 0.65f;
            entity.moveSpeed = Mathf.MoveTowards(entity.moveSpeed, newSpeed, speedDelta * entity.character.acceleration * GameUtility.timeDelta);

        }
        
        public override void StateUpdate(){

            base.StateUpdate();

            entity.SetRotation(-entity.gravityDown);

            
            // Hover over water as long as the entity is moving
            RaycastHit waterHoverHit = new RaycastHit();
            bool canWaterHover = entity.weight < entityWaterHoverWeightTreshold && entity.controller.moveInput.zeroTimer < 0.6f;
            bool waterHover = canWaterHover && entity.ColliderCast(Vector3.zero, entity.gravityDown * 0.2f, out waterHoverHit, 0.15f, Global.WaterMask);

            if ( waterHover ) {
                entity.onGround.SetVal(true);
                entity.groundHit = waterHoverHit;
                entity.rigidbody.velocity = entity.rigidbody.velocity.NullifyInDirection(entity.gravityDown);
            } else if ( entity.inWater && entity.weight < SwimmingState.entityWeightSinkTreshold ) {
                entity.SetState(new SwimmingState());
            }



            if ( !entity.isIdle )
                entity.absoluteForward = Vector3.Lerp( entity.absoluteForward, entity.moveDirection.normalized, 100f * GameUtility.timeDelta);

            if ( entity is ArmedEntity armed ) {

                if ( armed.evading.started)
                    armed.evadeCount--;
                if ( armed.onGround )
                    armed.evadeCount = 1;

                if ( armed.controller.evadeInput.started && armed.evadeCount > 0 )
                    armed.GroundedEvade( armed.isIdle ? -armed.absoluteForward : armed.moveDirection );
                    
            }


            /* const float jumpBufferTime = 0.1f; */
            const float jumpCoyoteTime = 0.2f;

            bool jumpInputPressed = ( /* entity.jumpInput.falseTimer < jumpBufferTime && */ entity.controller.jumpInput && entity.onGround.falseTimer < jumpCoyoteTime );
            if ( jumpInputPressed && entity.jumpCount > 0 && entity.jumpCooldownTimer.isDone ){
                entity.Jump(jumpDirection);
            }

            if ( entity.onGround.started ){
                entity.jumpCount = 1;
                entity.animator.CrossFade("Land", 0.1f);
            }




            additionalCameraDistance = /* entity.focusing ? -0.3f :  */0f;
            if (walkSpeed == WalkSpeed.sprint){
                additionalCameraDistance += 0.3f;
            }else if (walkSpeed == WalkSpeed.walk){
                additionalCameraDistance += -0.2f;
            }
            
            
            entity.RotateTowardsAbsolute(entity.absoluteForward, -entity.gravityDown);
        }

        public override void StateFixedUpdate(){

            base.StateFixedUpdate();

            entity.JumpGravity(entity.weight, entity.gravityDown, entity.controller.jumpInput);



            Vector3 walkingMovement = entity.moveSpeed * GameUtility.timeDelta * entity.absoluteForward;
            if (entity is ArmedEntity armed) {

                // Move when evading
                if ( armed.EvadeUpdate(out _, out float evadeSpeed) )
                    armed.GroundedMove( GameUtility.timeDelta * evadeSpeed * armed.character.evadeSpeed * armed.evadeDirection );

                walkingMovement *= (1 - evadeSpeed);

            }
            entity.GroundedMove(walkingMovement, entity.onGround);

            

            // entity.inertiaDirection = Vector3.Lerp( entity.inertiaDirection, entity.groundOrientation * entity.absoluteForward, 3f*GameUtility.timeDelta);

            // // Gain Speed when moving downwards
            // float slopeMultiplier = Vector3.Dot( entity.gravityDown, entity.inertiaDirection ) * entity.moveSpeed;
            // float fallMultiplier = Mathf.Min(Mathf.Max(-entity.fallVelocity, 0f), 1f);

            // float newInertia = entity.onGround ? slopeMultiplier : fallMultiplier;
            // newInertia = Mathf.Sign(newInertia) == 1f  ?  Mathf.Pow(newInertia,1.5f)  :  newInertia*0.8f ;
            
            // float inertiaChangeFactor = newInertia > entity.inertiaMultiplier ? 12.5f : 5f;

            // entity.inertiaMultiplier = Mathf.Min(Mathf.MoveTowards(entity.inertiaMultiplier, newInertia, inertiaChangeFactor * GameUtility.timeDelta), 25f);



        }



        

        public enum WalkSpeed {idle, crouch, walk, run, sprint};

    }
}