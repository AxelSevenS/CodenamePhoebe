using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SeleneGame.Core;
using SeleneGame.Weapons;

namespace SeleneGame.Entities {
    public class SeleneEntity : GravityShifterEntity {
        
        protected override void Awake(){

            data = new EntityData(){
                displayName = "Selene",
                moveIncrement = 90f,
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
            
            base.Awake();

            weapons.Set(1, new HypnosWeapon());
            weapons.Set(2, new ErisWeapon());
        }

    }
}
