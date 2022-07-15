using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using SevenGame.Utility;

namespace SeleneGame.Core {

    public class ObjectManager : Singleton<ObjectManager> {
        private void OnEnable() {
            SetCurrent();
        }
        
        public List<GameObject> objects = new List<GameObject>();

        public void DisableAllObjects(){
            /* foreach (GameObject obj in objects){
                obj.SetActive(false);
            } */
        }
    }
}