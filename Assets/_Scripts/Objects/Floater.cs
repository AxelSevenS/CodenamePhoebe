using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SeleneGame.Core;

namespace SeleneGame {

    [RequireComponent(typeof(CustomPhysicsComponent))]
    public class Floater : MonoBehaviour{
        
        public new Rigidbody rigidbody;
        public CustomPhysicsComponent physicsComponent;
        public int floaterCount;
        // Start is called before the first frame update

        private void Awake(){
            physicsComponent = GetComponent<CustomPhysicsComponent>();
        }

        void Start(){
            rigidbody.angularDrag = 0.2f*(floaterCount*2);
        }

        private void FixedUpdate(){
            rigidbody.AddForceAtPosition(Physics.gravity*1.5f/floaterCount, transform.position, ForceMode.Acceleration);
            physicsComponent.BodyFloat(rigidbody, transform.position, (2f/floaterCount)+0.1f);
        }
    }
}