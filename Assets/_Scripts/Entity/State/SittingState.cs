using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SeleneGame {
    
    public class SittingState : State{
        
        public override int id => 100;
        protected override Vector3 GetCameraPosition() => new Vector3(0f, 1f, -4f);

        // public override bool canJump => false;
        // public override bool canEvade => false;
        // public override bool canStep => false;
        // public override bool canTurn => false;
        // public override bool UseGravity => false;
        // public override bool Shifting => false;

        [HideInInspector] public Seat seat;

        private void OnEnable(){
            entity.crouchInputData.startAction += OnCrouchInput;
        }
        private void OnDisable(){
            entity.crouchInputData.startAction -= OnCrouchInput;
        }


        public override void StateFixedUpdate(){

            // Sitting
            entity._t.rotation = Quaternion.LookRotation( Vector3.ProjectOnPlane(seat.transform.position - entity._t.position, seat.transform.up), seat.transform.up);
            entity.absoluteForward = entity._t.forward;
            // entity.evadeDirection = Vector3.Lerp(entity.evadeDirection, entity.absoluteForward, 0.1f);
            // entity.rotationForward = Vector3.Lerp(entity.rotationForward, entity.relativeForward, 0.7f).normalized;
            entity._t.position = seat.sitPosition;
        }

        private void OnCrouchInput(float timer){
            seat.UnSit();
        }

        public override void HandleInput(){
            if (seat.seatEntity == null) return;

            SafeDictionary<string, bool> inputDict = new SafeDictionary<string, bool>();

            inputDict[ "LightAttack" ] = entity.lightAttackInputData.currentValue;
            inputDict[ "HeavyAttack" ] = entity.heavyAttackInputData.currentValue;
            inputDict[ "Jump" ] = entity.jumpInputData.currentValue;
            inputDict[ "Evade" ] = entity.evadeInputData.currentValue;
            inputDict[ "Focus" ] = entity.focusInputData.currentValue;
            inputDict[ "Shift" ] = entity.shiftInputData.currentValue;

            seat.seatEntity.EntityInput(entity.currentFrameMoveInput, entity.currentFrameLookRotation, inputDict);
        }
    }
}
