using System;
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

        public Action<Weapon.Instance> primaryAction;
        public Action<Weapon.Instance> secondaryAction;

        [SerializeField] private Image weaponPortrait;
        [SerializeField] private TextMeshProUGUI weaponName;



        public Weapon.Instance weapon {
            get => _weapon;
            set {
                _weapon = value;
                weaponPortrait.sprite = _weapon.costume.portrait;
                weaponName.text = _weapon.name;
            }
        }



        public override void OnPointerClick(PointerEventData eventData) {
            base.OnPointerClick(eventData);
            Debug.Log($"Weapon slot {weapon.name} clicked");
            if (eventData.button == PointerEventData.InputButton.Left) {
                primaryAction?.Invoke(weapon);
            } else if (eventData.button == PointerEventData.InputButton.Right) {
                secondaryAction?.Invoke(weapon);
            }
        }
    }
    
}
