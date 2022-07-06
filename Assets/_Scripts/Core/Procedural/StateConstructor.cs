using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SeleneGame.Utility {

    public static class StateConstructor {
        
        public static SeleneGame.Core.State CreateState(string type){
            switch (type) {
                default: return new SeleneGame.States.WalkingState();
                case "ShiftingState": return new SeleneGame.States.ShiftingState();
                case "SittingState": return new SeleneGame.States.SittingState();
                case "SwimmingState": return new SeleneGame.States.SwimmingState();
                case "VehicleState": return new SeleneGame.States.VehicleState();
            }
        }
    }
}
