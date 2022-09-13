Shader "Selene/Lit" {
    
    Properties {
        // Define the properties in a way Unity can understand
        [NoScaleOffset] _MainTex ("Main Texture", 2D) = "white" {}

        [NoScaleOffset] _NormalMap ("Normal Map", 2D) = "bump" {}
        _NormalIntensity ("Normal Intensity", Range(0,1)) = 0

        [MaterialToggle] _ProximityDither ("Proximity Dither", Float) = 0

        _SpecularIntensity ("Specular Intensity", Range(0,3)) = 0
        _Smoothness ("Smoothness", Range(0,1)) = 0
        
        _AccentIntensity ("Accent Intensity", Range(0, 5)) = 0
        // _AccentBrightness ("Accent Brightness", Range(0, 1)) = 1

        _EmissionIntensity ("Emission Intensity", Float) = 1

        [IntRange] _StencilID ("Stencil ID", Range(0, 255)) = 0
        [Enum(UnityEngine.Rendering.CompareFunction)] _StencilComp ("Stencil Comparison", Int) = 0
    }
    SubShader {

        Tags { "RenderType"="Transparent" }
        LOD 100

        HLSLINCLUDE

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Functions/CelLighting.hlsl"

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
                    output.positionCS = TransformObjectToHClip(output.position);
                    output.normal = normalize(input.normal);
                    output.tangent = normalize(input.tangent);
                    output.bitangent = cross(input.tangent, input.normal);
                    output.uv = input.uv;


                    return output;
                }

                float4 frag(VertexOutput input) : SV_Target {
                    half4 baseColor = tex2D(_MainTex, input.uv);

                    CelLightingInput lightingInput = GetCelLightingInput(input.position, input.normal, _SpecularIntensity, _Smoothness, _AccentIntensity);
                    
                    if ( _ProximityDither == 1 )
                        if ( ProximityDither(lightingInput.worldPosition, lightingInput.screenPosition) )
                            return half4(0, 0, 0, 0);

                    if (_NormalIntensity > 0) {
                        float3 normal = UnpackNormal( tex2D(_NormalMap, input.uv) );
                        normal = ComputeNormals(
                            TransformObjectToWorldNormal(input.normal),
                            TransformObjectToWorldNormal(input.tangent),
                            TransformObjectToWorldNormal(input.bitangent),
                            normal
                        );
                        lightingInput.worldNormal = lerp(lightingInput.worldNormal, normal, _NormalIntensity);
                    }

                    float4 finalColor = CelLighting(baseColor, lightingInput);

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
