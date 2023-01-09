using System.Collections.Generic;

using UnityEngine;

using SeleneGame.Core.UI;

namespace SeleneGame.Core {

    [System.Serializable]
    public class GameEvent {

        public EventCondition condition;
        public EventSubCondition[] subConditions;
        
        public GameManager.FlagType editedFlagType;
        public string editedFlagName;
        public int editedFlagValue;

        public EventType eventType;
        
        public DialogueLine targetLine;

        public string targetCharacterId;
        public CharacterCostume targetCharacterCostume;

        public string targetWeaponId;
        public WeaponCostume targetWeaponCostume;


        public GameEvent() {
            foreach( EventType val in System.Enum.GetValues(typeof(EventType)) ) {
                if ( DisplayEventType(val) ) {
                    eventType = val;
                    break;
                }
            }
        }


        public virtual bool DisplayEventType(System.Enum enumValue) {
            return true;
        }
        

        public void Invoke(GameObject dialogueObject) {

            if (condition.conditionType != EventCondition.ConditionType.Always) {
                bool conditionsMet = condition.Evaluate();
                foreach (EventSubCondition subCondition in subConditions) {
                    conditionsMet = subCondition.Evaluate(conditionsMet);
                }

                if (!conditionsMet) return;
                
            }


            switch (eventType) {
                case EventType.SetFlag:
                    GameManager.SetFlag(editedFlagName, editedFlagValue, editedFlagType == GameManager.FlagType.TemporaryFlag);
                    break;
                case EventType.RemoveFlag:
                    GameManager.RemoveFlag(editedFlagName, editedFlagType == GameManager.FlagType.TemporaryFlag);
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