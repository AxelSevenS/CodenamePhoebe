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
        public TimeInterval timer;

        private MixerTransition2DAsset.UnShared _evadeMixer;

        protected Vector2 animationDirection;

        public float Time { get; protected set; }
        public float Speed { get; protected set; }



        public virtual bool canEvade => timer.isDone;

        protected MixerTransition2DAsset.UnShared evadeMixer {
            get {
                UpdateEvadeMixer();
                return _evadeMixer;
            }
        }



        // public EvadeBehaviour(Entity entity) : base(entity) {;}

        public virtual void Initialize(Entity entity) {
            _entity = entity;
        }

        protected internal override void HandleInput(Player contoller) {;}


        protected internal virtual void Evade(Vector3 direction) {

            currentDirection = direction;
            timer.SetDuration(entity.character.data.totalEvadeDuration);
            Animation();
        }

        private void UpdateEvadeMixer(bool forceReload = false) {
            _evadeMixer ??= new MixerTransition2DAsset.UnShared();
            if ( forceReload || !_evadeMixer.HasAsset ) {
                _evadeMixer.Asset = entity.character.data.GetTransition("EvadeGrounded") as MixerTransition2DAsset;
                _evadeMixer.Events.OnEnd = () => {
                    entity.animancer.Stop(_evadeMixer);
                };
            }
        }

        protected virtual void Animation() {
            entity.animancer.Layers[0].Play(evadeMixer);
        }

        protected virtual void UpdateAnimations() {
            UpdateEvadeMixer(true);
        }


        private void OnSetCharacter(Character character) {
            UpdateAnimations();
        }

        private void OnEnable() {
            entity.onSetCharacter += OnSetCharacter;
        }

        private void OnDisable() {
            entity.onSetCharacter -= OnSetCharacter;
        }

        protected virtual void Update() {

            state.SetVal( timer > entity.character.data.evadeCooldown );

            Time = Mathf.Clamp01( 1 - ( (timer - entity.character.data.evadeCooldown) / entity.character.data.evadeDuration ) );
            
            float newSpeed = 0f;
            if ( state ) {
                newSpeed = Mathf.Clamp01( EntityManager.current.evadeCurve.Evaluate( Time ) );
            }
            Speed = Mathf.Lerp(Speed, newSpeed, 3f * GameUtility.timeDelta);

            if ( entity.modelTransform == null ) return;


            float rightDirection = Vector3.Dot(currentDirection, entity.modelTransform.right);
            float forwardDirection = Vector3.Dot(currentDirection, entity.modelTransform.forward);
            animationDirection = new Vector2(rightDirection, forwardDirection);

            if (evadeMixer.State != null)
                evadeMixer.State.Parameter = animationDirection;

            entity.Displace( Speed * entity.character.data.evadeSpeed * currentDirection );
        }

    }
}
