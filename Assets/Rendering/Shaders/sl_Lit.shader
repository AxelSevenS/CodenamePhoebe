Shader "Selene/Lit" {
    
    Properties {
        [NoScaleOffset] _BaseMap ("Main Texture", 2D) = "white" {}

        [NoScaleOffset] _NormalMap ("Normal Map", 2D) = "bump" {}
        _NormalIntensity ("Normal Intensity", Range(0,1)) = 0

        [NoScaleOffset] _SpecularMap ("Specular Map", 2D) = "white" {}
        _Smoothness ("Smoothness", Range(0,1)) = 0


        [MaterialToggle] _ProximityDither ("Proximity Dither", Float) = 0

        _EmissionIntensity ("Emission Intensity", Float) = 1

        [IntRange] _StencilID ("Stencil ID", Range(0, 255)) = 0
        [Enum(UnityEngine.Rendering.CompareFunction)] _StencilComp ("Stencil Comparison", Int) = 0
    }
    SubShader {

        Tags { "RenderType"="Opaque" }
        LOD 100

        HLSLINCLUDE

            #include "Functions/Lit/LitInput.hlsl"
            #include "Packages/com.seven.utility/ShaderLibrary/MathUtility.hlsl"


            CBUFFER_START(UnityPerMaterial)

                sampler2D _BaseMap;
                float4 _BaseMap_ST;

                sampler2D _NormalMap;
                float _NormalIntensity;

                sampler2D _SpecularMap;
                float _Smoothness;

                float _ProximityDither;

                float _EmissionIntensity;

            CBUFFER_END


            // Define the custom Vertex and Fragment functions.
            
            void SimpleVertexDisplacement( inout VertexOutput output ) {
                // output.positionWS += output.normalWS * 0.1;
            }

            void SimpleFragment( inout SurfaceData surfaceData, inout InputData inputData, VertexOutput input ) {

                if ( _ProximityDither == 1 )
                    ProximityDither(input.positionWS, input.positionSS);

                half4 baseColor = tex2D(_BaseMap, input.uv);
                surfaceData.albedo = baseColor.rgb;
                surfaceData.alpha = baseColor.a;
                surfaceData.specular = tex2D(_SpecularMap, input.uv);
                surfaceData.metallic = 0;
                surfaceData.smoothness = _Smoothness;
                surfaceData.emission = half3(0, 0, 0);

                if ( _NormalIntensity != 0) {
                    surfaceData.normalTS = UnpackNormal(tex2D(_NormalMap, input.uv));
                    half3 mapNormal = mul( inputData.tangentToWorld, surfaceData.normalTS);
                    inputData.normalWS = lerp(inputData.normalWS, mapNormal, _NormalIntensity);
                }
            }

            #define CustomVertexDisplacement(output) SimpleVertexDisplacement(output)
            #define CustomFragment(surfaceData, inputData, input) SimpleFragment(surfaceData, inputData, input)

            #include "Functions/Lit/LitSubShader.hlsl"

        ENDHLSL
        
        Pass {
            Stencil {
                Ref [_StencilID]
                Comp [_StencilComp]
            }

            // Don't Edit this.
            Name "StandardSurface"
            Tags { "LightMode" = "UniversalForward" }

            HLSLPROGRAM

                #include "Functions/Lit/LitForwardPass.hlsl"

            ENDHLSL
        }
        
        Pass {
            Stencil {
                Ref [_StencilID]
                Comp [_StencilComp]
            }

            // Don't Edit this.
            Name "StandardSurface"
            Tags { "LightMode" = "UniversalGBuffer" }

            HLSLPROGRAM

                #include "Functions/Lit/LitGBufferPass.hlsl"

            ENDHLSL
        }

        Pass {
            // Don't Edit this.
            Name "ShadowCaster"
            Tags { "LightMode" = "ShadowCaster" }
        
            HLSLPROGRAM
            
                #include "Functions/Lit/LitShadowCasterPass.hlsl"
        
            ENDHLSL
        }

        Pass {
            // Don't Edit this.
            Name "Meta"
            Tags{"LightMode" = "Meta"}

            Cull Off

            HLSLPROGRAM

                #include "Functions/Lit/LitMetaInput.hlsl"

                void SimpleGIContribution( MetaVaryings varyings, inout half4 albedo, inout half4 specularColor) {
                    
                    albedo = tex2D(_BaseMap, varyings.uv);
                    specularColor = tex2D(_SpecularMap, varyings.uv);

                }
                #define CustomGIContribution(varyings, albedo, specularColor) SimpleGIContribution(varyings, albedo, specularColor)

                #include "Functions/Lit/LitMetaPass.hlsl"

            ENDHLSL
        }

    }
}
