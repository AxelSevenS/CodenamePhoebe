using System;
using System.Collections;
using System.Collections.Generic;
using SevenGame.Utility;
using UnityEngine;

namespace SeleneGame.Core {

    public abstract class Costume<T> : InstantiableAsset<T> where T : Costume<T> {

        [Tooltip("The Portrait of the Costume, used as a preview in menus.")]
        public Sprite portrait;

        [Tooltip("The Display Name of the Costume, used in menus.")]
        public string displayName = "Default Costume Name";

        [Tooltip("The description of the Costume, only appears when it is not the Base Costume.")]
        [TextArea] 
        public string description = "Default Costume Description";

        [SerializeField] [ReadOnly] protected Entity _entity;

        

        public virtual void Initialize( Entity entity) {
            if ( !isInstance )
                throw new InvalidOperationException($"Asset {this.name} is not an instance");
            if ( _entity != null )
                throw new InvalidOperationException($"Asset {this.name} already initialized");

            _entity = entity;
        }

        public abstract void LoadModel();

        public abstract void UnloadModel();

    }
}
