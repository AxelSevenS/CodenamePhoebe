using System.Collections;
using System.Collections.Generic;
using System.IO;
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
            if (other == null) return;

            if ( other.TryGetComponent(out IDamageable damageable) || (other?.attachedRigidbody?.TryGetComponent(out damageable) ?? false) ) {
                damageable.Damage(damage, damageType, transform.rotation * knockback, owner);
            }
        }

        public enum DamageOperation {
            Add,
            Multiply
        }

        public static DamageZone Create(Entity owner, string address, Vector3 position, Quaternion rotation, Vector3 scale, Transform parent = null, float damageModifier = 0f, DamageOperation damageOperation = DamageOperation.Add) {
            GameObject damageObject = AddressablesUtils.GetAsset<GameObject>( Path.Combine("Attacks/", address) );
            damageObject = GameObject.Instantiate(damageObject, position, rotation, parent);
            damageObject.transform.localScale = Vector3.Scale(damageObject.transform.localScale, scale);

            DamageZone damageZoneComponent = damageObject.GetComponentInChildren<DamageZone>();
            damageZoneComponent.owner = owner;
            if (damageOperation == DamageOperation.Add)
                damageZoneComponent.damage += damageModifier;
            else if (damageOperation == DamageOperation.Multiply)
                damageZoneComponent.damage *= damageModifier;

            return damageZoneComponent;
        }

        public static DamageZone Create<T>(Entity owner, Vector3 position, Quaternion rotation, float damage, DamageType damageType, Vector3 knockback, float lifeTime = 2f) where T : Collider {
            GameObject damageZone = new GameObject("DamageZone");
            damageZone.transform.position = position;
            damageZone.transform.rotation = rotation;

            T collider = damageZone.AddComponent<T>();
            DamageZone damageZoneComponent = damageZone.AddComponent<DamageZone>();
            damageZoneComponent.owner = owner;
            damageZoneComponent.damage = damage;
            damageZoneComponent.damageType = damageType;
            damageZoneComponent.knockback = knockback;
            damageZoneComponent.lifeTime = lifeTime;

            return damageZoneComponent;
        }
        

    }

}
