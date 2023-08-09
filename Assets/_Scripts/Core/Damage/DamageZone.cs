using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace SeleneGame.Core {
    
    public class DamageZone : MonoBehaviour {

        public IDamageDealer owner;

        private HashSet<IDamageable> _damagedEntities = new HashSet<IDamageable>();
            
        public bool isParry = false;
        public bool isParryable = true;
        public bool isMelee = true;

        public float damage = 1f;
        public DamageType damageType = DamageType.Physical;
        public DamageKnockback knockback = DamageKnockback.Flinch;

        public float objectLifeTime = 2f;
        public float damageLifeTime = 0.5f;
        public float damageStartTime = 0f;


        public DamageData data => new DamageData(damage, damageType, knockback);



        public static DamageZone Create(IDamageDealer owner, string address, Action<DamageZone> modifier = null) {
            GameObject damageObject = AddressablesUtils.GetAsset<GameObject>( Path.Combine("Attacks/", address) );
            damageObject = GameObject.Instantiate(damageObject);

            DamageZone damageZoneComponent = damageObject.GetComponentInChildren<DamageZone>();
            damageZoneComponent.owner = owner;
            
            modifier?.Invoke(damageZoneComponent);

            return damageZoneComponent;
        }

        public bool GetParriedBy(DamageZone otherZone) {
            if ( otherZone == null || otherZone.owner == owner || !isParryable || !otherZone.isParry ) return false;

            if ( isMelee ) {
                DamageData parryData = new DamageData(0f, otherZone.damageType, DamageKnockback.Block);
            } else {
                // Send Projectile back
            }

            if (otherZone.owner == (IDamageDealer)Player.Current.Entity) {
                EntityManager.current.HardHitStop();
            }
            
            otherZone.owner?.AwardParry(otherZone.data);
            if (otherZone.owner is IDamageable otherDamageable)
                _damagedEntities.Remove(otherDamageable);

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
            
            // We use a buffer system for entity damaging to make sure that the damage zone can get parried by another attack before it damages the entity.
            // it also helps with duplicate damage calls.
            /// <see cref="OnTriggerEnter"/>

            DamageData damageData = data;
            foreach (IDamageable damageable in _damagedEntities) {
                damageable?.Damage(owner, damageData);
            }
            _damagedEntities.Clear();
        }

        private void OnTriggerEnter(Collider other) {
            if (other == null) return;

            if ( other.TryGetComponent(out DamageZone otherZone) || (other?.attachedRigidbody?.TryGetComponent(out otherZone) ?? false) ) {
                if ( GetParriedBy(otherZone) ) return;
            }

            // Buffer the entity for damage, we do not want to damage it immediately because we want to be late, so that parry interactions can happen before entity damage interactions.
            /// <see cref="LateUpdate"/>

            if ( other.TryGetComponent(out IDamageable damageable) || (other?.attachedRigidbody?.TryGetComponent(out damageable) ?? false) ) {
                
                if ( !owner.IsValidTarget(damageable) )
                    return;

                _damagedEntities.Add(damageable);
            }
        }
        

    }

}
