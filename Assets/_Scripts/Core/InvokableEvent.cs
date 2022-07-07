using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SeleneGame.Core {
    
    [System.Serializable]
    public struct InvokableEvent {

        public enum EventType { StartDialogue, SetEntityCostume, SetPlayerCostume, Destroy };

        public EventType eventType;

        public Conversation conversation;
        public Entity entity;
        public EntityCostume entityCostume;

        public void Invoke(GameObject gameObject){
            switch (eventType){
                case EventType.StartDialogue:
                    if (conversation == null) break;
                    GameEvents.StartDialogue(conversation, gameObject);
                    break;
                case EventType.SetEntityCostume:
                    if (entity == null || entityCostume == null) break;
                    entity.SetCostume(entityCostume);
                    break;
                case EventType.SetPlayerCostume:
                    if (entityCostume == null) break;
                    Player.current.entity.SetCostume(entityCostume);
                    break;
                case EventType.Destroy:
                    Object.Destroy(gameObject);
                    break;
            }
        }
    }
}