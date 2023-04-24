using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SeleneGame.Core {
    [CreateAssetMenu(fileName = "new Water Profile", menuName = "Environment/Water Profile", order = 0)]
    public class WaterProfile : ScriptableObject {
        public Vector4[] waves = new Vector4[0];

        [ContextMenu("Randomize Waves")]
        private void RandomizeWaves() {
            for (int i = 0; i < waves.Length; i++) {
                Vector2 direction = Random.insideUnitCircle.normalized;
                waves[i] = new Vector4(direction.x, direction.y, Random.value * 0.075f, Random.value * 40f + 2f);
            }
        }
    }
}
