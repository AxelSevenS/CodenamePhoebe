using System;
using UnityEngine;

using SevenGame.Utility;

namespace SeleneGame.Core {
    
    [System.Serializable]
    public abstract class Weapon : ICostumable<WeaponCostume> {

        public enum WeaponType {lightSword, heavySword, spear, swordAndShield, sparring};
        public abstract WeaponType weaponType { get; }

        [HideInInspector] public string name;

        
        [HideInInspector] public ArmedEntity entity { get; private set; }

        [SerializeField]
        private WeaponCostume _costume;

        public GameObject rightHandModel;
        public GameObject leftHandModel;


        public WeaponCostume costume { 
            get => _costume; 
            private set => _costume = value; 
        }


        public float weightModifier => GetWeightModifier();


        protected virtual float GetWeightModifier() => 1f;

        public virtual void OnAdd( ArmedEntity entity ) {
            name = GetType().Name.Replace("Weapon","");
            this.entity = entity;
            SetCostume( WeaponCostume.GetWeaponBaseCostume( GetType().Name ) );
        }
        public virtual void OnRemove() {
            DestroyModel();
        }

        public virtual void WeaponUpdate(){;}
        public virtual void WeaponFixedUpdate(){;}


        // [ContextMenu("LoadModel")]
        public void SetCostume(WeaponCostume costume){
            if (entity == null) return;

            DestroyModel();

            this.costume = costume;
            
            LoadModel();
            Hide();

        }

        public virtual void LoadModel(){
            if (entity == null || costume == null) return;

            if (costume == null || (costume.model == null && costume.leftHandModel == null) ) return;

            if (costume.model != null) {
                Transform rightWeapon = entity["weaponRight"].transform;
                rightHandModel = GameObject.Instantiate(costume.model, rightWeapon.position, rightWeapon.rotation, rightWeapon);
                rightHandModel.name = $"{name}WeaponModel";
            }

            if (costume.leftHandModel != null) {
                Transform leftWeapon = entity["weaponLeft"].transform;
                leftHandModel = GameObject.Instantiate(costume.leftHandModel, leftWeapon.position, leftWeapon.rotation, leftWeapon);
                leftHandModel.name = $"{name}WeaponModel";
            }
                
        }
        public virtual void DestroyModel(){
            rightHandModel = GameUtility.SafeDestroy(rightHandModel);
            leftHandModel = GameUtility.SafeDestroy(leftHandModel);
        }


        public virtual void Display(){
            if (entity == null) return;

            if (rightHandModel != null) rightHandModel.SetActive(true);
            if (leftHandModel != null) leftHandModel.SetActive(true);
            // if (secondaryModel != null) secondaryModel.SetActive(true);
        }
        public virtual void Hide(){
            if (entity == null) return;

            if (rightHandModel != null) rightHandModel.SetActive(false);
            if (leftHandModel != null) leftHandModel.SetActive(false);
            // if (secondaryModel != null) secondaryModel.SetActive(false);
        }

    }
}