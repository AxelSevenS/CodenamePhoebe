using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SevenGame.Utility;

namespace SeleneGame.Core {
    
    public class GameController : Singleton<GameController> {

        private void OnEnable() {
            SetCurrent();
        }
    }
}
