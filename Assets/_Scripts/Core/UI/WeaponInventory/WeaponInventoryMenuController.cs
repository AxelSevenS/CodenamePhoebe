using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using SevenGame.Utility;
using SevenGame.UI;

namespace SeleneGame.Core.UI {
    
    public class WeaponInventoryMenuController : UIModal<WeaponInventoryMenuController> {

        [SerializeField] private GameObject weaponInventoryMenu;
        [SerializeField] private GameObject weaponInventoryContainer;
        [SerializeField] private GameObject weaponSlotTemplate;
        
        [SerializeField] private List<WeaponSlot> weapons = new List<WeaponSlot>();

        [SerializeField] private ArmedEntity armedEntity;
        


        public void OpenInventory(ArmedEntity armedEntity) {

            if (armedEntity == null) {
                Debug.LogError("ArmedEntity is null");
                return;
            }

            this.armedEntity = armedEntity;


            Enable();

            GetEntityWeapons();
        }


        private void GetEntityWeapons(){

            if (armedEntity == null) {
                Debug.LogError("ArmedEntity is null");
                return;
            }

            weapons?.Clear();
            // Destroy all objects in the container
            foreach (Transform child in weaponInventoryContainer.transform) {
                Destroy(child.gameObject);
            }


            for (int i = 0; i < armedEntity.weapons.Count; i++) {
                CreateWeaponSlot(i, armedEntity);

                if (i != 0) continue;
                ResetGamePadSelection();
            }

        }

        private void CreateWeaponSlot(int index, ArmedEntity selectedEntity) {
            var slotObject = Instantiate(weaponSlotTemplate, weaponInventoryContainer.transform);
            var weaponSlot = slotObject.GetComponentInChildren<WeaponSlot>();

            weaponSlot.SetDisplayWeapon(selectedEntity.weapons[index]);
            weaponSlot.primaryAction = () => {
                WeaponSelectionMenuController.current.OpenSetEntityWeaponMenu(index, selectedEntity);
            };
            weaponSlot.secondaryAction = () => {
                WeaponCostumeSelectionMenuController.current.OpenSetWeaponCostumeMenu(index, selectedEntity);
            };
            
            weapons.Add( weaponSlot );

        }

        [ContextMenu("Enable")]
        public override void Enable() {

            base.Enable();

            weaponInventoryMenu.SetActive( true );
        }

        [ContextMenu("Disable")]
        public override void Disable() {
            base.Disable();

            weaponInventoryMenu.SetActive( false );
        }

        public override void Refresh() {
            GetEntityWeapons();
        }

        public override void EnableInteraction() {
            foreach (WeaponSlot weapon in weapons) {
                weapon.EnableInteraction();
            }
        }

        public override void DisableInteraction() {
            foreach (WeaponSlot weapon in weapons) {
                weapon.DisableInteraction();
            }
        }

        public override void ResetGamePadSelection(){
            EventSystem.current.SetSelectedGameObject(weapons[0].gameObject);
        }

    }
}