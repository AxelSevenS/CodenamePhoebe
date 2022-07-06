using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SeleneGame.Core;

namespace SeleneGame.Core {

    [RequireComponent(typeof(CustomPhysicsComponent))]
    public class FloatingObject : MonoBehaviour{
        
        private new Rigidbody rigidbody;
        private CustomPhysicsComponent physicsComponent;
        [SerializeField] [Range(0f, 3f)] private float floatability; 

        void OnEnable() => Global.objectManager.objectList.Add( this.gameObject );
        void OnDisable() => Global.objectManager.objectList.Remove( this.gameObject );

        private void Awake(){
            rigidbody = GetComponent<Rigidbody>();
            physicsComponent = GetComponent<CustomPhysicsComponent>();
        }
        private void FixedUpdate(){
            
            rigidbody.AddForceAtPosition(Physics.gravity*1.5f, transform.position, ForceMode.Acceleration);
            physicsComponent.BodyFloat(rigidbody, transform.position, floatability);

        }
    }
}