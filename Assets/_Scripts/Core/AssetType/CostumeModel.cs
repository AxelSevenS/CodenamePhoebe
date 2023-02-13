using System;
using System.Collections;
using System.Collections.Generic;
using SevenGame.Utility;
using UnityEngine;

namespace SeleneGame.Core {

    [Serializable]
    public abstract class CostumeModel<TCostume> : IDisposable where TCostume : Costume<TCostume> {

        private bool disposedValue;
        [SerializeField] [ReadOnly] protected TCostume _costume;
        [SerializeField] [ReadOnly] protected CostumeData _costumeData;


        public abstract Transform mainTransform { get; }
        public TCostume costume => _costume;
        public CostumeData costumeData => _costumeData;



        public CostumeModel(TCostume costume) {
            _costume = costume;
        }

        public abstract void Unload();


        public virtual void Update(){;}
        public virtual void FixedUpdate(){;}


        protected void Dispose(bool disposing) {

            if (!disposedValue) {
                if (disposing)
                    Unload();

                disposedValue = true;
            }
        }

        public void Dispose() {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
