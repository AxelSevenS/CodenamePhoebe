Shader "Selene/Eidolon" {
    
    Properties {
        [NoScaleOffset]_BaseMap ("Main Texture", 2D) = "white" {}
        [NoScaleOffset]_FlowMap ("Flow Texture", 2D) = "blue" {}
        [NoScaleOffset]_HuskMap ("Husk Texture", 2D) = "black" {}

        _HuskDistance ("Husk Distance", Range(0.0, 0.5)) = 0.05
        _Scale ("Scale", Float) = 1
    }
    SubShader {

        Tags { "RenderType"="Transparent" }
        LOD 100

        HLSLINCLUDE

            #include "Functions/Lit/LitInput.hlsl"
            #include "Packages/com.seven.utility/ShaderLibrary/MathUtility.hlsl"


            CBUFFER_START(UnityPerMaterial)

            sampler2D _BaseMap;
            float4 _BaseMap_ST;

            sampler2D _FlowMap;
            float4 _FlowMap_ST;

            sampler2D _HuskMap;
            float4 _HuskMap_ST;

            float _HuskDistance = 0.05;
            float _Scale = 1;

            CBUFFER_END
            
            void StandardVertexDisplacement( inout VertexOutput output ) {
            }

            void StandardFragment( inout SurfaceData surfaceData, inout InputData inputData, VertexOutput input ) {

                half4 baseColor = FlowMap(_BaseMap, _FlowMap, input.uv, _Time[0] * 2, 0.5);
                clip (baseColor.a <= 0.5 ? -1 : 0);

                    
                surfaceData.albedo = baseColor.rgb;
                surfaceData.alpha = baseColor.a;
                surfaceData.specular = 0;
                surfaceData.metallic = 0;
                surfaceData.smoothness = 0;
                surfaceData.emission = half3(0, 0, 0);
            }
            
            void HuskVertexDisplacement( inout VertexOutput output ) {
                output.positionWS += output.normalWS * _HuskDistance;
            }

            void HuskFragment( inout SurfaceData surfaceData, inout InputData inputData, VertexOutput input ) {

                half4 baseColor = tex2D(_HuskMap, input.uv);
                    
                if (baseColor.a > 0.5) {
                    float waveScale = 5 * _Scale;
                    float waveSpeed = _Time[1] * (_Scale + 1);
                    float wave = saturate( sin( waveScale * input.uv.y + waveSpeed ) ) ;

                    float noise = MovingFractalNoise( input.uv, _Time[1], 5 * _Scale);
                    noise = saturate( noise + 0.5 );

                    float mask = saturate( 1 - (noise * wave) );

                    clip (mask <= 0.5 ? -1 : 0);
                    baseColor *= mask;
                } else {
                    clip(-1);
                }

                surfaceData.albedo = baseColor.rgb;
                surfaceData.alpha = baseColor.a;
                surfaceData.specular = 0;
                surfaceData.metallic = 0;
                surfaceData.smoothness = 0;
                surfaceData.emission = half3(0, 0, 0);
            }


        ENDHLSL
        
        Pass {

            // Don't Edit this.
            Name "StandardForward"
            Tags { "LightMode" = "UniversalForward" }

            HLSLPROGRAM

                #define CustomVertexDisplacement(output) StandardVertexDisplacement(output)
                #define CustomFragment(surfaceData, inputData, input) StandardFragment(surfaceData, inputData, input)

                #include "Functions/Lit/LitSubShader.hlsl"
                #include "Functions/Lit/LitForwardPass.hlsl"

            ENDHLSL
        }

        Pass {

            // Don't Edit this.
            Name "StandardGBuffer"
            Tags { "LightMode" = "UniversalGBuffer" }

            HLSLPROGRAM

                #define CustomVertexDisplacement(output) StandardVertexDisplacement(output)
                #define CustomFragment(surfaceData, inputData, input) StandardFragment(surfaceData, inputData, input)

                #include "Functions/Lit/LitSubShader.hlsl"
                #include "Functions/Lit/LitGBufferPass.hlsl"

            ENDHLSL
        }
        
        Pass {

            // Don't Edit this.
            Name "HuskForward"
            // Tags { "LightMode" = "UniversalForward" }

            HLSLPROGRAM

                #define CustomVertexDisplacement(output) HuskVertexDisplacement(output)
                #define CustomFragment(surfaceData, inputData, input) HuskFragment(surfaceData, inputData, input)

                #include "Functions/Lit/LitSubShader.hlsl"
                #include "Functions/Lit/LitForwardPass.hlsl"

            ENDHLSL
        }

        Pass {
            // Don't Edit this.
            Name "ShadowCaster"
            Tags { "LightMode" = "ShadowCaster" }
        
            HLSLPROGRAM
            
                #define CustomVertexDisplacement(output) StandardVertexDisplacement(output)

                #include "Functions/Lit/LitSubShader.hlsl"
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
                    specularColor = 0.0.xxxx;

                }
                #define CustomGIContribution(varyings, albedo, specularColor) SimpleGIContribution(varyings, albedo, specularColor)

                #include "Functions/Lit/LitMetaPass.hlsl"

            ENDHLSL
        }

    }
}
