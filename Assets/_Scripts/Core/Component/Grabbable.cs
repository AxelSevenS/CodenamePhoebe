using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SevenGame.Utility;

namespace SeleneGame.Core {
    
    public class Grabbable : MonoBehaviour, IInteractable{

        public Rigidbody rb;
        private new Collider collider;
        
        // private float holdDistance;
        public bool grabbed;

        private void Awake(){
            rb = GetComponent<Rigidbody>();
            collider = GetComponent<Collider>();
            // GameUtility.SetLayerRecursively(gameObject, 6);
        }

        private void FixedUpdate() {
            collider.enabled = !grabbed;

            if (!grabbed) return;

        }
        
        public bool IsInteractable() => !grabbed;
        public string InteractDescription() => "Pick up";
        public void Interact(Entity entity) => entity.Grab(this);

    }
}