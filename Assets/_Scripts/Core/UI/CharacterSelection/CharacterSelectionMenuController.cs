using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using SevenGame.Utility;
using System.Reflection;

namespace SeleneGame.Core.UI {
    
    public class CharacterSelectionMenuController : UIMenu<CharacterSelectionMenuController> {

        public const int CHARACTER_CASES_PER_ROW = 5;

        

        [SerializeField] private GameObject characterSelectionMenu;
        [SerializeField] private GameObject characterSelectionContainer;
        [SerializeField] private GameObject characterCaseTemplate;
        
        [SerializeField] private List<CharacterCase> characters = new List<CharacterCase>();

        private Entity currentEntity;

        private Action<CharacterData> onCharacterDataSelected;



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
            EventSystem.current.SetSelectedGameObject(characters[0].gameObject);
        }

        public override void OnCancel() {
            Disable();
        }
        

        public void ReplaceCharacter(Entity entity) {
            currentEntity = entity;

            onCharacterDataSelected = (selectedCharacterData) => {
                currentEntity.SetCharacter(selectedCharacterData);
                OnCancel();
            };

            GetCharacters();

            Enable();
        }


        public void OnSelectCharacter(CharacterData data) {
            if ( !Enabled ) return;
            onCharacterDataSelected?.Invoke( data );
        }

        private void GetCharacters() {

            foreach ( CharacterCase costume in characters ) {
                GameUtility.SafeDestroy(costume.gameObject);
            }
            characters = new();

            
            CreateCharacterCase(AddressablesUtils.GetDefaultAsset<CharacterData>());
            ResetGamePadSelection();

            AddressablesUtils.GetAssets<CharacterData>((data) => {

                    if (characters.Exists( (c) => c.characterData == data ))
                        return;

                    CreateCharacterCase(data);
                }

            );


        }

        private void CreateCharacterCase(CharacterData data){
            var caseObject = Instantiate(characterCaseTemplate, characterSelectionContainer.transform);
            var characterCase = caseObject.GetComponentInChildren<CharacterCase>();
            characterCase.characterData = data;
            characters.Add( characterCase );

            if (characters.Count > 1) {
                CharacterCase previousCase = characters[characters.Count - 2];
                previousCase.elementRight = characterCase;
                characterCase.elementLeft = previousCase;
            }

            if (characters.Count > CHARACTER_CASES_PER_ROW) {
                CharacterCase aboveCase = characters[characters.Count - (CHARACTER_CASES_PER_ROW + 1)];
                aboveCase.elementDown = characterCase;
                characterCase.elementUp = aboveCase;
            }

        }



        private void Awake() {
            characterSelectionContainer.GetComponent<GridLayoutGroup>().constraintCount = CHARACTER_CASES_PER_ROW;
        }

    }
}