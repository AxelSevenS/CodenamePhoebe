using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace SeleneGame.Core {

    public class OutlineMaskPass : ScriptableRenderPass {

        private readonly RenderTargetHandle _maskHandle;
        private List<ShaderTagId> _shaderTagIds;

        public OutlineMaskPass() {
            _shaderTagIds = new List<ShaderTagId>() {
                new ShaderTagId("Outline")
            };

            _maskHandle.Init("_OutlineMask");
        }

        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor) {
            cmd.GetTemporaryRT(_maskHandle.id, cameraTextureDescriptor);
            ConfigureTarget(_maskHandle.Identifier());
            ConfigureClear(ClearFlag.All, Color.black);
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData) {


            var cmd = CommandBufferPool.Get("OutlineMaskPass");

            using (new ProfilingScope(cmd, new ProfilingSampler("OutlineMaskPass"))) {

                DrawingSettings drawingSettings = CreateDrawingSettings(_shaderTagIds, ref renderingData, SortingCriteria.CommonOpaque);
                FilteringSettings filteringSettings = new FilteringSettings(RenderQueueRange.opaque);

                context.DrawRenderers(renderingData.cullResults, ref drawingSettings, ref filteringSettings);

                // cmd.Blit(_maskHandle.Identifier(), renderingData.cameraData.renderer.cameraColorTarget);
            }

            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();
            CommandBufferPool.Release(cmd);
        }

        public override void OnCameraCleanup(CommandBuffer cmd) {
            cmd.ReleaseTemporaryRT(_maskHandle.id);
        }
    }
}
