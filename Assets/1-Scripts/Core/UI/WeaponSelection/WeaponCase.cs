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

        [SerializeField] private Sprite nullPortrait;



        public Sprite portraitSprite {
            get => weaponPortrait.sprite;
            set => weaponPortrait.sprite = value;
        }

        public string nameText {
            get => weaponName.text;
            set => weaponName.text = value;
        }


        public void SetDisplayWeapon(Weapon weapon) {
            _weapon = weapon;
            portraitSprite = weapon?.baseCostume?.portrait ?? nullPortrait;
            nameText = weapon?.displayName ?? "None";
        }



        public override void OnPointerClick(PointerEventData eventData) {
            base.OnPointerClick(eventData);
            Debug.Log($"Weapon case {weaponName.text} clicked");
            WeaponSelectionMenuController.current.OnSelectWeapon(_weapon);
        }
    }
    
}
