using System.Collections;
using System.Collections.Generic;
using SevenGame.Utility;
using UnityEngine;

namespace SeleneGame.Core {
    
    public class CustomPhysicsComponent : MonoBehaviour, IWaterDisplaceable {

        [SerializeField] public WaterController waterController;
        [SerializeField] public Collider waterCollider;

        Collider[] _colliderBuffer = new Collider[1];

        [SerializeField] private float _waveHeight;




        public bool inWater => waterCollider != null;
        
        public float waterHeight => inWater ? waterCollider.ClosestPoint(transform.position + new Vector3(0, waterCollider.bounds.size.y, 0)).y : 0f;
        public float waveHeight { 
            get => inWater ? _waveHeight : 0f; 
            set => _waveHeight = inWater ? value : 0f;
        }
        public float totalWaterHeight => waterHeight + waveHeight;


        public Vector3 position => transform.position;



        public void BodyFloat(Rigidbody rb, Vector3 position, float floatability){
            float totalWaterHeight = waterHeight + waveHeight;
            if (!inWater || position.y > totalWaterHeight ) return;


            float displacementMultiplier = Mathf.Clamp(totalWaterHeight - position.y, 0, 1);
            rb.AddForceAtPosition(new Vector3(0f, Mathf.Abs(Physics.gravity.y) * (displacementMultiplier * floatability), 0f), position, ForceMode.Acceleration);
            
        }

        private void OnDrawGizmos() {
            Gizmos.color = Color.red;
            Vector3 waterPosition = transform.position;
            waterPosition.y = totalWaterHeight;
            Gizmos.DrawWireSphere(waterPosition, 0.1f);
        }

        private void OnEnable() => WaterManager.AddWaterDisplaceable(this);
        private void OnDisable() => WaterManager.RemoveWaterDisplaceable(this);

        private void FixedUpdate(){

            waterCollider = null;
            waterController = null;

            _colliderBuffer[0] = null;
            Physics.OverlapSphereNonAlloc(transform.position, 5f, _colliderBuffer, Collision.WaterMask);
            foreach (Collider waterCollider in _colliderBuffer) {
                if (waterCollider != null && waterCollider.TryGetComponent<WaterController>(out WaterController waterController)){
                    this.waterCollider = waterCollider;
                    this.waterController = waterController;
                    break;
                }
            }
            
        }
    }
}