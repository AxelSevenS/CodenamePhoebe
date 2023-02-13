using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.AddressableAssets;

using SevenGame.Utility;

namespace SeleneGame.Core {

    public abstract class Character : Costumable<Character, CharacterCostume, CharacterModel> {

        [SerializeField] private Entity _entity;



        public Entity entity => _entity;

        public virtual float maxHealth => 100f;
        public virtual Vector3 size => new Vector3(0.5f, 2f, 0.5f);
        public virtual float stepHeight => 0.5f;
        public virtual float weight => 12f;
        public virtual float jumpHeight => 16f;

        public virtual float baseSpeed => 10f;
        public virtual float acceleration => 100f;
        public virtual float sprintMultiplier => 1.5f;
        public virtual float slowMultiplier => 0.5f;
        public virtual float swimMultiplier => 0.85f;

        public virtual float evadeSpeed => 27f;
        public virtual float evadeDuration => 0.6f;
        public virtual float evadeCooldown => 0.01f;


        public float totalEvadeDuration => evadeDuration + evadeCooldown;

        

        public Character(Entity entity, CharacterCostume costume = null) {
            _entity = entity;
            SetCostume(costume ?? baseCostume);
        }

        public override void SetCostume(CharacterCostume costume) {
            _model?.Dispose();
            Debug.Log(costume);
            _model = (CharacterModel)costume?.LoadModel(entity) ?? null;
        }

        protected override void DisposeBehavior() {
            model?.Dispose();
        }
    }
}
