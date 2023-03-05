using System;

using UnityEngine;

using SevenGame.Utility;

namespace SeleneGame.Core {
    
    public class Weapon : Costumable<WeaponData, WeaponCostume, WeaponModel> {

        [SerializeField] [ReadOnly] private ArmedEntity _armedEntity;



        public ArmedEntity armedEntity => _armedEntity;
        


        public Weapon(ArmedEntity armedEntity, WeaponData data, WeaponCostume costume = null) : base(data){
            _armedEntity = armedEntity;
            SetCostume(costume);
        }


        public override void SetCostume(WeaponCostume costume) {
            _model?.Dispose();

            costume ??= data.baseCostume ?? AddressablesUtils.GetDefaultAsset<WeaponCostume>();
            _model = costume?.LoadModel(armedEntity, this);

            if (displayed)
                _model?.Display();
            else
                _model?.Hide();
        }

        public virtual void OnEquip() {
            Display();
        }
        public virtual void OnUnequip() {
            Hide();
        }


        public virtual void Update() {;}
        public virtual void FixedUpdate() {;}



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