using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.AddressableAssets;

using SevenGame.Utility;

namespace SeleneGame.Core {

    public class Character : Costumable<CharacterData, CharacterCostume, CharacterModel> {

        [SerializeField] [ReadOnly] private Entity _entity;


        public Entity Entity => _entity;

        

        public Character(Entity entity, CharacterData data, CharacterCostume costume = null) : base(data) {
            _entity = entity;
            displayed = true;
            SetCostume(costume);
        }

        public override void SetCostume(CharacterCostume costume) {
            _model?.Dispose();

            costume ??= Data.baseCostume ?? AddressablesUtils.GetDefaultAsset<CharacterCostume>();
            _model = costume?.LoadModel(Entity);
            
            if (displayed)
                _model?.Display();
            else
                _model?.Hide();
        }


        public override void Update() {
            base.Update();

            Data?.CharacterUpdate(this);

            Model?.Update();
        }
        public override void LateUpdate() {
            base.LateUpdate();

            Data?.CharacterLateUpdate(this);

            Model?.LateUpdate();
        }
        public override void FixedUpdate() {
            base.FixedUpdate();

            Data?.CharacterFixedUpdate(this);

            Model?.FixedUpdate();
        }

    }

}
