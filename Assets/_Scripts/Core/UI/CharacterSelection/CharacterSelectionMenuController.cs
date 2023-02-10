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

        private Action<Character> onCharacterSelected;



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

            onCharacterSelected = (selectedCharacter) => {
                currentEntity.SetCharacter(selectedCharacter.GetType());
                OnCancel();
            };

            GetCharacters();

            Enable();
        }


        public void OnSelectCharacter(Character character) {
            if ( !Enabled ) return;
            onCharacterSelected?.Invoke( character );
        }

        private void GetCharacters() {

            foreach ( CharacterCase costume in characters ) {
                GameUtility.SafeDestroy(costume.gameObject);
            }
            characters = new();

            
            CreateCharacterCase(new SeleneCharacter(null, null));
            ResetGamePadSelection();

            foreach (Type characterType in Character._types) {
                if (characterType == typeof(SeleneCharacter)) 
                    continue;

                ConstructorInfo constructor = characterType.GetConstructor( new Type[] {typeof(Entity), typeof(CharacterCostume)});
                Character weapon = constructor.Invoke( new object[] {null, null} ) as Character;
                CreateCharacterCase(weapon);
            }


        }

        private void CreateCharacterCase(Character character){
            var caseObject = Instantiate(characterCaseTemplate, characterSelectionContainer.transform);
            var characterCase = caseObject.GetComponentInChildren<CharacterCase>();
            characterCase.character = character;
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