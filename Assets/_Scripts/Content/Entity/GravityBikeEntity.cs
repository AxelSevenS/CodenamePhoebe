using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SeleneGame.Core;
using SeleneGame.Weapons;
using SeleneGame.States;

namespace SeleneGame.Entities {

    [RequireComponent(typeof(Seat))]
    public class GravityBikeEntity : Entity {

        public override EntityController controller { get => seat?.seatOccupant?.controller ?? base.controller; }

        // public override EntityData data { 
        //     get => new EntityData(){
        //         displayName = "Gravity Bicycle",
        //         acceleration = 20f,
        //         weight = 20f,
        //         jumpHeight = 20f,

        //         baseSpeed = 32f,
        //         sprintMultiplier = 1f,
        //         slowMultiplier = 1f,
        //         swimMultiplier = 0f,

        //         evadeSpeed = 0f,
        //         evadeDuration = 0f,
        //         evadeCooldown = 0f
        //     };
        // }
        
        public override bool CanTurn() => base.CanTurn();
        public override bool CanWaterHover() => base.CanWaterHover();
        public override bool CanSink() => base.CanSink();
        public override float GravityMultiplier() => base.GravityMultiplier();
        public override float JumpMultiplier() => base.JumpMultiplier();

        public override State defaultState => new VehicleState();

        [SerializeField]private Seat seat;
        
        protected override void Reset(){
            
            base.Reset();

            seat = GetComponent<Seat>();
            Debug.Log(seat);
            seat.seatEntity = this;
            seat.SetDirections( new List<Vector4>(){
                new Vector4(1, 0, 0, 1),
                new Vector4(-1, 0, 0, 2)
            } );

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
        

        protected override void EntityAnimation(){
            base.EntityAnimation();

        }

        public override void LoadModel(){
            base.LoadModel();

        }
        public override void UnloadModel(){
            base.UnloadModel();

        }

        public override void SetStyle(int style){;}

    }
}
