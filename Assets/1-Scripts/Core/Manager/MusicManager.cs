using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SevenGame.Utility;

namespace SeleneGame.Core {

    public class MusicManager : Singleton<MusicManager> {
        
        private void OnEnable() {
            SetCurrent();
        }
    }
}