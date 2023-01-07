using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SeleneGame.Core {
    
    public interface IDamageable {

        void Damage(float amount, Vector3 knockback = default);
        void Heal(float amount);
    }
}