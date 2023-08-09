using System;

using UnityEngine;

using SevenGame.Utility;
using SeleneGame.Core;
using SeleneGame.Core.UI;

namespace SeleneGame.Content {

    [Serializable]
    public sealed partial class VehicleMoveBehaviour : EntityMoveBehaviour {

        private float _moveSpeed;
        private Vector3 _inputDirection;

        public override Vector3 Direction => _inputDirection;
        public override float Speed => _moveSpeed;
        public override bool CanMove => true;



        protected override void HandleInput(Player controller) {
            base.HandleInput(controller);

            controller.RawInputToGroundedMovement(out _, out Vector3 groundedMovement);
            Move(groundedMovement);
        }

        protected override void Move(Vector3 direction) {
            base.Move(direction);
            _inputDirection = direction;
        }
        

        private void Update() {
            
            float newSpeed = Vector3.Dot(Entity.AbsoluteForward, _inputDirection) * Entity.Character.Data.baseSpeed;
            float speedDelta = newSpeed > _moveSpeed ? 1f : 0.25f;
            _moveSpeed = Mathf.MoveTowards(_moveSpeed, newSpeed, speedDelta * Entity.Character.Data.acceleration * GameUtility.timeDelta);

            if (_inputDirection.sqrMagnitude != 0f)
                Entity.AbsoluteForward = Vector3.Slerp(Entity.AbsoluteForward, _inputDirection, GameUtility.timeDelta * 3f).normalized;

            Entity.Displace( _moveSpeed * Entity.AbsoluteForward );

            
            float groundFlatness = Vector3.Dot(Entity.groundHit.normal, -Entity.gravityDown);

            Vector3 groundUp = groundFlatness > 0.5f ? Entity.groundHit.normal : -Entity.gravityDown;
            Vector3 rightDir = Vector3.Cross(Entity.AbsoluteForward, groundUp).normalized;
            Vector3 finalUp = (groundUp*4f + (Vector3.Dot( _inputDirection, rightDir ) * rightDir)).normalized;
            Vector3 finalForward = Vector3.Cross(finalUp, rightDir);

            Entity.Character.Model.RotateTowards(finalForward, finalUp);

        }
        private void LateUpdate() {
            _inputDirection = Vector3.zero;
        }



        public sealed class Builder : Builder<VehicleMoveBehaviour> {

            public static readonly Builder Default = new();

            public Builder() : base() {;}

            protected override void ConfigureBehaviour(Context context) {
                base.ConfigureBehaviour(context);

                if (context.PreviousBehaviour == null) return;

                context.Behaviour.Move(context.PreviousBehaviour.Direction);
                context.Behaviour._moveSpeed = context.PreviousBehaviour.Speed;
            }
        }
    }
}
