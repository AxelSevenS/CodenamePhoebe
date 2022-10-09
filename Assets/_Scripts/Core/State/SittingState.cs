using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SeleneGame.Core {
    
    public class SittingState : NoGravityState {

        [HideInInspector] public Seat seat;


        public override Vector3 evadeDirection => Vector3.zero;
        public override bool canEvade => false;
        public override Vector3 cameraPosition => seat.seatEntity?.state.cameraPosition ?? base.cameraPosition;



        public override void HandleInput(EntityController controller) {
            if ( controller.crouchInput.started )
                seat.StopSitting();
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
