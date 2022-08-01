using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SeleneGame.Core;
using SeleneGame.Weapons;

namespace SeleneGame.Entities {
    public sealed class SeleneEntity : GravityShifterEntity {
        
        public override EntityData data { 
            get => new EntityData(){
                displayName = "Selene",
                acceleration = 90f,
                weight = 12.5f,
                jumpHeight = 17f,

                baseSpeed = 14f,
                sprintMultiplier = 1.35f,
                slowMultiplier = 0.7f,
                swimMultiplier = 0.85f,

                evadeSpeed = 23.5f,
                evadeDuration = 0.6f,
                evadeCooldown = 0.06f
            };
        }

        protected override void Reset(){
            
            base.Reset();

            weapons.Set(1, new HypnosWeapon());
            weapons.Set(2, new ErisWeapon());
        }

    }
}
