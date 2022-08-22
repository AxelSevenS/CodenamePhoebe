using System;

using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.AddressableAssets;

using SevenGame.Utility;

namespace SeleneGame.Core {
    
    [CreateAssetMenu(fileName = "new Weapon Costume", menuName = "Costume/Weapon")]
    public class WeaponCostume : ScriptableObject {

        public Sprite portrait;
        public GameObject rightHandModel;
        public GameObject leftHandModel;

        public string displayName = "Default Costume Name";
        [EnumFlag] public Weapon.WeaponType equippableOn;
        
        const string defaultPath = "Weapons/Costumes/Unarmed/Base";


        public bool IsEquippableOn(Weapon.WeaponType weaponType) {
            return (equippableOn & weaponType) == weaponType;
        }
        



        
        private static string GetPath(string weaponName, string costumeName) {
            return $"Weapons/Costumes/{weaponName}/{costumeName}";
        }

        public static WeaponCostume Get(string weaponName, string costumeName, Weapon.WeaponType weaponType = 0) {
            // Get Requested Weapon Costume
            AsyncOperationHandle<WeaponCostume> opHandle = Addressables.LoadAssetAsync<WeaponCostume>( GetPath(weaponName, costumeName) );
            WeaponCostume result = opHandle.WaitForCompletion();
            if (result == null) {
                // If not found, get Base costume of this Weapon
                Debug.LogWarning($"Error getting costume {costumeName} for weapon {weaponName}; Using Base Costume instead.");
                return GetBase(weaponName);
            }
            if ( !result.IsEquippableOn(weaponType) ) {
                // If the Costume is not equippable on this Weapon, get Base costume for this weapon.
                Debug.Log($"Error getting costume {costumeName} for weapon {weaponName}; Costume is not equippable on Weapon type {weaponType}.");
                return GetBase(weaponName);
            }

            return result;
        }
        public static WeaponCostume GetBase(string weaponName) {
            AsyncOperationHandle<WeaponCostume> opHandle = Addressables.LoadAssetAsync<WeaponCostume>( GetPath(weaponName, "Base") );
            WeaponCostume result = opHandle.WaitForCompletion();
            if (result == null) {
                // If not found, get Default costume.
                // This should never happen.
                Debug.LogError($"Error getting Base Costume for weapon {weaponName}");
                result = GetDefault();
            }

            return result;
        }
        public static WeaponCostume GetDefault() {
            AsyncOperationHandle<WeaponCostume> opHandle = Addressables.LoadAssetAsync<WeaponCostume>( defaultPath );
            return opHandle.WaitForCompletion();
        }



        public static void GetAsync(string weaponName, string costumeName, Action<WeaponCostume> callback, Weapon.WeaponType weaponType = 0) {
            // Get Requested Weapon Costume
            AsyncOperationHandle<WeaponCostume> opHandle = Addressables.LoadAssetAsync<WeaponCostume>( GetPath(weaponName, costumeName) );
            opHandle.Completed += operation => {
                if (operation.Status == AsyncOperationStatus.Failed) {
                    // If not found, get Base costume of this Weapon
                    Debug.LogWarning($"Error getting costume {costumeName} for weapon {weaponName}; Using Base Costume instead.");
                    GetBaseAsync(weaponName, callback);
                    return;
                }
                if ( !operation.Result.IsEquippableOn(weaponType) ) {
                    // If the Costume is not equippable on this Weapon, get Base costume for this weapon.
                    Debug.Log($"Error getting costume {costumeName} for weapon {weaponName}; Costume is not equippable on Weapon type {weaponType}.");
                    GetBaseAsync(weaponName, callback);
                    return;
                }

                callback(operation.Result);
            };
        }
        public static void GetBaseAsync(string weaponName, Action<WeaponCostume> callback) {
            // Get Weapon Base Costume
            AsyncOperationHandle<WeaponCostume> opHandle = Addressables.LoadAssetAsync<WeaponCostume>( GetPath(weaponName, "Base") );
            opHandle.Completed += operation => {
                if (operation.Status == AsyncOperationStatus.Failed) {
                    // If not found, get Default costume.
                    // This should never happen.
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