using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SeleneGame.Core {
    
    [System.Serializable]
    public struct InvokableEvent {

        public enum EventType { StartDialogue, SetEntityCostume, SetWeaponCostume, Destroy };

        public EventType eventType;

        public Conversation conversation;
        public EntityData entity;
        public EntityCostume entityCostume;
        public WeaponData weapon;
        public WeaponCostume weaponCostume;

        public void Invoke(GameObject gameObject){
            switch (eventType){
                case EventType.StartDialogue:
                    if (conversation == null) break;
                    GameEvents.StartDialogue(conversation, gameObject);
                    break;
                case EventType.SetEntityCostume:
                    if (entity == null || entityCostume == null) break;
                    GameEvents.SetEntityCostume(entity, entityCostume);
                    break;
                case EventType.SetWeaponCostume:
                    if (weapon == null || weaponCostume == null) break;
                    GameEvents.SetWeaponCostume(weapon, weaponCostume);
                    break;
                case EventType.Destroy:
                    Object.Destroy(gameObject);
                    break;
            }
        }
    }
}