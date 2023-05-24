using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using SevenGame.Utility;
using SevenGame.UI;
using System.Reflection;

namespace SeleneGame.Core.UI {
    
    public class WeaponSelectionMenuController : UIModal<WeaponSelectionMenuController> {

        public const int WEAPON_CASES_PER_ROW = 5;


        [SerializeField] private GameObject weaponSelectionMenu;
        [SerializeField] private GameObject weaponSelectionContainer;
        [SerializeField] private GameObject weaponCaseTemplate;
        
        [SerializeField] private List<WeaponCase> weapons = new List<WeaponCase>();

        public Action<WeaponData> onWeaponDataSelected { get; private set; }

        

        public void OpenSetEntityWeaponMenu(int index, ArmedEntity armedEntity) {

            if (armedEntity == null) {
                Debug.LogError("Armed Entity is null!");
                return;
            }


            onWeaponDataSelected = (selectedWeapon) => OnSetEntityWeapon(selectedWeapon, index, armedEntity);

            Enable();

            GetAllAvailableWeapons();


            void OnSetEntityWeapon(WeaponData selectedWeapon, int index, ArmedEntity armedEntity) {
                if (!Enabled)
                    return;

                armedEntity.weapons.Set(index, selectedWeapon);
                OnCancel();
            }
        }


        private void GetAllAvailableWeapons() {

            if (weapons == null) {
                weapons = new List<WeaponCase>();
            } else if (weapons.Count > 0) {
                ResetGamePadSelection();
                return;
            }

            
            CreateWeaponCase(null);
            ResetGamePadSelection();

            AddressablesUtils.GetAssets<WeaponData>((data) => {

                    if (weapons.Exists( (c) => c.weaponData == data ))
                        return;

                    CreateWeaponCase(data);
                }

            );


        }

        private void CreateWeaponCase(WeaponData weapon, int equippedIndex = -1){
            var caseObject = Instantiate(weaponCaseTemplate, weaponSelectionContainer.transform);
            var weaponCase = caseObject.GetComponentInChildren<WeaponCase>();

            weaponCase.SetDisplayWeapon(weapon);
            weapons.Add( weaponCase );
        }


        public override void Enable() {

            base.Enable();

            weaponSelectionMenu.SetActive( true );
        }

        public override void Disable() {

            base.Disable();

            weaponSelectionMenu.SetActive( false );
        }

        public override void Refresh() {
            GetAllAvailableWeapons();
        }

        public override void ResetGamePadSelection(){
            EventSystem.current.SetSelectedGameObject(weapons[0].gameObject);
        }

        public override void EnableInteraction() {
            foreach (WeaponCase weapon in weapons) {
                weapon.EnableInteraction();
            }
        }

        public override void DisableInteraction() {
            foreach (WeaponCase weapon in weapons) {
                weapon.DisableInteraction();
            }
        }

        public override void OnCancel() {
            Disable();
        }



        private void Awake() {
            weaponSelectionContainer.GetComponent<GridLayoutGroup>().constraintCount = WEAPON_CASES_PER_ROW;
        }

    }
}