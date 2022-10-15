using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

using SeleneGame.Core;

namespace SeleneGame.UI {

    public class WeaponCostumeCase : CustomButton {
        
        private WeaponCostume _weaponCostume;

        [SerializeField] private Image weaponCostumePortrait;
        [SerializeField] private TextMeshProUGUI weaponCostumeName;



        public WeaponCostume weaponCostume {
            get => _weaponCostume;
            set {
                _weaponCostume = value;
                weaponCostumePortrait.sprite = _weaponCostume.portrait;
                weaponCostumeName.text = _weaponCostume.displayName;
            }
        }



        public override void OnPointerClick(PointerEventData eventData) {
            base.OnPointerClick(eventData);
            Debug.Log($"Weapon case {_weaponCostume.name} clicked");
            WeaponCostumeSelectionMenuController.current.OnSelectWeaponCostume(_weaponCostume);
        }
    }
    
}
