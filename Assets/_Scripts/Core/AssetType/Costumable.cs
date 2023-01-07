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

    

        public static T Initialize( T costumable, Entity entity, TCostume costume = null) {
            if ( costumable == null )
                return null;

            if ( !costumable.isInstance )
                return Initialize( GetInstanceOf(costumable), entity, costume) ;

            if (costumable._entity != null)
                throw new InvalidOperationException($"Asset {costumable.name} already initialized");

            costumable._entity = entity;
            costumable.SetCostume( costume ?? costumable.baseCostume );

            costumable.Setup();

            return costumable;
        }

        protected virtual void Setup() {;}


        public void SetCostume(string costumeName) {
            SetCostume(Costume<TCostume>.GetInstance(costumeName));
        }

        public virtual void SetCostume(TCostume costume) {

            try {
                costume = Costume<TCostume>.Initialize( costume, _entity );
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
