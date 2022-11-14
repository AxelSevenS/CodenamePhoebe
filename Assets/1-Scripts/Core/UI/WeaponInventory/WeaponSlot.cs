using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

namespace SeleneGame.Core.UI {

    public class WeaponSlot : CustomButton {

        public Action primaryAction;
        public Action secondaryAction;

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
            portraitSprite = weapon?.costume?.portrait ?? nullPortrait;
            nameText = weapon?.displayName ?? "None";
        }

        public override void OnPointerClick(PointerEventData eventData) {
            base.OnPointerClick(eventData);
            Debug.Log($"Weapon slot {nameText} clicked");
            if (eventData.button == PointerEventData.InputButton.Left) {
                primaryAction?.Invoke();
            } else if (eventData.button == PointerEventData.InputButton.Right) {
                secondaryAction?.Invoke();
            }
        }
    }
    
}
