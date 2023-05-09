using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using SevenGame.Utility;

namespace SeleneGame.Core.UI {
    
    public class WeaponCostumeSelectionMenuController : UIMenu<WeaponCostumeSelectionMenuController> {

        public const int WEAPON_COSTUME_CASES_PER_ROW = 5;

        

        [SerializeField] private GameObject weaponSelectionMenu;
        [SerializeField] private GameObject weaponCostumeSelectionContainer;
        [SerializeField] private GameObject weaponCostumeCaseTemplate;
        
        [SerializeField] private List<WeaponCostumeCase> weaponCostumes = new List<WeaponCostumeCase>();

        public Action<WeaponCostume> onWeaponCostumeSelected { get; private set; }



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
            SetSelected(weaponCostumes[0].gameObject);
        }

        public override void OnCancel() {
            WeaponInventoryMenuController.current.Enable();
        }
        

        public void OpenSetWeaponCostumeMenu(int weaponIndex, ArmedEntity armedEntity) {

            Weapon weapon = armedEntity.weapons[weaponIndex];

            if (weapon == null) return;

            onWeaponCostumeSelected = (selectedCostume) => SetWeaponCostume(weapon, selectedCostume);

            Enable();
            
            GetEquippableCostumes(weapon);

            void SetWeaponCostume(Weapon weapon, WeaponCostume selectedCostume) {
                if ( !Enabled ) return;

                weapon.SetCostume((selectedCostume));
                OnCancel();
            }
        }

        private void GetEquippableCostumes(Weapon weapon) {

            if (weaponCostumes == null) {
                weaponCostumes = new List<WeaponCostumeCase>();
            } else if (weaponCostumes.Count > 0) {
                ResetGamePadSelection();
                return;
            }

            // Get the Default Costume (corresponds to an empty slot, should be in the first space)
            CreateWeaponCostumeCase(weapon.data.baseCostume);
            ResetGamePadSelection();

            // and then get all the other costumes.
            AddressablesUtils.GetAssets<WeaponCostume>((costume) => {

                    if ( !costume.accessibleInGame ) return;

                    // Add the costume to the list if it is a valid costume for the weapon.
                    // This means that either
                    // 1. The costume is specific to the weapon and contains the weapon's internal name (e.g. "Eris_Base" for Eris; this can work for multiple weapons)
                    // 2. The costume is generic to the weapon type which means it has the fitting flag set (see equippableOn and weaponType)
                    if ( !costume.name.Contains(weapon.data.name) && costume.name.Contains("_Base") && !costume.equippableOn.HasFlag(weapon.data.weaponType) )
                        return;

                    // If the costume is already in the list, don't add it again.
                    if ( weaponCostumes.Exists( (existingCase) => { return existingCase.weaponCostume == costume; }) ) 
                        return;

                    CreateWeaponCostumeCase(costume);
                }
            );


        }

        private void CreateWeaponCostumeCase(WeaponCostume costume){
            var caseObject = Instantiate(weaponCostumeCaseTemplate, weaponCostumeSelectionContainer.transform);
            var costumeCase = caseObject.GetComponentInChildren<WeaponCostumeCase>();

            costumeCase.SetDisplayWeaponCostume(costume);
            
            weaponCostumes.Add( costumeCase );
        }



        private void Awake() {
            weaponCostumeSelectionContainer.GetComponent<GridLayoutGroup>().constraintCount = WEAPON_COSTUME_CASES_PER_ROW;
        }

    }
}