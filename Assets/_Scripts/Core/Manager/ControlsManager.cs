using System;

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.DualShock;
using UnityEngine.InputSystem.XInput;
using UnityEngine.InputSystem.Switch;

using SevenGame.Utility;

namespace SeleneGame.Core {

    public class ControlsManager : Singleton<ControlsManager> {
        

        public const float cameraSpeed = 0.1f;
        public float mouseSpeed = 1f;
        public float stickSpeed = 5f;

        private static InputControls _inputControls;
        public static InputControls inputControls {
            get {
                if (_inputControls == null)
                    _inputControls = new InputControls();
                return _inputControls;
            }
        }

        public static InputActionMap playerMap => inputControls.Player.Get();
        public static InputActionMap uiMap => inputControls.UI.Get();

        #if UNITY_EDITOR

            public static InputActionMap debugMap => inputControls.Debug.Get();

        #endif

        public static ControllerType controllerType = ControllerType.MouseKeyboard;


        private KeyInputData cancelInput;

        public static event Action onCancelEvent;
        public static event Action<ControllerType> onControllerTypeChange;
        public static event Action<Guid> onUpdateKeybind;



        private void ControllerAction(InputAction.CallbackContext context){

            if ( context.control.device is Keyboard || context.control.device is Mouse ){
                if (controllerType == ControllerType.MouseKeyboard) return;

                controllerType = ControllerType.MouseKeyboard;
                Debug.Log("Switched to Keyboard and Mouse Controls.");

            } else if ( context.control.device is Gamepad gamepad ){

                if ( gamepad is DualShockGamepad ) {
                    if (controllerType == ControllerType.Dualshock) return;

                    controllerType = ControllerType.Dualshock;
                    Debug.Log("Switched to Dualshock Controls.");
                } else if ( gamepad is XInputController ) {
                    if (controllerType == ControllerType.Xbox) return;
                    
                    controllerType = ControllerType.Xbox;
                    Debug.Log("Switched to Xbox Controls.");
                } else if ( gamepad is SwitchProControllerHID ) {
                    if (controllerType == ControllerType.Switch) return;
                    
                    controllerType = ControllerType.Switch;
                    Debug.Log("Switched to Switch Controls.");
                } else {
                    if (controllerType == ControllerType.Gamepad) return;
                    
                    controllerType = ControllerType.Gamepad;
                    Debug.Log("Switched to Standard Gamepad Controls.");
                }

            }

            onControllerTypeChange?.Invoke(controllerType);
        }

        public static InputBinding GetInputBinding(string actionName){
            switch (controllerType) {
                default:
                    return GetMouseAndKeyboardInputBinding(actionName);
                case ControllerType.Gamepad:
                    return GetGamepadInputBinding(actionName);
                case ControllerType.Dualshock:
                    return GetDualshockInputBinding(actionName);
                case ControllerType.Xbox:
                    return GetXboxInputBinding(actionName);
                case ControllerType.Switch:
                    return GetSwitchInputBinding(actionName);
            }
        }

        public static InputBinding GetMouseAndKeyboardInputBinding(string actionName){
            return playerMap.FindAction(actionName).bindings[0];
        }

        public static InputBinding GetGamepadInputBinding(string actionName){
            return playerMap.FindAction(actionName).bindings[1];
        }

        public static InputBinding GetDualshockInputBinding(string actionName){
            return playerMap.FindAction(actionName).bindings[2];
        }

        public static InputBinding GetXboxInputBinding(string actionName){
            return playerMap.FindAction(actionName).bindings[3];
        }

        public static InputBinding GetSwitchInputBinding(string actionName){
            return playerMap.FindAction(actionName).bindings[4];
        }

        public void EnableControls(){
            playerMap.Enable();
            uiMap.Enable();
            #if UNITY_EDITOR
                debugMap.Enable();
            #endif
        }
        public void DisableControls(){
            playerMap.Disable();
            uiMap.Disable();
            #if UNITY_EDITOR
                debugMap.Disable();
            #endif
        }

        public static void UpdateKeybind(Guid keybindId){
            onUpdateKeybind?.Invoke(keybindId);
        }






        private void Update() {
            cancelInput.SetVal( uiMap.IsBindPressed("Cancel") ) ;

            if (cancelInput.started) {
                onCancelEvent?.Invoke();
            }
        }

        private void OnEnable() {
            SetCurrent();

            EnableControls();
            playerMap.actionTriggered += ctx => ControllerAction(ctx);
            uiMap.actionTriggered += ctx => ControllerAction(ctx);

        }
        private void OnDisable() {
            DisableControls();
            playerMap.actionTriggered -= ctx => ControllerAction(ctx);
            uiMap.actionTriggered -= ctx => ControllerAction(ctx);
        }


        public enum ControllerType{ MouseKeyboard, Gamepad, Dualshock, Xbox, Switch };
    }
}
