using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SeleneGame.Core {
    
    [System.Serializable]
    public sealed class Sitting : State {

        [HideInInspector] public Seat seat;
        


        public override float gravityMultiplier => 0f; 



        protected override Vector3 jumpDirection => Vector3.zero;

        protected override Vector3 evadeDirection => Vector3.zero;

        protected override bool canParry => false;



        public override void Transition(Vector3 direction, float speed) {;}
        public override void GetTransitionData(out Vector3 direction, out float speed) {
            direction = Vector3.zero;
            speed = 0f;
        }

        protected internal override void HandleInput(PlayerEntityController controller) {

            base.HandleInput(controller);

            if ( controller.crouchInput.started )
                seat.StopSitting();

            if (seat.seatEntity != null)
                seat.seatEntity.state.HandleInput(controller);
        }


        protected internal override void Move(Vector3 direction) {;}
        protected internal override void Jump() {;}
        protected internal override void Evade(Vector3 direction) {;}
        protected internal override void Parry() {;}
        protected internal override void LightAttack() {;}
        protected internal override void HeavyAttack() {;}
        protected internal override void SetSpeed(Entity.MovementSpeed speed) {;}

        protected internal override void OnDestroy() {
            base.OnDestroy();
            seat.seatOccupant = null;
        }


        private void FixedUpdate(){
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
