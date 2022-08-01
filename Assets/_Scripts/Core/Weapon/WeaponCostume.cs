using UnityEngine;

namespace SeleneGame.Core {
    
    [CreateAssetMenu(fileName = "new Weapon Costume", menuName = "Costume/Weapon")]
    public class WeaponCostume : Costume {

        public GameObject leftHandModel;
        
        const string defaultPath = "Costume/wc_UnarmedBase";

        private static string CostumeNameToPath(string costumeName) {
            if ( costumeName.Contains("wc_") )
                return $"Costume/{costumeName}";
            return $"Costume/wc_{costumeName}";
        }

        public static WeaponCostume GetWeaponCostume(string costumeName) {
            string path = CostumeNameToPath(costumeName);
            WeaponCostume costume = Resources.Load<WeaponCostume>(path);
            if (costume == null) {
                Debug.LogError($"No Costume was found at Path {path} ; Using default costume at Path {defaultPath}");
                costume = Resources.Load<WeaponCostume>(defaultPath);
            }
            return costume;
        }


        public static WeaponCostume TryGetWeaponCostumeOrBase(string weaponTypeName, string costumeName) {
            string path = CostumeNameToPath(costumeName);
            WeaponCostume costume = Resources.Load<WeaponCostume>(path);
            if (costume == null) {

                costume = GetWeaponBaseCostume(weaponTypeName);

                Debug.LogError($"No Costume was found at Path {path} ; Using weapon base costume");
            }
            return costume;
        }

        public static WeaponCostume TryGetWeaponCostumeOrBase(System.Type weaponType, string costumeName) {
            string path = CostumeNameToPath(costumeName);
            WeaponCostume costume = Resources.Load<WeaponCostume>(path);
            if (costume == null) {

                costume = GetWeaponBaseCostume(weaponType);

                Debug.LogError($"No Costume was found at Path {path} ; Using weapon base costume");
            }
            return costume;
        }


        public static WeaponCostume GetWeaponBaseCostume(string weaponTypeName){
            string defaultCostume = weaponTypeName.Replace("Weapon", "Base");
            return GetWeaponCostume(defaultCostume);
        }

        public static WeaponCostume GetWeaponBaseCostume(System.Type weaponType){
            string defaultCostume = weaponType.Name.Replace("Weapon", "Base");
            return GetWeaponCostume(defaultCostume);
        }
    }
}