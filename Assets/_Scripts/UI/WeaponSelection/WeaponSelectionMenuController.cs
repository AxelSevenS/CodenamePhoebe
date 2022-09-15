using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using SeleneGame.Core;

using SevenGame.Utility;

namespace SeleneGame.UI {
    
    public class WeaponSelectionMenuController : UIMenu<WeaponSelectionMenuController> {

        public const int WEAPON_CASES_PER_ROW = 5;

        

        [SerializeField] private GameObject weaponSelectionMenu;
        [SerializeField] private GameObject weaponSelectionContainer;
        [SerializeField] private GameObject weaponCaseTemplate;
        
        [SerializeField] private List<Weapon> equippedWeapons = new List<Weapon>();
        [SerializeField] private List<WeaponCase> weapons = new List<WeaponCase>();

        private Weapon.Instance currentWeapon;

        private Action<Weapon> onWeaponSelected;



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
        

        public void ReplaceWeapon(Weapon.Instance weapon, ArmedEntity armedEntity) {
            currentWeapon = weapon;

            onWeaponSelected = (selectedWeapon) => {
                currentWeapon.SetWeapon(selectedWeapon);
                OnCancel();
            };

            GetEntityWeapons(armedEntity);
            GetAllAvailableWeapons();

            Enable();
        }


        public void OnSelectWeapon(Weapon weapon) {
            if ( !Enabled ) return;
            onWeaponSelected?.Invoke(weapon);
        }

        private void GetEntityWeapons(ArmedEntity armedEntity) {
            equippedWeapons = new();
            
            foreach ( Weapon.Instance weaponInstance in armedEntity.weapons) {
                equippedWeapons.Add(weaponInstance.weapon);
            }
        }

        private void GetAllAvailableWeapons() {

            foreach ( WeaponCase weapon in weapons ) {
                GameUtility.SafeDestroy(weapon.gameObject);
            }
            weapons = new();
            
            Weapon.GetDefaultAsync( (defaultWeapon) => {

                    // Get the Default Weapon (corresponds to an empty slot, should be in the first space)
                    CreateWeaponCase(defaultWeapon);
                    ResetGamePadSelection();

                    // and then get all the other weapons.
                    Weapon.GetWeapons( (weapon) => {
                            if ( weapons.Exists( (obj) => { return obj.weapon == weapon; }) ) 
                                return;

                            int index = -1;
                            for (int i = 0; i < equippedWeapons.Count; i++) {
                                if ( equippedWeapons[i] == weapon ) {
                                    index = i;
                                    break;
                                }
                            }

                            CreateWeaponCase(weapon, index);
                        }
                    );
                }
            );


        }

        private void CreateWeaponCase(Weapon weapon, int equippedIndex = -1){
            var caseObject = Instantiate(weaponCaseTemplate, weaponSelectionContainer.transform);
            var weaponCase = caseObject.GetComponentInChildren<WeaponCase>();
            weaponCase.weapon = weapon;
            
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