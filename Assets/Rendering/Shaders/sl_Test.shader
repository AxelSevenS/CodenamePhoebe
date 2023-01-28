Shader "Selene/Test" {
    
    Properties {
        [NoScaleOffset] _BaseMap ("Main Texture", 2D) = "white" {}

        [NoScaleOffset] _NormalMap ("Normal Map", 2D) = "bump" {}
        _NormalIntensity ("Normal Intensity", Range(0,1)) = 0

        [MaterialToggle] _ProximityDither ("Proximity Dither", Float) = 0

        _SpecularIntensity ("Specular Intensity", Range(0,3)) = 0
        _Smoothness ("Smoothness", Range(0,1)) = 0
        
        _AccentIntensity ("Accent Intensity", Range(0, 1)) = 0

        _EmissionMap ("Emission Map Texture", 2D) = "white" {}

        [IntRange] _StencilID ("Stencil ID", Range(0, 255)) = 0
        [Enum(UnityEngine.Rendering.CompareFunction)] _StencilComp ("Stencil Comparison", Int) = 0
    }


    SubShader {
        // Universal Pipeline tag is required. If Universal render pipeline is not set in the graphics settings
        // this Subshader will fail. One can add a subshader below or fallback to Standard built-in to make this
        // material work with both Universal Render Pipeline and Builtin Unity Pipeline
        Tags{"RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" "UniversalMaterialType" = "Lit" "IgnoreProjector" = "True" "ShaderModel"="4.5"}
        LOD 300


        Pass {
            // Lightmode matches the ShaderPassName set in UniversalRenderPipeline.cs. SRPDefaultUnlit and passes with
            // no LightMode tag are also rendered by Universal Render Pipeline
            Name "ForwardLit"
            Tags{"LightMode" = "UniversalForward"}

            Stencil {
                Ref [_StencilID]
                Comp [_StencilComp]
            }
            Blend[_SrcBlend][_DstBlend]
            ZWrite[_ZWrite]
            Cull[_Cull]

            HLSLPROGRAM
            // #pragma exclude_renderers gles gles3 glcore
            // #pragma target 4.5

            // // -------------------------------------
            // // Material Keywords
            // #pragma shader_feature_local _NORMALMAP
            // #pragma shader_feature_local_fragment _ALPHATEST_ON
            // //#pragma shader_feature_local_fragment _ALPHAPREMULTIPLY_ON
            // #pragma shader_feature_local_fragment _EMISSION
            // #pragma shader_feature_local_fragment _METALLICSPECGLOSSMAP
            // #pragma shader_feature_local_fragment _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A
            // #pragma shader_feature_local_fragment _OCCLUSIONMAP
            // #pragma shader_feature_local _PARALLAXMAP
            // #pragma shader_feature_local _ _DETAIL_MULX2 _DETAIL_SCALED

            // #pragma shader_feature_local_fragment _SPECULARHIGHLIGHTS_OFF
            // #pragma shader_feature_local_fragment _ENVIRONMENTREFLECTIONS_OFF
            // #pragma shader_feature_local_fragment _SPECULAR_SETUP
            // #pragma shader_feature_local _RECEIVE_SHADOWS_OFF

            // // -------------------------------------
            // // Universal Pipeline keywords
            // #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN
            // //#pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
            // //#pragma multi_compile _ _ADDITIONAL_LIGHT_SHADOWS
            // #pragma multi_compile_fragment _ _REFLECTION_PROBE_BLENDING
            // #pragma multi_compile_fragment _ _REFLECTION_PROBE_BOX_PROJECTION
            // #pragma multi_compile_fragment _ _SHADOWS_SOFT
            // #pragma multi_compile_fragment _ _DBUFFER_MRT1 _DBUFFER_MRT2 _DBUFFER_MRT3
            // #pragma multi_compile_fragment _ _LIGHT_LAYERS
            // #pragma multi_compile_fragment _ _RENDER_PASS_ENABLED

            // // -------------------------------------
            // // Unity defined keywords
            // #pragma multi_compile _ LIGHTMAP_SHADOW_MIXING
            // #pragma multi_compile _ SHADOWS_SHADOWMASK
            // #pragma multi_compile _ DIRLIGHTMAP_COMBINED
            // #pragma multi_compile _ LIGHTMAP_ON
            // #pragma multi_compile _ DYNAMICLIGHTMAP_ON
            // #pragma multi_compile_fragment _ _GBUFFER_NORMALS_OCT

            // //--------------------------------------
            // // GPU Instancing
            // #pragma multi_compile_instancing
            // #pragma instancing_options renderinglayer
            // #pragma multi_compile _ DOTS_INSTANCING_ON

            // #pragma vertex LitForwardPassVertex
            // #pragma fragment LitForwardPassFragment

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            CBUFFER_START(UnityPerMaterial)
                float4 _BaseMap_ST;
                float4 _NormalMap_ST;
                half _NormalIntensity;
                int _ProximityDither;
                half _SpecularIntensity;
                half _Smoothness;
                half _AccentIntensity;
                float4 _EmissionMap_ST;
            CBUFFER_END

            TEXTURE2D(_BaseMap);        SAMPLER(sampler_BaseMap);
            TEXTURE2D(_NormalMap);        SAMPLER(sampler_NormalMap);
            TEXTURE2D(_EmissionMap);        SAMPLER(sampler_EmissionMap);

            struct Attributes {
                float4 positionOS   : POSITION;
                float3 normalOS     : NORMAL;
                float4 tangentOS    : TANGENT;
                float2 texcoord     : TEXCOORD0;
                float2 staticLightmapUV   : TEXCOORD1;
                float2 dynamicLightmapUV  : TEXCOORD2;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings {
                float2 uv                       : TEXCOORD0;
                float3 positionWS               : TEXCOORD1;

                float3 normalWS                 : TEXCOORD2;
                half4 tangentWS                : TEXCOORD3;
                float3 viewDirWS                : TEXCOORD4;

            #ifdef _ADDITIONAL_LIGHTS_VERTEX
                half4 fogFactorAndVertexLight   : TEXCOORD5; // x: fogFactor, yzw: vertex light
            #else
                half  fogFactor                 : TEXCOORD5;
            #endif
                float4 shadowCoord              : TEXCOORD6;

            #if defined(REQUIRES_TANGENT_SPACE_VIEW_DIR_INTERPOLATOR)
                half3 viewDirTS                : TEXCOORD7;
            #endif

                DECLARE_LIGHTMAP_OR_SH(staticLightmapUV, vertexSH, 8);
            #ifdef DYNAMICLIGHTMAP_ON
                float2  dynamicLightmapUV : TEXCOORD9; // Dynamic lightmap UVs
            #endif

                float4 positionCS               : SV_POSITION;
                UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_VERTEX_OUTPUT_STEREO
            };

            Varyings LitForwardPassVertex(Attributes input) {
                Varyings output = (Varyings)0;

                // UNITY_SETUP_INSTANCE_ID(input);
                // UNITY_TRANSFER_INSTANCE_ID(input, output);
                // UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

                output.positionWS = TransformObjectToWorld(input.positionOS.xyz);
                // half4 positionVS = TransformWorldToView(positionWS);
                
                // real sign = real(input.tangentOS.w) * GetOddNegativeScale();
                // output.normalWS = TransformObjectToWorldNormal(input.normalOS);
                // output.tangentWS = half4(TransformObjectToWorldDir(input.tangentOS.xyz), sign);
                // output.bitangentWS = half3(cross(output.normalWS, float3(output.tangentWS))) * sign;

            //     half3 vertexLight = VertexLighting(output.positionWS, output.normalWS);

            //     half fogFactor = 0;
            // #if !defined(_FOG_FRAGMENT)
            //     fogFactor = ComputeFogFactor(output.positionCS.z);
            // #endif

            //     output.uv = TRANSFORM_TEX(input.texcoord, _BaseMap);

            // #if defined(REQUIRES_TANGENT_SPACE_VIEW_DIR_INTERPOLATOR)
            //     output.viewDirWS = GetWorldSpaceNormalizeViewDir(output.positionWS);
            //     output.viewDirTS = GetViewDirectionTangentSpace(output.tangentWS, output.normalWS, output.viewDirWS);
            // #endif

            //     OUTPUT_LIGHTMAP_UV(input.staticLightmapUV, unity_LightmapST, output.staticLightmapUV);
            // #ifdef DYNAMICLIGHTMAP_ON
            //     output.dynamicLightmapUV = input.dynamicLightmapUV.xy * unity_DynamicLightmapST.xy + unity_DynamicLightmapST.zw;
            // #endif
            //     OUTPUT_SH(output.normalWS.xyz, output.vertexSH);
            // #ifdef _ADDITIONAL_LIGHTS_VERTEX
            //     output.fogFactorAndVertexLight = half4(fogFactor, vertexLight);
            // #else
            //     output.fogFactor = fogFactor;
            // #endif

            // #if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
            //     #if defined(_MAIN_LIGHT_SHADOWS_SCREEN) && !defined(_SURFACE_TYPE_TRANSPARENT)
            //         output.shadowCoord = ComputeScreenPos(output.positionCS);
            //     #else
            //         output.shadowCoord = TransformWorldToShadowCoord(output.positionWS);
            //     #endif
            // #endif

                // VERTEX DISPLACEMENT GOES HERE

                output.positionCS = TransformWorldToHClip(output.positionWS);
                // CustomVertexDisplacement(input, output);

                return output;
            }

            half4 LitForwardPassFragment(Varyings input) : SV_Target {
            //     UNITY_SETUP_INSTANCE_ID(input);
            //     UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

            // #if defined(_PARALLAXMAP)
            //     ApplyPerPixelDisplacement(input.viewDirTS, input.uv);
            // #endif



            //     SurfaceData surfaceData;

            //     half4 albedo = (SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, input.uv));
            //     surfaceData.albedo = albedo.rgb;
            //     surfaceData.alpha = albedo.a;

            //     // half4 specGloss = SampleMetallicSpecGloss(input.uv, surfaceData.alpha);

            //     surfaceData.metallic = 0;
            //     surfaceData.occlusion = 0;
            //     surfaceData.specular = half3(_SpecularIntensity, _AccentIntensity, 0);

            //     surfaceData.smoothness = _Smoothness;
            //     surfaceData.normalTS = UnpackNormal(SAMPLE_TEXTURE2D(_NormalMap, sampler_NormalMap, input.uv));
            //     surfaceData.emission = SAMPLE_TEXTURE2D(_EmissionMap, sampler_EmissionMap, input.uv).rgb;

            // #if defined(_CLEARCOAT) || defined(_CLEARCOATMAP)
            //     half2 clearCoat = SampleClearCoat(uv);
            //     surfaceData.clearCoatMask       = clearCoat.r;
            //     surfaceData.clearCoatSmoothness = clearCoat.g;
            // #else
            //     surfaceData.clearCoatMask       = half(0.0);
            //     surfaceData.clearCoatSmoothness = half(0.0);
            // #endif

            // #if defined(_DETAIL)
            //     half detailMask = SAMPLE_TEXTURE2D(_DetailMask, sampler_DetailMask, uv).a;
            //     float2 detailUv = uv * _DetailAlbedoMap_ST.xy + _DetailAlbedoMap_ST.zw;
            //     surfaceData.albedo = ApplyDetailAlbedo(detailUv, surfaceData.albedo, detailMask);
            //     surfaceData.normalTS = ApplyDetailNormal(detailUv, surfaceData.normalTS, detailMask);
            // #endif



            //     InputData inputData = (InputData)0;

            //     inputData.positionWS = input.positionWS;

            //     // half3 viewDirWS = GetWorldSpaceNormalizeViewDir(input.positionWS);
            // // #if defined(_NORMALMAP) || defined(_DETAIL)
            // //     float sgn = input.tangentWS.w;      // should be either +1 or -1
            // //     float3 bitangent = sgn * cross(input.normalWS.xyz, input.tangentWS.xyz);
            // //     half3x3 tangentToWorld = half3x3(input.tangentWS.xyz, bitangent.xyz, input.normalWS.xyz);

            // //     #if defined(_NORMALMAP)
            // //     inputData.tangentToWorld = tangentToWorld;
            // //     #endif
            // //     inputData.normalWS = TransformTangentToWorld(normalTS, tangentToWorld);
            // // #else
            // //     inputData.normalWS = input.normalWS;
            // // #endif

            //     inputData.normalWS = NormalizeNormalPerPixel(inputData.normalWS);
            //     inputData.viewDirectionWS = GetWorldSpaceNormalizeViewDir(input.positionWS);

            // #if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
            //     inputData.shadowCoord = input.shadowCoord;
            // #elif defined(MAIN_LIGHT_CALCULATE_SHADOWS)
            //     inputData.shadowCoord = TransformWorldToShadowCoord(inputData.positionWS);
            // #else
            //     inputData.shadowCoord = float4(0, 0, 0, 0);
            // #endif
            // #ifdef _ADDITIONAL_LIGHTS_VERTEX
            //     inputData.fogCoord = InitializeInputDataFog(float4(input.positionWS, 1.0), input.fogFactorAndVertexLight.x);
            //     inputData.vertexLighting = input.fogFactorAndVertexLight.yzw;
            // #else
            //     inputData.fogCoord = InitializeInputDataFog(float4(input.positionWS, 1.0), input.fogFactor);
            // #endif

            // #if defined(DYNAMICLIGHTMAP_ON)
            //     inputData.bakedGI = SAMPLE_GI(input.staticLightmapUV, input.dynamicLightmapUV, input.vertexSH, inputData.normalWS);
            // #else
            //     inputData.bakedGI = SAMPLE_GI(input.staticLightmapUV, input.vertexSH, inputData.normalWS);
            // #endif


            //     inputData.normalizedScreenSpaceUV = GetNormalizedScreenSpaceUV(input.positionCS);
            //     inputData.shadowMask = SAMPLE_SHADOWMASK(input.staticLightmapUV);

            // #if defined(DEBUG_DISPLAY)
            // #if defined(DYNAMICLIGHTMAP_ON)
            //     inputData.dynamicLightmapUV = input.dynamicLightmapUV;
            // #endif
            // #if defined(LIGHTMAP_ON)
            //     inputData.staticLightmapUV = input.staticLightmapUV;
            // #else
            //     inputData.vertexSH = input.vertexSH;
            // #endif
            // #endif
            //     SETUP_DEBUG_TEXTURE_DATA(inputData, input.uv, _BaseMap);

            // #ifdef _DBUFFER
            //     ApplyDecalToSurfaceData(input.positionCS, surfaceData, inputData);
            // #endif




                // half4 color = UniversalFragmentPBR(inputData, surfaceData);

                half4 color = half4(1, 1, 1, 1);

                // color.rgb = MixFog(color.rgb, inputData.fogCoord);
                // color.a = OutputAlpha(color.a, _Surface);

                return color;
            }
            ENDHLSL
        }

        // Pass {
        //     // Lightmode matches the ShaderPassName set in UniversalRenderPipeline.cs. SRPDefaultUnlit and passes with
        //     // no LightMode tag are also rendered by Universal Render Pipeline
        //     Name "GBuffer"
        //     Tags{"LightMode" = "UniversalGBuffer"}
            
        //     Stencil {
        //         Ref [_StencilID]
        //         Comp [_StencilComp]
        //     }
        //     ZWrite[_ZWrite]
        //     ZTest LEqual
        //     Cull[_Cull]

        //     HLSLPROGRAM
        //     #pragma exclude_renderers gles gles3 glcore
        //     #pragma target 4.5

        //     // -------------------------------------
        //     // Material Keywords
        //     #pragma shader_feature_local _NORMALMAP
        //     #pragma shader_feature_local_fragment _ALPHATEST_ON
        //     //#pragma shader_feature_local_fragment _ALPHAPREMULTIPLY_ON
        //     #pragma shader_feature_local_fragment _EMISSION
        //     #pragma shader_feature_local_fragment _METALLICSPECGLOSSMAP
        //     #pragma shader_feature_local_fragment _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A
        //     #pragma shader_feature_local_fragment _OCCLUSIONMAP
        //     #pragma shader_feature_local _PARALLAXMAP
        //     #pragma shader_feature_local _ _DETAIL_MULX2 _DETAIL_SCALED

        //     #pragma shader_feature_local_fragment _SPECULARHIGHLIGHTS_OFF
        //     #pragma shader_feature_local_fragment _ENVIRONMENTREFLECTIONS_OFF
        //     #pragma shader_feature_local_fragment _SPECULAR_SETUP
        //     #pragma shader_feature_local _RECEIVE_SHADOWS_OFF

        //     // -------------------------------------
        //     // Universal Pipeline keywords
        //     #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN
        //     //#pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
        //     //#pragma multi_compile _ _ADDITIONAL_LIGHT_SHADOWS
        //     #pragma multi_compile_fragment _ _REFLECTION_PROBE_BLENDING
        //     #pragma multi_compile_fragment _ _REFLECTION_PROBE_BOX_PROJECTION
        //     #pragma multi_compile_fragment _ _SHADOWS_SOFT
        //     #pragma multi_compile_fragment _ _DBUFFER_MRT1 _DBUFFER_MRT2 _DBUFFER_MRT3
        //     #pragma multi_compile_fragment _ _LIGHT_LAYERS
        //     #pragma multi_compile_fragment _ _RENDER_PASS_ENABLED

        //     // -------------------------------------
        //     // Unity defined keywords
        //     #pragma multi_compile _ LIGHTMAP_SHADOW_MIXING
        //     #pragma multi_compile _ SHADOWS_SHADOWMASK
        //     #pragma multi_compile _ DIRLIGHTMAP_COMBINED
        //     #pragma multi_compile _ LIGHTMAP_ON
        //     #pragma multi_compile _ DYNAMICLIGHTMAP_ON
        //     #pragma multi_compile_fragment _ _GBUFFER_NORMALS_OCT

        //     //--------------------------------------
        //     // GPU Instancing
        //     #pragma multi_compile_instancing
        //     #pragma instancing_options renderinglayer
        //     #pragma multi_compile _ DOTS_INSTANCING_ON

        //     #pragma vertex LitGBufferPassVertex
        //     #pragma fragment LitGBufferPassFragment

        //     #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        //     #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

        //     CBUFFER_START(UnityPerMaterial)
        //         float4 _BaseMap_ST;
        //         float4 _NormalMap_ST;
        //         half _NormalIntensity;
        //         int _ProximityDither;
        //         half _SpecularIntensity;
        //         half _Smoothness;
        //         half _AccentIntensity;
        //         float4 _EmissionMap_ST;
        //     CBUFFER_END

        //     TEXTURE2D(_BaseMap);        SAMPLER(sampler_BaseMap);
        //     TEXTURE2D(_NormalMap);        SAMPLER(sampler_NormalMap);
        //     TEXTURE2D(_EmissionMap);        SAMPLER(sampler_EmissionMap);

        //     struct Attributes {
        //         float4 positionOS   : POSITION;
        //         float3 normalOS     : NORMAL;
        //         float4 tangentOS    : TANGENT;
        //         float2 texcoord     : TEXCOORD0;
        //         float2 staticLightmapUV   : TEXCOORD1;
        //         float2 dynamicLightmapUV  : TEXCOORD2;
        //         UNITY_VERTEX_INPUT_INSTANCE_ID
        //     };

        //     struct Varyings {
        //         float2 uv                       : TEXCOORD0;
        //         float3 positionWS               : TEXCOORD1;

        //         float3 normalWS                 : TEXCOORD2;
        //         half4 tangentWS                : TEXCOORD3;
        //         float3 viewDirWS                : TEXCOORD4;

        //     #ifdef _ADDITIONAL_LIGHTS_VERTEX
        //         half4 fogFactorAndVertexLight   : TEXCOORD5; // x: fogFactor, yzw: vertex light
        //     #else
        //         half  fogFactor                 : TEXCOORD5;
        //     #endif
        //         float4 shadowCoord              : TEXCOORD6;

        //     #if defined(REQUIRES_TANGENT_SPACE_VIEW_DIR_INTERPOLATOR)
        //         half3 viewDirTS                : TEXCOORD7;
        //     #endif

        //         DECLARE_LIGHTMAP_OR_SH(staticLightmapUV, vertexSH, 8);
        //     #ifdef DYNAMICLIGHTMAP_ON
        //         float2  dynamicLightmapUV : TEXCOORD9; // Dynamic lightmap UVs
        //     #endif

        //         float4 positionCS               : SV_POSITION;
        //         UNITY_VERTEX_INPUT_INSTANCE_ID
        //         UNITY_VERTEX_OUTPUT_STEREO
        //     };

        //     Varyings LitGBufferPassVertex(Attributes input) {
        //         Varyings output = (Varyings)0;

        //         UNITY_SETUP_INSTANCE_ID(input);
        //         UNITY_TRANSFER_INSTANCE_ID(input, output);
        //         UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

        //         output.positionWS = TransformObjectToWorld(input.positionOS.xyz);
        //         // half4 positionVS = TransformWorldToView(positionWS);
                
        //         real sign = real(input.tangentOS.w) * GetOddNegativeScale();
        //         output.normalWS = TransformObjectToWorldNormal(input.normalOS);
        //         output.tangentWS = half4(TransformObjectToWorldDir(input.tangentOS.xyz), sign);
        //         // output.bitangentWS = half3(cross(output.normalWS, float3(output.tangentWS))) * sign;

        //         half3 vertexLight = VertexLighting(output.positionWS, output.normalWS);

        //         half fogFactor = 0;
        //     #if !defined(_FOG_FRAGMENT)
        //         fogFactor = ComputeFogFactor(output.positionCS.z);
        //     #endif

        //         output.uv = TRANSFORM_TEX(input.texcoord, _BaseMap);

        //     #if defined(REQUIRES_TANGENT_SPACE_VIEW_DIR_INTERPOLATOR)
        //         output.viewDirWS = GetWorldSpaceNormalizeViewDir(output.positionWS);
        //         output.viewDirTS = GetViewDirectionTangentSpace(output.tangentWS, output.normalWS, output.viewDirWS);
        //     #endif

        //         OUTPUT_LIGHTMAP_UV(input.staticLightmapUV, unity_LightmapST, output.staticLightmapUV);
        //     #ifdef DYNAMICLIGHTMAP_ON
        //         output.dynamicLightmapUV = input.dynamicLightmapUV.xy * unity_DynamicLightmapST.xy + unity_DynamicLightmapST.zw;
        //     #endif
        //         OUTPUT_SH(output.normalWS.xyz, output.vertexSH);
        //     #ifdef _ADDITIONAL_LIGHTS_VERTEX
        //         output.fogFactorAndVertexLight = half4(fogFactor, vertexLight);
        //     #else
        //         output.fogFactor = fogFactor;
        //     #endif

        //     #if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
        //         #if defined(_MAIN_LIGHT_SHADOWS_SCREEN) && !defined(_SURFACE_TYPE_TRANSPARENT)
        //             output.shadowCoord = ComputeScreenPos(output.positionCS);
        //         #else
        //             output.shadowCoord = TransformWorldToShadowCoord(output.positionWS);
        //         #endif
        //     #endif

        //         // VERTEX DISPLACEMENT GOES HERE

        //         output.positionCS = TransformWorldToHClip(output.positionWS);
        //         // CustomVertexDisplacement(input, output);

        //         return output;
        //     }

        //     // Used in Standard (Physically Based) shader
        //     half4 LitGBufferPassFragment(Varyings input) : SV_Target {
        //         UNITY_SETUP_INSTANCE_ID(input);
        //         UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

        //     #if defined(_PARALLAXMAP)
        //         ApplyPerPixelDisplacement(input.viewDirTS, input.uv);
        //     #endif



        //         SurfaceData surfaceData;

        //         half4 albedo = (SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, input.uv));
        //         surfaceData.albedo = albedo.rgb;
        //         surfaceData.alpha = albedo.a;

        //         // half4 specGloss = SampleMetallicSpecGloss(input.uv, surfaceData.alpha);

        //         surfaceData.metallic = 0;
        //         surfaceData.occlusion = 0;
        //         surfaceData.specular = half3(_SpecularIntensity, _AccentIntensity, 0);

        //         surfaceData.smoothness = _Smoothness;
        //         surfaceData.normalTS = UnpackNormal(SAMPLE_TEXTURE2D(_NormalMap, sampler_NormalMap, input.uv));
        //         surfaceData.emission = SAMPLE_TEXTURE2D(_EmissionMap, sampler_EmissionMap, input.uv).rgb;

        //     #if defined(_CLEARCOAT) || defined(_CLEARCOATMAP)
        //         half2 clearCoat = SampleClearCoat(uv);
        //         surfaceData.clearCoatMask       = clearCoat.r;
        //         surfaceData.clearCoatSmoothness = clearCoat.g;
        //     #else
        //         surfaceData.clearCoatMask       = half(0.0);
        //         surfaceData.clearCoatSmoothness = half(0.0);
        //     #endif

        //     #if defined(_DETAIL)
        //         half detailMask = SAMPLE_TEXTURE2D(_DetailMask, sampler_DetailMask, uv).a;
        //         float2 detailUv = uv * _DetailAlbedoMap_ST.xy + _DetailAlbedoMap_ST.zw;
        //         surfaceData.albedo = ApplyDetailAlbedo(detailUv, surfaceData.albedo, detailMask);
        //         surfaceData.normalTS = ApplyDetailNormal(detailUv, surfaceData.normalTS, detailMask);
        //     #endif



        //         InputData inputData = (InputData)0;

        //         inputData.positionWS = input.positionWS;

        //         half3 viewDirWS = GetWorldSpaceNormalizeViewDir(input.positionWS);
        //     #if defined(_NORMALMAP) || defined(_DETAIL)
        //         float sgn = input.tangentWS.w;      // should be either +1 or -1
        //         float3 bitangent = sgn * cross(input.normalWS.xyz, input.tangentWS.xyz);
        //         half3x3 tangentToWorld = half3x3(input.tangentWS.xyz, bitangent.xyz, input.normalWS.xyz);

        //         #if defined(_NORMALMAP)
        //         inputData.tangentToWorld = tangentToWorld;
        //         #endif
        //         inputData.normalWS = TransformTangentToWorld(normalTS, tangentToWorld);
        //     #else
        //         inputData.normalWS = input.normalWS;
        //     #endif

        //         inputData.normalWS = NormalizeNormalPerPixel(inputData.normalWS);
        //         inputData.viewDirectionWS = viewDirWS;

        //     #if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
        //         inputData.shadowCoord = input.shadowCoord;
        //     #elif defined(MAIN_LIGHT_CALCULATE_SHADOWS)
        //         inputData.shadowCoord = TransformWorldToShadowCoord(inputData.positionWS);
        //     #else
        //         inputData.shadowCoord = float4(0, 0, 0, 0);
        //     #endif
        //     #ifdef _ADDITIONAL_LIGHTS_VERTEX
        //         inputData.fogCoord = InitializeInputDataFog(float4(input.positionWS, 1.0), input.fogFactorAndVertexLight.x);
        //         inputData.vertexLighting = input.fogFactorAndVertexLight.yzw;
        //     #else
        //         inputData.fogCoord = InitializeInputDataFog(float4(input.positionWS, 1.0), input.fogFactor);
        //     #endif

        //     #if defined(DYNAMICLIGHTMAP_ON)
        //         inputData.bakedGI = SAMPLE_GI(input.staticLightmapUV, input.dynamicLightmapUV, input.vertexSH, inputData.normalWS);
        //     #else
        //         inputData.bakedGI = SAMPLE_GI(input.staticLightmapUV, input.vertexSH, inputData.normalWS);
        //     #endif


        //         inputData.normalizedScreenSpaceUV = GetNormalizedScreenSpaceUV(input.positionCS);
        //         inputData.shadowMask = SAMPLE_SHADOWMASK(input.staticLightmapUV);

        //     #if defined(DEBUG_DISPLAY)
        //     #if defined(DYNAMICLIGHTMAP_ON)
        //         inputData.dynamicLightmapUV = input.dynamicLightmapUV;
        //     #endif
        //     #if defined(LIGHTMAP_ON)
        //         inputData.staticLightmapUV = input.staticLightmapUV;
        //     #else
        //         inputData.vertexSH = input.vertexSH;
        //     #endif
        //     #endif
        //         SETUP_DEBUG_TEXTURE_DATA(inputData, input.uv, _BaseMap);

        //     #ifdef _DBUFFER
        //         ApplyDecalToSurfaceData(input.positionCS, surfaceData, inputData);
        //     #endif




        //         half4 color = UniversalFragmentPBR(inputData, surfaceData);

        //         color.rgb = MixFog(color.rgb, inputData.fogCoord);
        //         // color.a = OutputAlpha(color.a, _Surface);

        //         return color;
        //     }
        //     ENDHLSL
        // }

        Pass {
            Name "ShadowCaster"
            Tags{"LightMode" = "ShadowCaster"}

            ZWrite On
            ZTest LEqual
            ColorMask 0
            Cull[_Cull]

            HLSLPROGRAM
            #pragma exclude_renderers gles gles3 glcore
            #pragma target 4.5

            // -------------------------------------
            // Material Keywords
            #pragma shader_feature_local_fragment _ALPHATEST_ON
            #pragma shader_feature_local_fragment _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A

            //--------------------------------------
            // GPU Instancing
            #pragma multi_compile_instancing
            #pragma multi_compile _ DOTS_INSTANCING_ON

            // -------------------------------------
            // Universal Pipeline keywords

            // This is used during shadow map generation to differentiate between directional and punctual light shadows, as they use different formulas to apply Normal Bias
            #pragma multi_compile_vertex _ _CASTING_PUNCTUAL_LIGHT_SHADOW

            #pragma vertex ShadowPassVertex
            #pragma fragment ShadowPassFragment

            #include "Packages/com.unity.render-pipelines.universal/Shaders/LitInput.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/ShadowCasterPass.hlsl"
            ENDHLSL
        }
    }
}
