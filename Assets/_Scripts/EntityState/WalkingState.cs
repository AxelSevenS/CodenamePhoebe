using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SeleneGame.Core;

namespace SeleneGame.States {
    
    public class WalkingState : State{

        protected override float GetSpeedMultiplier() => entity.currentWeapon.speedMultiplier;
        protected override Vector3 GetJumpDirection() => entity.currentWeapon.jumpDirection;
        protected override Vector3 GetCameraPosition() => entity.currentWeapon.cameraPosition - new Vector3(0,0,additionalCameraDistance);

        public override bool masked => Vector3.Dot ( entity.gravityDown, Vector3.down ) < 0.95f || entity.inWater || entity.currentWeapon.shifting;

        private bool crouching;

        private float additionalCameraDistance;

        public float coyoteTimer = 0f;

        protected override void StateEnable(){

            entity.onJump += OnEntityJump;
            entity.onEvade += OnEntityEvade;
            entity.evadeInputData.started += OnEvadeInputStart;
            entity.groundData.started += OnEntityLand;
            entity.evadingData.stopped += OnEntityEvadeStop;
        }
        protected override void StateDisable(){

            entity.onJump -= OnEntityJump;
            entity.onEvade -= OnEntityEvade;
            entity.evadeInputData.started -= OnEvadeInputStart;
            entity.groundData.started -= OnEntityLand;
            entity.evadingData.stopped -= OnEntityEvadeStop;
        }
        
        protected override void StateUpdate(){
            coyoteTimer = entity.onGround ? 0.4f : Mathf.MoveTowards( coyoteTimer, 0f, Global.timeDelta );

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

            entity.SetRotation(-entity.gravityDown);

            entity.JumpGravity(entity.gravityForce, entity.gravityDown, entity.jumpInputData.currentValue);

            // Move when evading
            if ( entity.EvadeUpdate(out float evadeSpeed) )
                entity.GroundedMove( Global.timeDelta * evadeSpeed * entity.evadeDirection );
            

            
            if ( entity.groundData.currentValue ){
                evadeCount = 1;
                if( entity.jumpCooldown == 0 )
                    jumpCount = 1;
            }

            entity.inertiaDirection = Vector3.Lerp( entity.inertiaDirection, entity.groundOrientation * entity.absoluteForward, 3f*Global.timeDelta);

            // Gain Speed when moving downwards
            float slopeMultiplier = Vector3.Dot( entity.gravityDown, entity.inertiaDirection ) * entity.moveSpeed;
            float fallMultiplier = Mathf.Min(Mathf.Max(-entity.fallVelocity, 0f), 1f);

            float newInertia = entity.onGround ? slopeMultiplier : fallMultiplier;
            newInertia = Mathf.Sign(newInertia) == 1f  ?  Mathf.Pow(newInertia,1.5f)  :  newInertia*0.8f ;
            
            float inertiaChangeFactor = newInertia > entity.inertiaMultiplier ? 12.5f : 5f;

            entity.inertiaMultiplier = Mathf.Min(Mathf.MoveTowards(entity.inertiaMultiplier, newInertia, inertiaChangeFactor * Global.timeDelta), 25f);



            if (entity.moveDirection.magnitude > 0f && !entity.sliding){

                entity.absoluteForward = entity.moveDirection.normalized;

                float finalSpeed = entity.moveSpeed + entity.inertiaMultiplier * Vector3.Dot(entity.moveDirection, entity.inertiaDirection);
                
                entity.GroundedMove(finalSpeed * Global.timeDelta * entity.moveDirection, entity.onGround);
            }else if (entity.sliding){

                entity.absoluteForward = Vector3.Lerp( entity.absoluteForward, entity.moveDirection, Global.timeDelta);

                entity.GroundedMove(Global.timeDelta * 2.5f * entity.inertia );
            }
            
            Vector3 finalRotation = entity?.currentWeapon.overrideRotation ?? entity.relativeForward;
            entity.RotateTowardsRelative(finalRotation, -entity.gravityDown);
        }

        protected override void UpdateMoveSpeed(){
            float newSpeed = entity.moveInputData.currentValue.magnitude > 0f ? entity.data.baseSpeed : 0f;
            if (entity.walkSpeed != Entity.WalkSpeed.run) 
                newSpeed *= entity.walkSpeed == Entity.WalkSpeed.sprint ? /* entity.data.sprintSpeed */1f : entity.data.slowSpeed;

            newSpeed = newSpeed * speedMultiplier;
            
            entity.moveSpeed = Mathf.MoveTowards(entity.moveSpeed, newSpeed, entity.data.moveIncrement * Global.timeDelta);
        }

        public override void HandleInput(){

            entity.RawInputToGroundedMovement(out Vector3 camRight, out Vector3 camForward, out Vector3 groundDirection, out Vector3 groundDirection3D);
            entity.moveDirection = groundDirection;

            if (entity.crouchInputData.currentValue) entity.SetWalkSpeed(Entity.WalkSpeed.crouch);
            else if ((groundDirection.magnitude <= 0.25f || entity.walkInputData.currentValue) && entity.onGround) entity.SetWalkSpeed(Entity.WalkSpeed.walk);
            else entity.SetWalkSpeed(Entity.WalkSpeed.run);
            
            entity.slidingData.SetVal( entity.evadeInputData.currentValue && !entity.evading && (entity.inertiaMultiplier > 2f | !entity.onGround) && !entity.inWater );
            
            if ( entity.jumpInputData.currentValue && jumpCount != 0 && coyoteTimer != 0f )
                entity.Jump(jumpDirection);
        }

        private void OnEntityJump(Vector3 jumpDirection){
            jumpCount--;
        }
        private void OnEntityEvade(Vector3 evadeDirection){
            evadeCount--;
        }

        private void OnEvadeInputStart(float timer){
            if ( evadeCount == 0 ) return;

            Vector3 evadeDir = entity.moveDirection.magnitude == 0f ? -entity.absoluteForward : entity.moveDirection;
            entity.GroundedEvade(evadeDir);
        }

        private void OnEntityEvadeStop(float timer){
            if ( entity.sliding ) return;

            entity.StartWalkAnim();
        }

        private void OnEntityLand(float timer){
            entity.StartWalkAnim();
        }
    }
}
