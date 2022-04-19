using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SeleneGame.Core;

namespace SeleneGame.States {
    
    public class WalkingState : State{

        protected override float GetSpeedMultiplier() => entity.currentWeapon.speedMultiplier;
        protected override Vector3 GetJumpDirection() => entity.currentWeapon.jumpDirection;
        protected override Vector3 GetCameraPosition() => entity.currentWeapon.cameraPosition - new Vector3(0,0,additionalCameraDistance);
        protected override Vector3 GetEntityUp(){
            if (entity.inWater && !useGravity)
                return Vector3.Cross(entity.moveDirection, Vector3.Cross(entity.moveDirection, entity.gravityDown));
            else
                return -entity.gravityDown;
        }

        protected override bool canJump => ( (coyoteTimer > 0f && jumpCount > 0 && useGravity) || entity.currentWeapon.canJump) && entity.jumpCooldown == 0;
        protected override bool canEvade => (evadeCount > 0f && entity.evadeTimer == 0f) || entity.currentWeapon.canEvade;

        protected override bool canTurn => (entity.evadeTimer < entity.data.evadeCooldown) && !entity.turningTo && !entity.currentWeapon.cannotTurn;
        protected override bool useGravity => !entity.currentWeapon.noGravity;

        public override bool masked => Vector3.Dot ( entity.gravityDown, Vector3.down ) < 0.95f || entity.inWater || entity.currentWeapon.shifting;

        private bool crouching;

        private float additionalCameraDistance;

        public float coyoteTimer = 0f;

        protected override void StateEnable(){

            entity.groundData.started += OnEntityLand;
            entity.evadeInputData.started += OnEvadeInputStart;
            entity.evadingData.stopped += OnEntityEvadeStop;
        }
        protected override void StateDisable(){

            entity.groundData.started -= OnEntityLand;
            entity.evadeInputData.started -= OnEvadeInputStart;
            entity.evadingData.stopped -= OnEntityEvadeStop;
        }
        
        protected override void StateUpdate(){

            if (entity.inWater && entity.currentWeapon.weightModifier <= 1f){
                entity.SetState("Swimming");
            }

            additionalCameraDistance = entity.focusing ? -0.3f : 0f;
            if (entity.walkSpeed == Entity.WalkSpeed.sprint){
                additionalCameraDistance += 0.3f;
            }else if (entity.walkSpeed == Entity.WalkSpeed.walk){
                additionalCameraDistance += -0.2f;
            }

            if (entity.shiftInputData.trueTimer > Player.current.holdDuration){
                entity.gravityDown = Vector3.down;
            }
        }

