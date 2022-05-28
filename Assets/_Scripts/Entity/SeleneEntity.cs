using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SeleneGame.Core;
using SeleneGame.States;

namespace SeleneGame.Entities {

    public class SeleneEntity : GravityShifterEntity {


        protected override void EntityEnable(){
        }
        protected override void EntityDisable(){
        }

        protected override void EntityDestroy(){

            base.EntityDestroy();
        }

        protected override void EntityAwake(){

            base.EntityAwake();
            
            weapons.Set(1, Weapon.GetWeaponTypeByName("Hypnos"));
            weapons.Set(2, Weapon.GetWeaponTypeByName("Eris"));
        }

        protected override void EntityUpdate(){
            base.EntityUpdate();
        }

        protected override void EntityFixedUpdate(){
            base.EntityFixedUpdate();
        }
    }
}