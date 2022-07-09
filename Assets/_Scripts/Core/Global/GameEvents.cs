using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SeleneGame.UI;

namespace SeleneGame {
    
    public static class GameEvents{

        public static void Reset(){
            onPlayerInput = null;
            onToggleMenu = null;
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

        public static void StartDialogue(Dialogue dialogue, GameObject dialogueObject){
            DialogueController.current.StartDialogue(dialogue, dialogueObject);
        }
        // public static void SetEntityCostume(Entity entity, EntityCostume costume){
        //     entity.SetCostume(costume);
        // }
        
        // public static void SetWeaponCostume(Weapon weapon, WeaponCostume costume){
        //     weapon.SetCostume(costume);
        // }
    }
}