using System.Collections;
using System.Collections.Generic;
using SevenGame.Utility;
using UnityEngine;

namespace SeleneGame.Core {

    public abstract class WeaponModel : CostumeModel<WeaponCostume> {
        
        [SerializeField] [ReadOnly] protected Entity _entity;
        [SerializeReference] [HideInInspector] protected Weapon _weapon;

        public Entity entity => _entity;
        public Weapon weapon => _weapon;
        
        public WeaponModel(Entity entity, Weapon weapon, WeaponCostume costume) : base(costume) {
            _entity = entity;
            _weapon = weapon;
        }

    }
}
