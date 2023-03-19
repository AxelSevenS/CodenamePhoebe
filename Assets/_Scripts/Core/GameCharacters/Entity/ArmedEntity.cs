using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SevenGame.Utility;

namespace SeleneGame.Core {

    public class ArmedEntity : Entity {

        
        [Header("Weapons")]
        [SerializeReference] protected WeaponInventory _weapons = null;


        [Header("Parrying")]
        public TimeInterval parryTimer;



        public override float weight => weight * (weapons?.current?.data.weight ?? 1f); 

        public WeaponInventory weapons {
            get {
                if (_weapons == null) {
                    ResetWeapons();
                }
                return _weapons;
            }
        }



        protected internal virtual void ResetWeapons(){
            _weapons?.Dispose();
            _weapons = new ListWeaponInventory(this, 1);
        }


        public override void SetStyle(int newStyle) {
            weapons.Switch(newStyle);
        }

        // protected internal override void LoadModel() {
        //     base.LoadModel();
        //     // foreach(Weapon weapon in weapons) {
        //     //     weapon.LoadModel();
        //     // }
        // }


        protected override void EntityAnimation() {
            base.EntityAnimation();

            // animator.SetFloat("WeaponType", (float)(weapons.current.weaponType) );
        }



        protected override void EntityReset(){
            base.EntityReset();
            ResetWeapons();
        }

        protected override void Update(){
            base.Update();
            
            foreach (Weapon weapon in weapons)
                weapon?.Update();
        }

        protected override void FixedUpdate() {
            base.FixedUpdate();

            foreach (Weapon weapon in weapons)
                weapon?.FixedUpdate();
        }

    }
}