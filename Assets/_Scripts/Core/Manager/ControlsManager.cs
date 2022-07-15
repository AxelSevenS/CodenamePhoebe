using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.InputSystem;

using SevenGame.Utility;

namespace SeleneGame.Core {

    public class ControlsManager : Singleton<ControlsManager> {
        
        public InputActionAsset actionAsset;


        public InputActionMap playerMap { get; private set; }
        public Dictionary<string, InputAction> playerBindings { get; private set; }

        #if UNITY_EDITOR
            public InputActionMap debugMap { get; private set; }
            public Dictionary<string, InputAction> debugBindings { get; private set; }
        #endif

        private void OnEnable() {
            SetCurrent();

            playerMap = actionAsset.FindActionMap("Player"); 

            playerBindings = new Dictionary<string, InputAction>();
            foreach (InputAction action in playerMap.actions) {
                playerBindings.Add(action.name, action);
            }

            #if UNITY_EDITOR
                debugMap = actionAsset.FindActionMap("Debug");

                debugBindings = new Dictionary<string, InputAction>();
                foreach (InputAction action in debugMap.actions) {
                    debugBindings.Add(action.name, action);
                }
            #endif

            EnableControls();

        }
        private void OnDisable() {
            DisableControls();
        }

        public void EnableControls(){
            playerMap.Enable();
            #if UNITY_EDITOR
                debugMap.Enable();
            #endif
        }
        public void DisableControls(){
            playerMap.Disable();
            #if UNITY_EDITOR
                debugMap.Disable();
            #endif
        }
    }
}
