using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SeleneGame.Core {

    public struct DamageData {
        public float amount;
        public DamageType damageType;
        public DamageKnockback knockback;

        public DamageData(float amount, DamageType damageType, DamageKnockback knockback) {
            this.amount = amount;
            this.damageType = damageType;
            this.knockback = knockback;
        }
    }
    
}
