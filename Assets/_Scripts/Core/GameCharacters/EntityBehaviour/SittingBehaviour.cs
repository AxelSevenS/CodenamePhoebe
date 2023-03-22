using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SeleneGame.Core {
    
    [System.Serializable]
    public sealed class SittingBehaviour : EntityBehaviour {

        [SerializeField] private Seat _seat;
        

        public Seat seat {
            get => _seat;
        }

        public override float gravityMultiplier => 0f; 

        public override Vector3 direction => Vector3.zero;
        public override float speed => 0f;

        protected override Vector3 jumpDirection => Vector3.zero;

        protected override Vector3 evadeDirection => Vector3.zero;

        protected override bool canParry => false;



        public SittingBehaviour(Entity entity, EntityBehaviour previousBehaviour, Seat seat) : base(entity, previousBehaviour) {

            if (seat == null)
                throw new System.ArgumentNullException("Seat cannot be null!");

            seat.StopSitting();
            _seat = seat;
            seat.seatOccupant = entity;

            entity.rigidbody.detectCollisions = false;
        }

        protected override void DisposeBehavior() {
            base.DisposeBehavior();
            seat.seatOccupant = null;
            entity.rigidbody.detectCollisions = true;
        }
        

        protected internal override void HandleInput(PlayerEntityController controller) {

            base.HandleInput(controller);

            if ( controller.crouchInput.started )
                seat.StopSitting();

            if (seat.seatEntity != null)
                seat.seatEntity.behaviour.HandleInput(controller);
        }


        protected internal override void Move(Vector3 direction) {;}
        protected internal override void Jump() {;}
        protected internal override void Evade(Vector3 direction) {;}
        protected internal override void Parry() {;}
        protected internal override void LightAttack() {;}
        protected internal override void HeavyAttack() {;}
        protected internal override void SetSpeed(Entity.MovementSpeed speed) {;}
        


        public override void LateUpdate(){

            base.LateUpdate();
            
            if (seat.affectCamera) {
                entity.transform.SetPositionAndRotation(seat.sitPosition, seat.sitRotation);
            } else {
                entity.transform.position = seat.sitPosition;
            }
            entity.character.model.RotateTowards(seat.sitRotation, Mathf.Infinity);
            entity.absoluteForward = entity.transform.forward;
        }
    }
}
