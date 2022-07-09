using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SeleneGame.Core;
// using SeleneGame.Entities;
using SeleneGame.Utility;

namespace SeleneGame.Core {
    
    public class Grabbable : MonoBehaviour, IInteractable{

        public Rigidbody rb;
        private Collider collider;
        
        // private float holdDistance;
        public bool grabbed;

        void Awake(){
            rb = GetComponent<Rigidbody>();
            collider = GetComponent<Collider>();
            // GameUtility.SetLayerRecursively(gameObject, 6);
        }

        void FixedUpdate() {
            collider.enabled = !grabbed;

            if (!grabbed) return;

        }
        
        public string interactionDescription {
            get => "Grab";
            set{;}
        }

        public void Interact(Entity entity){
            entity.Grab(this);
        }

    }
}