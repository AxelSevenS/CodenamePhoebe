using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Animancer;

using SevenGame.Utility;

namespace SeleneGame.Core {

    public partial class GroundedBehaviour {

        private float movementSpeedWeight;
        [SerializeField] [HideInInspector] private LinearMixerTransitionAsset.UnShared _movementStartMixer;
        [SerializeField] [HideInInspector] private LinearMixerTransitionAsset.UnShared _movementMixer;
        [SerializeField] [HideInInspector] private LinearMixerTransitionAsset.UnShared _movementStopMixer;
        [SerializeField] [HideInInspector] private AnimancerState _idleState;

        private AnimationClip _fallAnimation;
        private AnimationClip _landAnimation;


        private AnimancerLayer _layer => entity.animancer.Layers[0];

        public LinearMixerTransitionAsset.UnShared movementStartMixer {
            get {
                UpdateMovementStartMixer();
                return _movementStartMixer;
            }
        }

        public LinearMixerTransitionAsset.UnShared movementMixer {
            get {
                UpdateMovementMixer();
                return _movementMixer;
            }
        }

        public LinearMixerTransitionAsset.UnShared movementStopMixer {
            get {
                UpdateMovementStopMixer();
                return _movementStopMixer;
            }
        }

        public AnimationClip fallAnimation {
            get {
                UpdateFallAnimation();
                return _fallAnimation;
            }
        }

        public AnimationClip landAnimation {
            get {
                UpdateLandAnimation();
                return _landAnimation;
            }
        }


        protected AnimancerState idleState {
            get {
                _idleState ??= _layer.GetOrCreateState(entity.character.data.GetAnimation("Idle"));
                return _idleState;
            }
        }

        private void UpdateMovementStartMixer(bool forceReload = false) {
            _movementStartMixer ??= new LinearMixerTransitionAsset.UnShared();
            if ( forceReload || !_movementStartMixer.HasAsset ) {
                _movementStartMixer.Asset = entity.character.data.GetTransition("MovementStart") as LinearMixerTransitionAsset;
                _movementStartMixer.Events.OnEnd = () => {
                    MovementCycleAnimation();
                };
            }
        }

        private void UpdateMovementMixer(bool forceReload = false) {
            _movementMixer ??= new LinearMixerTransitionAsset.UnShared();
            if ( forceReload || !_movementMixer.HasAsset) {
                _movementMixer.Asset = entity.character.data.GetTransition("MovementCycle") as LinearMixerTransitionAsset;
            }
        }

        private void UpdateMovementStopMixer(bool forceReload = false) {
            _movementStopMixer ??= new LinearMixerTransitionAsset.UnShared();
            if ( forceReload || !_movementStopMixer.HasAsset) {
                _movementStopMixer.Asset = entity.character.data.GetTransition("MovementStop") as LinearMixerTransitionAsset;
                _movementStopMixer.Events.OnEnd = () => {
                    IdleAnimation();
                };
            }
        }

        private void UpdateFallAnimation(bool forceReload = false) {
            if ( forceReload || _fallAnimation == null ) {
                _fallAnimation = entity.character.data.GetAnimation("Fall");
            }
        }

        private void UpdateLandAnimation(bool forceReload = false) {
            if ( forceReload || _landAnimation == null ) {
                _landAnimation = entity.character.data.GetAnimation("Land");
            }
        }

        private void UpdateAnimations() {

            UpdateFallAnimation(true);
            UpdateLandAnimation(true);
            UpdateMovementStartMixer(true);
            UpdateMovementMixer(true);
            UpdateMovementStopMixer(true);

        }

        private void DefaultAnimationState() {

            if (entity.onGround) {

                if (movementSpeed == Entity.MovementSpeed.Idle) {
                    IdleAnimation();
                } else {
                    MovementStartAnimation(movementSpeed);
                }
            } else {
                _layer.Play(fallAnimation, 0.3f);
            }
        }

        private void MovementStartAnimation(Entity.MovementSpeed speed) {
            _layer.Play(movementStartMixer);
            _movementStartMixer.State.Parameter = (float)speed;
        }

        private void MovementCycleAnimation() {
            _layer.Play(movementMixer);
            _movementMixer.State.Parameter = (float)movementSpeed;
        }

        private void MovementStopAnimation() {

            if (movementSpeed == Entity.MovementSpeed.Idle) return;

            _layer.Play(movementStopMixer);
            _movementStopMixer.State.Parameter = (float)movementSpeed;

        }

        private void IdleAnimation() {
            _layer.Play(idleState, 0.15f);
        }




        private void Animation(){

            movementSpeedWeight = Mathf.MoveTowards(movementSpeedWeight, (float)movementSpeed, 5f * GameUtility.timeDelta);

            if ( movementMixer.State != null )
                _movementMixer.State.Parameter = movementSpeedWeight;

            if (!_layer.IsAnyStatePlaying()) {
                DefaultAnimationState();
            }

            if ( entity.onGround.started ) {
                AnimancerState landState = _layer.Play(landAnimation, 0.1f);
                landState.Events.OnEnd = () => {
                    DefaultAnimationState();
                };
            }
        }
    }
}
