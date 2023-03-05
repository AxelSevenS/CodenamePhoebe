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



        public bool Evaluate(EntityController entityController) {

            KeyInputData keyInputData = GetInputData(entityController, inputKey);

            switch (inputType) {
                default:
                case InputConditionType.Actuated:
                    return keyInputData;
                case InputConditionType.Started:
                    return keyInputData.started;
                case InputConditionType.Stopped:
                    return keyInputData.stopped;
                case InputConditionType.Tapped:
                    return keyInputData.Tapped(holdTime);
                case InputConditionType.Held:
                    return keyInputData.Held(holdTime);
                case InputConditionType.DoubleTapped:
                    KeyInputData doubleTapKeyInputData = GetInputData(entityController, doubleTapKey);
                    return keyInputData.SimultaneousTap(doubleTapKeyInputData, holdTime);
            }
        }

        private KeyInputData GetInputData(EntityController entityController, InputKey key) {
            
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
                case InputKey.Shift:
                    keyInputData = entityController.shiftInput;
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
            Focus,
            Shift
        }

        public enum InputConditionType {
            Actuated,
            Started,
            Stopped,
            Tapped,
            Held,
            DoubleTapped,
            
        }
    }
}
