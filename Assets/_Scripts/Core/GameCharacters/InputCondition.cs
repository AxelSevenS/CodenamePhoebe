using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Scribe;
using SevenGame.Utility;

namespace SeleneGame.Core {

    public class InputCondition : ScribeCondition<InputCondition.InputConditionType> {
        
        [ScribeField] 
        public InputKey inputKey;

        [ScribeField((int)InputConditionType.Tap)] 
        [ScribeField((int)InputConditionType.Hold)] 
        [ScribeField((int)InputConditionType.DoubleTap)] 
        public float holdTime = 0.15f;

        [ScribeField((int)InputConditionType.DoubleTap)] 
        public InputKey doubleTapKey;



        public bool Evaluate(EntityController entityController) {

            KeyInputData keyInputData = GetInputData(entityController, inputKey);

            switch (conditionType) {
                default:
                case InputConditionType.KeyDown:
                    return keyInputData.started;
                case InputConditionType.KeyUp:
                    return keyInputData.stopped;
                case InputConditionType.Tap:
                    return keyInputData.Tapped(holdTime);
                case InputConditionType.Hold:
                    return keyInputData.Held(holdTime);
                case InputConditionType.DoubleTap:
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
            KeyDown,
            KeyUp,
            Tap,
            Hold,
            DoubleTap,
            
        }
    }
}
