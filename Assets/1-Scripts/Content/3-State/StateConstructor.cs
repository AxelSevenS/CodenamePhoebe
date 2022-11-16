using System;

using SevenGame.Utility;

namespace SeleneGame.Content {

    public static class StateConstructor {

        
        public static SeleneGame.Core.State CreateState(Type type){
            return CreateState(type.Name);
        }

        public static SeleneGame.Core.State CreateState(string type){
            switch (type) {
                default: return new SeleneGame.Core.HumanoidGroundedState();
                case "SittingState": return new SeleneGame.Core.SittingState();
                case "SwimmingState": return new SeleneGame.Core.SwimmingState();
                case "MaskedState": return new SeleneGame.Content.MaskedState();
                case "VehicleState": return new SeleneGame.Content.VehicleState();
            }
        }
    }
}
