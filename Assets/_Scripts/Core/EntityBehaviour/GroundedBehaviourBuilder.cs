using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SeleneGame.Core {
    public class GroundedBehaviourBuilder : EntityBehaviourBuilder<GroundedBehaviour> {

        public readonly static GroundedBehaviourBuilder Default = new GroundedBehaviourBuilder();

        private GroundedBehaviourBuilder() { }

        public override GroundedBehaviour Build(Entity entity, EntityBehaviour previousBehaviour) {
            return new GroundedBehaviour(entity, previousBehaviour);
        }
    }
}
