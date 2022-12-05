using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SeleneGame.Core {

    [System.Serializable]
    public abstract class VehicleCharacter : Character {

        public Seat.SittingPose[] sittingPoses;

    }
}
