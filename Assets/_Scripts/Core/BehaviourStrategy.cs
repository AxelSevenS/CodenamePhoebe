using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SeleneGame.Core {

    [System.Serializable]
    public abstract class BehaviourStrategy : IDisposable {

        private bool disposedValue;


        [SerializeField] protected Entity _entity;

        public Entity entity => _entity;

        public BehaviourStrategy(Entity entity) {
            _entity = entity;
        }

        protected internal abstract void HandleInput(PlayerEntityController contoller);


        public virtual void Update() {;}
        public virtual void LateUpdate() {;}
        public virtual void FixedUpdate() {;}
        

        protected void Dispose(bool disposing) {

            if (!disposedValue) {
                if (disposing)
                    DisposeBehavior();

                disposedValue = true;
            }
        }

        protected virtual void DisposeBehavior() {
        }

        public void Dispose() {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
