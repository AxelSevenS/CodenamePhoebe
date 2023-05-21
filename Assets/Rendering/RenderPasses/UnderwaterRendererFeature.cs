using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace SeleneGame.Core {

    public class UnderwaterRendererFeature : ScriptableRendererFeature {
            
        [SerializeField] private UnderwaterEffectPass _underwaterEffectPass;

        [SerializeField] private UnderwaterPassSettings _outlinePassSettings = new UnderwaterPassSettings(); 

        public override void Create() {
            _underwaterEffectPass = new UnderwaterEffectPass(_outlinePassSettings);
            _underwaterEffectPass.renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData) {
            renderer.EnqueuePass(_underwaterEffectPass);
        }
    }
}
