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

        protected override void EntityAwake(){
            weapons = new WeaponInventory(this);
        }

        protected override void EntityUpdate(){
            parryTimer = Mathf.MoveTowards( parryTimer, 0f, Global.timeDelta );

            bool lightActuated = lightAttackInput.trueTimer < 0.15f && lightAttackInput && heavyAttackInput.started;
            bool heavyActuated = heavyAttackInput.trueTimer < 0.15f && heavyAttackInput && lightAttackInput.started;
            if (lightActuated || heavyActuated)
                Parry();
            
            foreach (Weapon weapon in weapons)
                weapon.WeaponUpdate();
        }

        protected override void EntityFixedUpdate() {
            foreach (Weapon weapon in weapons)
                weapon.WeaponFixedUpdate();
        }

        protected override void EntityLoadModel() {
            foreach ( Weapon weapon in weapons){
                weapon.LoadModel();
            }
        }

        public void Parry(){
            Debug.Log("Parry");
            parryTimer = 0.15f;

            onParry?.Invoke();
        }

    }
}