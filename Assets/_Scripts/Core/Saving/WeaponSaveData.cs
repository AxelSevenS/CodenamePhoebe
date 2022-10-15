using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace SeleneGame.Core {

    [System.Serializable]
    public class WeaponSaveData {
        public string name = "Unarmed";
        public string costumeName = "Unarmed_Base";

        public WeaponSaveData(string name, string costumeName) {
            this.name = name;
            this.costumeName = costumeName;
        }

        public WeaponSaveData(Weapon weapon){
            if (weapon == null)
                return;

            name = weapon.name;
            costumeName = weapon.costume.name;
        }
    }

}
