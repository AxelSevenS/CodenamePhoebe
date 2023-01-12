using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SeleneGame.Core {

    [System.Serializable]
    public class ConditionalDialogue {

        public EventCondition condition;
        public EventSubCondition[] subConditions;
        
        public DialogueSource dialogueSource;


        public bool Evaluate() {
            if (condition.conditionType == EventCondition.ConditionType.Always) return true;

            bool conditionsMet = condition.Evaluate();
            foreach (EventSubCondition subCondition in subConditions) {
                conditionsMet = subCondition.Evaluate(conditionsMet);
            }
            return conditionsMet;
        }
    }
}
