using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SeleneGame.Core.UI;

using SevenGame.Utility;

namespace SeleneGame.Core {
    
    [System.Serializable]
    public partial class GroundedBehaviour : EntityBehaviour {


        public Entity.MovementSpeed movementSpeed = Entity.MovementSpeed.Normal;

        private Vector3 rotationForward;
        private Vector3Data moveDirection;
        public float moveSpeed;




        public override float gravityMultiplier => 1f;
        // public override Vector3 cameraPosition {
        //     get {
        //         float dynamicCameraDistance = moveSpeed;
        //         if (entity.character.baseSpeed > 0f)
        //             dynamicCameraDistance /= entity.character.baseSpeed;

        //         return base.cameraPosition - new Vector3(0, 0, dynamicCameraDistance);
        //     }
        // }
        public override CameraController.CameraType cameraType => CameraController.CameraType.ThirdPersonGrounded;


        protected override Vector3 jumpDirection => base.jumpDirection;

        protected override Vector3 evadeDirection => entity.isIdle ? -entity.absoluteForward : base.evadeDirection;

        protected override bool canParry => base.canParry;


        public override Vector3 direction => moveDirection;
        public override float speed => moveSpeed;



        public GroundedBehaviour(Entity entity, EntityBehaviour previousBehaviour) : base(entity, previousBehaviour) {

            _evadeBehaviour = new GroundedEvadeBehaviour(entity);
            _jumpBehaviour = new GroundedJumpBehaviour(entity);

            IdleAnimation();

            if (previousBehaviour == null) return;

            Move(previousBehaviour.direction);
            moveSpeed = previousBehaviour.speed;
        }
        


        protected internal override void HandleInput(PlayerEntityController controller){

            base.HandleInput(controller);

            controller.RawInputToGroundedMovement(out Quaternion cameraRotation, out Vector3 groundedMovement);

            if (groundedMovement.sqrMagnitude <= 0f)
                SetSpeed(Entity.MovementSpeed.Idle);
            else if ( DialogueController.current.Enabled || ((groundedMovement.sqrMagnitude <= 0.25 || controller.walkInput) && entity.onGround) ) 
                SetSpeed(Entity.MovementSpeed.Slow);
            else if ( controller.evadeInput )
                SetSpeed(Entity.MovementSpeed.Fast);
            else
                SetSpeed(Entity.MovementSpeed.Normal);
                

            Move(groundedMovement);

            if ( entity is ArmedEntity armedEntity )
                armedEntity.weapons.HandleInput(controller);

            if ( controller.lightAttackInput.started )
                LightAttack();

            if ( controller.heavyAttackInput.started )
                HeavyAttack();

            if ( controller.evadeInput.Tapped() )
                Evade(evadeDirection);

            if ( KeyInputData.SimultaneousTap( controller.lightAttackInput, controller.heavyAttackInput ) )
                Parry();
                
            if ( controller.jumpInput )
                Jump();


            // if (controller.focusInput.trueTimer > Keybinds.HOLD_TIME){
            //     entity.gravityDown = Vector3.down;
            // }

        }


        protected internal override void Move(Vector3 direction) {
            this.moveDirection.SetVal(Vector3.ProjectOnPlane(direction, entity.gravityDown).normalized );
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
            
            if (entity.onGround) {
                if (speed == Entity.MovementSpeed.Idle) {

                    MovementStopAnimation();
                
                } else if ((int)speed > (int)movementSpeed) {

                    MovementStartAnimation(speed);

                }
            }
            movementSpeed = speed;
        }

        protected override void ParryAction() {
            base.ParryAction();

            // if ( entity is ArmedEntity armed ) {
            //     armed.Parry();
            // }
        }
        protected override void LightAttackAction() {
            base.LightAttackAction();
            if ( entity is ArmedEntity armed ) {
                armed.weapons.LightAttack();
            }
        }
        protected override void HeavyAttackAction() {
            base.HeavyAttackAction();
            if ( entity is ArmedEntity armed ) {
                armed.weapons.HeavyAttack();
            }
        }
        

        private bool WaterHoverCheck() {
            // foreach (RaycastHit collision in entity.rigidbody.SweepTestAll(entity.gravityDown, 0.2f, QueryTriggerInteraction.Ignore)) {

            //     if (collision.collider.gameObject.layer == Global.WaterMask) {

            //         entity.groundHit = collision;
            //         entity.onGround.SetVal(true);
            //         entity.rigidbody.velocity = entity.rigidbody.velocity.NullifyInDirection(entity.gravityDown);
            //         return true;

            //     }
            // }

            if ( entity.character.model.ColliderCast( Vector3.zero, entity.gravityDown, out RaycastHit hit, out _, 0.15f, Global.WaterMask ) ) {
                entity.groundHit = hit;
                entity.onGround.SetVal(true);
                entity.rigidbody.velocity = entity.rigidbody.velocity.NullifyInDirection(entity.gravityDown);
                return true;
            }

            return false;
        }


        public override void Update(){

            base.Update();


            entity.transform.rotation = Quaternion.FromToRotation(entity.transform.up, -entity.gravityDown) * entity.transform.rotation;

            
            // Hover over water as long as the entity is moving
            bool canWaterHover = entity.weightCategory == Entity.WeightCategory.Light && moveDirection.zeroTimer < 0.6f;
            bool waterHover = canWaterHover && WaterHoverCheck();


            bool canSwim = !waterHover && entity.weightCategory != Entity.WeightCategory.Heavy;
            float distanceToWaterSurface = entity.physicsComponent.totalWaterHeight - entity.transform.position.y;
            
            if ( entity.inWater && canSwim && distanceToWaterSurface > 0f ) {
                entity.SetState( new SwimmingBehaviourBuilder() );
            }


            if ( moveDirection.sqrMagnitude != 0f )
                entity.absoluteForward = Vector3.Lerp( entity.absoluteForward, moveDirection, 100f * GameUtility.timeDelta);

                
            if ( entity.onGround ) {
                entity.rigidbody.velocity *= 0.995f;
            }


            if ( !_evadeBehaviour.state ){
                rotationForward = entity.absoluteForward;
            } else if ( moveDirection.sqrMagnitude != 0f ) {
                _evadeBehaviour.currentDirection = Vector3.Lerp(_evadeBehaviour.currentDirection, entity.absoluteForward, _evadeBehaviour.Time * _evadeBehaviour.Time);
            }
            
            entity.character.model.RotateTowards(rotationForward, -entity.gravityDown);


            float newSpeed = moveDirection.sqrMagnitude == 0f ? 0f : entity.character.data.baseSpeed;
            if (movementSpeed != Entity.MovementSpeed.Normal) 
                newSpeed *= movementSpeed == Entity.MovementSpeed.Fast ? entity.character.data.sprintMultiplier : entity.character.data.slowMultiplier;

            // Acceleration is quicker than Deceleration 
            float speedDelta = newSpeed > moveSpeed ? 1f : 0.45f;
            moveSpeed = Mathf.MoveTowards(moveSpeed, newSpeed, speedDelta * entity.character.data.acceleration * GameUtility.timeDelta);
            
            // Evade movement restricts the Walking movement.
            moveSpeed *= Mathf.Max(1 - _evadeBehaviour.Speed, 0.05f);

            entity.Displace(entity.absoluteForward * moveSpeed);

            Animation();
            
        }

    }
}