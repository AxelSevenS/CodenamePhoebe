using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

namespace SeleneGame.Core.UI {

    public class WeaponCase : CustomButton {
        
        private Weapon _weapon;

        [SerializeField] private Image weaponPortrait;
        [SerializeField] private TextMeshProUGUI weaponName;



        public Weapon weapon {
            get => _weapon;
            set {
                _weapon = value;
                weaponPortrait.sprite = _weapon.baseCostume.portrait;
                weaponName.text = _weapon.name;
            }
        }



        public override void OnPointerClick(PointerEventData eventData) {
            base.OnPointerClick(eventData);
            Debug.Log($"Weapon case {_weapon.name} clicked");
            WeaponSelectionMenuController.current.OnSelectWeapon(_weapon);
        }
    }
    
}
