using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SevenGame.Utility;

namespace SeleneGame.Core {

    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    public class WaterController : MonoBehaviour {



        
        [SerializeField] private Collider _collider;



        public new Collider collider {
            get {
                _collider ??= GetComponent<Collider>();
                return _collider;
            }
        }

    
        private void Awake(){

            if ( collider is MeshCollider meshCollider && meshCollider.sharedMesh == null ){
                MeshFilter meshFilter = GetComponent<MeshFilter>();
                meshCollider.sharedMesh = meshFilter.sharedMesh;
            }

        }

        private void Start() {
            if ( collider is MeshCollider meshCollider )
                meshCollider.convex = true;
            
            GameUtility.SetLayerRecursively(gameObject, 4);
        }

        // private void OnTriggerEnter(Collider col){
        //     if (col.attachedRigidbody.TryGetComponent<CustomPhysicsComponent>(out var physicsComponent)) {
        //         physicsComponent.UpdateWaterBody(this);
        //         physicsComponent.isNearWater = true;
        //     }
        // }
        // private void OnTriggerExit(Collider col){
        //     if (col.attachedRigidbody.TryGetComponent<CustomPhysicsComponent>(out var physicsComponent)) {
        //         physicsComponent.isNearWater = false;
        //     }
        // }
    }
}