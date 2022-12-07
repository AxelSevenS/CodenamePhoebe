using System;
using UnityEngine;

using SevenGame.Utility;

namespace SeleneGame.Core {
    
    [System.Serializable]
    [RequireComponent(typeof(Entity))]
    public abstract class State : MonoBehaviour {

        private Entity _entity;

        [SerializeReference] [ReadOnly] protected EvadeBehaviour evadeBehaviour;
        [SerializeReference] [ReadOnly] protected JumpBehaviour jumpBehaviour; 



        public virtual float gravityMultiplier => 1f;
        public virtual Vector3 cameraPosition => Global.cameraDefaultPosition;

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
        


        protected internal virtual void HandleInput(PlayerEntityController controller) {
            jumpBehaviour?.HandleInput(controller);
            evadeBehaviour?.HandleInput(controller);
        }


        protected internal abstract void Move(Vector3 direction);
        protected internal virtual void Jump() {
            if (jumpBehaviour == null) return;

            if ( jumpBehaviour.canJump )
                jumpBehaviour.Jump(jumpDirection);
        }
        protected internal virtual void Evade(Vector3 direction) {
            if (evadeBehaviour == null) return;

            if ( evadeBehaviour.canEvade )
                evadeBehaviour.Evade(evadeDirection);
        }
        protected internal virtual void Parry() {
            if (canParry)
                ParryAction();
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


        protected virtual void ParryAction() {;}
        protected virtual void LightAttackAction() {;}
        protected virtual void HeavyAttackAction() {;}


        protected internal virtual void Awake() {;}

        protected internal virtual void OnDestroy(){
            GameObject.Destroy(evadeBehaviour);
            GameObject.Destroy(jumpBehaviour);
        }

        protected internal virtual void StateAnimation(){;}

    }
}