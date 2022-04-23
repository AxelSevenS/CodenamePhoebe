using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SeleneGame.Core {
    
    [CreateAssetMenu(fileName = "new Weapon Data", menuName = "Data/Weapon")]
    public class WeaponData : ScriptableObject, IData {

        public static string defaultData => "Unarmed";

        public string displayName;
        
        public WeaponCostume costume;

        public event System.Action onChangeCostume;

        public void SetCostume(string costumeName){
            costume = Resources.Load<WeaponCostume>($"{DataGetter.GetDataPath<WeaponData>()}/{name}/{costumeName}");
            onChangeCostume?.Invoke();
        }
        
        public enum WeaponType {lightSword, heavySword, spear, swordAndShield, sparring};
        
        public WeaponType weaponType;
    }
}