using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Scribe;

namespace SeleneGame.Core {

    [System.Serializable]
    public class ConditionalDialogue {

        public ScribeMultiCondition<GameCondition> conditions = new ScribeMultiCondition<GameCondition>();
        
        public DialogueSource dialogueSource;

        public bool Evaluate() {
            bool left = conditions.condition.Evaluate();
            foreach (ScribeSubCondition<GameCondition> subCondition in conditions.subConditions) {
                bool right = subCondition.condition.Evaluate();
                left = subCondition.MultiConditionEvaluate(left, right);
            }
            return left;
        }
    }
}
