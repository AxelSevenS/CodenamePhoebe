using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace SeleneGame.Core {

    public class OutlinePass : ScriptableRenderPass {

        [SerializeField] private RenderTargetHandle _temporaryBuffer;
        [SerializeField] private OutlinePassSettings _settings;

        public OutlinePass(OutlinePassSettings settings) {
            _settings = settings;

            // _settings.outlineShader ??= Shader.Find("Hidden/Outline");
        }

        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor) {
            cmd.GetTemporaryRT(_temporaryBuffer.id, cameraTextureDescriptor);
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData) {



            CommandBuffer cmd = CommandBufferPool.Get("OutlinePass");

            RenderTargetIdentifier source = renderingData.cameraData.renderer.cameraColorTarget;

            using (new ProfilingScope(cmd, new ProfilingSampler("OutlinePass"))) {
                cmd.Blit(source, _temporaryBuffer.Identifier(), _settings.outlineMaterial);
                cmd.Blit(_temporaryBuffer.Identifier(), source);
            }
            

            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();
            CommandBufferPool.Release(cmd);
        }

        public override void OnCameraCleanup(CommandBuffer cmd) {
            cmd.ReleaseTemporaryRT(_temporaryBuffer.id);
        }
    }

    [System.Serializable]
    public class OutlinePassSettings {
        // [SerializeField] private Shader _outlineShader;
        // public Shader outlineShader {
        //     get {
        //         _outlineShader ??= Shader.Find("Hidden/Outline");

        //         return _outlineShader;
        //     }
        // }

        // public Color outlineColor = Color.black;
        // public float outlineWidthNormal = 0.1f;
        // public float outlineCutoffNormal = 0.3f;

        // public float outlineWidthDepth = 0.1f;
        // public float outlineCutoffDepth = 0.3f;

        // public float noiseScale = 0.1f;
        // public float noiseIntensity = 0.1f;

        public Material outlineMaterial;
    }
}
