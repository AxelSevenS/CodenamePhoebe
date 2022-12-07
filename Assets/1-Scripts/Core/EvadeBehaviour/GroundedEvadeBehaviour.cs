using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Animancer;

using SevenGame.Utility;

namespace SeleneGame.Core {

    [System.Serializable]
    public class GroundedEvadeBehaviour : EvadeBehaviour {



        protected int evadeCount = 1;

        private CartesianMixerState _airEvadeMixer;


        public override bool canEvade => base.canEvade && evadeCount > 0;

        protected CartesianMixerState airEvadeMixer {
            get {
                if (_airEvadeMixer == null) {
                    _airEvadeMixer = new CartesianMixerState();
                    _airEvadeMixer.Initialize(4);
                    _airEvadeMixer.CreateChild(0, entity.character.animations.evadeAirForwardAnimation, new Vector2(0, 1));
                    _airEvadeMixer.CreateChild(1, entity.character.animations.evadeAirBackwardAnimation, new Vector2(0, -1));
                    _airEvadeMixer.CreateChild(2, entity.character.animations.evadeAirRightAnimation, new Vector2(1, 0));
                    _airEvadeMixer.CreateChild(3, entity.character.animations.evadeAirLeftAnimation, new Vector2(-1, 0));

                    entity.animancer.Layers[0].AddChild(_airEvadeMixer);
                    _airEvadeMixer.SetDebugName("Air Evade");
                    _airEvadeMixer.Stop();
                }
                return _airEvadeMixer;
            }
        }



        protected override void Update() {

            base.Update();

            airEvadeMixer.Parameter = evadeMixer.Parameter;

            if ( entity.onGround ) {
                evadeCount = 1;
            }
        }


        protected internal override void Evade(Vector3 direction) {

            base.Evade(direction);

            evadeCount--;
                
            if (entity.state.gravityMultiplier > 0f) {

                Vector3 newVelocity = entity.rigidbody.velocity.NullifyInDirection( entity.gravityDown );
                if (!entity.onGround){
                    newVelocity += -entity.gravityDown.normalized * 5f;
                }
                entity.rigidbody.velocity = newVelocity;
            }

        }

        protected override void Animation() {
            
            if (entity.onGround) {
                base.Animation();
            } else {
                airEvadeMixer.Stop();
                entity.animancer.Layers[0].Play(airEvadeMixer, 0.1f);
                airEvadeMixer.Events.OnEnd = () => {
                    airEvadeMixer.Stop();
                };
            }
        }
    }
}
