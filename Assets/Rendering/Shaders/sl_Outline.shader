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
                float4 _OutlineColor;
                float _OutlineWidth;
                float _NoiseScale;
                float _NoiseIntensity;

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

                float bw(sampler2D col, float2 uv) {
                    float3 third = float3(1/3, 1/3, 1/3);
                    return dot(tex2D(col, uv).xyz, third);
                }

                float2 nUv(float2 uv) {
                    return uv/_ScreenParams.xy;
                }

                float sobelHorizontal(float2 centerUV) {
                    float k1 = bw(_ViewSpaceNormals, nUv(centerUV-1))*-1;
                    float k2 = bw(_ViewSpaceNormals, nUv(centerUV-float2(1, -1)))*-1;
                    float k3 = bw(_ViewSpaceNormals, nUv(centerUV-float2(1, 0)))*-2;
                    float k4 = bw(_ViewSpaceNormals, nUv(centerUV+1));
                    float k5 = bw(_ViewSpaceNormals, nUv(centerUV+float2(1, -1)));
                    float k6 = bw(_ViewSpaceNormals, nUv(centerUV+float2(1, 0)))*2;
                    return((k1+k2+k3+k4+k5+k6)/6);
                }
                float sobelVertical(float2 centerUV) {
                    float k1 = bw(_ViewSpaceNormals, nUv(centerUV-1))*-1;
                    float k2 = bw(_ViewSpaceNormals, nUv(centerUV-float2(-1, 1)))*-1;
                    float k3 = bw(_ViewSpaceNormals, nUv(centerUV-float2(0, 1)))*-2;
                    float k4 = bw(_ViewSpaceNormals, nUv(centerUV+1));
                    float k5 = bw(_ViewSpaceNormals, nUv(centerUV+float2(-1, 1)));
                    float k6 = bw(_ViewSpaceNormals, nUv(centerUV+float2(0, 1)))*2;
                    return((k1+k2+k3+k4+k5+k6)/6);
                }

                VertexOutput ForwardPassVertex(VertexInput input) {
                    
                    VertexOutput output = (VertexOutput)0;
                    output.positionCS = TransformWorldToHClip( TransformObjectToWorld(input.positionOS.xyz) );
                    output.normalVS = normalize(mul((float3x3)UNITY_MATRIX_MV, input.normalOS));
                    output.uv = input.uv;

                    return output;
                }



                float4 ForwardPassFragment(VertexOutput input) : SV_Target {
                    
                    float3 offset = float3((1.0 / _ScreenParams.x), (1.0 / _ScreenParams.y), 0.0) * _OutlineWidth;

                    float sobel = sobelHorizontal(input.uv) + sobelVertical(input.uv);

                    
                    
                    return float4(sobel, sobel, sobel, 1);
                    return tex2D(_ViewSpaceNormals, input.uv);
                }

            ENDHLSL
        }
    }
}
