using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SevenGame.Utility;

namespace SeleneGame.Core {
    
    [DefaultExecutionOrder(-1000)]
    public class GameManager : Singleton<GameManager> {

        private void OnEnable() {
            SetCurrent();
        }
    }
}
