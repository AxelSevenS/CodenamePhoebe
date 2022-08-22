using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SeleneGame.Core;
using SevenGame.Utility;

namespace SeleneGame.States {
    
    public class VehicleState : State{

        public override StateType stateType => StateType.groundState;
        protected override Vector3 GetCameraPosition() => new Vector3(0.3f, 0.5f, -6.5f);

        // public override bool masked => false;

        private float accelerationLinger = 0f;
        private Vector3 finalDirection = Vector3.zero;
        private Vector3 inputDirection;
        

        public override void OnEnter(Entity entity){
            base.OnEnter(entity);

            entity.onJump += OnEntityJump;
        }
        public override void OnExit(){

            entity.onJump -= OnEntityJump;
        }

        public override void StateUpdate(){

            // if (entity.onGround.started)
                // entity.StartWalkAnim();

        }

        public override void StateFixedUpdate(){

            entity.JumpGravity(entity.GravityMultiplier(), entity.gravityDown, entity.controller.jumpInput);
            
            if ( entity.onGround ){
                if( entity.jumpCooldownTimer.isDone )
                    entity.jumpCount = 1;
            }

            entity.absoluteForward = Vector3.Slerp(entity.absoluteForward, inputDirection, GameUtility.timeDelta * 3f).normalized;
            entity.moveDirection.SetVal(entity.absoluteForward);

            entity.GroundedMove( entity.moveSpeed * GameUtility.timeDelta * entity.moveDirection, false );

            bool terrainFlatEnough = Vector3.Dot(entity.groundHit.normal, -entity.gravityDown) > 0.75f;

            Vector3 groundUp = terrainFlatEnough ? entity.groundHit.normal : -entity.gravityDown;
            Vector3 rightDir = Vector3.Cross(entity.absoluteForward, groundUp);
            Vector3 finalUp = (groundUp*4f + (Vector3.Dot( inputDirection, rightDir ) * rightDir)).normalized;
            // finalUp = entity.onGround ? finalUp : (finalUp + finalUp + finalUp - entity.transform.forward).normalized;

            entity.RotateTowardsAbsolute(entity.absoluteForward, finalUp);



            // // When the Entity is sliding
            // if (entity.sliding)
            //     entity.rigidbody.velocity += entity.groundOrientation * entity.evadeDirection *entity.character.baseSpeed * entity.inertiaMultiplier * GameUtility.timeDelta;


        }


        public override void HandleInput(){

            entity.controller.RawInputToGroundedMovement(out _, out Vector3 groundedMovement);

            if (groundedMovement.sqrMagnitude != 0f)
                inputDirection = groundedMovement.normalized;

            float newLinger = groundedMovement.magnitude;
            accelerationLinger = Mathf.Lerp(accelerationLinger, newLinger, GameUtility.timeDelta * (newLinger > accelerationLinger ? 3f : 2f) );

            // entity.sliding.SetVal(entity.controller.evadeInput && entity.onGround);
            
            // Jump if the Jump key is pressed.
            if ( entity.controller.jumpInput.started && entity.jumpCount != 0 && entity.onGround.falseTimer <= 0.4f )
                entity.Jump( -entity.gravityDown );
            
            float newSpeed = Vector3.Dot(entity.moveDirection, inputDirection) * accelerationLinger * entity.character.baseSpeed;

            float speedDelta = newSpeed > entity.moveSpeed ? 1f : 0.65f;
            entity.moveSpeed = Mathf.MoveTowards(entity.moveSpeed, newSpeed, speedDelta * entity.character.acceleration * GameUtility.timeDelta);
        }

        private void OnEntityJump(Vector3 jumpDirection){
            entity.jumpCount--;
        }
    }
}
