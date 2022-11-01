using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.AddressableAssets;

using SevenGame.Utility;

namespace SeleneGame.Core {
    
    public abstract class Weapon : InstantiableAsset<Weapon> {

        [SerializeField] protected ArmedEntity entity;


        [Header("Weapon Info")]

        [SerializeField] private WeaponCostume _baseCostume;

        [SerializeField] [NotFlagEnum] private WeaponType _weaponType;

        [SerializeField] private string _displayName = "Default Weapon Name";
        
        [SerializeField] [TextArea] private string _description = "Default Weapon Description";

        public float weightModifier = 1f;


        [Header("Weapon Data")]

        [SerializeField] /* [ReadOnly]  */private WeaponCostume _costume;



        public WeaponCostume baseCostume {
            get {
                return _baseCostume;
            }
            set {
                _baseCostume = value;
            }
        }

        public WeaponType weaponType => _weaponType;

        public string displayName => _displayName;
        public string description => _description;

        public WeaponCostume costume {
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



        public void Initialize( ArmedEntity entity, WeaponCostume costume = null) {
            if (this.entity != null)
                return;

            this.entity = entity;
            SetCostume( costume ?? baseCostume );
        }

        protected internal virtual void OnEquip(){
            Display();
        }
        protected internal virtual void OnUnequip(){
            Hide();
        }

        protected internal abstract void LoadModel();
        protected internal abstract void UnloadModel();


        public void SetCostume(WeaponCostume costume){
            _costume = costume;
            LoadModel();
        }

        public abstract void Display();
        public abstract void Hide();


        protected internal virtual void Update( ){;}
        protected internal virtual void FixedUpdate( ){;}
        


        [Flags]
        public enum WeaponType : byte {
            OneHanded = 1 << 0,
            TwoHanded = 1 << 1,
            Staff = 1 << 2,
            DoubleOneHanded = 1 << 3,
            Sparring = 1 << 4
        };

    }
}