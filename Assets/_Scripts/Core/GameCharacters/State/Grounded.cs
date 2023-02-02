using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SeleneGame.Core.UI;

using SevenGame.Utility;

namespace SeleneGame.Core {
    
    [System.Serializable]
    public partial class Grounded : State {

        private const float WATER_HOVER_WEIGHT_THRESHOLD = 10f;


        public Entity.MovementSpeed movementSpeed = Entity.MovementSpeed.Normal;

        private Vector3 rotationForward;
        private Vector3Data moveDirection;
        public float moveSpeed;




        public override float gravityMultiplier => 1f;
        public override Vector3 cameraPosition {
            get {
                return base.cameraPosition - new Vector3(0, 0, moveSpeed / entity.character.baseSpeed);
            }
        }


        protected override Vector3 jumpDirection => base.jumpDirection;

        protected override Vector3 evadeDirection => entity.isIdle ? -entity.absoluteForward : base.evadeDirection;

        protected override bool canParry => base.canParry;


        
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

            if ( controller.evadeInput.started )
                Evade(evadeDirection);

            if ( KeyInputData.SimultaneousTap( controller.lightAttackInput, controller.heavyAttackInput ) )
                Parry();
                
            if ( controller.jumpInput )
                Jump();


            if (controller.shiftInput.trueTimer > Keybinds.HOLD_TIME){
                entity.gravityDown = Vector3.down;
            }

        }


        protected internal override void Move(Vector3 direction) {
            moveDirection.SetVal( (direction.normalized).NullifyInDirection(entity.gravityDown) );
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

            if ( entity is ArmedEntity armed ) {
                armed.Parry();
            }
        }
        protected override void LightAttackAction() {
            base.LightAttackAction();
        }
        protected override void HeavyAttackAction() {
            base.HeavyAttackAction();
        }



        protected internal override void Awake() {
            base.Awake();
            evadeBehaviour = gameObject.AddComponent<GroundedEvadeBehaviour>();
            jumpBehaviour = gameObject.AddComponent<GroundedJumpBehaviour>();
            IdleAnimation();
        }

        protected internal override void OnDestroy(){
            base.OnDestroy();
            // layer.DestroyStates();
        }

        private void Update(){


            entity.transform.rotation = Quaternion.FromToRotation(entity.transform.up, -entity.gravityDown) * entity.transform.rotation;

            
            // Hover over water as long as the entity is moving
            bool canWaterHover = entity.weight < WATER_HOVER_WEIGHT_THRESHOLD && moveDirection.zeroTimer < 0.6f;

            if ( canWaterHover && entity.ColliderCast(Vector3.zero, entity.gravityDown * 0.2f, out RaycastHit waterHoverHit, 0.15f, Global.WaterMask) ) {
                entity.groundHit = waterHoverHit;
                entity.onGround.SetVal(true);
                entity.rigidbody.velocity = entity.rigidbody.velocity.NullifyInDirection(entity.gravityDown);
            } else if ( entity.inWater && entity.weight < Swimming.entityWeightSinkTreshold ) {
                entity.SetState<Swimming>();
            }




            if ( moveDirection.sqrMagnitude != 0f )
                entity.absoluteForward = Vector3.Slerp( entity.absoluteForward, moveDirection, 100f * GameUtility.timeDelta);

                
            if ( entity.onGround ) {
                entity.rigidbody.velocity *= 0.995f;
            }


            if ( !evadeBehaviour.state ){
                rotationForward = entity.absoluteForward;
            } else if ( moveDirection.sqrMagnitude != 0f ) {
                evadeBehaviour.currentDirection = Vector3.Slerp(evadeBehaviour.currentDirection, entity.absoluteForward, evadeBehaviour.Time * evadeBehaviour.Time);
            }
            
            entity.RotateModelTowards(rotationForward, -entity.gravityDown);
            
        }

        private void FixedUpdate(){


            float newSpeed = moveDirection.sqrMagnitude == 0f ? 0f : entity.character.baseSpeed;
            if (movementSpeed != Entity.MovementSpeed.Normal) 
                newSpeed *= movementSpeed == Entity.MovementSpeed.Fast ? entity.character.sprintMultiplier : entity.character.slowMultiplier;

            // Acceleration is quicker than Deceleration 
            float speedDelta = newSpeed > moveSpeed ? 1f : 0.65f;
            moveSpeed = Mathf.MoveTowards(moveSpeed, newSpeed, speedDelta * entity.character.acceleration * GameUtility.timeDelta);
            
            // Evade movement restricts the Walking movement.
            moveSpeed *= Mathf.Max(1 - evadeBehaviour.Speed, 0.05f);

            entity.Displace(entity.absoluteForward * moveSpeed);

        }

    }
}