using System;
using UnityEngine;

using SevenGame.Utility;

namespace SeleneGame.Core {
    
    [System.Serializable]
    public abstract class EntityBehaviour : BehaviourStrategy {

        [SerializeReference] [ReadOnly] protected EvadeBehaviour _evadeBehaviour;
        [SerializeReference] [ReadOnly] protected JumpBehaviour _jumpBehaviour;


        public EvadeBehaviour evadeBehaviour => _evadeBehaviour;
        public JumpBehaviour jumpBehaviour => _jumpBehaviour;

        public virtual float gravityMultiplier => 1f;
        public virtual CameraController.CameraType cameraType => CameraController.CameraType.ThirdPerson;
        

        public abstract Vector3 direction {get;}
        public abstract float speed {get;}

        protected virtual Vector3 jumpDirection => -entity.gravityDown;

        protected virtual Vector3 evadeDirection => entity.absoluteForward;

        protected virtual bool canParry => true;

        

        protected EntityBehaviour(Entity entity, EntityBehaviour previousBehaviour) : base(entity) {;}

        protected override void DisposeBehavior() {
            base.DisposeBehavior();
            _jumpBehaviour?.Dispose();
            _evadeBehaviour?.Dispose();
        }

        protected internal override void HandleInput(PlayerEntityController controller) {
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
        protected internal abstract void SetSpeed(Entity.MovementSpeed speed);


        protected virtual void LightAttackAction() {;}
        protected virtual void HeavyAttackAction() {;}


        public override void Update() {
            base.Update();

            _jumpBehaviour?.Update();
            _evadeBehaviour?.Update();
        }
    }
}