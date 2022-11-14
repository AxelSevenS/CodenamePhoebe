using System.Collections;
using System.Collections.Generic;
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

        public abstract void LoadModel();

        public abstract void UnloadModel();

    }
}
