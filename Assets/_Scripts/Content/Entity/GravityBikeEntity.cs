using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SeleneGame.Core;
using SeleneGame.Weapons;
using SeleneGame.States;

namespace SeleneGame.Entities {

    [RequireComponent(typeof(Seat))]
    public sealed class GravityBikeEntity : Entity {


        [SerializeField]private Seat _seat;


        public override EntityController controller { get => seat?.seatOccupant?.controller ?? base.controller; }
        public override State defaultState => new VehicleState();
        public Seat seat {
            get {
                if (_seat == null) {
                    _seat = GetComponent<Seat>();

                    seat.seatEntity = this;
                    SetSeatSittingPositions();
                }
                return _seat;
            }
        }


        protected /* virtual */ void SetSeatSittingPositions() {
            seat.SetDirections( new List<Vector4>() {
                new Vector4(1, 0, 0, 1),
                new Vector4(-1, 0, 0, 2)
            } );
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
