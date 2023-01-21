Shader "Selene/Lit" {
    
    Properties {
        [NoScaleOffset] _MainTex ("Main Texture", 2D) = "white" {}

        [NoScaleOffset] _NormalMap ("Normal Map", 2D) = "bump" {}
        _NormalIntensity ("Normal Intensity", Range(0,1)) = 0

        [MaterialToggle] _ProximityDither ("Proximity Dither", Float) = 0

        _SpecularIntensity ("Specular Intensity", Range(0,3)) = 0
        _Smoothness ("Smoothness", Range(0,1)) = 0
        
        _AccentIntensity ("Accent Intensity", Range(0, 1)) = 0

        _EmissionIntensity ("Emission Intensity", Float) = 1

        [IntRange] _StencilID ("Stencil ID", Range(0, 255)) = 0
        [Enum(UnityEngine.Rendering.CompareFunction)] _StencilComp ("Stencil Comparison", Int) = 0
    }
    SubShader {

        Tags { "RenderType"="Opaque" }
        LOD 100

        HLSLINCLUDE

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.seven.utility/ShaderLibrary/MathUtility.hlsl"
            #include "Assets/Rendering/Shaders/Functions/CelLighting.hlsl"

            CBUFFER_START(UnityPerMaterial)

                sampler2D _MainTex;

                sampler2D _NormalMap;
                float _NormalIntensity;

                float _ProximityDither;

                float _SpecularIntensity;
                float _Smoothness;

                float _AccentIntensity;
                // float _AccentBrightness;

                float _EmissionIntensity;

            CBUFFER_END

            struct VertexInput {
                float4 position : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float2 uv : TEXCOORD0;
            };

            struct VertexOutput{
                float4 positionCS : SV_POSITION;
                float4 position : TEXCOORD1;
                float3 normal : TEXCOORD2;
                float3 tangent : TEXCOORD3;
                float3 bitangent : TEXCOORD4;
                float2 uv : TEXCOORD0;
                VertLightingInput lightingInput : TEXCOORD5;
            };

        ENDHLSL
        
        Pass {


            Stencil {
                Ref [_StencilID]
                Comp [_StencilComp]
            }

            Name "StandardSurface"
            Tags { "LightMode"="UniversalForward" }

            HLSLPROGRAM
                #pragma vertex vert
                #pragma fragment frag

                VertexOutput vert(VertexInput input) {
                    VertexOutput output;

                    output.position = input.position;
                    output.positionCS = TransformObjectToHClip(output.position.xyz);
                    output.normal = normalize(input.normal);
                    output.tangent = normalize(input.tangent.xyz);
                    output.bitangent = cross(output.tangent, output.normal);
                    output.uv = input.uv;
                    output.lightingInput = GetVertLightingInput(output.position.xyz, output.normal);


                    return output;
                }



                float4 frag(VertexOutput input) : SV_Target {
                    half4 baseColor = tex2D(_MainTex, input.uv);

                    CelLightingInput CelLightingInput = GetCelFragLightingInput(input.lightingInput, _SpecularIntensity, _Smoothness, _AccentIntensity);
                    
                    if ( _ProximityDither == 1 )
                        if ( ProximityDither(CelLightingInput.worldPosition, CelLightingInput.screenPosition) )
                            return half4(0, 0, 0, 0);

                    if (_NormalIntensity > 0) {
                        float3 normalWS = ComputeTangentToWorldNormal(input.normal, input.tangent, input.bitangent, input.uv, _NormalMap);
                        CelLightingInput.worldNormal = lerp(CelLightingInput.worldNormal, normalWS, _NormalIntensity);
                    }

                    float4 finalColor = CelLighting(baseColor, CelLightingInput);


                    if (_EmissionIntensity != 1)
                        finalColor *= _EmissionIntensity;


                    return finalColor; 
                }

            ENDHLSL
        }
        

        Pass {

            Name "ShadowCaster"
            Tags { "LightMode"="ShadowCaster" }
        
            HLSLPROGRAM
            
            // #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/CommonMaterial.hlsl"
            // #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/SurfaceInput.hlsl"
            #include "Passes/ShadowCasterPass.hlsl"
        
            ENDHLSL
        }

    }
}
