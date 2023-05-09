using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SeleneGame.Core {

    public class AIController : EntityController {

        private void Update() {
            entity?.HandleAI(this);
        }
    }
}
