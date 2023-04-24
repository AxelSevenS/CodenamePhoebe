using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SeleneGame.Core {

    public struct DamageData {
        public Entity owner;
        public float amount;
        public DamageType damageType;
        public DamageKnockback knockback;
        public Vector3 direction;

        public DamageData(Entity owner, float amount, DamageType damageType, DamageKnockback knockback, Vector3 direction) {
            this.owner = owner;
            this.amount = amount;
            this.damageType = damageType;
            this.knockback = knockback;
            this.direction = direction;
        }
    }
    
}
