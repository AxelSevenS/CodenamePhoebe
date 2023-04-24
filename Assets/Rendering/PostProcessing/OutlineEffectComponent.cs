using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace SeleneGame.Core {
    
    [System.Serializable]
    [VolumeComponentMenuForRenderPipeline("SeleneGame/Outline", typeof(UniversalRenderPipeline))]
    public class OutlineEffectComponent : VolumeComponent, IPostProcessComponent {

        // public Color outlineColor = Color.white;
        // public float outlineWidth = 0.1f;

        // public float noiseScale = 0.1f;
        // public float noiseIntensity = 0.1f;
        public ColorParameter outlineColor = new ColorParameter(Color.black);
        public ClampedFloatParameter outlineWidth = new ClampedFloatParameter(0.1f, 0f, 1f);

        public ClampedFloatParameter noiseScale = new ClampedFloatParameter(0.1f, 0f, 1f);
        public ClampedFloatParameter noiseIntensity = new ClampedFloatParameter(0.1f, 0f, 1f);


        public bool IsActive()
        {
            return true;
        }

        public bool IsTileCompatible()
        {
            return false;
        }
    }
}
