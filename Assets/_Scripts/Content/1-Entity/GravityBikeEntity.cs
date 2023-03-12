using System;
using System.Collections.Generic;
using UnityEngine;

using SeleneGame.Core;

namespace SeleneGame.Content {

    [RequireComponent(typeof(Seat))]
    public sealed class GravityBikeEntity : Entity {


        [SerializeField]private Seat _seat;



        public override Type defaultState => typeof(VehicleState);
        public Seat seat {
            get {
                if (_seat == null) {
                    _seat = GetComponent<Seat>();

                    seat.seatEntity = this;
                    SetSittingPoses();
                }
                return _seat;
            }
        }


        public override void SetCharacter(CharacterData characterData, CharacterCostume costume = null) {
            base.SetCharacter(characterData, costume);
            SetSittingPoses();
        }

        private void SetSittingPoses() {
            if (character is VehicleCharacter vehicle) {
                seat.SetSittingPoses(vehicle.sittingPoses);
            }
        }
        

        public override void SetStyle(int style){;}


        protected override void Start() {
            base.Start();
            // rigidbody.constraints = RigidbodyConstraints.None;
        }
    }
}
