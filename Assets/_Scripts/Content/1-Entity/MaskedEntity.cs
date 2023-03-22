using System;
using System.Collections.Generic;
using UnityEngine;

using SeleneGame.Core;

using SevenGame.Utility;
using System.Reflection;

namespace SeleneGame.Content {

    public class MaskedEntity : ArmedEntity {

        [Header("Mask")]
        
        [SerializeReference] /* [ReadOnly]  */protected EidolonMask _mask;

        public bool focusing;

        // private SpeedlinesEffect speedlines;
        public float shiftEnergy = 0f;


        public EidolonMask mask => _mask;

        public override float weight {
            get {
                return (character?.data?.weight ?? 1f) * (weapons?.current?.data.weight ?? 1f);
            }
        }

        public override Type defaultState => typeof(GroundedBehaviour);


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

        private void Awake() {
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




