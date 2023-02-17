using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace SeleneGame.Core {

    public class OutlinePass : ScriptableRenderPass {

        private RenderTargetHandle _temporaryBuffer;
        [SerializeField] private Material _material;

        public OutlinePass() {
            _material = new Material(Shader.Find("Hidden/Outline"));
            // _material.SetTexture("_ViewSpaceNormals", Shader.GetGlobalTexture("_ViewSpaceNormals"));
        }

        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor) {
            cmd.GetTemporaryRT(_temporaryBuffer.id, cameraTextureDescriptor);
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData) {

            if (_material == null) {
                Debug.LogError("Missing Shader. OutlinePass will not execute. Check for missing reference in the renderer resources.");
                return;
            }

            CommandBuffer cmd = CommandBufferPool.Get("OutlinePass");

            using (new ProfilingScope(cmd, new ProfilingSampler("OutlinePass"))) {
                context.ExecuteCommandBuffer(cmd);
                cmd.Clear();
                cmd.Blit(renderingData.cameraData.renderer.cameraColorTarget, _temporaryBuffer.Identifier(), _material);
                cmd.Blit(_temporaryBuffer.Identifier(), renderingData.cameraData.renderer.cameraColorTarget);
            }
            

            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();
            CommandBufferPool.Release(cmd);
        }

        public override void OnCameraCleanup(CommandBuffer cmd) {
            cmd.ReleaseTemporaryRT(_temporaryBuffer.id);
        }
    }
}
