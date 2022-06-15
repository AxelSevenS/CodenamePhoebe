using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SeleneGame.Core {
    
    public class GameEvents{

        public static void Reset(){
            onPlayerInput = null;
            onToggleMenu = null;
            onStartDialogue = null;
            // onSetEntityCostume = null;
            // onSetWeaponCostume = null;
        }

        public static event Action onPlayerInput;
        public static void PlayerInput(){
            if (onPlayerInput != null)
                onPlayerInput(); 
        }
        public static event Action onToggleMenu;
        public static void ToggleMenu(){
            if (onToggleMenu != null)
                onToggleMenu();
        }
        

        public static event Action<Conversation,GameObject> onStartDialogue;
        public static void StartDialogue(Conversation dialogue, GameObject dialogueObject){
            if (onStartDialogue != null)
                onStartDialogue(dialogue, dialogueObject);
        }

        // public static event Action<string> onSetEntityCostume;
        public static void SetEntityCostume(EntityData entityData, EntityCostume costume){
            Debug.Log(entityData);
            Debug.Log(costume);
            Debug.Log("Set " + entityData.name + " Entity costume to : " + costume.name);
            entityData.costume = costume;
            // EntityManager.current.entityCostumes[entityData] = costume;

            entityData.onChangeCostume?.Invoke();
            // if (onSetEntityCostume != null)
            //     onSetEntityCostume(entityData);
        }

        // public static event Action<string> onSetWeaponCostume;
        public static void SetWeaponCostume(WeaponData weaponData, WeaponCostume costume){
            weaponData.costume = costume;
            // EntityManager.current.weaponCostumes[weaponName] = costumeName;
            Debug.Log("Set " + weaponData.name + " Weapon costume to : " + costume.name);

            weaponData.onChangeCostume?.Invoke();
            // if (onSetWeaponCostume != null)
            //     onSetWeaponCostume(weaponName);
        }
    }
}