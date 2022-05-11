using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SeleneGame.Core {
    
    public class GameController : MonoBehaviour {

        public static event System.Action onUpdate;
        public static event System.Action onLateUpdate;
        public static event System.Action onFixedUpdate;

        private void Awake(){
            DontDestroyOnLoad(this.gameObject); 
        }
        private void Update(){
            onUpdate?.Invoke();
        }
        private void LateUpdate(){
            onLateUpdate?.Invoke();
        }
        private void FixedUpdate(){
            onFixedUpdate?.Invoke();
        }

    }
}
