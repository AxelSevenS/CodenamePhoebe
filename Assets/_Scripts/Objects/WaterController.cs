using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SeleneGame.Core;
using SeleneGame.Utility;

namespace SeleneGame {

    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    public class WaterController : MonoBehaviour {
        
        public new Collider collider;

        new Renderer renderer;
    
        void Awake(){
            collider = GetComponent<Collider>();
            if ( collider is MeshCollider meshCollider && meshCollider.sharedMesh == null ){
                MeshFilter meshFilter = GetComponent<MeshFilter>();
                meshCollider.sharedMesh = meshFilter.sharedMesh;
            }
            if (renderer == null)
                renderer = GetComponent<Renderer>();
            if (renderer.sharedMaterial == null)
                renderer.sharedMaterial = new Material(Shader.Find("Shader Graphs/Water"));
        }

        void Start() {
            if ( collider is MeshCollider meshCollider )
                meshCollider.convex = true;
            // collider.isTrigger = true;
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