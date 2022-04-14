using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SeleneGame.Core {
    
    [CreateAssetMenu(fileName = "new Entity Data", menuName = "Data/Entity")]
    public class EntityData : UnitData<EntityCostume> {
        
        public float maxHealth;
        public Vector3 size = new Vector3(1f, 1f, 1f);
        public float stepHeight = 1f;
        public float moveIncrement = 20f;
        public float weight = 15f;
        public float jumpHeight= 1f;
        public float baseSpeed = 1f;
        public float slowSpeed = 1f;
        public float crouchSpeed = 1f;
        public float swimSpeed;
        public float evadeDuration;
        public float evadeCooldown;
        public float totalEvadeDuration => evadeDuration + evadeCooldown;
        public AnimationCurve evadeCurve;
        public RuntimeAnimatorController controller;
        
    }
}