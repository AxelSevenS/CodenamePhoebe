using System;

using UnityEngine;

using SevenGame.Utility;

namespace SeleneGame.Core {
    
    public class Weapon : Costumable<WeaponData, WeaponCostume, WeaponModel>, IDamageDealer {

        [SerializeField] [ReadOnly] private ArmedEntity _armedEntity;
        [SerializeReference] [ReadOnly] private WeaponInventory _inventory;



        public ArmedEntity armedEntity => _armedEntity;
        public WeaponInventory inventory => _inventory;
        


        public Weapon(ArmedEntity armedEntity, WeaponData data, WeaponCostume costume = null, WeaponInventory inventory = null) : base(data){
            _armedEntity = armedEntity;
            SetCostume(costume);
        }


        public void AwardDamage(DamageData damageData) {
            damageData.AddProxy(this);
            armedEntity?.AwardDamage(damageData);
        }

        public void AwardParry(DamageData damageData) {
            damageData.AddProxy(this);
            armedEntity?.AwardParry(damageData);
        }

        public bool IsValidTarget(IDamageable target) {
            return armedEntity.IsValidTarget(target);
        }


        public virtual void HandleInput(Player playerController) {
            Data?.HandleInput(this, playerController);
        }

        public virtual void LightAttack() {
            Data?.LightAttack(this);
        }

        public virtual void HeavyAttack() {
            Data?.HeavyAttack(this);
        }


        public override void SetCostume(WeaponCostume costume) {
            _model?.Dispose();

            costume ??= Data.baseCostume ?? AddressablesUtils.GetDefaultAsset<WeaponCostume>();
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


        public override void Update() {
            base.Update();

            Data?.WeaponUpdate(this);

            Model?.Update();
        }
        public override void LateUpdate() {
            base.LateUpdate();

            Data?.WeaponLateUpdate(this);

            Model?.LateUpdate();
        }
        public override void FixedUpdate() {
            base.FixedUpdate();

            Data?.WeaponFixedUpdate(this);

            Model?.FixedUpdate();
        }

        [Flags]
        public enum WeaponType : byte {
            Sparring = 1 << 0,
            OneHanded = 1 << 1,
            TwoHanded = 1 << 2,
            Polearm = 1 << 3,
            Dual = 1 << 4,
            ShieldAndSword = 1 << 5,
            // = 1 << 6,
            // = 1 << 7 /// Do not exceed 8 flags <see cref="byte"/>
        };

    }
}