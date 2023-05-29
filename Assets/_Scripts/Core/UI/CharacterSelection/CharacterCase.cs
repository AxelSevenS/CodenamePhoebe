using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using TMPro;

using SevenGame.UI;


namespace SeleneGame.Core.UI {

    public sealed class CharacterCase : CustomButton {
        
        private CharacterData _characterData;

        [SerializeField] private Image characterCostumePortrait;
        [SerializeField] private TextMeshProUGUI characterCostumeName;



        public CharacterData characterData {
            get => _characterData;
            set {
                _characterData = value;
                characterCostumePortrait.sprite = _characterData.baseCostume.portrait;
                characterCostumeName.text = _characterData.displayName.GetLocalizedString();
            }
        }



        public override void OnSubmit(BaseEventData eventData) {
            if (!interactable) return;

            base.OnSubmit(eventData);
            Debug.Log($"Character case {_characterData.displayName} clicked");
            CharacterSelectionMenuController.current.SelectCharacter(_characterData);
        }
    }
    
}
