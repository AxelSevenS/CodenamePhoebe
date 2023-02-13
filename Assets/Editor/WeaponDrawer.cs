using UnityEditor;

namespace SeleneGame.Core {

    [CustomPropertyDrawer(typeof(Weapon), true)]
    public class WeaponDrawer : CostumableDrawer<Weapon, WeaponCostume, WeaponModel> {

    }
}
