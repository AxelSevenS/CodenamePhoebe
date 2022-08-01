using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace SeleneGame.Core {
    
    public static class GameEvents{

        public static void Reset(){
            // onPlayerInput = null;
            onToggleMenu = null;
        }

        // public static event Action onPlayerInput;
        // public static void PlayerInput(){
        //     onPlayerInput?.Invoke();
        // }
        
        public static event Action onToggleMenu;
        public static void ToggleMenu(){
            onToggleMenu?.Invoke();
        }


        public static event Action<Guid> onUpdateKeybind;
        public static void UpdateKeybind(Guid keybindId){
            onUpdateKeybind?.Invoke(keybindId);
        }
        // public static void SetEntityCostume(Entity entity, EntityCostume costume){
        //     entity.SetCostume(costume);
        // }
        
        // public static void SetWeaponCostume(Weapon weapon, WeaponCostume costume){
        //     weapon.SetCostume(costume);
        // }
    }
}