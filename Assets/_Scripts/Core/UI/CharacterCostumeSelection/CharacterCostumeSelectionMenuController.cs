using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using SevenGame.Utility;

namespace SeleneGame.Core.UI {
    
    public class CharacterCostumeSelectionMenuController : UIMenu<CharacterCostumeSelectionMenuController> {

        public const int CHARACTER_COSTUME_CASES_PER_ROW = 5;

        

        [SerializeField] private GameObject characterSelectionMenu;
        [SerializeField] private GameObject characterCostumeSelectionContainer;
        [SerializeField] private GameObject characterCostumeCaseTemplate;
        
        [SerializeField] private List<CharacterCostumeCase> characterCostumes = new List<CharacterCostumeCase>();

        private Character currentCharacter;

        private Action<CharacterCostume> onCharacterCostumeSelected;



        public override void Enable() {

            base.Enable();

            characterSelectionMenu.SetActive( true );

            UIController.current.UpdateMenuState();
        }

        public override void Disable() {

            base.Disable();

            characterSelectionMenu.SetActive( false );

            UIController.current.UpdateMenuState();
        }

        public override void ResetGamePadSelection(){
            EventSystem.current.SetSelectedGameObject(characterCostumes[0].gameObject);
        }

        public override void OnCancel() {
            Disable();
        }
        

        public void ReplaceCharacterCostume(Character character) {
            currentCharacter = character;

            onCharacterCostumeSelected = (selectedCostume) => {
                currentCharacter.SetCostume(selectedCostume);
                OnCancel();
            };

            GetEquippableCostumes();

            Enable();
        }


        public void OnSelectCharacterCostume(CharacterCostume characterCostume) {
            if ( !Enabled ) return;
            onCharacterCostumeSelected?.Invoke( characterCostume );
        }

        private void GetEquippableCostumes() {

            foreach ( CharacterCostumeCase costume in characterCostumes ) {
                GameUtility.SafeDestroy(costume.gameObject);
            }
            characterCostumes = new();

            // Get the Default Costume (corresponds to an empty slot, should be in the first space)
            CreateCharacterCostumeCase( currentCharacter.baseCostume );
            ResetGamePadSelection();

            // and then get all the other costumes.
            CharacterCostume.GetAssets( (costume) => {

                    if ( !costume.accessibleInGame ) return;

                    if ( !costume.name.Contains(currentCharacter.name) && costume.name.Contains("_Base") )
                        return;

                    if ( characterCostumes.Exists( (obj) => { return obj.characterCostume == costume; }) ) 
                        return;

                    CreateCharacterCostumeCase(costume);
                }
            );


        }

        private void CreateCharacterCostumeCase(CharacterCostume costume){
            var caseObject = Instantiate(characterCostumeCaseTemplate, characterCostumeSelectionContainer.transform);
            var costumeCase = caseObject.GetComponentInChildren<CharacterCostumeCase>();
            costumeCase.characterCostume = costume;
            characterCostumes.Add( costumeCase );

            if (characterCostumes.Count > 1) {
                CharacterCostumeCase previousCase = characterCostumes[characterCostumes.Count - 2];
                previousCase.elementRight = costumeCase;
                costumeCase.elementLeft = previousCase;
            }

            if (characterCostumes.Count > CHARACTER_COSTUME_CASES_PER_ROW) {
                CharacterCostumeCase aboveCase = characterCostumes[characterCostumes.Count - (CHARACTER_COSTUME_CASES_PER_ROW + 1)];
                aboveCase.elementDown = costumeCase;
                costumeCase.elementUp = aboveCase;
            }

        }



        private void Awake() {
            characterCostumeSelectionContainer.GetComponent<GridLayoutGroup>().constraintCount = CHARACTER_COSTUME_CASES_PER_ROW;
        }

    }
}