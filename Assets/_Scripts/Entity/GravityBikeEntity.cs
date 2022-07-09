using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SeleneGame.Core;
using SeleneGame.Weapons;
using SeleneGame.States;

namespace SeleneGame.Entities {

    [RequireComponent(typeof(Seat))]
    public class GravityBikeEntity : Entity {
        
        public override bool CanTurn() => base.CanTurn();
        public override bool CanWaterHover() => base.CanWaterHover();
        public override bool CanSink() => base.CanSink();
        public override float GravityMultiplier() => base.GravityMultiplier();
        public override float JumpMultiplier() => base.JumpMultiplier();

        public override State defaultState => new VehicleState();

        private Seat seat;
        
        protected override void Awake(){

            data = new EntityData(){
                displayName = "Gravity Bicycle",
                moveIncrement = 20f,
                weight = 20f,
                jumpHeight = 20f,

                baseSpeed = 32f,
                sprintMultiplier = 1f,
                slowMultiplier = 1f,
                swimMultiplier = 0f,

                evadeSpeed = 0f,
                evadeDuration = 0f,
                evadeCooldown = 0f
            };
            
            base.Awake();

            seat = GetComponent<Seat>();
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
        protected override void LateUpdate(){
            base.LateUpdate();

        }

        protected override void EntityAnimation(){
            base.EntityAnimation();

        }

        public override void LoadModel(){
            base.LoadModel();

        }
        public override void DestroyModel(){
            base.DestroyModel();

        }

        public override void SetStyle(int style){;}

    }
}
