using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SeleneGame.Core {
    
    public class GameController : MonoBehaviour {

        private void Awake(){
            DontDestroyOnLoad(this.gameObject); 
        }

    }
}
