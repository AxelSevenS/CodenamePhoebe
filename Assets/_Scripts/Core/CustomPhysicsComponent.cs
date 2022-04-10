using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SeleneGame {
    
    public class CustomPhysicsComponent : MonoBehaviour{
        public bool isNearWater = false;
        public float displacementMultiplier;
        public bool inWater => (transform.position.y < waterHeight && isNearWater);
        
        public float waterHeight;
        public WaterController waterController;

        void Awake(){
        }

        void FixedUpdate(){
            if (!isNearWater) return;

            waterHeight = CalculateNoise() + CalculateWave();
            waterHeight += waterController.collider is MeshCollider ? waterController.collider.ClosestPoint(transform.position).y : waterController.transform.position.y;

            displacementMultiplier = Mathf.Clamp(waterHeight - transform.position.y, 0, 1);

            Debug.DrawRay(new Vector3(transform.position.x, waterHeight, transform.position.z), Vector3.up, Color.red);
        }

        private float CalculateNoise(){
            Vector2 noiseUV = new Vector2(transform.position.x*waterController.noiseScale + Time.time*-WeatherManager.current.windDirection.x/20f*waterController.noiseSpeed, transform.position.z*waterController.noiseScale + Time.time*-WeatherManager.current.windDirection.z/20f*waterController.noiseSpeed);
            return (waterController.noiseMap.GetPixelBilinear(noiseUV.x, noiseUV.y).r*waterController.noiseStrength);
        }

        private float CalculateWave(){
            return Mathfs.CalculateWave(waterController.waveStrength, Time.time*2f * waterController.waveSpeed, Vector3.Scale(transform.position, WeatherManager.current.windDirection), waterController.waveFrequency);
        }

        public void UpdateWaterBody(WaterController controller){
            waterController = controller;
        }

        public void BodyFloat(Rigidbody rb, float floatability){
            if (transform.position.y-1f < waterHeight && isNearWater){
                rb.AddForceAtPosition(new Vector3(0f, Mathf.Abs(Physics.gravity.y) * (displacementMultiplier * floatability), 0f), transform.position, ForceMode.Acceleration);
            }
        }
    }
}