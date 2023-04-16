using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace SeleneGame.Core {
    
    public class DamageZone : MonoBehaviour {

        public Entity owner;

        private HashSet<IDamageable> _damagedEntities = new HashSet<IDamageable>();
            
        public bool isParry = false;
        public bool isParryable = true;
        public bool isMelee = true;

        public float damage = 1f;
        public DamageType damageType = DamageType.Physical;
        public DamageKnockback knockback = DamageKnockback.Flinch;

        public float objectLifeTime = 2f;
        public float damageLifeTime = 0.5f;


        public DamageData data => new DamageData(owner, damage, damageType, knockback, transform.forward);



        public static DamageZone Create(Entity owner, string address, Action<DamageZone> modifier = null) {
            GameObject damageObject = AddressablesUtils.GetAsset<GameObject>( Path.Combine("Attacks/", address) );
            damageObject = GameObject.Instantiate(damageObject);

            DamageZone damageZoneComponent = damageObject.GetComponentInChildren<DamageZone>();
            damageZoneComponent.owner = owner;
            
            modifier?.Invoke(damageZoneComponent);

            return damageZoneComponent;
        }

        // public static DamageZone Create<T>(Entity owner, Vector3 position, Quaternion rotation, float damage, DamageType damageType, DamageKnockback knockback, float objectLifeTime = 2f, float damageLifeTime = 0.5f) where T : Collider {
        //     GameObject damageZone = new GameObject("DamageZone");
        //     damageZone.transform.position = position;
        //     damageZone.transform.rotation = rotation;

        //     T collider = damageZone.AddComponent<T>();
        //     DamageZone damageZoneComponent = damageZone.AddComponent<DamageZone>();
        //     damageZoneComponent.owner = owner;
        //     damageZoneComponent.damage = damage;
        //     damageZoneComponent.damageType = damageType;
        //     damageZoneComponent.knockback = knockback;
        //     damageZoneComponent.objectLifeTime = objectLifeTime;
        //     damageZoneComponent.damageLifeTime = damageLifeTime;

        //     return damageZoneComponent;
        // }

        public bool GetParriedBy(DamageZone otherZone) {
            if ( otherZone == null || otherZone.owner == owner || !isParryable || !otherZone.isParry ) return false;

            if ( isMelee ) {
                DamageData parryData = new DamageData(owner, 0f, otherZone.damageType, DamageKnockback.Block, otherZone.transform.forward);
                owner?.Damage(parryData);
            } else {
                // Send Projectile back
            }
            
            otherZone.owner?.AwardParry(otherZone.data);
            _damagedEntities.Remove(otherZone.owner);

            owner = otherZone.owner;
                
            return true;
        }

        private void Start() {
            if (damageLifeTime > 0f)
                Destroy(this, damageLifeTime);

            if (objectLifeTime > 0f)
                Destroy(gameObject, objectLifeTime);
        }

        private void LateUpdate() {
            
            // We use a queue system for entity damaging to make sure that the damage zone can get parried by another attack before it damages the entity.
            // it also helps with duplicate damage calls.
            /// <see cref="OnTriggerEnter"/>

            DamageData damageData = data;
            foreach (IDamageable damageable in _damagedEntities) {
                damageable?.Damage(damageData);
            }
            _damagedEntities.Clear();
        }

        private void OnTriggerEnter(Collider other) {
            if (other == null) return;

            if ( other.TryGetComponent(out DamageZone otherZone) || (other?.attachedRigidbody?.TryGetComponent(out otherZone) ?? false) ) {
                if ( GetParriedBy(otherZone) ) return;
            }

            // Queue up the entity for damage, we do not want to damage it immediately because we want to be late, so that parry interactions can happen before entity damage interactions.
            /// <see cref="LateUpdate"/>

            if ( other.TryGetComponent(out IDamageable damageable) || (other?.attachedRigidbody?.TryGetComponent(out damageable) ?? false) ) {
                _damagedEntities.Add(damageable);
            }
        }
        

    }

}
