using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.AddressableAssets;

using SeleneGame.Core.UI;

namespace SeleneGame.Core {
    
    [System.Serializable]
    public struct InvokableEvent {


        public EventType eventType;
        public Entity entity;
        public CharacterCostume characterCostume;
        public Dialogue dialogue;


        public void Invoke(GameObject gameObject){
            switch (eventType){
                case EventType.SetEntityCostume:
                    if (entity == null || characterCostume == null) break;
                    entity.SetCostume(characterCostume);
                    break;
                case EventType.SetPlayerCostume:
                    if (characterCostume == null) break;
                    PlayerEntityController.current.entity.SetCostume(characterCostume);
                    break;
                case EventType.StartDialogue:
                    if (dialogue == null) break;
                    DialogueController.current.StartDialogue(dialogue, gameObject);
                    break;
                case EventType.StartAlert:
                    if (dialogue == null) break;
                    AlertController.current.StartDialogue(dialogue, gameObject);
                    break;
                case EventType.Destroy:
                    Object.Destroy(gameObject);
                    break;
            }
        }


        public enum EventType { SetEntityCostume, SetPlayerCostume, StartDialogue, StartAlert, Destroy };
    }
}