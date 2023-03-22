using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SevenGame.Utility;

namespace SeleneGame.Core {
    
    [System.Serializable]
    public class Grabbable : MonoBehaviour, IGrabbable {

        public Rigidbody rb;
        private new Collider collider;
        
        public bool grabbed;



        public bool isGrabbable => !grabbed;
        public Transform grabTransform => transform;



        public void Grab(){
            grabbed = true;
            rb.isKinematic = true;
            collider.enabled = false;
        }

        public void Throw() {
            grabbed = false;
            rb.isKinematic = false;
            collider.enabled = true;
        }
        public void Throw(Vector3 direction) {
            Throw();
            rb.AddForce(direction, ForceMode.VelocityChange);
        }


        private void OnDisable() {
            if (grabbed)
                Throw();
        }


        private void Awake(){
            rb = GetComponent<Rigidbody>();
            collider = GetComponent<Collider>();
        }

    }
}