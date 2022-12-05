using System;
using System.Collections.Generic;
using UnityEngine;

using SevenGame.Utility;

namespace SeleneGame.Core {

    public abstract class Costumable<T, TCostume> : InstantiableAsset<T>, IDisposable where T : Costumable<T, TCostume> where TCostume : Costume<TCostume> {
        
        
        [Header("Info")]

        [SerializeField] protected TCostume _baseCostume;

        [SerializeField] protected string _displayName = "Default Name";
        [SerializeField] [TextArea] protected string _description = "Default Description";


        [Header("Instance Data")]
        
        [SerializeField] [ReadOnly] protected Entity _entity;

        [SerializeField] [ReadOnly] protected TCostume _costume;

        private bool disposedValue;



        public TCostume baseCostume {
            get => _baseCostume;
            set {
                _baseCostume = value;
            }
        }

        public string displayName => _displayName;
        public string description => _description;

        public TCostume costume => _costume;



        public virtual void Initialize( Entity entity, TCostume costume = null) {
            if ( !isInstance )
                // throw new InvalidOperationException($"Asset {this.name} is not an instance");
                costume = Costume<TCostume>.GetInstanceOf(costume);

            if (_entity != null)
                throw new InvalidOperationException($"Asset {this.name} already initialized");

            _entity = entity;
            SetCostume( Costume<TCostume>.GetInstanceOf(costume ?? baseCostume) );
        }


        public void SetCostume(string costumeName) {
            SetCostume(Costume<TCostume>.GetInstance(costumeName));
        }

        public virtual void SetCostume(TCostume costume) {

            try {
                costume.Initialize(_entity);
            } catch (Exception e) {
                Debug.LogError(e);
                return;
            }

            _costume?.UnloadModel();

            _costume = costume;
            _costume.LoadModel();
        }


        public void LoadModel() => _costume?.LoadModel();

        public void UnloadModel() => _costume?.UnloadModel();

        

        protected void Dispose(bool disposing) {

            if (!disposedValue) {
                if (disposing)
                    DisposeBehavior();

                disposedValue = true;
            }
        }

        protected virtual void DisposeBehavior() {
            UnloadModel();
        }

        public void Dispose() {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
    
}
