using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SeleneGame.Core {    
    
    [CreateAssetMenu(fileName = "new Character Data", menuName = "Data/Character")]
    public class CharacterData : CostumableData<CharacterCostume> {

        public float maxHealth = 100f;
        public Vector3 size = new Vector3(0.5f, 2f, 0.5f);
        public float stepHeight = 0.5f;
        public float weight = 12f;
        public float jumpHeight = 16f;

        public float baseSpeed = 10f;
        public float acceleration = 100f;
        public float sprintMultiplier = 1.5f;
        public float slowMultiplier = 0.5f;
        public float swimMultiplier = 0.85f;

        public float evadeSpeed = 27f;
        public float evadeDuration = 0.6f;
        public float evadeCooldown = 0.01f;


        public float totalEvadeDuration => evadeDuration + evadeCooldown;

        public virtual Character GetCharacter(Entity entity, CharacterCostume costume = null) { 
            return new Character(entity, this, costume ?? baseCostume ?? AddressablesUtils.GetDefaultAsset<CharacterCostume>());
        }

    }
}
