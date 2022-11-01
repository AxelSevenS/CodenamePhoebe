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
        public TimeUntil parryTimer;



        public override float weight => Mathf.Min( base.weight, weapons.current.weightModifier ); 

        public WeaponInventory weapons {
            get {
                if (_weapons == null) {
                    ResetWeapons();
                }
                return _weapons;
            }
        }



        protected internal virtual void ResetWeapons(){
            _weapons = new ListWeaponInventory(this, 1);
        }


        public override void SetStyle(int newStyle) {
            weapons.Switch(newStyle);
        }

        protected internal override void LoadModel() {
            base.LoadModel();
            foreach(Weapon weapon in weapons) {
                weapon.LoadModel();
            }
        }

        public virtual void Parry(){

            if (parryTimer.isDone) {
                Debug.Log("Parry");
                parryTimer.SetDuration(0.1f);

                // onParry?.Invoke();
            }
        }


        protected override void EntityAnimation() {
            base.EntityAnimation();

            // animator.SetFloat("WeaponType", (float)(weapons.current.weaponType) );
        }



        protected override void Reset(){
            base.Reset();
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