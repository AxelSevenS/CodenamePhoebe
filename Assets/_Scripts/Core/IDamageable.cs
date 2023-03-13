using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SeleneGame.Core {
    
    public interface IDamageable {

        void Damage(float amount, DamageType damageType = DamageType.Physical, Vector3 knockback = default, Entity owner = null);
        void Heal(float amount);
        void Kill();
        
    }
}