using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SevenGame.Utility;

namespace SeleneGame.Core {
    public class FloatingMoveBehaviour : EntityMoveBehaviour {

        public const float swimTreshold = 1f;
        public const float surfaceTreshold = 2f;



        [SerializeField] private Vector3 _moveDirection;
        [SerializeField] private float _moveSpeed;


        private bool IsOnWaterSurface => Mathf.Abs(DistanceToWaterSurface) < surfaceTreshold;
        private float DistanceToWaterSurface => Entity.PhysicsComponent.totalWaterHeight - Entity.CenterOfMass.y;

        public override Vector3 Direction => _moveDirection;

        public override float Speed => _moveSpeed;

        public override bool CanMove => true;

        protected internal override void HandleInput(Player controller){

            base.HandleInput(controller);

            if (IsOnWaterSurface && controller.jumpInput.started) {
                JumpOutOfWater();
                return;
            }
            
            controller.RawInputToCameraRelativeMovement(out _, out Vector3 cameraRelativeMovement);
            float verticalInput = (controller.jumpInput ? 1f: 0f) - (controller.crouchInput ? 1f: 0f);

            Move( cameraRelativeMovement + (Vector3.up * verticalInput) );

        }

        protected internal override void Move(Vector3 direction) {
            _moveDirection = direction;
        }

        private void JumpOutOfWater() {
            if (Entity.Behaviour.GetType() != typeof(SwimmingBehaviour))
                return;

            Entity.transform.position = Entity.transform.position + (Vector3.up * Mathf.Max(DistanceToWaterSurface + swimTreshold/2f, 0f)); 

            Entity.ResetBehaviour();
            
            Entity.Behaviour.JumpBehaviour.Jump(Entity.Behaviour.JumpBehaviour.DefaultDirection * 1.1f);
        }
        private void OnEnable() {
            Entity.gravityDown = Vector3.down;
        }

        private void Update() {

            bool canSwim = Entity.WeightCategory != Entity.EntityWeightCategory.Heavy;
            if ( !Entity.InWater || !canSwim )
                Entity.ResetBehaviour();

            if (_moveDirection.sqrMagnitude != 0f){
                Entity.AbsoluteForward = Vector3.Lerp( Entity.AbsoluteForward, _moveDirection, 20f * GameUtility.timeDelta);
            }


            float newSpeed = _moveDirection.sqrMagnitude == 0f ? 0f : Entity.Character.Data.baseSpeed;
            float speedDelta = newSpeed > _moveSpeed ? 0.5f : 0.25f;
            _moveSpeed = Mathf.MoveTowards(_moveSpeed, newSpeed, speedDelta * Entity.Character.Data.acceleration * GameUtility.timeDelta);




            Vector3 newUp;
            Vector3 modelForward;
            if (IsOnWaterSurface) {                
                newUp = -Entity.gravityDown;
                modelForward = Vector3.Cross(newUp, Vector3.Cross(Entity.AbsoluteForward, newUp));
            } else {
                newUp = Vector3.Cross(Entity.AbsoluteForward, Vector3.Cross(Entity.AbsoluteForward, Entity.gravityDown));
                modelForward = Entity.AbsoluteForward;
            }

            Entity.transform.rotation = Quaternion.FromToRotation(Entity.transform.up, -Entity.gravityDown) * Entity.transform.rotation;

            Entity.Character.Model.RotateTowards(modelForward, newUp, 5f);
            


            float floatingDisplacement = IsOnWaterSurface ? DistanceToWaterSurface + swimTreshold/3f : 0f;
            Vector3 floatingDisplacementVector = Vector3.up * floatingDisplacement * 3f;

            Vector3 movement = Entity.AbsoluteForward * _moveSpeed;
            if (IsOnWaterSurface && !Entity.onGround) {
                EntityEvadeBehaviour currentEvadeBehaviour = Entity.Behaviour.EvadeBehaviour;
                if (currentEvadeBehaviour != null) {
                    currentEvadeBehaviour.currentDirection = currentEvadeBehaviour.currentDirection.NullifyInDirection(Vector3.up);
                }
                Entity.Inertia = Entity.Inertia.NullifyInDirection(Vector3.up);
                movement = movement.NullifyInDirection(Vector3.up);
            }

            Vector3 displacement = (floatingDisplacementVector + movement) * GameUtility.timeDelta;

            Entity.Displace( displacement, deltaTime: 1f );



            Entity.Inertia *= 0.95f;

        }

        public sealed class Builder : Builder<FloatingMoveBehaviour> {

            public static readonly Builder Default = new();

            protected override void ConfigureBehaviour(Context context) {
                base.ConfigureBehaviour(context);
                context.Entity.gravityDown = Vector3.down;

                if (context.PreviousBehaviour == null) return;

                context.Behaviour._moveDirection = context.PreviousBehaviour.Direction;
                context.Behaviour._moveSpeed = context.PreviousBehaviour.Speed;
            }
        }
    }
}
