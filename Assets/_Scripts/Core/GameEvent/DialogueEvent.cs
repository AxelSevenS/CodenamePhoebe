using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SeleneGame.Core.UI;

namespace SeleneGame.Core {

    [System.Serializable]
    public sealed class DialogueEvent : ConditionalEvent {

        public EventType eventType = EventType.StartDialogue;
        

        public void Invoke(GameObject dialogueObject) {
            Invoke((ConditionalEvent.EventType)eventType, dialogueObject);
        }

        public enum EventType {
            StartDialogue = ConditionalEvent.EventType.StartDialogue,
            StartAlert = ConditionalEvent.EventType.StartAlert,
            EndDialogue = ConditionalEvent.EventType.EndDialogue
        }
    }
}
