using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SeleneGame.Core;
using SeleneGame.Entities;

namespace SeleneGame.Core {
    
    public class Grabbable : MonoBehaviour, IInteractable{

        private Rigidbody _rigidbody;
        private Collider _collider;
        
        private float holdDistance;
        private bool _grabbed;
        private bool inFront;
        private Vector3 randomSpin;

        void Awake(){
            _rigidbody = GetComponent<Rigidbody>();
            _collider = GetComponent<Collider>();
        }

        void FixedUpdate() {
            _collider.enabled = !_grabbed;

            if (!_grabbed) return;

            _rigidbody.AddTorque(randomSpin.x, randomSpin.y, randomSpin.z, ForceMode.VelocityChange);

        }
        
        public string interactionDescription {
            get => "Grab";
            set{;}
        }

        public void Interact(Entity entity){
            Grab(entity);
        }

        public void Grab(Entity entity){
            if (entity.grabbedObjects.Count >= 4) return;

            _grabbed = true;

            entity.grabbedObjects.Add(this);
            randomSpin = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized * 0.3f ;
        }

        public void Throw(Entity entity){
            
            if ( !entity.grabbedObjects.Contains(this) ) return;

            _grabbed = false;

            entity.grabbedObjects.Remove(this);
            _rigidbody.AddForce(entity.lookRotationData.currentValue*Vector3.forward*30f*_rigidbody.mass, ForceMode.Impulse);


            var impulseParticle = Instantiate(Global.LoadParticle("ShiftImpulseParticles"), entity._transform.position + entity.lookRotationData.currentValue * Vector3.forward*2f, entity.lookRotationData.currentValue);
            Destroy(impulseParticle, 1.2f);
        }

    }
}