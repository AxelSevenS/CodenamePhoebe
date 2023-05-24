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
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Passes/Lit/LitInput.hlsl"
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

            float2 NormalUVOffset(float2 position, float2 movement, float noiseSpeed, float noiseScale, float time) {
                return float2(position.x * noiseScale + time * -movement.x/20 * noiseSpeed, position.y * noiseScale + time * -movement.y/20 * noiseSpeed);
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

                    float2 positionX = inputData.positionWS.zy;
                    float2 windX = _WindDirection.zy;
                    float2 positionY = inputData.positionWS.xz;
                    float2 windY = _WindDirection.xz;
                    float2 positionZ = inputData.positionWS.xy;
                    float2 windZ = _WindDirection.xy;

                    float2 noiseUVX1 = NormalUVOffset(positionX, windX, _NormalSpeed * 2, _NormalScale, _Time[1]);
                    float2 noiseUVX2 = NormalUVOffset(positionX, windX, _NormalSpeed * 2 * -0.1, _NormalScale, _Time[1]) * float2(-1, -1);
                    float2 noiseUVY1 = NormalUVOffset(positionY, windY, _NormalSpeed * 2, _NormalScale, _Time[1]);
                    float2 noiseUVY2 = NormalUVOffset(positionY, windY, _NormalSpeed * 2 * -0.1, _NormalScale, _Time[1]) * float2(-1, -1);
                    float2 noiseUVZ1 = NormalUVOffset(positionZ, windZ, _NormalSpeed * 2, _NormalScale, _Time[1]);
                    float2 noiseUVZ2 = NormalUVOffset(positionZ, windZ, _NormalSpeed * 2 * -0.1, _NormalScale, _Time[1]) * float2(-1, -1);

                    float3 triW = abs(inputData.normalWS);
                    triW / (triW.x + triW.y + triW.z);

                    float3 normalX1 = UnpackNormal(tex2D(_NormalMap, noiseUVX1));
                    float3 normalX2 = UnpackNormal(tex2D(_NormalMap, noiseUVX2));
                    float3 normalX = SafeNormalize( (normalX1 + normalX2) / 2 );

                    float3 normalY1 = UnpackNormal(tex2D(_NormalMap, noiseUVY1));
                    float3 normalY2 = UnpackNormal(tex2D(_NormalMap, noiseUVY2));
                    float3 normalY = SafeNormalize( (normalY1 + normalY2) / 2 );

                    float3 normalZ1 = UnpackNormal(tex2D(_NormalMap, noiseUVZ1));
                    float3 normalZ2 = UnpackNormal(tex2D(_NormalMap, noiseUVZ2));
                    float3 normalZ = SafeNormalize( (normalZ1 + normalZ2) / 2 );


                    surfaceData.normalTS = SafeNormalize( (normalX * triW.x + normalY * triW.y + normalZ * triW.z) );
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

                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Passes/Lit/LitSubShader.hlsl"
                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Passes/Lit/LitForwardPass.hlsl"

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

                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Passes/Lit/LitSubShader.hlsl"
                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Passes/Lit/LitGBufferPass.hlsl"

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

                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Passes/Unlit/UnlitSubShader.hlsl"
                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Passes/Unlit/UnlitForwardPass.hlsl"

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

                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Passes/Lit/LitSubShader.hlsl"
                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Passes/Lit/LitDepthOnlyPass.hlsl"

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

                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Passes/Lit/LitSubShader.hlsl"
                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Passes/Lit/LitDepthNormalsPass.hlsl"

            ENDHLSL
        }

        // Pass {
        //     // Don't Edit this.
        //     Name "ShadowCaster"
        //     Tags { "LightMode" = "ShadowCaster" }
        //     Cull Off
        
        //     HLSLPROGRAM
            
        //         #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Passes/Lit/LitSubShader.hlsl"
        //         #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Passes/Lit/LitShadowCasterPass.hlsl"
        
        //     ENDHLSL
        // }

        // Pass {
        //     // Don't Edit this.
        //     Name "Meta"
        //     Tags{"LightMode" = "Meta"}
        //     Cull Off

        //     HLSLPROGRAM

        //         #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Passes/Lit/LitMetaInput.hlsl"

        //         void SimpleGIContribution( MetaVaryings varyings, inout half4 albedo, inout half4 specularColor) {
                    
        //             albedo = half4(0,0,1,1);
        //             specularColor = tex2D(_SpecularMap, varyings.uv);

        //         }
        //         #define CustomGIContribution(varyings, albedo, specularColor) SimpleGIContribution(varyings, albedo, specularColor)

        //         #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Passes/Lit/LitMetaPass.hlsl"

        //     ENDHLSL
        // }

    }
}
