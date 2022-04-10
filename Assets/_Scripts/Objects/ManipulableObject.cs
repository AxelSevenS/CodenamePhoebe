using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SeleneGame {
    
    [RequireComponent(typeof(FloatingObject))]
    public class ManipulableObject : MonoBehaviour, IManipulable, IInteractable{

        private new Rigidbody rigidbody;
        private Collider _collider;
        private Camera _camera;
        
        private float holdDistance;
        private bool inFront;
        private Vector3 randomSpin;

        void Awake(){
            rigidbody = GetComponent<Rigidbody>();
            _collider = GetComponent<Collider>();
            _camera = Camera.main;
            
        }
        
        public string interactionDescription {
            get => "Grab";
            set{;}
        }

        public void Interact(Entity entity){
            Grab(entity);
        }

        void Update(){
        }

        void FixedUpdate() {

        }

        public void Grab(Entity entity){

            if (entity.manipulatedObject.Count >= 4) return;

            _collider.enabled = false;
            entity.manipulatedObject.Add(this);
            randomSpin = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized ;
        }

        public void Grabbed(Entity entity){
            rigidbody.AddTorque(randomSpin.x, randomSpin.y, randomSpin.z, ForceMode.VelocityChange);
        }

        public void Hold(Entity entity){
            _collider.enabled = true;
            entity.manipulatedObject.Remove(this);
            rigidbody.AddForce(_camera.transform.forward*30f*rigidbody.mass, ForceMode.Impulse);


            var impulseParticle = Instantiate(Global.LoadParticle("ShiftImpulseParticles"), entity._t.position + _camera.transform.forward*2f, Quaternion.FromToRotation(Vector3.forward, _camera.transform.forward));
            Destroy(impulseParticle, 1.2f);
        }

    }
}