using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace SeleneGame.Core {

    public class UnderwaterEffectPass : ScriptableRenderPass {

        [SerializeField] private RenderTargetHandle _temporaryBuffer;
        private readonly RenderTargetHandle _maskHandle;
        private List<ShaderTagId> _shaderTagIds;
        
        [SerializeField] private UnderwaterPassSettings _settings;

        public UnderwaterEffectPass(UnderwaterPassSettings settings) {
            _settings = settings;
            _shaderTagIds = new List<ShaderTagId>() {
                new ShaderTagId("Underwater")
            };

            _maskHandle.Init("_UnderwaterMask");
        }

        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor) {
            cmd.GetTemporaryRT(_temporaryBuffer.id, cameraTextureDescriptor);
            cmd.GetTemporaryRT(_maskHandle.id, cameraTextureDescriptor);
            ConfigureTarget(_maskHandle.Identifier());
            ConfigureClear(ClearFlag.All, Color.black);
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData) {


            var cmd = CommandBufferPool.Get("UnderwaterEffectPass");

            RenderTargetIdentifier source = renderingData.cameraData.renderer.cameraColorTarget;

            using (new ProfilingScope(cmd, new ProfilingSampler("UnderwaterEffectPass"))) {

                DrawingSettings drawingSettings = CreateDrawingSettings(_shaderTagIds, ref renderingData, SortingCriteria.CommonOpaque);
                FilteringSettings filteringSettings = new FilteringSettings(RenderQueueRange.transparent);

                context.DrawRenderers(renderingData.cullResults, ref drawingSettings, ref filteringSettings);

                cmd.Blit(source, _temporaryBuffer.Identifier(), _settings.underwaterMaterial);
                cmd.Blit(_temporaryBuffer.Identifier(), source);
            }

            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();
            CommandBufferPool.Release(cmd);
        }

        public override void OnCameraCleanup(CommandBuffer cmd) {
            cmd.ReleaseTemporaryRT(_temporaryBuffer.id);
            cmd.ReleaseTemporaryRT(_maskHandle.id);
        }
    }

    
    [System.Serializable]
    public class UnderwaterPassSettings {

        public Material underwaterMaterial;
    }
}
