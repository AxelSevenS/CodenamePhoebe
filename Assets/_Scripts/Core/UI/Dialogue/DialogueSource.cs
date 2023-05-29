using System.Collections;
using System.Collections.Generic;
using Scribe;
using UnityEngine;

namespace SeleneGame.Core {

    public abstract class DialogueSource : ScriptableObject {

        public int GetFlags() {
            return ScribeFlags.GetFlag(GetHashCode().ToString());
        }

        public bool GetFlag(DialogueFlag flag) {
            int flags = GetFlags();
            return (flags & (int)flag) != 0;
        }

        public bool SetFlag(DialogueFlag flag) {
            int flags = GetFlags();
            if ((flags & (int)flag) != 0) return false;
            ScribeFlags.SetFlag(GetHashCode().ToString(), flags | (int)flag);
            return true;
        }

        public bool ClearFlag(DialogueFlag flag) {
            int flags = GetFlags();
            if ((flags & (int)flag) == 0) return false;
            ScribeFlags.SetFlag(GetHashCode().ToString(), flags & ~(int)flag);
            return true;
        }

        public abstract DialogueLine GetDialogue();

    }

    public enum DialogueFlag {
        Interrupted = 1 << 30,
        Spent = 1 << 31,
    }
}
