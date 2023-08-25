Shader "Selene/Water" {
    
    Properties {
        [NoScaleOffset] _NormalMap ("Normal Map", 2D) = "bump" {}
        _NormalIntensity ("Normal Intensity", Range(0,1)) = 0
        _NormalSpeed ("Normal Speed", Range(0,1)) = 0
        _NormalScale ("Normal Scale", Range(0,1)) = 0

        _Specular ("Specular", Float) = 1
        _Smoothness ("Smoothness", Range(0,1)) = 0

        _AlphaStrength ("Alpha Strength", Range(0,1)) = 0
        _DepthStrength ("Depth Strength", Range(0,1)) = 0

        _ShallowColor ("Shallow Color", Color) = (0, 0, 0.5)
        _DeepColor ("Deep Color", Color) = (0, 0, 1)
    }
    SubShader {

        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 100

        HLSLINCLUDE

            #include "Packages/com.seven.lunar-render-pipeline/ShaderLibrary/Core.hlsl"

            CBUFFER_START(UnityPerMaterial)

                sampler2D _NormalMap;
                float _NormalIntensity;
                float _NormalSpeed;
                float _NormalScale;

                float _Specular;
                float _Smoothness;

                float _AlphaStrength;
                float _DepthStrength;

                float3 _ShallowColor;
                float3 _DeepColor;

            CBUFFER_END

        ENDHLSL
        
        // Pass {

        //     // Don't Edit this.
        //     Name "StandardSurface"
        //     Tags { "LightMode" = "UniversalForward" }
        //     Cull Off
        //     Blend SrcAlpha OneMinusSrcAlpha
        //     // ZWrite Off

        //     HLSLPROGRAM

        //         #define CustomFragment(surfaceData, inputData, input, facing) WaterFragment(surfaceData, inputData, input, facing)

        //         #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Passes/Lit/LitSubShader.hlsl"
        //         #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Passes/Lit/LitForwardPass.hlsl"

        //     ENDHLSL
        // }
        
        // Pass {

        //     // Don't Edit this.
        //     Name "StandardSurface"
        //     Tags { "LightMode" = "UniversalGBuffer" }
        //     Cull Off
        //     Blend SrcAlpha OneMinusSrcAlpha
        //     // ZWrite Off

        //     HLSLPROGRAM

        //         #define CustomFragment(surfaceData, inputData, input, facing) WaterFragment(surfaceData, inputData, input, facing)

        //         #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Passes/Lit/LitSubShader.hlsl"
        //         #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Passes/Lit/LitGBufferPass.hlsl"

        //     ENDHLSL
        // }
        Pass {

            // Don't Edit this.
            Name "StandardSurface"
            Tags { "LightMode" = "LunarForward" }
            Cull Off
            Blend SrcAlpha OneMinusSrcAlpha
            // ZWrite Off

            HLSLPROGRAM

                #include "Packages/com.seven.lunar-render-pipeline/ShaderLibrary/Input/LitInput.hlsl"
                #include "Functions/WaterFragment.hlsl"

                #include "Packages/com.seven.lunar-render-pipeline/ShaderLibrary/Passes/Lit/LitSubShader.hlsl"
                #include "Packages/com.seven.lunar-render-pipeline/ShaderLibrary/Passes/Lit/LitForwardPass.hlsl"

            ENDHLSL
        }
        
        Pass {

            // Don't Edit this.
            Name "StandardSurface"
            Tags { "LightMode" = "LunarGBuffer" }
            Cull Off
            Blend SrcAlpha OneMinusSrcAlpha
            // ZWrite Off

            HLSLPROGRAM

                #include "Packages/com.seven.lunar-render-pipeline/ShaderLibrary/Input/LitInput.hlsl"
                #include "Functions/WaterFragment.hlsl"

                #include "Packages/com.seven.lunar-render-pipeline/ShaderLibrary/Passes/Lit/LitSubShader.hlsl"
                #include "Packages/com.seven.lunar-render-pipeline/ShaderLibrary/Passes/Lit/LitGBufferPass.hlsl"

            ENDHLSL
        }

        Pass {
            Name "UnderWaterMask"
            Tags{"LightMode" = "Underwater"}
            Cull Off

            HLSLPROGRAM

                #include "Packages/com.seven.lunar-render-pipeline/ShaderLibrary/SurfaceData.hlsl"
                #include "Packages/com.seven.lunar-render-pipeline/ShaderLibrary/Input/UnlitInput.hlsl"
                #include "Functions/WaterVertex.hlsl"

                void UnderWaterMaskFragment( inout SurfaceData surfaceData, inout InputData inputData, VertexOutput input, half facing ) {

                    // float distance = saturate(length(input.positionWS - _WorldSpaceCameraPos) * _DepthStrength);

                    // float fresnel = 1 - saturate(dot(inputData.viewDirectionWS, inputData.normalWS));

                    if (facing > 0) {
                        surfaceData.albedo = half3(0,0,0);
                        surfaceData.alpha = 0;
                    } else {

                        float distance = length(input.positionWS - _WorldSpaceCameraPos);
                        distance = saturate(distance * 1.25);
                        
                        float fogStrength = 1 - log(distance * 0.5 + 1) / log(2);
                        half3 baseColor = lerp(_ShallowColor, _DeepColor, distance);
                        surfaceData.albedo = _DeepColor;
                        // surfaceData.alpha = saturate(log(distance * 15));
                        surfaceData.alpha = distance;
                    }


                }

                #define CustomFragment(surfaceData, inputData, input, facing) UnderWaterMaskFragment(surfaceData, inputData, input, facing)

                #include "Packages/com.seven.lunar-render-pipeline/ShaderLibrary/Passes/Unlit/UnlitSubShader.hlsl"
                #include "Packages/com.seven.lunar-render-pipeline/ShaderLibrary/Passes/Unlit/UnlitForwardPass.hlsl"

            ENDHLSL
        }

        Pass
        {
            Name "DepthOnly"
            Tags{"LightMode" = "DepthOnly"}
            Cull Off

            ZWrite On
            ColorMask 0

            HLSLPROGRAM

                #include "Packages/com.seven.lunar-render-pipeline/ShaderLibrary/Passes/Lit/LitSubShader.hlsl"
                #include "Packages/com.seven.lunar-render-pipeline/ShaderLibrary/Passes/Lit/LitDepthOnlyPass.hlsl"

            ENDHLSL
        }

        // This pass is used when drawing to a _CameraNormalsTexture texture
        Pass
        {
            Name "DepthNormals"
            Tags{"LightMode" = "DepthNormals"}
            Cull Off

            ZWrite On

            HLSLPROGRAM

                #include "Packages/com.seven.lunar-render-pipeline/ShaderLibrary/Passes/Lit/LitSubShader.hlsl"
                #include "Packages/com.seven.lunar-render-pipeline/ShaderLibrary/Passes/Lit/LitDepthNormalsPass.hlsl"

            ENDHLSL
        }

        // Pass {
        //     // Don't Edit this.
        //     Name "ShadowCaster"
        //     Tags { "LightMode" = "ShadowCaster" }
        //     Cull Off
        
        //     HLSLPROGRAM
            
        //         #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Passes/Lit/LitSubShader.hlsl"
        //         #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Passes/ShadowCasterPass.hlsl"
        
        //     ENDHLSL
        // }

        Pass {
            // Don't Edit this.
            Name "Meta"
            Tags{"LightMode" = "Meta"}
            Cull Off

            HLSLPROGRAM

                #include "Packages/com.seven.lunar-render-pipeline/ShaderLibrary/Input/MetaInput.hlsl"

                void SimpleGIContribution( MetaVaryings varyings, inout half4 albedo, inout half4 specularColor) {
                    
                    albedo = half4(0,0,1,1);
                    specularColor = tex2D(_SpecularMap, varyings.uv);

                }
                #define CustomGIContribution(varyings, albedo, specularColor) SimpleGIContribution(varyings, albedo, specularColor)

                #include "Packages/com.seven.lunar-render-pipeline/ShaderLibrary/MetaPass.hlsl"

            ENDHLSL
        }

    }
}
