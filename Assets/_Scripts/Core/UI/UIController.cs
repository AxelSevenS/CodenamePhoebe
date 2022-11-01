using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using TMPro;

using SeleneGame.Core;

using SevenGame.Utility;

namespace SeleneGame.Core.UI {

    public class UIController : Singleton<UIController>{

        public static IUIMenu currentMenu;

        private BoolData keyBindMenuInput;
        private BoolData saveMenuInput;



        public static bool IsMenuOpen => currentMenu != null;



        public void UpdateMenuState(){

            bool menuUI = currentMenu != null;

            if (menuUI) {
                ControlsManager.playerMap.Disable();
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            } else {
                ControlsManager.playerMap.Enable();
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }

            bool gameInterrupted = menuUI && currentMenu is IUIPausedMenu;

            if ( gameInterrupted ){
                Time.timeScale = 0;
            } else {
                Time.timeScale = 1;
            }
        }


        private void OnCancel() {
            currentMenu?.OnCancel();
        }

        private void OnControllerTypeChange(ControlsManager.ControllerType controllerType) {
            if ( controllerType != ControlsManager.ControllerType.MouseKeyboard )
                currentMenu?.ResetGamePadSelection();
        }



        private void OnEnable() {
            SetCurrent();
            UpdateMenuState();
            ControlsManager.onCancel += OnCancel;
            ControlsManager.onControllerTypeChange += OnControllerTypeChange;
        }

        private void OnDisable() {
            ControlsManager.onCancel -= OnCancel;
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