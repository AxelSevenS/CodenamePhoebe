Shader "Selene/Eidolon" {
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
            #include "Assets/Rendering/Shaders/Functions/Utility.hlsl"
            #include "Assets/Rendering/Shaders/Functions/CelLighting.hlsl"

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
                float4 positionCS : SV_POSITION;
                float4 position : TEXCOORD1;
                float3 normal : TEXCOORD2;
                float2 uv : TEXCOORD0;
                VertLightingInput lightingInput : TEXCOORD3;
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
                    output.positionCS = TransformObjectToHClip(input.position.xyz);
                    output.normal = normalize(input.normal);
                    output.uv = input.uv;
                    output.lightingInput = GetVertLightingInput(output.position.xyz, output.normal);


                    return output;
                }

                float4 frag(VertexOutput input) : SV_Target {
                    half4 baseColor = FlowMap(_MainTex, _FlowTex, input.uv, _Time[0] * 2, 0.5);

                    clip (baseColor.a <= 0.5 ? -1 : 0);

                    CelLightingInput lightingInput = GetCelFragLightingInput(input.lightingInput);
                    return SimpleCelLighting(baseColor, lightingInput);
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

                    output.position = input.position + half4(input.normal, 0) * _HuskDistance;
                    output.positionCS = TransformObjectToHClip(output.position);
                    output.normal = normalize(input.normal);
                    output.uv = input.uv;
                    output.lightingInput = GetVertLightingInput(output.position.xyz, output.normal);


                    return output;
                }

                float4 frag(VertexOutput input) : SV_Target {
                    half4 baseColor = tex2D(_HuskTex, input.uv);

                    CelLightingInput lightingInput = GetCelFragLightingInput(input.lightingInput); 

                    half4 shadedColor = SimpleCelLighting(baseColor, lightingInput);

                    if (shadedColor.a > 0.5) {
                        float waveScale = 5 * _Scale;
                        float waveSpeed = _Time[1] * (_Scale + 1) /* * clamp( waveDirection, -1, 1) */;
                        float wave = saturate( sin( waveScale * input.uv.y + waveSpeed ) ) ;
                        // float wave = MovingFractalNoise( input.uv, -_Time[1] * 0.25, 0.5 * _Scale);
                        // wave = saturate( wave + 0.5 );

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
