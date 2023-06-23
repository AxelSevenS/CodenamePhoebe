Shader "Selene/Underwater" {
    
    Properties {
        [HideInInspector] _MainTex("Texture", 2D) = "white" {}
        _FogStart("Fog Start", Float) = 0.0
        _FogEnd("Fog End", Float) = 70.0
    }
    SubShader {

        Tags { "RenderType"="Opaque" }
        LOD 100

        HLSLINCLUDE

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/SurfaceInput.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareNormalsTexture.hlsl"

            CBUFFER_START(UnityPerMaterial)

                sampler2D _MainTex;
                
                float _FogStart;
                float _FogEnd;

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

            Name "RenderUnderwaterEffect"

            HLSLPROGRAM
                    
                #pragma vertex ForwardPassVertex
                #pragma fragment ForwardPassFragment
                
                uniform sampler2D _UnderwaterMask;


                VertexOutput ForwardPassVertex(VertexInput input) {
                    
                    VertexOutput output = (VertexOutput)0;
                    output.positionCS = TransformWorldToHClip( TransformObjectToWorld(input.positionOS.xyz) );
                    output.normalVS = normalize(mul((float3x3)UNITY_MATRIX_MV, input.normalOS));
                    output.uv = input.uv;

                    return output;
                }

                float4 ForwardPassFragment(VertexOutput input) : SV_Target {

                    float4 sceneColor = tex2D(_MainTex, input.uv).rgba;
                    float4 underwaterBuffer = tex2D(_UnderwaterMask, input.uv);
                    float3 underwaterColor = underwaterBuffer.rgb;
                    float4 underwaterMask = underwaterBuffer.a;

                    float depth = Linear01Depth(SampleSceneDepth(input.uv).r, _ZBufferParams) * _ProjectionParams.z;
                    
                    // fog effect for underwater

                    float fogFactor = underwaterMask > 0 ? saturate((depth - _FogStart) / (_FogEnd - _FogStart)) * underwaterMask : 0;

                    float3 finalColor = lerp(sceneColor, underwaterColor, fogFactor);


                    return float4(finalColor.r, finalColor.g, finalColor.b, 1);
                }

            ENDHLSL
        }
    }
}
