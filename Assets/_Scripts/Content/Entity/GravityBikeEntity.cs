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
        

        public override void LoadModel(){
            base.LoadModel();

        }
        public override void UnloadModel(){
            base.UnloadModel();

        }

        public override void SetStyle(int style){;}
        
        protected override void EntityAnimation(){
            base.EntityAnimation();

        }

        


        protected override void Reset(){
            base.Reset();

        }
        protected override void OnDestroy(){
            base.OnDestroy();

        }

        protected override void Update(){
            base.Update();

        }
        protected override void FixedUpdate(){
            base.FixedUpdate();

        }

    }
}
