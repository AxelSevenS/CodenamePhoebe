using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using SeleneGame.Core;

using SevenGame.Utility;

namespace SeleneGame.UI {
    
    public class WeaponInventoryMenuController : MenuController<WeaponInventoryMenuController> {

        [SerializeField] private GameObject weaponInventoryMenu;
        [SerializeField] private GameObject weaponInventoryContainer;
        [SerializeField] private GameObject weaponSlotTemplate;
        
        [SerializeField] private List<WeaponSlot> weapons = new List<WeaponSlot>();



        [ContextMenu("Enable")]
        public override void Enable() {
            if (!Enabled)
                UIController.current.DisableAllMenus();

            base.Enable();

            GetPlayerWeapons();

            weaponInventoryMenu.SetActive( true );

            SetSelectedObject();

            UIController.current.UpdateMenuState();
        }

        [ContextMenu("Disable")]
        public override void Disable() {
            base.Disable();

            weaponInventoryMenu.SetActive( false );

            UIController.current.UpdateMenuState();
        }

        public override void SetSelectedObject(){
            if ( Enabled && ControlsManager.controllerType != ControlsManager.ControllerType.MouseKeyboard )
                EventSystem.current.SetSelectedGameObject(weapons[0].gameObject);
        }


        private void GetPlayerWeapons(){
            weapons = new List<WeaponSlot>();
            int childCount = weaponInventoryContainer.transform.childCount;
            for (int i = 0; i < childCount; i++) {
                Transform child = weaponInventoryContainer.transform.GetChild(0);
                GameUtility.SafeDestroy(child.gameObject);
            }

            if (PlayerEntityController.current.entity is ArmedEntity armed){

                foreach (Weapon.Instance weapon in armed.weapons){
                    CreateWeaponSlot(weapon);
                }

            } else {
                Disable();
                return;
            }

            // WeaponSlot lastButton = weapons[weapons.Count - 1];
            // lastButton.elementDown = returnButton;
            // returnButton.elementUp = lastButton;

        }

        private void CreateWeaponSlot(Weapon.Instance weapon){
            var slotObject = Instantiate(weaponSlotTemplate, weaponInventoryContainer.transform);
            var weaponSlot = slotObject.GetComponentInChildren<WeaponSlot>();
            weaponSlot.weapon = weapon;
            
            weapons.Add( weaponSlot );
            if (weapons.Count > 1) {
                WeaponSlot previousSlot = weapons[weapons.Count - 2];
                previousSlot.elementDown = weaponSlot;
                weaponSlot.elementUp = previousSlot;
            }

        }

    }
}