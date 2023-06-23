using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace SeleneGame.Core {

    public class OutlinePass : ScriptableRenderPass {

// couldn't find a way to properly blit the effect to the camera using RTHandle so I just used the old way
#pragma warning disable 618

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
        public Material outlineMaterial;
    }

#pragma warning restore 618

}
