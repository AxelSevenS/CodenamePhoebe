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
    
    public class CharacterSelectionMenuController : UIModal<CharacterSelectionMenuController> {

        public const int CHARACTER_CASES_PER_ROW = 5;

        

        [SerializeField] private GameObject characterSelectionMenu;
        [SerializeField] private GameObject characterSelectionContainer;
        [SerializeField] private GameObject characterCaseTemplate;
        
        [SerializeField] private List<CharacterCase> characters = new List<CharacterCase>();

        private Action<CharacterData> onCharacterDataSelected;



        public void ReplaceCharacter(Entity entity) {

            onCharacterDataSelected = (selectedCharacterData) => OnSelectCharacter(selectedCharacterData, entity);

            Enable();

            GetCharacters();


            void OnSelectCharacter(CharacterData characterData, Entity entity) {
                if (!Enabled)
                    return;

                entity.SetCharacter(characterData);
                OnCancel();
            }
        }


        public void SelectCharacter(CharacterData data) {
            if ( !Enabled ) return;
            onCharacterDataSelected?.Invoke( data );
        }

        private void GetCharacters() {

            if (characters == null) {
                characters = new List<CharacterCase>();
            } else if (characters.Count > 0) {
                ResetGamePadSelection();
                return;
            }

            
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
            GetCharacters();
        }

        public override void EnableInteraction() {
            foreach (CharacterCase character in characters) {
                character.EnableInteraction();
            }
        }

        public override void DisableInteraction() {
            foreach (CharacterCase character in characters) {
                character.DisableInteraction();
            }
        }

        public override void ResetGamePadSelection(){
            EventSystem.current.SetSelectedGameObject(characters[0].gameObject);
        }

        public override void OnCancel() {
            Disable();
        }
        

        
        private void Awake() {
            characterSelectionContainer.GetComponent<GridLayoutGroup>().constraintCount = CHARACTER_CASES_PER_ROW;
        }

    }
}