using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.InputSystem;

using SevenGame.Utility;

namespace SeleneGame.Core {

    public class ControlsManager : Singleton<ControlsManager> {
        
        public InputActionAsset actionAsset;

        public const float cameraSpeed = 0.1f;
        public float mouseSpeed = 1f;
        public float stickSpeed = 5f;


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
            ControlsManager.current.playerMap.actionTriggered += ctx => ControllerAction(ctx);

        }
        private void OnDisable() {
            DisableControls();
            ControlsManager.current.playerMap.actionTriggered -= ctx => ControllerAction(ctx);
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



        public enum ControllerType{ MouseKeyboard, Controller };

        public ControllerType controllerType = ControllerType.MouseKeyboard;

        private void ControllerAction(InputAction.CallbackContext context){
            string path = context.control.path;

            if ( path.Contains("Keyboard") || path.Contains("Mouse")  ){
                if (controllerType == ControllerType.MouseKeyboard) return;

                controllerType = ControllerType.MouseKeyboard;
                Debug.Log("Switched to Keyboard and Mouse Controls.");

            }else{
                if (controllerType == ControllerType.Controller) return;
                
                controllerType = ControllerType.Controller;
                Debug.Log("Switched to Controller Controls.");
            }
        }
    }
}
