using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace SeleneGame.Core {

    public class OutlinePass : ScriptableRenderPass {

        private RenderTargetHandle _temporaryBuffer;
        [SerializeField] private OutlinePassSettings _settings;
        [SerializeField] private Material _material;

        public OutlinePass(OutlinePassSettings settings) {
            _settings = settings;

            _material = new Material(settings.outlineShader);

            _material.SetColor("_OutlineColor", settings.outlineColor);
            _material.SetFloat("_OutlineWidth", settings.outlineWidth);
            _material.SetFloat("_NoiseScale", settings.noiseScale);
            _material.SetFloat("_NoiseIntensity", settings.noiseIntensity);
            
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

    [System.Serializable]
    public class OutlinePassSettings {
        public Shader outlineShader;

        public Color outlineColor = Color.white;
        public float outlineWidth = 0.1f;

        public float noiseScale = 0.1f;
        public float noiseIntensity = 0.1f;
    }
}
