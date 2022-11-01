using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.AddressableAssets;

using SevenGame.Utility;

namespace SeleneGame.Core {
    
    [CreateAssetMenu(fileName = "new Weapon Costume", menuName = "Costume/Weapon")]
    public class WeaponCostume : AddressableAsset<WeaponCostume> {

        
        const string defaultPath = "Weapons/Costumes/Unarmed/Base";


        [Tooltip("The Portrait of the Character Costume, used as a preview in menus.")]
        public Sprite portrait;

        [Tooltip("The Display Name of the Character Costume, used in menus.")]
        public string displayName = "Default Costume Name";

        [Tooltip("The description of the Character Costume, only appears when it is not the Base Costume of the Selected Character.")]
        [TextArea] 
        public string description = "Default Costume Description";


        public GameObject model;


        public Weapon.WeaponType equippableOn;
        // [EnumFlag] public Weapon.WeaponType equippableOn;

    }
}