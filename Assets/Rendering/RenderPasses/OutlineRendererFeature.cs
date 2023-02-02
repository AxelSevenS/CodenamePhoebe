using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace SeleneGame.Core {

    public class OutlineRendererFeature : ScriptableRendererFeature {
            
        private ViewSpaceNormalsPass _viewSpaceNormalsPass;
        private OutlinePass _outlinePass;

        public override void Create() {
            _viewSpaceNormalsPass = new ViewSpaceNormalsPass();
            _viewSpaceNormalsPass.renderPassEvent = RenderPassEvent.AfterRenderingSkybox;
            _outlinePass = new OutlinePass();
            _outlinePass.renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData) {
            renderer.EnqueuePass(_viewSpaceNormalsPass);
            renderer.EnqueuePass(_outlinePass);
        }
    }
}
