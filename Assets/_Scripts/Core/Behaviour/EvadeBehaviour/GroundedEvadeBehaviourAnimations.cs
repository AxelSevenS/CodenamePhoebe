using System.Collections;
using System.Collections.Generic;
using Animancer;
using UnityEngine;

namespace SeleneGame.Core {

    public partial class GroundedEvadeBehaviour {

        private MixerTransition2DAsset.UnShared _airEvadeMixer;

        private MixerTransition2DAsset.UnShared AirEvadeMixer {
            get {
                _airEvadeMixer ??= new MixerTransition2DAsset.UnShared();
                if ( !_airEvadeMixer.HasAsset ) {
                    UpdateAirEvadeMixer();
                }
                return _airEvadeMixer;
            }
        }

        private void UpdateAirEvadeMixer(bool forceReload = false) {
            _airEvadeMixer ??= new MixerTransition2DAsset.UnShared();
            if ( forceReload || !_airEvadeMixer.HasAsset ) {
                _airEvadeMixer.Asset = Entity.Character.Data.GetTransition("EvadeAerial") as MixerTransition2DAsset;
                _airEvadeMixer.Events.OnEnd = () => {
                    Entity.Animancer.Stop(_airEvadeMixer);
                };
            }
        }
        protected override void Animation() {
            if (Entity.onGround) {
                base.Animation();
            } else {
                Entity.Animancer.Layers[0].Play(AirEvadeMixer);
            }
        }
        protected override void UpdateAnimations() {
            base.UpdateAnimations();
            UpdateAirEvadeMixer(true);
        }
    }
}
