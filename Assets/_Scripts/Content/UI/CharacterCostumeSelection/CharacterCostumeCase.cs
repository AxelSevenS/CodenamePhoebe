using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

using SeleneGame.Core;

namespace SeleneGame.UI {

    public class CharacterCostumeCase : CustomButton {
        
        private CharacterCostume _characterCostume;

        [SerializeField] private Image characterCostumePortrait;
        [SerializeField] private TextMeshProUGUI characterCostumeName;



        public CharacterCostume characterCostume {
            get => _characterCostume;
            set {
                _characterCostume = value;
                characterCostumePortrait.sprite = _characterCostume.portrait;
                characterCostumeName.text = _characterCostume.displayName;
            }
        }



        public override void OnPointerClick(PointerEventData eventData) {
            base.OnPointerClick(eventData);
            Debug.Log($"Character case {_characterCostume.name} clicked");
            CharacterCostumeSelectionMenuController.current.OnSelectCharacterCostume(_characterCostume);
        }
    }
    
}
