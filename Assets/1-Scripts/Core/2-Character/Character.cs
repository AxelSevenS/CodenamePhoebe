using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.AddressableAssets;

using SevenGame.Utility;

namespace SeleneGame.Core {

    public abstract class Character : Costumable<Character, CharacterCostume> {
        

        [Header("Character Info")]
        
        [SerializeField] private float _maxHealth;
        [SerializeField] private Vector3 _size;
        [SerializeField] private float _stepHeight;
        [SerializeField] private float _weight;
        [SerializeField] private float _jumpHeight;

        [Header("Movement Speed")]
        [SerializeField] private float _baseSpeed;
        [SerializeField] private float _acceleration;
        [SerializeField] private float _sprintMultiplier;
        [SerializeField] private float _slowMultiplier;
        [SerializeField] private float _swimMultiplier;

        [Header("Evade")]
        [SerializeField] private float _evadeSpeed;
        [SerializeField] private float _evadeDuration;
        [SerializeField] private float _evadeCooldown;


        [Header("Character Data")]
        [SerializeField] [ReadOnly] protected Entity entity;



        public float maxHealth => _maxHealth;
        public Vector3 size => _size;
        public float stepHeight => _stepHeight;
        public float weight => _weight;
        public float jumpHeight => _jumpHeight;

        public float baseSpeed => _baseSpeed;
        public float acceleration => _acceleration;
        public float sprintMultiplier => _sprintMultiplier;
        public float slowMultiplier => _slowMultiplier;
        public float swimMultiplier => _swimMultiplier;

        public float evadeSpeed => _evadeSpeed;
        public float evadeDuration => _evadeDuration;
        public float evadeCooldown => _evadeCooldown;

        public float totalEvadeDuration => evadeDuration + evadeCooldown;

        public GameObject model => costume.modelInstance;
        public CostumeData costumeData => _costume.costumeData;




        public virtual void Initialize( Entity entity, CharacterCostume costume = null) {
            if (this.entity != null)
                throw new InvalidOperationException("Character already initialized");

            this.entity = entity;
            SetCostume( CharacterCostume.GetInstanceOf(costume ?? baseCostume) );
        }

        public override void SetCostume(CharacterCostume costume) {
            if (costume == null) return;

            _costume?.UnloadModel();

            _costume = costume;
            _costume.Initialize(entity);
            _costume.LoadModel();
        }
        
        

        protected internal virtual void CharacterUpdate( Entity entity ){;}
        protected internal virtual void CharacterFixedUpdate( Entity entity ){;}

    }
}
