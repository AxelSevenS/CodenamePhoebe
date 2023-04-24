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



        public override float weight => base.weight * (weapons?.current?.data.weight ?? 1f); 

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



        protected override void EntityReset(){
            base.EntityReset();
            ResetWeapons();
        }

        protected override void Update(){
            base.Update();
            
            weapons?.Update();
        }

        protected override void LateUpdate() {
            base.LateUpdate();

            weapons?.LateUpdate();
        }

        protected override void FixedUpdate() {
            base.FixedUpdate();

            weapons?.FixedUpdate();
        }

    }
}