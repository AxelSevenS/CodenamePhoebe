using System;

using UnityEngine;

using Animancer;

using SevenGame.Utility;
using SeleneGame.Core.UI;

namespace SeleneGame.Core {

    [Serializable]
    public sealed partial class GroundedMoveBehaviour : EntityMoveBehaviour {

        private Entity.MovementSpeed _movementSpeed = Entity.MovementSpeed.Normal;

        private Vector3 _rotationForward;
        private Vector3Data _moveDirection;
        private float _moveSpeed;

        public override Vector3 Direction => _moveDirection;
        public override float Speed => _moveSpeed;
        public override bool CanMove => true;



        protected internal override void HandleInput(Player controller) {
            base.HandleInput(controller);
            controller.RawInputToGroundedMovement(out _, out Vector3 groundedMovement);

            if (groundedMovement.sqrMagnitude <= 0f)
                SetSpeed(Entity.MovementSpeed.Idle);
            else if ( DialogueController.current.Enabled || ((groundedMovement.sqrMagnitude <= 0.25 || controller.walkInput) && Entity.onGround) ) 
                SetSpeed(Entity.MovementSpeed.Slow);
            else if ( controller.evadeInput )
                SetSpeed(Entity.MovementSpeed.Fast);
            else
                SetSpeed(Entity.MovementSpeed.Normal);
                

            Move(groundedMovement);

            // if ( controller.evadeInput.Tapped() )
            //     Evade();

            // // if ( KeyInputData.SimultaneousTap( controller.lightAttackInput, controller.heavyAttackInput ) ){
            // //     // parry
            // // }
                
            // if ( controller.jumpInput )
            //     Jump();
            
            // if ( Entity is ArmedEntity armedEntity) {
            //     armedEntity.Weapons.HandleInput(controller);
            // }
        }

        protected internal override void SetSpeed(Entity.MovementSpeed speed) {
            base.SetSpeed(speed);
            if (Entity.onGround && !Entity.Behaviour.EvadeBehaviour.state) {
                if (speed == Entity.MovementSpeed.Idle) {

                    MovementStopAnimation();
                
                } else if ((int)speed > (int)_movementSpeed) {

                    MovementStartAnimation(speed);

                }
            }
            _movementSpeed = speed;
        }

        protected internal override void Move(Vector3 direction) {
            base.Move(direction);
            _moveDirection.SetVal( Vector3.ClampMagnitude(direction, 1f) );
        }


        private void OnSetCharacter(Character character) {
            UpdateAnimations();
        }
        

        private void OnEnable() {
            Entity.OnSetCharacter += OnSetCharacter;
        }
        private void OnDisable() {
            Entity.OnSetCharacter -= OnSetCharacter;
        }
        private void Update() {
            // Update the Entity's up direction
            Entity.transform.rotation = Quaternion.FromToRotation(Entity.transform.up, -Entity.gravityDown) * Entity.transform.rotation;

            
            // Hover over water as long as the entity is moving
            bool canWaterHover = Entity.WeightCategory == Entity.EntityWeightCategory.Light && _moveDirection.zeroTimer < 0.6f;
            bool waterHover = canWaterHover && HandleWaterHover();

            // If the entity can do it, swim in water
            bool canSwim = !waterHover && Entity.WeightCategory != Entity.EntityWeightCategory.Heavy;
            float distanceToWaterSurface = Entity.PhysicsComponent.totalWaterHeight - Entity.CenterOfMass.y;
            
            if ( Entity.InWater && canSwim && distanceToWaterSurface > 0f ) {
                Entity.SetBehaviour( SwimmingBehaviour.Builder.Default );
            }

            if ( Entity.onGround ) {
                Entity.Inertia = Vector3.MoveTowards(Entity.Inertia, Vector3.zero, 10f * GameUtility.timeDelta);
            }


            if ( _moveDirection.sqrMagnitude != 0f ) {

                Vector3 groundedMovement = _moveDirection;
                if (Entity.groundDetected) {
                    groundedMovement = Quaternion.FromToRotation(-Entity.gravityDown, Entity.groundHit.normal) * groundedMovement;
                    Debug.DrawRay(Entity.transform.position, Entity.groundHit.normal, Color.red);
                    Debug.DrawRay(Entity.transform.position, groundedMovement, Color.blue);
                }

                Entity.AbsoluteForward = Vector3.Slerp( Entity.AbsoluteForward, groundedMovement, 100f * GameUtility.timeDelta);
            }


            EntityEvadeBehaviour currentEvadeBehaviour = Entity.Behaviour?.EvadeBehaviour;
            if ( currentEvadeBehaviour != null && currentEvadeBehaviour.state ) {
                currentEvadeBehaviour.currentDirection = Vector3.Lerp(currentEvadeBehaviour.currentDirection, Entity.AbsoluteForward, currentEvadeBehaviour.Time * currentEvadeBehaviour.Time);
            } else if ( _moveDirection.sqrMagnitude != 0f ) {
                _rotationForward = Vector3.ProjectOnPlane(Entity.AbsoluteForward, -Entity.gravityDown).normalized;
            }
            
            Entity.Character.Model.RotateTowards(_rotationForward, -Entity.gravityDown);


            float newSpeed = 0f;
            if (_moveDirection.sqrMagnitude != 0f) {
                switch (_movementSpeed) {
                    case Entity.MovementSpeed.Slow:
                        newSpeed = Entity.Character.Data.slowSpeed;
                        break;
                    case Entity.MovementSpeed.Normal:
                        newSpeed = Entity.Character.Data.baseSpeed;
                        break;
                    case Entity.MovementSpeed.Fast:
                        newSpeed = Entity.Character.Data.sprintSpeed;
                        break;
                }
            }

            // Acceleration is quicker than Deceleration 
            float speedDelta = newSpeed > _moveSpeed ? 1f : 0.45f;
            _moveSpeed = Mathf.MoveTowards(_moveSpeed, newSpeed, speedDelta * Entity.Character.Data.acceleration * GameUtility.timeDelta);
            
            // Evade movement restricts the Walking movement.
            _moveSpeed *= Mathf.Max(1 - currentEvadeBehaviour.Speed, 0.05f);

            Animation();

            Entity.DisplaceStep(Entity.AbsoluteForward * _moveSpeed);


            bool HandleWaterHover() {
                if ( Entity.Character.Model.ColliderCast( Vector3.zero, Entity.gravityDown, out RaycastHit hit, out _, 0.15f, CollisionUtils.WaterMask ) ) {
                    Entity.groundHit = hit;
                    Entity.onGround.SetVal(true);
                    return true;
                }

                return false;
            }
        }
        private void LateUpdate() {
            _moveDirection.SetVal(Vector3.zero);
        }



        public sealed class Builder : Builder<GroundedMoveBehaviour> {

            public static readonly Builder Default = new();

            public Builder() : base() {;}

            protected override void ConfigureBehaviour(Context context) {
                base.ConfigureBehaviour(context);
                context.Behaviour.UpdateAnimations();
                context.Behaviour.IdleAnimation();

                if (context.PreviousBehaviour == null) return;

                context.Behaviour.Move(context.PreviousBehaviour.Direction);
                context.Behaviour._moveSpeed = context.PreviousBehaviour.Speed;
            }
        }
    }
}
