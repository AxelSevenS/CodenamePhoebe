using System;
using UnityEngine;

using SevenGame.Utility;

namespace SeleneGame.Core {
    
    [System.Serializable]
    [DefaultExecutionOrder(10)]
    public abstract class EntityBehaviour : MonoBehaviour {

        [SerializeField] protected Entity _entity;

        [SerializeField] [ReadOnly] protected EntityMoveBehaviour _moveBehaviour;
        [SerializeField] [ReadOnly] protected EntityEvadeBehaviour _evadeBehaviour;
        [SerializeField] [ReadOnly] protected EntityJumpBehaviour _jumpBehaviour;


        public Entity Entity => _entity;

        public EntityMoveBehaviour MoveBehaviour => _moveBehaviour;
        public EntityEvadeBehaviour EvadeBehaviour => _evadeBehaviour;
        public EntityJumpBehaviour JumpBehaviour => _jumpBehaviour;


        public virtual float GravityMultiplier => 1f;
        public virtual CameraController.CameraStyle CameraType => CameraController.CameraStyle.ThirdPerson;
        

        public void SetMoveBehaviour<T, TPreviousBehaviour>(EntityMoveBehaviour.Builder<T> stateBuilder, TPreviousBehaviour previousBehaviour = null) where T : EntityMoveBehaviour where TPreviousBehaviour : EntityMoveBehaviour {
            if ( stateBuilder == null )
                throw new ArgumentNullException( nameof(stateBuilder) );

            if ( _moveBehaviour is T )
                return;

            try {
                EntityMoveBehaviour newBehaviour = stateBuilder.Build(_entity, this, previousBehaviour ?? _moveBehaviour);
                GameUtility.SafeDestroy( _moveBehaviour );
                _moveBehaviour = newBehaviour;
            } catch ( Exception e ) {
                Debug.LogError( e );
            }
        }
        public void SetEvadeBehaviour<T, TPreviousBehaviour>(EntityEvadeBehaviour.Builder<T> stateBuilder, TPreviousBehaviour previousBehaviour = null) where T : EntityEvadeBehaviour where TPreviousBehaviour : EntityEvadeBehaviour {
            if ( stateBuilder == null )
                throw new ArgumentNullException( nameof(stateBuilder) );

            if ( _evadeBehaviour is T )
                return;

            try {
                EntityEvadeBehaviour newBehaviour = stateBuilder.Build(_entity, this, previousBehaviour ?? _evadeBehaviour);
                GameUtility.SafeDestroy( _evadeBehaviour );
                _evadeBehaviour = newBehaviour;
            } catch ( Exception e ) {
                Debug.LogError( e );
            }
        }
        public void SetJumpBehaviour<T, TPreviousBehaviour>(EntityJumpBehaviour.Builder<T> stateBuilder, TPreviousBehaviour previousBehaviour = null) where T : EntityJumpBehaviour where TPreviousBehaviour : EntityJumpBehaviour {
            if ( stateBuilder == null )
                throw new ArgumentNullException( nameof(stateBuilder) );

            if ( _jumpBehaviour is T )
                return;

            try {
                EntityJumpBehaviour newBehaviour = stateBuilder.Build(_entity, this, previousBehaviour ?? _jumpBehaviour);
                GameUtility.SafeDestroy( _jumpBehaviour );
                _jumpBehaviour = newBehaviour;
            } catch ( Exception e ) {
                Debug.LogError( e );
            }
        }

        protected internal virtual void HandleInput(Player controller) {
            _moveBehaviour?.HandleInput(controller);
            _jumpBehaviour?.HandleInput(controller);
            _evadeBehaviour?.HandleInput(controller);
        }

        protected internal virtual void HandleAI(AIController controller) {;}


        protected internal virtual void SetSpeed(Entity.MovementSpeed speed) {
            if (_moveBehaviour == null) return;

            _moveBehaviour.SetSpeed(speed);
        }
        protected internal virtual void Move(Vector3 direction = default) {
            if (_moveBehaviour == null) return;

            if ( _moveBehaviour.CanMove )
                _moveBehaviour.Move(direction);
        }
        protected internal virtual void Jump(Vector3 direction = default) {
            if (_jumpBehaviour == null) return;

            if ( _jumpBehaviour.CanJump )
                _jumpBehaviour.Jump(direction);
        }
        protected internal virtual void Evade(Vector3 direction = default) {
            if (_evadeBehaviour == null) return;

            if ( _evadeBehaviour.CanEvade )
                _evadeBehaviour.Evade(direction);
        }
        protected internal virtual void LightAttack() {
            if (true)
                LightAttackAction();
        }
        protected internal virtual void HeavyAttack() {
            if (true)
                HeavyAttackAction();
        }


        protected virtual void LightAttackAction() {;}
        protected virtual void HeavyAttackAction() {;}


        protected virtual void OnDestroy() {
            GameUtility.SafeDestroy(_moveBehaviour);
            GameUtility.SafeDestroy(_jumpBehaviour);
            GameUtility.SafeDestroy(_evadeBehaviour);
        }



        public abstract class Builder<TBehaviour> where TBehaviour : EntityBehaviour {
            
            public virtual TBehaviour Build(Entity entity, EntityBehaviour previousBehaviour) {
                using Context process = new(entity, previousBehaviour);
                ConfigureBehaviour(process);
                return process.Behaviour;
            }

            protected virtual void ConfigureBehaviour(Context context) {;}


            protected readonly struct Context : IDisposable {

                private readonly bool wasEnabled;
                private readonly Entity _entity;
                private readonly TBehaviour _behaviour;
                private readonly EntityBehaviour _previousBehaviour;

                public Entity Entity => _entity;
                public TBehaviour Behaviour => _behaviour;
                public EntityBehaviour PreviousBehaviour => _previousBehaviour;


                public Context(Entity entity, EntityBehaviour previousBehaviour){
                    _entity = entity;
                    _previousBehaviour = previousBehaviour;

                    wasEnabled = entity.gameObject.activeSelf;
                    entity.gameObject.SetActive(false);
                    _behaviour = entity.gameObject.AddComponent<TBehaviour>();
                    _behaviour._entity = entity;
                }

                public void Dispose() {
                    _entity.gameObject.SetActive(wasEnabled);
                }
            }
        }

    }
}