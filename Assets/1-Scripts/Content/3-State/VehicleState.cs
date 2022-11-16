using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SeleneGame.Core;
using SevenGame.Utility;

namespace SeleneGame.Content {
    
    public class VehicleState : State {

        private float _gravityMultiplier = 1f;

        private float moveSpeed;
        private float accelerationLinger = 0f;
        private Vector3 inputDirection;

        [SerializeField] private int jumpCount;
        [SerializeField] private TimeUntil jumpCooldownTimer;



        public override float gravityMultiplier => _gravityMultiplier;
        public override Vector3 cameraPosition => new Vector3(0.3f, 0.5f, -6.5f);


        protected override Vector3 jumpDirection => base.jumpDirection;
        protected override bool canJump => base.canJump && jumpCount > 0 && jumpCooldownTimer.isDone;

        protected override Vector3 evadeDirection => base.evadeDirection;
        protected override bool canEvade => base.canEvade;

        protected override bool canParry => base.canParry;
        


        protected override void OnEnter(Entity entity){
            base.OnEnter(entity);
        }
        protected override void OnExit(){
        }


        protected override void HandleInput(EntityController controller){

            controller.RawInputToGroundedMovement(out _, out Vector3 groundedMovement);

            Move(groundedMovement);

            if (controller.jumpInput) 
                Jump();
            
            // If Jump input is pressed, slow down the fall.
            _gravityMultiplier = controller.jumpInput ? 0.75f : 1f;
        }


        protected override void Move(Vector3 direction) {
                
            float newLinger = direction.magnitude;
            accelerationLinger = Mathf.Lerp(accelerationLinger, newLinger, (newLinger > accelerationLinger ? 3f : 2f) * GameUtility.timeDelta );

            if (newLinger != 0f)
                inputDirection = direction / newLinger;

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


        protected override void JumpAction(Vector3 direction) {
            base.JumpAction(direction);
            jumpCooldownTimer.SetDuration(0.25f);
            jumpCount--;
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



        protected override void StateUpdate(){

            bool terrainFlatEnough = Vector3.Dot(entity.groundHit.normal, -entity.gravityDown) > 0.75f;
            
            float newSpeed = Vector3.Dot(entity.absoluteForward, inputDirection) * accelerationLinger * entity.character.baseSpeed;
            float speedDelta = newSpeed > moveSpeed ? 1f : 0.65f;
            moveSpeed = Mathf.MoveTowards(moveSpeed, newSpeed, speedDelta * entity.character.acceleration * GameUtility.timeDelta);

            if (inputDirection.sqrMagnitude != 0f)
                entity.absoluteForward = Vector3.Slerp(entity.absoluteForward, inputDirection, GameUtility.timeDelta * 3f).normalized;


            Vector3 groundUp = terrainFlatEnough ? entity.groundHit.normal : -entity.gravityDown;
            Vector3 rightDir = Vector3.Cross(entity.absoluteForward, groundUp);
            Vector3 finalUp = (groundUp*4f + (Vector3.Dot( inputDirection, rightDir ) * rightDir)).normalized;

            entity.RotateModelTowards(entity.absoluteForward, finalUp);

            inputDirection = Vector3.zero;


            if ( entity.onGround ) {
                jumpCount = 1;
                entity.rigidbody.velocity *= 0.995f;
            }


            // if (entity.onGround.started)
                // entity.StartWalkAnim();

        }

        protected override void StateFixedUpdate(){


            entity.Displace( moveSpeed * entity.absoluteForward );



            // // When the Entity is sliding
            // if (entity.sliding)
            //     entity.rigidbody.velocity += entity.groundOrientation * entity.evadeDirection *entity.character.baseSpeed * entity.inertiaMultiplier * GameUtility.timeDelta;


        }
    }
}
