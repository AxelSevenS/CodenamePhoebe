using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Animancer;

using SevenGame.Utility;

namespace SeleneGame.Core {

    [System.Serializable]
    public class EvadeBehaviour : BehaviourStrategy {


        public BoolData state;
        public Vector3 currentDirection = Vector3.forward;
        public TimeUntil timer;

        private CartesianMixerState _evadeMixer;
        
        public float Time { get; protected set; }
        public float Speed { get; protected set; }



        public virtual bool canEvade => timer.isDone;

        protected CartesianMixerState evadeMixer {
            get {
                if (_evadeMixer == null) {
                    _evadeMixer = new CartesianMixerState();
                    _evadeMixer.Initialize(4);
                    _evadeMixer.CreateChild(0, entity.character.animations.evadeForwardAnimation, new Vector2(0, 1));
                    _evadeMixer.CreateChild(1, entity.character.animations.evadeBackwardAnimation, new Vector2(0, -1));
                    _evadeMixer.CreateChild(2, entity.character.animations.evadeRightAnimation, new Vector2(1, 0));
                    _evadeMixer.CreateChild(3, entity.character.animations.evadeLeftAnimation, new Vector2(-1, 0));

                    entity.animancer.Layers[0].AddChild(_evadeMixer);
                    _evadeMixer.SetDebugName("Evade");
                    _evadeMixer.Stop();
                }
                return _evadeMixer;
            }
        }



        public EvadeBehaviour(State entityState) : base(entityState) { }

        protected internal override void HandleInput(PlayerEntityController contoller) {;}

        protected internal override void Update() { 

            // Debug.Log(entityState);

            state.SetVal( timer > entity.character.evadeCooldown );

            if ( !state ) {
                Time = 0f;
                Speed = 0f;
            } else {
                Time = Mathf.Clamp01( 1 - ( (timer - entity.character.evadeCooldown) / entity.character.evadeDuration ) );
                Speed = Mathf.Clamp01( EntityManager.current.evadeCurve.Evaluate( Time ) );
            }


            float rightDirection = Vector3.Dot(currentDirection, entity.modelTransform.right);
            float forwardDirection = Vector3.Dot(currentDirection, entity.modelTransform.forward);

            evadeMixer.Parameter = new Vector2(rightDirection, forwardDirection);

        }

        protected internal override void FixedUpdate() {
            if (state)
                entity.Displace( Speed * entity.character.evadeSpeed * currentDirection );
        }

        protected internal virtual void Evade(Vector3 direction) {

            currentDirection = direction;
            timer.SetDuration(entity.character.totalEvadeDuration);
            Animation();
        }

        protected virtual void Animation() {
            evadeMixer.Stop();
            entity.animancer.Layers[0].Play(evadeMixer, 0.1f);
            evadeMixer.Events.OnEnd = () => {
                evadeMixer.Stop();
            };
        }
    }
}
