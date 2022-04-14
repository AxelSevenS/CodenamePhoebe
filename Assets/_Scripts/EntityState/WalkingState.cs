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

        public override bool sliding => entity.evadeInputData.currentValue && !entity.evading && !(entity.inertiaMultiplier < 0.1f && entity.onGround) && !entity.inWater;
        private bool crouching;

        private float additionalCameraDistance;

        public float coyoteTimer = 0f;

        private void OnEnable(){
            entity.groundData.startAction += OnEntityLand;
            entity.evadeInputData.startAction += OnEvadeInputStart;
            entity.evadingData.stopAction += OnEntityEvadeStop;
        }
        private void OnDisable(){
            entity.groundData.startAction -= OnEntityLand;
            entity.evadeInputData.startAction -= OnEvadeInputStart;
            entity.evadingData.stopAction -= OnEntityEvadeStop;
        }
        
        public override void StateUpdate(){

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

        public override void StateFixedUpdate(){
            
            if (!entity.currentWeapon.noGravity)
                Gravity(entity.gravityForce, entity.gravityDown);

            coyoteTimer = Mathf.Max( Mathf.MoveTowards( coyoteTimer, 0f, Time.deltaTime ), (System.Convert.ToSingle(entity.onGround) * 0.4f) );
            
            if ( entity.groundData.currentValue ){
                evadeCount = 1;
                if( entity.jumpCooldown == 0 )
                    jumpCount = 1;
            }
            
            // Jump if the Jump key is pressed.
            if (entity.jumpInputData.currentValue){
                Jump();
            }

            entity.inertiaDirection = Vector3.Lerp( entity.inertiaDirection, entity.absoluteForward, 3f*Time.deltaTime);

            // Gain Speed when moving downwards
            float slopeMultiplier = Vector3.Dot( entity.gravityDown, entity.groundOrientation * entity.inertiaDirection ) * entity.moveSpeed;
            float fallMultiplier = Mathf.Min(Mathf.Max(-entity.fallVelocity, 0f), 1f);
            float newInertia = Mathf.Pow( (entity.onGround ? slopeMultiplier : fallMultiplier), 3f );
            float inertiaChangeFactor = newInertia > entity.inertiaMultiplier ? 12.5f : 7.5f;

            entity.inertiaMultiplier = Mathf.Min(Mathf.MoveTowards(entity.inertiaMultiplier, newInertia, inertiaChangeFactor * Time.deltaTime), 25f);
            // Debug.Log(entity.inertiaMultiplier);



            // --------------------------------------------------------------------------------------------------------------------------
            // Evading
            // --------------------------------------------------------------------------------------------------------------------------

            if ( (entity.evadeTimer > entity.data.totalEvadeDuration - 0.2f || !entity.evading) && entity.moveDirection.magnitude > 0f){

                // Change Evade Direction if it's still early in the Evade.
                entity.evadeDirection = entity.absoluteForward;
            }else if (!entity.evading && !entity.sliding && entity.onGround && entity.moveDirection.magnitude == 0f){

                // Backstep when Evading while standing still.
                entity.evadeDirection = -entity.absoluteForward;
            }

            if (entity.evading){

                entity.inertiaDirection = entity.evadeDirection;
                entity.GroundedMove(entity.evadeDirection * Time.deltaTime * 24f * entity.data.evadeCurve.Evaluate( 1 - ( (entity.evadeTimer - entity.data.evadeCooldown) / entity.data.evadeDuration ) ));

            }


            // --------------------------------------------------------------------------------------------------------------------------
            // Sliding
            // --------------------------------------------------------------------------------------------------------------------------
            




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
                
                entity.GroundedMove(entity.moveSpeed * Time.deltaTime * entity.moveDirection, entity.onGround);
            }
            if (entity.sliding){
                entity.GroundedMove(entity.inertia * Time.deltaTime);
            }
            

            RotateEntity(entity.rotationForward);

            // float newHeight = (entity.sliding || entity.walkSpeed == Entity.WalkSpeed.crouch) ? entity.data.size.y*2f/3f : entity.data.size.y;
            // Vector3 newSize = new Vector3(entity.data.size.x, newHeight, entity.data.size.z);

            // entity.SetSize( Vector3.Lerp( entity._collider.GetSize(), newSize, 10f * Time.deltaTime) );
        }

        private void OnEntityLand(float timer){
            entity.StartWalkAnim();
        }

        private void OnEvadeInputStart(float timer){
            Evade();
        }
        private void OnEntityEvadeStop(float timer){
            if ( !entity.sliding )
                entity.StartWalkAnim();
        }
    }
}
