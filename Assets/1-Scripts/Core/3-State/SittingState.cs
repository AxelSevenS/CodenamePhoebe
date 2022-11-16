using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SeleneGame.Core {
    
    public sealed class SittingState : State {

        [HideInInspector] public Seat seat;
        


        public override float gravityMultiplier => 0f; 

        public override Vector3 cameraPosition => seat.seatEntity?.state.cameraPosition ?? base.cameraPosition;


        protected override Vector3 jumpDirection => Vector3.zero;
        protected override bool canJump => false;

        protected override Vector3 evadeDirection => Vector3.zero;
        protected override bool canEvade => false;

        protected override bool canParry => false;



        protected internal override void HandleInput(EntityController controller) {
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



        protected internal override void StateUpdate(){
        }

        protected internal override void StateFixedUpdate(){
            // Sitting
            entity.transform.SetPositionAndRotation(seat.sitPosition, seat.sitRotation);
            entity.RotateModelTowards(seat.sitRotation);
            entity.absoluteForward = entity.transform.forward;
        }
    }
}
