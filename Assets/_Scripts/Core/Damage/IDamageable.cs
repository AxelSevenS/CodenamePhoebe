using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SeleneGame.Core {
    
    public interface IDamageable {

        void Damage(Entity owner, DamageData damageData);
        void Heal(float amount);
        
    }
}