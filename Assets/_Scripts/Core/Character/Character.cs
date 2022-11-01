using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.AddressableAssets;

using SevenGame.Utility;

namespace SeleneGame.Core {

    public abstract class Character : InstantiableAsset<Character> {

        [SerializeField] protected Entity entity;
        

        [Header("Character Info")]

        [SerializeField] private CharacterCostume _baseCostume;

        [SerializeField] private string _displayName = "Default Entity Name";
        
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

        [SerializeField] [ReadOnly] private CharacterCostume _costume;
        [SerializeField] [ReadOnly] private GameObject _model;
        [SerializeField] [ReadOnly] private CostumeData _costumeData;



        public CharacterCostume baseCostume {
            get {
                return _baseCostume;
            }
            set {
                _baseCostume = value;
            }
        }

        public string displayName {
            get {
                return _displayName;
            }
            set {
                _displayName = value;
            }
        }

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


        public CharacterCostume costume {
            get {
                if (_costume == null) {
                    SetCostume(_baseCostume);
                }
                return _costume;
            }
            set {
                SetCostume(value);
            }
        }

        public CostumeData costumeData => _costumeData;
        public GameObject model => _model;



        public virtual void Initialize( Entity entity, CharacterCostume costume = null) {
            this.entity = entity;
            SetCostume( costume ?? baseCostume );
        }

        public void SetCostume(string costumeName) {
            SetCostume(CharacterCostume.GetAsset(costumeName));
        }

        public void SetCostume(CharacterCostume costume){
            _costume = costume;
            
            LoadModel();
        }

        public void LoadModel(){
            UnloadModel();

            if (entity == null || costume == null) return;

            if (costume.model != null) {
                Transform entityTransform = entity.transform;
                _model = GameObject.Instantiate(costume.model, entityTransform.position, entityTransform.rotation, entityTransform);
                _model.name = $"{name}CharacterModel";

                _costumeData = model.GetComponent<CostumeData>();
            }

            entity.animator.runtimeAnimatorController = costumeData.animatorController;
            entity.animator.avatar = costumeData.animatorAvatar;
            entity.animator.Rebind();

        }
        public void UnloadModel(){
            _model = GameUtility.SafeDestroy(model);
            _costumeData = null;
        }

        public virtual void CharacterUpdate( Entity entity ){;}
        public virtual void CharacterFixedUpdate( Entity entity ){;}

    }
}
