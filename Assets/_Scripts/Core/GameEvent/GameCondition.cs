using UnityEngine;

using Scribe;

namespace SeleneGame.Core {

    [System.Serializable]
    public class GameCondition : ScribeCondition {

        [ScribeHideLabel]
        [ScribeOption]
        public ConditionType conditionType;

        [ScribeHideLabel]
        [ScribeField(nameof(conditionType), (int)ConditionType.Flag)]
        public FlagCondition flagCondition;


        [ScribeHideLabel]
        [ScribeField(nameof(conditionType), (int)ConditionType.DialogueFlag)]
        public DialogueSource dialogueSource;

        [ScribeHideLabel]
        [ScribeField(nameof(conditionType), (int)ConditionType.DialogueFlag)]
        public DialogueFlag dialogueFlag;



        public bool Evaluate() {

            if (binaryModifier == BinaryModifier.Always)
                return true;
            
            bool postOperation = false;
            switch (conditionType) {
                case ConditionType.Flag:
                    postOperation = flagCondition.Evaluate();
                    break;
                case ConditionType.DialogueFlag:
                    postOperation = dialogueSource.GetFlag(dialogueFlag);
                    break;
            }

            return binaryModifier == BinaryModifier.If ? postOperation : !postOperation;
        }



        public enum ConditionType {
            Flag,
            DialogueFlag,
        }
    }
}