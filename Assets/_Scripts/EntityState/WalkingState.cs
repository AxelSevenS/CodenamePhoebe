using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SeleneGame.Core;

namespace SeleneGame.States {
    
    public class WalkingState : State {

        protected override float GetSpeedMultiplier() => entity.currentWeapon.speedMultiplier;
        protected override Vector3 GetJumpDirection() => entity.currentWeapon.jumpDirection;
        protected override Vector3 GetCameraPosition() => entity.currentWeapon.cameraPosition - new Vector3(0,0,additionalCameraDistance);

        public override bool masked => Vector3.Dot ( entity.gravityDown, Vector3.down ) < 0.95f || entity.inWater || entity.currentWeapon.shifting;

        private bool crouching;

        private float additionalCameraDistance;

        BoolData waterHover = new BoolData();

        RaycastHit waterHoverHit;



        private void OnEnable(){

            entity.onJump += OnEntityJump;
            entity.onEvade += OnEntityEvade;
        }
        private void OnDisable(){

            entity.onJump -= OnEntityJump;
            entity.onEvade -= OnEntityEvade;
        }
        
        protected override void StateUpdate(){

            entity.jumpCooldown = Mathf.MoveTowards( entity.jumpCooldown, 0f, Global.timeDelta );
            
            if ( entity.onGround ){
                entity.evadeCount = 1;
                if( entity.jumpCooldown == 0 )
                    entity.jumpCount = 1;
            }

            waterHover.SetVal( entity.currentWeapon.weightModifier < 0.8f && entity.moveInput.zeroTimer < 0.6f && entity.ColliderCast( entity.gravityDown.normalized * 0.15f, out waterHoverHit, 0.15f, Global.WaterMask ) );
            entity.onGround.SetVal( entity.onGround || waterHover );

            if (entity.inWater && entity.currentWeapon.weightModifier <= 1f){
                entity.SetState("Swimming");
            }

            if (entity.evadeInput.started && entity.evadeCount != 0)
                entity.GroundedEvade( entity.moveDirection.magnitude == 0f ? -entity.absoluteForward : entity.moveDirection );

            if ( entity.onGround.started || (entity.evading.stopped && !entity.sliding) ) 
                entity.StartWalkAnim();

            additionalCameraDistance = entity.focusing ? -0.3f : 0f;
            if (entity.walkSpeed == Entity.WalkSpeed.sprint){
                additionalCameraDistance += 0.3f;
            }else if (entity.walkSpeed == Entity.WalkSpeed.walk){
                additionalCameraDistance += -0.2f;
            }

            if (entity.shiftInput.trueTimer > Player.current.holdDuration){
                entity.gravityDown = Vector3.down;
            }
        }

        protected override void StateFixedUpdate(){

            entity.SetRotation(-entity.gravityDown);

            entity.JumpGravity(entity.gravityForce, entity.gravityDown, entity.jumpInput);

            

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


            if (entity.moveDirection.magnitude > 0f){

                if ( !entity.evading )
                    entity.absoluteForward = Vector3.Lerp( entity.absoluteForward, entity.moveDirection.normalized, 20f * Global.timeDelta);

                if (entity.evadeTimer > entity.data.totalEvadeDuration - 0.15f)
                    entity.evadeDirection = entity.moveDirection.normalized;

            }

            // Move when evading
            if ( entity.EvadeUpdate(out float evadeCurve) )
                entity.GroundedMove( Global.timeDelta * evadeCurve * entity.data.evadeSpeed * entity.evadeDirection );

            entity.GroundedMove(entity.moveSpeed * ( 1 - evadeCurve ) * Global.timeDelta * entity.absoluteForward, entity.onGround);

            // entity.GroundedMove(Global.timeDelta * 2.5f * entity.inertia );
            
            Vector3 finalRotation = entity?.currentWeapon.overrideRotation ?? entity.relativeForward;
            entity.RotateTowardsAbsolute(finalRotation, -entity.gravityDown);
        }

        protected override void UpdateMoveSpeed(){
            float newSpeed = entity.moveDirection.magnitude > 0f ? entity.data.baseSpeed : 0f;
            // if (entity.walkSpeed != Entity.WalkSpeed.run) 
            //     newSpeed *= entity.walkSpeed == Entity.WalkSpeed.sprint ? /* entity.data.sprintSpeed */1f : entity.data.slowSpeed;

            newSpeed *= speedMultiplier;

            float speedDelta = newSpeed > entity.moveSpeed ? 40f : 80f;
            
            entity.moveSpeed = Mathf.MoveTowards(entity.moveSpeed, newSpeed, speedDelta * Global.timeDelta);
        }

        public override void HandleInput(){

            entity.RawInputToGroundedMovement(out Vector3 camRight, out Vector3 camForward, out Vector3 groundDirection, out Vector3 groundDirection3D);
            entity.moveDirection = groundDirection;

            if (entity.crouchInput) entity.SetWalkSpeed(Entity.WalkSpeed.crouch);
            else if ((groundDirection.magnitude <= 0.25f || entity.walkInput) && entity.onGround) entity.SetWalkSpeed(Entity.WalkSpeed.walk);
            else entity.SetWalkSpeed(Entity.WalkSpeed.run);
            
            // entity.slidingData.SetVal( entity.evadeInput && !entity.evading && (entity.inertiaMultiplier > 2f | !entity.onGround) && !entity.inWater );
            
            if ( entity.jumpInput && entity.jumpCount != 0 && entity.onGround.falseTimer < 0.4f && entity.jumpCooldown == 0f )
                entity.Jump(jumpDirection);
        }

        
        private void OnEntityJump(Vector3 jumpDirection){
            entity.jumpCount--;
        }
        private void OnEntityEvade(Vector3 evadeDirection){
            entity.evadeCount--;
        }

    }
}