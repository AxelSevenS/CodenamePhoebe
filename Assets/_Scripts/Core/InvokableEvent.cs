using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SeleneGame.Core {
    
    [System.Serializable]
    public struct InvokableEvent {

        public enum EventType { StartDialogue, SetEntityCostume, SetWeaponCostume, Destroy };

        public EventType eventType;

        public string parameter1;
        public string parameter2;
        public string parameter3;

        public void Invoke(GameObject gameObject){
            switch (eventType){
                case EventType.StartDialogue:
                    GameEvents.StartDialogue(Conversation.GetConversationByName(parameter1), gameObject);
                    break;
                case EventType.SetEntityCostume:
                    GameEvents.SetEntityCostume(parameter1, parameter2);
                    break;
                case EventType.SetWeaponCostume:
                    GameEvents.SetWeaponCostume(parameter1, parameter2);
                    break;
                case EventType.Destroy:
                    Object.Destroy(gameObject);
                    break;
            }
        }
    }
}