using System;
using System.Collections;
using System.Collections.Generic;
using SevenGame.Utility;
using UnityEngine;
using UnityEngine.Localization;

namespace SeleneGame.Core {

    public abstract class Costume : ScriptableObject {

        public bool accessibleInGame = false;
        
        [Tooltip("The Portrait of the Costume, used as a preview in menus.")]
        public Sprite portrait;

        [Tooltip("The Display Name of the Costume, used in menus.")]
        public LocalizedString displayName;

        [Tooltip("The description of the Costume, only appears when it is not the Base Costume.")]
        public LocalizedString description;


    }
}
