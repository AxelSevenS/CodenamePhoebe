using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SeleneGame.Core {
    
    public class CustomPhysicsComponent : MonoBehaviour{
        // public bool isNearWater => waterCollider != null/* = false */;
        public bool inWater => waterCollider != null /* => isNearWater && transform.position.y < waterHeight */;
        
        public float waterHeight => inWater ? waterCollider.ClosestPoint(transform.position + new Vector3(0, waterCollider.bounds.size.y, 0)).y : 0f;
        public WaterController waterController;
        public Collider waterCollider;

        public Entity entity;
        [SerializeField] private Rigidbody _rb;

        void Awake(){
            entity = GetComponent<Entity>();
            _rb = GetComponent<Rigidbody>();
        }

        // private void Update(){
        //     if (!isNearWater) return;

        //     waterHeight = CalculateNoise() + CalculateWave();
        //     waterHeight += waterCollider is MeshCollider ? waterCollider.ClosestPoint(transform.position).y : waterCollider.transform.position.y;
        // }

        private void FixedUpdate(){

            Collider[] hits = /* entity == null ? */ Physics.OverlapSphere(transform.position, 1f, Global.WaterMask) /* : entity.ColliderOverlap(0f, Global.WaterMask) */;
            waterCollider = hits.Length > 0 ? hits[0] : null;
            
            // Debug.DrawRay(new Vector3(transform.position.x, waterHeight, transform.position.z), Vector3.up, Color.red);
        }

        private float CalculateNoise(){
            return 0f;
            // Vector2 noiseUV = new Vector2(transform.position.x*waterController.noiseScale + Time.time*-WeatherManager.current.windDirection.x/20f*waterController.noiseSpeed, transform.position.z*waterController.noiseScale + Time.time*-WeatherManager.current.windDirection.z/20f*waterController.noiseSpeed);
            // return (waterController.noiseMap.GetPixelBilinear(noiseUV.x, noiseUV.y).r*waterController.noiseStrength);
        }

        private float CalculateWave(){
            return 0f;
            // return Mathfs.CalculateWave(waterController.waveStrength, Time.time*2f * waterController.waveSpeed, Vector3.Scale(transform.position, WeatherManager.current.windDirection), waterController.waveFrequency);
        }

        public void UpdateWaterBody(WaterController controller){
            waterController = controller;
        }

        public void BodyFloat(Rigidbody rb, Vector3 position, float floatability){
            if (position.y > waterHeight || !inWater) return;


            float displacementMultiplier = Mathf.Clamp(waterHeight - position.y, 0, 1);
            rb.AddForceAtPosition(new Vector3(0f, Mathf.Abs(Physics.gravity.y) * (displacementMultiplier * floatability), 0f), position, ForceMode.Acceleration);
            
        }
    }
}