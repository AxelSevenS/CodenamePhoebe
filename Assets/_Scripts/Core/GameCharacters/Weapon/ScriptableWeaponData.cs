using System.Collections.Generic;
using UnityEngine;

using Scribe;

namespace SeleneGame.Core {

    [CreateAssetMenu(fileName = "New Scriptable Weapon", menuName = "Data/Scriptable Weapon", order = 0)]
    public class ScriptableWeaponData : WeaponData {

        public List<ScribeEvent<WeaponAction, InputCondition>> inputEvents = new List<ScribeEvent<WeaponAction, InputCondition>>();
        
    }
}
