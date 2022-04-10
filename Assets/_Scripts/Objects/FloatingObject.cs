using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SeleneGame {

    [RequireComponent(typeof(CustomPhysicsComponent))]
    public class FloatingObject : MonoBehaviour{
        
        private new Rigidbody rigidbody;
        private CustomPhysicsComponent _physicsComponent;
        [SerializeField] [Range(0f, 3f)] private float floatability; 

        void OnEnable() => Global.objectManager.objectList.Add( this.gameObject );
        void OnDisable() => Global.objectManager.objectList.Remove( this.gameObject );

        private void Awake(){
            rigidbody = GetComponent<Rigidbody>();
            _physicsComponent = GetComponent<CustomPhysicsComponent>();
        }
        private void FixedUpdate(){
            
            rigidbody.AddForceAtPosition(Physics.gravity*1.5f, transform.position, ForceMode.Acceleration);
            _physicsComponent.BodyFloat(rigidbody, floatability);

        }
    }
}