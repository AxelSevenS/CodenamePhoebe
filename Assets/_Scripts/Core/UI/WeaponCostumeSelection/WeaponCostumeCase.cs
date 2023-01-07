using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

namespace SeleneGame.Core.UI {

    public class WeaponCostumeCase : CustomButton {
        
        private WeaponCostume _weaponCostume;

        [SerializeField] private Image weaponCostumePortrait;
        [SerializeField] private TextMeshProUGUI weaponCostumeName;



        [SerializeField] private Sprite nullPortrait;

        public Sprite portraitSprite {
            get => weaponCostumePortrait.sprite;
            set => weaponCostumePortrait.sprite = value;
        }

        public string nameText {
            get => weaponCostumeName.text;
            set => weaponCostumeName.text = value;
        }


        public void SetDisplayWeaponCostume(WeaponCostume weaponCostume) {
            _weaponCostume = weaponCostume;
            portraitSprite = weaponCostume?.portrait ?? nullPortrait;
            nameText = weaponCostume?.displayName ?? "None";
        }



        public override void OnPointerClick(PointerEventData eventData) {
            base.OnPointerClick(eventData);
            Debug.Log($"Weapon case {nameText} clicked");
            WeaponCostumeSelectionMenuController.current.OnSelectWeaponCostume(_weaponCostume);
        }
    }
    
}
