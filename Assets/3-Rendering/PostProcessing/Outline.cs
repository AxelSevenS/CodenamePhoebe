using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace SeleneGame {

    public sealed class OutlineRenderer : PostProcessEffectRenderer<Outline> {

        private const string shaderPath = "Hidden/SeleneGame/Outline";
        public override void Render(PostProcessRenderContext context) {
            var shader = Shader.Find(shaderPath);
            if (shader == null) {
                Debug.LogError($"Could not find shader : {shaderPath}");
                return;
            }

            var propertySheets = context.propertySheets.Get(shader);
            if (propertySheets == null) {
                Debug.LogError($"Could not find property sheets for shader : {shaderPath}");
                return;
            }

            propertySheets.properties.SetFloat("_OutlineWidth", settings.outlineWidth);
            propertySheets.properties.SetFloat("_DepthStrength", settings.depthStrength);
            propertySheets.properties.SetFloat("_DepthThickness", settings.depthThickness);
            propertySheets.properties.SetFloat("_DepthThreshold", settings.depthThreshold);

            context.command.BlitFullscreenTriangle(context.source, context.destination, propertySheets, 0);
        }
    }

    [System.Serializable]
    [PostProcess(typeof(OutlineRenderer), PostProcessEvent.BeforeStack, "Outline")]
    public class Outline : PostProcessEffectSettings {
        public FloatParameter outlineWidth = new FloatParameter { value = 0.01f };
        public FloatParameter depthStrength = new FloatParameter { value = 0.1f };
        public FloatParameter depthThickness = new FloatParameter { value = 0.1f };
        public FloatParameter depthThreshold = new FloatParameter { value = 0.1f };

    }

}

