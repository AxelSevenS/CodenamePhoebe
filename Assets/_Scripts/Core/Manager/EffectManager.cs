using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SevenGame.Utility;

namespace SeleneGame.Core {
    public class EffectManager : Singleton<EffectManager> {
        
        private void OnEnable(){
            SetCurrent();
        }
    }
}
