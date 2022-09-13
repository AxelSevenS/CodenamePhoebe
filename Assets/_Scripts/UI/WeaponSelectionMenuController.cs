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
    
    public class WeaponSelectionMenuController : MenuController<WeaponSelectionMenuController> {

        public const int WEAPON_CASES_PER_ROW = 5;

        

        [SerializeField] private GameObject weaponSelectionMenu;
        [SerializeField] private GameObject weaponSelectionContainer;
        [SerializeField] private GameObject weaponCaseTemplate;
        
        [SerializeField] private List<Weapon> equippedWeapons = new List<Weapon>();
        [SerializeField] private List<WeaponCase> weapons = new List<WeaponCase>();

        private Weapon.Instance currentWeapon;



        public void ReplaceWeapon(Weapon.Instance weapon) {
            currentWeapon = weapon;
            Enable();
        }

        public override void Enable() {
            if (!Enabled)
                UIController.current.DisableAllMenus();

            base.Enable();

            GetAllEquippedWeapons();
            GetAllAvailableWeapons();

            weaponSelectionMenu.SetActive( true );

            SetSelectedObject();

            UIController.current.UpdateMenuState();
        }

        public override void Disable() {
            base.Disable();

            weaponSelectionMenu.SetActive( false );

            UIController.current.UpdateMenuState();
        }

        public override void SetSelectedObject(){
            if ( Enabled && ControlsManager.controllerType != ControlsManager.ControllerType.MouseKeyboard )
                EventSystem.current.SetSelectedGameObject(weapons[0].gameObject);
        }

        private void GetAllEquippedWeapons() {
            equippedWeapons = new();

            if ( ! (PlayerEntityController.current.entity is ArmedEntity armed) ) return;
            
            foreach ( Weapon.Instance weaponInstance in armed.weapons) {
                equippedWeapons.Add(weaponInstance.weapon);
            }
        }

        private void GetAllAvailableWeapons() {
            
            Weapon.GetDefaultAsync( (defaultWeapon) => {

                    // Get the Default Weapon (corresponds to an empty slot, should be in the first space)
                    CreateWeaponCase(defaultWeapon);

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

    }
}