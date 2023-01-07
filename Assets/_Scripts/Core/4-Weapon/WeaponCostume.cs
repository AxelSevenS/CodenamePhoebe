using System;

using UnityEngine;

using SevenGame.Utility;

namespace SeleneGame.Core {
    
    [CreateAssetMenu(fileName = "new Weapon Costume", menuName = "Costume/Weapon")]
    public class WeaponCostume : Costume<WeaponCostume> {
        
        [SerializeField] private GameObject model;

        public Weapon.WeaponType equippableOn;

        [ReadOnly] public GameObject modelInstance;



        public override void LoadModel() {
            if (model != null) {
                modelInstance = Instantiate(model, _entity["handRight"].transform);
            }
        }

        public override void UnloadModel() {
            modelInstance = GameUtility.SafeDestroy(modelInstance);
        }

    }
}