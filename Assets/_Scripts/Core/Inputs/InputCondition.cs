using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Scribe;
using SevenGame.Utility;

namespace SeleneGame.Core {

    [System.Serializable]
    public class InputCondition : ScribeCondition {
        
        public InputKey inputKey;

        [ScribeHideLabel]
        [ScribeOption] 
        public InputConditionType inputType;

        [ScribeField(nameof(inputType), (int)InputConditionType.Tapped)] 
        [ScribeField(nameof(inputType), (int)InputConditionType.Held)] 
        [ScribeField(nameof(inputType), (int)InputConditionType.DoubleTapped)] 
        public float holdTime = 0.15f;

        [ScribeHideLabel]
        [ScribeField(nameof(inputType), (int)InputConditionType.DoubleTapped)] 
        public InputKey doubleTapKey;



        public bool Evaluate(Player playerController) {

            if (binaryModifier == BinaryModifier.Always)
                return true;

            KeyInputData keyInputData = GetInputData(playerController, inputKey);

            bool postOperation = false;
            switch (inputType) {
                default:
                case InputConditionType.Actuated:
                    postOperation = keyInputData;
                    break;
                case InputConditionType.Started:
                    postOperation = keyInputData.started;
                    break;
                case InputConditionType.Stopped:
                    postOperation = keyInputData.stopped;
                    break;
                case InputConditionType.Tapped:
                    postOperation = keyInputData.Tapped(holdTime);
                    break;
                case InputConditionType.Held:
                    postOperation = keyInputData.Held(holdTime);
                    break;
                case InputConditionType.DoubleTapped:
                    KeyInputData doubleTapKeyInputData = GetInputData(playerController, doubleTapKey);
                    postOperation = keyInputData.SimultaneousTap(doubleTapKeyInputData, holdTime);
                    break;
            }

            return binaryModifier == BinaryModifier.If ? postOperation : !postOperation;
        }

        private KeyInputData GetInputData(Player entityController, InputKey key) {
            
            KeyInputData keyInputData;
            switch (key) {
                default:
                case InputKey.LightAttack:
                    keyInputData = entityController.lightAttackInput;
                    break;
                case InputKey.HeavyAttack:
                    keyInputData = entityController.heavyAttackInput;
                    break;
                case InputKey.Jump:
                    keyInputData = entityController.jumpInput;
                    break;
                case InputKey.Interact:
                    keyInputData = entityController.interactInput;
                    break;
                case InputKey.Evade:
                    keyInputData = entityController.evadeInput;
                    break;
                case InputKey.Walk:
                    keyInputData = entityController.walkInput;
                    break;
                case InputKey.Crouch:
                    keyInputData = entityController.crouchInput;
                    break;
                case InputKey.Focus:
                    keyInputData = entityController.focusInput;
                    break;
            }

            return keyInputData;
        }

        public enum InputKey {
            LightAttack,
            HeavyAttack,
            Jump,
            Interact,
            Evade,
            Walk,
            Crouch,
            Focus
        };

        public enum InputConditionType {
            Actuated,
            Started,
            Stopped,
            Tapped,
            Held,
            DoubleTapped,
            
        };
    }
}
