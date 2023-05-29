using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SeleneGame.Core {

    public interface IDamageDealer {
        
        void AwardDamage(DamageData damageData);

        void AwardParry(DamageData damageData);

        bool IsValidTarget(IDamageable target);

    }
}
