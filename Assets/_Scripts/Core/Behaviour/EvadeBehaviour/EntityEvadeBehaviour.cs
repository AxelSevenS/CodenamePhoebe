using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Animancer;

using SevenGame.Utility;

namespace SeleneGame.Core {

    [System.Serializable]
    public abstract class EntityEvadeBehaviour : CompositeBehaviour {


        private MixerTransition2DAsset.UnShared _evadeMixer;

        protected Vector2 animationDirection;

        public BoolData state;
        public Vector3 currentDirection = Vector3.forward;
        public TimeInterval timer;

        public float Time { get; protected set; }
        public float Speed { get; protected set; }

        public virtual Vector3 DefaultDirection => Entity.transform.forward;



        public virtual bool CanEvade => timer.isDone;

        protected MixerTransition2DAsset.UnShared EvadeMixer {
            get {
                UpdateEvadeMixer();
                return _evadeMixer;
            }
        }



        protected internal override void HandleInput(Player contoller) {;}


        protected internal virtual void Evade(Vector3 direction) {
            if ( !CanEvade ) return;

            currentDirection = direction;
            timer.SetDuration(Entity.Character.Data.TotalEvadeDuration);
            Animation();
        }
        protected internal void Evade() {
            Evade(DefaultDirection);
        }
        

        private void UpdateEvadeMixer(bool forceReload = false) {
            _evadeMixer ??= new MixerTransition2DAsset.UnShared();
            if ( forceReload || !_evadeMixer.HasAsset ) {
                _evadeMixer.Asset = Entity.Character.Data.GetTransition("EvadeGrounded") as MixerTransition2DAsset;
                _evadeMixer.Events.OnEnd = () => {
                    Entity.Animancer.Stop(_evadeMixer);
                };
            }
        }

        protected virtual void Animation() {
            Entity.Animancer.Layers[0].Play(EvadeMixer);
        }

        protected virtual void UpdateAnimations() {
            UpdateEvadeMixer(true);
        }


        private void OnSetCharacter(Character character) {
            UpdateAnimations();
        }


        private void OnEnable() {
            Entity.OnSetCharacter += OnSetCharacter;
        }

        private void OnDisable() {
            Entity.OnSetCharacter -= OnSetCharacter;
        }

        protected virtual void Update() {

            state.SetVal( timer > Entity.Character.Data.evadeCooldown );

            Time = Mathf.Clamp01( 1 - ( (timer - Entity.Character.Data.evadeCooldown) / Entity.Character.Data.evadeDuration ) );
            
            float newSpeed = 0f;
            if ( state ) {
                newSpeed = Mathf.Clamp01( EntityManager.current.evadeCurve.Evaluate( Time ) );
            }
            Speed = Mathf.Lerp(Speed, newSpeed, 3f * GameUtility.timeDelta);

            if ( Entity.ModelTransform == null ) return;


            float rightDirection = Vector3.Dot(currentDirection, Entity.ModelTransform.right);
            float forwardDirection = Vector3.Dot(currentDirection, Entity.ModelTransform.forward);
            animationDirection = new Vector2(rightDirection, forwardDirection);

            if (EvadeMixer.State != null)
                EvadeMixer.State.Parameter = animationDirection;

            Entity.Displace( Speed * Entity.Character.Data.evadeSpeed * currentDirection );
        }



        public abstract class Builder<TEvadeBehaviour> : Builder<TEvadeBehaviour, EntityEvadeBehaviour> where TEvadeBehaviour : EntityEvadeBehaviour {

            public override TEvadeBehaviour Build(Entity entity, EntityBehaviour parentBehaviour, EntityEvadeBehaviour previousBehaviour = null) {
                return base.Build(entity, parentBehaviour, previousBehaviour);
            }

        }
    }
}
