using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.AddressableAssets;


namespace SeleneGame.Core {
    
    [System.Serializable]
    public struct InvokableEvent {


        public EventType eventType;
        public Entity entity;
        public CharacterCostume characterCostume;


        public void Invoke(GameObject gameObject){
            switch (eventType){
                case EventType.SetEntityCostume:
                    if (entity == null || characterCostume == null) break;
                    entity.SetCostume(characterCostume);
                    break;
                case EventType.SetPlayerCostume:
                    if (entity == null || characterCostume == null) break;
                    entity.SetCostume(characterCostume);
                    PlayerEntityController.current.entity.SetCostume(characterCostume);
                    break;
                case EventType.Destroy:
                    Object.Destroy(gameObject);
                    break;
            }
        }


        public enum EventType { SetEntityCostume, SetPlayerCostume, Destroy };
    }
}