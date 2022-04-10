using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SeleneGame {
    
    [System.Serializable]
    public struct InvokableEvent {
        public string eventName;
        public string parameter1;
        public string parameter2;
        public string parameter3;

        public void Invoke(GameObject gameObject){
            switch (eventName){
                case "StartDialogue":
                    GameEvents.StartDialogue(Conversation.GetConversationByName(parameter1), gameObject);
                    break;
                case "SetEntityCostume":
                    GameEvents.SetEntityCostume(parameter1, parameter2);
                    break;
                case "SetWeaponCostume":
                    GameEvents.SetWeaponCostume(parameter1, parameter2);
                    break;
                case "Destroy":
                    UnityEngine.Object.Destroy(gameObject);
                    break;
            }
        }
    }
}