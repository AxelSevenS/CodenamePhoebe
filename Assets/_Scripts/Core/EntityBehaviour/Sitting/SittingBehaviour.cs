using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SeleneGame.Core {
    
    [System.Serializable]
    public sealed class SittingBehaviour : EntityBehaviour {

        [SerializeField] private Seat _seat;
        [SerializeField] private Seat.SittingPose _sittingPose;
        

        public Seat seat {
            get => _seat;
        }

        public override float gravityMultiplier => 0f; 

        public override Vector3 direction => Vector3.zero;
        public override float speed => 0f;

        protected override Vector3 jumpDirection => Vector3.zero;

        protected override Vector3 evadeDirection => Vector3.zero;

        protected override bool canParry => false;



        // public SittingBehaviour(Entity entity, EntityBehaviour previousBehaviour, Seat seat, Seat.SittingPose sittingPose) : base(entity, previousBehaviour) {

        //     if (seat == null)
        //         throw new System.ArgumentNullException("Seat cannot be null!");

        //     seat.StopSitting();
        //     _seat = seat;
        //     seat.seatOccupant = entity;
        //     _sittingPose = sittingPose;

        //     entity.rigidbody.detectCollisions = false;
        // }

        public void SetSeat(Seat seat) {
            if (seat == null)
                throw new System.ArgumentNullException("Seat cannot be null!");

            seat.StopSitting();
            _seat = seat;
            seat.seatOccupant = entity;
        }

        public void SetPose(Seat.SittingPose sittingPose) {
            _sittingPose = sittingPose;
        }

        public override void Initialize(Entity entity, EntityBehaviour previousBehaviour = null) {
            _entity = entity;
        }

        protected internal override void HandleInput(Player controller) {

            base.HandleInput(controller);

            if ( controller.crouchInput.started )
                seat.StopSitting();

            seat?.seatEntity?.behaviour?.HandleInput(controller);
        }


        protected internal override void Move(Vector3 direction) {;}
        protected internal override void Jump() {;}
        protected internal override void Evade(Vector3 direction) {;}
        protected internal override void LightAttack() {;}
        protected internal override void HeavyAttack() {;}
        protected internal override void SetSpeed(Entity.MovementSpeed speed) {;}
        


        private void OnEnable() {
            entity.rigidbody.detectCollisions = false;
        }

        private void Update() {

            Transform seatTransform = seat.seatEntity?.modelTransform ?? seat.transform;

            Vector3 sitPosition = seatTransform.TransformPoint(_sittingPose.position) + (entity.modelTransform.up * entity.character.data.size.y/2f);
            Quaternion sitRotation = seatTransform.rotation * _sittingPose.rotation;
            
            if (_sittingPose.affectCamera) {
                entity.transform.SetPositionAndRotation(sitPosition, sitRotation);
            } else {
                entity.transform.position = sitPosition;
            }
            entity.character.model.RotateTowards(sitRotation, Mathf.Infinity);
            entity.absoluteForward = entity.transform.forward;
        }
    }
}
