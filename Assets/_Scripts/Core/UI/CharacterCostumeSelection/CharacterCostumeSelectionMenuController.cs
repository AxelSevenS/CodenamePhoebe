using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using SevenGame.Utility;
using SevenGame.UI;

namespace SeleneGame.Core.UI {
    
    public class CharacterCostumeSelectionMenuController : UIModal<CharacterCostumeSelectionMenuController> {

        public const int CHARACTER_COSTUME_CASES_PER_ROW = 5;

        

        [SerializeField] private GameObject characterSelectionMenu;
        [SerializeField] private GameObject characterCostumeSelectionContainer;
        [SerializeField] private GameObject characterCostumeCaseTemplate;
        
        [SerializeField] private List<CharacterCostumeCase> characterCostumes = new List<CharacterCostumeCase>();

        private Action<CharacterCostume> onCharacterCostumeSelected;

        [SerializeField] private Character character;


        public void ReplaceCharacterCostume(Character character) {

            if (character == null) {
                Debug.LogError("Character is null!");
                return;
            }

            this.character = character;


            onCharacterCostumeSelected = (selectedCostume) => OnSelectCharacterCostume(selectedCostume, this.character);

            Enable();

            GetEquippableCostumes();


            void OnSelectCharacterCostume(CharacterCostume selectedCostume, Character character) {
                if (!Enabled)
                    return;

                character.SetCostume(selectedCostume);
                OnCancel();
            }
        }


        public void SelectCharacterCostume(CharacterCostume characterCostume) {
            if ( !Enabled ) return;
            onCharacterCostumeSelected?.Invoke( characterCostume );
        }

        private void GetEquippableCostumes() {

            if (characterCostumes == null) {
                characterCostumes = new List<CharacterCostumeCase>();
            } else if (characterCostumes.Count > 0) {
                ResetGamePadSelection();
                return;
            }

            // Get the Default Costume (corresponds to an empty slot, should be in the first space)
            CreateCharacterCostumeCase( character.Data.baseCostume );
            ResetGamePadSelection();

            // and then get all the other costumes.
            AddressablesUtils.GetAssets<CharacterCostume>( (costume) => {

                    if ( !costume.accessibleInGame ) return;

                    if ( !costume.name.Contains(character.Data.name) && costume.name.Contains("_Base") )
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
        }


        public override void Enable() {

            base.Enable();

            characterSelectionMenu.SetActive( true );
        }

        public override void Disable() {

            base.Disable();

            characterSelectionMenu.SetActive( false );
        }

        public override void Refresh() {
            GetEquippableCostumes();
        }

        public override void EnableInteraction() {
            foreach (CharacterCostumeCase characterCostume in characterCostumes) {
                characterCostume.EnableInteraction();
            }
        }

        public override void DisableInteraction() {
            foreach (CharacterCostumeCase characterCostume in characterCostumes) {
                characterCostume.DisableInteraction();
            }
        }

        public override void ResetGamePadSelection(){
            EventSystem.current.SetSelectedGameObject(characterCostumes[0].gameObject);
        }

        public override void OnCancel() {
            Disable();
        }


        private void Awake() {
            characterCostumeSelectionContainer.GetComponent<GridLayoutGroup>().constraintCount = CHARACTER_COSTUME_CASES_PER_ROW;
        }

    }
}