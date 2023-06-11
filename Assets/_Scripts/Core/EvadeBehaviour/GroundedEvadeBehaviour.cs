using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Animancer;

using SevenGame.Utility;

namespace SeleneGame.Core {

    [System.Serializable]
    public class GroundedEvadeBehaviour : EvadeBehaviour {



        protected int evadeCount = 1;

        private MixerTransition2DAsset.UnShared _airEvadeMixer;


        public override bool canEvade => base.canEvade && evadeCount > 0;

        protected MixerTransition2DAsset.UnShared airEvadeMixer {
            get {
                _airEvadeMixer ??= new MixerTransition2DAsset.UnShared();
                if ( !_airEvadeMixer.HasAsset ) {
                    UpdateAirEvadeMixer();
                }
                return _airEvadeMixer;
            }
        }


        // public GroundedEvadeBehaviour(Entity entity) : base(entity) {;}


        protected internal override void Evade(Vector3 direction) {

            base.Evade(direction);

            evadeCount--;
                
            if (entity.behaviour.gravityMultiplier > 0f) {

                Vector3 newInertia = entity.inertia.NullifyInDirection( entity.gravityDown );
                if (!entity.onGround){
                    newInertia += -entity.gravityDown.normalized * 5f;
                }
                entity.inertia = newInertia;
            }

        }

        private void UpdateAirEvadeMixer(bool forceReload = false) {
            _airEvadeMixer ??= new MixerTransition2DAsset.UnShared();
            if ( forceReload || !_airEvadeMixer.HasAsset ) {
                _airEvadeMixer.Asset = entity.character.data.GetTransition("EvadeAerial") as MixerTransition2DAsset;
                _airEvadeMixer.Events.OnEnd = () => {
                    entity.animancer.Stop(_airEvadeMixer);
                };
            }
        }

        protected override void Animation() {
            if (entity.onGround) {
                base.Animation();
            } else {
                entity.animancer.Layers[0].Play(airEvadeMixer);
            }
        }

        protected override void UpdateAnimations() {
            base.UpdateAnimations();
            UpdateAirEvadeMixer(true);
        }


        protected override void Update() {

            base.Update();

            if ( airEvadeMixer.State != null )
                _airEvadeMixer.State.Parameter = animationDirection;

            if ( entity.onGround ) {
                evadeCount = 1;
            }
        }
    }
}
