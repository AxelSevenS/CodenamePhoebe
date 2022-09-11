using System;

using SevenGame.Utility;

namespace SeleneGame.States {

    public static class StateConstructor {

        
        public static SeleneGame.Core.State CreateState(Type type){
            return CreateState(type.Name);
        }

        public static SeleneGame.Core.State CreateState(string type){
            switch (type) {
                default: return new SeleneGame.Core.WalkingState();
                case "SwimmingState": return new SeleneGame.Core.SwimmingState();
                case "MaskedState": return new SeleneGame.States.MaskedState();
                case "SittingState": return new SeleneGame.States.SittingState();
                case "VehicleState": return new SeleneGame.States.VehicleState();
            }
        }
    }
}
