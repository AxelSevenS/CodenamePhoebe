using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using SevenGame.Utility;

namespace SeleneGame.Core.UI {
    
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
            weapons?.Clear();
            // Destroy all objects in the container
            foreach (Transform child in weaponInventoryContainer.transform) {
                Destroy(child.gameObject);
            }

            // if (PlayerEntityController.current.entity is ArmedEntity armed){

            for (int i = 0; i < selectedEntity.weapons.Count; i++) {
                CreateWeaponSlot(i, selectedEntity);
            }

            // } else {
            //     Disable();
            //     return;
            // }

            // WeaponSlot lastButton = weapons[weapons.Count - 1];
            // lastButton.elementDown = returnButton;
            // returnButton.elementUp = lastButton;

        }

        private void CreateWeaponSlot(int index, ArmedEntity selectedEntity) {
            var slotObject = Instantiate(weaponSlotTemplate, weaponInventoryContainer.transform);
            var weaponSlot = slotObject.GetComponentInChildren<WeaponSlot>();

            weaponSlot.SetDisplayWeapon(selectedEntity.weapons[index]);
            weaponSlot.primaryAction = () => {
                WeaponSelectionMenuController.current.ReplaceWeapon(index, selectedEntity);
            };
            weaponSlot.secondaryAction = () => {
                WeaponCostumeSelectionMenuController.current.ReplaceWeaponCostume(index, selectedEntity);
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