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

        

        [SerializeField] private GameObject weaponSelectionMenu;
        [SerializeField] private GameObject weaponSelectionContainer;
        [SerializeField] private GameObject weaponCaseTemplate;
        
        [SerializeField] private List<Weapon> equippedWeapons = new List<Weapon>();
        [SerializeField] private List<WeaponCase> weapons = new List<WeaponCase>();

        private Action<WeaponData> onWeaponDataSelected;


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
            EventSystem.current.SetSelectedGameObject(weapons[0].gameObject);
        }

        public override void OnCancel() {
            WeaponInventoryMenuController.current.Enable();
        }
        

        public void ReplaceWeapon(int index, ArmedEntity armedEntity) {

            onWeaponDataSelected = (selectedWeapon) => {
                armedEntity.weapons.Set(index, selectedWeapon);
                OnCancel();
            };

            GetEntityWeapons(armedEntity);
            GetAllAvailableWeapons();

            Enable();
        }


        public void OnSelectWeapon(WeaponData weapon) {
            if ( !Enabled ) return;
            onWeaponDataSelected?.Invoke( weapon );
        }

        private void GetEntityWeapons(ArmedEntity armedEntity) {
            equippedWeapons.Clear();
            
            foreach ( Weapon weapon in armedEntity.weapons) {
                equippedWeapons.Add(weapon);
            }
        }

        private void GetAllAvailableWeapons() {

            if (weapons == null) {
                weapons = new List<WeaponCase>();
            } else if (weapons.Count > 0) {
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
            if (weapons.Count > 1) {
                WeaponCase previousCase = weapons[weapons.Count - 2];
                previousCase.elementRight = weaponCase;
                weaponCase.elementLeft = previousCase;
            }

            if (weapons.Count > WEAPON_CASES_PER_ROW) {
                WeaponCase aboveCase = weapons[weapons.Count - (WEAPON_CASES_PER_ROW + 1)];
                aboveCase.elementDown = weaponCase;
                weaponCase.elementUp = aboveCase;
            }

        }



        private void Awake() {
            weaponSelectionContainer.GetComponent<GridLayoutGroup>().constraintCount = WEAPON_CASES_PER_ROW;
        }

    }
}