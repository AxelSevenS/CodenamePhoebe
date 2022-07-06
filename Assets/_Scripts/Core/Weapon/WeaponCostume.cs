using UnityEngine;

namespace SeleneGame.Core {
    
    [CreateAssetMenu(fileName = "new Weapon Costume", menuName = "Costume/Weapon")]
    public class WeaponCostume : Costume {

        public GameObject secondaryModel;
        
        public static WeaponCostume GetWeaponBaseCostume(Weapon weapon){
            string defaultCostume = weapon.GetType().Name.Replace("Weapon", "Base");
            return GetWeaponCostume(defaultCostume);
        }
        public static WeaponCostume TryGetWeaponCostumeOrBase(Weapon weapon, string costumeName) {
            string path = $"Costume/Weapon/{costumeName}";
            WeaponCostume costume = Resources.Load<WeaponCostume>(path);
            if (costume == null) {
                string basePath = $"Costume/Weapon/{weapon.GetType().Name.Replace("Weapon", "Base")}";
                Debug.LogError($"No Costume was found at Path {path} ; Using weapon base costume at Path {basePath}");
                costume = GetWeaponBaseCostume(weapon);
            }
            return costume;
        }
        public static WeaponCostume GetWeaponCostume(string costumeName) {
            string path = $"Costume/Weapon/{costumeName}";
            WeaponCostume costume = Resources.Load<WeaponCostume>(path);
            if (costume == null) {
                const string defaultPath = "Costume/Weapon/UnarmedBase";
                Debug.LogError($"No Costume was found at Path {path} ; Using default costume at Path {defaultPath}");
                costume = Resources.Load<WeaponCostume>(defaultPath);
            }
            return costume;
        }
    }
}