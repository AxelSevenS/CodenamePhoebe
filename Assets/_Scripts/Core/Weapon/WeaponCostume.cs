using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.AddressableAssets;

using SevenGame.Utility;

namespace SeleneGame.Core {
    
    [CreateAssetMenu(fileName = "new Weapon Costume", menuName = "Costume/Weapon")]
    public class WeaponCostume : ScriptableObject {

        
        const string defaultPath = "Weapons/Costumes/Unarmed/Base";


        [Tooltip("The Portrait of the Character Costume, used as a preview in menus.")]
        public Sprite portrait;

        [Tooltip("The Display Name of the Character Costume, used in menus.")]
        public string displayName = "Default Costume Name";

        [Tooltip("The description of the Character Costume, only appears when it is not the Base Costume of the Selected Character.")]
        [TextArea] 
        public string description = "Default Costume Description";


        public GameObject rightHandModel;
        public GameObject leftHandModel;


        [EnumFlag] public Weapon.WeaponType equippableOn;


        
        
        public bool IsEquippableOn(Weapon.WeaponType weaponType) {
            return ((byte)equippableOn & (byte)weaponType) == (byte)weaponType;
        }


        
        
        public static void GetCostumesFor(Weapon weapon, Action<WeaponCostume> callback) {

            AsyncOperationHandle<IList<WeaponCostume>> opHandle = Addressables.LoadAssetsAsync<WeaponCostume>(
                "WeaponCostume",
                (costume) => {

                    if ( costume == null )
                        return;

                    // Only accept a base costume if it's the base of this weapon, not if it's the base of another weapon.
                    // The base Costume doesn't have to pass the weaponType test, it is accepted either way.
                    // for the Weapon Eris : Eris_Base is accepted but not Hypnos_Base...
                    if ( costume.name.Contains(weapon.name) || ( !costume.name.Contains("_Base") && costume.IsEquippableOn(weapon.weaponType) ) )
                        callback?.Invoke( costume );

                }
            );
        }
        
        private static string GetPath(string costumeName) {
            return $"Weapons/Costumes/{costumeName}";
        }
        private static string GetBasePath(string weaponName) {
            return GetPath($"{weaponName}_Base");
        }

        public static WeaponCostume Get(string weaponName, string costumeName, Weapon.WeaponType weaponType = 0) {
            // Get Requested Weapon Costume
            AsyncOperationHandle<WeaponCostume> opHandle = Addressables.LoadAssetAsync<WeaponCostume>( GetPath(costumeName) );
            WeaponCostume result = opHandle.WaitForCompletion();

            // If not found, get Base costume of this Weapon
            if (result == null) {
                Debug.LogWarning($"Error getting costume {costumeName} for weapon {weaponName}; Using Base Costume instead.");
                return GetBase(weaponName);
            }

            // If the Costume is not equippable on this Weapon, get Base costume for this weapon.
            if ( !result.IsEquippableOn(weaponType) ) {
                Debug.Log($"Error getting costume {costumeName} for weapon {weaponName}; Costume is not equippable on Weapon type {weaponType}.");
                return GetBase(weaponName);
            }

            return result;
        }
        public static WeaponCostume GetBase(string weaponName) {
            AsyncOperationHandle<WeaponCostume> opHandle = Addressables.LoadAssetAsync<WeaponCostume>( GetBasePath(weaponName) );
            WeaponCostume result = opHandle.WaitForCompletion();

            // If not found, get Default costume.
            // This should never happen.
            if (result == null) {
                Debug.LogError($"Error getting Base Costume for weapon {weaponName}");
                return GetDefault();
            }

            return result;
        }
        public static WeaponCostume GetDefault() {
            AsyncOperationHandle<WeaponCostume> opHandle = Addressables.LoadAssetAsync<WeaponCostume>( defaultPath );
            return opHandle.WaitForCompletion();
        }



        public static void GetAsync(string weaponName, string costumeName, Action<WeaponCostume> callback, Weapon.WeaponType weaponType = 0) {
            // Get Requested Weapon Costume
            AsyncOperationHandle<WeaponCostume> opHandle = Addressables.LoadAssetAsync<WeaponCostume>( GetPath(costumeName) );
            opHandle.Completed += operation => {

                // If not found, get Base costume of this Weapon
                if (operation.Status == AsyncOperationStatus.Failed) {
                    Debug.LogWarning($"Error getting costume {costumeName} for weapon {weaponName}; Using Base Costume instead.");
                    GetBaseAsync(weaponName, callback);
                    return;
                }

                // If the Costume is not equippable on this Weapon, get Base costume for this weapon.
                if ( !operation.Result.IsEquippableOn(weaponType) ) {
                    Debug.Log($"Error getting costume {costumeName} for weapon {weaponName}; Costume is not equippable on Weapon type {weaponType}.");
                    GetBaseAsync(weaponName, callback);
                    return;
                }

                callback(operation.Result);
            };
        }
        public static void GetBaseAsync(string weaponName, Action<WeaponCostume> callback) {
            // Get Weapon Base Costume
            AsyncOperationHandle<WeaponCostume> opHandle = Addressables.LoadAssetAsync<WeaponCostume>( GetBasePath(weaponName) );
            opHandle.Completed += operation => {

                // If not found, get Default costume.
                // This should never happen.
                if (operation.Status == AsyncOperationStatus.Failed) {
                    Debug.LogError($"Error getting Base Costume for weapon {weaponName}");
                    GetDefaultAsync(callback);
                    return;
                }

                callback(operation.Result);
            };
        }
        public static void GetDefaultAsync(Action<WeaponCostume> callback) {
            AsyncOperationHandle<WeaponCostume> opHandle = Addressables.LoadAssetAsync<WeaponCostume>( defaultPath );
            opHandle.Completed += operation => {
                callback(operation.Result);
            };
        }

    }
}