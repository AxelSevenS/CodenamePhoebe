using System;

using UnityEngine;
using UnityEngine.UI;
using TMPro;

using SeleneGame.Core;

using SevenGame.Utility;

namespace SeleneGame.Core.UI {

    [DefaultExecutionOrder(-100)]
    public class UIController : Singleton<UIController> {

        // public static IUIMenu currentMenu;
        public static IUIMenu modalLeaf;
        public static IDialogueReader currentDialogueReader;

        // public static bool IsMenuOpen => currentMenu != null;


        // public event Action onCancel;

        

        private BoolData keyBindMenuInput;
        private BoolData saveMenuInput;


        public static void DisableModalTree() {
            IUIMenu branch = modalLeaf;
            while ( branch is IUIModal modalBranch && modalBranch.previousModal != null) {
                branch = modalBranch.previousModal;
                branch.Disable();
            }
            branch.Disable();
            modalLeaf = null;
        }

        public IUIMenu GetBaseMenu() {
            IUIMenu branch = modalLeaf;
            while ( branch is IUIModal modalBranch && modalBranch.previousModal != null) {
                branch = modalBranch.previousModal;
            }
            return branch;
        }


        public void Cancel() {
            // onCancel?.Invoke();
            modalLeaf?.OnCancel();
        }

        public void UpdateMenuState(){

            IUIMenu currentMenu = GetBaseMenu();

            bool menuUI = currentMenu != null;

            if (menuUI) {
                Keybinds.playerMap.Disable();
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            } else {
                Keybinds.playerMap.Enable();
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }

            bool gameInterrupted = menuUI && currentMenu is IPausedMenu;

            if ( gameInterrupted ){
                Time.timeScale = 0;
            } else {
                Time.timeScale = 1;
            }
        }


        private void OnControllerTypeChange(Keybinds.ControllerType controllerType) {
            // if ( controllerType != Keybinds.ControllerType.MouseKeyboard )
            //     currentMenu?.ResetGamePadSelection();
        }



        private void OnEnable() {
            SetCurrent();
            UpdateMenuState();
            Keybinds.onControllerTypeChange += OnControllerTypeChange;
        }

        private void OnDisable() {
            Keybinds.onControllerTypeChange += OnControllerTypeChange;
        }

        private void Update() {
           
        // #if UNITY_EDITOR 
            keyBindMenuInput.SetVal( Keybinds.debugMap.IsBindPressed("DebugKeyBindMenu") );
            if (keyBindMenuInput.started)
                KeyBindingMenuController.current.Toggle();

            saveMenuInput.SetVal( Keybinds.debugMap.IsBindPressed("DebugSaveMenu") );
            if (saveMenuInput.started)
                SaveMenuController<GameSaveData>.current.Toggle();
        // #endif
        }

    }
}