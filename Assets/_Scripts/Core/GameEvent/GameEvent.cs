using System.Collections.Generic;

using UnityEngine;

using Scribe;

using SeleneGame.Core.UI;

namespace SeleneGame.Core {

    [System.Serializable]
    public class GameEvent : ScribeEvent<GameEvent.EventType, FlagCondition> {

        private GameObject eventObject;
        
        [ScribeField((int)GameEvent.EventType.SetFlag)]
        [ScribeField((int)GameEvent.EventType.RemoveFlag)]
        public ScribeFlags.FlagType editedFlagType;
        [ScribeField((int)GameEvent.EventType.SetFlag)]
        [ScribeField((int)GameEvent.EventType.RemoveFlag)]
        public string editedFlagName;
        [ScribeField((int)GameEvent.EventType.SetFlag)]
        public int editedFlagValue;
        
        [ScribeField((int)GameEvent.EventType.StartAlert)]
        [ScribeField((int)GameEvent.EventType.StartDialogue)]
        [ScribeField((int)GameEvent.EventType.SkipToLine)]
        public DialogueSource dialogueSource;

        [ScribeField((int)GameEvent.EventType.SetCharacterCostume)]
        public string targetCharacterId;
        [ScribeField((int)GameEvent.EventType.SetCharacterCostume)]
        public CharacterCostume targetCharacterCostume;

        [ScribeField((int)GameEvent.EventType.SetWeaponCostume)]
        public string targetWeaponId;
        [ScribeField((int)GameEvent.EventType.SetWeaponCostume)]
        public WeaponCostume targetWeaponCostume;

        public void InvokeEvent(GameObject eventObject) {
            this.eventObject = eventObject;
            Invoke();
        }

        protected override void Invoke() {
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