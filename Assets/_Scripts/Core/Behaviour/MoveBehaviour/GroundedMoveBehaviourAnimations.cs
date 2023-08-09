using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Animancer;

using SevenGame.Utility;

namespace SeleneGame.Core {

    public partial class GroundedMoveBehaviour {

        private float movementSpeedWeight;
        [SerializeField] [HideInInspector] private LinearMixerTransitionAsset.UnShared _movementStartMixer;
        [SerializeField] [HideInInspector] private LinearMixerTransitionAsset.UnShared _movementMixer;
        [SerializeField] [HideInInspector] private LinearMixerTransitionAsset.UnShared _movementStopMixer;
        [SerializeField] [HideInInspector] private AnimancerState _idleState;

        private AnimationClip _fallAnimation;
        private AnimationClip _landAnimation;


        private AnimancerLayer AnimationLayer => Entity.Animancer.Layers[0];

        public LinearMixerTransitionAsset.UnShared MovementStartMixer {
            get {
                UpdateMovementStartMixer();
                return _movementStartMixer;
            }
        }

        public LinearMixerTransitionAsset.UnShared MovementMixer {
            get {
                UpdateMovementMixer();
                return _movementMixer;
            }
        }

        public LinearMixerTransitionAsset.UnShared MovementStopMixer {
            get {
                UpdateMovementStopMixer();
                return _movementStopMixer;
            }
        }

        public AnimationClip FallAnimation {
            get {
                UpdateFallAnimation();
                return _fallAnimation;
            }
        }

        public AnimationClip LandAnimation {
            get {
                UpdateLandAnimation();
                return _landAnimation;
            }
        }


        protected AnimancerState IdleState {
            get {
                _idleState ??= AnimationLayer.GetOrCreateState(Entity.Character.Data.GetAnimation("Idle"));
                return _idleState;
            }
        }

        private void UpdateMovementStartMixer(bool forceReload = false) {
            _movementStartMixer ??= new LinearMixerTransitionAsset.UnShared();
            if ( forceReload || !_movementStartMixer.HasAsset ) {
                _movementStartMixer.Asset = Entity.Character.Data.GetTransition("MovementStart") as LinearMixerTransitionAsset;
                _movementStartMixer.Events.OnEnd = () => {
                    MovementCycleAnimation();
                };
            }
        }

        private void UpdateMovementMixer(bool forceReload = false) {
            _movementMixer ??= new LinearMixerTransitionAsset.UnShared();
            if ( forceReload || !_movementMixer.HasAsset) {
                _movementMixer.Asset = Entity.Character.Data.GetTransition("MovementCycle") as LinearMixerTransitionAsset;
            }
        }

        private void UpdateMovementStopMixer(bool forceReload = false) {
            _movementStopMixer ??= new LinearMixerTransitionAsset.UnShared();
            if ( forceReload || !_movementStopMixer.HasAsset) {
                _movementStopMixer.Asset = Entity.Character.Data.GetTransition("MovementStop") as LinearMixerTransitionAsset;
                _movementStopMixer.Events.OnEnd = () => {
                    IdleAnimation();
                };
            }
        }

        private void UpdateFallAnimation(bool forceReload = false) {
            if ( forceReload || _fallAnimation == null ) {
                _fallAnimation = Entity.Character.Data.GetAnimation("Fall");
            }
        }

        private void UpdateLandAnimation(bool forceReload = false) {
            if ( forceReload || _landAnimation == null ) {
                _landAnimation = Entity.Character.Data.GetAnimation("Land");
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

            if (Entity.onGround) {

                if (_movementSpeed == Entity.MovementSpeed.Idle) {
                    IdleAnimation();
                } else {
                    MovementStartAnimation(_movementSpeed);
                }
            } else {
                AnimationLayer.Play(FallAnimation, 0.3f);
            }
        }

        private void MovementStartAnimation(Entity.MovementSpeed speed) {
            AnimationLayer.Play(MovementStartMixer);
            _movementStartMixer.State.Parameter = (float)speed;
        }

        private void MovementCycleAnimation() {
            AnimationLayer.Play(MovementMixer);
            _movementMixer.State.Parameter = (float)_movementSpeed;
        }

        private void MovementStopAnimation() {

            if (_movementSpeed == Entity.MovementSpeed.Idle) return;

            AnimationLayer.Play(MovementStopMixer);
            _movementStopMixer.State.Parameter = (float)_movementSpeed;

        }

        private void IdleAnimation() {
            AnimationLayer.Play(IdleState, 0.15f);
        }




        private void Animation(){

            movementSpeedWeight = Mathf.MoveTowards(movementSpeedWeight, (float)_movementSpeed, 5f * GameUtility.timeDelta);

            if ( MovementMixer.State != null )
                _movementMixer.State.Parameter = movementSpeedWeight;

            if (!AnimationLayer.IsAnyStatePlaying()) {
                DefaultAnimationState();
            }

            if ( Entity.onGround.started ) {
                AnimancerState landState = AnimationLayer.Play(LandAnimation, 0.1f);
                landState.Events.OnEnd = () => {
                    DefaultAnimationState();
                };
            }
        }
    }
}
