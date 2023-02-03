using System;

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.DualShock;
using UnityEngine.InputSystem.XInput;
using UnityEngine.InputSystem.Switch;

// using SevenGame.Utility;

namespace SeleneGame.Core {
    public static class Keybinds {
        
        public const float HOLD_TIME = 0.2f;

        public static bool IsBindPressed(this InputActionMap controlMap, string bindName) => controlMap[bindName].ReadValue<float>() > 0;
        public static bool IsActuated(this InputAction action) => action.ReadValue<float>() > 0;


        public static float mouseSpeed = 1f;
        public static float stickSpeed = 5f;

        public static float cameraSpeed = 0.1f;


        public static ControllerType controllerType = ControllerType.MouseKeyboard;


        public static event Action<ControllerType> onControllerTypeChange;
        public static event Action<Guid> onUpdateKeybind;


        private static readonly InputControls inputControls = new InputControls();

        public static readonly InputActionMap playerMap = inputControls.Player.Get();
        public static readonly InputActionMap uiMap = inputControls.UI.Get();
        #if UNITY_EDITOR
            public static readonly InputActionMap debugMap = inputControls.Debug.Get();
        #endif


        static Keybinds() {
            EnableControls();
            playerMap.actionTriggered += ctx => ControllerAction(ctx);
            uiMap.actionTriggered += ctx => ControllerAction(ctx);
        }


        public static void UpdateKeybind(Guid keybindId){
            Debug.Log("Update Keybind: " + keybindId);
            onUpdateKeybind?.Invoke(keybindId);
        }

        public static InputBinding GetInputBinding(string actionName) {
            return GetInputBinding(playerMap.FindAction(actionName));
        }

        public static InputBinding GetInputBinding(InputAction action){
            switch (controllerType) {
                default:
                    return GetMouseAndKeyboardInputBinding(action);
                case ControllerType.Gamepad:
                    return GetGamepadInputBinding(action);
                case ControllerType.Dualshock:
                    return GetDualshockInputBinding(action);
                case ControllerType.Xbox:
                    return GetXboxInputBinding(action);
                case ControllerType.Switch:
                    return GetSwitchInputBinding(action);
            }
        }
        public static InputBinding GetMouseAndKeyboardInputBinding(InputAction action){
            return action.bindings[0];
        }
        public static InputBinding GetGamepadInputBinding(InputAction action){
            return action.bindings[1];
        }
        public static InputBinding GetDualshockInputBinding(InputAction action){
            return action.bindings[2];
        }
        public static InputBinding GetXboxInputBinding(InputAction action){
            return action.bindings[3];
        }
        public static InputBinding GetSwitchInputBinding(InputAction action){
            return action.bindings[4];
        }


        public static string GetKeybindDisplay(InputBinding binding) {
            return binding.ToDisplayString();
        }
        public static string GetKeybindDisplay(InputAction action) {
            return (GetKeybindDisplay(GetInputBinding(action.name)));
        }

        public static string GetMouseAndKeyboardKeybindDisplay(InputAction action) {
            return GetKeybindDisplay(GetMouseAndKeyboardInputBinding(action));
        }
        public static string GetGamepadKeybindDisplay(InputAction action) {
            return GetKeybindDisplay(GetGamepadInputBinding(action));
        }
        public static string GetDualshockKeybindDisplay(InputAction action) {
            return GetKeybindDisplay(GetDualshockInputBinding(action));
        }
        public static string GetXboxKeybindDisplay(InputAction action) {
            return GetKeybindDisplay(GetXboxInputBinding(action));
        }
        public static string GetSwitchKeybindDisplay(InputAction action) {
            return GetKeybindDisplay(GetSwitchInputBinding(action));
        }


        private static void ControllerAction(InputAction.CallbackContext context){

            ControllerType newControllerType = ControllerType.MouseKeyboard;

            if ( context.control.device is Keyboard || context.control.device is Mouse ){

                controllerType = ControllerType.MouseKeyboard;
                
            } else if ( context.control.device is Gamepad gamepad ){

                if ( gamepad is DualShockGamepad ) {
                    controllerType = ControllerType.Dualshock;
                } else if ( gamepad is XInputController ) {
                    controllerType = ControllerType.Xbox;
                } else if ( gamepad is SwitchProControllerHID ) {
                    controllerType = ControllerType.Switch;
                } else {
                    controllerType = ControllerType.Gamepad;
                }

            }

            if (controllerType == newControllerType) return;

            controllerType = newControllerType;
            Debug.Log($"Switched to {newControllerType.ToString()} Controls");
            onControllerTypeChange?.Invoke(controllerType);
        }

        public static void EnableControls(){
            playerMap.Enable();
            uiMap.Enable();
            #if UNITY_EDITOR
                debugMap.Enable();
            #endif
        }
        public static void DisableControls(){
            playerMap.Disable();
            uiMap.Disable();
            #if UNITY_EDITOR
                debugMap.Disable();
            #endif
        }



        public enum ControllerType{ MouseKeyboard, Gamepad, Dualshock, Xbox, Switch };

    }
}
