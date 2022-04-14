using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SeleneGame.Core;

namespace SeleneGame.Entities {

    public class GravityBikeEntity : Entity {
        
        protected override void EntityAwake(){
            SetState("Vehicle");
        }
    }
}