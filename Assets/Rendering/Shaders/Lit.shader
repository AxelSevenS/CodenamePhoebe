Shader "Selene/Lit" {
    Properties {
        // Define the properties in a way Unity can understand
        [NoScaleOffset]_MainTex ("Main Texture", 2D) = "white" {}
        [MaterialToggle] _ProximityDither("Proximity Dither", Float) = 0
        
    }
    SubShader {

        Tags { "RenderType"="Opaque" }
        LOD 100

        HLSLINCLUDE

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Functions/CelLighting.hlsl"

            CBUFFER_START(UnityPerMaterial)

            // Redefine the properties, in a way the shader code can understand, using actual types (Color -> float4; Vector3 -> float3...)
            sampler2D _MainTex;
            float _ProximityDither;

            CBUFFER_END

            struct VertexInput {
                float4 position : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct VertexOutput{
                float4 positionCS : SV_POSITION;
                float3 normal : TEXCOORD2;
                float4 position : TEXCOORD3;
                float2 uv : TEXCOORD0;
            };

        ENDHLSL
        
        Pass {

            Name "StandardSurface"
            Tags { "LightMode"="UniversalForward" }

            HLSLPROGRAM
                #pragma vertex vert
                #pragma fragment frag

                VertexOutput vert(VertexInput input) {
                    VertexOutput output;

                    output.position = input.position;
                    output.positionCS = TransformObjectToHClip(output.position);
                    output.normal = normalize(input.normal);
                    output.uv = input.uv;


                    return output;
                }

                float4 frag(VertexOutput input) : SV_Target {
                    half4 baseColor = tex2D(_MainTex, input.uv);

                    LightingInput lightingInput = GetLightingInput(input.position, input.normal); 
                    
                    if (_ProximityDither == 1) {

                        float proximityAlphaMultiplier = (distance(_WorldSpaceCameraPos, lightingInput.worldPosition) - 0.25) * 3;

                        float ditherMask = Dither(lightingInput.screenPosition.xy/lightingInput.screenPosition.w);
                        clip (ditherMask <= 1 - proximityAlphaMultiplier ? -1 : 0);
                    }


                    return CelLighting(baseColor, lightingInput, 1, 0.05, half3(1, 1, 1)); 
                }

            ENDHLSL
        }
        
        Pass {

            Name "ShadowCaster"
            Tags { "LightMode"="ShadowCaster" }
        
            ZWrite On
            ZTest LEqual
        
            HLSLPROGRAM
            // Required to compile gles 2.0 with standard srp library
            #pragma prefer_hlslcc gles
            #pragma exclude_renderers d3d11_9x gles
            //#pragma target 4.5
        
            // Material Keywords
            #pragma shader_feature _ALPHATEST_ON
            #pragma shader_feature _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A
        
            // GPU Instancing
            #pragma multi_compile_instancing
            #pragma multi_compile _ DOTS_INSTANCING_ON
                    
            #pragma vertex ShadowPassVertex
            #pragma fragment ShadowPassFragment
            
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/CommonMaterial.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/SurfaceInput.hlsl"
            #include "Passes/ShadowCasterPass.hlsl"
        
            ENDHLSL
        }

    }
}
