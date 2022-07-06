using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SeleneGame.Core;
using SeleneGame.Entities;

namespace SeleneGame.Core {
    
    public class Grabbable : MonoBehaviour, IInteractable{

        private Rigidbody rb;
        private Collider collider;
        
        private float holdDistance;
        private bool grabbed;
        private bool inFront;
        private Vector3 randomSpin;

        void Awake(){
            rb = GetComponent<Rigidbody>();
            collider = GetComponent<Collider>();
            Global.SetLayerRecursively(gameObject, 6);
        }

        void FixedUpdate() {
            collider.enabled = !grabbed;

            if (!grabbed) return;

            rb.AddTorque(randomSpin.x, randomSpin.y, randomSpin.z, ForceMode.VelocityChange);

        }
        
        public string interactionDescription {
            get => "Grab";
            set{;}
        }

        public void Interact(Entity entity){
            if (entity is GravityShifterEntity shifter)
                Grab(shifter);
        }

        public void Grab(GravityShifterEntity entity){
            if (entity.grabbedObjects.Count >= 4) return;

            grabbed = true;

            entity.grabbedObjects.Add(this);
            randomSpin = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized * 0.3f ;
        }

        public void Throw(GravityShifterEntity entity){
            
            if ( !entity.grabbedObjects.Contains(this) ) return;

            grabbed = false;

            entity.grabbedObjects.Remove(this);
            rb.AddForce(entity.cameraRotation*Vector3.forward*30f*rb.mass, ForceMode.Impulse);


            var impulseParticle = Instantiate(Global.LoadParticle("ShiftImpulseParticles"), entity.transform.position + entity.cameraRotation * Vector3.forward*2f, entity.cameraRotation);
            Destroy(impulseParticle, 1.2f);
        }

    }
}