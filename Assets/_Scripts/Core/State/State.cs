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
            name = GetType().Name.Replace("State","");
            this.entity = entity;
        }
        protected internal virtual void OnExit(){;}
        


        public abstract void HandleInput(EntityController controller);


        public abstract void Move(Vector3 direction);
        public virtual void Jump() {
            if (canJump)
                JumpAction(jumpDirection);
        }
        public virtual void Evade(Vector3 direction) {
            if (canEvade)
                EvadeAction(evadeDirection);
        }
        public virtual void Parry() {
            if (canParry)
                ParryAction();
        }
        public virtual void LightAttack() {
            if (true)
                LightAttackAction();
        }
        public virtual void HeavyAttack() {
            if (true)
                HeavyAttackAction();
        }
        public abstract void SetSpeed(MovementSpeed speed);


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


        public enum MovementSpeed {
            Slow,
            Normal,
            Fast
        }
    }
}