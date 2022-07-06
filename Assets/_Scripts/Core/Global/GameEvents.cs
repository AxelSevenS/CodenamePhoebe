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
        // public static void SetEntityCostume(Entity entity, EntityCostume costume){
        //     entity.SetCostume(costume);
        // }
        
        // public static void SetWeaponCostume(Weapon weapon, WeaponCostume costume){
        //     weapon.SetCostume(costume);
        // }
    }
}