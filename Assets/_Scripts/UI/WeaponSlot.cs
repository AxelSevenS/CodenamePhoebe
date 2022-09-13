using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

using SeleneGame.Core;

namespace SeleneGame.UI {

    public class WeaponSlot : CustomButton {
        
        private Weapon.Instance _weapon;

        [SerializeField] private Image weaponPortrait;
        [SerializeField] private TextMeshProUGUI weaponName;



        public Weapon.Instance weapon {
            get => _weapon;
            set {
                _weapon = value;
                weaponPortrait.sprite = _weapon.baseCostume.portrait;
                weaponName.text = _weapon.name;
            }
        }



        public override void OnPointerClick(PointerEventData eventData) {
            base.OnPointerClick(eventData);
            Debug.Log($"Weapon slot {weapon.name} clicked");
            WeaponSelectionMenuController.current.ReplaceWeapon( weapon );
        }
    }
    
}
