using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SeleneGame.Core {

    [System.Serializable]
    public class ConditionalDialogue {

        public EventMultiCondition conditions;
        
        public DialogueSource dialogueSource;


        public bool Evaluate() => conditions.Evaluate();
    }
}
