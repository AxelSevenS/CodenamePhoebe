using System.Collections;
using System.Collections.Generic;
using SevenGame.Utility;
using UnityEngine;

namespace SeleneGame.Core {

    [CreateAssetMenu(fileName = "new Weapon Costume", menuName = "Costume/Simple Weapon")]
    public sealed class SimpleWeaponCostume : WeaponCostume {

        [SerializeField] public GameObject model;

        public override WeaponModel LoadModel(Entity entity, Weapon weapon) {
            return new SimpleWeaponModel(entity, weapon, this);
        }
    }

    public sealed class SimpleWeaponModel : WeaponModel {

        [ReadOnly] public GameObject model;

        public override Transform mainTransform => model.transform;

        public SimpleWeaponModel(Entity entity, Weapon weapon, SimpleWeaponCostume costume) : base(entity, weapon, costume) {
            if (entity != null && costume?.model != null)
                model = GameObject.Instantiate(costume.model, entity["weaponRight"].transform, false);
        }

        public override void Unload() {
            model = GameUtility.SafeDestroy(model);
        }
    }
}
