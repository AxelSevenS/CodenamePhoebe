using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace SeleneGame.Core {

    public class OutlineRendererFeature : ScriptableRendererFeature {
            
        // [SerializeField] private ViewSpaceNormalsPass _viewSpaceNormalsPass;
        [SerializeField] private OutlineMaskPass _outlineMaskPass;
        [SerializeField] private OutlinePass _outlinePass;

        [SerializeField] private OutlinePassSettings _outlinePassSettings = new OutlinePassSettings(); 

        public override void Create() {
            // _viewSpaceNormalsPass = new ViewSpaceNormalsPass();
            // _viewSpaceNormalsPass.renderPassEvent = RenderPassEvent.AfterRenderingSkybox;
            // _outlineMaskPass = new OutlineMaskPass();
            // _outlineMaskPass.renderPassEvent = RenderPassEvent.AfterRenderingTransparents;
            _outlinePass = new OutlinePass(_outlinePassSettings);
            _outlinePass.renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData) {
            // renderer.EnqueuePass(_viewSpaceNormalsPass);
            // renderer.EnqueuePass(_outlineMaskPass);
            renderer.EnqueuePass(_outlinePass);
        }
    }
}
