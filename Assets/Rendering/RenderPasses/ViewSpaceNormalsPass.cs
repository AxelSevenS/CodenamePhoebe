using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace SeleneGame.Core {

    public class ViewSpaceNormalsPass : ScriptableRenderPass {

        private readonly RenderTargetHandle _normalTextureHandle;
        private Material _material;
        private List<ShaderTagId> _shaderTagIds;

        public ViewSpaceNormalsPass() {
            _material = new Material(Shader.Find("Hidden/ViewSpaceNormals"));
            _shaderTagIds = new List<ShaderTagId>() {
                new ShaderTagId("UniversalForward"),
                new ShaderTagId("UniversalForwardOnly"),
                new ShaderTagId("LightweightForward"),
                new ShaderTagId("SRPDefaultUnlit")
            };

            _normalTextureHandle.Init("_ViewSpaceNormals");
        }

        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor) {
            cmd.GetTemporaryRT(_normalTextureHandle.id, cameraTextureDescriptor);
            ConfigureTarget(_normalTextureHandle.Identifier());
            ConfigureClear(ClearFlag.All, Color.black);
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData) {

            if (_material == null) {
                Debug.LogError("Missing Shader. ViewSpaceNormalsPass will not execute. Check for missing reference in the renderer resources.");
                return;
            }



            // var cmd = CommandBufferPool.Get("ViewSpaceNormalsPass");

            // using (new ProfilingScope(cmd, new ProfilingSampler("ViewSpaceNormalsPass"))) {
                // context.ExecuteCommandBuffer(cmd);
                // cmd.Clear();

                DrawingSettings drawingSettings = CreateDrawingSettings(_shaderTagIds, ref renderingData, SortingCriteria.CommonOpaque);
                drawingSettings.overrideMaterial = _material;
                FilteringSettings filteringSettings = new FilteringSettings(RenderQueueRange.opaque);

                context.DrawRenderers(renderingData.cullResults, ref drawingSettings, ref filteringSettings);
            // }

            // context.ExecuteCommandBuffer(cmd);
            // cmd.Clear();
            // CommandBufferPool.Release(cmd);
        }

        public override void OnCameraCleanup(CommandBuffer cmd) {
            cmd.ReleaseTemporaryRT(_normalTextureHandle.id);
        }
    }
}
