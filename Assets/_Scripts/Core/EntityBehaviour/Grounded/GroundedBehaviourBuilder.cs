using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SeleneGame.Core {
    public class GroundedBehaviourBuilder : EntityBehaviourBuilder<GroundedBehaviour> {

        public readonly static GroundedBehaviourBuilder Default = new GroundedBehaviourBuilder();

        private GroundedBehaviourBuilder() { }

        public override GroundedBehaviour Build(Entity entity, EntityBehaviour previousBehaviour, GameObject gameObject) {

            bool wasEnabled = gameObject.activeSelf;
            gameObject.SetActive(false);

            Type type = entity is ArmedEntity ? typeof(ArmedGroundedBehaviour) : typeof(GroundedBehaviour);
            GroundedBehaviour behaviour = gameObject.AddComponent(type) as GroundedBehaviour;
            behaviour.Initialize(entity, previousBehaviour);

            gameObject.SetActive(wasEnabled);


            return behaviour;
        }
    }
}
