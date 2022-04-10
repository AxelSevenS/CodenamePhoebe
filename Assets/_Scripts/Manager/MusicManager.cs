using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SeleneGame {

    public class MusicManager : MonoBehaviour{
        
        public static MusicManager current;
        
        void OnEnable(){
            if (current != null)
                Destroy(current);
            current = this;
        }
        
        // Start is called before the first frame update
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {
            
        }
    }
}