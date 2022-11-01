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



        public override void HandleInput(EntityController controller) {
            if ( controller.crouchInput.started )
                seat.StopSitting();

            if (seat.seatEntity != null)
                seat.seatEntity.state.HandleInput(controller);
        }


        public override void Move(Vector3 direction) {;}
        public override void Jump() {;}
        public override void Evade(Vector3 direction) {;}
        public override void Parry() {;}
        public override void LightAttack() {;}
        public override void HeavyAttack() {;}
        public override void SetSpeed(MovementSpeed speed) {;}



        protected internal override void StateUpdate(){
        }

        protected internal override void StateFixedUpdate(){
            // Sitting
            entity.transform.position = seat.sitPosition;
            entity.RotateTowardsAbsolute(seat.sitRotation);
            entity.absoluteForward = entity.transform.forward;
        }
    }
}
