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
        
        private Weapon _weapon;

        public Action<Weapon> primaryAction;
        public Action<Weapon> secondaryAction;

        [SerializeField] private Image weaponPortrait;
        [SerializeField] private TextMeshProUGUI weaponName;



        public Weapon weapon {
            get => _weapon;
            set {
                _weapon = value;
                if ( _weapon?.costume?.portrait != null )
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
