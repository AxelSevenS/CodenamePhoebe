using System;

using SevenGame.Utility;

namespace SeleneGame.States {

    public static class StateConstructor {

        
        public static SeleneGame.Core.State CreateState(Type type){
            return CreateState(type.Name);
        }

        public static SeleneGame.Core.State CreateState(string type){
            switch (type) {
                default: return new SeleneGame.States.WalkingState();
            }
        }
    }
}
