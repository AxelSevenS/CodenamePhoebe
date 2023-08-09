using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SeleneGame.Core {
    
    [System.Serializable]
    [DefaultExecutionOrder(100)]
    public sealed class SittingBehaviour : EntityBehaviour {

        [SerializeField] private EntitySeat _seat;
        [SerializeField] private EntitySeat.SittingPose _sittingPose;
        
        public override float GravityMultiplier => 0f; 
        public EntitySeat Seat => _seat;
        public EntitySeat.SittingPose SittingPose => _sittingPose;


        protected internal override void HandleInput(Player controller) {
            base.HandleInput(controller);

            if ( controller.crouchInput.started )
                _seat.StopSitting();

            _seat?.seatEntity?.Behaviour?.HandleInput(controller);
        }

        public void SetSeat(EntitySeat seat) {
            if (seat == null)
                throw new System.ArgumentNullException("Seat cannot be null!");

            if ( seat.seatOccupant != Entity) {
                seat.StopSitting();
                seat.seatOccupant = Entity;
            }
            
            _seat = seat;
        }
        public void SetPose(EntitySeat.SittingPose sittingPose) {
            _sittingPose = sittingPose;
        }


        private void OnEnable() {
            Entity.Rigidbody.detectCollisions = false;
        }
        private void Update() {
            Transform seatTransform = _seat.seatEntity?.ModelTransform ?? _seat.transform;

            Vector3 sitPosition = seatTransform.TransformPoint(_sittingPose.position) + (Entity.ModelTransform.up * Entity.Character.Data.size.y/2f);
            Quaternion sitRotation = seatTransform.rotation * _sittingPose.rotation;
            
            if (_sittingPose.affectCamera) {
                Entity.transform.SetPositionAndRotation(sitPosition, sitRotation);
            } else {
                Entity.transform.position = sitPosition;
            }
            Entity.Character.Model.RotateTowards(sitRotation, Mathf.Infinity);
            Entity.AbsoluteForward = Entity.transform.forward;
        }



        public class Builder : Builder<SittingBehaviour> {

            private readonly EntitySeat seat;
            private EntitySeat.SittingPose sittingPose;

            public Builder( EntitySeat seat, EntitySeat.SittingPose sittingPose = default ) {
                this.seat = seat;
                this.sittingPose = sittingPose;
            }

            public override SittingBehaviour Build(Entity entity, EntityBehaviour previousBehaviour = null) {

                if (seat == null) {
                    throw new System.ArgumentNullException("Seat cannot be null!");
                }

                return base.Build(entity, previousBehaviour);
            }

            protected override void ConfigureBehaviour(Context context) {
                base.ConfigureBehaviour(context);
                context.Behaviour.SetSeat(seat);
                context.Behaviour.SetPose(sittingPose);
            }
        }
    }
}
