Shader "Unlit/Eidolon" {
    Properties {
        // Define the properties in a way Unity can understand
        [NoScaleOffset]_MainTex ("Main Texture", 2D) = "white" {}
        [NoScaleOffset]_FlowTex ("Flow Texture", 2D) = "blue" {}
        [NoScaleOffset]_HuskTex ("Husk Texture", 2D) = "black" {}

        _HuskDistance ("Husk Distance", Range(0.0, 0.5)) = 0.05
        _Scale ("Scale", Float) = 1
        
    }
    SubShader {

        Tags { "RenderType"="Transparent" }
        LOD 100

        HLSLINCLUDE
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Functions/Utility.hlsl"
            #include "Functions/CelLighting.hlsl"

            CBUFFER_START(UnityPerMaterial)

            // // Redefine the properties, in a way the shader code can understand, using actual types (Color -> float4; Vector3 -> float3...)
            float _HuskDistance = 0.05;
            float _Scale = 1;

            // Redefine the texture properties along with a sampler
            sampler2D _MainTex;
            sampler2D _FlowTex;
            sampler2D _HuskTex;

            CBUFFER_END

            struct VertexInput {
                float4 position : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct VertexOutput{
                float4 position : SV_POSITION;
                float3 normal : TEXCOORD1;
                float3 worldPosition : TEXCOORD2;
                float3 worldNormal : TEXCOORD3;
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

                    output.position = TransformObjectToHClip(input.position.xyz);
                    output.normal = input.normal;
                    output.worldPosition = TransformObjectToWorld(input.position.xyz);
                    output.worldNormal = normalize(TransformObjectToWorldNormal(input.normal.xyz));
                    output.uv = input.uv;


                    return output;
                }

                // VertexOutput frag(VertexOutput input) : SV_Target {
                //     float4 baseTex = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv);

                //     return baseTex * _AmbientLight;
                // }

                float4 frag(VertexOutput input) : SV_Target {
                    half4 baseColor = FlowMap(_MainTex, _FlowTex, input.uv, _Time[0] * 2, 0.5);

                    clip (baseColor.a <= 0.5 ? -1 : 0);

                    return SimpleCelLighting(baseColor, input.worldPosition.xyz, input.worldNormal);
                    // return baseColor;
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

                    half3 normal = input.normal;
                    float3 position = input.position.xyz + input.normal * _HuskDistance;

                    output.position = TransformObjectToHClip(position);
                    output.normal = input.normal;
                    output.worldPosition = TransformObjectToWorld(input.position.xyz);
                    output.worldNormal = normalize(TransformObjectToWorldNormal(input.normal.xyz));
                    output.uv = input.uv;


                    return output;
                }

                float4 frag(VertexOutput input) : SV_Target {
                    half4 baseColor = tex2D(_HuskTex, input.uv);

                    half4 shadedColor = SimpleCelLighting(baseColor, input.worldPosition.xyz, input.worldNormal);

                    if (shadedColor.a > 0.5) {
                        // float waveScale = 5 * _Scale;
                        // float waveSpeed = _Time[1] * (_Scale + 1) /* * clamp( waveDirection, -1, 1) */;
                        // float wave = saturate( sin( waveScale * input.uv.y + waveSpeed ) ) ;
                        float wave = MovingFractalNoise( input.uv, -_Time[1] * 0.25, 0.5 * _Scale);
                        wave = saturate( wave + 0.5 );

                        float noise = MovingFractalNoise( input.uv, _Time[1], 5 * _Scale);
                        noise = saturate( noise + 0.5 );

                        float mask = saturate( 1 - (noise * wave) );

                        clip (mask <= 0.5 ? -1 : 0);
                        return shadedColor * mask;
                    } else {
                        clip(-1);
                        return 0;
                    }
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
