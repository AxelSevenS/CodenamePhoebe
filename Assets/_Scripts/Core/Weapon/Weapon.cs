using System;
using UnityEngine;
using SeleneGame.Entities;

namespace SeleneGame.Core {
    
    [System.Serializable]
    public abstract class Weapon {

        [Space(35)]
        [ReadOnly] public ArmedEntity entity;
        [ReadOnly] public WeaponData data;

        [ReadOnly] public GameObject model;
        [ReadOnly] public GameObject secondaryModel;

        public new string name => data.name;

        // public float speedMultiplier => GetSpeedMultiplier();
        public float weightModifier => GetWeightModifier();
        public Vector3 jumpDirection => GetJumpDirection();
        public Vector3 cameraPosition => GetCameraPosition();
        public Vector3 overrideRotation => GetOverrideRotation();


        // protected virtual float GetSpeedMultiplier() => 1f;
        protected virtual float GetWeightModifier() => 1f;
        protected virtual Vector3 GetJumpDirection() => -entity.gravityDown;
        protected virtual Vector3 GetCameraPosition() => Player.current.defaultCameraPosition;
        protected virtual Vector3 GetOverrideRotation() => entity.absoluteForward;

        public bool isEquipped => entity.currentWeapon == this;

        public virtual void OnAdd() {
            SetData();
            LoadModel();

            data.onChangeCostume += LoadModel;
        }
        public virtual void OnRemove() {
            DestroyModel();

            data.onChangeCostume -= LoadModel;
        }
        public virtual void WeaponUpdate(){;}
        public virtual void WeaponFixedUpdate(){;}


        private void SetData(){
            if (data != null) return;

            string dataName = GetType().Name.Replace("Weapon","");
            data = DataGetter.GetData<WeaponData>( dataName );
            Debug.Log(dataName);
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
                model = GameObject.Instantiate(data.costume.model, rightWeapon.position, rightWeapon.rotation, rightWeapon);
                model.name = $"WeaponModel{data.name}";
            }

            if (data?.costume.secondaryModel != null){
                Transform leftWeapon = entity["weaponLeft"].transform;
                secondaryModel = GameObject.Instantiate(data.costume.secondaryModel, leftWeapon.position, leftWeapon.rotation, leftWeapon);
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