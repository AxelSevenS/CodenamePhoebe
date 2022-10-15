using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using SeleneGame.Core;

using SevenGame.Utility;

namespace SeleneGame.UI {
    
    public class WeaponInventoryMenuController : UIMenu<WeaponInventoryMenuController> {

        [SerializeField] private GameObject weaponInventoryMenu;
        [SerializeField] private GameObject weaponInventoryContainer;
        [SerializeField] private GameObject weaponSlotTemplate;
        
        [SerializeField] private List<WeaponSlot> weapons = new List<WeaponSlot>();

        private ArmedEntity selectedEntity;

        public void OpenInventory(ArmedEntity armedEntity) {
            selectedEntity = armedEntity;
            Enable();
        }

        [ContextMenu("Enable")]
        public override void Enable() {

            base.Enable();

            GetEntityWeapons();

            weaponInventoryMenu.SetActive( true );

            ResetGamePadSelection();

            UIController.current.UpdateMenuState();
        }

        [ContextMenu("Disable")]
        public override void Disable() {
            base.Disable();

            weaponInventoryMenu.SetActive( false );

            UIController.current.UpdateMenuState();
        }

        public override void ResetGamePadSelection(){
            EventSystem.current.SetSelectedGameObject(weapons[0].gameObject);
        }


        private void GetEntityWeapons(){
            weapons = new List<WeaponSlot>();
            int childCount = weaponInventoryContainer.transform.childCount;
            for (int i = 0; i < childCount; i++) {
                Transform child = weaponInventoryContainer.transform.GetChild(0);
                GameUtility.SafeDestroy(child.gameObject);
            }

            // if (PlayerEntityController.current.entity is ArmedEntity armed){

            foreach (Weapon weapon in selectedEntity.weapons){
                CreateWeaponSlot(weapon);
            }

            // } else {
            //     Disable();
            //     return;
            // }

            // WeaponSlot lastButton = weapons[weapons.Count - 1];
            // lastButton.elementDown = returnButton;
            // returnButton.elementUp = lastButton;

        }

        private void CreateWeaponSlot(Weapon weapon){
            var slotObject = Instantiate(weaponSlotTemplate, weaponInventoryContainer.transform);
            var weaponSlot = slotObject.GetComponentInChildren<WeaponSlot>();
            weaponSlot.weapon = weapon;
            weaponSlot.primaryAction = (weapon) => {
                WeaponSelectionMenuController.current.ReplaceWeapon(weapon, selectedEntity);
            };
            weaponSlot.secondaryAction = (weapon) => {
                WeaponCostumeSelectionMenuController.current.ReplaceWeaponCostume(weapon);
            };
            
            weapons.Add( weaponSlot );
            if (weapons.Count > 1) {
                WeaponSlot previousSlot = weapons[weapons.Count - 2];
                previousSlot.elementDown = weaponSlot;
                weaponSlot.elementUp = previousSlot;
            }

        }

    }
}