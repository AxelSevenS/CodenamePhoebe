using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

using SevenGame.Utility;

namespace SeleneGame.Core {
    
    public class SceneManager : Singleton<SceneManager> {
        private void OnEnable() {
            SetCurrent();
        }
    }
}