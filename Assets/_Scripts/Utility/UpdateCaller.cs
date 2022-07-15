using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SevenGame.Utility {
    public class UpdateCaller : Singleton<UpdateCaller> {

        public static event System.Action onUpdate;
        public static event System.Action onLateUpdate;
        public static event System.Action onFixedUpdate;

        private void OnEnable() {
            SetCurrent();
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
