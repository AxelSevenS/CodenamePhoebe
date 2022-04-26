using System;
using UnityEngine;

namespace SeleneGame.Core {
    
    public abstract class Weapon : MonoBehaviour {

        [Space(35)]
        [ReadOnly] public Entity entity;
        [ReadOnly] public WeaponData data;

        [ReadOnly] public GameObject model;
        [ReadOnly] public GameObject secondaryModel;

        public new string name => data.name;

        public float speedMultiplier => GetSpeedMultiplier();
        public float weightModifier => GetWeightModifier();
        public Vector3 jumpDirection => GetJumpDirection();
        public Vector3 cameraPosition => GetCameraPosition();
        public Vector3 overrideRotation => GetOverrideRotation();


        protected virtual float GetSpeedMultiplier() => 1f;
        protected virtual float GetWeightModifier() => 1f;
        protected virtual Vector3 GetJumpDirection() => -entity.gravityDown;
        protected virtual Vector3 GetCameraPosition() => new Vector3(1f, 0f, -3.5f);
        protected virtual Vector3 GetOverrideRotation() => entity.absoluteForward;

        public virtual bool shifting => false;

        protected virtual void WeaponStart(){;}
        protected virtual void WeaponAwake(){;}
        protected virtual void WeaponEnable(){;}
        protected virtual void WeaponDisable(){;}
        protected virtual void UpdateAlways(){;}
        protected virtual void UpdateEquipped(){;}
        protected virtual void FixedUpdateAlways(){;}
        protected virtual void FixedUpdateEquipped(){;}

        private void SetData(){
            if (data != null) return;

            string dataName = GetType().Name.Replace("Weapon","");
            data = DataGetter.GetData<WeaponData>( dataName );
            Debug.Log(dataName);
        }

        private void Start(){
            WeaponStart();
        }
        private void Awake(){
            SetData();
            WeaponAwake();
            Reset();
            LoadModel();
        }

        private void Reset(){
            entity = GetComponent<Entity>();
        }

        private void OnEnable(){
            data.onChangeCostume += LoadModel;
            WeaponEnable();
        }
        private void OnDisable(){
            data.onChangeCostume -= LoadModel;
            WeaponDisable();
        }

        private void OnDestroy(){
            DestroyModel();
        }

        private void Update(){
            UpdateAlways();
            if (entity.currentWeapon == this)
                UpdateEquipped();
        }
        private void FixedUpdate(){
            FixedUpdateAlways();
            if (entity.currentWeapon == this)
                FixedUpdateEquipped();
        }

        public static Type GetWeaponTypeByName(string weaponName){
            return Type.GetType($"SeleneGame.Weapons.{weaponName}Weapon");
        }

        [ContextMenu("LoadModel")]
        public void LoadModel(){
            if (entity == null | data == null) return;

            DestroyModel();

            if (data?.costume.model != null){
                Transform rightWeapon = entity["weaponRight"].transform;
                model = Instantiate(data.costume.model, rightWeapon.position, rightWeapon.rotation, rightWeapon);
                model.name = $"WeaponModel{data.name}";
            }

            if (data?.costume.secondaryModel != null){
                Transform leftWeapon = entity["weaponLeft"].transform;
                secondaryModel = Instantiate(data.costume.secondaryModel, leftWeapon.position, leftWeapon.rotation, leftWeapon);
                secondaryModel.name = $"WeaponModel{data.name}";
            }
            
            Hide();

            // Debug.Log($"Loaded {entity.name}'s {data.name} model.");
        }

        public void DestroyModel(){
            model = Global.SafeDestroy(model);
            secondaryModel = Global.SafeDestroy(secondaryModel);
        }

        public void Display(){
            if (entity == null) return;

            if (model != null) model.SetActive(true);
            if (secondaryModel != null) secondaryModel.SetActive(true);
        }

        public void Hide(){
            if (entity == null) return;

            if (model != null) model.SetActive(false);
            if (secondaryModel != null) secondaryModel.SetActive(false);
        }

    }
}