Shader "CelShaded/Crystal"
{
    Properties
    {
        [NoScaleOffset]Texture2D_3c75bcc60ecc4d7485718a7ecd47a5d1("ColorMap", 2D) = "white" {}
        [NoScaleOffset]Texture2D_4f60ab3d73c44afeb98ad3eed5ede20e("NormalMap", 2D) = "grey" {}
        Vector1_981b8d57da164c75ac12775b5d189f10("IndexOfRefraction", Float) = 0
        Vector1_dbffe0c7baa44a9b8467243fa97e9b01("NormalStrength", Range(0, 1)) = 0
        [NonModifiableTextureData][NoScaleOffset]_CelShading_6ae06c46a72c4ceab41183f7f9e2e422_Texture2D9666dd2f2f8f4fddb6c2a5c61ddfa9bd_3072536305("Texture2D", 2D) = "white" {}
        [HideInInspector][NoScaleOffset]unity_Lightmaps("unity_Lightmaps", 2DArray) = "" {}
        [HideInInspector][NoScaleOffset]unity_LightmapsInd("unity_LightmapsInd", 2DArray) = "" {}
        [HideInInspector][NoScaleOffset]unity_ShadowMasks("unity_ShadowMasks", 2DArray) = "" {}
    }
    SubShader
    {
        Tags
        {
            "RenderPipeline"="UniversalPipeline"
            "RenderType"="Transparent"
            "UniversalMaterialType" = "Unlit"
            "Queue"="Transparent"
        }
        Pass
        {
            Name "Pass"
            Tags
            {
                // LightMode: <None>
            }

            // Render State
            Cull Back
        Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
        ZTest LEqual
        ZWrite Off

            // Debug
            // <None>

            // --------------------------------------------------
            // Pass

            HLSLPROGRAM

            // Pragmas
            #pragma target 4.5
        #pragma exclude_renderers gles gles3 glcore
        #pragma multi_compile_instancing
        #pragma multi_compile_fog
        #pragma multi_compile _ DOTS_INSTANCING_ON
        #pragma vertex vert
        #pragma fragment frag

            // DotsInstancingOptions: <None>
            // HybridV1InjectedBuiltinProperties: <None>

            // Keywords
            #pragma multi_compile _ LIGHTMAP_ON
        #pragma multi_compile _ DIRLIGHTMAP_COMBINED
        #pragma shader_feature _ _SAMPLE_GI
            #pragma multi_compile _ _SHADOWS_SOFT
        #pragma multi_compile _ _ADDITIONAL_LIGHTS
        #pragma shader_feature _ CEL_ACCENT_ON
        #pragma multi_compile_local _ CEL_SPECULAR_ON



            // Defines
            #define _SURFACE_TYPE_TRANSPARENT 1
            #define ATTRIBUTES_NEED_NORMAL
            #define ATTRIBUTES_NEED_TANGENT
            #define ATTRIBUTES_NEED_TEXCOORD0
            #define VARYINGS_NEED_POSITION_WS
            #define VARYINGS_NEED_NORMAL_WS
            #define VARYINGS_NEED_TANGENT_WS
            #define VARYINGS_NEED_TEXCOORD0
            #define VARYINGS_NEED_VIEWDIRECTION_WS
            #define FEATURES_GRAPH_VERTEX
            /* WARNING: $splice Could not find named fragment 'PassInstancing' */
            #define SHADERPASS SHADERPASS_UNLIT
        #define REQUIRE_OPAQUE_TEXTURE
            /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */

            // Includes
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"

            // --------------------------------------------------
            // Structs and Packing

            struct Attributes
        {
            float3 positionOS : POSITION;
            float3 normalOS : NORMAL;
            float4 tangentOS : TANGENT;
            float4 uv0 : TEXCOORD0;
            #if UNITY_ANY_INSTANCING_ENABLED
            uint instanceID : INSTANCEID_SEMANTIC;
            #endif
        };
        struct Varyings
        {
            float4 positionCS : SV_POSITION;
            float3 positionWS;
            float3 normalWS;
            float4 tangentWS;
            float4 texCoord0;
            float3 viewDirectionWS;
            #if UNITY_ANY_INSTANCING_ENABLED
            uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        struct SurfaceDescriptionInputs
        {
            float3 WorldSpaceNormal;
            float3 WorldSpaceTangent;
            float3 WorldSpaceBiTangent;
            float3 WorldSpaceViewDirection;
            float3 WorldSpacePosition;
            float3 AbsoluteWorldSpacePosition;
            float4 ScreenPosition;
            float4 uv0;
        };
        struct VertexDescriptionInputs
        {
            float3 ObjectSpaceNormal;
            float3 ObjectSpaceTangent;
            float3 ObjectSpacePosition;
        };
        struct PackedVaryings
        {
            float4 positionCS : SV_POSITION;
            float3 interp0 : TEXCOORD0;
            float3 interp1 : TEXCOORD1;
            float4 interp2 : TEXCOORD2;
            float4 interp3 : TEXCOORD3;
            float3 interp4 : TEXCOORD4;
            #if UNITY_ANY_INSTANCING_ENABLED
            uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };

            PackedVaryings PackVaryings (Varyings input)
        {
            PackedVaryings output;
            output.positionCS = input.positionCS;
            output.interp0.xyz =  input.positionWS;
            output.interp1.xyz =  input.normalWS;
            output.interp2.xyzw =  input.tangentWS;
            output.interp3.xyzw =  input.texCoord0;
            output.interp4.xyz =  input.viewDirectionWS;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        Varyings UnpackVaryings (PackedVaryings input)
        {
            Varyings output;
            output.positionCS = input.positionCS;
            output.positionWS = input.interp0.xyz;
            output.normalWS = input.interp1.xyz;
            output.tangentWS = input.interp2.xyzw;
            output.texCoord0 = input.interp3.xyzw;
            output.viewDirectionWS = input.interp4.xyz;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }

            // --------------------------------------------------
            // Graph

            // Graph Properties
            CBUFFER_START(UnityPerMaterial)
        float4 _CelShading_6ae06c46a72c4ceab41183f7f9e2e422_Texture2D9666dd2f2f8f4fddb6c2a5c61ddfa9bd_3072536305_TexelSize;
        float4 Texture2D_3c75bcc60ecc4d7485718a7ecd47a5d1_TexelSize;
        float4 Texture2D_4f60ab3d73c44afeb98ad3eed5ede20e_TexelSize;
        float Vector1_981b8d57da164c75ac12775b5d189f10;
        float Vector1_dbffe0c7baa44a9b8467243fa97e9b01;
        CBUFFER_END

        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(_CelShading_6ae06c46a72c4ceab41183f7f9e2e422_Texture2D9666dd2f2f8f4fddb6c2a5c61ddfa9bd_3072536305);
        SAMPLER(sampler_CelShading_6ae06c46a72c4ceab41183f7f9e2e422_Texture2D9666dd2f2f8f4fddb6c2a5c61ddfa9bd_3072536305);
        TEXTURE2D(Texture2D_3c75bcc60ecc4d7485718a7ecd47a5d1);
        SAMPLER(samplerTexture2D_3c75bcc60ecc4d7485718a7ecd47a5d1);
        TEXTURE2D(Texture2D_4f60ab3d73c44afeb98ad3eed5ede20e);
        SAMPLER(samplerTexture2D_4f60ab3d73c44afeb98ad3eed5ede20e);

            // Graph Functions
            
        void Unity_NormalStrength_float(float3 In, float Strength, out float3 Out)
        {
            Out = float3(In.rg * Strength, lerp(1, In.b, saturate(Strength)));
        }

        void Unity_NormalBlend_float(float3 A, float3 B, out float3 Out)
        {
            Out = SafeNormalize(float3(A.rg + B.rg, A.b * B.b));
        }

        void Refract_float(float3 viewDirectionWS, float3 normalWS, float IOR, out float3 Result){
            Result = refract(viewDirectionWS, normalWS, IOR);
        }

        void Unity_Add_float3(float3 A, float3 B, out float3 Out)
        {
            Out = A + B;
        }

        void Unity_SceneColor_float(float4 UV, out float3 Out)
        {
            Out = SHADERGRAPH_SAMPLE_SCENE_COLOR(UV.xy);
        }

        struct Bindings_Refract_26c8a0f8188165546bb953f6b76e6aa5
        {
            float3 WorldSpaceNormal;
            float3 WorldSpaceTangent;
            float3 WorldSpaceBiTangent;
            float3 WorldSpaceViewDirection;
            float4 ScreenPosition;
        };

        void SG_Refract_26c8a0f8188165546bb953f6b76e6aa5(float3 Vector3_bd748ae7e86942d19e55561595d3929d, float Vector1_7ef7cd04e0624bb4a861106f705da30a, float Vector1_5d8545c9e2ea4717b10008b3e5399441, Bindings_Refract_26c8a0f8188165546bb953f6b76e6aa5 IN, out float3 New_0)
        {
            float3 _Property_2600564299d1489caa3247b009356fc4_Out_0 = Vector3_bd748ae7e86942d19e55561595d3929d;
            float _Property_7ceb734ce8604d67941ebf9073fbcdf7_Out_0 = Vector1_7ef7cd04e0624bb4a861106f705da30a;
            float3 _NormalStrength_64a4c56c014d458faac97d8c739e3c41_Out_2;
            Unity_NormalStrength_float(_Property_2600564299d1489caa3247b009356fc4_Out_0, _Property_7ceb734ce8604d67941ebf9073fbcdf7_Out_0, _NormalStrength_64a4c56c014d458faac97d8c739e3c41_Out_2);
            float3 _NormalBlend_335e1359764a46b2903ad655cfd769bf_Out_2;
            Unity_NormalBlend_float(IN.WorldSpaceNormal, _NormalStrength_64a4c56c014d458faac97d8c739e3c41_Out_2, _NormalBlend_335e1359764a46b2903ad655cfd769bf_Out_2);
            float _Property_987fed52f0d045b096b6a6e514f88fe4_Out_0 = Vector1_5d8545c9e2ea4717b10008b3e5399441;
            float3 _RefractCustomFunction_ff17cdb18fb04835a7dc53c0167c2016_Result_3;
            Refract_float(IN.WorldSpaceViewDirection, _NormalBlend_335e1359764a46b2903ad655cfd769bf_Out_2, _Property_987fed52f0d045b096b6a6e514f88fe4_Out_0, _RefractCustomFunction_ff17cdb18fb04835a7dc53c0167c2016_Result_3);
            float3x3 Transform_23bebe0f55fb4237988de7818efeea31_tangentTransform_AbsoluteWorld = float3x3(IN.WorldSpaceTangent, IN.WorldSpaceBiTangent, IN.WorldSpaceNormal);
            float3 _Transform_23bebe0f55fb4237988de7818efeea31_Out_1 = TransformWorldToTangent(GetCameraRelativePositionWS(_RefractCustomFunction_ff17cdb18fb04835a7dc53c0167c2016_Result_3.xyz), Transform_23bebe0f55fb4237988de7818efeea31_tangentTransform_AbsoluteWorld);
            float4 _ScreenPosition_e67aaacfd47b4000bd9b24b6cd8feb17_Out_0 = float4(IN.ScreenPosition.xy / IN.ScreenPosition.w, 0, 0);
            float3 _Add_75b7eb54210c4fb59148299f30703a93_Out_2;
            Unity_Add_float3(_Transform_23bebe0f55fb4237988de7818efeea31_Out_1, (_ScreenPosition_e67aaacfd47b4000bd9b24b6cd8feb17_Out_0.xyz), _Add_75b7eb54210c4fb59148299f30703a93_Out_2);
            float3 _SceneColor_e0b24c9aca1e45d68d7e8807e63ca4a2_Out_1;
            Unity_SceneColor_float((float4(_Add_75b7eb54210c4fb59148299f30703a93_Out_2, 1.0)), _SceneColor_e0b24c9aca1e45d68d7e8807e63ca4a2_Out_1);
            New_0 = _SceneColor_e0b24c9aca1e45d68d7e8807e63ca4a2_Out_1;
        }

        void Unity_Multiply_float(float3 A, float3 B, out float3 Out)
        {
            Out = A * B;
        }

        void Unity_Normalize_float3(float3 In, out float3 Out)
        {
            Out = normalize(In);
        }

        // 9d45c41649cfe89fdaaa688721614302
        #include "Assets/Rendering/Shaders/Functions/ShaderGraphCel.hlsl"

        struct Bindings_CelShading_802007f66a3e13d42802a3875ed35370
        {
            float3 WorldSpaceNormal;
            float3 WorldSpaceTangent;
            float3 WorldSpaceBiTangent;
            float3 WorldSpaceViewDirection;
            float3 AbsoluteWorldSpacePosition;
            half4 uv0;
        };

        void SG_CelShading_802007f66a3e13d42802a3875ed35370(float4 Color_a950a8e798b94a5aade2710e3fcfde01, float3 Vector3_26519f981d2f4fc0957e66b16a8bc951, float Vector1_d968c91d28bd470db1395895e4bfcfad, float Vector1_ba2b8374bf944684af420d888351f6bd, UnityTexture2D Texture2D_9666dd2f2f8f4fddb6c2a5c61ddfa9bd, Bindings_CelShading_802007f66a3e13d42802a3875ed35370 IN, out half4 Color_1)
        {
            float4 _Property_61f5588b2cd94abebf90def44b873f56_Out_0 = Color_a950a8e798b94a5aade2710e3fcfde01;
            float3 _Property_b297331139c44f59939aac58082c32cd_Out_0 = Vector3_26519f981d2f4fc0957e66b16a8bc951;
            float3x3 Transform_1e7abfa1ecca4936a05f1180040b4744_transposeTangent = transpose(float3x3(IN.WorldSpaceTangent, IN.WorldSpaceBiTangent, IN.WorldSpaceNormal));
            float3 _Transform_1e7abfa1ecca4936a05f1180040b4744_Out_1 = normalize(mul(Transform_1e7abfa1ecca4936a05f1180040b4744_transposeTangent, _Property_b297331139c44f59939aac58082c32cd_Out_0.xyz).xyz);
            float3 _Add_29b1cebd4c974883b111230a3e9dae3f_Out_2;
            Unity_Add_float3(IN.WorldSpaceNormal, _Transform_1e7abfa1ecca4936a05f1180040b4744_Out_1, _Add_29b1cebd4c974883b111230a3e9dae3f_Out_2);
            float3 _Normalize_8a60e1ba11d148d0b782a9efe42f72aa_Out_1;
            Unity_Normalize_float3(_Add_29b1cebd4c974883b111230a3e9dae3f_Out_2, _Normalize_8a60e1ba11d148d0b782a9efe42f72aa_Out_1);
            float _Property_44bd7e8a332240ee8c38a4305a964dbd_Out_0 = Vector1_d968c91d28bd470db1395895e4bfcfad;
            float _Property_211f6a1766924485a76fa34a66c38d04_Out_0 = Vector1_ba2b8374bf944684af420d888351f6bd;
            UnityTexture2D _Property_db331177928b412e83c43f772fd66a68_Out_0 = Texture2D_9666dd2f2f8f4fddb6c2a5c61ddfa9bd;
            float4 _SampleTexture2D_c62c901d5b5b4a6387ce31eba0df3ce0_RGBA_0 = SAMPLE_TEXTURE2D(_Property_db331177928b412e83c43f772fd66a68_Out_0.tex, _Property_db331177928b412e83c43f772fd66a68_Out_0.samplerstate, IN.uv0.xy);
            float _SampleTexture2D_c62c901d5b5b4a6387ce31eba0df3ce0_R_4 = _SampleTexture2D_c62c901d5b5b4a6387ce31eba0df3ce0_RGBA_0.r;
            float _SampleTexture2D_c62c901d5b5b4a6387ce31eba0df3ce0_G_5 = _SampleTexture2D_c62c901d5b5b4a6387ce31eba0df3ce0_RGBA_0.g;
            float _SampleTexture2D_c62c901d5b5b4a6387ce31eba0df3ce0_B_6 = _SampleTexture2D_c62c901d5b5b4a6387ce31eba0df3ce0_RGBA_0.b;
            float _SampleTexture2D_c62c901d5b5b4a6387ce31eba0df3ce0_A_7 = _SampleTexture2D_c62c901d5b5b4a6387ce31eba0df3ce0_RGBA_0.a;
            half4 _CelLightingCustomFunction_2513510943474503a09a9bf6a58817e3_CelShaded_4;
            CelLighting_half(_Property_61f5588b2cd94abebf90def44b873f56_Out_0, IN.AbsoluteWorldSpacePosition, IN.WorldSpaceViewDirection, _Normalize_8a60e1ba11d148d0b782a9efe42f72aa_Out_1, _Property_44bd7e8a332240ee8c38a4305a964dbd_Out_0, _Property_211f6a1766924485a76fa34a66c38d04_Out_0, (_SampleTexture2D_c62c901d5b5b4a6387ce31eba0df3ce0_RGBA_0.xyz), _CelLightingCustomFunction_2513510943474503a09a9bf6a58817e3_CelShaded_4);
            Color_1 = _CelLightingCustomFunction_2513510943474503a09a9bf6a58817e3_CelShaded_4;
        }

            // Graph Vertex
            struct VertexDescription
        {
            float3 Position;
            float3 Normal;
            float3 Tangent;
        };

        VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
        {
            VertexDescription description = (VertexDescription)0;
            description.Position = IN.ObjectSpacePosition;
            description.Normal = IN.ObjectSpaceNormal;
            description.Tangent = IN.ObjectSpaceTangent;
            return description;
        }

            // Graph Pixel
            struct SurfaceDescription
        {
            float3 BaseColor;
            float Alpha;
        };

        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            UnityTexture2D _Property_bf5b6d4b643f47d3a7d0a5e6a611a17d_Out_0 = UnityBuildTexture2DStructNoScale(Texture2D_3c75bcc60ecc4d7485718a7ecd47a5d1);
            float4 _SampleTexture2D_eda2a7e276c94ea5a28ebe4d4c303455_RGBA_0 = SAMPLE_TEXTURE2D(_Property_bf5b6d4b643f47d3a7d0a5e6a611a17d_Out_0.tex, _Property_bf5b6d4b643f47d3a7d0a5e6a611a17d_Out_0.samplerstate, IN.uv0.xy);
            float _SampleTexture2D_eda2a7e276c94ea5a28ebe4d4c303455_R_4 = _SampleTexture2D_eda2a7e276c94ea5a28ebe4d4c303455_RGBA_0.r;
            float _SampleTexture2D_eda2a7e276c94ea5a28ebe4d4c303455_G_5 = _SampleTexture2D_eda2a7e276c94ea5a28ebe4d4c303455_RGBA_0.g;
            float _SampleTexture2D_eda2a7e276c94ea5a28ebe4d4c303455_B_6 = _SampleTexture2D_eda2a7e276c94ea5a28ebe4d4c303455_RGBA_0.b;
            float _SampleTexture2D_eda2a7e276c94ea5a28ebe4d4c303455_A_7 = _SampleTexture2D_eda2a7e276c94ea5a28ebe4d4c303455_RGBA_0.a;
            UnityTexture2D _Property_3172601f6c4e4049840cb124c8deade8_Out_0 = UnityBuildTexture2DStructNoScale(Texture2D_4f60ab3d73c44afeb98ad3eed5ede20e);
            float4 _SampleTexture2D_459aa756f0964b4c9ac9b71a3b343b71_RGBA_0 = SAMPLE_TEXTURE2D(_Property_3172601f6c4e4049840cb124c8deade8_Out_0.tex, _Property_3172601f6c4e4049840cb124c8deade8_Out_0.samplerstate, IN.uv0.xy);
            _SampleTexture2D_459aa756f0964b4c9ac9b71a3b343b71_RGBA_0.rgb = UnpackNormal(_SampleTexture2D_459aa756f0964b4c9ac9b71a3b343b71_RGBA_0);
            float _SampleTexture2D_459aa756f0964b4c9ac9b71a3b343b71_R_4 = _SampleTexture2D_459aa756f0964b4c9ac9b71a3b343b71_RGBA_0.r;
            float _SampleTexture2D_459aa756f0964b4c9ac9b71a3b343b71_G_5 = _SampleTexture2D_459aa756f0964b4c9ac9b71a3b343b71_RGBA_0.g;
            float _SampleTexture2D_459aa756f0964b4c9ac9b71a3b343b71_B_6 = _SampleTexture2D_459aa756f0964b4c9ac9b71a3b343b71_RGBA_0.b;
            float _SampleTexture2D_459aa756f0964b4c9ac9b71a3b343b71_A_7 = _SampleTexture2D_459aa756f0964b4c9ac9b71a3b343b71_RGBA_0.a;
            float _Property_157b37790e6c4f56988772a691bec097_Out_0 = Vector1_dbffe0c7baa44a9b8467243fa97e9b01;
            float _Property_6ca9f76e022c43c4b19899ee52fb4da1_Out_0 = Vector1_981b8d57da164c75ac12775b5d189f10;
            Bindings_Refract_26c8a0f8188165546bb953f6b76e6aa5 _Refract_8be2073ea0044184ab5046c81d992b9c;
            _Refract_8be2073ea0044184ab5046c81d992b9c.WorldSpaceNormal = IN.WorldSpaceNormal;
            _Refract_8be2073ea0044184ab5046c81d992b9c.WorldSpaceTangent = IN.WorldSpaceTangent;
            _Refract_8be2073ea0044184ab5046c81d992b9c.WorldSpaceBiTangent = IN.WorldSpaceBiTangent;
            _Refract_8be2073ea0044184ab5046c81d992b9c.WorldSpaceViewDirection = IN.WorldSpaceViewDirection;
            _Refract_8be2073ea0044184ab5046c81d992b9c.ScreenPosition = IN.ScreenPosition;
            float3 _Refract_8be2073ea0044184ab5046c81d992b9c_New_0;
            SG_Refract_26c8a0f8188165546bb953f6b76e6aa5((_SampleTexture2D_459aa756f0964b4c9ac9b71a3b343b71_RGBA_0.xyz), _Property_157b37790e6c4f56988772a691bec097_Out_0, _Property_6ca9f76e022c43c4b19899ee52fb4da1_Out_0, _Refract_8be2073ea0044184ab5046c81d992b9c, _Refract_8be2073ea0044184ab5046c81d992b9c_New_0);
            float3 _Multiply_a6dba51456cc4601835830353f47999c_Out_2;
            Unity_Multiply_float((_SampleTexture2D_eda2a7e276c94ea5a28ebe4d4c303455_RGBA_0.xyz), _Refract_8be2073ea0044184ab5046c81d992b9c_New_0, _Multiply_a6dba51456cc4601835830353f47999c_Out_2);
            Bindings_CelShading_802007f66a3e13d42802a3875ed35370 _CelShading_6ae06c46a72c4ceab41183f7f9e2e422;
            _CelShading_6ae06c46a72c4ceab41183f7f9e2e422.WorldSpaceNormal = IN.WorldSpaceNormal;
            _CelShading_6ae06c46a72c4ceab41183f7f9e2e422.WorldSpaceTangent = IN.WorldSpaceTangent;
            _CelShading_6ae06c46a72c4ceab41183f7f9e2e422.WorldSpaceBiTangent = IN.WorldSpaceBiTangent;
            _CelShading_6ae06c46a72c4ceab41183f7f9e2e422.WorldSpaceViewDirection = IN.WorldSpaceViewDirection;
            _CelShading_6ae06c46a72c4ceab41183f7f9e2e422.AbsoluteWorldSpacePosition = IN.AbsoluteWorldSpacePosition;
            _CelShading_6ae06c46a72c4ceab41183f7f9e2e422.uv0 = IN.uv0;
            half4 _CelShading_6ae06c46a72c4ceab41183f7f9e2e422_Color_1;
            SG_CelShading_802007f66a3e13d42802a3875ed35370((float4(_Multiply_a6dba51456cc4601835830353f47999c_Out_2, 1.0)), float3 (0, 0, 0), 0, 0, UnityBuildTexture2DStructNoScale(_CelShading_6ae06c46a72c4ceab41183f7f9e2e422_Texture2D9666dd2f2f8f4fddb6c2a5c61ddfa9bd_3072536305), _CelShading_6ae06c46a72c4ceab41183f7f9e2e422, _CelShading_6ae06c46a72c4ceab41183f7f9e2e422_Color_1);
            surface.BaseColor = (_CelShading_6ae06c46a72c4ceab41183f7f9e2e422_Color_1.xyz);
            surface.Alpha = 1;
            return surface;
        }

            // --------------------------------------------------
            // Build Graph Inputs

            VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
        {
            VertexDescriptionInputs output;
            ZERO_INITIALIZE(VertexDescriptionInputs, output);

            output.ObjectSpaceNormal =           input.normalOS;
            output.ObjectSpaceTangent =          input.tangentOS.xyz;
            output.ObjectSpacePosition =         input.positionOS;

            return output;
        }
            SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);

        	// must use interpolated tangent, bitangent and normal before they are normalized in the pixel shader.
        	float3 unnormalizedNormalWS = input.normalWS;
            const float renormFactor = 1.0 / length(unnormalizedNormalWS);

        	// use bitangent on the fly like in hdrp
        	// IMPORTANT! If we ever support Flip on double sided materials ensure bitangent and tangent are NOT flipped.
            float crossSign = (input.tangentWS.w > 0.0 ? 1.0 : -1.0) * GetOddNegativeScale();
        	float3 bitang = crossSign * cross(input.normalWS.xyz, input.tangentWS.xyz);

            output.WorldSpaceNormal =            renormFactor*input.normalWS.xyz;		// we want a unit length Normal Vector node in shader graph

        	// to preserve mikktspace compliance we use same scale renormFactor as was used on the normal.
        	// This is explained in section 2.2 in "surface gradient based bump mapping framework"
            output.WorldSpaceTangent =           renormFactor*input.tangentWS.xyz;
        	output.WorldSpaceBiTangent =         renormFactor*bitang;

            output.WorldSpaceViewDirection =     input.viewDirectionWS; //TODO: by default normalized in HD, but not in universal
            output.WorldSpacePosition =          input.positionWS;
            output.AbsoluteWorldSpacePosition =  GetAbsolutePositionWS(input.positionWS);
            output.ScreenPosition =              ComputeScreenPos(TransformWorldToHClip(input.positionWS), _ProjectionParams.x);
            output.uv0 =                         input.texCoord0;
        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
        #else
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        #endif
        #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN

            return output;
        }

            // --------------------------------------------------
            // Main

            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/UnlitPass.hlsl"

            ENDHLSL
        }
        Pass
        {
            Name "ShadowCaster"
            Tags
            {
                "LightMode" = "ShadowCaster"
            }

            // Render State
            Cull Back
        Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
        ZTest LEqual
        ZWrite On
        ColorMask 0

            // Debug
            // <None>

            // --------------------------------------------------
            // Pass

            HLSLPROGRAM

            // Pragmas
            #pragma target 4.5
        #pragma exclude_renderers gles gles3 glcore
        #pragma multi_compile_instancing
        #pragma multi_compile _ DOTS_INSTANCING_ON
        #pragma vertex vert
        #pragma fragment frag

            // DotsInstancingOptions: <None>
            // HybridV1InjectedBuiltinProperties: <None>

            // Keywords
            // PassKeywords: <None>
            // GraphKeywords: <None>

            // Defines
            #define _SURFACE_TYPE_TRANSPARENT 1
            #define ATTRIBUTES_NEED_NORMAL
            #define ATTRIBUTES_NEED_TANGENT
            #define FEATURES_GRAPH_VERTEX
            /* WARNING: $splice Could not find named fragment 'PassInstancing' */
            #define SHADERPASS SHADERPASS_SHADOWCASTER
            /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */

            // Includes
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"

            // --------------------------------------------------
            // Structs and Packing

            struct Attributes
        {
            float3 positionOS : POSITION;
            float3 normalOS : NORMAL;
            float4 tangentOS : TANGENT;
            #if UNITY_ANY_INSTANCING_ENABLED
            uint instanceID : INSTANCEID_SEMANTIC;
            #endif
        };
        struct Varyings
        {
            float4 positionCS : SV_POSITION;
            #if UNITY_ANY_INSTANCING_ENABLED
            uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        struct SurfaceDescriptionInputs
        {
        };
        struct VertexDescriptionInputs
        {
            float3 ObjectSpaceNormal;
            float3 ObjectSpaceTangent;
            float3 ObjectSpacePosition;
        };
        struct PackedVaryings
        {
            float4 positionCS : SV_POSITION;
            #if UNITY_ANY_INSTANCING_ENABLED
            uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };

            PackedVaryings PackVaryings (Varyings input)
        {
            PackedVaryings output;
            output.positionCS = input.positionCS;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        Varyings UnpackVaryings (PackedVaryings input)
        {
            Varyings output;
            output.positionCS = input.positionCS;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }

            // --------------------------------------------------
            // Graph

            // Graph Properties
            CBUFFER_START(UnityPerMaterial)
        float4 _CelShading_6ae06c46a72c4ceab41183f7f9e2e422_Texture2D9666dd2f2f8f4fddb6c2a5c61ddfa9bd_3072536305_TexelSize;
        float4 Texture2D_3c75bcc60ecc4d7485718a7ecd47a5d1_TexelSize;
        float4 Texture2D_4f60ab3d73c44afeb98ad3eed5ede20e_TexelSize;
        float Vector1_981b8d57da164c75ac12775b5d189f10;
        float Vector1_dbffe0c7baa44a9b8467243fa97e9b01;
        CBUFFER_END

        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(_CelShading_6ae06c46a72c4ceab41183f7f9e2e422_Texture2D9666dd2f2f8f4fddb6c2a5c61ddfa9bd_3072536305);
        SAMPLER(sampler_CelShading_6ae06c46a72c4ceab41183f7f9e2e422_Texture2D9666dd2f2f8f4fddb6c2a5c61ddfa9bd_3072536305);
        TEXTURE2D(Texture2D_3c75bcc60ecc4d7485718a7ecd47a5d1);
        SAMPLER(samplerTexture2D_3c75bcc60ecc4d7485718a7ecd47a5d1);
        TEXTURE2D(Texture2D_4f60ab3d73c44afeb98ad3eed5ede20e);
        SAMPLER(samplerTexture2D_4f60ab3d73c44afeb98ad3eed5ede20e);

            // Graph Functions
            // GraphFunctions: <None>

            // Graph Vertex
            struct VertexDescription
        {
            float3 Position;
            float3 Normal;
            float3 Tangent;
        };

        VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
        {
            VertexDescription description = (VertexDescription)0;
            description.Position = IN.ObjectSpacePosition;
            description.Normal = IN.ObjectSpaceNormal;
            description.Tangent = IN.ObjectSpaceTangent;
            return description;
        }

            // Graph Pixel
            struct SurfaceDescription
        {
            float Alpha;
        };

        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            surface.Alpha = 1;
            return surface;
        }

            // --------------------------------------------------
            // Build Graph Inputs

            VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
        {
            VertexDescriptionInputs output;
            ZERO_INITIALIZE(VertexDescriptionInputs, output);

            output.ObjectSpaceNormal =           input.normalOS;
            output.ObjectSpaceTangent =          input.tangentOS.xyz;
            output.ObjectSpacePosition =         input.positionOS;

            return output;
        }
            SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);





        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
        #else
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        #endif
        #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN

            return output;
        }

            // --------------------------------------------------
            // Main

            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShadowCasterPass.hlsl"

            ENDHLSL
        }
        Pass
        {
            Name "DepthOnly"
            Tags
            {
                "LightMode" = "DepthOnly"
            }

            // Render State
            Cull Back
        Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
        ZTest LEqual
        ZWrite On
        ColorMask 0

            // Debug
            // <None>

            // --------------------------------------------------
            // Pass

            HLSLPROGRAM

            // Pragmas
            #pragma target 4.5
        #pragma exclude_renderers gles gles3 glcore
        #pragma multi_compile_instancing
        #pragma multi_compile _ DOTS_INSTANCING_ON
        #pragma vertex vert
        #pragma fragment frag

            // DotsInstancingOptions: <None>
            // HybridV1InjectedBuiltinProperties: <None>

            // Keywords
            // PassKeywords: <None>
            // GraphKeywords: <None>

            // Defines
            #define _SURFACE_TYPE_TRANSPARENT 1
            #define ATTRIBUTES_NEED_NORMAL
            #define ATTRIBUTES_NEED_TANGENT
            #define FEATURES_GRAPH_VERTEX
            /* WARNING: $splice Could not find named fragment 'PassInstancing' */
            #define SHADERPASS SHADERPASS_DEPTHONLY
            /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */

            // Includes
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"

            // --------------------------------------------------
            // Structs and Packing

            struct Attributes
        {
            float3 positionOS : POSITION;
            float3 normalOS : NORMAL;
            float4 tangentOS : TANGENT;
            #if UNITY_ANY_INSTANCING_ENABLED
            uint instanceID : INSTANCEID_SEMANTIC;
            #endif
        };
        struct Varyings
        {
            float4 positionCS : SV_POSITION;
            #if UNITY_ANY_INSTANCING_ENABLED
            uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        struct SurfaceDescriptionInputs
        {
        };
        struct VertexDescriptionInputs
        {
            float3 ObjectSpaceNormal;
            float3 ObjectSpaceTangent;
            float3 ObjectSpacePosition;
        };
        struct PackedVaryings
        {
            float4 positionCS : SV_POSITION;
            #if UNITY_ANY_INSTANCING_ENABLED
            uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };

            PackedVaryings PackVaryings (Varyings input)
        {
            PackedVaryings output;
            output.positionCS = input.positionCS;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        Varyings UnpackVaryings (PackedVaryings input)
        {
            Varyings output;
            output.positionCS = input.positionCS;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }

            // --------------------------------------------------
            // Graph

            // Graph Properties
            CBUFFER_START(UnityPerMaterial)
        float4 _CelShading_6ae06c46a72c4ceab41183f7f9e2e422_Texture2D9666dd2f2f8f4fddb6c2a5c61ddfa9bd_3072536305_TexelSize;
        float4 Texture2D_3c75bcc60ecc4d7485718a7ecd47a5d1_TexelSize;
        float4 Texture2D_4f60ab3d73c44afeb98ad3eed5ede20e_TexelSize;
        float Vector1_981b8d57da164c75ac12775b5d189f10;
        float Vector1_dbffe0c7baa44a9b8467243fa97e9b01;
        CBUFFER_END

        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(_CelShading_6ae06c46a72c4ceab41183f7f9e2e422_Texture2D9666dd2f2f8f4fddb6c2a5c61ddfa9bd_3072536305);
        SAMPLER(sampler_CelShading_6ae06c46a72c4ceab41183f7f9e2e422_Texture2D9666dd2f2f8f4fddb6c2a5c61ddfa9bd_3072536305);
        TEXTURE2D(Texture2D_3c75bcc60ecc4d7485718a7ecd47a5d1);
        SAMPLER(samplerTexture2D_3c75bcc60ecc4d7485718a7ecd47a5d1);
        TEXTURE2D(Texture2D_4f60ab3d73c44afeb98ad3eed5ede20e);
        SAMPLER(samplerTexture2D_4f60ab3d73c44afeb98ad3eed5ede20e);

            // Graph Functions
            // GraphFunctions: <None>

            // Graph Vertex
            struct VertexDescription
        {
            float3 Position;
            float3 Normal;
            float3 Tangent;
        };

        VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
        {
            VertexDescription description = (VertexDescription)0;
            description.Position = IN.ObjectSpacePosition;
            description.Normal = IN.ObjectSpaceNormal;
            description.Tangent = IN.ObjectSpaceTangent;
            return description;
        }

            // Graph Pixel
            struct SurfaceDescription
        {
            float Alpha;
        };

        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            surface.Alpha = 1;
            return surface;
        }

            // --------------------------------------------------
            // Build Graph Inputs

            VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
        {
            VertexDescriptionInputs output;
            ZERO_INITIALIZE(VertexDescriptionInputs, output);

            output.ObjectSpaceNormal =           input.normalOS;
            output.ObjectSpaceTangent =          input.tangentOS.xyz;
            output.ObjectSpacePosition =         input.positionOS;

            return output;
        }
            SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);





        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
        #else
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        #endif
        #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN

            return output;
        }

            // --------------------------------------------------
            // Main

            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/DepthOnlyPass.hlsl"

            ENDHLSL
        }
    }
    SubShader
    {
        Tags
        {
            "RenderPipeline"="UniversalPipeline"
            "RenderType"="Transparent"
            "UniversalMaterialType" = "Unlit"
            "Queue"="Transparent"
        }
        Pass
        {
            Name "Pass"
            Tags
            {
                // LightMode: <None>
            }

            // Render State
            Cull Back
        Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
        ZTest LEqual
        ZWrite Off

            // Debug
            // <None>

            // --------------------------------------------------
            // Pass

            HLSLPROGRAM

            // Pragmas
            #pragma target 2.0
        #pragma only_renderers gles gles3 glcore d3d11
        #pragma multi_compile_instancing
        #pragma multi_compile_fog
        #pragma vertex vert
        #pragma fragment frag

            // DotsInstancingOptions: <None>
            // HybridV1InjectedBuiltinProperties: <None>

            // Keywords
            #pragma multi_compile _ LIGHTMAP_ON
        #pragma multi_compile _ DIRLIGHTMAP_COMBINED
        #pragma shader_feature _ _SAMPLE_GI
            #pragma multi_compile _ _SHADOWS_SOFT
        #pragma multi_compile _ _ADDITIONAL_LIGHTS
        #pragma shader_feature _ CEL_ACCENT_ON
        #pragma multi_compile_local _ CEL_SPECULAR_ON



            // Defines
            #define _SURFACE_TYPE_TRANSPARENT 1
            #define ATTRIBUTES_NEED_NORMAL
            #define ATTRIBUTES_NEED_TANGENT
            #define ATTRIBUTES_NEED_TEXCOORD0
            #define VARYINGS_NEED_POSITION_WS
            #define VARYINGS_NEED_NORMAL_WS
            #define VARYINGS_NEED_TANGENT_WS
            #define VARYINGS_NEED_TEXCOORD0
            #define VARYINGS_NEED_VIEWDIRECTION_WS
            #define FEATURES_GRAPH_VERTEX
            /* WARNING: $splice Could not find named fragment 'PassInstancing' */
            #define SHADERPASS SHADERPASS_UNLIT
        #define REQUIRE_OPAQUE_TEXTURE
            /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */

            // Includes
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"

            // --------------------------------------------------
            // Structs and Packing

            struct Attributes
        {
            float3 positionOS : POSITION;
            float3 normalOS : NORMAL;
            float4 tangentOS : TANGENT;
            float4 uv0 : TEXCOORD0;
            #if UNITY_ANY_INSTANCING_ENABLED
            uint instanceID : INSTANCEID_SEMANTIC;
            #endif
        };
        struct Varyings
        {
            float4 positionCS : SV_POSITION;
            float3 positionWS;
            float3 normalWS;
            float4 tangentWS;
            float4 texCoord0;
            float3 viewDirectionWS;
            #if UNITY_ANY_INSTANCING_ENABLED
            uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        struct SurfaceDescriptionInputs
        {
            float3 WorldSpaceNormal;
            float3 WorldSpaceTangent;
            float3 WorldSpaceBiTangent;
            float3 WorldSpaceViewDirection;
            float3 WorldSpacePosition;
            float3 AbsoluteWorldSpacePosition;
            float4 ScreenPosition;
            float4 uv0;
        };
        struct VertexDescriptionInputs
        {
            float3 ObjectSpaceNormal;
            float3 ObjectSpaceTangent;
            float3 ObjectSpacePosition;
        };
        struct PackedVaryings
        {
            float4 positionCS : SV_POSITION;
            float3 interp0 : TEXCOORD0;
            float3 interp1 : TEXCOORD1;
            float4 interp2 : TEXCOORD2;
            float4 interp3 : TEXCOORD3;
            float3 interp4 : TEXCOORD4;
            #if UNITY_ANY_INSTANCING_ENABLED
            uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };

            PackedVaryings PackVaryings (Varyings input)
        {
            PackedVaryings output;
            output.positionCS = input.positionCS;
            output.interp0.xyz =  input.positionWS;
            output.interp1.xyz =  input.normalWS;
            output.interp2.xyzw =  input.tangentWS;
            output.interp3.xyzw =  input.texCoord0;
            output.interp4.xyz =  input.viewDirectionWS;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        Varyings UnpackVaryings (PackedVaryings input)
        {
            Varyings output;
            output.positionCS = input.positionCS;
            output.positionWS = input.interp0.xyz;
            output.normalWS = input.interp1.xyz;
            output.tangentWS = input.interp2.xyzw;
            output.texCoord0 = input.interp3.xyzw;
            output.viewDirectionWS = input.interp4.xyz;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }

            // --------------------------------------------------
            // Graph

            // Graph Properties
            CBUFFER_START(UnityPerMaterial)
        float4 _CelShading_6ae06c46a72c4ceab41183f7f9e2e422_Texture2D9666dd2f2f8f4fddb6c2a5c61ddfa9bd_3072536305_TexelSize;
        float4 Texture2D_3c75bcc60ecc4d7485718a7ecd47a5d1_TexelSize;
        float4 Texture2D_4f60ab3d73c44afeb98ad3eed5ede20e_TexelSize;
        float Vector1_981b8d57da164c75ac12775b5d189f10;
        float Vector1_dbffe0c7baa44a9b8467243fa97e9b01;
        CBUFFER_END

        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(_CelShading_6ae06c46a72c4ceab41183f7f9e2e422_Texture2D9666dd2f2f8f4fddb6c2a5c61ddfa9bd_3072536305);
        SAMPLER(sampler_CelShading_6ae06c46a72c4ceab41183f7f9e2e422_Texture2D9666dd2f2f8f4fddb6c2a5c61ddfa9bd_3072536305);
        TEXTURE2D(Texture2D_3c75bcc60ecc4d7485718a7ecd47a5d1);
        SAMPLER(samplerTexture2D_3c75bcc60ecc4d7485718a7ecd47a5d1);
        TEXTURE2D(Texture2D_4f60ab3d73c44afeb98ad3eed5ede20e);
        SAMPLER(samplerTexture2D_4f60ab3d73c44afeb98ad3eed5ede20e);

            // Graph Functions
            
        void Unity_NormalStrength_float(float3 In, float Strength, out float3 Out)
        {
            Out = float3(In.rg * Strength, lerp(1, In.b, saturate(Strength)));
        }

        void Unity_NormalBlend_float(float3 A, float3 B, out float3 Out)
        {
            Out = SafeNormalize(float3(A.rg + B.rg, A.b * B.b));
        }

        void Refract_float(float3 viewDirectionWS, float3 normalWS, float IOR, out float3 Result){
            Result = refract(viewDirectionWS, normalWS, IOR);
        }

        void Unity_Add_float3(float3 A, float3 B, out float3 Out)
        {
            Out = A + B;
        }

        void Unity_SceneColor_float(float4 UV, out float3 Out)
        {
            Out = SHADERGRAPH_SAMPLE_SCENE_COLOR(UV.xy);
        }

        struct Bindings_Refract_26c8a0f8188165546bb953f6b76e6aa5
        {
            float3 WorldSpaceNormal;
            float3 WorldSpaceTangent;
            float3 WorldSpaceBiTangent;
            float3 WorldSpaceViewDirection;
            float4 ScreenPosition;
        };

        void SG_Refract_26c8a0f8188165546bb953f6b76e6aa5(float3 Vector3_bd748ae7e86942d19e55561595d3929d, float Vector1_7ef7cd04e0624bb4a861106f705da30a, float Vector1_5d8545c9e2ea4717b10008b3e5399441, Bindings_Refract_26c8a0f8188165546bb953f6b76e6aa5 IN, out float3 New_0)
        {
            float3 _Property_2600564299d1489caa3247b009356fc4_Out_0 = Vector3_bd748ae7e86942d19e55561595d3929d;
            float _Property_7ceb734ce8604d67941ebf9073fbcdf7_Out_0 = Vector1_7ef7cd04e0624bb4a861106f705da30a;
            float3 _NormalStrength_64a4c56c014d458faac97d8c739e3c41_Out_2;
            Unity_NormalStrength_float(_Property_2600564299d1489caa3247b009356fc4_Out_0, _Property_7ceb734ce8604d67941ebf9073fbcdf7_Out_0, _NormalStrength_64a4c56c014d458faac97d8c739e3c41_Out_2);
            float3 _NormalBlend_335e1359764a46b2903ad655cfd769bf_Out_2;
            Unity_NormalBlend_float(IN.WorldSpaceNormal, _NormalStrength_64a4c56c014d458faac97d8c739e3c41_Out_2, _NormalBlend_335e1359764a46b2903ad655cfd769bf_Out_2);
            float _Property_987fed52f0d045b096b6a6e514f88fe4_Out_0 = Vector1_5d8545c9e2ea4717b10008b3e5399441;
            float3 _RefractCustomFunction_ff17cdb18fb04835a7dc53c0167c2016_Result_3;
            Refract_float(IN.WorldSpaceViewDirection, _NormalBlend_335e1359764a46b2903ad655cfd769bf_Out_2, _Property_987fed52f0d045b096b6a6e514f88fe4_Out_0, _RefractCustomFunction_ff17cdb18fb04835a7dc53c0167c2016_Result_3);
            float3x3 Transform_23bebe0f55fb4237988de7818efeea31_tangentTransform_AbsoluteWorld = float3x3(IN.WorldSpaceTangent, IN.WorldSpaceBiTangent, IN.WorldSpaceNormal);
            float3 _Transform_23bebe0f55fb4237988de7818efeea31_Out_1 = TransformWorldToTangent(GetCameraRelativePositionWS(_RefractCustomFunction_ff17cdb18fb04835a7dc53c0167c2016_Result_3.xyz), Transform_23bebe0f55fb4237988de7818efeea31_tangentTransform_AbsoluteWorld);
            float4 _ScreenPosition_e67aaacfd47b4000bd9b24b6cd8feb17_Out_0 = float4(IN.ScreenPosition.xy / IN.ScreenPosition.w, 0, 0);
            float3 _Add_75b7eb54210c4fb59148299f30703a93_Out_2;
            Unity_Add_float3(_Transform_23bebe0f55fb4237988de7818efeea31_Out_1, (_ScreenPosition_e67aaacfd47b4000bd9b24b6cd8feb17_Out_0.xyz), _Add_75b7eb54210c4fb59148299f30703a93_Out_2);
            float3 _SceneColor_e0b24c9aca1e45d68d7e8807e63ca4a2_Out_1;
            Unity_SceneColor_float((float4(_Add_75b7eb54210c4fb59148299f30703a93_Out_2, 1.0)), _SceneColor_e0b24c9aca1e45d68d7e8807e63ca4a2_Out_1);
            New_0 = _SceneColor_e0b24c9aca1e45d68d7e8807e63ca4a2_Out_1;
        }

        void Unity_Multiply_float(float3 A, float3 B, out float3 Out)
        {
            Out = A * B;
        }

        void Unity_Normalize_float3(float3 In, out float3 Out)
        {
            Out = normalize(In);
        }

        // 9d45c41649cfe89fdaaa688721614302
        #include "Assets/Rendering/Shaders/Functions/ShaderGraphCel.hlsl"

        struct Bindings_CelShading_802007f66a3e13d42802a3875ed35370
        {
            float3 WorldSpaceNormal;
            float3 WorldSpaceTangent;
            float3 WorldSpaceBiTangent;
            float3 WorldSpaceViewDirection;
            float3 AbsoluteWorldSpacePosition;
            half4 uv0;
        };

        void SG_CelShading_802007f66a3e13d42802a3875ed35370(float4 Color_a950a8e798b94a5aade2710e3fcfde01, float3 Vector3_26519f981d2f4fc0957e66b16a8bc951, float Vector1_d968c91d28bd470db1395895e4bfcfad, float Vector1_ba2b8374bf944684af420d888351f6bd, UnityTexture2D Texture2D_9666dd2f2f8f4fddb6c2a5c61ddfa9bd, Bindings_CelShading_802007f66a3e13d42802a3875ed35370 IN, out half4 Color_1)
        {
            float4 _Property_61f5588b2cd94abebf90def44b873f56_Out_0 = Color_a950a8e798b94a5aade2710e3fcfde01;
            float3 _Property_b297331139c44f59939aac58082c32cd_Out_0 = Vector3_26519f981d2f4fc0957e66b16a8bc951;
            float3x3 Transform_1e7abfa1ecca4936a05f1180040b4744_transposeTangent = transpose(float3x3(IN.WorldSpaceTangent, IN.WorldSpaceBiTangent, IN.WorldSpaceNormal));
            float3 _Transform_1e7abfa1ecca4936a05f1180040b4744_Out_1 = normalize(mul(Transform_1e7abfa1ecca4936a05f1180040b4744_transposeTangent, _Property_b297331139c44f59939aac58082c32cd_Out_0.xyz).xyz);
            float3 _Add_29b1cebd4c974883b111230a3e9dae3f_Out_2;
            Unity_Add_float3(IN.WorldSpaceNormal, _Transform_1e7abfa1ecca4936a05f1180040b4744_Out_1, _Add_29b1cebd4c974883b111230a3e9dae3f_Out_2);
            float3 _Normalize_8a60e1ba11d148d0b782a9efe42f72aa_Out_1;
            Unity_Normalize_float3(_Add_29b1cebd4c974883b111230a3e9dae3f_Out_2, _Normalize_8a60e1ba11d148d0b782a9efe42f72aa_Out_1);
            float _Property_44bd7e8a332240ee8c38a4305a964dbd_Out_0 = Vector1_d968c91d28bd470db1395895e4bfcfad;
            float _Property_211f6a1766924485a76fa34a66c38d04_Out_0 = Vector1_ba2b8374bf944684af420d888351f6bd;
            UnityTexture2D _Property_db331177928b412e83c43f772fd66a68_Out_0 = Texture2D_9666dd2f2f8f4fddb6c2a5c61ddfa9bd;
            float4 _SampleTexture2D_c62c901d5b5b4a6387ce31eba0df3ce0_RGBA_0 = SAMPLE_TEXTURE2D(_Property_db331177928b412e83c43f772fd66a68_Out_0.tex, _Property_db331177928b412e83c43f772fd66a68_Out_0.samplerstate, IN.uv0.xy);
            float _SampleTexture2D_c62c901d5b5b4a6387ce31eba0df3ce0_R_4 = _SampleTexture2D_c62c901d5b5b4a6387ce31eba0df3ce0_RGBA_0.r;
            float _SampleTexture2D_c62c901d5b5b4a6387ce31eba0df3ce0_G_5 = _SampleTexture2D_c62c901d5b5b4a6387ce31eba0df3ce0_RGBA_0.g;
            float _SampleTexture2D_c62c901d5b5b4a6387ce31eba0df3ce0_B_6 = _SampleTexture2D_c62c901d5b5b4a6387ce31eba0df3ce0_RGBA_0.b;
            float _SampleTexture2D_c62c901d5b5b4a6387ce31eba0df3ce0_A_7 = _SampleTexture2D_c62c901d5b5b4a6387ce31eba0df3ce0_RGBA_0.a;
            half4 _CelLightingCustomFunction_2513510943474503a09a9bf6a58817e3_CelShaded_4;
            CelLighting_half(_Property_61f5588b2cd94abebf90def44b873f56_Out_0, IN.AbsoluteWorldSpacePosition, IN.WorldSpaceViewDirection, _Normalize_8a60e1ba11d148d0b782a9efe42f72aa_Out_1, _Property_44bd7e8a332240ee8c38a4305a964dbd_Out_0, _Property_211f6a1766924485a76fa34a66c38d04_Out_0, (_SampleTexture2D_c62c901d5b5b4a6387ce31eba0df3ce0_RGBA_0.xyz), _CelLightingCustomFunction_2513510943474503a09a9bf6a58817e3_CelShaded_4);
            Color_1 = _CelLightingCustomFunction_2513510943474503a09a9bf6a58817e3_CelShaded_4;
        }

            // Graph Vertex
            struct VertexDescription
        {
            float3 Position;
            float3 Normal;
            float3 Tangent;
        };

        VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
        {
            VertexDescription description = (VertexDescription)0;
            description.Position = IN.ObjectSpacePosition;
            description.Normal = IN.ObjectSpaceNormal;
            description.Tangent = IN.ObjectSpaceTangent;
            return description;
        }

            // Graph Pixel
            struct SurfaceDescription
        {
            float3 BaseColor;
            float Alpha;
        };

        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            UnityTexture2D _Property_bf5b6d4b643f47d3a7d0a5e6a611a17d_Out_0 = UnityBuildTexture2DStructNoScale(Texture2D_3c75bcc60ecc4d7485718a7ecd47a5d1);
            float4 _SampleTexture2D_eda2a7e276c94ea5a28ebe4d4c303455_RGBA_0 = SAMPLE_TEXTURE2D(_Property_bf5b6d4b643f47d3a7d0a5e6a611a17d_Out_0.tex, _Property_bf5b6d4b643f47d3a7d0a5e6a611a17d_Out_0.samplerstate, IN.uv0.xy);
            float _SampleTexture2D_eda2a7e276c94ea5a28ebe4d4c303455_R_4 = _SampleTexture2D_eda2a7e276c94ea5a28ebe4d4c303455_RGBA_0.r;
            float _SampleTexture2D_eda2a7e276c94ea5a28ebe4d4c303455_G_5 = _SampleTexture2D_eda2a7e276c94ea5a28ebe4d4c303455_RGBA_0.g;
            float _SampleTexture2D_eda2a7e276c94ea5a28ebe4d4c303455_B_6 = _SampleTexture2D_eda2a7e276c94ea5a28ebe4d4c303455_RGBA_0.b;
            float _SampleTexture2D_eda2a7e276c94ea5a28ebe4d4c303455_A_7 = _SampleTexture2D_eda2a7e276c94ea5a28ebe4d4c303455_RGBA_0.a;
            UnityTexture2D _Property_3172601f6c4e4049840cb124c8deade8_Out_0 = UnityBuildTexture2DStructNoScale(Texture2D_4f60ab3d73c44afeb98ad3eed5ede20e);
            float4 _SampleTexture2D_459aa756f0964b4c9ac9b71a3b343b71_RGBA_0 = SAMPLE_TEXTURE2D(_Property_3172601f6c4e4049840cb124c8deade8_Out_0.tex, _Property_3172601f6c4e4049840cb124c8deade8_Out_0.samplerstate, IN.uv0.xy);
            _SampleTexture2D_459aa756f0964b4c9ac9b71a3b343b71_RGBA_0.rgb = UnpackNormal(_SampleTexture2D_459aa756f0964b4c9ac9b71a3b343b71_RGBA_0);
            float _SampleTexture2D_459aa756f0964b4c9ac9b71a3b343b71_R_4 = _SampleTexture2D_459aa756f0964b4c9ac9b71a3b343b71_RGBA_0.r;
            float _SampleTexture2D_459aa756f0964b4c9ac9b71a3b343b71_G_5 = _SampleTexture2D_459aa756f0964b4c9ac9b71a3b343b71_RGBA_0.g;
            float _SampleTexture2D_459aa756f0964b4c9ac9b71a3b343b71_B_6 = _SampleTexture2D_459aa756f0964b4c9ac9b71a3b343b71_RGBA_0.b;
            float _SampleTexture2D_459aa756f0964b4c9ac9b71a3b343b71_A_7 = _SampleTexture2D_459aa756f0964b4c9ac9b71a3b343b71_RGBA_0.a;
            float _Property_157b37790e6c4f56988772a691bec097_Out_0 = Vector1_dbffe0c7baa44a9b8467243fa97e9b01;
            float _Property_6ca9f76e022c43c4b19899ee52fb4da1_Out_0 = Vector1_981b8d57da164c75ac12775b5d189f10;
            Bindings_Refract_26c8a0f8188165546bb953f6b76e6aa5 _Refract_8be2073ea0044184ab5046c81d992b9c;
            _Refract_8be2073ea0044184ab5046c81d992b9c.WorldSpaceNormal = IN.WorldSpaceNormal;
            _Refract_8be2073ea0044184ab5046c81d992b9c.WorldSpaceTangent = IN.WorldSpaceTangent;
            _Refract_8be2073ea0044184ab5046c81d992b9c.WorldSpaceBiTangent = IN.WorldSpaceBiTangent;
            _Refract_8be2073ea0044184ab5046c81d992b9c.WorldSpaceViewDirection = IN.WorldSpaceViewDirection;
            _Refract_8be2073ea0044184ab5046c81d992b9c.ScreenPosition = IN.ScreenPosition;
            float3 _Refract_8be2073ea0044184ab5046c81d992b9c_New_0;
            SG_Refract_26c8a0f8188165546bb953f6b76e6aa5((_SampleTexture2D_459aa756f0964b4c9ac9b71a3b343b71_RGBA_0.xyz), _Property_157b37790e6c4f56988772a691bec097_Out_0, _Property_6ca9f76e022c43c4b19899ee52fb4da1_Out_0, _Refract_8be2073ea0044184ab5046c81d992b9c, _Refract_8be2073ea0044184ab5046c81d992b9c_New_0);
            float3 _Multiply_a6dba51456cc4601835830353f47999c_Out_2;
            Unity_Multiply_float((_SampleTexture2D_eda2a7e276c94ea5a28ebe4d4c303455_RGBA_0.xyz), _Refract_8be2073ea0044184ab5046c81d992b9c_New_0, _Multiply_a6dba51456cc4601835830353f47999c_Out_2);
            Bindings_CelShading_802007f66a3e13d42802a3875ed35370 _CelShading_6ae06c46a72c4ceab41183f7f9e2e422;
            _CelShading_6ae06c46a72c4ceab41183f7f9e2e422.WorldSpaceNormal = IN.WorldSpaceNormal;
            _CelShading_6ae06c46a72c4ceab41183f7f9e2e422.WorldSpaceTangent = IN.WorldSpaceTangent;
            _CelShading_6ae06c46a72c4ceab41183f7f9e2e422.WorldSpaceBiTangent = IN.WorldSpaceBiTangent;
            _CelShading_6ae06c46a72c4ceab41183f7f9e2e422.WorldSpaceViewDirection = IN.WorldSpaceViewDirection;
            _CelShading_6ae06c46a72c4ceab41183f7f9e2e422.AbsoluteWorldSpacePosition = IN.AbsoluteWorldSpacePosition;
            _CelShading_6ae06c46a72c4ceab41183f7f9e2e422.uv0 = IN.uv0;
            half4 _CelShading_6ae06c46a72c4ceab41183f7f9e2e422_Color_1;
            SG_CelShading_802007f66a3e13d42802a3875ed35370((float4(_Multiply_a6dba51456cc4601835830353f47999c_Out_2, 1.0)), float3 (0, 0, 0), 0, 0, UnityBuildTexture2DStructNoScale(_CelShading_6ae06c46a72c4ceab41183f7f9e2e422_Texture2D9666dd2f2f8f4fddb6c2a5c61ddfa9bd_3072536305), _CelShading_6ae06c46a72c4ceab41183f7f9e2e422, _CelShading_6ae06c46a72c4ceab41183f7f9e2e422_Color_1);
            surface.BaseColor = (_CelShading_6ae06c46a72c4ceab41183f7f9e2e422_Color_1.xyz);
            surface.Alpha = 1;
            return surface;
        }

            // --------------------------------------------------
            // Build Graph Inputs

            VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
        {
            VertexDescriptionInputs output;
            ZERO_INITIALIZE(VertexDescriptionInputs, output);

            output.ObjectSpaceNormal =           input.normalOS;
            output.ObjectSpaceTangent =          input.tangentOS.xyz;
            output.ObjectSpacePosition =         input.positionOS;

            return output;
        }
            SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);

        	// must use interpolated tangent, bitangent and normal before they are normalized in the pixel shader.
        	float3 unnormalizedNormalWS = input.normalWS;
            const float renormFactor = 1.0 / length(unnormalizedNormalWS);

        	// use bitangent on the fly like in hdrp
        	// IMPORTANT! If we ever support Flip on double sided materials ensure bitangent and tangent are NOT flipped.
            float crossSign = (input.tangentWS.w > 0.0 ? 1.0 : -1.0) * GetOddNegativeScale();
        	float3 bitang = crossSign * cross(input.normalWS.xyz, input.tangentWS.xyz);

            output.WorldSpaceNormal =            renormFactor*input.normalWS.xyz;		// we want a unit length Normal Vector node in shader graph

        	// to preserve mikktspace compliance we use same scale renormFactor as was used on the normal.
        	// This is explained in section 2.2 in "surface gradient based bump mapping framework"
            output.WorldSpaceTangent =           renormFactor*input.tangentWS.xyz;
        	output.WorldSpaceBiTangent =         renormFactor*bitang;

            output.WorldSpaceViewDirection =     input.viewDirectionWS; //TODO: by default normalized in HD, but not in universal
            output.WorldSpacePosition =          input.positionWS;
            output.AbsoluteWorldSpacePosition =  GetAbsolutePositionWS(input.positionWS);
            output.ScreenPosition =              ComputeScreenPos(TransformWorldToHClip(input.positionWS), _ProjectionParams.x);
            output.uv0 =                         input.texCoord0;
        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
        #else
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        #endif
        #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN

            return output;
        }

            // --------------------------------------------------
            // Main

            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/UnlitPass.hlsl"

            ENDHLSL
        }
        Pass
        {
            Name "ShadowCaster"
            Tags
            {
                "LightMode" = "ShadowCaster"
            }

            // Render State
            Cull Back
        Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
        ZTest LEqual
        ZWrite On
        ColorMask 0

            // Debug
            // <None>

            // --------------------------------------------------
            // Pass

            HLSLPROGRAM

            // Pragmas
            #pragma target 2.0
        #pragma only_renderers gles gles3 glcore d3d11
        #pragma multi_compile_instancing
        #pragma vertex vert
        #pragma fragment frag

            // DotsInstancingOptions: <None>
            // HybridV1InjectedBuiltinProperties: <None>

            // Keywords
            // PassKeywords: <None>
            // GraphKeywords: <None>

            // Defines
            #define _SURFACE_TYPE_TRANSPARENT 1
            #define ATTRIBUTES_NEED_NORMAL
            #define ATTRIBUTES_NEED_TANGENT
            #define FEATURES_GRAPH_VERTEX
            /* WARNING: $splice Could not find named fragment 'PassInstancing' */
            #define SHADERPASS SHADERPASS_SHADOWCASTER
            /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */

            // Includes
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"

            // --------------------------------------------------
            // Structs and Packing

            struct Attributes
        {
            float3 positionOS : POSITION;
            float3 normalOS : NORMAL;
            float4 tangentOS : TANGENT;
            #if UNITY_ANY_INSTANCING_ENABLED
            uint instanceID : INSTANCEID_SEMANTIC;
            #endif
        };
        struct Varyings
        {
            float4 positionCS : SV_POSITION;
            #if UNITY_ANY_INSTANCING_ENABLED
            uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        struct SurfaceDescriptionInputs
        {
        };
        struct VertexDescriptionInputs
        {
            float3 ObjectSpaceNormal;
            float3 ObjectSpaceTangent;
            float3 ObjectSpacePosition;
        };
        struct PackedVaryings
        {
            float4 positionCS : SV_POSITION;
            #if UNITY_ANY_INSTANCING_ENABLED
            uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };

            PackedVaryings PackVaryings (Varyings input)
        {
            PackedVaryings output;
            output.positionCS = input.positionCS;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        Varyings UnpackVaryings (PackedVaryings input)
        {
            Varyings output;
            output.positionCS = input.positionCS;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }

            // --------------------------------------------------
            // Graph

            // Graph Properties
            CBUFFER_START(UnityPerMaterial)
        float4 _CelShading_6ae06c46a72c4ceab41183f7f9e2e422_Texture2D9666dd2f2f8f4fddb6c2a5c61ddfa9bd_3072536305_TexelSize;
        float4 Texture2D_3c75bcc60ecc4d7485718a7ecd47a5d1_TexelSize;
        float4 Texture2D_4f60ab3d73c44afeb98ad3eed5ede20e_TexelSize;
        float Vector1_981b8d57da164c75ac12775b5d189f10;
        float Vector1_dbffe0c7baa44a9b8467243fa97e9b01;
        CBUFFER_END

        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(_CelShading_6ae06c46a72c4ceab41183f7f9e2e422_Texture2D9666dd2f2f8f4fddb6c2a5c61ddfa9bd_3072536305);
        SAMPLER(sampler_CelShading_6ae06c46a72c4ceab41183f7f9e2e422_Texture2D9666dd2f2f8f4fddb6c2a5c61ddfa9bd_3072536305);
        TEXTURE2D(Texture2D_3c75bcc60ecc4d7485718a7ecd47a5d1);
        SAMPLER(samplerTexture2D_3c75bcc60ecc4d7485718a7ecd47a5d1);
        TEXTURE2D(Texture2D_4f60ab3d73c44afeb98ad3eed5ede20e);
        SAMPLER(samplerTexture2D_4f60ab3d73c44afeb98ad3eed5ede20e);

            // Graph Functions
            // GraphFunctions: <None>

            // Graph Vertex
            struct VertexDescription
        {
            float3 Position;
            float3 Normal;
            float3 Tangent;
        };

        VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
        {
            VertexDescription description = (VertexDescription)0;
            description.Position = IN.ObjectSpacePosition;
            description.Normal = IN.ObjectSpaceNormal;
            description.Tangent = IN.ObjectSpaceTangent;
            return description;
        }

            // Graph Pixel
            struct SurfaceDescription
        {
            float Alpha;
        };

        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            surface.Alpha = 1;
            return surface;
        }

            // --------------------------------------------------
            // Build Graph Inputs

            VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
        {
            VertexDescriptionInputs output;
            ZERO_INITIALIZE(VertexDescriptionInputs, output);

            output.ObjectSpaceNormal =           input.normalOS;
            output.ObjectSpaceTangent =          input.tangentOS.xyz;
            output.ObjectSpacePosition =         input.positionOS;

            return output;
        }
            SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);





        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
        #else
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        #endif
        #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN

            return output;
        }

            // --------------------------------------------------
            // Main

            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShadowCasterPass.hlsl"

            ENDHLSL
        }
        Pass
        {
            Name "DepthOnly"
            Tags
            {
                "LightMode" = "DepthOnly"
            }

            // Render State
            Cull Back
        Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
        ZTest LEqual
        ZWrite On
        ColorMask 0

            // Debug
            // <None>

            // --------------------------------------------------
            // Pass

            HLSLPROGRAM

            // Pragmas
            #pragma target 2.0
        #pragma only_renderers gles gles3 glcore d3d11
        #pragma multi_compile_instancing
        #pragma vertex vert
        #pragma fragment frag

            // DotsInstancingOptions: <None>
            // HybridV1InjectedBuiltinProperties: <None>

            // Keywords
            // PassKeywords: <None>
            // GraphKeywords: <None>

            // Defines
            #define _SURFACE_TYPE_TRANSPARENT 1
            #define ATTRIBUTES_NEED_NORMAL
            #define ATTRIBUTES_NEED_TANGENT
            #define FEATURES_GRAPH_VERTEX
            /* WARNING: $splice Could not find named fragment 'PassInstancing' */
            #define SHADERPASS SHADERPASS_DEPTHONLY
            /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */

            // Includes
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"

            // --------------------------------------------------
            // Structs and Packing

            struct Attributes
        {
            float3 positionOS : POSITION;
            float3 normalOS : NORMAL;
            float4 tangentOS : TANGENT;
            #if UNITY_ANY_INSTANCING_ENABLED
            uint instanceID : INSTANCEID_SEMANTIC;
            #endif
        };
        struct Varyings
        {
            float4 positionCS : SV_POSITION;
            #if UNITY_ANY_INSTANCING_ENABLED
            uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        struct SurfaceDescriptionInputs
        {
        };
        struct VertexDescriptionInputs
        {
            float3 ObjectSpaceNormal;
            float3 ObjectSpaceTangent;
            float3 ObjectSpacePosition;
        };
        struct PackedVaryings
        {
            float4 positionCS : SV_POSITION;
            #if UNITY_ANY_INSTANCING_ENABLED
            uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };

            PackedVaryings PackVaryings (Varyings input)
        {
            PackedVaryings output;
            output.positionCS = input.positionCS;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        Varyings UnpackVaryings (PackedVaryings input)
        {
            Varyings output;
            output.positionCS = input.positionCS;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }

            // --------------------------------------------------
            // Graph

            // Graph Properties
            CBUFFER_START(UnityPerMaterial)
        float4 _CelShading_6ae06c46a72c4ceab41183f7f9e2e422_Texture2D9666dd2f2f8f4fddb6c2a5c61ddfa9bd_3072536305_TexelSize;
        float4 Texture2D_3c75bcc60ecc4d7485718a7ecd47a5d1_TexelSize;
        float4 Texture2D_4f60ab3d73c44afeb98ad3eed5ede20e_TexelSize;
        float Vector1_981b8d57da164c75ac12775b5d189f10;
        float Vector1_dbffe0c7baa44a9b8467243fa97e9b01;
        CBUFFER_END

        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(_CelShading_6ae06c46a72c4ceab41183f7f9e2e422_Texture2D9666dd2f2f8f4fddb6c2a5c61ddfa9bd_3072536305);
        SAMPLER(sampler_CelShading_6ae06c46a72c4ceab41183f7f9e2e422_Texture2D9666dd2f2f8f4fddb6c2a5c61ddfa9bd_3072536305);
        TEXTURE2D(Texture2D_3c75bcc60ecc4d7485718a7ecd47a5d1);
        SAMPLER(samplerTexture2D_3c75bcc60ecc4d7485718a7ecd47a5d1);
        TEXTURE2D(Texture2D_4f60ab3d73c44afeb98ad3eed5ede20e);
        SAMPLER(samplerTexture2D_4f60ab3d73c44afeb98ad3eed5ede20e);

            // Graph Functions
            // GraphFunctions: <None>

            // Graph Vertex
            struct VertexDescription
        {
            float3 Position;
            float3 Normal;
            float3 Tangent;
        };

        VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
        {
            VertexDescription description = (VertexDescription)0;
            description.Position = IN.ObjectSpacePosition;
            description.Normal = IN.ObjectSpaceNormal;
            description.Tangent = IN.ObjectSpaceTangent;
            return description;
        }

            // Graph Pixel
            struct SurfaceDescription
        {
            float Alpha;
        };

        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            surface.Alpha = 1;
            return surface;
        }

            // --------------------------------------------------
            // Build Graph Inputs

            VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
        {
            VertexDescriptionInputs output;
            ZERO_INITIALIZE(VertexDescriptionInputs, output);

            output.ObjectSpaceNormal =           input.normalOS;
            output.ObjectSpaceTangent =          input.tangentOS.xyz;
            output.ObjectSpacePosition =         input.positionOS;

            return output;
        }
            SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);





        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
        #else
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        #endif
        #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN

            return output;
        }

            // --------------------------------------------------
            // Main

            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/DepthOnlyPass.hlsl"

            ENDHLSL
        }
    }
    FallBack "Hidden/Shader Graph/FallbackError"
}