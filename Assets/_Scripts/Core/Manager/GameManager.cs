using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SevenGame.Utility;

namespace SeleneGame.Core {
    
    public class GameManager : Singleton<GameManager> {

        private void OnEnable() {
            SetCurrent();
        }
    }
}
