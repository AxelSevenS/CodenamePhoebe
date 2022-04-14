using System;
using UnityEngine;

namespace SeleneGame.Core {
    
    public abstract class Weapon : MonoBehaviour {

        public enum WeaponType {lightSword, heavySword, spear, swordAndShield, sparring};
        public virtual WeaponType weaponType => WeaponType.sparring;

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
        protected virtual Vector3 GetOverrideRotation() => entity.relativeForward;

        public virtual bool canJump => false;
        public virtual bool canEvade => false;
        public virtual bool cannotTurn => false;
        public virtual bool noGravity => false;

        public virtual bool shifting => false;

        protected virtual void OnEnableWeapon(){;}

        protected virtual void OnDisableWeapon(){;}

        protected virtual void UpdateAlways(){;}

        protected virtual void UpdateEquipped(){;}

        private void Awake(){
            Reset();
            LoadModel();
        }

        private void Reset(){
            entity = GetComponent<Entity>();
            string dataName = GetType().Name.Replace("Weapon","");
            data = UnitData.GetDataByName<WeaponData>( dataName );
        }

        private void OnEnable(){
            data.changeCostume += LoadModel;
            OnEnableWeapon();
        }
        private void OnDisable(){
            data.changeCostume -= LoadModel;
            OnDisableWeapon();
        }

        private void OnDestroy(){
            DestroyModel();
        }

        private void Update(){
            UpdateAlways();
            if (entity.currentWeapon == this)
                UpdateEquipped();
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

            if (entity.currentWeapon == this)
                Display();
            else
                Hide();

            // Debug.Log($"Loaded {entity.name}'s {data.name} model.");
        }

        public void DestroyModel(){
            if (entity == null) return;

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