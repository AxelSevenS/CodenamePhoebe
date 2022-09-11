using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

using SeleneGame.Core;

namespace SeleneGame.UI {

    public class WeaponSlot : CustomButton {
        
        private Weapon.Instance weapon;

        [SerializeField] private Image weaponPortrait;
        [SerializeField] private TextMeshProUGUI weaponName;


        public void SetWeapon(Weapon.Instance weapon) {
            this.weapon = weapon;
            weaponPortrait.sprite = weapon.costume.portrait;
            weaponName.text = weapon.name;
        }

        public override void OnPointerClick(PointerEventData eventData) {
            base.OnPointerClick(eventData);
            Debug.Log($"Weapon slot {weapon.name} clicked");
        }
    }
    
}
