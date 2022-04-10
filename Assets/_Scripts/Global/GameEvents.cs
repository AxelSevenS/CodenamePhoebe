using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SeleneGame {
    
    public class GameEvents{

        public static void Reset(){
            onPlayerInput = null;
            onToggleMenu = null;
            onStartDialogue = null;
            onSetEntityCostume = null;
            onSetWeaponCostume = null;
        }

        public static event Action onPlayerInput;
        public static void PlayerInput(){
            if (onPlayerInput != null){
                onPlayerInput(); 
            }
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

        public static event Action<string> onSetEntityCostume;
        public static void SetEntityCostume(string charName, string costumeName){
            UnitData.GetDataByName<EntityData>(charName).SetCostume(costumeName);
            EntityManager.current.entityCostumes[charName] = costumeName;
            Debug.Log("Set " + charName + " Entity costume to : " + costumeName);

            if (onSetEntityCostume != null)
                onSetEntityCostume(charName);
        }

        public static event Action<string> onSetWeaponCostume;
        public static void SetWeaponCostume(string weaponName, string costumeName){
            UnitData.GetDataByName<WeaponData>(weaponName).SetCostume(costumeName);
            EntityManager.current.weaponCostumes[weaponName] = costumeName;
            Debug.Log("Set " + weaponName + " Weapon costume to : " + costumeName);

            if (onSetWeaponCostume != null)
                onSetWeaponCostume(weaponName);
        }
    }
}