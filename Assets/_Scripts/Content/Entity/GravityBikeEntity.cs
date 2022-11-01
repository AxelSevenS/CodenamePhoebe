using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SeleneGame.Core;
using SeleneGame.Weapons;
using SeleneGame.States;

namespace SeleneGame.Entities {

    [RequireComponent(typeof(BikeSeat))]
    public sealed class GravityBikeEntity : Entity {


        [SerializeField]private Seat _seat;



        public override State defaultState => new VehicleState();
        public Seat seat {
            get {
                if (_seat == null) {
                    _seat = GetComponent<BikeSeat>();

                    seat.seatEntity = this;
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
