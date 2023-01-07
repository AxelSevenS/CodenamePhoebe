using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.AddressableAssets;

using SevenGame.Utility;

namespace SeleneGame.Core {

    public abstract class Character : Costumable<Character, CharacterCostume> {
        

        [Header("Character Info")]
        public float maxHealth;
        public Vector3 size;
        public float stepHeight;
        public float weight;
        public float jumpHeight;

        [Header("Movement Speed")]
        public float baseSpeed;
        public float acceleration;
        public float sprintMultiplier;
        public float slowMultiplier;
        public float swimMultiplier;

        [Header("Evade")]
        public float evadeSpeed;
        public float evadeDuration;
        public float evadeCooldown;


        [Header("Animations")]
        public CharacterAnimationSet animations;



        public float totalEvadeDuration => evadeDuration + evadeCooldown;

        public GameObject model => costume.modelInstance;
        public CostumeData costumeData => _costume.costumeData;
        

        protected internal virtual void CharacterUpdate( Entity entity ){;}
        protected internal virtual void CharacterFixedUpdate( Entity entity ){;}

        public override void SetCostume(CharacterCostume costume) {
            
            Quaternion modelRotation = model.transform.rotation;

            base.SetCostume(costume);

            model.transform.rotation = modelRotation;
        }
    }
}
