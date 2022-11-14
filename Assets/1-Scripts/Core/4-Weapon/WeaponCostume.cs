using System;

using UnityEngine;

using SevenGame.Utility;

namespace SeleneGame.Core {
    
    [CreateAssetMenu(fileName = "new Weapon Costume", menuName = "Costume/Weapon")]
    public class WeaponCostume : Costume<WeaponCostume> {
        
        [SerializeField] private GameObject model;

        public Weapon.WeaponType equippableOn;


        [SerializeField] [ReadOnly] private Entity attachedEntity;

        [ReadOnly] public GameObject modelInstance;



        public void Initialize(ArmedEntity attachedEntity) {
            if (this.attachedEntity != null)
                throw new InvalidOperationException("Weapon Costume already initialized");

            this.attachedEntity = attachedEntity;
        }

        public override void LoadModel() {
            if (model != null) {
                modelInstance = Instantiate(model, attachedEntity["handRight"].transform);
            }
        }

        public override void UnloadModel() {
            modelInstance = GameUtility.SafeDestroy(modelInstance);
        }

    }
}