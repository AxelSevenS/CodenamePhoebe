using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;


namespace SeleneGame.Core.UI {

    public sealed class CharacterCostumeCase : CustomButton {
        
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



        public override void OnSubmit(BaseEventData eventData) {
            if (!interactable) return;

            base.OnSubmit(eventData);
            Debug.Log(message: $"Character case {_characterCostume.name} clicked");
            CharacterCostumeSelectionMenuController.current.SelectCharacterCostume(_characterCostume);
        }
    }
    
}
