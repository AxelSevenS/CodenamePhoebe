using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SeleneGame.Core {

    [System.Serializable]
    public abstract class BehaviourStrategy : MonoBehaviour {


        private Entity _entity;


        public Entity entity {
            get {
                if (_entity == null)
                    _entity = GetComponent<Entity>();
                return _entity;
            }
            private set => _entity = value;
        }

        protected internal abstract void HandleInput(PlayerEntityController contoller);
        
    }
}
