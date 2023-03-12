using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SeleneGame.Core;

namespace SeleneGame {
    
    public class Boat : MonoBehaviour {

        [SerializeField] private GameObject Sail;
        [SerializeField] private List<GameObject> floaters = new List<GameObject>();
        [SerializeField] private Rigidbody _rigidbody;
        
        void OnEnable() => ObjectManager.current.objects.Add( this.gameObject );
        void OnDisable() => ObjectManager.current.objects.Remove( this.gameObject );

        void Awake(){
            
            if (Sail != null){
                var sailC = Sail.AddComponent<Sail>();
                sailC.rigidbody = _rigidbody;
            }
            foreach (GameObject floaterObject in floaters){
                var floater = floaterObject.AddComponent<Floater>();
                floater.rigidbody = _rigidbody;
                floater.floaterCount = floaters.Count;
            }
        }
    }
}