        protected override void StateFixedUpdate(){
            
            if (!entity.currentWeapon.noGravity)
                Gravity(entity.gravityForce, entity.gravityDown);

            coyoteTimer = entity.onGround ? 0.4f : Mathf.MoveTowards( coyoteTimer, 0f, Time.deltaTime );
            
            if ( entity.groundData.currentValue ){
                evadeCount = 1;
                if( entity.jumpCooldown == 0 )
                    jumpCount = 1;
            }

            entity.inertiaDirection = Vector3.Lerp( entity.inertiaDirection, entity.absoluteForward, 3f*Time.deltaTime);

            // Gain Speed when moving downwards
            float slopeMultiplier = Vector3.Dot( entity.gravityDown, entity.groundOrientation * entity.inertiaDirection ) * entity.moveSpeed;
            float fallMultiplier = Mathf.Min(Mathf.Max(-entity.fallVelocity, 0f), 1f);
            float newInertia = Mathf.Pow( (entity.onGround ? slopeMultiplier : fallMultiplier), 1.5f );
            float inertiaChangeFactor = newInertia > entity.inertiaMultiplier ? 12.5f : 5f;

            entity.inertiaMultiplier = Mathf.Min(Mathf.MoveTowards(entity.inertiaMultiplier, newInertia, inertiaChangeFactor * Time.deltaTime), 25f);
            // Debug.Log(entity.inertiaMultiplier);



            // --------------------------------------------------------------------------------------------------------------------------
            // Evading
            // --------------------------------------------------------------------------------------------------------------------------

            if ( (entity.evadeTimer > entity.data.totalEvadeDuration - 0.1f || !entity.evading) && entity.moveDirection.magnitude > 0f){

                // Change Evade Direction if it's still early in the Evade.
                entity.evadeDirection = entity.absoluteForward;
            }else if (!entity.evading && !entity.sliding && entity.onGround && entity.moveDirection.magnitude == 0f){

                // Backstep when Evading while standing still.
                entity.evadeDirection = -entity.absoluteForward;
            }

            if (entity.evading){

                entity.inertiaDirection = entity.evadeDirection;
                float evadeCurve = entity.data.evadeCurve.Evaluate( 1 - ( (entity.evadeTimer - entity.data.evadeCooldown) / entity.data.evadeDuration ) );
                float evadeSpeed = 24f;
                entity.GroundedMove( Time.deltaTime * evadeSpeed * evadeCurve * entity.evadeDirection );

            }


            // --------------------------------------------------------------------------------------------------------------------------
            // Sliding
            // --------------------------------------------------------------------------------------------------------------------------
            

            if (entity.sliding){
                entity.absoluteForward = Vector3.Lerp( entity.absoluteForward, entity.moveDirection, Time.deltaTime);
                entity.GroundedMove(Time.deltaTime * 2.5f * entity.inertia );
            }



            // --------------------------------------------------------------------------------------------------------------------------
            // Crouching
            // --------------------------------------------------------------------------------------------------------------------------





            // --------------------------------------------------------------------------------------------------------------------------
            // Movement and misc.
            // --------------------------------------------------------------------------------------------------------------------------

            entity.SetRotation(-entity.gravityDown);

            Vector3 finalRotation = entity.currentWeapon != null ? entity.currentWeapon.overrideRotation : entity.relativeForward;
            entity.rotationForward = Vector3.Lerp(entity.rotationForward, finalRotation, 0.7f).normalized;

            if (entity.moveDirection.magnitude > 0f && !entity.sliding){

                entity.absoluteForward = entity.moveDirection.normalized;

                float finalSpeed = entity.moveSpeed + Vector3.Dot(entity.moveDirection, entity.inertiaDirection) * entity.inertiaMultiplier;
                
                entity.GroundedMove(finalSpeed * Time.deltaTime * entity.moveDirection, entity.onGround);
            }
            

            RotateEntity(entity.rotationForward);

            // float newHeight = (entity.sliding || entity.walkSpeed == Entity.WalkSpeed.crouch) ? entity.data.size.y*2f/3f : entity.data.size.y;
            // Vector3 newSize = new Vector3(entity.data.size.x, newHeight, entity.data.size.z);

            // entity.SetSize( Vector3.Lerp( entity._collider.GetSize(), newSize, 10f * Time.deltaTime) );
        }

        protected override void UpdateMoveSpeed(){
            float newSpeed = entity.walkSpeed != Entity.WalkSpeed.idle ? entity.data.baseSpeed : 0f;
            if (entity.walkSpeed != Entity.WalkSpeed.run) 
                newSpeed *= entity.walkSpeed == Entity.WalkSpeed.sprint ? /* entity.data.sprintSpeed */1f : entity.data.slowSpeed;

            newSpeed = newSpeed * speedMultiplier;
            
            entity.moveSpeed = Mathf.MoveTowards(entity.moveSpeed, newSpeed, entity.data.moveIncrement * Time.deltaTime);
        }

        public override void HandleInput(){

            RawInputToGroundedMovement(entity, out Vector3 camRight, out Vector3 camForward, out Vector3 groundDirection, out Vector3 groundDirection3D);
            entity.moveDirection = groundDirection;

            if (entity.crouchInputData.currentValue) entity.SetWalkSpeed(Entity.WalkSpeed.crouch);
            else if ((groundDirection.magnitude <= 0.25f || entity.walkInputData.currentValue) && entity.onGround) entity.SetWalkSpeed(Entity.WalkSpeed.walk);
            else entity.SetWalkSpeed(Entity.WalkSpeed.run);
            
            entity.slidingData.SetVal( entity.evadeInputData.currentValue && !entity.evading && (entity.inertiaMultiplier > 2f | !entity.onGround) && !entity.inWater );
            
            // Jump if the Jump key is pressed.
            if (entity.jumpInputData.currentValue){
                Jump();
                jumpCount--;
            }
        }

        private void OnEntityLand(float timer){
            entity.StartWalkAnim();
        }

        private void OnEvadeInputStart(float timer){
            Evade();
            evadeCount--;
        }
        private void OnEntityEvadeStop(float timer){
            if ( !entity.sliding )
                entity.StartWalkAnim();
        }
    }
}
