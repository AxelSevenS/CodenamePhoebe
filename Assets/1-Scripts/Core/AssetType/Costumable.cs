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



        public void SetCostume(string costumeName) {
            SetCostume(Costume<TCostume>.GetInstance(costumeName));
        }

        public virtual void SetCostume(TCostume costume) {
            if (costume == null) return;

            _costume = costume;
            LoadModel();
        }


        public void LoadModel() => _costume?.LoadModel();

        public void UnloadModel() => _costume?.UnloadModel();

        

        protected virtual void Dispose(bool disposing) {

            if (!disposedValue) {
                if (disposing)
                    UnloadModel();

                disposedValue = true;
            }
        }

        public void Dispose() {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
    
}
