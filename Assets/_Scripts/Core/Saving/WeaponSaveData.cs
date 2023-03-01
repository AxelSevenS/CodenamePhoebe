using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace SeleneGame.Core {

    [System.Serializable]
    public class WeaponSaveData {
        public string weaponName;
        public string costumeName;

        public WeaponSaveData(string weaponName, string costumeName) {
            this.weaponName = weaponName;
            this.costumeName = costumeName;
        }

        public WeaponSaveData(Weapon weapon){
            if (weapon == null)
                return;

            weaponName = weapon.data?.name;
            costumeName = weapon.model?.costume?.name;
        }
    }

}
