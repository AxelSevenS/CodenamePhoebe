using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SeleneGame.Core;

namespace SeleneGame {
    
    public class Sail : MonoBehaviour, IManipulable{

        public new Rigidbody rigidbody;
        public Vector3 bodyVelocity;
        public float sailTimer;

        void FixedUpdate() {
            if (sailTimer>0f){
                sailTimer -= 0.5f*Time.deltaTime;
                // rigidbody.transform.Translate(bodyVelocity * (sailTimer+0.3f) * Time.deltaTime, Space.World);
                rigidbody.AddForce(bodyVelocity*sailTimer, ForceMode.Acceleration);
                rigidbody.AddRelativeTorque(0f, (2f - (Vector3.Dot(bodyVelocity.normalized, rigidbody.transform.forward) + 1f)) * Vector3.Dot(rigidbody.transform.right, bodyVelocity.normalized), 0f, ForceMode.Acceleration);
            }
        }

        public void Grab() {
            // bodyVelocity = Vector3.Cross(Vector3.down, Camera.main.transform.right).normalized;
            // sailTimer = 10f;
            
            //rigidbody.AddForce(bodyVelocity/Time.deltaTime, ForceMode.Impulse);
        }

        public void Grabbed(){
        }

        public void Hold() {
        }
    }

}