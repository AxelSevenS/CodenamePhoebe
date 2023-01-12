using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SeleneGame.Core {

    public abstract class DialogueSource : ScriptableObject, IDialogueSource {
        public abstract DialogueLine GetDialogue();
    }
}
