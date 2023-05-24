using System;

using UnityEngine;
using UnityEngine.UI;
using TMPro;

using SeleneGame.Core;

using SevenGame.Utility;
using SevenGame.UI;

namespace SeleneGame.Core.UI {

    [DefaultExecutionOrder(-100)]
    public class UIController : Singleton<UIController> {
        

        private BoolData keyBindMenuInput;
        private BoolData saveMenuInput;
        private BoolData loadMenuInput;

        public static IDialogueReader currentDialogueReader;


        private void OnControllerTypeChange(Keybinds.ControllerType controllerType) {
            if ( controllerType != Keybinds.ControllerType.MouseKeyboard )
                UIManager.modalLeaf?.ResetGamePadSelection();
        }



        private void OnEnable() {
            SetCurrent();
            UIManager.UpdateMenuState();
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