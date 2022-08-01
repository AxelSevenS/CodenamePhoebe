Shader "Selene/Template" {
    Properties {
        // Define the properties in a way Unity can understand
        [NoScaleOffset]_MainTex ("Main Texture", 2D) = "white" {}
        
    }
    SubShader {

        Tags { "RenderType"="Transparent" }
        LOD 100

        HLSLINCLUDE
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Functions/CelLighting.hlsl"

            CBUFFER_START(UnityPerMaterial)

            // // Redefine the properties, in a way the shader code can understand, using actual types (Color -> float4; Vector3 -> float3...)

            // Redefine the texture properties along with a sampler
            sampler2D _MainTex;

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

                    CelLightingInput lightingInput = GetCelLightingInput(input.position, input.normal);

                    return SimpleCelLighting(baseColor, lightingInput);
                }

            ENDHLSL
        }
        
        
        Pass {

            Name "Hull"
            Cull Off
            
            HLSLPROGRAM
                #pragma vertex vert
                #pragma fragment frag

                VertexOutput vert(VertexInput input) {
                    VertexOutput output;
                    
                    output.position = input.position + half4(input.normal, 0);
                    output.positionCS = TransformObjectToHClip(output.position);
                    output.normal = normalize(input.normal);
                    output.uv = input.uv;


                    return output;
                }

                float4 frag(VertexOutput input) : SV_Target {
                    half4 baseColor = tex2D(_MainTex, input.uv);

                    CelLightingInput lightingInput = GetCelLightingInput(input.position, input.normal);
                    return SimpleCelLighting(float4(0,1,0,0.5), lightingInput);
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
