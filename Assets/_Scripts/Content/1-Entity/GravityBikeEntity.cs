using System;
using System.Collections.Generic;
using UnityEngine;

using SeleneGame.Core;

namespace SeleneGame.Content {

    // Default Order Execution is used to ensure the vehicle updates its position before any seated entity
    // preventing jitter while moving.
    [DefaultExecutionOrder(-10)]
    [RequireComponent(typeof(Seat))]
    public sealed class GravityBikeEntity : Entity {


        [SerializeField] private Seat _seat;


        public override void SetCharacter(CharacterData characterData, CharacterCostume costume = null) {
            base.SetCharacter(characterData, costume);
            SetSittingPoses();
        }

        private void SetSittingPoses() {
            if (character is VehicleCharacter vehicle) {
                _seat.SetSittingPoses(vehicle.sittingPoses);
            }
        }

        public override void ResetBehaviour() {
            SetBehaviour(VehicleBehaviourBuilder.Default);
        }

        public override void SetStyle(int style){;}


        protected override void Awake() {
            base.Awake();
            _seat = GetComponent<Seat>();

            _seat.seatEntity = this;
            SetSittingPoses();
        }
    }
}
