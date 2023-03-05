using System.Collections.Generic;

using UnityEngine;

using Scribe;

using SeleneGame.Core.UI;

namespace SeleneGame.Core {

    [System.Serializable]
    public class GameEvent : ScribeEvent<FlagCondition> {

        [ScribeHideLabel]
        [ScribeOption] 
        public GameEvent.EventType eventType;
        
        [ScribeHideLabel]
        [ScribeField(nameof(eventType), (int)GameEvent.EventType.SetFlag)]
        [ScribeField(nameof(eventType), (int)GameEvent.EventType.RemoveFlag)]
        public ScribeFlags.FlagType editedFlagType;

        [ScribeField(nameof(eventType), (int)GameEvent.EventType.SetFlag)]
        [ScribeField(nameof(eventType), (int)GameEvent.EventType.RemoveFlag)]
        public string editedFlagName;

        [ScribeField(nameof(eventType), (int)GameEvent.EventType.SetFlag)]
        public int editedFlagValue;
        

        [ScribeHideLabel]
        [ScribeField(nameof(eventType), (int)GameEvent.EventType.StartAlert)]
        [ScribeField(nameof(eventType), (int)GameEvent.EventType.StartDialogue)]
        [ScribeField(nameof(eventType), (int)GameEvent.EventType.SkipToLine)]
        public DialogueSource dialogueSource;

        [ScribeField(nameof(eventType), (int)GameEvent.EventType.SetCharacterCostume)]
        public string targetCharacterId;

        [ScribeHideLabel]
        [ScribeField(nameof(eventType), (int)GameEvent.EventType.SetCharacterCostume)]
        public CharacterCostume targetCharacterCostume;

        [ScribeField(nameof(eventType), (int)GameEvent.EventType.SetWeaponCostume)]
        public string targetWeaponId;
        
        [ScribeHideLabel]
        [ScribeField(nameof(eventType), (int)GameEvent.EventType.SetWeaponCostume)]
        public WeaponCostume targetWeaponCostume;

        public void Invoke(GameObject eventObject) {

            switch (eventType) {
                case GameEvent.EventType.SetFlag:
                    ScribeFlags.SetFlag(editedFlagName, editedFlagValue, editedFlagType == ScribeFlags.FlagType.TemporaryFlag);
                    break;
                case GameEvent.EventType.RemoveFlag:
                    ScribeFlags.RemoveFlag(editedFlagName, editedFlagType == ScribeFlags.FlagType.TemporaryFlag);
                    break;
                case GameEvent.EventType.StartDialogue:
                    DialogueController.current.StartDialogue(dialogueSource, eventObject);
                    break;
                case GameEvent.EventType.StartAlert:
                    AlertController.current.StartDialogue(dialogueSource, eventObject);
                    break;
                case GameEvent.EventType.SkipToLine:
                    UIController.currentDialogueReader.SkipToLine(dialogueSource);
                    break;
                case GameEvent.EventType.EndDialogue:
                    UIController.currentDialogueReader.EndDialogue();
                    break;
                case GameEvent.EventType.SetCharacterCostume:
                    // Character character;
                    // if (targetCharacterId == "Player") {
                    //     character = PlayerEntityController.current.entity.character;
                    // } else {
                    //     character = Character.GetInstanceWithId(targetCharacterId);
                    // }
                    // character?.SetCostume(targetCharacterCostume);
                    break;
                case GameEvent.EventType.SetWeaponCostume:
                    // Weapon weapon;
                    // weapon = Weapon.GetInstanceWithId(targetWeaponId);
                    // weapon?.SetCostume(targetWeaponCostume);
                    break;
                case GameEvent.EventType.Destroy:
                    Object.Destroy(eventObject);
                    break;
            }
        }

        public bool Evaluate() {
            bool left = conditions.condition.Evaluate();
            foreach (ScribeSubCondition<FlagCondition> subCondition in conditions.subConditions) {
                left = subCondition.MultiConditionEvaluate(left, subCondition.condition.Evaluate());
            }
            return left;
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