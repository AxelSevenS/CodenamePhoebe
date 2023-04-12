using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SevenGame.Utility;

namespace SeleneGame.Core {

    [DefaultExecutionOrder(-1000)]
    public class EffectManager : Singleton<EffectManager> {
        
        private void OnEnable(){
            SetCurrent();
        }
    }
}
