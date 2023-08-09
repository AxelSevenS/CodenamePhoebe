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



        public override float Weight => base.Weight * (Weapons?.current?.Data.weight ?? 1f); 

        public WeaponInventory Weapons {
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
            Weapons.Switch(newStyle);
        }



        protected override void EntityReset(){
            base.EntityReset();
            ResetWeapons();
        }

        protected override void Update(){
            base.Update();
            
            Weapons?.Update();
        }

        protected override void LateUpdate() {
            base.LateUpdate();

            Weapons?.LateUpdate();
        }

        protected override void FixedUpdate() {
            base.FixedUpdate();

            Weapons?.FixedUpdate();
        }

    }
}