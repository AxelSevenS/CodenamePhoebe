using System;
using UnityEngine;

using SevenGame.Utility;

namespace SeleneGame.Core {
    
    [System.Serializable]
    public abstract class Weapon : ICostumable<WeaponCostume> {

        public enum WeaponType {lightSword, heavySword, spear, swordAndShield, sparring};
        public abstract WeaponType weaponType { get; }

        [HideInInspector] public string name;

        
        [HideInInspector] public ArmedEntity entity;

        public WeaponCostume costume;
        public GameObject model;

        public bool isEquipped => entity.weapons.current == this;


        public float weightModifier => GetWeightModifier();
        public Vector3 jumpDirection => GetJumpDirection();


        protected virtual float GetWeightModifier() => 1f;
        protected virtual Vector3 GetJumpDirection() => -entity.gravityDown;

        public virtual void OnAdd() {
            name = GetType().Name.Replace("Weapon","");
            SetCostume( WeaponCostume.GetWeaponBaseCostume( GetType() ) );
        }
        public virtual void OnRemove() {
            DestroyModel();
        }

        public virtual void WeaponUpdate(){;}
        public virtual void WeaponFixedUpdate(){;}


        // [ContextMenu("LoadModel")]
        public void SetCostume(WeaponCostume newCostume){
            if (entity == null) return;

            DestroyModel();

            costume = newCostume;
            
            LoadModel();
            Hide();

        }

        public virtual void LoadModel(){
            if (costume == null || costume.model == null) return;

            Transform rightWeapon = entity["weaponRight"].transform;
            model = GameObject.Instantiate(costume.model, rightWeapon.position, rightWeapon.rotation, rightWeapon);
            model.name = "WeaponModel";
                
        }
        public virtual void DestroyModel(){
            model = GameUtility.SafeDestroy(model);
        }


        public virtual void Display(){
            if (entity == null) return;

            if (model != null) model.SetActive(true);
            // if (secondaryModel != null) secondaryModel.SetActive(true);
        }
        public virtual void Hide(){
            if (entity == null) return;

            if (model != null) model.SetActive(false);
            // if (secondaryModel != null) secondaryModel.SetActive(false);
        }

    }
}