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

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/GlobalIllumination.hlsl"

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

            struct VertexInput {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float4 tangentOS : TANGENT;
                float2 uv : TEXCOORD0;
                float2 staticLightmapUV : TEXCOORD1;
                float2 dynamicLightmapUV : TEXCOORD2;
            };

            struct VertexOutput{
                float4 positionCS : SV_POSITION;
                float3 positionWS : TEXCOORD1;
                float3 normalWS : TEXCOORD2;
                float3 tangentWS : TEXCOORD3;
                float3 bitangentWS : TEXCOORD4;
                float4 positionSS : TEXCOORD5;
                float3 viewDirectionWS : TEXCOORD6;
                float2 uv : TEXCOORD0;

                DECLARE_LIGHTMAP_OR_SH(staticLightmapUV, vertexSH, 8);
            #ifdef DYNAMICLIGHTMAP_ON
                float2 dynamicLightmapUV : TEXCOORD9; // Dynamic lightmap UVs
            #endif
            };


            float Dither(float2 ScreenPosition) {
                float2 uv = ScreenPosition * _ScreenParams.xy;
                float DITHER_THRESHOLDS[16] = {
                    1.0 / 17.0,  9.0 / 17.0,  3.0 / 17.0, 11.0 / 17.0,
                    13.0 / 17.0,  5.0 / 17.0, 15.0 / 17.0,  7.0 / 17.0,
                    4.0 / 17.0, 12.0 / 17.0,  2.0 / 17.0, 10.0 / 17.0,
                    16.0 / 17.0,  8.0 / 17.0, 14.0 / 17.0,  6.0 / 17.0
                };
                uint index = (uint(uv.x) % 4) * 4 + uint(uv.y) % 4;
                return 1 - DITHER_THRESHOLDS[index];
            }

            bool ProximityDither(float3 worldPosition, float4 screenPosition) {
                float proximityAlphaMultiplier = (distance(_WorldSpaceCameraPos, worldPosition) - 0.25) * 3;

                float ditherMask = Dither( screenPosition.xy / screenPosition.w );
                bool shouldClip = ditherMask <= 1 - proximityAlphaMultiplier;
                clip ( shouldClip ? -1 : 0 );
                return shouldClip;
            }
            

            void CustomVertexDisplacement( inout float3 positionWS, inout float3 normalWS ) {
                // positionWS += normalWS * 0.1;
            }

            void CustomFragment( inout SurfaceData surfaceData, inout InputData inputData, VertexOutput input ) {
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

            void InitializeVertexOutput( inout VertexOutput output, VertexInput input ) {

                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_TRANSFER_INSTANCE_ID(input, output);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

                output.positionWS = TransformObjectToWorld(input.positionOS.xyz);
                output.normalWS = TransformObjectToWorldNormal(input.normalOS.xyz);


                CustomVertexDisplacement( output.positionWS, output.normalWS );


                output.positionCS = TransformWorldToHClip( output.positionWS );
                #ifdef SHADERGRAPH_PREVIEW
                    output.positionSS = float4(0,0,0,0);
                #else
                    output.positionSS = ComputeScreenPos( output.positionCS );
                #endif
                
                output.tangentWS = TransformObjectToWorldNormal(input.tangentOS.xyz);
                output.bitangentWS = input.tangentOS.w * cross(output.normalWS.xyz, output.tangentWS.xyz);

                output.viewDirectionWS = normalize( _WorldSpaceCameraPos.xyz - output.positionWS );

                
            #if (SHADERPASS == SHADERPASS_FORWARD) || (SHADERPASS == SHADERPASS_GBUFFER)
                OUTPUT_LIGHTMAP_UV(input.staticLightmapUV, unity_LightmapST, output.staticLightmapUV);
                #if defined(DYNAMICLIGHTMAP_ON)
                    output.dynamicLightmapUV.xy = input.dynamicLightmapUV.xy * unity_DynamicLightmapST.xy + unity_DynamicLightmapST.zw;
                #endif
                // OUTPUT_SH(normalWS, output.vertexSH);
            #endif

                output.uv = input.uv;
            }

            void InitiliazeEmptySurfaceData( inout SurfaceData surfaceData ) {
                surfaceData.normalTS = float3(0, 1, 0);
                surfaceData.occlusion = 1;
                surfaceData.clearCoatMask = 0;
                surfaceData.clearCoatSmoothness = 0;
                surfaceData.albedo = 1.0.xxx;
                surfaceData.alpha = 1;
                surfaceData.specular = 1.0.xxx;
                surfaceData.metallic = 0;
                surfaceData.smoothness = 0.5;
                surfaceData.emission = 0.0.xxx;
            }

            void InitializeLightingData( inout SurfaceData surfaceData, inout InputData inputData, VertexOutput input ) {

                InitiliazeEmptySurfaceData( surfaceData );

                inputData.positionWS = input.positionWS;
                inputData.positionCS = input.positionCS;
                inputData.normalWS = input.normalWS;
                inputData.viewDirectionWS = input.viewDirectionWS;
            #ifdef SHADERGRAPH_PREVIEW
                inputData.shadowCoord = float4(0,0,0,0);
            #else
                #if SHADOWS_SCREEN
                    inputData.shadowCoord = input.positionSS;
                #else 
                    inputData.shadowCoord = TransformWorldToShadowCoord(input.positionWS);
                #endif
            #endif
                inputData.vertexLighting = 0.0.xxx;
                
            #if defined(DEBUG_DISPLAY)
                #if defined(DYNAMICLIGHTMAP_ON)
                    inputData.dynamicLightmapUV = input.dynamicLightmapUV;
                #endif
                #if defined(LIGHTMAP_ON)
                    inputData.staticLightmapUV = input.staticLightmapUV;
                #else
                    // inputData.vertexSH = input.vertexSH;
                #endif
            #endif
                half3 vertexSH = SampleSHVertex(inputData.normalWS);

            #if defined(DYNAMICLIGHTMAP_ON)
                inputData.bakedGI = SAMPLE_GI(input.staticLightmapUV, input.dynamicLightmapUV.xy, vertexSH, inputData.normalWS);
            #else
                inputData.bakedGI = SAMPLE_GI(input.staticLightmapUV, vertexSH, inputData.normalWS);
            #endif
                inputData.normalizedScreenSpaceUV = input.positionSS;
                inputData.shadowMask = SAMPLE_SHADOWMASK(input.staticLightmapUV);

                inputData.tangentToWorld = half3x3(
                    input.bitangentWS.x, input.tangentWS.x, input.normalWS.x, 
                    input.bitangentWS.y, input.tangentWS.y, input.normalWS.y, 
                    input.bitangentWS.z, input.tangentWS.z, input.normalWS.z
                );
                
            }

        ENDHLSL
        
        Pass {


            Stencil {
                Ref [_StencilID]
                Comp [_StencilComp]
            }

            Name "StandardSurface"
            Tags { "LightMode"="UniversalForward" }

            HLSLPROGRAM


                // -------------------------------------
                // Material Keywords
                // #pragma shader_feature_local _NORMALMAP
                // #pragma shader_feature_local _PARALLAXMAP
                #pragma shader_feature_local _RECEIVE_SHADOWS_OFF
                #pragma shader_feature_local _ _DETAIL_MULX2 _DETAIL_SCALED
                #pragma shader_feature_local_fragment _SURFACE_TYPE_TRANSPARENT
                #pragma shader_feature_local_fragment _ALPHATEST_ON
                #pragma shader_feature_local_fragment _ALPHAPREMULTIPLY_ON
                #pragma shader_feature_local_fragment _EMISSION
                #pragma shader_feature_local_fragment _METALLICSPECGLOSSMAP
                #pragma shader_feature_local_fragment _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A
                #pragma shader_feature_local_fragment _OCCLUSIONMAP
                #pragma shader_feature_local_fragment _SPECULARHIGHLIGHTS_OFF
                #pragma shader_feature_local_fragment _ENVIRONMENTREFLECTIONS_OFF
                #pragma shader_feature_local_fragment _SPECULAR_SETUP

                // -------------------------------------
                // Universal Pipeline keywords
                #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN
                #pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
                #pragma multi_compile_fragment _ _ADDITIONAL_LIGHT_SHADOWS
                #pragma multi_compile_fragment _ _REFLECTION_PROBE_BLENDING
                #pragma multi_compile_fragment _ _REFLECTION_PROBE_BOX_PROJECTION
                #pragma multi_compile_fragment _ _SHADOWS_SOFT
                #pragma multi_compile_fragment _ _SCREEN_SPACE_OCCLUSION
                #pragma multi_compile_fragment _ _DBUFFER_MRT1 _DBUFFER_MRT2 _DBUFFER_MRT3
                #pragma multi_compile_fragment _ _LIGHT_LAYERS
                #pragma multi_compile_fragment _ _LIGHT_COOKIES
                #pragma multi_compile _ _CLUSTERED_RENDERING

                // -------------------------------------
                // Unity defined keywords
                #pragma multi_compile _ LIGHTMAP_SHADOW_MIXING
                #pragma multi_compile _ SHADOWS_SHADOWMASK
                #pragma multi_compile _ DIRLIGHTMAP_COMBINED
                #pragma multi_compile _ LIGHTMAP_ON
                #pragma multi_compile _ DYNAMICLIGHTMAP_ON
                #pragma multi_compile_fog
                #pragma multi_compile_fragment _ DEBUG_DISPLAY

                //--------------------------------------
                // GPU Instancing
                #pragma multi_compile_instancing
                #pragma instancing_options renderinglayer
                #pragma multi_compile _ DOTS_INSTANCING_ON
                    
                #pragma vertex ForwardPassVertex
                #pragma fragment ForwardPassFragment

                VertexOutput ForwardPassVertex(VertexInput input) {
                    
                    VertexOutput output = (VertexOutput)0;
                    InitializeVertexOutput(output, input);

                    return output;
                }



                float4 ForwardPassFragment(VertexOutput input) : SV_Target {
                    
                    if ( _ProximityDither == 1 )
                        if ( ProximityDither(input.positionWS, input.positionSS) )
                            return half4(0, 0, 0, 0);


                    SurfaceData surfaceData = (SurfaceData)0;
                    InputData inputData = (InputData)0;
                    InitializeLightingData( surfaceData, inputData, input );

                    CustomFragment(surfaceData, inputData, input);


                    return UniversalFragmentPBR(inputData, surfaceData); 
                }

            ENDHLSL
        }
        
        Pass {


            Stencil {
                Ref [_StencilID]
                Comp [_StencilComp]
            }

            Name "StandardSurface"
            Tags { "LightMode"="UniversalGBuffer" }

            HLSLPROGRAM

                // -------------------------------------
                // Material Keywords
                #pragma shader_feature_local _NORMALMAP
                #pragma shader_feature_local_fragment _ALPHATEST_ON
                //#pragma shader_feature_local_fragment _ALPHAPREMULTIPLY_ON
                #pragma shader_feature_local_fragment _EMISSION
                #pragma shader_feature_local_fragment _METALLICSPECGLOSSMAP
                #pragma shader_feature_local_fragment _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A
                #pragma shader_feature_local_fragment _OCCLUSIONMAP
                #pragma shader_feature_local _PARALLAXMAP
                #pragma shader_feature_local _ _DETAIL_MULX2 _DETAIL_SCALED

                #pragma shader_feature_local_fragment _SPECULARHIGHLIGHTS_OFF
                #pragma shader_feature_local_fragment _ENVIRONMENTREFLECTIONS_OFF
                #pragma shader_feature_local_fragment _SPECULAR_SETUP
                #pragma shader_feature_local _RECEIVE_SHADOWS_OFF

                // -------------------------------------
                // Universal Pipeline keywords
                #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN
                #pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
                #pragma multi_compile _ _ADDITIONAL_LIGHT_SHADOWS
                #pragma multi_compile_fragment _ _REFLECTION_PROBE_BLENDING
                #pragma multi_compile_fragment _ _REFLECTION_PROBE_BOX_PROJECTION
                #pragma multi_compile_fragment _ _SHADOWS_SOFT
                #pragma multi_compile_fragment _ _DBUFFER_MRT1 _DBUFFER_MRT2 _DBUFFER_MRT3
                #pragma multi_compile_fragment _ _LIGHT_LAYERS
                #pragma multi_compile_fragment _ _RENDER_PASS_ENABLED

                // -------------------------------------
                // Unity defined keywords
                #pragma multi_compile _ LIGHTMAP_SHADOW_MIXING
                #pragma multi_compile _ SHADOWS_SHADOWMASK
                #pragma multi_compile _ DIRLIGHTMAP_COMBINED
                #pragma multi_compile _ LIGHTMAP_ON
                #pragma multi_compile _ DYNAMICLIGHTMAP_ON
                #pragma multi_compile_fragment _ _GBUFFER_NORMALS_OCT

                //--------------------------------------
                // GPU Instancing
                #pragma multi_compile_instancing
                #pragma instancing_options renderinglayer
                #pragma multi_compile _ DOTS_INSTANCING_ON

                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/UnityGBuffer.hlsl"
                    
                #pragma vertex GBufferPassVertex
                #pragma fragment GBufferPassFragment

                VertexOutput GBufferPassVertex(VertexInput input) {
                    
                    VertexOutput output = (VertexOutput)0;
                    InitializeVertexOutput(output, input);


                    return output;
                }

                FragmentOutput GBufferPassFragment(VertexOutput input) : SV_Target {
                    
                    if ( _ProximityDither == 1 )
                        if ( ProximityDither(input.positionWS, input.positionSS) )
                            return (FragmentOutput)0;


                    SurfaceData surfaceData = (SurfaceData)0;
                    InputData inputData = (InputData)0;
                    InitializeLightingData( surfaceData, inputData, input );

                    CustomFragment( surfaceData, inputData, input );

                    return SurfaceDataToGbuffer( surfaceData, inputData, inputData.bakedGI * surfaceData.albedo, 0 ); 
                }

            ENDHLSL
        }
        

        Pass {

            Name "ShadowCaster"
            Tags { "LightMode"="ShadowCaster" }
        
            HLSLPROGRAM
            
            #if defined(LOD_FADE_CROSSFADE)
                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/LODCrossFade.hlsl"
            #endif

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

            // Shadow Casting Light geometric parameters. These variables are used when applying the shadow Normal Bias and are set by UnityEngine.Rendering.Universal.ShadowUtils.SetupShadowCasterConstantBuffer in com.unity.render-pipelines.universal/Runtime/ShadowUtils.cs
            // For Directional lights, _LightDirection is used when applying shadow Normal Bias.
            // For Spot lights and Point lights, _LightPosition is used to compute the actual light direction because it is different at each shadow caster geometry vertex.
            float3 _LightDirection;
            float3 _LightPosition;

            struct Attributes {
                float4 positionOS   : POSITION;
                float3 normalOS     : NORMAL;
                float2 texcoord     : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings {
                float2 uv           : TEXCOORD0;
                float4 positionCS   : SV_POSITION;
            };

            float4 GetShadowPositionHClip(Attributes input) {

                float3 positionWS = TransformObjectToWorld(input.positionOS.xyz);
                float3 normalWS = TransformObjectToWorldNormal(input.normalOS);

                CustomVertexDisplacement( positionWS, normalWS );

                #if _CASTING_PUNCTUAL_LIGHT_SHADOW
                    float3 lightDirectionWS = normalize(_LightPosition - positionWS);
                #else
                    float3 lightDirectionWS = _LightDirection;
                #endif


                float4 positionCS = TransformWorldToHClip(ApplyShadowBias(positionWS, normalWS, lightDirectionWS));


                #if UNITY_REVERSED_Z
                    positionCS.z = min(positionCS.z, UNITY_NEAR_CLIP_VALUE);
                #else
                    positionCS.z = max(positionCS.z, UNITY_NEAR_CLIP_VALUE);
                #endif

                return positionCS;
            }

            Varyings ShadowPassVertex(Attributes input) {
                Varyings output;
                UNITY_SETUP_INSTANCE_ID(input);
                // UNITY_INITIALIZE_OUTPUT(Varyings, output);

                // output.uv = TRANSFORM_TEX(input.texcoord, _BaseMap);
                output.positionCS = GetShadowPositionHClip(input);
                return output;
            }

            half4 ShadowPassFragment(Varyings input) : SV_TARGET {
                // Alpha(SampleAlbedoAlpha(input.uv, TEXTURE2D_ARGS(_BaseMap, sampler_BaseMap)).a, _BaseColor, _Cutoff);

                #ifdef LOD_FADE_CROSSFADE
                    LODFadeCrossFade(input.positionCS);
                #endif

                return 0;
            }
        
            ENDHLSL
        }

        Pass
        {
            Name "Meta"
            Tags{"LightMode" = "Meta"}

            Cull Off

            HLSLPROGRAM
            #pragma only_renderers gles gles3 glcore d3d11
            #pragma target 2.0

            // #pragma shader_feature EDITOR_VISUALIZATION
            #pragma shader_feature_local_fragment _SPECULAR_SETUP
            #pragma shader_feature_local_fragment _EMISSION
            #pragma shader_feature_local_fragment _METALLICSPECGLOSSMAP
            #pragma shader_feature_local_fragment _ALPHATEST_ON
            #pragma shader_feature_local_fragment _ _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A
            #pragma shader_feature_local _ _DETAIL_MULX2 _DETAIL_SCALED

            #pragma shader_feature_local_fragment _SPECGLOSSMAP


            struct Attributes {
                float4 positionOS   : POSITION;
                float3 normalOS     : NORMAL;
                float2 uv0          : TEXCOORD0;
                float2 uv1          : TEXCOORD1;
                float2 uv2          : TEXCOORD2;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings {
                float4 positionCS   : SV_POSITION;
                float2 uv           : TEXCOORD0;
            #ifdef EDITOR_VISUALIZATION
                float2 VizUV        : TEXCOORD1;
                float4 LightCoord   : TEXCOORD2;
            #endif
            };

            #pragma vertex UniversalVertexMeta
            #pragma fragment UniversalFragmentMetaLit

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/MetaInput.hlsl"

            Varyings UniversalVertexMeta(Attributes input) {
                Varyings output = (Varyings)0;
                output.positionCS = UnityMetaVertexPosition(input.positionOS.xyz, input.uv1, input.uv2);
                output.uv = TRANSFORM_TEX(input.uv0, _BaseMap);
                return output;
            }

            half4 UniversalFragmentMetaLit(Varyings input) : SV_Target {
                half4 baseColor = tex2D(_BaseMap, input.uv);
                half4 specularColor = tex2D(_SpecularMap, input.uv);
                half3 diffuse = baseColor.rgb * (1.0.xxx - specularColor);

                MetaInput metaInput;
                metaInput.Albedo = diffuse + specularColor * 0.5;
                metaInput.Emission = 10 * baseColor.rgb;
            #ifdef EDITOR_VISUALIZATION
                metaInput.VizUV = input.VizUV;
                metaInput.LightCoord = input.LightCoord;
            #endif

                return UnityMetaFragment(metaInput);
            }

            ENDHLSL
        }

    }
}
