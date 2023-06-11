using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SeleneGame.Core {

    public abstract class EntityBehaviourBuilder<T> where T : EntityBehaviour {
        public abstract T Build(Entity entity, EntityBehaviour previousBehaviour, GameObject gameObject);
    }
    
}
