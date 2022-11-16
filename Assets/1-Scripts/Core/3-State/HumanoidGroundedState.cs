using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SevenGame.Utility;

namespace SeleneGame.Core {
    
    public class HumanoidGroundedState : HumanoidState {

        private const float WATER_HOVER_WEIGHT_THRESHOLD = 10f;



        private float _gravityMultiplier = 1f;

        public Entity.MovementSpeed movementSpeed = Entity.MovementSpeed.Normal;

        private Vector3 rotationForward;
        private Vector3Data moveDirection;
        public float moveSpeed;

        private int jumpCount = 1;
        [SerializeField] private TimeUntil jumpCooldownTimer;

        private int evadeCount = 1;



        public override float gravityMultiplier => _gravityMultiplier;
        public override Vector3 cameraPosition {
            get {
                return base.cameraPosition - new Vector3(0, 0, moveSpeed / entity.character.baseSpeed);
            }
        }


        protected override Vector3 jumpDirection => base.jumpDirection;
        protected override bool canJump => base.canJump && jumpCount > 0 && jumpCooldownTimer.isDone;

        protected override Vector3 evadeDirection => entity.isIdle ? -entity.absoluteForward : base.evadeDirection;
        protected override bool canEvade => base.canEvade && evadeCount > 0;

        protected override bool canParry => base.canParry;



        protected internal override void OnEnter(Entity entity){
            base.OnEnter(entity);

        }
        protected internal override void OnExit(){
            base.OnExit();
        }


        // public void SetWalkSpeed(WalkSpeed newSpeed) {
        //     if (walkSpeed != newSpeed){
        //         // if ((int)walkSpeed < (int)newSpeed && !currentAnimationState.Contains("Run")) SetAnimationState("RunningStart", 0.1f);
        //         walkSpeed = newSpeed;
        //     }
        // }
        
        protected internal override void HandleInput(EntityController controller){

            base.HandleInput(controller);

            controller.RawInputToGroundedMovement(out Quaternion cameraRotation, out Vector3 groundedMovement);

            if ( (groundedMovement.sqrMagnitude <= 0.25 || controller.walkInput) && entity.onGround ) 
                SetSpeed(Entity.MovementSpeed.Slow);
            else if ( controller.evadeInput )
                SetSpeed(Entity.MovementSpeed.Fast);
            else if ( movementSpeed != Entity.MovementSpeed.Fast )
                SetSpeed(Entity.MovementSpeed.Normal);

            Move(groundedMovement);

            
            if ( controller.jumpInput )
                Jump();


            if (controller.shiftInput.trueTimer > ControlsManager.HOLD_TIME){
                entity.gravityDown = Vector3.down;
            }

            // If Jump input is pressed, slow down the fall.
            _gravityMultiplier = controller.jumpInput ? 0.75f : 1f;

        }


        protected internal override void Move(Vector3 direction) {
            moveDirection.SetVal( (direction.normalized).NullifyInDirection(entity.gravityDown) );
            if (direction.sqrMagnitude == 0f)
                SetSpeed(Entity.MovementSpeed.Normal);

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
            movementSpeed = speed;
        }


        protected override void JumpAction(Vector3 direction) {
            base.JumpAction(direction);
            jumpCooldownTimer.SetDuration( 0.4f );
            jumpCount--;
        }
        protected override void EvadeAction(Vector3 direction) {
            base.EvadeAction(direction);
            evadeCount--;
        }
        protected override void ParryAction() {
            base.ParryAction();
            if (entity is ArmedEntity armed) {
                armed.Parry();
            }
        }
        protected override void LightAttackAction() {
            base.LightAttackAction();
        }
        protected override void HeavyAttackAction() {
            base.HeavyAttackAction();
        }


        
        protected internal override void StateUpdate(){
            base.StateUpdate();

            // entity.SetUp(-entity.gravityDown);
            entity.transform.rotation = Quaternion.FromToRotation(entity.transform.up, -entity.gravityDown) * entity.transform.rotation;


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


            if ( evading.started)
                evadeCount--;
                
            if ( entity.onGround ) {
                evadeCount = 1;
                entity.rigidbody.velocity *= 0.995f;
            }
                
            if ( !evading )
                rotationForward = entity.absoluteForward;
            
            entity.RotateModelTowards(rotationForward, -entity.gravityDown);
            
        }

        protected internal override void StateFixedUpdate(){

            base.StateFixedUpdate();


            float newSpeed = moveDirection.sqrMagnitude == 0f ? 0f : entity.character.baseSpeed;
            if (movementSpeed != Entity.MovementSpeed.Normal) 
                newSpeed *= movementSpeed == Entity.MovementSpeed.Fast ? entity.character.sprintMultiplier : entity.character.slowMultiplier;

            // Acceleration is quicker than Deceleration 
            float speedDelta = newSpeed > moveSpeed ? 1f : 0.65f;
            moveSpeed = Mathf.MoveTowards(moveSpeed, newSpeed, speedDelta * entity.character.acceleration * GameUtility.timeDelta);
            
            // Evade movement restricts the Walking movement.
            moveSpeed *= Mathf.Max(1 - evadeCurve, 0.05f);

            entity.Displace(entity.absoluteForward * moveSpeed);

        }

    }
}