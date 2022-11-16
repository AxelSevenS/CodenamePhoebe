using System;
using UnityEngine;

using SevenGame.Utility;

namespace SeleneGame.Core {
    
    [System.Serializable]
    public abstract class State {


        [ReadOnly] public string name;
        public Entity entity { get; private set; }



        public virtual float gravityMultiplier => 1f;
        public virtual Vector3 cameraPosition => Global.cameraDefaultPosition;


        protected virtual Vector3 jumpDirection => -entity.gravityDown;
        protected virtual bool canJump => entity.onGround.falseTimer < 0.2f;

        protected virtual Vector3 evadeDirection => entity.absoluteForward;
        protected virtual bool canEvade => true;

        protected virtual bool canParry => true;



        protected internal virtual void OnEnter(Entity entity){
            if (this.entity != null) {
                throw new InvalidOperationException("State already has an entity");
            }
            name = GetType().Name.Replace("State","");
            this.entity = entity;
        }
        protected internal virtual void OnExit(){;}
        


        protected internal abstract void HandleInput(EntityController controller);


        protected internal abstract void Move(Vector3 direction);
        protected internal virtual void Jump() {
            if (canJump)
                JumpAction(jumpDirection);
        }
        protected internal virtual void Evade(Vector3 direction) {
            if (canEvade)
                EvadeAction(evadeDirection);
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


        protected virtual void JumpAction(Vector3 jumpDirection) {

            Debug.Log(entity.character.jumpHeight * entity.jumpMultiplier);

            Vector3 newVelocity = entity.rigidbody.velocity.NullifyInDirection( -jumpDirection );
            newVelocity += entity.character.jumpHeight * entity.jumpMultiplier * jumpDirection;
            entity.rigidbody.velocity = newVelocity;

            entity.animator.SetTrigger("Jump");

            // entity.jumpCooldownTimer.SetDuration( 0.4f );
            // onJump?.Invoke(jumpDirection);
        }
        protected virtual void EvadeAction(Vector3 direction) {;}
        protected virtual void ParryAction() {;}
        protected virtual void LightAttackAction() {;}
        protected virtual void HeavyAttackAction() {;}



        protected internal virtual void StateUpdate(){;}
        protected internal virtual void StateFixedUpdate(){;}
        protected internal virtual void StateAnimation(){;}

    }
}