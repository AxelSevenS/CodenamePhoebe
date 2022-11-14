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
            #include "Assets/3-Rendering/Shaders/Functions/CelLighting.hlsl"

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



                // float2 Panner(float2 uv, float2 direction, float speed) {
                //     return uv + normalize(direction)*speed*_Time.y;
                // }

                // float3 MotionFourWaySparkle(sampler2D tex, float2 uv, float4 coordinateScale, float speed) {
                //     float2 uv1 = Panner(uv * coordinateScale.x, float2( 0.1,  0.1), speed);
                //     float2 uv2 = Panner(uv * coordinateScale.y, float2(-0.1, -0.1), speed);
                //     float2 uv3 = Panner(uv * coordinateScale.z, float2(-0.1,  0.1), speed);
                //     float2 uv4 = Panner(uv * coordinateScale.w, float2( 0.1, -0.1), speed);

                //     float3 sample1 = UnpackNormal(tex2D(tex, uv1)).rgb;
                //     float3 sample2 = UnpackNormal(tex2D(tex, uv2)).rgb;
                //     float3 sample3 = UnpackNormal(tex2D(tex, uv3)).rgb;
                //     float3 sample4 = UnpackNormal(tex2D(tex, uv4)).rgb;

                //     float3 normalA = float3(sample1.x, sample2.y, 1);
                //     float3 normalB = float3(sample3.x, sample4.y, 1);
                    
                //     return normalize(float3( (normalA+normalB).xy, (normalA*normalB).z ));
                // }

                // // Tiles a texture in an interesting wave-like way.
                // // Inspired by the function of the same name in Unreal Engine.
                // float3 MotionFourWayChaos(sampler2D tex, float2 uv, float speed, bool unpackNormal)
                // {
                //     float2 uv1 = Panner(uv + float2(0.000, 0.000), float2( 0.1,  0.1), speed);
                //     float2 uv2 = Panner(uv + float2(0.418, 0.355), float2(-0.1, -0.1), speed);
                //     float2 uv3 = Panner(uv + float2(0.865, 0.148), float2(-0.1,  0.1), speed);
                //     float2 uv4 = Panner(uv + float2(0.651, 0.752), float2( 0.1, -0.1), speed);

                //     float3 sample1;
                //     float3 sample2;
                //     float3 sample3;
                //     float3 sample4;

                //     if (unpackNormal)
                //     {
                //         sample1 = UnpackNormal(tex2D(tex, uv1)).rgb;
                //         sample2 = UnpackNormal(tex2D(tex, uv2)).rgb;
                //         sample3 = UnpackNormal(tex2D(tex, uv3)).rgb;
                //         sample4 = UnpackNormal(tex2D(tex, uv4)).rgb;

                //         return normalize(sample1 + sample2 + sample3 + sample4);
                //     }
                //     else
                //     {
                //         sample1 = tex2D(tex, uv1).rgb;
                //         sample2 = tex2D(tex, uv2).rgb;
                //         sample3 = tex2D(tex, uv3).rgb;
                //         sample4 = tex2D(tex, uv4).rgb;

                //         return (sample1 + sample2 + sample3 + sample4) / 4.0;
                //     }
                // }



                float4 frag(VertexOutput input) : SV_Target {
                    half4 baseColor = tex2D(_MainTex, input.uv);

                    CelLightingInput CelLightingInput = GetCelLightingInput(input.lightingInput, _SpecularIntensity, _Smoothness, _AccentIntensity);
                    
                    if ( _ProximityDither == 1 )
                        if ( ProximityDither(CelLightingInput.worldPosition, CelLightingInput.screenPosition) )
                            return half4(0, 0, 0, 0);

                    if (_NormalIntensity > 0) {
                        float3 normalWS = ComputeTangentToWorldNormal(input.normal, input.tangent, input.bitangent, input.uv, _NormalMap);
                        CelLightingInput.worldNormal = lerp(CelLightingInput.worldNormal, normalWS, _NormalIntensity);
                    }

                    float4 finalColor = CelLighting(baseColor, CelLightingInput);




                    // float3 _SparkleColor = float3(0, 1, 0);
                    // float _SparkleScale = 0.1;
                    // float _SparkleSpeed = 0;
                    // float _SparkleExponent = 0.1;

                    // if (_EmissionIntensity != 1)
                    //     finalColor *= _EmissionIntensity;

                    // // Get some random sparkly normals.
                    // float3 sparkly1 = MotionFourWaySparkle(_NormalMap, input.lightingInput.worldPosition.xz / _SparkleScale, float4(1,2,3,4), _SparkleSpeed);
                    // float3 sparkly2 = MotionFourWaySparkle(_NormalMap, input.lightingInput.worldPosition.xz / _SparkleScale, float4(1,0.5,2.5,2), _SparkleSpeed);
                    
                    // // Dot them to make a sparkly mask.
                    // float sparkleMask = dot(sparkly1, sparkly2) * saturate(3.0 * sqrt(saturate(dot(sparkly1.x, sparkly2.x))));
                    // sparkleMask = ceil(saturate(pow(sparkleMask, _SparkleExponent))) /* * shadowMask * distanceMask */;

                    // // Get the sparkle specular color to add later on.
                    // float3 sparkleColor = lerp(0, _SparkleColor, 1-sparkleMask);

                    // finalColor += float4(sparkleColor, 0);



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
