using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SeleneGame.Core {

    public partial class Entity {


        public virtual void HandleInput(EntityController controller) {
            state?.HandleInput(controller);
        }


        public virtual void Move(Vector3 direction) {
            state?.Move(direction);
        }
        public virtual void Jump() {
            state?.Jump();
        }
        public virtual void Evade(Vector3 direction) {
            state?.Evade(direction);
        }
        public virtual void Parry() {
            state?.Parry();
        }
        public virtual void LightAttack() {
            state?.LightAttack();
        }
        public virtual void HeavyAttack() {
            state?.HeavyAttack();
        }
        public virtual void SetSpeed(MovementSpeed speed) {
            state?.SetSpeed(speed);
        }

        public enum MovementSpeed {
            Slow,
            Normal,
            Fast
        }
        
    }
}
