Shader "Hidden/Outline" {
    
    Properties {
        _MainTex("Texture", 2D) = "white" {}
    }
    SubShader {

        Tags { "RenderType"="Opaque" }
        LOD 100

        HLSLINCLUDE

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            CBUFFER_START(UnityPerMaterial)

                sampler2D _MainTex;

            CBUFFER_END
        


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

            Name "RenderOutlines"

            HLSLPROGRAM
                    
                #pragma vertex ForwardPassVertex
                #pragma fragment ForwardPassFragment

                uniform sampler2D _ViewSpaceNormals;

                VertexOutput ForwardPassVertex(VertexInput input) {
                    
                    VertexOutput output = (VertexOutput)0;
                    output.positionCS = TransformWorldToHClip( TransformObjectToWorld(input.positionOS.xyz) );
                    output.normalVS = normalize(mul((float3x3)UNITY_MATRIX_MV, input.normalOS));
                    output.uv = input.uv;

                    return output;
                }



                float4 ForwardPassFragment(VertexOutput input) : SV_Target {
                    
                    // return SAMPLE_TEXTURE2D(_ViewSpaceNormals, sampler_ViewSpaceNormals, input.uv);
                    return tex2D(_ViewSpaceNormals, input.uv);
                }

            ENDHLSL
        }
    }
}
