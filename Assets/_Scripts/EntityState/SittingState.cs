using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SeleneGame.Core;
using SeleneGame.Entities;

namespace SeleneGame.States {
    
    [System.Serializable]
    public class SittingState : State{
        
        public override int id => 100;
        protected override Vector3 GetCameraPosition() => seat.seatEntity?.state.cameraPosition ?? base.GetCameraPosition();

        // public override bool masked => true;

        [HideInInspector] public Seat seat;

        public override void StateUpdate(){
            if (entity.crouchInput.started)
                seat.StopSitting();
        }

        public override void StateFixedUpdate(){
            // Sitting
            entity._transform.position = seat.sitPosition;
            entity.absoluteForward = entity._transform.forward;
            entity.RotateTowardsAbsolute(Vector3.ProjectOnPlane(seat.transform.rotation * -seat.sittingDir, seat.transform.up), seat.transform.up);
        }

        // public override float UpdateMoveSpeed(){
        //     return base.UpdateMoveSpeed();
        // }

        public override void HandleInput(){
            if (seat.seatEntity == null) return;

            SafeDictionary<string, bool> inputDict = new SafeDictionary<string, bool>();

            inputDict[ "LightAttack" ] = entity.lightAttackInput;
            inputDict[ "HeavyAttack" ] = entity.heavyAttackInput;
            inputDict[ "Jump" ] = entity.jumpInput;
            inputDict[ "Evade" ] = entity.evadeInput;
            inputDict[ "Focus" ] = entity.focusInput;
            inputDict[ "Shift" ] = entity.shiftInput;

            seat.seatEntity.EntityInput(entity.moveInput, entity.cameraRotation, inputDict);
        }
    }
}
