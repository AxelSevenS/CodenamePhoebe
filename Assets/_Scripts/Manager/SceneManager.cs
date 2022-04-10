using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

namespace SeleneGame {
    
    public class SceneManager : MonoBehaviour{

        public static SceneManager current;
        
        void OnEnable(){
            if (current != null)
                Destroy(current);
            current = this;
        }

        void Start(){
            
        }
        void Update(){
            
        }
    }
}