using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Rendering.Universal;

namespace SeleneGame.Core {

    public class UnderwaterRendererFeature : ScriptableRendererFeature {
            
        [SerializeField] private UnderwaterEffectPass _underwaterEffectPass;

        [SerializeField] private UnderwaterPassSettings _underwaterPassSettings = new UnderwaterPassSettings(); 

        public override void Create() {
            _underwaterEffectPass = new UnderwaterEffectPass(_underwaterPassSettings);
            _underwaterEffectPass.renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData) {
            renderer.EnqueuePass(_underwaterEffectPass);
        }
    }
}
