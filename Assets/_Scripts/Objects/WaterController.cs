using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SeleneGame {

    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    public class WaterController : MonoBehaviour{
        
        public new Collider collider;
        public Color shallowColor;
        public Color deepColor;
        public Texture2D noiseMap;
        public Texture2D normalMap;
        [Range(0, 1)] public float shininess;
        [Range(0, 1)] public float smoothness;
        [Range(0, 1)] public float normalIntensity = 0.47f;

        [Space(15f)]
        [Range(0, 1)] public float waveStrength;
        [Range(0, 1)] public float waveSpeed;
        [Range(0, 1)] public float waveFrequency;
        [Range(0, 0.1f)] public float noiseScale;
        [Range(0, 1)] public float noiseSpeed;
        [Range(0, 10)] public float noiseStrength;
        public float foamDistance;
        public bool isPlane = true;

        new Renderer renderer;

        MaterialPropertyBlock mpb;
        public MaterialPropertyBlock Mpb {
            get { 
                if (mpb == null)
                    mpb = new MaterialPropertyBlock();
                return mpb;
            }
        }

        void OnValidate(){
            ApplyParams();
        }

        void ApplyParams(){
            if ( collider is MeshCollider && (collider as MeshCollider).sharedMesh == null ){
                MeshFilter meshFilter = GetComponent<MeshFilter>();
                (collider as MeshCollider).sharedMesh = meshFilter.sharedMesh;
            }
            if (renderer == null)
                renderer = GetComponent<Renderer>();
            if (renderer.sharedMaterial == null)
                renderer.sharedMaterial = new Material(Shader.Find("Shader Graphs/Water"));

            Mpb.SetColor("_ColorShallow", shallowColor);
            Mpb.SetColor("_ColorDeep", deepColor);
            Mpb.SetTexture("_WaterNormalMap", normalMap);
            Mpb.SetTexture("_WaterNoiseMap", noiseMap);
            Mpb.SetFloat("_Shininess", shininess);
            Mpb.SetFloat("_Smoothness", smoothness);
            Mpb.SetFloat("_NormalIntensity", normalIntensity);
            Mpb.SetFloat("_WaveStrength", waveStrength);
            Mpb.SetFloat("_WaveSpeed", waveSpeed);
            Mpb.SetFloat("_WaveFrequency", waveFrequency);
            Mpb.SetFloat("_NoiseStrength", noiseStrength);
            Mpb.SetFloat("_NoiseSpeed", noiseSpeed);
            Mpb.SetFloat("_NoiseScale", noiseScale);
            Mpb.SetInt ("_Cull", 2);

            renderer.SetPropertyBlock( Mpb );
        }
    
        void Awake(){
            collider = GetComponent<Collider>();
            ApplyParams();
        }

        void Start() {
            if ( collider is MeshCollider ) (collider as MeshCollider).convex = true;
            collider.isTrigger = true;
            gameObject.layer = 4;
        }

        private void OnTriggerEnter(Collider col){
            if (col.attachedRigidbody.TryGetComponent<CustomPhysicsComponent>(out var physicsComponent)) {
                physicsComponent.UpdateWaterBody(this);
                physicsComponent.isNearWater = true;
            }
        }
        // private void OnTriggerStay(Collider col){
        //     if (col.attachedRigidbody.TryGetComponent<CustomPhysicsComponent>(out var physicsComponent)) {
        //         physicsComponent.isNearWater = true;
        //     }
        // }
        private void OnTriggerExit(Collider col){
            if (col.attachedRigidbody.TryGetComponent<CustomPhysicsComponent>(out var physicsComponent)) {
                physicsComponent.isNearWater = false;
            }
        }
    }
}