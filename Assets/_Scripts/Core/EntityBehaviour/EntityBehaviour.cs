using System;
using UnityEngine;

using SevenGame.Utility;

namespace SeleneGame.Core {
    
    [System.Serializable]
    [DefaultExecutionOrder(10)]
    public abstract class EntityBehaviour : BehaviourStrategy {

        [SerializeField] [ReadOnly] protected EvadeBehaviour _evadeBehaviour;
        [SerializeField] [ReadOnly] protected JumpBehaviour _jumpBehaviour;


        public EvadeBehaviour evadeBehaviour => _evadeBehaviour;
        public JumpBehaviour jumpBehaviour => _jumpBehaviour;

        public virtual float gravityMultiplier => 1f;
        public virtual CameraController.CameraType cameraType => CameraController.CameraType.ThirdPerson;
        

        public abstract Vector3 direction {get;}
        public abstract float speed {get;}

        protected virtual Vector3 jumpDirection => -entity.gravityDown;

        protected virtual Vector3 evadeDirection => entity.absoluteForward;

        protected virtual bool canParry => true;

        

        public abstract void Initialize(Entity entity, EntityBehaviour previousBehaviour = null);

        protected internal override void HandleInput(Player controller) {
            _jumpBehaviour?.HandleInput(controller);
            _evadeBehaviour?.HandleInput(controller);
        }

        protected internal virtual void HandleAI(AIController controller) {
            
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
        protected internal abstract void SetSpeed(Entity.MovementSpeed speed);


        protected virtual void LightAttackAction() {;}
        protected virtual void HeavyAttackAction() {;}

        protected virtual void OnDestroy() {
            GameUtility.SafeDestroy(_jumpBehaviour);
            GameUtility.SafeDestroy(_evadeBehaviour);
        } 

    }
}