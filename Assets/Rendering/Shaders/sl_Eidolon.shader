Shader "Selene/Eidolon" {
    
    Properties {
        [NoScaleOffset]_MainTex ("Main Texture", 2D) = "white" {}
        [NoScaleOffset]_FlowMap ("Flow Texture", 2D) = "blue" {}
        [NoScaleOffset]_HuskMap ("Husk Texture", 2D) = "black" {}

        _HuskDistance ("Husk Distance", Range(0.0, 0.5)) = 0.05
        _Scale ("Scale", Float) = 1
    }
    SubShader {

        Tags { "RenderType"="Opaque" }
        LOD 100

        HLSLINCLUDE

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Passes/Lit/LitInput.hlsl"
            #include "Packages/com.seven.utility/ShaderLibrary/MathUtility.hlsl"


            CBUFFER_START(UnityPerMaterial)

            sampler2D _MainTex;
            float4 _MainTex_ST;

            sampler2D _FlowMap;
            float4 _FlowMap_ST;

            sampler2D _HuskMap;
            float4 _HuskMap_ST;

            float _HuskDistance = 0.05;
            float _Scale = 1;

            CBUFFER_END

            half4 baseColor;
            
            void StandardVertexDisplacement( inout VertexOutput output ) {
            }

            bool StandardClipping( in VertexOutput input, half facing ) {
                baseColor = FlowMap(_MainTex, _FlowMap, input.uv, _Time[0] * 2, 0.5);
                clip (baseColor.a <= 0.5 ? -1 : 0);

                return false;
            }

            void StandardFragment( inout SurfaceData surfaceData, inout InputData inputData, VertexOutput input, half facing ) {

                surfaceData.albedo = baseColor.rgb;
                surfaceData.alpha = baseColor.a;
                surfaceData.specular = 0;
                surfaceData.metallic = 0;
                surfaceData.smoothness = 0;
                surfaceData.emission = half3(0, 0, 0);
            }
            
            void HuskVertexDisplacement( inout VertexOutput output ) {
                output.positionWS += output.normalWS * _HuskDistance;
            }

            bool HuskClipping( in VertexOutput input, half facing ) {
                baseColor = tex2D(_HuskMap, input.uv);
                    
                if (baseColor.a > 0.5) {
                    float waveScale = 5 * _Scale;
                    float waveSpeed = _Time[1] * (_Scale + 1);
                    float wave = saturate( sin( waveScale * input.uv.y + waveSpeed ) ) ;

                    float noise = MovingFractalNoise( input.uv, _Time[1], 5 * _Scale);
                    noise = saturate( noise + 0.5 );

                    float mask = saturate( 1 - (noise * wave) );

                    clip (mask <= 0.5 ? -1 : 0);
                    baseColor *= mask;
                } else {
                    clip(-1);
                }

                return false;
            }

            void HuskFragment( inout SurfaceData surfaceData, inout InputData inputData, VertexOutput input, half facing ) {

                surfaceData.albedo = baseColor.rgb;
                surfaceData.alpha = baseColor.a;
                surfaceData.specular = 0;
                surfaceData.metallic = 0;
                surfaceData.smoothness = 0;
                surfaceData.emission = surfaceData.albedo;
            }


        ENDHLSL
        
        Pass {

            // Don't Edit this.
            Name "StandardForward"
            Tags { "LightMode" = "UniversalForward" }

            HLSLPROGRAM

                #define CustomVertexDisplacement(output) StandardVertexDisplacement(output)
                #define CustomClipping(output, facing) StandardClipping(output, facing)
                #define CustomFragment(surfaceData, inputData, input, facing) StandardFragment(surfaceData, inputData, input, facing)

                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Passes/Lit/LitSubShader.hlsl"
                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Passes/Lit/LitForwardPass.hlsl"

            ENDHLSL
        }

        Pass {

            // Don't Edit this.
            Name "StandardGBuffer"
            Tags { "LightMode" = "UniversalGBuffer" }

            HLSLPROGRAM

                #define CustomVertexDisplacement(output) StandardVertexDisplacement(output)
                #define CustomClipping(output, facing) StandardClipping(output, facing)
                #define CustomFragment(surfaceData, inputData, input, facing) StandardFragment(surfaceData, inputData, input, facing)

                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Passes/Lit/LitSubShader.hlsl"
                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Passes/Lit/LitGBufferPass.hlsl"

            ENDHLSL
        }
        
        Pass {

            // Don't Edit this.
            Name "HuskForward"
            // Tags { "LightMode" = "UniversalForward" }

            HLSLPROGRAM

                #define CustomVertexDisplacement(output) HuskVertexDisplacement(output)
                #define CustomClipping(output, facing) HuskClipping(output, facing)
                #define CustomFragment(surfaceData, inputData, input, facing) HuskFragment(surfaceData, inputData, input, facing)

                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Passes/Lit/LitSubShader.hlsl"
                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Passes/Lit/LitForwardPass.hlsl"

            ENDHLSL
        }

        Pass {
            Name "DepthOnly"
            Tags{"LightMode" = "DepthOnly"}

            ZWrite On
            ColorMask 0

            HLSLPROGRAM

                #define CustomClipping(output, facing) StandardClipping(output, facing)

                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Passes/Lit/LitSubShader.hlsl"
                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Passes/Lit/LitDepthOnlyPass.hlsl"

            ENDHLSL
        }

        // This pass is used when drawing to a _CameraNormalsTexture texture
        Pass {
            Name "DepthNormals"
            Tags{"LightMode" = "DepthNormals"}

            ZWrite On

            HLSLPROGRAM

                #define CustomClipping(output, facing) StandardClipping(output, facing)

                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Passes/Lit/LitSubShader.hlsl"
                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Passes/Lit/LitDepthNormalsPass.hlsl"

            ENDHLSL
        }

        Pass {
            // Don't Edit this.
            Name "ShadowCaster"
            Tags { "LightMode" = "ShadowCaster" }
        
            HLSLPROGRAM
            
                #define CustomClipping(output, facing) StandardClipping(output, facing)
                #define CustomVertexDisplacement(output) StandardVertexDisplacement(output)

                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Passes/Lit/LitSubShader.hlsl"
                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Passes/Lit/LitShadowCasterPass.hlsl"
        
            ENDHLSL
        }

        Pass {
            // Don't Edit this.
            Name "Meta"
            Tags{"LightMode" = "Meta"}

            Cull Off

            HLSLPROGRAM

                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Passes/Lit/LitMetaInput.hlsl"

                void SimpleGIContribution( MetaVaryings varyings, inout half4 albedo, inout half4 specularColor) {
                    
                    albedo = tex2D(_MainTex, varyings.uv);
                    specularColor = 0.0.xxxx;

                }
                #define CustomGIContribution(varyings, albedo, specularColor) SimpleGIContribution(varyings, albedo, specularColor)

                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Passes/Lit/LitMetaPass.hlsl"

            ENDHLSL
        }

    }
}
