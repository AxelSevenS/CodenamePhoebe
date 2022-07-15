using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SeleneGame.Core;

namespace SeleneGame {
    
    public class Boat : MonoBehaviour {

        [SerializeField] private GameObject Sail;
        [SerializeField] private List<GameObject> floaters = new List<GameObject>();
        
        void OnEnable() => ObjectManager.current.objects.Add( this.gameObject );
        void OnDisable() => ObjectManager.current.objects.Remove( this.gameObject );

        void Awake(){
            if (Sail != null){
                var sailC = Sail.AddComponent<Sail>();
                sailC.rigidbody = GetComponent<Rigidbody>();
            }
            foreach (GameObject floater in floaters){
                var floaterC = floater.AddComponent<Floater>();
                floaterC.rigidbody = GetComponent<Rigidbody>();
                floaterC.floaterCount = floaters.Count;
            }
        }
    }
}