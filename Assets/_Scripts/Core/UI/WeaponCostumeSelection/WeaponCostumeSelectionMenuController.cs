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

namespace SeleneGame.Core.UI {
    
    public class WeaponCostumeSelectionMenuController : UIModal<WeaponCostumeSelectionMenuController> {

        public const int WEAPON_COSTUME_CASES_PER_ROW = 5;

        

        [SerializeField] private GameObject weaponSelectionMenu;
        [SerializeField] private GameObject weaponCostumeSelectionContainer;
        [SerializeField] private GameObject weaponCostumeCaseTemplate;
        
        [SerializeField] private List<WeaponCostumeCase> weaponCostumes = new List<WeaponCostumeCase>();

        public Action<WeaponCostume> onWeaponCostumeSelected { get; private set; }


        [SerializeField] private Weapon weapon;

        

        public void OpenSetWeaponCostumeMenu(int weaponIndex, ArmedEntity armedEntity) {

            if (armedEntity == null) {
                Debug.LogError("Armed Entity is null!");
                return;
            }

            weapon = armedEntity.Weapons[weaponIndex];

            if (weapon == null) return;


            onWeaponCostumeSelected = (selectedCostume) => OnSetWeaponCostume(weapon, selectedCostume);

            Enable();
            
            GetEquippableCostumes();
            

            void OnSetWeaponCostume(Weapon weapon, WeaponCostume selectedCostume) {
                if ( !Enabled ) 
                    return;

                weapon.SetCostume((selectedCostume));
                OnCancel();
            }
        }

        private void GetEquippableCostumes() {

            if (weaponCostumes == null) {
                weaponCostumes = new List<WeaponCostumeCase>();
            } else if (weaponCostumes.Count > 0) {
                ResetGamePadSelection();
                return;
            }

            // Get the Default Costume (corresponds to an empty slot, should be in the first space)
            CreateWeaponCostumeCase(weapon.Data.baseCostume);
            ResetGamePadSelection();

            // and then get all the other costumes.
            AddressablesUtils.GetAssets<WeaponCostume>((costume) => {

                    if ( !costume.accessibleInGame ) return;

                    // Add the costume to the list if it is a valid costume for the weapon.
                    // This means that either
                    // 1. The costume is specific to the weapon and contains the weapon's internal name (e.g. "Eris_Base" for Eris; this can work for multiple weapons)
                    // 2. The costume is generic to the weapon type which means it has the fitting flag set (see equippableOn and weaponType)
                    if ( !costume.name.Contains(weapon.Data.name) && costume.name.Contains("_Base") && !costume.equippableOn.HasFlag(weapon.Data.weaponType) )
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


        public override void Enable() {

            base.Enable();

            weaponSelectionMenu.SetActive( true );
        }

        public override void Disable() {

            base.Disable();

            weaponSelectionMenu.SetActive( false );
        }

        public override void Refresh() {
            GetEquippableCostumes();
        }

        public override void EnableInteraction() {
            foreach (WeaponCostumeCase weapons in weaponCostumes) {
                weapons.EnableInteraction();
            }
        }

        public override void DisableInteraction() {
            foreach (WeaponCostumeCase weapons in weaponCostumes) {
                weapons.DisableInteraction();
            }
        }

        public override void ResetGamePadSelection(){
            EventSystem.current.SetSelectedGameObject(weaponCostumes[0].gameObject);
        }

        public override void OnCancel() {
            WeaponInventoryMenuController.current.Enable();
        }



        private void Awake() {
            weaponCostumeSelectionContainer.GetComponent<GridLayoutGroup>().constraintCount = WEAPON_COSTUME_CASES_PER_ROW;
        }

    }
}