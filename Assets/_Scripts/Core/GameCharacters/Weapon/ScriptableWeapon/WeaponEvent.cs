using System.Collections.Generic;
using UnityEngine;

using Scribe;

namespace SeleneGame.Core {

    [System.Serializable]
    public class WeaponEvent : ScribeEvent<WeaponAction, FlagCondition> {
        public void Invoke (Weapon weapon) {
            foreach (WeaponAction action in actions) {
                action.Invoke(weapon);
            }
        }

        public bool Evaluate(Weapon weapon) {
            return true;
        }
    }
    
}
