using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SeleneGame.Core.UI {
    public interface IDialogueReader {

        void StartDialogue(DialogueLine source, GameObject newDialogueObject = null);
        void EndDialogue();
        // void SkipToLine(DialogueSource source);
        void InterruptDialogue();

    }
}
