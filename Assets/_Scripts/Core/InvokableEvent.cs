using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SeleneGame.Core;
using SeleneGame.UI;

namespace SeleneGame {
    
    [System.Serializable]
    public struct InvokableEvent {

        public enum EventType { StartDialogue, SetEntityCostume, SetPlayerCostume, Destroy };

        public EventType eventType;

        public Dialogue dialogue;
        public Entity entity;
        public EntityCostume entityCostume;

        public void Invoke(GameObject gameObject){
            switch (eventType){
                case EventType.StartDialogue:
                    if (dialogue == null) break;
                    GameEvents.StartDialogue(dialogue, gameObject);
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