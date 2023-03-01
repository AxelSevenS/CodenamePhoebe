using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;


namespace SeleneGame.Core.UI {

    public class CharacterCase : CustomButton {
        
        private CharacterData _characterData;

        [SerializeField] private Image characterCostumePortrait;
        [SerializeField] private TextMeshProUGUI characterCostumeName;



        public CharacterData characterData {
            get => _characterData;
            set {
                _characterData = value;
                characterCostumePortrait.sprite = _characterData.baseCostume.portrait;
                characterCostumeName.text = _characterData.displayName;
            }
        }



        public override void OnPointerClick(PointerEventData eventData) {
            base.OnPointerClick(eventData);
            Debug.Log($"Character case {_characterData.displayName} clicked");
            CharacterSelectionMenuController.current.OnSelectCharacter(_characterData);
        }
    }
    
}
