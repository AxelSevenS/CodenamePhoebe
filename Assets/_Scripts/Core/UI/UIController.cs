using System;

using UnityEngine;
using UnityEngine.UI;
using TMPro;

using SeleneGame.Core;

using SevenGame.Utility;

namespace SeleneGame.Core.UI {

    [DefaultExecutionOrder(-100)]
    public class UIController : Singleton<UIController> {

        public static IUIMenu currentMenu { get; private set; }
        public static IUIMenu modalLeaf;
        public static IUIMenu modalRoot;
        public static IDialogueReader currentDialogueReader;
        

        private BoolData keyBindMenuInput;
        private BoolData saveMenuInput;
        private BoolData loadMenuInput;



        public static void DisableModalTree() {
            // Disable all menus in the modal tree
            IUIMenu branch = modalLeaf;
            while ( branch is IUIModal branchModal && branch != null ) {
                branch.Disable();
                branch = branchModal.previousModal;
            }
            modalLeaf = null;
        }

        public static IUIMenu GetBaseMenu() {
            IUIMenu branch = modalLeaf;
            while ( branch is IUIModal modalBranch && modalBranch.previousModal != null) {
                branch = modalBranch.previousModal;
            }
            return branch;
        }


        public static void Cancel() {
            modalLeaf?.OnCancel();
        }

        public static void UpdateMenuState(){

            currentMenu = GetBaseMenu();

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
            if ( controllerType != Keybinds.ControllerType.MouseKeyboard )
                modalLeaf?.ResetGamePadSelection();
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
           
        #if UNITY_EDITOR 
            keyBindMenuInput.SetVal( Keybinds.debugMap.IsBindPressed("DebugKeyBindMenu") );
            if (keyBindMenuInput.started)
                KeyBindingMenuController.current.Toggle();

            saveMenuInput.SetVal( Keybinds.debugMap.IsBindPressed("DebugSaveMenu") );
            if (saveMenuInput.started)
                SaveMenuController<GameSaveData>.current.OpenSaveDataMenu();

            loadMenuInput.SetVal( Keybinds.debugMap.IsBindPressed(bindName: "DebugLoadMenu") );
            if (loadMenuInput.started)
                SaveMenuController<GameSaveData>.current.OpenLoadDataMenu();
        #endif

        }

    }
}