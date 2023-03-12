using System;
using UnityEngine;

using SevenGame.Utility;

namespace SeleneGame.Core {
    
    [System.Serializable]
    [RequireComponent(typeof(Entity))]
    public abstract class State : MonoBehaviour {

        private Entity _entity;

        [SerializeReference] [ReadOnly] protected EvadeBehaviour _evadeBehaviour;
        [SerializeReference] [ReadOnly] protected JumpBehaviour _jumpBehaviour; 


        public EvadeBehaviour evadeBehaviour => _evadeBehaviour;
        public JumpBehaviour jumpBehaviour => _jumpBehaviour;

        public virtual float gravityMultiplier => 1f;
        public virtual CameraController.CameraType cameraType => CameraController.CameraType.ThirdPerson;

        public Entity entity {
            get {
                if (_entity == null)
                    _entity = GetComponent<Entity>();
                return _entity;
            }
            private set => _entity = value;
        }

        protected virtual Vector3 jumpDirection => -entity.gravityDown;

        protected virtual Vector3 evadeDirection => entity.absoluteForward;

        protected virtual bool canParry => true;


        public abstract void Transition(Vector3 direction = default, float speed = 0f);
        public abstract void GetTransitionData(out Vector3 direction, out float speed);
        


        protected internal virtual void HandleInput(PlayerEntityController controller) {
            _jumpBehaviour?.HandleInput(controller);
            _evadeBehaviour?.HandleInput(controller);
        }


        protected internal abstract void Move(Vector3 direction);
        protected internal virtual void Jump() {
            if (_jumpBehaviour == null) return;

            if ( _jumpBehaviour.canJump )
                _jumpBehaviour.Jump(jumpDirection);
        }
        protected internal virtual void Evade(Vector3 direction) {
            if (_evadeBehaviour == null) return;

            if ( _evadeBehaviour.canEvade )
                _evadeBehaviour.Evade(evadeDirection);
        }
        protected internal virtual void LightAttack() {
            if (true)
                LightAttackAction();
        }
        protected internal virtual void HeavyAttack() {
            if (true)
                HeavyAttackAction();
        }
        protected internal virtual void Parry() {
            if (canParry)
                ParryAction();
        }
        protected internal abstract void SetSpeed(Entity.MovementSpeed speed);


        protected virtual void LightAttackAction() {;}
        protected virtual void HeavyAttackAction() {;}
        protected virtual void ParryAction() {;}


        protected internal virtual void Awake() {;}

        protected internal virtual void OnDestroy(){
            GameObject.Destroy(evadeBehaviour);
            GameObject.Destroy(jumpBehaviour);
        }

        protected internal virtual void Animation(){;}

    }
}