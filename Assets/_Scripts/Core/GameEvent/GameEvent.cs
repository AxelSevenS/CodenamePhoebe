using System.Collections.Generic;

using UnityEngine;

using Scribe;

using SeleneGame.Core.UI;

namespace SeleneGame.Core {

    [System.Serializable]
    public class GameEvent : ScribeEvent<GameEvent.EventType> {
        
        [ScribeEventData((int)GameEvent.EventType.SetFlag)]
        [ScribeEventData((int)GameEvent.EventType.RemoveFlag)]
        public ScribeFlags.FlagType editedFlagType;
        [ScribeEventData((int)GameEvent.EventType.SetFlag)]
        [ScribeEventData((int)GameEvent.EventType.RemoveFlag)]
        public string editedFlagName;
        [ScribeEventData((int)GameEvent.EventType.SetFlag)]
        public int editedFlagValue;
        
        [ScribeEventData((int)GameEvent.EventType.StartAlert)]
        [ScribeEventData((int)GameEvent.EventType.StartDialogue)]
        [ScribeEventData((int)GameEvent.EventType.SkipToLine)]
        public DialogueSource dialogueSource;

        [ScribeEventData((int)GameEvent.EventType.SetCharacterCostume)]
        public string targetCharacterId;
        [ScribeEventData((int)GameEvent.EventType.SetCharacterCostume)]
        public CharacterCostume targetCharacterCostume;

        [ScribeEventData((int)GameEvent.EventType.SetWeaponCostume)]
        public string targetWeaponId;
        [ScribeEventData((int)GameEvent.EventType.SetWeaponCostume)]
        public WeaponCostume targetWeaponCostume;

        

        public override void Invoke(GameObject dialogueObject) { 

            switch (eventType) {
                case GameEvent.EventType.SetFlag:
                    ScribeFlags.SetFlag(editedFlagName, editedFlagValue, editedFlagType == ScribeFlags.FlagType.TemporaryFlag);
                    break;
                case GameEvent.EventType.RemoveFlag:
                    ScribeFlags.RemoveFlag(editedFlagName, editedFlagType == ScribeFlags.FlagType.TemporaryFlag);
                    break;
                case GameEvent.EventType.StartDialogue:
                    DialogueController.current.StartDialogue(dialogueSource, dialogueObject);
                    break;
                case GameEvent.EventType.StartAlert:
                    AlertController.current.StartDialogue(dialogueSource, dialogueObject);
                    break;
                case GameEvent.EventType.SkipToLine:
                    UIController.currentDialogueReader.SkipToLine(dialogueSource);
                    break;
                case GameEvent.EventType.EndDialogue:
                    UIController.currentDialogueReader.EndDialogue();
                    break;
                case GameEvent.EventType.SetCharacterCostume:
                    Character character;
                    if (targetCharacterId == "Player") {
                        character = PlayerEntityController.current.entity.character;
                    } else {
                        character = Character.GetInstanceWithId(targetCharacterId);
                    }
                    character?.SetCostume(targetCharacterCostume);
                    break;
                case GameEvent.EventType.SetWeaponCostume:
                    Weapon weapon;
                    weapon = Weapon.GetInstanceWithId(targetWeaponId);
                    weapon?.SetCostume(targetWeaponCostume);
                    break;
                case GameEvent.EventType.Destroy:
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