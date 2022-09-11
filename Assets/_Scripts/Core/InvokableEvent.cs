using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.AddressableAssets;


namespace SeleneGame.Core {
    
    [System.Serializable]
    public struct InvokableEvent {


        public EventType eventType;
        public Entity entity;
        public AssetReferenceT<CharacterCostume> characterCostume;


        public void Invoke(GameObject gameObject){
            switch (eventType){
                case EventType.SetEntityCostume:
                    if (entity == null) break;
                    CharacterCostume entityCostume = LoadCostume(characterCostume);
                    if (entityCostume == null) break;
                    entity.SetCostume(entityCostume);
                    break;
                case EventType.SetPlayerCostume:
                    CharacterCostume playerCostume = LoadCostume(characterCostume);
                    if (playerCostume == null) break;
                    PlayerEntityController.current.entity.SetCostume(playerCostume);
                    break;
                case EventType.Destroy:
                    Object.Destroy(gameObject);
                    break;
            }

            CharacterCostume LoadCostume(AssetReferenceT<CharacterCostume> costumeReference){
                if ( string.IsNullOrEmpty( (string)costumeReference.RuntimeKey )  ) {
                    return null;
                } else {
                    AsyncOperationHandle<CharacterCostume> opHandle = costumeReference.LoadAssetAsync();
                    return opHandle.WaitForCompletion();
                }
            }
        }


        public enum EventType { SetEntityCostume, SetPlayerCostume, Destroy };
    }
}