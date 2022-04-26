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

        BoolData waterHoverData = new BoolData();

        RaycastHit waterHoverHit;

        private bool waterHover => waterHoverData.currentValue;

        private float jumpCooldown;


        private void OnEnable(){

            entity.onJump += OnEntityJump;
            entity.onEvade += OnEntityEvade;
        }
        private void OnDisable(){

            entity.onJump -= OnEntityJump;
            entity.onEvade -= OnEntityEvade;
        }
        
        protected override void StateUpdate(){

            jumpCooldown = Mathf.MoveTowards( jumpCooldown, 0f, Global.timeDelta );

            waterHoverData.SetVal( entity.currentWeapon.weightModifier < 0.8f && entity.moveInputData.zeroTimer < 0.6f && entity.ColliderCast( entity.gravityDown.normalized * 0.15f, out waterHoverHit, 0.15f, Global.WaterMask ) );
            entity.groundData.SetVal( entity.groundData.currentValue || waterHover );

            if (entity.inWater && entity.currentWeapon.weightModifier <= 1f){
                entity.SetState("Swimming");
            }

            if (entity.evadeInputData.started && evadeCount != 0)
                entity.GroundedEvade( entity.moveDirection.magnitude == 0f ? -entity.absoluteForward : entity.moveDirection );

            if ( entity.groundData.started || (entity.evadingData.stopped && !entity.sliding) ) 
                entity.StartWalkAnim();

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
            
            if ( entity.onGround ){
                evadeCount = 1;
                if( jumpCooldown == 0 )
                    jumpCount = 1;
            }

            // entity.inertiaDirection = Vector3.Lerp( entity.inertiaDirection, entity.groundOrientation * entity.absoluteForward, 3f*Global.timeDelta);

            // // Gain Speed when moving downwards
            // float slopeMultiplier = Vector3.Dot( entity.gravityDown, entity.inertiaDirection ) * entity.moveSpeed;
            // float fallMultiplier = Mathf.Min(Mathf.Max(-entity.fallVelocity, 0f), 1f);

            // float newInertia = entity.onGround ? slopeMultiplier : fallMultiplier;
            // newInertia = Mathf.Sign(newInertia) == 1f  ?  Mathf.Pow(newInertia,1.5f)  :  newInertia*0.8f ;
            
            // float inertiaChangeFactor = newInertia > entity.inertiaMultiplier ? 12.5f : 5f;

            // entity.inertiaMultiplier = Mathf.Min(Mathf.MoveTowards(entity.inertiaMultiplier, newInertia, inertiaChangeFactor * Global.timeDelta), 25f);


            // Hover over water as long as the player is moving
            if ( waterHover ){
                entity._rb.velocity = entity._rb.velocity.NullifyInDirection(entity.gravityDown);
                entity.groundHit = waterHoverHit;
            }


            if (entity.moveDirection.magnitude > 0f && !entity.sliding){

                entity.absoluteForward = entity.moveDirection.normalized;
            
                entity.GroundedMove(entity.moveSpeed * Global.timeDelta * entity.absoluteForward, entity.onGround);
                
            }else if (entity.sliding){

                entity.absoluteForward = Vector3.Lerp( entity.absoluteForward, entity.moveDirection, Global.timeDelta);

            }

            // entity.GroundedMove(Global.timeDelta * 2.5f * entity.inertia );
            
            Vector3 finalRotation = entity?.currentWeapon.overrideRotation ?? entity.relativeForward;
            entity.RotateTowardsAbsolute(finalRotation, -entity.gravityDown);
        }

        private void LateUpdate(){
            waterHoverData.Update();
        }

        protected override void UpdateMoveSpeed(){
            float newSpeed = entity.moveInputData.currentValue.magnitude > 0f && !entity.sliding ? entity.data.baseSpeed : 0f;
            // if (entity.walkSpeed != Entity.WalkSpeed.run) 
            //     newSpeed *= entity.walkSpeed == Entity.WalkSpeed.sprint ? /* entity.data.sprintSpeed */1f : entity.data.slowSpeed;

            newSpeed *= speedMultiplier;

            float speedDelta = newSpeed > entity.moveSpeed ? 40f : 60f;
            
            entity.moveSpeed = Mathf.MoveTowards(entity.moveSpeed, newSpeed, speedDelta * Global.timeDelta);
        }

        public override void HandleInput(){

            entity.RawInputToGroundedMovement(out Vector3 camRight, out Vector3 camForward, out Vector3 groundDirection, out Vector3 groundDirection3D);
            entity.moveDirection = groundDirection;

            if (entity.crouchInputData.currentValue) entity.SetWalkSpeed(Entity.WalkSpeed.crouch);
            else if ((groundDirection.magnitude <= 0.25f || entity.walkInputData.currentValue) && entity.onGround) entity.SetWalkSpeed(Entity.WalkSpeed.walk);
            else entity.SetWalkSpeed(Entity.WalkSpeed.run);
            
            // entity.slidingData.SetVal( entity.evadeInputData.currentValue && !entity.evading && (entity.inertiaMultiplier > 2f | !entity.onGround) && !entity.inWater );
            
            if ( entity.jumpInputData.currentValue && jumpCount != 0 && entity.groundData.falseTimer < 0.4f && jumpCooldown == 0f )
                entity.Jump(jumpDirection);
        }

        private void OnEntityJump(Vector3 jumpDirection){
            jumpCount--;
            jumpCooldown = 0.4f;
        }
        private void OnEntityEvade(Vector3 evadeDirection){
            evadeCount--;
            // entity.inertiaMultiplier = Mathf.Max( 6.5f, entity.inertiaMultiplier);
        }
    }
}