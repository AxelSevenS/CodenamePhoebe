using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Scribe;

namespace SeleneGame.Core {

    [System.Serializable]
    public sealed class GameEvent : ScribeEvent<GameAction, GameCondition> {
        public bool Evaluate() {
            bool left = conditions.condition.Evaluate();
            foreach (ScribeSubCondition<GameCondition> subCondition in conditions.subConditions) {
                bool right = subCondition.condition.Evaluate();
                left = subCondition.MultiConditionEvaluate(left, right);
            }

            return left;
        }

        public void Invoke(GameObject eventObject) {
            foreach (GameAction action in actions) {
                action.Invoke(eventObject);
            }
        }
    }
}
