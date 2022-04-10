using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SeleneGame {

    public class GravityBikeEntity : Entity {
        
        protected override void EntityAwake(){
            SetState("Vehicle");
        }
    }
}