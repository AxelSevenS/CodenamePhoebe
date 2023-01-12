using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SeleneGame.Core.UI {
    public interface IDialogueReader {

        void StartDialogue(IDialogueSource source, GameObject newDialogueObject);
        void EndDialogue();
        void SkipToLine(IDialogueSource source);
        void InterruptDialogue();

    }
}
