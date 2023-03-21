using System;
using System.Collections.Generic;
using UnityEngine;

using SevenGame.Utility;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.AddressableAssets;
using UnityEditor;

namespace SeleneGame.Core {

    [Serializable]
    public abstract class Costumable<TData, TCostume, TModel> where TData : CostumableData<TCostume> where TCostume : Costume where TModel : CostumeModel<TCostume> {

        [SerializeField] /* [ReadOnly] */ protected bool displayed = false;

        private bool disposedValue;

        [SerializeField] [HideInInspector] protected TData _data;
        [SerializeReference] protected TModel _model;



        public TData data => _data;
        public TModel model => _model;
        


        public Costumable(TData data) {
            _data = data;
        }




        public void SetCostume(string costumeName) {
            SetCostume(AddressablesUtils.GetAsset<TCostume>(costumeName));
        }

        public abstract void SetCostume(TCostume costume);


        public void Display() {
            if (displayed)
                return;

            displayed = true;
            _model?.Display();
        }

        public void Hide() {
            if (!displayed)
                return;

            displayed = false;
            _model?.Hide();
        }

        

        protected void Dispose(bool disposing) {

            if (!disposedValue) {
                if (disposing)
                    DisposeBehavior();

                disposedValue = true;
            }
        }

        protected virtual void DisposeBehavior() {
            _model?.Dispose();
        }

        public void Dispose() {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }


        public virtual void Update() {
            _model?.Update();
        }
        public virtual void LateUpdate() {
            _model?.LateUpdate();
        }
        public virtual void FixedUpdate() {
            _model?.FixedUpdate();
        }
    }
    
}
