using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SeleneGame.Core {
    public interface IWaterDisplaceable {
        
        Vector3 position { get; }
        float waveStrength { get; }
        float waveSpeed { get; }
        float waveFrequency { get; }
        float waveHeight { get; set; }

    }
}
