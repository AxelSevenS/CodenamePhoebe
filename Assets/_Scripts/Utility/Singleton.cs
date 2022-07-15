using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SevenGame.Utility {
    public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour {
        
        public static T current;

        protected void SetCurrent() {
            if (current != null && current != this)
                Destroy(current);
            current = this as T;
        }
    }
}
