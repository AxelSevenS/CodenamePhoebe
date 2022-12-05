using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SevenGame.Utility;

namespace SeleneGame.Core {
    
    [System.Serializable]
    public sealed class Swimming : State {

        public const float entityWeightSinkTreshold = 16.25f;



        private Vector3 moveDirection;
        private float moveSpeed;



        public sealed override float gravityMultiplier => 0f; 
        public override Vector3 cameraPosition => Global.cameraDefaultPosition;


        protected override Vector3 jumpDirection => base.jumpDirection;
        // protected override bool canJump => entity.isOnWaterSurface;

        protected override Vector3 evadeDirection => base.evadeDirection;

        protected override bool canParry => base.canParry;




        protected internal override void OnEnter(Entity entity){
            base.OnEnter(entity);
            evadeBehaviour = new EvadeBehaviour(this);
            entity.gravityDown = Vector3.down;
        }

        protected internal override void OnExit(){
            base.OnExit();
        }



        protected internal override void HandleInput(PlayerEntityController controller){

            base.HandleInput(controller);
            
            if ( controller.evadeInput.started )
                Evade(evadeDirection);
            
            controller.RawInputToCameraRelativeMovement(out Quaternion cameraRotation, out Vector3 cameraRelativeMovement);
            float verticalInput = (controller.jumpInput ? 1f: 0f) - (controller.crouchInput ? 1f: 0f);

            Move( cameraRelativeMovement + (cameraRotation * Vector3.up * verticalInput) );

        }


        protected internal override void Move(Vector3 direction) {
            moveDirection = direction;
        }
        protected internal override void Jump() {
            base.Jump();
        }
        protected internal override void Evade(Vector3 direction) {
            base.Evade(direction);
        }
        protected internal override void Parry() {
            base.Parry();
        }
        protected internal override void LightAttack() {
            base.LightAttack();
        }
        protected internal override void HeavyAttack() {
            base.HeavyAttack();
        }
        protected internal override void SetSpeed(Entity.MovementSpeed speed) {
            
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



        protected internal override void StateUpdate() {

            base.StateUpdate();


            if ( !entity.inWater || entity.weight > entityWeightSinkTreshold ){
                entity.SetState(entity.defaultState);
            }

            if (moveDirection.sqrMagnitude != 0f){
                
                entity.absoluteForward = Vector3.Slerp( entity.absoluteForward, moveDirection, 100f * GameUtility.timeDelta);
                Vector3 newUp = Vector3.Cross(entity.absoluteForward, Vector3.Cross(entity.absoluteForward, entity.gravityDown));

                entity.RotateModelTowards(entity.absoluteForward, newUp);
            }

            float newSpeed = moveDirection.sqrMagnitude == 0f ? 0f : entity.character.baseSpeed;
            float speedDelta = newSpeed > moveSpeed ? 1f : 0.65f;
            moveSpeed = Mathf.MoveTowards(moveSpeed, newSpeed, speedDelta * entity.character.acceleration * GameUtility.timeDelta);

        }

        protected internal override void StateFixedUpdate() {

            base.StateFixedUpdate();


            // entity.SetUp(-entity.gravityDown);
            entity.transform.rotation = Quaternion.FromToRotation(entity.transform.up, -entity.gravityDown) * entity.transform.rotation;
            
            entity.Displace( moveDirection * moveSpeed );

            entity.rigidbody.velocity = Vector3.Dot(entity.rigidbody.velocity.normalized, entity.gravityDown) > 0f ? entity.rigidbody.velocity / 1.1f : entity.rigidbody.velocity;

        }
    }
}