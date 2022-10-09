using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SevenGame.Utility;

namespace SeleneGame.Core {
    
    public class WalkingState : State {

        private const float WATER_HOVER_WEIGHT_THRESHOLD = 10f;



        private float _gravityMultiplier = 1f;

        public WalkSpeed walkSpeed;

        private Vector3 rotationForward;
        private Vector3Data moveDirection;
        public float moveSpeed;

        private int jumpCount;



        public override float gravityMultiplier => _gravityMultiplier;
        
        public override bool canJump => base.canJump && jumpCount > 0;

        public override Vector3 evadeDirection => entity.isIdle ? -entity.absoluteForward : base.evadeDirection;


        public override Vector3 cameraPosition {
            get {
                return base.cameraPosition - new Vector3(0, 0, moveSpeed / entity.character.baseSpeed);
            }
        }



        public void SetWalkSpeed(WalkSpeed newSpeed) {
            if (walkSpeed != newSpeed){
                // if ((int)walkSpeed < (int)newSpeed && !currentAnimationState.Contains("Run")) SetAnimationState("RunningStart", 0.1f);
                walkSpeed = newSpeed;
            }
        }


        public override void OnEnter(Entity entity){
            base.OnEnter(entity);

            entity.onJump += OnJump;

        }
        public override void OnExit(){
            base.OnExit();

            entity.onJump -= OnJump;
        }


        private void OnJump(Vector3 jumpDirection) {
            jumpCount--;
        }
        
        public override void HandleInput(EntityController controller){


            controller.RawInputToGroundedMovement(out Quaternion cameraRotation, out Vector3 groundedMovement);

            if (controller.crouchInput)
                SetWalkSpeed(WalkSpeed.crouch);
            else if ( (groundedMovement.sqrMagnitude <= 0.25 || controller.walkInput) && entity.onGround ) 
                SetWalkSpeed(WalkSpeed.walk);
            else if ( controller.evadeInput )
                SetWalkSpeed(WalkSpeed.sprint);
            else 
                SetWalkSpeed(WalkSpeed.run);

            moveDirection.SetVal(groundedMovement.normalized);


            if (controller.shiftInput.trueTimer > ControlsManager.HOLD_TIME){
                entity.gravityDown = Vector3.down;
            }

            // If Jump input is pressed, slow down the fall.
            _gravityMultiplier = controller.jumpInput ? 0.75f : 1f;

        }
        
        public override void StateUpdate(){

            base.StateUpdate();

            entity.SetRotation(-entity.gravityDown);


            if ( entity.onGround )
                jumpCount = 1;

            if ( entity.onGround.started )
                entity.animator.CrossFade("Land", 0.1f);

            
            // Hover over water as long as the entity is moving
            bool canWaterHover = entity.weight < WATER_HOVER_WEIGHT_THRESHOLD && moveDirection.zeroTimer < 0.6f;

            if ( canWaterHover && entity.ColliderCast(Vector3.zero, entity.gravityDown * 0.2f, out RaycastHit waterHoverHit, 0.15f, Global.WaterMask) ) {
                entity.groundHit = waterHoverHit;
                entity.onGround.SetVal(true);
                entity.rigidbody.velocity = entity.rigidbody.velocity.NullifyInDirection(entity.gravityDown);
            } else if ( entity.inWater && entity.weight < SwimmingState.entityWeightSinkTreshold ) {
                entity.SetState( new SwimmingState() );
            }




            if ( moveDirection.sqrMagnitude != 0f )
                entity.absoluteForward = Vector3.Slerp( entity.absoluteForward, moveDirection, 100f * GameUtility.timeDelta);


            if ( entity is ArmedEntity armed ) {

                if ( armed.evading.started)
                    armed.evadeCount--;
                if ( armed.onGround )
                    armed.evadeCount = 1;
                    
                if ( !armed.evading )
                    rotationForward = entity.absoluteForward;

            } else {
                rotationForward = entity.absoluteForward;
            }
            
            entity.RotateTowardsAbsolute(rotationForward, -entity.gravityDown);
            
        }

        public override void StateFixedUpdate(){

            base.StateFixedUpdate();


            float newSpeed = moveDirection.sqrMagnitude == 0f ? 0f : entity.character.baseSpeed;
            if (walkSpeed != WalkSpeed.run) 
                newSpeed *= walkSpeed == WalkSpeed.sprint ? entity.character.sprintMultiplier : entity.character.slowMultiplier;

            float speedDelta = newSpeed > moveSpeed ? 1f : 0.65f;
            moveSpeed = Mathf.MoveTowards(moveSpeed, newSpeed, speedDelta * entity.character.acceleration * GameUtility.timeDelta);
            
            if (entity is ArmedEntity armed) {
                // Evade movement restricts the Walking movement.
                moveSpeed *= (1 - armed.evadeCurve);
            }

            entity.Move(entity.absoluteForward * moveSpeed);

        }



        

        public enum WalkSpeed {idle, crouch, walk, run, sprint};

    }
}