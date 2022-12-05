using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SeleneGame.Core;
using SevenGame.Utility;

namespace SeleneGame {
    
    public class Sail : MonoBehaviour, IManipulable{

        public new Rigidbody rigidbody;
        public Vector3 bodyVelocity;
        public float sailTimer;

        void FixedUpdate() {
            if (sailTimer>0f){
                sailTimer -= 0.5f*GameUtility.timeDelta;
                // rigidbody.transform.Translate(bodyVelocity * (sailTimer+0.3f) * GameUtility.timeDelta, Space.World);
                rigidbody.AddForce(bodyVelocity*sailTimer, ForceMode.Acceleration);
                rigidbody.AddRelativeTorque(0f, (2f - (Vector3.Dot(bodyVelocity.normalized, rigidbody.transform.forward) + 1f)) * Vector3.Dot(rigidbody.transform.right, bodyVelocity.normalized), 0f, ForceMode.Acceleration);
            }
        }

        public void Grab() {
            // bodyVelocity = Vector3.Cross(Vector3.down, Camera.main.transform.right).normalized;
            // sailTimer = 10f;
            
            //rigidbody.AddForce(bodyVelocity/GameUtility.timeDelta, ForceMode.Impulse);
        }

        public void Grabbed(){
        }

        public void Hold() {
        }
    }

}