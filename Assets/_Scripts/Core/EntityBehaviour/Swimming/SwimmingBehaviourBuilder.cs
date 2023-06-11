using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SeleneGame.Core {

    public class SwimmingBehaviourBuilder : EntityBehaviourBuilder<SwimmingBehaviour> {

        public readonly static SwimmingBehaviourBuilder Default = new SwimmingBehaviourBuilder();

        private SwimmingBehaviourBuilder() { }
        
        public override SwimmingBehaviour Build(Entity entity, EntityBehaviour previousBehaviour, GameObject gameObject) {

            bool wasEnabled = gameObject.activeSelf;

            gameObject.SetActive(false);

            SwimmingBehaviour behaviour = gameObject.AddComponent<SwimmingBehaviour>();
            behaviour.Initialize(entity, previousBehaviour);

            gameObject.SetActive(wasEnabled);

            return behaviour;
        }
    }
}
