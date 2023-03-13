using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SeleneGame.Core;
using SevenGame.Utility;

namespace SeleneGame.Content {
    
    [System.Serializable]
    public class VehicleState : State {

        private float _gravityMultiplier = 1f;

        private float moveSpeed;
        private Vector3 inputDirection;



        public override float gravityMultiplier => _gravityMultiplier;
        public override CameraController.CameraType cameraType => CameraController.CameraType.ThirdPersonGrounded;


        protected override Vector3 jumpDirection => base.jumpDirection;

        protected override Vector3 evadeDirection => base.evadeDirection;

        protected override bool canParry => base.canParry;
        


        public override void Transition(Vector3 direction, float speed) {
            Move(direction);
            moveSpeed = speed;
        }
        public override void GetTransitionData(out Vector3 direction, out float speed) {
            direction = inputDirection;
            speed = moveSpeed;
        }

        protected override void HandleInput(PlayerEntityController controller){

            base.HandleInput(controller);

            controller.RawInputToGroundedMovement(out _, out Vector3 groundedMovement);

            Move(groundedMovement);

            if (controller.jumpInput)
                Jump();
            
            // If Jump input is pressed, slow down the fall.
            _gravityMultiplier = controller.jumpInput ? 0.75f : 1f;
        }


        protected override void Move(Vector3 direction) {
                
            inputDirection = direction;

        }
        protected override void Jump() {
            base.Jump();
        }
        protected override void Evade(Vector3 direction) {
            base.Evade(direction);
        }
        protected override void Parry() {
            base.Parry();
        }
        protected override void LightAttack() {
            base.LightAttack();
        }
        protected override void HeavyAttack() {
            base.HeavyAttack();
        }
        protected override void SetSpeed(Entity.MovementSpeed speed) {
            
        }


        protected override void ParryAction() {
            base.ParryAction();
        }
        protected override void LightAttackAction() {
            base.LightAttackAction();
        }
        protected override void HeavyAttackAction() {
            base.HeavyAttackAction();
        }


        protected override void Awake(){
            base.Awake();
            _evadeBehaviour = gameObject.AddComponent<GroundedEvadeBehaviour>();
            _jumpBehaviour = gameObject.AddComponent<GroundedJumpBehaviour>();
        }

        protected override void OnDestroy(){
            base.OnDestroy();
        }

        private void Update(){

            
            float newSpeed = Vector3.Dot(entity.absoluteForward, inputDirection) * entity.character.data.baseSpeed;
            float speedDelta = newSpeed > moveSpeed ? 1f : 0.25f;
            moveSpeed = Mathf.MoveTowards(moveSpeed, newSpeed, speedDelta * entity.character.data.acceleration * GameUtility.timeDelta);

            if (inputDirection.sqrMagnitude != 0f)
                entity.absoluteForward = Vector3.Slerp(entity.absoluteForward, inputDirection, GameUtility.timeDelta * 3f).normalized;


            inputDirection = Vector3.zero;


            if ( entity.onGround ) {
                entity.rigidbody.velocity *= 0.995f;
            }

        }

        private void FixedUpdate() {

            entity.Displace( moveSpeed * entity.absoluteForward );

            
            float groundFlatness = Vector3.Dot(entity.groundHit.normal, -entity.gravityDown);

            Vector3 groundUp = groundFlatness > 0.5f ? entity.groundHit.normal : -entity.gravityDown;
            Vector3 rightDir = Vector3.Cross(entity.absoluteForward, groundUp).normalized;
            Vector3 finalUp = (groundUp*4f + (Vector3.Dot( inputDirection, rightDir ) * rightDir)).normalized;
            Vector3 finalForward = Vector3.Cross(finalUp, rightDir);

            entity.character.model.RotateTowards(finalForward, finalUp);


        }
    }
}
