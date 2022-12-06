using System;
using System.Collections.Generic;
using UnityEngine;

using SeleneGame.Core;

namespace SeleneGame.Content {

    [RequireComponent(typeof(Seat))]
    public sealed class GravityBikeEntity : Entity {


        [SerializeField]private Seat _seat;



        public override State defaultState => new VehicleState();
        public Seat seat {
            get {
                if (_seat == null) {
                    _seat = GetComponent<Seat>();

                    seat.seatEntity = this;
                    if (character is VehicleCharacter vehicle) {
                        seat.SetSittingPoses(vehicle.sittingPoses);
                    }
                }
                return _seat;
            }
        }
        

        protected override void LoadModel(){
            base.LoadModel();

        }
        protected override void UnloadModel(){
            base.UnloadModel();

        }

        public override void SetStyle(int style){;}


        protected override void Start() {
            base.Start();
            // rigidbody.constraints = RigidbodyConstraints.None;
        }
    }
}
