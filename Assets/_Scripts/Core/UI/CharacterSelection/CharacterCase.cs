using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;


namespace SeleneGame.Core.UI {

    public class CharacterCase : CustomButton {
        
        private Character _character;

        [SerializeField] private Image characterCostumePortrait;
        [SerializeField] private TextMeshProUGUI characterCostumeName;



        public Character character {
            get => _character;
            set {
                _character = value;
                characterCostumePortrait.sprite = _character.baseCostume.portrait;
                characterCostumeName.text = _character.displayName;
            }
        }



        public override void OnPointerClick(PointerEventData eventData) {
            base.OnPointerClick(eventData);
            Debug.Log($"Character case {_character.displayName} clicked");
            CharacterSelectionMenuController.current.OnSelectCharacter(_character);
        }
    }
    
}
