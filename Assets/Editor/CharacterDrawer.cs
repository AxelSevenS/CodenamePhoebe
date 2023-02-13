using UnityEditor;

namespace SeleneGame.Core {

    [CustomPropertyDrawer(typeof(Character), true)]
    public class CharacterDrawer : CostumableDrawer<Character, CharacterCostume, CharacterModel> {

    }
}
