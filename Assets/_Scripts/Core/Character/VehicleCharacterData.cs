using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SeleneGame.Core {

    [System.Serializable]
    [CreateAssetMenu(fileName = "Vehicle Character Data", menuName = "Data/Vehicle Character", order = 1)]
    public class VehicleCharacterData : CharacterData {
        
        public Seat.SittingPose[] sittingPoses;

        public override Character GetCharacter(Entity entity, CharacterCostume costume = null) { 
            return new VehicleCharacter(entity, this, costume ?? baseCostume ?? AddressablesUtils.GetDefaultAsset<CharacterCostume>());
        }

    }
}
