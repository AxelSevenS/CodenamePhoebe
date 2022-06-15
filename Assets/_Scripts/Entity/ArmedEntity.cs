using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SeleneGame.Core;

namespace SeleneGame.Entities {

    public class ArmedEntity : Entity {
        
        public WeaponInventory weapons;
        public Weapon currentWeapon => weapons.currentItem;


        public event Action onParry;
        public float parryTimer;

        public override bool CanTurn() => !evading;


        protected override void EntityDestroy(){
        }

        protected override void EntityAwake(){
            weapons = new WeaponInventory(gameObject);
        }

        protected override void EntityUpdate(){
            parryTimer = Mathf.MoveTowards( parryTimer, 0f, Global.timeDelta );

            bool lightActuated = lightAttackInput.trueTimer < 0.15f && lightAttackInput && heavyAttackInput.started;
            bool heavyActuated = heavyAttackInput.trueTimer < 0.15f && heavyAttackInput && lightAttackInput.started;
            if (lightActuated || heavyActuated)
                Parry();
            
        }

        protected override void EntityFixedUpdate() {
        }

        protected override void EntityLoadModel() {
            foreach ( ValuePair<int, Weapon> pair in weapons){
                pair.valueTwo.LoadModel();
            }
        }

        public void Parry(){
            Debug.Log("Parry");
            parryTimer = 0.15f;

            onParry?.Invoke();
        }

    }
}