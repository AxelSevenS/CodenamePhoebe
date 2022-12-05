using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SeleneGame.Core {

    [System.Serializable]
    public abstract class BehaviourStrategy {

        [SerializeReference] [HideInInspector] protected State entityState;


        protected Entity entity => entityState?.entity;


        public BehaviourStrategy(State entityState) {
            this.entityState = entityState;
        }


        protected internal abstract void HandleInput(PlayerEntityController contoller);

        protected internal abstract void Update();

        protected internal abstract void FixedUpdate();
        
    }
}
