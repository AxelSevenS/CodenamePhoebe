using System.Collections.Generic;

using UnityEngine;

using SeleneGame.Core.UI;

namespace SeleneGame.Core {

    [System.Serializable]
    public class DialogueEvent {

        public ConditionType conditionType;
        public FlagType flagType;
        public string flagName;
        public OperatorType operatorType;
        public int flagValue;

        public EventType eventType;
        
        public FlagType editedFlagType;
        public string editedFlagName;
        public int editedFlagValue;
        
        public DialogueLine targetLine;

        public string targetCharacterId;
        public CharacterCostume targetCharacterCostume;

        public string targetWeaponId;
        public WeaponCostume targetWeaponCostume;
        

        
        public bool Evaluate() {

            if (conditionType == ConditionType.Always)
                return true;

            int flagValue = Dialogue.GetFlag(flagName, flagType == FlagType.TemporaryFlag);

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

        public void Invoke(GameObject dialogueObject) {

            if (!Evaluate()) return;

            switch (eventType) {
                case EventType.SetFlag:
                    Dialogue.SetFlag(editedFlagName, editedFlagValue, editedFlagType == FlagType.TemporaryFlag);
                    break;
                case EventType.RemoveFlag:
                    Dialogue.RemoveFlag(editedFlagName, editedFlagType == FlagType.TemporaryFlag);
                    break;
                case EventType.StartDialogue:
                    DialogueController.current.StartDialogue(targetLine, dialogueObject);
                    break;
                case EventType.StartAlert:
                    AlertController.current.StartDialogue(targetLine, dialogueObject);
                    break;
                case EventType.SkipToLine:
                    UIController.currentDialogueReader.SkipToLine(targetLine);
                    break;
                case EventType.EndDialogue:
                    UIController.currentDialogueReader.EndDialogue();
                    break;
                case EventType.SetCharacterCostume:
                    Character character;
                    if (targetCharacterId == "Player") {
                        character = PlayerEntityController.current.entity.character;
                    } else {
                        character = Character.GetInstanceWithId(targetCharacterId);
                    }
                    character?.SetCostume(targetCharacterCostume);
                    break;
                case EventType.SetWeaponCostume:
                    Weapon weapon;
                    weapon = Weapon.GetInstanceWithId(targetWeaponId);
                    weapon?.SetCostume(targetWeaponCostume);
                    break;
                case EventType.Destroy:
                    Object.Destroy(dialogueObject);
                    break;
            }
        }


        public enum ConditionType {
            If,
            IfNot,
            Always
        }

        public enum FlagType {
            GlobalFlag,
            TemporaryFlag
        }

        public enum OperatorType {
            Equals,
            NotEquals,
            GreaterThan,
            LessThan,
            GreaterThanOrEquals,
            LessThanOrEquals
        }

        public enum EventType {
            SetFlag,
            RemoveFlag,
            StartDialogue,
            StartAlert,
            SkipToLine,
            EndDialogue,
            SetCharacterCostume,
            SetWeaponCostume,
            Destroy
        }
    }

}