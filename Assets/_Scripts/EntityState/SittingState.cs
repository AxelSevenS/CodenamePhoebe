using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SeleneGame.Core;
using SeleneGame.Entities;

namespace SeleneGame.States {
    
    public class SittingState : State{
        
        public override int id => 100;
        protected override Vector3 GetCameraPosition() => seat.seatEntity.state.cameraPosition;

        public override bool masked => true;

        [HideInInspector] public Seat seat;

        protected override void StateUpdate(){
            if (entity.crouchInputData.started)
                seat.StopSitting();
        }

        protected override void StateFixedUpdate(){
            // Sitting
            entity._transform.position = seat.sitPosition;
            entity.absoluteForward = entity._transform.forward;
            entity._transform.rotation = Quaternion.LookRotation( Vector3.ProjectOnPlane(seat.transform.position - entity._transform.position, seat.transform.up), seat.transform.up);
        }

        protected override void UpdateMoveSpeed(){;}

        public override void HandleInput(){
            if (seat.seatEntity == null) return;

            SafeDictionary<string, bool> inputDict = new SafeDictionary<string, bool>();

            inputDict[ "LightAttack" ] = entity.lightAttackInputData.currentValue;
            inputDict[ "HeavyAttack" ] = entity.heavyAttackInputData.currentValue;
            inputDict[ "Jump" ] = entity.jumpInputData.currentValue;
            inputDict[ "Evade" ] = entity.evadeInputData.currentValue;
            inputDict[ "Focus" ] = entity.focusInputData.currentValue;
            inputDict[ "Shift" ] = entity.shiftInputData.currentValue;

            seat.seatEntity.EntityInput(entity.moveInputData.currentValue, entity.lookRotation, inputDict);
        }
    }
}
