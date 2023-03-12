using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SeleneGame.Core {

    [System.Serializable]
    public class VehicleCharacter : Character {

        public Seat.SittingPose[] sittingPoses;

        public VehicleCharacter(Entity entity, VehicleCharacterData data, CharacterCostume costume = null) : base(entity, data, costume) {
            this.sittingPoses = data.sittingPoses;
        }
    }
}
