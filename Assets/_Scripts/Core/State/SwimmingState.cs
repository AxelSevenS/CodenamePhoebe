using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SevenGame.Utility;

namespace SeleneGame.Core {
    
    public sealed class SwimmingState : HumanoidState {

        public const float entityWeightSinkTreshold = 16.25f;



        private Vector3 moveDirection;
        private float moveSpeed;



        public sealed override float gravityMultiplier => 0f; 
        public override Vector3 cameraPosition => Global.cameraDefaultPosition;


        protected override Vector3 jumpDirection => base.jumpDirection;
        protected override bool canJump => entity.isOnWaterSurface;

        protected override Vector3 evadeDirection => base.evadeDirection;
        protected override bool canEvade => base.canEvade;

        protected override bool canParry => base.canParry;




        protected internal override void OnEnter(Entity entity){
            base.OnEnter(entity);
            entity.gravityDown = Vector3.down;
        }

        protected internal override void OnExit(){
            base.OnExit();
        }



        public override void HandleInput(EntityController controller){
            base.HandleInput(controller);
            
            controller.RawInputToCameraRelativeMovement(out Quaternion cameraRotation, out Vector3 cameraRelativeMovement);
            float verticalInput = (controller.jumpInput ? 1f: 0f) - (controller.crouchInput ? 1f: 0f);
            Move( cameraRelativeMovement + (cameraRotation * Vector3.up * verticalInput) );

        }


        public override void Move(Vector3 direction) {
            moveDirection = direction;
        }
        public override void Jump() {
            base.Jump();
        }
        public override void Evade(Vector3 direction) {
            base.Evade(direction);
        }
        public override void Parry() {
            base.Parry();
        }
        public override void LightAttack() {
            base.LightAttack();
        }
        public override void HeavyAttack() {
            base.HeavyAttack();
        }
        public override void SetSpeed(MovementSpeed speed) {
            
        }


        protected override void JumpAction(Vector3 jumpDirection) {
            base.JumpAction(jumpDirection);
        }
        protected override void EvadeAction(Vector3 direction) {
            base.EvadeAction(direction);
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



        protected internal override void StateUpdate(){
            base.StateUpdate();

            if ( !entity.inWater || entity.weight > entityWeightSinkTreshold ){
                entity.SetState(entity.defaultState);
            }

            if (moveDirection.sqrMagnitude != 0f){
                
                entity.absoluteForward = Vector3.Slerp( entity.absoluteForward, moveDirection, 100f * GameUtility.timeDelta);
                Vector3 newUp = Vector3.Cross(entity.absoluteForward, Vector3.Cross(entity.absoluteForward, entity.gravityDown));

                entity.RotateTowardsAbsolute(entity.absoluteForward, newUp);
            }

            float newSpeed = moveDirection.sqrMagnitude == 0f ? 0f : entity.character.baseSpeed;
            float speedDelta = newSpeed > moveSpeed ? 1f : 0.65f;
            moveSpeed = Mathf.MoveTowards(moveSpeed, newSpeed, speedDelta * entity.character.acceleration * GameUtility.timeDelta);

        }

        protected internal override void StateFixedUpdate(){
            base.StateFixedUpdate();

            entity.SetUp(-entity.gravityDown);
            
            entity.Move( moveDirection * moveSpeed );

            entity.rigidbody.velocity = Vector3.Dot(entity.rigidbody.velocity.normalized, entity.gravityDown) > 0f ? entity.rigidbody.velocity / 1.1f : entity.rigidbody.velocity;

        }
    }
}