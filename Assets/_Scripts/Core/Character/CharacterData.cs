using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SeleneGame.Core {    
    
    [CreateAssetMenu(fileName = "new Character Data", menuName = "Data/Character")]
    public class CharacterData : CostumableData<CharacterCostume> {

        public float maxHealth = 100f;
        public Vector3 size = new(0.5f, 2f, 0.5f);
        public float stepHeight = 0.5f;
        public float weight = 12f;
        public float jumpHeight = 16f;

        public float baseSpeed = 8f;
        public float sprintSpeed = 13f;
        public float slowSpeed = 5f;
        public float acceleration = 100f;
        public float swimMultiplier = 0.85f;

        public float evadeSpeed = 27f;
        public float evadeDuration = 0.6f;
        public float evadeCooldown = 0.01f;


        public float TotalEvadeDuration => evadeDuration + evadeCooldown;

        public virtual Character GetCharacter(Entity entity, CharacterCostume costume = null) { 
            return new Character(entity, this, costume ?? baseCostume ?? AddressablesUtils.GetDefaultAsset<CharacterCostume>());
        }

        public virtual void CharacterUpdate(Character character) {;}
        public virtual void CharacterLateUpdate(Character character) {;}
        public virtual void CharacterFixedUpdate(Character character) {;}

    }
}
