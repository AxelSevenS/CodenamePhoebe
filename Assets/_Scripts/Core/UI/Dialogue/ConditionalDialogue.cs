using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Scribe;

namespace SeleneGame.Core {

    [System.Serializable]
    public class ConditionalDialogue {

        public ScribeEventMultiCondition conditions;
        
        public DialogueSource dialogueSource;


        public bool Evaluate() => conditions.Evaluate();
    }
}
