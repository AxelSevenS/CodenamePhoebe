using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace SeleneGame.Core {

    [System.Serializable]
    public class WeaponSaveData {
        public Type weaponType = typeof(UnarmedWeapon);
        public string costumeName = "Unarmed_Base";

        public WeaponSaveData(Type weaponType, string costumeName) {
            this.weaponType = weaponType;
            this.costumeName = costumeName;
        }

        public WeaponSaveData(Weapon weapon){
            if (weapon == null)
                return;

            weaponType = weapon.GetType();
            costumeName = weapon.model?.costume?.name;
        }
    }

}
