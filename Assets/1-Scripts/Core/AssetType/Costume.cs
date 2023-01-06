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

        

        public static T Initialize( T costume, Entity entity ) {
            if ( costume == null )
                return null;

            if ( !costume.isInstance )
                return Initialize(GetInstanceOf(costume), entity);

            if ( costume._entity != null )
                throw new InvalidOperationException($"Asset {costume.name} already initialized");

            costume._entity = entity;
            costume.Setup();

            return costume;
        }

        protected virtual void Setup() {;}

        public abstract void LoadModel();

        public abstract void UnloadModel();

    }
}
