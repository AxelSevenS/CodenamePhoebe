using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using TMPro;

using SeleneGame.Core;

using SevenGame.Utility;

namespace SeleneGame.UI {

    public class UIController : Singleton<UIController>{

        private BoolData keyBindMenuInput;
        private BoolData saveMenuInput;


        public void DisableAllMenus() {
            SaveMenuController.current.Disable();
            KeyBindingMenuController.current.Disable();
            WeaponInventoryMenuController.current.Disable();
        }

        public void UpdateMenuState(){

            bool menuUI = SaveMenuController.current.Enabled | KeyBindingMenuController.current.Enabled | WeaponInventoryMenuController.current.Enabled;

            if (menuUI) {
                ControlsManager.playerMap.Disable();
                PlayerEntityController.current.menu = true;
            } else {
                ControlsManager.playerMap.Enable();
                PlayerEntityController.current.menu = false;
            }

            bool gameInterrupted = menuUI && (KeyBindingMenuController.current.Enabled | SaveMenuController.current.Enabled);

            if ( gameInterrupted ){
                Time.timeScale = 0;
            } else {
                Time.timeScale = 1;
            }
        }



        private void OnEnable() { 
            SetCurrent();
        }

        private void Update() {
            
            keyBindMenuInput.SetVal( ControlsManager.debugMap.IsBindPressed("DebugKeyBindMenu") );
            if (keyBindMenuInput.started)
                KeyBindingMenuController.current.Toggle();

            saveMenuInput.SetVal( ControlsManager.debugMap.IsBindPressed("DebugSaveMenu") );
            if (saveMenuInput.started)
                SaveMenuController.current.Toggle();
        }

    }
}