using System;
using System.Collections.Generic;
using UnityEngine;

using SeleneGame.Core;

using SevenGame.Utility;
using System.Reflection;

namespace SeleneGame.Content {

    public class MaskedEntity : ArmedEntity {

        [Header("Mask")]
        
        [SerializeReference] protected EidolonMask _mask;


        public EidolonMask mask => _mask;


        public override void HandleInput(PlayerEntityController controller) {
            base.HandleInput(controller);
            mask?.HandleInput(controller);
        }

        public void SetMask(EidolonMaskData data, EidolonMaskCostume costume = null) {

            _mask?.Dispose();
            _mask = null;
            
            if (data == null) return;

            _mask = data.GetMask(this, costume);

        }


        protected sealed override void ResetWeapons() {
            _weapons?.Dispose();
            _weapons = new MaskedWeaponInventory(this);
        }



        protected override void EntityReset(){
            base.EntityReset();
            SetMask( null );
        }

        protected override void Update(){
            base.Update();

            mask?.Update();
        }

        protected override void LateUpdate() {
            base.LateUpdate();

            mask?.LateUpdate();
        }

        protected override void FixedUpdate() {
            base.FixedUpdate();

            mask?.FixedUpdate();
        }
        

    }
}




