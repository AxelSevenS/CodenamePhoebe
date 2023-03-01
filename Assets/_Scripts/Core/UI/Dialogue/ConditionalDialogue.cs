using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Scribe;

namespace SeleneGame.Core {

    [System.Serializable]
    public class ConditionalDialogue {

        public ScribeMultiCondition<FlagCondition> conditions;
        
        public DialogueSource dialogueSource;

        public bool Evaluate() {
            bool left = conditions.condition.Evaluate();
            foreach (ScribeSubCondition<FlagCondition> subCondition in conditions.subConditions) {
                left = subCondition.MultiConditionEvaluate(left, subCondition.condition.Evaluate());
            }
            return left;
        }
    }
}
