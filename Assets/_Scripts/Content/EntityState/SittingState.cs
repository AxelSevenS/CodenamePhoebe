using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SeleneGame.Core;
using SeleneGame.Entities;

namespace SeleneGame.States {
    
    public class SittingState : State{
        
        public override StateType stateType => StateType.immobileState;
        protected override Vector3 GetCameraPosition() => seat.seatEntity?.state.cameraPosition ?? base.GetCameraPosition();

        // public override bool masked => true;

        [HideInInspector] public Seat seat;

        public override void StateUpdate(){
        }

        public override void StateFixedUpdate(){
            // Sitting
            entity.transform.position = seat.sitPosition;
            entity.absoluteForward = entity.transform.forward;
            entity.RotateTowardsAbsolute(Vector3.ProjectOnPlane(seat.transform.rotation * -seat.sittingDir, seat.transform.up), seat.transform.up);
        }

        public override void HandleInput(){
            if (seat.seatEntity == null) return;

            if (entity.controller.crouchInput.started)
                seat.StopSitting();

            entity.moveSpeed = 0;
        }
    }
}
