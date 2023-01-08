using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SeleneGame.Core.UI {
    public interface IDialogueReader {

        void StartDialogue(DialogueLine dialogue, GameObject newDialogueObject);
        void EndDialogue();
        void SkipToLine(DialogueLine dialogueLine);
        void InterruptDialogue();

    }
}
