using System;
using UnityEngine;

namespace SeleneGame.Core {
    
    [System.Serializable]
    public abstract class Weapon : ICostumable<WeaponCostume> {
        public enum WeaponType {lightSword, heavySword, spear, swordAndShield, sparring};

        [HideInInspector] public new string name;

        
        [HideInInspector] public ArmedEntity entity;

        public WeaponCostume costume;
        public GameObject model;
        
        public virtual WeaponType weaponType => WeaponType.sparring;


        public float weightModifier => GetWeightModifier();
        public Vector3 jumpDirection => GetJumpDirection();
        public Vector3 cameraPosition => GetCameraPosition();


        protected virtual float GetWeightModifier() => 1f;
        protected virtual Vector3 GetJumpDirection() => -entity.gravityDown;
        protected virtual Vector3 GetCameraPosition() => Player.current.defaultCameraPosition;

        public bool isEquipped => entity.weapons.current == this;

        public virtual void OnAdd() {
            name = GetType().Name.Replace("Weapon","");
            SetCostume( WeaponCostume.GetWeaponBaseCostume(this) );
        }
        public virtual void OnRemove() {
            DestroyModel();
        }
        public virtual void WeaponUpdate(){;}
        public virtual void WeaponFixedUpdate(){;}


        // private void SetData(){
        //     if (data != null) return;

        //     string dataName = GetType().Name.Replace("Weapon","");
        //     data = DataGetter.GetData<WeaponData>( dataName );
        //     Debug.Log(dataName);
        // }

        // public static Type GetWeaponTypeByName(string weaponName){
        //     return Type.GetType($"SeleneGame.Weapons.{weaponName}Weapon");
        // }

        // [ContextMenu("LoadModel")]
        public void SetCostume(WeaponCostume newCostume){
            if (entity == null) return;

            DestroyModel();

            costume = newCostume;
            
            LoadModel();
            Hide();

            // Debug.Log($"Loaded {entity.name}'s {data.name} model.");
        }

        public void LoadModel(){
            if (costume == null || costume.model == null) return;

            Transform rightWeapon = entity["weaponRight"].transform;
            model = GameObject.Instantiate(costume.model, rightWeapon.position, rightWeapon.rotation, rightWeapon);
            model.name = "WeaponModel";
                
        }

        public void DestroyModel(){
            model = Global.SafeDestroy(model);
            // secondaryModel = Global.SafeDestroy(secondaryModel);
        }

        public void Display(){
            if (entity == null) return;

            if (model != null) model.SetActive(true);
            // if (secondaryModel != null) secondaryModel.SetActive(true);
        }

        public void Hide(){
            if (entity == null) return;

            if (model != null) model.SetActive(false);
            // if (secondaryModel != null) secondaryModel.SetActive(false);
        }

    }
}