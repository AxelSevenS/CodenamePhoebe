using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SeleneGame.Core;

namespace SeleneGame {

    public class VehiculeAnchor : MonoBehaviour {

        [SerializeField] private GameObject followedObject;
        [SerializeField] private Vector3 relativePlacement = new Vector3(0,0,0);
        [SerializeField] private Vector3 directionalPlacement = new Vector3(0,0,0);
        public void SetFollowedObject(GameObject newTarget) => followedObject = newTarget;
        private void FollowObject(){
            if (followedObject != null){
                transform.position = followedObject.transform.position + relativePlacement + followedObject.transform.rotation * directionalPlacement;
                transform.rotation = followedObject.transform.rotation;
            }
        }

        private void Start(){
            gameObject.layer = 6;
        }

        private void Update(){
            FollowObject();
        }

        void OnTriggerEnter(Collider col){
            if (col.gameObject.TryGetComponent<CustomPhysicsComponent>(out var physicsComponent)){
                col.transform.SetParent(this.transform);
            }
        }
        void OnTriggerExit(Collider col){
            if (col.gameObject.TryGetComponent<CustomPhysicsComponent>(out var physicsComponent) && col.transform.parent == this.transform ){
                col.transform.SetParent(null);
            }
        }
    }
}