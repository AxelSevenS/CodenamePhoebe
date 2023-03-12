using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SevenGame.Utility;

namespace SeleneGame.Core {

    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    public class WaterController : MonoBehaviour {



        
        [SerializeField] private Collider _collider;
        [SerializeField] private Renderer _renderer;

        [SerializeField] private Material material;

        [SerializeField] private float _waveStrength;
        [SerializeField] private float _waveSpeed;
        [SerializeField] private float _waveFrequency;

        private MaterialPropertyBlock _propertyBlock; 



        public new Collider collider {
            get {
                _collider ??= GetComponent<Collider>();
                return _collider;
            }
        }
        public new Renderer renderer {
            get {
                _renderer ??= GetComponent<Renderer>();
                return _renderer;
            }
        }

        public float waveStrength => _waveStrength;
        public float waveSpeed => _waveSpeed;
        public float waveFrequency => _waveFrequency;

        private MaterialPropertyBlock propertyBlock {
            get {
                _propertyBlock ??= new MaterialPropertyBlock();
                return _propertyBlock;
            }
        }

    
        private void Awake(){
            renderer.sharedMaterial = material;

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

        private void OnValidate() {

            if ( renderer == null ) return;

            propertyBlock.SetFloat("_WaveStrength", waveStrength);
            propertyBlock.SetFloat("_WaveSpeed", waveSpeed);
            propertyBlock.SetFloat("_WaveFrequency", waveFrequency);
            renderer.SetPropertyBlock(propertyBlock);
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