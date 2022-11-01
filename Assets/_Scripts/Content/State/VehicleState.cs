using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SeleneGame.Core;
using SevenGame.Utility;

namespace SeleneGame.States {
    
    public class VehicleState : State {

        private float _gravityMultiplier = 1f;

        private float moveSpeed;
        private float accelerationLinger = 0f;
        private Vector3 finalDirection = Vector3.zero;
        private Vector3 inputDirection;

        [SerializeField] private int jumpCount;



        public override float gravityMultiplier => _gravityMultiplier;

        public override bool canJump => base.canJump && jumpCount > 0;

        public override Vector3 cameraPosition => new Vector3(0.3f, 0.5f, -6.5f);
        


        public override void OnEnter(Entity entity){
            base.OnEnter(entity);

            entity.onJump += OnJump;
        }
        public override void OnExit(){

            entity.onJump -= OnJump;
        }


        private void OnJump(Vector3 jumpDirection){
            jumpCount--;
        }

        public override void HandleInput(EntityController controller){

            controller.RawInputToGroundedMovement(out _, out Vector3 groundedMovement);

            if (groundedMovement.sqrMagnitude != 0f)
                inputDirection = groundedMovement.normalized;

            float newLinger = groundedMovement.magnitude;
            accelerationLinger = Mathf.Lerp(accelerationLinger, newLinger, GameUtility.timeDelta * (newLinger > accelerationLinger ? 3f : 2f) );
            
            float newSpeed = Vector3.Dot(entity.absoluteForward, inputDirection) * accelerationLinger * entity.character.baseSpeed;
            float speedDelta = newSpeed > moveSpeed ? 1f : 0.65f;
            moveSpeed = Mathf.MoveTowards(moveSpeed, newSpeed, speedDelta * entity.character.acceleration * GameUtility.timeDelta);


            entity.absoluteForward = Vector3.Slerp(entity.absoluteForward, inputDirection, GameUtility.timeDelta * 3f).normalized;
            


            // If Jump input is pressed, slow down the fall.
            _gravityMultiplier = controller.jumpInput ? 0.75f : 1f;
        }

        public override void StateUpdate(){

            bool terrainFlatEnough = Vector3.Dot(entity.groundHit.normal, -entity.gravityDown) > 0.75f;

            Vector3 groundUp = terrainFlatEnough ? entity.groundHit.normal : -entity.gravityDown;
            Vector3 rightDir = Vector3.Cross(entity.absoluteForward, groundUp);
            Vector3 finalUp = (groundUp*4f + (Vector3.Dot( inputDirection, rightDir ) * rightDir)).normalized;


            entity.RotateTowardsAbsolute(entity.absoluteForward, finalUp);


            if ( entity.onGround )
                jumpCount = 1;


            // if (entity.onGround.started)
                // entity.StartWalkAnim();

        }

        public override void StateFixedUpdate(){


            entity.Move( moveSpeed * entity.absoluteForward );



            // // When the Entity is sliding
            // if (entity.sliding)
            //     entity.rigidbody.velocity += entity.groundOrientation * entity.evadeDirection *entity.character.baseSpeed * entity.inertiaMultiplier * GameUtility.timeDelta;


        }
    }
}
