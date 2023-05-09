using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using SevenGame.Utility;
using System.Reflection;

namespace SeleneGame.Core.UI {
    
    public class WeaponSelectionMenuController : UIMenu<WeaponSelectionMenuController> {

        public const int WEAPON_CASES_PER_ROW = 5;

        
        [SerializeField] [HideInInspector] private ArmedEntity currentEntity;

        [SerializeField] private GameObject weaponSelectionMenu;
        [SerializeField] private GameObject weaponSelectionContainer;
        [SerializeField] private GameObject weaponCaseTemplate;
        
        [SerializeField] private List<WeaponCase> weapons = new List<WeaponCase>();

        public Action<WeaponData> onWeaponDataSelected { get; private set; }


        // private static void 



        public override void Enable() {

            base.Enable();

            weaponSelectionMenu.SetActive( true );

            UIController.current.UpdateMenuState();
        }

        public override void Disable() {

            base.Disable();

            weaponSelectionMenu.SetActive( false );

            UIController.current.UpdateMenuState();
        }

        public override void ResetGamePadSelection(){
            SetSelected(weapons[0].gameObject);
        }

        public override void OnCancel() {
            WeaponInventoryMenuController.current.OpenInventory(currentEntity);
        }
        

        public void OpenSetEntityWeaponMenu(int index, ArmedEntity armedEntity) {

            currentEntity = armedEntity;

            onWeaponDataSelected = (selectedWeapon) => SetEntityWeapon(selectedWeapon, index, armedEntity);

            Enable();

            GetAllAvailableWeapons();
        }


        private void SetEntityWeapon(WeaponData selectedWeapon, int index, ArmedEntity armedEntity) {
            if (!Enabled)
                return;

            armedEntity.weapons.Set(index, selectedWeapon);
            OnCancel();
        }


        private void GetAllAvailableWeapons() {

            if (weapons == null) {
                weapons = new List<WeaponCase>();
            } else if (weapons.Count > 0) {
                ResetGamePadSelection();
                return;
            }

            
            CreateWeaponCase(null);

            AddressablesUtils.GetAssets<WeaponData>((data) => {

                    if (weapons.Exists( (c) => c.weaponData == data ))
                        return;

                    CreateWeaponCase(data);
                }

            );
            ResetGamePadSelection();


        }

        private void CreateWeaponCase(WeaponData weapon, int equippedIndex = -1){
            var caseObject = Instantiate(weaponCaseTemplate, weaponSelectionContainer.transform);
            var weaponCase = caseObject.GetComponentInChildren<WeaponCase>();

            weaponCase.SetDisplayWeapon(weapon);
            weapons.Add( weaponCase );
        }



        private void Awake() {
            weaponSelectionContainer.GetComponent<GridLayoutGroup>().constraintCount = WEAPON_CASES_PER_ROW;
        }

    }
}