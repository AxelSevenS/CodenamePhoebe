using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Animancer;

using SevenGame.Utility;

namespace SeleneGame.Core {

    public partial class Grounded {

        private float movementSpeedWeight;

        [SerializeField] [HideInInspector] private AnimancerLayer _layer;
        [SerializeField] [HideInInspector] private LinearMixerState _movementStartMixer;
        [SerializeField] [HideInInspector] private LinearMixerState _movementMixer;
        [SerializeField] [HideInInspector] private LinearMixerState _movementStopMixer;
        [SerializeField] [HideInInspector] private AnimancerState _idleState;



        protected AnimancerLayer layer {
            get {
                if (_layer == null) {
                    _layer = entity.animancer.Layers[0];
                }
                return _layer;
            }
        }

        protected AnimancerState idleState {
            get {
                if (_idleState == null) {
                    _idleState = layer.GetOrCreateState(entity.character.animations.idleAnimation);
                }
                return _idleState;
            }
        }

        protected LinearMixerState movementStartMixer {
            get {
                if (_movementStartMixer == null) {

                    _movementStartMixer = new LinearMixerState();
                    _movementStartMixer.Initialize(
                        entity.character.animations.moveStartSlowAnimation,
                        entity.character.animations.moveStartAnimation,
                        entity.character.animations.moveStartFastAnimation,
                        1f,
                        2f,
                        3f
                    );

                    layer.AddChild(_movementStartMixer);
                    _movementStartMixer.SetDebugName("Movement Start");
                    _movementStartMixer.Stop();
                }
                return _movementStartMixer;
            }
        }

        protected LinearMixerState movementMixer {
            get {
                if (_movementMixer == null) {

                    _movementMixer = new LinearMixerState();
                    _movementMixer.Initialize(
                        entity.character.animations.moveCycleSlowAnimation,
                        entity.character.animations.moveCycleAnimation,
                        entity.character.animations.moveCycleFastAnimation,
                        1f,
                        2f,
                        3f
                    );

                    layer.AddChild(_movementMixer);
                    _movementMixer.SetDebugName("Movement");
                    _movementMixer.Stop();
                }
                return _movementMixer;
            }
        }

        protected LinearMixerState movementStopMixer {
            get {
                if (_movementStopMixer == null) {

                    _movementStopMixer = new LinearMixerState();
                    _movementStopMixer.Initialize(
                        entity.character.animations.moveStopSlowAnimation,
                        entity.character.animations.moveStopAnimation,
                        entity.character.animations.moveStopFastAnimation,
                        1f,
                        2f,
                        3f
                    );

                    layer.AddChild(_movementStopMixer);
                    _movementStopMixer.SetDebugName("Movement Stop");
                    _movementStopMixer.Stop();
                }
                return _movementStopMixer;
            }
        }




        private void DefaultAnimationState() {

            if (entity.onGround) {
                if (movementSpeed == Entity.MovementSpeed.Idle) {
                    IdleAnimation();
                } else {
                    MovementStartAnimation(movementSpeed);
                }
            } else {
                layer.Play(entity.character?.animations.fallAnimation, 0.3f);
            }
        }

        private void MovementCycleAnimation() {

            movementMixer.Parameter = (float)movementSpeed;
            layer.Play(movementMixer, 0.15f);
        }

        private void MovementStartAnimation(Entity.MovementSpeed speed) {

            // Debug.Log("start");

            movementStartMixer.Parameter = (float)speed;
            layer.Play(movementStartMixer, 0.15f);

            movementStartMixer.Events.OnEnd = () => {
                // movementStartMixer.Stop();
                MovementCycleAnimation();
            };
        }

        private void MovementStopAnimation() {

            if (movementSpeed == Entity.MovementSpeed.Idle) return;

            // Debug.Log("stop");

            movementStopMixer.Parameter = (float)movementSpeed;
            layer.Play(movementStopMixer, 0.15f);

            movementStopMixer.Events.OnEnd = () => {
                // movementStopMixer.Stop();
                IdleAnimation();
            };
        }

        private void IdleAnimation() {

            // Debug.Log("idle");

            layer.Play(idleState, 0.15f);
        }




        protected internal override void StateAnimation(){

            base.StateAnimation();

            movementSpeedWeight = Mathf.MoveTowards(movementSpeedWeight, (float)movementSpeed, 5f * GameUtility.timeDelta);

            movementMixer.Parameter = movementSpeedWeight;

            if (!layer.IsAnyStatePlaying()) {
                DefaultAnimationState();
            }

            // if ( !entity.onGround ) {
            //     movementMixer.Stop();
            //     movementStartMixer.Stop();
            //     movementStopMixer.Stop();

            //     if ( Vector3.Dot(entity.rigidbody.velocity, entity.gravityDown) > 0.2f )
            //         layer.Play(entity.character?.fallAnimation, 0.25f);
            // }

            if ( entity.onGround.started ) {
                AnimancerState landState = layer.Play(entity.character.animations.landAnimation, 0.1f);
                landState.Events.OnEnd = () => {
                    DefaultAnimationState();
                };
            }
        }

    }
}
