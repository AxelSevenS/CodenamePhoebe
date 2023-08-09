using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Animancer;

using SevenGame.Utility;

namespace SeleneGame.Core {

    [System.Serializable]
    public abstract class EntityMoveBehaviour : CompositeBehaviour {


        public bool IsIdle => Direction.sqrMagnitude == 0 || Speed == 0;

        public abstract Vector3 Direction {get;}
        public abstract float Speed {get;}
        public abstract bool CanMove {get;}



        protected internal override void HandleInput(Player contoller) {;}

        protected internal virtual void SetSpeed(Entity.MovementSpeed speed) {;}
        protected internal virtual void Move(Vector3 direction) {;}



        public abstract class Builder<TMoveBehaviour> : Builder<TMoveBehaviour, EntityMoveBehaviour> where TMoveBehaviour : EntityMoveBehaviour {

            public override TMoveBehaviour Build(Entity entity, EntityBehaviour parentBehaviour, EntityMoveBehaviour previousBehaviour = null) {
                return base.Build(entity, parentBehaviour, previousBehaviour);
            }

        }
    }
}
