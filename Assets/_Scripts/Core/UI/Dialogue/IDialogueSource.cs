using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SeleneGame.Core {
    public interface IDialogueSource {
        DialogueLine GetDialogue();
    }
}
