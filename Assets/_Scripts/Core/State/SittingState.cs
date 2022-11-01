using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SeleneGame.Core {
    
    public sealed class SittingState : State {

        [HideInInspector] public Seat seat;
        


        public override float gravityMultiplier => 0f; 

        public override Vector3 jumpDirection => Vector3.zero;
        public override bool canJump => false;

        public override Vector3 evadeDirection => Vector3.zero;
        public override bool canEvade => false;

        public override bool canParry => false;


        public override Vector3 cameraPosition => seat.seatEntity?.state.cameraPosition ?? base.cameraPosition;



        public override void HandleInput(EntityController controller) {
            if ( controller.crouchInput.started )
                seat.StopSitting();

            if (seat.seatEntity != null)
                seat.seatEntity.state.HandleInput(controller);
        }

        public override void StateUpdate(){
        }

        public override void StateFixedUpdate(){
            // Sitting
            entity.transform.position = seat.sitPosition;
            entity.RotateTowardsAbsolute(seat.sitRotation);
            entity.absoluteForward = entity.transform.forward;
        }
    }
}
