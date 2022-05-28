Shader "Unlit/Template" {
    Properties {
        // Define the properties in a way Unity can understand
        _MainTex ("Texture", 2D) = "white" {}
        
    }
    SubShader {

        Tags { "RenderType"="Opaque" }
        LOD 100

        HLSLINCLUDE
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            CBUFFER_START(UnityPerMaterial)

            uniform float4 _AmbientLight;
            // Redefine the properties, in a way the shader code can understand, using actual types (Color -> float4; Vector3 -> float3...)

            CBUFFER_END

            // Redefine the texture properties along with a sampler
            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            struct VertexInput {
                float4 position : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct VertexOutput{
                float4 position : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

        ENDHLSL
        
        Pass {
            HLSLPROGRAM
                #pragma vertex vert
                #pragma fragment frag

                VertexOutput vert(VertexInput input) {
                    VertexOutput o;

                    o.position = TransformObjectToHClip(input.position.xyz);
                    o.uv = input.uv;


                    return o;
                }

                float4 frag(VertexOutput input) : SV_Target {
                    float4 baseTex = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv);

                    return baseTex * _AmbientLight;
                }

            ENDHLSL
        }
    }
}
