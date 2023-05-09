using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Scribe;

namespace SeleneGame.Core {

    [System.Serializable]
    public class WeaponInputEvent : ScribeEvent<WeaponAction, InputCondition> {

        public string name;

        public void Invoke (Weapon weapon) {
            foreach (WeaponAction action in actions) {
                action.Invoke(weapon);
            }
        }

        public bool Evaluate(Player playerController) {
            bool left = conditions.condition.Evaluate(playerController);
            foreach (ScribeSubCondition<InputCondition> subCondition in conditions.subConditions) {
                left = subCondition.MultiConditionEvaluate(left, subCondition.condition.Evaluate(playerController));
            }
            return left;
        }
    }
    
}
