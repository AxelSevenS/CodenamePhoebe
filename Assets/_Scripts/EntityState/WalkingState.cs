using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SeleneGame.Core;
using SeleneGame.Entities;
using SeleneGame.Utility;

namespace SeleneGame.States {
    
    public class WalkingState : State {

        public override StateType stateType => StateType.groundState;

        protected override Vector3 GetJumpDirection(){
            if (entity is GravityShifterEntity gravityShifter)
                return gravityShifter.weapons.current.jumpDirection;
            return base.GetJumpDirection();
        }
        protected override Vector3 GetCameraPosition() {
            // if (entity is ArmedEntity armed) {
            //     return armed.weapons.current.cameraPosition - new Vector3(0,0,additionalCameraDistance);
            // }
            return base.GetCameraPosition() - new Vector3(0,0,additionalCameraDistance);
        }

        private bool crouching;

        private float additionalCameraDistance;

        BoolData waterHover = new BoolData();

        RaycastHit waterHoverHit;



        public override void OnEnter(Entity entity){
            base.OnEnter(entity);

            entity.onJump += OnEntityJump;
        }
        public override void OnExit(){
            base.OnExit();

            entity.onJump -= OnEntityJump;
        }
        
        public override void StateUpdate(){

            base.StateUpdate();

            entity.jumpCooldown = Mathf.MoveTowards( entity.jumpCooldown, 0f, GameUtility.timeDelta );

            /* const float jumpBufferTime = 0.1f; */
            const float jumpCoyoteTime = 0.2f;

            bool jumpInputPressed = ( /* entity.jumpInput.falseTimer < jumpBufferTime && */ entity.jumpInput && entity.onGround.falseTimer < jumpCoyoteTime );
            if ( jumpInputPressed && entity.jumpCount > 0 && entity.jumpCooldown == 0f ){
                entity.Jump(jumpDirection);
                entity.jumpCount--;
            }

            if ( entity.onGround.started ){
                entity.jumpCount = 1;
                entity.animator?.CrossFade("Land", 0.1f);
            }

            if (entity.inWater && !entity.CanSink() && !entity.CanWaterHover())
                entity.SetState(new SwimmingState());

            // if ( entity.onGround ) entity.SetAnimationState( entity.moveDirection.magnitude > 0f ? "Run" : "Idle", 0.1f);
            // if ( entity.onGround) {
                
            //     if ( entity.moveDirection != Vector3.zero && !entity.currentAnimationState.Contains("Run") ){
            //         entity.animator?.SetTrigger("RunningStart");
            //     }
            //     // else
            //     //     entity.SetAnimationState("Idle", 0.1f);

            // }



            if (entity is ArmedEntity armed) {
                // if (armed.evading.stopped && !armed.sliding)
                //     armed.StartWalkAnim();

                if ( armed.evading.started)
                    armed.evadeCount--;
                if ( armed.onGround.started )
                    armed.evadeCount = 1;

                if ( armed.evadeInput.started && armed.evadeCount != 0 )
                    armed.GroundedEvade( armed.moveDirection.magnitude == 0f ? -armed.absoluteForward : armed.moveDirection );
            }

                



            additionalCameraDistance = /* entity.focusing ? -0.3f :  */0f;
            if (entity.walkSpeed == Entity.WalkSpeed.sprint){
                additionalCameraDistance += 0.3f;
            }else if (entity.walkSpeed == Entity.WalkSpeed.walk){
                additionalCameraDistance += -0.2f;
            }

            if (entity.shiftInput.trueTimer > Player.current.holdDuration){
                entity.gravityDown = Vector3.down;
            }
        }

        public override void StateFixedUpdate(){

            base.StateFixedUpdate();

            entity.SetRotation(-entity.gravityDown);

            entity.JumpGravity(entity.GravityMultiplier(), entity.gravityDown, entity.jumpInput);

            

            // entity.inertiaDirection = Vector3.Lerp( entity.inertiaDirection, entity.groundOrientation * entity.absoluteForward, 3f*GameUtility.timeDelta);

            // // Gain Speed when moving downwards
            // float slopeMultiplier = Vector3.Dot( entity.gravityDown, entity.inertiaDirection ) * entity.moveSpeed;
            // float fallMultiplier = Mathf.Min(Mathf.Max(-entity.fallVelocity, 0f), 1f);

            // float newInertia = entity.onGround ? slopeMultiplier : fallMultiplier;
            // newInertia = Mathf.Sign(newInertia) == 1f  ?  Mathf.Pow(newInertia,1.5f)  :  newInertia*0.8f ;
            
            // float inertiaChangeFactor = newInertia > entity.inertiaMultiplier ? 12.5f : 5f;

            // entity.inertiaMultiplier = Mathf.Min(Mathf.MoveTowards(entity.inertiaMultiplier, newInertia, inertiaChangeFactor * GameUtility.timeDelta), 25f);


            // Hover over water as long as the player is moving
            if ( waterHover ){
                entity.rb.velocity = entity.rb.velocity.NullifyInDirection(entity.gravityDown);
                entity.groundHit = waterHoverHit;
            }

            if (entity.moveDirection.magnitude > 0f && entity.CanTurn() )
                entity.absoluteForward = Vector3.Lerp( entity.absoluteForward, entity.moveDirection.normalized, 100f * GameUtility.timeDelta);


            Vector3 walkingMovement = entity.moveSpeed * GameUtility.timeDelta * entity.absoluteForward;
            if (entity is ArmedEntity armed) {

                // Move when evading
                if ( armed.EvadeUpdate(out _, out float evadeSpeed) )
                    armed.GroundedMove( GameUtility.timeDelta * evadeSpeed * armed.data.evadeSpeed * armed.evadeDirection );

                entity.GroundedMove((1 - evadeSpeed) * walkingMovement, entity.onGround);
            }else {
                entity.GroundedMove(walkingMovement, entity.onGround);
            }
            
            
            entity.RotateTowardsAbsolute(entity.absoluteForward, -entity.gravityDown);
        }

        public override void HandleInput(){

            if (entity == null) return;

            entity.RawInputToGroundedMovement(out Vector3 camRight, out Vector3 camForward, out Vector3 groundDirection, out Vector3 groundDirection3D);
            entity.moveDirection.SetVal(groundDirection);

            if (entity.crouchInput)
                entity.SetWalkSpeed(Entity.WalkSpeed.crouch);
            else if ( (groundDirection.magnitude <= 0.25f || entity.walkInput) && entity.onGround ) 
                entity.SetWalkSpeed(Entity.WalkSpeed.walk);
            else if ( entity.evadeInput )
                entity.SetWalkSpeed(Entity.WalkSpeed.sprint);
            else 
                entity.SetWalkSpeed(Entity.WalkSpeed.run);
            
            // entity.slidingData.SetVal( entity.evadeInput && !entity.evading && (entity.inertiaMultiplier > 2f | !entity.onGround) && !entity.inWater );

            float newSpeed = entity.moveDirection.magnitude > 0f ? entity.data.baseSpeed : 0f;
            if (entity.walkSpeed != Entity.WalkSpeed.run) 
                newSpeed *= entity.walkSpeed == Entity.WalkSpeed.sprint ? entity.data.sprintMultiplier : entity.data.slowMultiplier;

            float speedDelta = newSpeed > entity.moveSpeed ? 1f : 0.65f;
            entity.moveSpeed = Mathf.MoveTowards(entity.moveSpeed, newSpeed, speedDelta * entity.data.moveIncrement * GameUtility.timeDelta);
        }

        
        private void OnEntityJump(Vector3 jumpDirection){
            entity.jumpCount--;
        }

    }
}