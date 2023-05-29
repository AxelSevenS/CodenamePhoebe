using System.Collections.Generic;

using UnityEngine;

using Scribe;

using SeleneGame.Core.UI;

namespace SeleneGame.Core {

    [System.Serializable]
    public class GameAction : ScribeAction {

        [ScribeHideLabel]
        [ScribeOption] 
        public GameAction.EventType eventType;
        
        [ScribeHideLabel]
        [ScribeField(nameof(eventType), (int)GameAction.EventType.EditFlag)]
        public EditFlagAction editFlagAction;
        

        [ScribeHideLabel]
        [ScribeField(nameof(eventType), (int)GameAction.EventType.StartAlert)]
        [ScribeField(nameof(eventType), (int)GameAction.EventType.StartDialogue)]
        // [ScribeField(nameof(eventType), (int)GameAction.EventType.SkipToLine)]
        public DialogueSource dialogueSource;

        [ScribeField(nameof(eventType), (int)GameAction.EventType.SetCharacterCostume)]
        public string targetCharacterId;

        [ScribeHideLabel]
        [ScribeField(nameof(eventType), (int)GameAction.EventType.SetCharacterCostume)]
        public CharacterCostume targetCharacterCostume;

        [ScribeField(nameof(eventType), (int)GameAction.EventType.SetWeaponCostume)]
        public string targetWeaponId;
        
        [ScribeHideLabel]
        [ScribeField(nameof(eventType), (int)GameAction.EventType.SetWeaponCostume)]
        public WeaponCostume targetWeaponCostume;

        public void Invoke(GameObject eventObject) {

            switch (eventType) {
                case GameAction.EventType.EditFlag:
                    editFlagAction.Invoke();
                    break;
                case GameAction.EventType.StartDialogue:
                    DialogueController.current.StartDialogue(dialogueSource?.GetDialogue(), eventObject);
                    break;
                case GameAction.EventType.StartAlert:
                    AlertController.current.StartDialogue(dialogueSource?.GetDialogue(), eventObject);
                    break;
                // case GameAction.EventType.SkipToLine:
                //     UIController.currentDialogueReader.SkipToLine(dialogueSource);
                //     break;
                case GameAction.EventType.EndDialogue:
                    UIController.currentDialogueReader.EndDialogue();
                    break;
                case GameAction.EventType.SetCharacterCostume:
                    // Character character;
                    // if (targetCharacterId == "Player") {
                    //     character = PlayerEntityController.current.entity.character;
                    // } else {
                    //     character = Character.GetInstanceWithId(targetCharacterId);
                    // }
                    // character?.SetCostume(targetCharacterCostume);
                    break;
                case GameAction.EventType.SetWeaponCostume:
                    // Weapon weapon;
                    // weapon = Weapon.GetInstanceWithId(targetWeaponId);
                    // weapon?.SetCostume(targetWeaponCostume);
                    break;
                case GameAction.EventType.Destroy:
                    Object.Destroy(eventObject);
                    break;
            }
        }

        public enum EventType {
            EditFlag,
            StartDialogue,
            StartAlert,
            // SkipToLine,
            EndDialogue,
            SetCharacterCostume,
            SetWeaponCostume,
            Destroy
        }
    }

}