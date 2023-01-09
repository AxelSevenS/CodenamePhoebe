using System.Collections.Generic;

using UnityEngine;

using SeleneGame.Core.UI;

namespace SeleneGame.Core {

    [System.Serializable]
    public class EventCondition {

        public ConditionType conditionType;
        public GameManager.FlagType flagType;
        public string flagName;
        public OperatorType operatorType;
        public int flagValue;



        public bool Evaluate() {

            if (conditionType == ConditionType.Always)
                return true;

            int flagValue = GameManager.GetFlag(flagName, flagType == GameManager.FlagType.TemporaryFlag);

            bool postOperation = false;
            switch (operatorType) {
                case OperatorType.Equals:
                    postOperation = flagValue == this.flagValue;
                    break;
                case OperatorType.NotEquals:
                    postOperation = flagValue != this.flagValue;
                    break;
                case OperatorType.GreaterThan:
                    postOperation = flagValue > this.flagValue;
                    break;
                case OperatorType.LessThan:
                    postOperation = flagValue < this.flagValue;
                    break;
                case OperatorType.GreaterThanOrEquals:
                    postOperation = flagValue >= this.flagValue;
                    break;
                case OperatorType.LessThanOrEquals:
                    postOperation = flagValue <= this.flagValue;
                    break;
            }

            return conditionType == ConditionType.If ? postOperation : !postOperation;
        }


        public enum ConditionType {
            If,
            IfNot,
            Always
        }

        public enum OperatorType {
            Equals,
            NotEquals,
            GreaterThan,
            LessThan,
            GreaterThanOrEquals,
            LessThanOrEquals
        }
    }
}
