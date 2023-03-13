using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SeleneGame.Core {
    
    public class DamageZone : MonoBehaviour {

        public Entity owner;
            
        public float damage = 1f;
        public DamageType damageType = DamageType.Physical;
        public Vector3 knockback = Vector3.zero;

        public float lifeTime = 2f;

        private void Start() {
            if (lifeTime > 0f)
                Destroy(gameObject, lifeTime);
        }

        private void OnTriggerEnter(Collider other) {

            if (other.TryGetComponent(out IDamageable damageable) || other.attachedRigidbody.TryGetComponent(out damageable)) {
                damageable.Damage(damage, damageType, transform.rotation * knockback, owner);
            }
        }
        

    }

}
