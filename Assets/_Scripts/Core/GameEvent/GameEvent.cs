using System.Collections.Generic;

using UnityEngine;

using SeleneGame.Core.UI;

namespace SeleneGame.Core {

    [System.Serializable]
    public sealed class GameEvent : ConditionalEvent {

        public ConditionalEvent.EventType eventType;
        

        public void Invoke(GameObject dialogueObject) {
            Invoke(eventType, dialogueObject);
        }
    }

}