using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SeleneGame.Core;

namespace SeleneGame.Content {

    public class VehicleBehaviourBuilder : EntityBehaviourBuilder<VehicleBehaviour> {

        public readonly static VehicleBehaviourBuilder Default = new VehicleBehaviourBuilder();

        private VehicleBehaviourBuilder() { }

        public override VehicleBehaviour Build(Entity entity, EntityBehaviour previousBehaviour) {
            return new VehicleBehaviour(entity, previousBehaviour);
        }
    }

}

