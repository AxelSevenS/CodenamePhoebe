using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SeleneGame.Core {
    [CreateAssetMenu(fileName = "new Water Profile", menuName = "Environment/Water Profile", order = 0)]
    public class WaterProfile : ScriptableObject {
        [SerializeField] private float strengthFactor = 0.075f;
        [SerializeField] private float frequencyFactor = 40f;
        public Vector4[] waves = new Vector4[0];

        [ContextMenu("Randomize Waves")]
        public void RandomizeWaves() {
            for (int i = 0; i < waves.Length; i++) {
                Vector2 direction = Random.insideUnitCircle.normalized;
                waves[i] = new Vector4(direction.x, direction.y, Random.value * strengthFactor, Random.value * frequencyFactor + 2f);
            }
        }
    }
}
