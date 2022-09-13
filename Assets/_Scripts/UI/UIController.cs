using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using TMPro;

using SeleneGame.Core;

using SevenGame.Utility;

namespace SeleneGame.UI {

    public class UIController : Singleton<UIController>{

        public static IUIMenu currentMenu;

        private BoolData keyBindMenuInput;
        private BoolData saveMenuInput;


        public void UpdateMenuState(){

            bool menuUI = SaveMenuController.current.Enabled | KeyBindingMenuController.current.Enabled | WeaponInventoryMenuController.current.Enabled | WeaponSelectionMenuController.current.Enabled;

            if (menuUI) {
                ControlsManager.playerMap.Disable();
                PlayerEntityController.current.menu = true;
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            } else {
                ControlsManager.playerMap.Enable();
                PlayerEntityController.current.menu = false;
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }

            bool gameInterrupted = menuUI && ( KeyBindingMenuController.current.Enabled | SaveMenuController.current.Enabled );

            if ( gameInterrupted ){
                Time.timeScale = 0;
            } else {
                Time.timeScale = 1;
            }
        }


        private void OnCancelEvent() {
            currentMenu?.OnCancel();
        }

        private void OnControllerTypeChange(ControlsManager.ControllerType controllerType) {
            if ( controllerType != ControlsManager.ControllerType.MouseKeyboard )
                currentMenu?.ResetGamePadSelection();
        }



        private void OnEnable() { 
            SetCurrent();
            ControlsManager.onCancelEvent += OnCancelEvent;
            ControlsManager.onControllerTypeChange += OnControllerTypeChange;
        }

        private void OnDisable() {
            ControlsManager.onCancelEvent -= OnCancelEvent;
            ControlsManager.onControllerTypeChange += OnControllerTypeChange;
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