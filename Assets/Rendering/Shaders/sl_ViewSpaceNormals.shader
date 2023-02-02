Shader "Hidden/ViewSpaceNormals" {
    
    Properties {
    }
    SubShader {

        Tags { "RenderType"="Opaque" }
        LOD 100

        HLSLINCLUDE

            struct VertexInput {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct VertexOutput{
                float4 positionCS : SV_POSITION;
                float3 normalVS : TEXCOORD1;
                float2 uv : TEXCOORD0;
            };

        ENDHLSL
        
        Pass {

            Name "StandardSurface"
            Tags { "LightMode"="UniversalForward" }

            HLSLPROGRAM

                // #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN
                // #pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
                // #pragma multi_compile_fragment _ _ADDITIONAL_LIGHT_SHADOWS
                // #pragma multi_compile_fragment _ _SHADOWS_SOFT
                // #pragma multi_compile_fragment _ _SCREEN_SPACE_OCCLUSION
                // #pragma multi_compile_fragment _ _DBUFFER_MRT1 _DBUFFER_MRT2 _DBUFFER_MRT3
                // #pragma multi_compile_fragment _ _REFLECTION_PROBE_BLENDING
                // #pragma multi_compile_fragment _ _REFLECTION_PROBE_BOX_PROJECTION
                // #pragma multi_compile_fragment _ _LIGHT_LAYERS
                // #pragma multi_compile_fragment _ _LIGHT_COOKIES
                // #pragma multi_compile _ _CLUSTERED_RENDERING
                    
                #pragma vertex ForwardPassVertex
                #pragma fragment ForwardPassFragment

                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

                VertexOutput ForwardPassVertex(VertexInput input) {
                    
                    VertexOutput output = (VertexOutput)0;
                    output.positionCS = TransformWorldToHClip( TransformObjectToWorld(input.positionOS.xyz) );
                    output.normalVS = normalize(mul((float3x3)UNITY_MATRIX_MV, input.normalOS));
                    output.uv = input.uv;

                    return output;
                }



                float4 ForwardPassFragment(VertexOutput input) : SV_Target {
                    
                    return float4(((input.normalVS.xyz + 1) / 2), 1);
                }

            ENDHLSL
        }
    }
}
