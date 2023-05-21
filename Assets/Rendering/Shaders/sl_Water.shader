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
    }
    SubShader {

        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 100

        HLSLINCLUDE

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"
            // #include "Packages/com.seven.utility/ShaderLibrary/MathUtility.hlsl"
            #include "Functions/Lit/LitInput.hlsl"
            #include "Functions/WaterWaveDisplacement.hlsl"


            CBUFFER_START(UnityPerMaterial)

                sampler2D _NormalMap;
                float _NormalIntensity;
                float _NormalSpeed;
                float _NormalScale;

                float _Specular;
                float _Smoothness;

                float _AlphaStrength;
                float _DepthStrength;

            CBUFFER_END


            // Define the custom Vertex and Fragment functions.
            
            void WaterVertexDisplacement( inout VertexOutput output ) {
                float3 tangent;
                float3 binormal;
                float3 normal;
                float3 displacement = WaveDisplacement(output.positionWS, _Time[1], tangent, binormal, normal);
                output.positionWS += displacement;
                output.normalWS = normal;
                output.tangentWS = tangent;
                output.bitangentWS = -binormal;
            }

            bool WaterClipping( in VertexOutput input, half facing ) {
                return false;
            }

            float UnpackDepth( float depth, float posW ) {
                return - depth;
            }

            void WaterFragment( inout SurfaceData surfaceData, inout InputData inputData, VertexOutput input, half facing ) {

                float3 deepColor = float3(0.04343182, 0.06204228, 0.1509434);
                float3 shallowColor = float3(0.5613652, 0.7575701, 0.8207547);

                if (facing > 0) {

                    float depth = - Linear01Depth(SampleSceneDepth(input.positionSS.xy / input.positionSS.w).r, _ZBufferParams) * _ProjectionParams.z + input.positionSS.w;

                    // float depthColorFactor = saturate(1 - exp(depth * _DepthStrength));
                    float depthColorFactor = saturate(1 - exp(depth * _DepthStrength));

                    surfaceData.albedo = lerp(shallowColor, deepColor, depthColorFactor);


                    float alphaDepth = saturate(1 - exp(depth * _AlphaStrength));
                    // float fresnel = 1;
                    float fresnel = pow(1 - saturate(dot(inputData.viewDirectionWS, inputData.normalWS)), _AlphaStrength);
                    
                    surfaceData.alpha = saturate(alphaDepth * fresnel);

                } else {

                    surfaceData.albedo = shallowColor;
                    surfaceData.alpha = saturate(length(input.positionWS - _WorldSpaceCameraPos) * _AlphaStrength * 0.5);
                }

                

                surfaceData.specular = _Specular;
                surfaceData.metallic = 0;
                surfaceData.smoothness = _Smoothness;
                surfaceData.emission = half3(0, 0, 0);

                if ( _NormalIntensity != 0) {
                    surfaceData.normalTS = UnpackNormal(tex2D(_NormalMap, input.uv * _NormalScale));
                    half3 mapNormal = mul( inputData.tangentToWorld, surfaceData.normalTS);
                    inputData.normalWS = lerp(inputData.normalWS, mapNormal, _NormalIntensity);
                }
            }

            #define CustomVertexDisplacement(output) WaterVertexDisplacement(output)
            #define CustomClipping(output, facing) WaterClipping(output, facing)

        ENDHLSL
        
        Pass {

            // Don't Edit this.
            Name "StandardSurface"
            Tags { "LightMode" = "UniversalForward" }
            Cull Off
            Blend SrcAlpha OneMinusSrcAlpha
            // ZWrite Off

            HLSLPROGRAM

                #define CustomFragment(surfaceData, inputData, input, facing) WaterFragment(surfaceData, inputData, input, facing)

                #include "Functions/Lit/LitSubShader.hlsl"
                #include "Functions/Lit/LitForwardPass.hlsl"

            ENDHLSL
        }
        
        Pass {

            // Don't Edit this.
            Name "StandardSurface"
            Tags { "LightMode" = "UniversalGBuffer" }
            Cull Off
            Blend SrcAlpha OneMinusSrcAlpha
            // ZWrite Off

            HLSLPROGRAM

                #define CustomFragment(surfaceData, inputData, input, facing) WaterFragment(surfaceData, inputData, input, facing)

                #include "Functions/Lit/LitSubShader.hlsl"
                #include "Functions/Lit/LitGBufferPass.hlsl"

            ENDHLSL
        }

        Pass
        {
            Name "UnderWaterMask"
            Tags{"LightMode" = "Underwater"}
            Cull Off

            HLSLPROGRAM

                void UnderWaterMaskFragment( inout SurfaceData surfaceData, inout InputData inputData, VertexOutput input, half facing ) {

                    float distance = saturate(length(input.positionWS - _WorldSpaceCameraPos) * _DepthStrength);

                    half3 baseColor = facing < 0 ? distance.xxx : half3(0,0,0);
                    surfaceData.albedo = baseColor;
                    surfaceData.alpha = 1;

                }

                #define CustomFragment(surfaceData, inputData, input, facing) UnderWaterMaskFragment(surfaceData, inputData, input, facing)

                #include "Functions/Unlit/UnlitSubShader.hlsl"
                #include "Functions/Unlit/UnlitForwardPass.hlsl"

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

                #include "Functions/Lit/LitSubShader.hlsl"
                #include "Functions/Lit/LitDepthOnlyPass.hlsl"

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

                #include "Functions/Lit/LitSubShader.hlsl"
                #include "Functions/Lit/LitDepthNormalsPass.hlsl"

            ENDHLSL
        }

        // Pass {
        //     // Don't Edit this.
        //     Name "ShadowCaster"
        //     Tags { "LightMode" = "ShadowCaster" }
        //     Cull Off
        
        //     HLSLPROGRAM
            
        //         #include "Functions/Lit/LitSubShader.hlsl"
        //         #include "Functions/Lit/LitShadowCasterPass.hlsl"
        
        //     ENDHLSL
        // }

        // Pass {
        //     // Don't Edit this.
        //     Name "Meta"
        //     Tags{"LightMode" = "Meta"}
        //     Cull Off

        //     HLSLPROGRAM

        //         #include "Functions/Lit/LitMetaInput.hlsl"

        //         void SimpleGIContribution( MetaVaryings varyings, inout half4 albedo, inout half4 specularColor) {
                    
        //             albedo = half4(0,0,1,1);
        //             specularColor = tex2D(_SpecularMap, varyings.uv);

        //         }
        //         #define CustomGIContribution(varyings, albedo, specularColor) SimpleGIContribution(varyings, albedo, specularColor)

        //         #include "Functions/Lit/LitMetaPass.hlsl"

        //     ENDHLSL
        // }

    }
}
