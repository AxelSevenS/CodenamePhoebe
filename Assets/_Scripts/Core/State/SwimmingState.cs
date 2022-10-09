using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SevenGame.Utility;

namespace SeleneGame.Core {
    
    public class SwimmingState : NoGravityState {

        public const float entityWeightSinkTreshold = 16.25f;



        private Vector3 moveDirection;
        private float moveSpeed;


        public override Vector3 cameraPosition => Global.cameraDefaultPosition;



        public override void OnEnter(Entity entity){
            base.OnEnter(entity);

            entity.gravityDown = Vector3.down;
        }

        public override void OnExit(){
            base.OnExit();
        }

        public override void HandleInput(EntityController controller){
            
            controller.RawInputToCameraRelativeMovement(out Quaternion cameraRotation, out Vector3 cameraRelativeMovement);
            float verticalInput = (controller.jumpInput ? 1f: 0f) - (controller.crouchInput ? 1f: 0f);
            moveDirection = cameraRelativeMovement + (cameraRotation * Vector3.up * verticalInput);

            float newSpeed = moveDirection.sqrMagnitude == 0f ? 0f : entity.character.baseSpeed;
            float speedDelta = newSpeed > moveSpeed ? 1f : 0.65f;
            moveSpeed = Mathf.MoveTowards(moveSpeed, newSpeed, speedDelta * entity.character.acceleration * GameUtility.timeDelta);

        }

        public override void StateUpdate(){

            if ( !entity.inWater || entity.weight > entityWeightSinkTreshold ){
                entity.SetState(entity.defaultState);
            }

            if (moveDirection.sqrMagnitude != 0f){
                
                entity.absoluteForward = Vector3.Slerp( entity.absoluteForward, moveDirection, 100f * GameUtility.timeDelta);
                Vector3 newUp = Vector3.Cross(entity.absoluteForward, Vector3.Cross(entity.absoluteForward, entity.gravityDown));

                entity.RotateTowardsAbsolute(entity.absoluteForward, newUp);
            }


        }

        public override void StateFixedUpdate(){

            entity.SetRotation(-entity.gravityDown);
            
            entity.Move( moveDirection * moveSpeed );

            entity.rigidbody.velocity = Vector3.Dot(entity.rigidbody.velocity.normalized, entity.gravityDown) > 0f ? entity.rigidbody.velocity / 1.1f : entity.rigidbody.velocity;



        }
    }
}