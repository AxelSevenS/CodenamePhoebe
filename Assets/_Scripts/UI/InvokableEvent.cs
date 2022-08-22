using UnityEngine;

using SeleneGame.Core;


namespace SeleneGame.UI {
    
    [System.Serializable]
    public struct InvokableEvent {

        public enum EventType { StartDialogue, SetEntityCostume, SetPlayerCostume, Destroy };

        public EventType eventType;

        public Dialogue dialogue;
        public Entity entity;
        public CharacterCostume characterCostume;

        public void Invoke(GameObject gameObject){
            switch (eventType){
                case EventType.StartDialogue:
                    if (dialogue == null) break;
                    DialogueController.current.StartDialogue(dialogue, gameObject);
                    break;
                case EventType.SetEntityCostume:
                    if (entity == null || characterCostume == null) break;
                    entity.SetCostume(characterCostume);
                    break;
                case EventType.SetPlayerCostume:
                    if (characterCostume == null) break;
                    PlayerEntityController.current.entity.SetCostume(characterCostume);
                    break;
                case EventType.Destroy:
                    Object.Destroy(gameObject);
                    break;
            }
        }
    }
}