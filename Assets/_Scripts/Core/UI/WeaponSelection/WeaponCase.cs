using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

namespace SeleneGame.Core.UI {

    public class WeaponCase : CustomButton {
        
        private WeaponData _weaponData;

        [SerializeField] private Image weaponPortrait;
        [SerializeField] private TextMeshProUGUI weaponName;

        [SerializeField] private Sprite nullPortrait;



        public WeaponData weaponData => _weaponData;

        public Sprite portraitSprite {
            get => weaponPortrait.sprite;
            set => weaponPortrait.sprite = value;
        }

        public string nameText {
            get => weaponName.text;
            set => weaponName.text = value;
        }


        public void SetDisplayWeapon(WeaponData data) {

            _weaponData = data ?? AddressablesUtils.GetDefaultAsset<WeaponData>();

            portraitSprite = _weaponData.baseCostume?.portrait ?? nullPortrait;
            nameText = _weaponData?.displayName ?? "None";
        }



        public override void OnSubmit(BaseEventData eventData) {
            base.OnSubmit(eventData);
            Debug.Log($"Weapon case {weaponName.text} submitted");
            WeaponSelectionMenuController.current.onWeaponDataSelected?.Invoke(_weaponData);
        }
    }
    
}
