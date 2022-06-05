using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SeleneGame.Core;
using SeleneGame.Entities;

namespace SeleneGame.States {
    
    public class WalkingState : State {

        // protected override float GetSpeedMultiplier() => 1f/* entity.currentWeapon.speedMultiplier */;
        protected override Vector3 GetJumpDirection(){
            if (entity is GravityShifterEntity gravityShifter)
                return gravityShifter.currentWeapon.jumpDirection;
            return base.GetJumpDirection();
        }
        protected override Vector3 GetCameraPosition() {
            if (entity is ArmedEntity armed) {
                return armed.currentWeapon.cameraPosition - new Vector3(0,0,additionalCameraDistance);
            }
            return Player.current.defaultCameraPosition - new Vector3(0,0,additionalCameraDistance);
        }

        // public override bool masked => Vector3.Dot ( entity.gravityDown, Vector3.down ) < 0.95f || entity.inWater || entity.currentWeapon.shifting;

        private bool crouching;

        private float additionalCameraDistance;

        BoolData waterHover = new BoolData();

        RaycastHit waterHoverHit;



        private void OnEnable(){

            entity.onJump += OnEntityJump;
        }
        private void OnDisable(){

            entity.onJump -= OnEntityJump;
        }
        
        protected override void StateUpdate(){

            entity.jumpCooldown = Mathf.MoveTowards( entity.jumpCooldown, 0f, Global.timeDelta );
            
            if ( entity.onGround && entity.jumpCooldown == 0f )
                entity.jumpCount = 1;

            // waterHover.SetVal( entity.CanWaterHover() && entity.ColliderCast( Vector3.zero, entity.gravityDown.normalized * 0.15f, out waterHoverHit, 0.15f, Global.WaterMask ) );
            // entity.onGround.SetVal( entity.onGround || waterHover );

            if (entity.inWater && !entity.CanSink() && !entity.CanWaterHover()){
                entity.SetState("Swimming");
            }

            if ( entity.onGround.started || (entity.evading.stopped && !entity.sliding)) 
                entity.StartWalkAnim();

            if ( entity.onGround )
                entity.evadeCount = 1;

            if ( entity.evading.started)
                entity.evadeCount--;
                

            if ( entity.evadeInput.started && entity.evadeCount != 0 )
                entity.GroundedEvade( entity.moveDirection.magnitude == 0f ? -entity.absoluteForward : entity.moveDirection );


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

        protected override void StateFixedUpdate(){

            entity.SetRotation(-entity.gravityDown);

            entity.JumpGravity(entity.GravityMultiplier(), entity.gravityDown, entity.jumpInput);

            

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

            if (entity.moveDirection.magnitude > 0f && entity.CanTurn() )
                entity.absoluteForward = Vector3.Lerp( entity.absoluteForward, entity.moveDirection.normalized, 100f * Global.timeDelta);

            // Move when evading
            if ( entity.EvadeUpdate(out _, out float evadeSpeed) )
                entity.GroundedMove( Global.timeDelta * evadeSpeed * entity.data.evadeSpeed * entity.evadeDirection );

            entity.GroundedMove(entity.moveSpeed * (1 - evadeSpeed) * Global.timeDelta * entity.absoluteForward, entity.onGround);

            // entity.GroundedMove(Global.timeDelta * 2.5f * entity.inertia );
            
            entity.RotateTowardsAbsolute(entity.absoluteForward, -entity.gravityDown);
        }

        public override float UpdateMoveSpeed(){
            float newSpeed = entity.moveDirection.magnitude > 0f ? entity.data.baseSpeed : 0f;
            // newSpeed = entity.evading ? newSpeed * 0.15f : newSpeed;
            if (entity.walkSpeed != Entity.WalkSpeed.run) 
                newSpeed *= entity.walkSpeed == Entity.WalkSpeed.sprint ? entity.data.sprintSpeed : entity.data.slowSpeed;

            return newSpeed;
        }

        public override void HandleInput(){

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
            
            if ( entity.jumpInput && entity.jumpCount != 0 && entity.onGround.falseTimer < 0.4f && entity.jumpCooldown == 0f )
                entity.Jump(jumpDirection);
        }

        
        private void OnEntityJump(Vector3 jumpDirection){
            entity.jumpCount--;
        }

    }
}