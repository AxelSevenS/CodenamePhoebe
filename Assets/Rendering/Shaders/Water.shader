Shader "CelShaded/Water"
{
    Properties
    {
        Vector1_0e212c6562134ef7add6f6766d6419b4("## Color", Float) = 0
        Color_213be8c364f242cdbabe6ef25c0efbd9("- Deep Color", Color) = (0, 0, 0, 0)
        Color_8b23519dc989478fa19369d8efda52f6("- Shallow Color", Color) = (0, 0, 0, 0)
        Vector1_40661633426347afb1a705a362429b36("## Noise", Float) = 0
        [NoScaleOffset]Texture2D_c773e99a7970488a9d6bcbd08807a081("- Noise Map &", 2D) = "white" {}
        Vector1_45a27133fb8b4113a0746c0d1954c57b("- Noise Scale", Float) = 0
        Vector1_f133b62c64f14525895d4b7d5afdd79f("- Noise Speed", Float) = 0
        Vector1_57cb41fdb69545e8b53a796761acb08d("- Noise Strength", Float) = 0
        Vector1_76a8f3bdc13d493dbc42eca7e6de6c4e("## Normal", Float) = 0
        [NoScaleOffset]Texture2D_c23a14955d09487682029de7801dfc17("- Normal Map &", 2D) = "white" {}
        Vector1_28676355978446f285fbfe6f1a574316("- Normal Scale", Float) = 0
        Vector1_ba885a3188cc490b9d04d880f79cd1b0("- Normal Speed", Float) = 0
        Vector1_bf79c34252fe4364976a552df388436c("- Normal Strength", Float) = 0
        Vector1_f586ab191e3e497491345288306a18c0("## Wave", Float) = 0
        Vector1_4da6e98cbfa5408fb6352b485aa76d05("- Wave Strength", Float) = 0
        Vector1_b266d7e71de2489ea084cb27060cacca("- Wave Speed", Float) = 0
        Vector1_d09cc377b0524610970658ffdacd497f("- Wave Frequency", Float) = 0
        Vector1_1f93deba5516424f927cee1eb2c93ba3("## Miscellaneous", Float) = 0
        Vector1_cc6e37850778480781084b62719d58c3("- Depth Strength", Float) = 0.4
        Vector1_5be3734798f64a63b2827794b79391c3("- Transparency", Range(0, 1)) = 0.07
        Vector1_3578eca6d8ac4314939bfb16bfdbde27("- Smoothness", Float) = 0
        Vector1_775ef292c0cf4cda91aecf4845cd67bd("- Specular Intensity", Float) = 0
        [NonModifiableTextureData][NoScaleOffset]_CelShading_ad7871fb5bf1404e9f1fa53eab938a3a_Texture2D9666dd2f2f8f4fddb6c2a5c61ddfa9bd_3072536305("Texture2D", 2D) = "white" {}
        [HideInInspector][NoScaleOffset]unity_Lightmaps("unity_Lightmaps", 2DArray) = "" {}
        [HideInInspector][NoScaleOffset]unity_LightmapsInd("unity_LightmapsInd", 2DArray) = "" {}
        [HideInInspector][NoScaleOffset]unity_ShadowMasks("unity_ShadowMasks", 2DArray) = "" {}
        [Toggle]CEL_SPECULAR("Specular", Float) = 1
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
            #pragma multi_compile_local _ CEL_SPECULAR_ON
        #pragma multi_compile _ _SHADOWS_SOFT
        #pragma multi_compile _ _ADDITIONAL_LIGHTS
        #pragma shader_feature _ CEL_ACCENT_ON

        #if defined(CEL_SPECULAR_ON)
            #define KEYWORD_PERMUTATION_0
        #else
            #define KEYWORD_PERMUTATION_1
        #endif


            // Defines
        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        #define _SURFACE_TYPE_TRANSPARENT 1
        #endif

        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        #define ATTRIBUTES_NEED_NORMAL
        #endif

        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        #define ATTRIBUTES_NEED_TANGENT
        #endif

        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        #define ATTRIBUTES_NEED_TEXCOORD0
        #endif

        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        #define VARYINGS_NEED_POSITION_WS
        #endif

        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        #define VARYINGS_NEED_NORMAL_WS
        #endif

        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        #define VARYINGS_NEED_TANGENT_WS
        #endif

        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        #define VARYINGS_NEED_TEXCOORD0
        #endif

        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        #define VARYINGS_NEED_VIEWDIRECTION_WS
        #endif

            #define FEATURES_GRAPH_VERTEX
            /* WARNING: $splice Could not find named fragment 'PassInstancing' */
            #define SHADERPASS SHADERPASS_UNLIT
        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        #define REQUIRE_DEPTH_TEXTURE
        #endif
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
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float3 positionOS : POSITION;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float3 normalOS : NORMAL;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float4 tangentOS : TANGENT;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float4 uv0 : TEXCOORD0;
            #endif
            #if UNITY_ANY_INSTANCING_ENABLED
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            uint instanceID : INSTANCEID_SEMANTIC;
            #endif
            #endif
        };
        struct Varyings
        {
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float4 positionCS : SV_POSITION;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float3 positionWS;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float3 normalWS;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float4 tangentWS;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float4 texCoord0;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float3 viewDirectionWS;
            #endif
            #if UNITY_ANY_INSTANCING_ENABLED
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
            #endif
        };
        struct SurfaceDescriptionInputs
        {
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float3 WorldSpaceNormal;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float3 WorldSpaceTangent;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float3 WorldSpaceBiTangent;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float3 WorldSpaceViewDirection;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float3 ObjectSpacePosition;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float3 WorldSpacePosition;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float3 AbsoluteWorldSpacePosition;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float4 ScreenPosition;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float4 uv0;
            #endif
        };
        struct VertexDescriptionInputs
        {
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float3 ObjectSpaceNormal;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float3 WorldSpaceNormal;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float3 ObjectSpaceTangent;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float3 WorldSpaceTangent;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float3 ObjectSpaceBiTangent;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float3 WorldSpaceBiTangent;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float3 ObjectSpacePosition;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float3 AbsoluteWorldSpacePosition;
            #endif
        };
        struct PackedVaryings
        {
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float4 positionCS : SV_POSITION;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float3 interp0 : TEXCOORD0;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float3 interp1 : TEXCOORD1;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float4 interp2 : TEXCOORD2;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float4 interp3 : TEXCOORD3;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float3 interp4 : TEXCOORD4;
            #endif
            #if UNITY_ANY_INSTANCING_ENABLED
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
            #endif
        };

            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
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
        #endif

            // --------------------------------------------------
            // Graph

            // Graph Properties
            CBUFFER_START(UnityPerMaterial)
        float4 _CelShading_ad7871fb5bf1404e9f1fa53eab938a3a_Texture2D9666dd2f2f8f4fddb6c2a5c61ddfa9bd_3072536305_TexelSize;
        float Vector1_0e212c6562134ef7add6f6766d6419b4;
        float4 Color_213be8c364f242cdbabe6ef25c0efbd9;
        float4 Color_8b23519dc989478fa19369d8efda52f6;
        float Vector1_40661633426347afb1a705a362429b36;
        float4 Texture2D_c773e99a7970488a9d6bcbd08807a081_TexelSize;
        float Vector1_45a27133fb8b4113a0746c0d1954c57b;
        float Vector1_f133b62c64f14525895d4b7d5afdd79f;
        float Vector1_57cb41fdb69545e8b53a796761acb08d;
        float Vector1_76a8f3bdc13d493dbc42eca7e6de6c4e;
        float4 Texture2D_c23a14955d09487682029de7801dfc17_TexelSize;
        float Vector1_28676355978446f285fbfe6f1a574316;
        float Vector1_ba885a3188cc490b9d04d880f79cd1b0;
        float Vector1_bf79c34252fe4364976a552df388436c;
        float Vector1_f586ab191e3e497491345288306a18c0;
        float Vector1_4da6e98cbfa5408fb6352b485aa76d05;
        float Vector1_b266d7e71de2489ea084cb27060cacca;
        float Vector1_d09cc377b0524610970658ffdacd497f;
        float Vector1_1f93deba5516424f927cee1eb2c93ba3;
        float Vector1_cc6e37850778480781084b62719d58c3;
        float Vector1_5be3734798f64a63b2827794b79391c3;
        float Vector1_3578eca6d8ac4314939bfb16bfdbde27;
        float Vector1_775ef292c0cf4cda91aecf4845cd67bd;
        CBUFFER_END

        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(_CelShading_ad7871fb5bf1404e9f1fa53eab938a3a_Texture2D9666dd2f2f8f4fddb6c2a5c61ddfa9bd_3072536305);
        SAMPLER(sampler_CelShading_ad7871fb5bf1404e9f1fa53eab938a3a_Texture2D9666dd2f2f8f4fddb6c2a5c61ddfa9bd_3072536305);
        TEXTURE2D(Texture2D_c773e99a7970488a9d6bcbd08807a081);
        SAMPLER(samplerTexture2D_c773e99a7970488a9d6bcbd08807a081);
        TEXTURE2D(Texture2D_c23a14955d09487682029de7801dfc17);
        SAMPLER(samplerTexture2D_c23a14955d09487682029de7801dfc17);

            // Graph Functions
            
        // c9d40f89572cca24e7b23cdf7d4f0592
        #include "Assets/Rendering/Shaders/Functions/URPFonctions.hlsl"

        void Unity_Multiply_float(float A, float B, out float Out)
        {
            Out = A * B;
        }

        void Unity_Add_float3(float3 A, float3 B, out float3 Out)
        {
            Out = A + B;
        }

        struct Bindings_WaveDisplacement_81e878f76fb7ea94995742377f80ac36
        {
        };

        void SG_WaveDisplacement_81e878f76fb7ea94995742377f80ac36(float3 Vector3_774baa01c42a49f6abc99dad6b54a583, float Vector1_b53e914fc7984637b80324d57e274b5f, float Vector1_af1a9e8da22b4e08851f991318dec789, float Vector1_e937c75465ce4f99ba008e1a397c452b, Bindings_WaveDisplacement_81e878f76fb7ea94995742377f80ac36 IN, out float3 OutVector3_2)
        {
            float3 _Property_4f1419a54dd7457eb25ffd09e1294716_Out_0 = Vector3_774baa01c42a49f6abc99dad6b54a583;
            float _Property_61f840140291436e8912958fef5953c3_Out_0 = Vector1_b53e914fc7984637b80324d57e274b5f;
            float _Property_aa3d6115a8f7408182a5ad6eaf4dea9c_Out_0 = Vector1_af1a9e8da22b4e08851f991318dec789;
            float _Property_318bfb7c8fcb48359c265b1829bb9333_Out_0 = Vector1_e937c75465ce4f99ba008e1a397c452b;
            float _CalculateWaveCustomFunction_babfa0b8a0ce4e20b113bd73598d4cd6_New_0;
            CalculateWave_float(_Property_4f1419a54dd7457eb25ffd09e1294716_Out_0, _Property_61f840140291436e8912958fef5953c3_Out_0, _Property_aa3d6115a8f7408182a5ad6eaf4dea9c_Out_0, _Property_318bfb7c8fcb48359c265b1829bb9333_Out_0, _CalculateWaveCustomFunction_babfa0b8a0ce4e20b113bd73598d4cd6_New_0);
            float3 _Vector3_fd59b684dbbf45499ba224d658a9f2ec_Out_0 = float3(0, _CalculateWaveCustomFunction_babfa0b8a0ce4e20b113bd73598d4cd6_New_0, 0);
            float3 _Add_bb0b83c602bf41909e47fdeba507406b_Out_2;
            Unity_Add_float3(_Vector3_fd59b684dbbf45499ba224d658a9f2ec_Out_0, _Property_4f1419a54dd7457eb25ffd09e1294716_Out_0, _Add_bb0b83c602bf41909e47fdeba507406b_Out_2);
            OutVector3_2 = _Add_bb0b83c602bf41909e47fdeba507406b_Out_2;
        }

        void Unity_Sign_float(float In, out float Out)
        {
            Out = sign(In);
        }

        void Unity_Multiply_float(float3 A, float3 B, out float3 Out)
        {
            Out = A * B;
        }

        void Unity_Add_float(float A, float B, out float Out)
        {
            Out = A + B;
        }

        void Unity_Subtract_float(float A, float B, out float Out)
        {
            Out = A - B;
        }

        void Unity_Absolute_float(float In, out float Out)
        {
            Out = abs(In);
        }

        void Unity_Saturate_float(float In, out float Out)
        {
            Out = saturate(In);
        }

        void Unity_Divide_float(float A, float B, out float Out)
        {
            Out = A / B;
        }

        void Unity_Combine_float(float R, float G, float B, float A, out float4 RGBA, out float3 RGB, out float2 RG)
        {
            RGBA = float4(R, G, B, A);
            RGB = float3(R, G, B);
            RG = float2(R, G);
        }

        void Unity_Smoothstep_float(float Edge1, float Edge2, float In, out float Out)
        {
            Out = smoothstep(Edge1, Edge2, In);
        }

        void Unity_Lerp_float3(float3 A, float3 B, float3 T, out float3 Out)
        {
            Out = lerp(A, B, T);
        }

        void Unity_Subtract_float3(float3 A, float3 B, out float3 Out)
        {
            Out = A - B;
        }

        void Unity_Normalize_float3(float3 In, out float3 Out)
        {
            Out = normalize(In);
        }

        void Unity_CrossProduct_float(float3 A, float3 B, out float3 Out)
        {
            Out = cross(A, B);
        }

        struct Bindings_Neighbors_df68ff96a41468e448052764fa9bdba1
        {
            float3 ObjectSpaceNormal;
        };

        void SG_Neighbors_df68ff96a41468e448052764fa9bdba1(float3 Vector3_8954E4F1, float Vector1_B5D26639, Bindings_Neighbors_df68ff96a41468e448052764fa9bdba1 IN, out float3 Neighbor1_1, out float3 Neighbor2_2)
        {
            float _Split_609f2b09351ec288b5ed925103f0f9e0_R_1 = IN.ObjectSpaceNormal[0];
            float _Split_609f2b09351ec288b5ed925103f0f9e0_G_2 = IN.ObjectSpaceNormal[1];
            float _Split_609f2b09351ec288b5ed925103f0f9e0_B_3 = IN.ObjectSpaceNormal[2];
            float _Split_609f2b09351ec288b5ed925103f0f9e0_A_4 = 0;
            float _Sign_e3911d5b4df6208882d5722613bfa7df_Out_1;
            Unity_Sign_float(_Split_609f2b09351ec288b5ed925103f0f9e0_R_1, _Sign_e3911d5b4df6208882d5722613bfa7df_Out_1);
            float3 _Property_8a0179cbbf558586ab0af83d3af77169_Out_0 = Vector3_8954E4F1;
            float3 _Multiply_a5a5ef928369cd84997685e6caac87b0_Out_2;
            Unity_Multiply_float(_Property_8a0179cbbf558586ab0af83d3af77169_Out_0, IN.ObjectSpaceNormal, _Multiply_a5a5ef928369cd84997685e6caac87b0_Out_2);
            float _Split_8977fb298fca9e8182c1edaad752997c_R_1 = _Multiply_a5a5ef928369cd84997685e6caac87b0_Out_2[0];
            float _Split_8977fb298fca9e8182c1edaad752997c_G_2 = _Multiply_a5a5ef928369cd84997685e6caac87b0_Out_2[1];
            float _Split_8977fb298fca9e8182c1edaad752997c_B_3 = _Multiply_a5a5ef928369cd84997685e6caac87b0_Out_2[2];
            float _Split_8977fb298fca9e8182c1edaad752997c_A_4 = 0;
            float _Add_eeda377c8b7c038596bc86d83f7f321e_Out_2;
            Unity_Add_float(_Split_8977fb298fca9e8182c1edaad752997c_R_1, _Split_8977fb298fca9e8182c1edaad752997c_G_2, _Add_eeda377c8b7c038596bc86d83f7f321e_Out_2);
            float _Add_61a089896fe3468b82b211a762b6ec21_Out_2;
            Unity_Add_float(_Add_eeda377c8b7c038596bc86d83f7f321e_Out_2, _Split_8977fb298fca9e8182c1edaad752997c_B_3, _Add_61a089896fe3468b82b211a762b6ec21_Out_2);
            float _Split_d79bc2f0568d4b8586a29273c4b01ac3_R_1 = IN.ObjectSpaceNormal[0];
            float _Split_d79bc2f0568d4b8586a29273c4b01ac3_G_2 = IN.ObjectSpaceNormal[1];
            float _Split_d79bc2f0568d4b8586a29273c4b01ac3_B_3 = IN.ObjectSpaceNormal[2];
            float _Split_d79bc2f0568d4b8586a29273c4b01ac3_A_4 = 0;
            float _Float_9d499dd7c2614a8589e8a1cb04daeb5d_Out_0 = 0.05;
            float _Split_802c7c2944faa986ba720f07d8db74ab_R_1 = _Property_8a0179cbbf558586ab0af83d3af77169_Out_0[0];
            float _Split_802c7c2944faa986ba720f07d8db74ab_G_2 = _Property_8a0179cbbf558586ab0af83d3af77169_Out_0[1];
            float _Split_802c7c2944faa986ba720f07d8db74ab_B_3 = _Property_8a0179cbbf558586ab0af83d3af77169_Out_0[2];
            float _Split_802c7c2944faa986ba720f07d8db74ab_A_4 = 0;
            float _Add_4555b76b861a778d97276d765af07483_Out_2;
            Unity_Add_float(_Float_9d499dd7c2614a8589e8a1cb04daeb5d_Out_0, _Split_802c7c2944faa986ba720f07d8db74ab_G_2, _Add_4555b76b861a778d97276d765af07483_Out_2);
            float _Multiply_1e591f42bea8b58c8797ab2c90faa345_Out_2;
            Unity_Multiply_float(_Split_d79bc2f0568d4b8586a29273c4b01ac3_G_2, _Add_4555b76b861a778d97276d765af07483_Out_2, _Multiply_1e591f42bea8b58c8797ab2c90faa345_Out_2);
            float _Subtract_68ba98ef07340b83b46992e63c68d7ca_Out_2;
            Unity_Subtract_float(_Add_61a089896fe3468b82b211a762b6ec21_Out_2, _Multiply_1e591f42bea8b58c8797ab2c90faa345_Out_2, _Subtract_68ba98ef07340b83b46992e63c68d7ca_Out_2);
            float _Multiply_d0572ed2941ee0879549c1da1c572b15_Out_2;
            Unity_Multiply_float(_Split_d79bc2f0568d4b8586a29273c4b01ac3_B_3, _Split_802c7c2944faa986ba720f07d8db74ab_B_3, _Multiply_d0572ed2941ee0879549c1da1c572b15_Out_2);
            float _Subtract_3d9ad256eba2ee8e839158fc0f45ee81_Out_2;
            Unity_Subtract_float(_Subtract_68ba98ef07340b83b46992e63c68d7ca_Out_2, _Multiply_d0572ed2941ee0879549c1da1c572b15_Out_2, _Subtract_3d9ad256eba2ee8e839158fc0f45ee81_Out_2);
            float _Float_654fffd8f366ff808a39bb5a6a1f1fdb_Out_0 = 0.01;
            float _Absolute_9fde5c96b588aa8fbed6af91404da630_Out_1;
            Unity_Absolute_float(_Split_609f2b09351ec288b5ed925103f0f9e0_R_1, _Absolute_9fde5c96b588aa8fbed6af91404da630_Out_1);
            float _Subtract_7173428a9289a58abbac98045c08ceb9_Out_2;
            Unity_Subtract_float(_Absolute_9fde5c96b588aa8fbed6af91404da630_Out_1, _Float_654fffd8f366ff808a39bb5a6a1f1fdb_Out_0, _Subtract_7173428a9289a58abbac98045c08ceb9_Out_2);
            float _Saturate_e8fcca777a01738ab601adda864265d1_Out_1;
            Unity_Saturate_float(_Subtract_7173428a9289a58abbac98045c08ceb9_Out_2, _Saturate_e8fcca777a01738ab601adda864265d1_Out_1);
            float _Add_71e184a7749fc9848483bc81af14e30e_Out_2;
            Unity_Add_float(_Float_654fffd8f366ff808a39bb5a6a1f1fdb_Out_0, _Saturate_e8fcca777a01738ab601adda864265d1_Out_1, _Add_71e184a7749fc9848483bc81af14e30e_Out_2);
            float _Divide_7cc37db0bc8630899cffaa8d565d6d55_Out_2;
            Unity_Divide_float(_Subtract_3d9ad256eba2ee8e839158fc0f45ee81_Out_2, _Add_71e184a7749fc9848483bc81af14e30e_Out_2, _Divide_7cc37db0bc8630899cffaa8d565d6d55_Out_2);
            float _Multiply_9c6b2289e8d2db84a1c0d5ca2a655b93_Out_2;
            Unity_Multiply_float(_Sign_e3911d5b4df6208882d5722613bfa7df_Out_1, _Divide_7cc37db0bc8630899cffaa8d565d6d55_Out_2, _Multiply_9c6b2289e8d2db84a1c0d5ca2a655b93_Out_2);
            float4 _Combine_8fcb17287675378fb024a84210c6677a_RGBA_4;
            float3 _Combine_8fcb17287675378fb024a84210c6677a_RGB_5;
            float2 _Combine_8fcb17287675378fb024a84210c6677a_RG_6;
            Unity_Combine_float(_Multiply_9c6b2289e8d2db84a1c0d5ca2a655b93_Out_2, _Add_4555b76b861a778d97276d765af07483_Out_2, _Split_802c7c2944faa986ba720f07d8db74ab_B_3, 0, _Combine_8fcb17287675378fb024a84210c6677a_RGBA_4, _Combine_8fcb17287675378fb024a84210c6677a_RGB_5, _Combine_8fcb17287675378fb024a84210c6677a_RG_6);
            float _Float_9b0eee4975b51e828a4269a5edcee5b1_Out_0 = 0.05;
            float _Split_f205281c3443d68aa9b0c1a2feace9df_R_1 = _Property_8a0179cbbf558586ab0af83d3af77169_Out_0[0];
            float _Split_f205281c3443d68aa9b0c1a2feace9df_G_2 = _Property_8a0179cbbf558586ab0af83d3af77169_Out_0[1];
            float _Split_f205281c3443d68aa9b0c1a2feace9df_B_3 = _Property_8a0179cbbf558586ab0af83d3af77169_Out_0[2];
            float _Split_f205281c3443d68aa9b0c1a2feace9df_A_4 = 0;
            float _Add_db37e0cfea249d8d9ede45e675501301_Out_2;
            Unity_Add_float(_Float_9b0eee4975b51e828a4269a5edcee5b1_Out_0, _Split_f205281c3443d68aa9b0c1a2feace9df_R_1, _Add_db37e0cfea249d8d9ede45e675501301_Out_2);
            float4 _Combine_9530e51a49a77f849e6eac5cb0ce15ad_RGBA_4;
            float3 _Combine_9530e51a49a77f849e6eac5cb0ce15ad_RGB_5;
            float2 _Combine_9530e51a49a77f849e6eac5cb0ce15ad_RG_6;
            Unity_Combine_float(_Add_db37e0cfea249d8d9ede45e675501301_Out_2, _Split_f205281c3443d68aa9b0c1a2feace9df_G_2, _Split_f205281c3443d68aa9b0c1a2feace9df_B_3, 0, _Combine_9530e51a49a77f849e6eac5cb0ce15ad_RGBA_4, _Combine_9530e51a49a77f849e6eac5cb0ce15ad_RGB_5, _Combine_9530e51a49a77f849e6eac5cb0ce15ad_RG_6);
            float _Multiply_625b1e551cdfd0829aa19290646e6739_Out_2;
            Unity_Multiply_float(-1, _Subtract_7173428a9289a58abbac98045c08ceb9_Out_2, _Multiply_625b1e551cdfd0829aa19290646e6739_Out_2);
            float _Smoothstep_b60ff4818667938f9be3fa9ecf03f163_Out_3;
            Unity_Smoothstep_float(0, 0.01, _Multiply_625b1e551cdfd0829aa19290646e6739_Out_2, _Smoothstep_b60ff4818667938f9be3fa9ecf03f163_Out_3);
            float3 _Lerp_7d115f30f5f0fc8cb46b9ba33440e47a_Out_3;
            Unity_Lerp_float3(_Combine_8fcb17287675378fb024a84210c6677a_RGB_5, _Combine_9530e51a49a77f849e6eac5cb0ce15ad_RGB_5, (_Smoothstep_b60ff4818667938f9be3fa9ecf03f163_Out_3.xxx), _Lerp_7d115f30f5f0fc8cb46b9ba33440e47a_Out_3);
            float3 _Subtract_24a46f67e2876e80b3b01e0b77a50456_Out_2;
            Unity_Subtract_float3(_Lerp_7d115f30f5f0fc8cb46b9ba33440e47a_Out_3, _Property_8a0179cbbf558586ab0af83d3af77169_Out_0, _Subtract_24a46f67e2876e80b3b01e0b77a50456_Out_2);
            float3 _Normalize_f544245d7bd37f8fbaee4a87c2403675_Out_1;
            Unity_Normalize_float3(_Subtract_24a46f67e2876e80b3b01e0b77a50456_Out_2, _Normalize_f544245d7bd37f8fbaee4a87c2403675_Out_1);
            float3 _Normalize_9a359141b951ea84b3220ad5c3123b7c_Out_1;
            Unity_Normalize_float3(IN.ObjectSpaceNormal, _Normalize_9a359141b951ea84b3220ad5c3123b7c_Out_1);
            float3 _CrossProduct_1dbde50ff8c546829c8b59ec1630c252_Out_2;
            Unity_CrossProduct_float(_Normalize_f544245d7bd37f8fbaee4a87c2403675_Out_1, _Normalize_9a359141b951ea84b3220ad5c3123b7c_Out_1, _CrossProduct_1dbde50ff8c546829c8b59ec1630c252_Out_2);
            float3 _Normalize_3eb509f6a593a18d856dabb44191008a_Out_1;
            Unity_Normalize_float3(_CrossProduct_1dbde50ff8c546829c8b59ec1630c252_Out_2, _Normalize_3eb509f6a593a18d856dabb44191008a_Out_1);
            float _Property_4448eb2908e4228e840b80f046abf12e_Out_0 = Vector1_B5D26639;
            float3 _Multiply_37bf5602003a2885bb744b13f93296fe_Out_2;
            Unity_Multiply_float(_Normalize_3eb509f6a593a18d856dabb44191008a_Out_1, (_Property_4448eb2908e4228e840b80f046abf12e_Out_0.xxx), _Multiply_37bf5602003a2885bb744b13f93296fe_Out_2);
            float3 _Add_bd2759426a510983979415cc0e46c2d4_Out_2;
            Unity_Add_float3(_Multiply_37bf5602003a2885bb744b13f93296fe_Out_2, _Property_8a0179cbbf558586ab0af83d3af77169_Out_0, _Add_bd2759426a510983979415cc0e46c2d4_Out_2);
            float3 _Multiply_273e0ecf939b858582031df2660ec861_Out_2;
            Unity_Multiply_float(_Normalize_f544245d7bd37f8fbaee4a87c2403675_Out_1, (_Property_4448eb2908e4228e840b80f046abf12e_Out_0.xxx), _Multiply_273e0ecf939b858582031df2660ec861_Out_2);
            float3 _Add_2259c39580c12b8297f6b81a426bd3c0_Out_2;
            Unity_Add_float3(_Property_8a0179cbbf558586ab0af83d3af77169_Out_0, _Multiply_273e0ecf939b858582031df2660ec861_Out_2, _Add_2259c39580c12b8297f6b81a426bd3c0_Out_2);
            Neighbor1_1 = _Add_bd2759426a510983979415cc0e46c2d4_Out_2;
            Neighbor2_2 = _Add_2259c39580c12b8297f6b81a426bd3c0_Out_2;
        }

        struct Bindings_NewNormal_c8d6b0224f233004190264cd9e5ca192
        {
        };

        void SG_NewNormal_c8d6b0224f233004190264cd9e5ca192(float3 Vector3_988291EB, float3 Vector3_EB4E2DE7, float3 Vector3_1973C67A, Bindings_NewNormal_c8d6b0224f233004190264cd9e5ca192 IN, out float3 OutVector3_1)
        {
            float3 _Property_fd61227e10c3348bb61b5f1d39e68328_Out_0 = Vector3_EB4E2DE7;
            float3 _Property_b8cacddd74e568809402e877976dffa8_Out_0 = Vector3_988291EB;
            float3 _Subtract_1f3c648b5559d28f9acfbe1af362b37b_Out_2;
            Unity_Subtract_float3(_Property_fd61227e10c3348bb61b5f1d39e68328_Out_0, _Property_b8cacddd74e568809402e877976dffa8_Out_0, _Subtract_1f3c648b5559d28f9acfbe1af362b37b_Out_2);
            float3 _Normalize_888167f431fbc788b4ad8e2092e7f28b_Out_1;
            Unity_Normalize_float3(_Subtract_1f3c648b5559d28f9acfbe1af362b37b_Out_2, _Normalize_888167f431fbc788b4ad8e2092e7f28b_Out_1);
            float3 _Property_6008e654ce04488cbe1d5caac8afe08a_Out_0 = Vector3_1973C67A;
            float3 _Subtract_208696e2abb5f18d84a5878fd9a81553_Out_2;
            Unity_Subtract_float3(_Property_6008e654ce04488cbe1d5caac8afe08a_Out_0, _Property_b8cacddd74e568809402e877976dffa8_Out_0, _Subtract_208696e2abb5f18d84a5878fd9a81553_Out_2);
            float3 _Normalize_7250d413c7450d8aaa805c923843f2dd_Out_1;
            Unity_Normalize_float3(_Subtract_208696e2abb5f18d84a5878fd9a81553_Out_2, _Normalize_7250d413c7450d8aaa805c923843f2dd_Out_1);
            float3 _CrossProduct_5566a5ab348aa8809609d834bbe4451f_Out_2;
            Unity_CrossProduct_float(_Normalize_888167f431fbc788b4ad8e2092e7f28b_Out_1, _Normalize_7250d413c7450d8aaa805c923843f2dd_Out_1, _CrossProduct_5566a5ab348aa8809609d834bbe4451f_Out_2);
            float3 _Normalize_a640075232b8c78498848f10ca1e7ba8_Out_1;
            Unity_Normalize_float3(_CrossProduct_5566a5ab348aa8809609d834bbe4451f_Out_2, _Normalize_a640075232b8c78498848f10ca1e7ba8_Out_1);
            OutVector3_1 = _Normalize_a640075232b8c78498848f10ca1e7ba8_Out_1;
        }

        void Unity_SceneDepth_Linear01_float(float4 UV, out float Out)
        {
            Out = Linear01Depth(SHADERGRAPH_SAMPLE_SCENE_DEPTH(UV.xy), _ZBufferParams);
        }

        void Unity_Clamp_float(float In, float Min, float Max, out float Out)
        {
            Out = clamp(In, Min, Max);
        }

        void Unity_Lerp_float4(float4 A, float4 B, float4 T, out float4 Out)
        {
            Out = lerp(A, B, T);
        }

        void Unity_Multiply_float(float2 A, float2 B, out float2 Out)
        {
            Out = A * B;
        }

        void Unity_NormalBlend_float(float3 A, float3 B, out float3 Out)
        {
            Out = SafeNormalize(float3(A.rg + B.rg, A.b * B.b));
        }

        void Unity_NormalStrength_float(float3 In, float Strength, out float3 Out)
        {
            Out = float3(In.rg * Strength, lerp(1, In.b, saturate(Strength)));
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

        void Unity_Power_float(float A, float B, out float Out)
        {
            Out = pow(A, B);
        }

        void Unity_FresnelEffect_float(float3 Normal, float3 ViewDir, float Power, out float Out)
        {
            Out = pow((1.0 - saturate(dot(normalize(Normal), normalize(ViewDir)))), Power);
        }

        void Unity_Maximum_float(float A, float B, out float Out)
        {
            Out = max(A, B);
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
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            UnityTexture2D _Property_52e3e5c2509c4df984c47cc092c466d8_Out_0 = UnityBuildTexture2DStructNoScale(Texture2D_c773e99a7970488a9d6bcbd08807a081);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Property_736f2734d3e643c398a2586801e4cc17_Out_0 = Vector1_f133b62c64f14525895d4b7d5afdd79f;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Property_7851f92dfd4d4302a7beb8a17dd4bd8b_Out_0 = Vector1_45a27133fb8b4113a0746c0d1954c57b;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float2 _CalculateNoiseCustomFunction_a0c2603a229c4d299ac32a59a8664765_Out_0;
            CalculateNoise_float(IN.AbsoluteWorldSpacePosition, _Property_736f2734d3e643c398a2586801e4cc17_Out_0, _Property_7851f92dfd4d4302a7beb8a17dd4bd8b_Out_0, _CalculateNoiseCustomFunction_a0c2603a229c4d299ac32a59a8664765_Out_0);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            #if defined(SHADER_API_GLES) && (SHADER_TARGET < 30)
              float4 _SampleTexture2DLOD_fe1fede5211b477cbca93fc4c2f76d12_RGBA_0 = float4(0.0f, 0.0f, 0.0f, 1.0f);
            #else
              float4 _SampleTexture2DLOD_fe1fede5211b477cbca93fc4c2f76d12_RGBA_0 = SAMPLE_TEXTURE2D_LOD(_Property_52e3e5c2509c4df984c47cc092c466d8_Out_0.tex, _Property_52e3e5c2509c4df984c47cc092c466d8_Out_0.samplerstate, _CalculateNoiseCustomFunction_a0c2603a229c4d299ac32a59a8664765_Out_0, 0);
            #endif
            float _SampleTexture2DLOD_fe1fede5211b477cbca93fc4c2f76d12_R_5 = _SampleTexture2DLOD_fe1fede5211b477cbca93fc4c2f76d12_RGBA_0.r;
            float _SampleTexture2DLOD_fe1fede5211b477cbca93fc4c2f76d12_G_6 = _SampleTexture2DLOD_fe1fede5211b477cbca93fc4c2f76d12_RGBA_0.g;
            float _SampleTexture2DLOD_fe1fede5211b477cbca93fc4c2f76d12_B_7 = _SampleTexture2DLOD_fe1fede5211b477cbca93fc4c2f76d12_RGBA_0.b;
            float _SampleTexture2DLOD_fe1fede5211b477cbca93fc4c2f76d12_A_8 = _SampleTexture2DLOD_fe1fede5211b477cbca93fc4c2f76d12_RGBA_0.a;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Property_fab5f23c0a9e449bb605bc77e42f5c55_Out_0 = Vector1_57cb41fdb69545e8b53a796761acb08d;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Multiply_be99c4e752404fa4b5a42166d27a7cc0_Out_2;
            Unity_Multiply_float(_SampleTexture2DLOD_fe1fede5211b477cbca93fc4c2f76d12_R_5, _Property_fab5f23c0a9e449bb605bc77e42f5c55_Out_0, _Multiply_be99c4e752404fa4b5a42166d27a7cc0_Out_2);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float3 _Vector3_9e43fda8531d474c890ea7595c453773_Out_0 = float3(0, _Multiply_be99c4e752404fa4b5a42166d27a7cc0_Out_2, 0);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Property_0ea561f00aaf4fbb8fdfeab5a2a0dd08_Out_0 = Vector1_4da6e98cbfa5408fb6352b485aa76d05;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Property_e4937aaae9bc4c098cebea935a7ee1ea_Out_0 = Vector1_b266d7e71de2489ea084cb27060cacca;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Property_2d3470f269ac4477a8dc467d895fe472_Out_0 = Vector1_d09cc377b0524610970658ffdacd497f;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            Bindings_WaveDisplacement_81e878f76fb7ea94995742377f80ac36 _WaveDisplacement_5f54fff0c6334b26bedfff0b63df12b3;
            float3 _WaveDisplacement_5f54fff0c6334b26bedfff0b63df12b3_OutVector3_2;
            SG_WaveDisplacement_81e878f76fb7ea94995742377f80ac36(IN.AbsoluteWorldSpacePosition, _Property_0ea561f00aaf4fbb8fdfeab5a2a0dd08_Out_0, _Property_e4937aaae9bc4c098cebea935a7ee1ea_Out_0, _Property_2d3470f269ac4477a8dc467d895fe472_Out_0, _WaveDisplacement_5f54fff0c6334b26bedfff0b63df12b3, _WaveDisplacement_5f54fff0c6334b26bedfff0b63df12b3_OutVector3_2);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float3 _Add_942b46ad98eb4925b4bfa00a0a5781c7_Out_2;
            Unity_Add_float3(_Vector3_9e43fda8531d474c890ea7595c453773_Out_0, _WaveDisplacement_5f54fff0c6334b26bedfff0b63df12b3_OutVector3_2, _Add_942b46ad98eb4925b4bfa00a0a5781c7_Out_2);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float3 _Transform_3fce2cebd3054a72a946f448c89c2e3d_Out_1 = TransformWorldToObject(GetCameraRelativePositionWS(_Add_942b46ad98eb4925b4bfa00a0a5781c7_Out_2.xyz));
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            Bindings_Neighbors_df68ff96a41468e448052764fa9bdba1 _Neighbors_e648c110f4754cefb6491b8c82fe074f;
            _Neighbors_e648c110f4754cefb6491b8c82fe074f.ObjectSpaceNormal = IN.ObjectSpaceNormal;
            float3 _Neighbors_e648c110f4754cefb6491b8c82fe074f_Neighbor1_1;
            float3 _Neighbors_e648c110f4754cefb6491b8c82fe074f_Neighbor2_2;
            SG_Neighbors_df68ff96a41468e448052764fa9bdba1(IN.AbsoluteWorldSpacePosition, 0.01, _Neighbors_e648c110f4754cefb6491b8c82fe074f, _Neighbors_e648c110f4754cefb6491b8c82fe074f_Neighbor1_1, _Neighbors_e648c110f4754cefb6491b8c82fe074f_Neighbor2_2);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            Bindings_WaveDisplacement_81e878f76fb7ea94995742377f80ac36 _WaveDisplacement_2fa36261c2534cbe8639a41fdfe4910a;
            float3 _WaveDisplacement_2fa36261c2534cbe8639a41fdfe4910a_OutVector3_2;
            SG_WaveDisplacement_81e878f76fb7ea94995742377f80ac36(_Neighbors_e648c110f4754cefb6491b8c82fe074f_Neighbor1_1, _Property_0ea561f00aaf4fbb8fdfeab5a2a0dd08_Out_0, _Property_e4937aaae9bc4c098cebea935a7ee1ea_Out_0, _Property_2d3470f269ac4477a8dc467d895fe472_Out_0, _WaveDisplacement_2fa36261c2534cbe8639a41fdfe4910a, _WaveDisplacement_2fa36261c2534cbe8639a41fdfe4910a_OutVector3_2);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            Bindings_WaveDisplacement_81e878f76fb7ea94995742377f80ac36 _WaveDisplacement_c9fd16638edd4ccfaf9e3eb2efa55bfa;
            float3 _WaveDisplacement_c9fd16638edd4ccfaf9e3eb2efa55bfa_OutVector3_2;
            SG_WaveDisplacement_81e878f76fb7ea94995742377f80ac36(_Neighbors_e648c110f4754cefb6491b8c82fe074f_Neighbor2_2, _Property_0ea561f00aaf4fbb8fdfeab5a2a0dd08_Out_0, _Property_e4937aaae9bc4c098cebea935a7ee1ea_Out_0, _Property_2d3470f269ac4477a8dc467d895fe472_Out_0, _WaveDisplacement_c9fd16638edd4ccfaf9e3eb2efa55bfa, _WaveDisplacement_c9fd16638edd4ccfaf9e3eb2efa55bfa_OutVector3_2);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            Bindings_NewNormal_c8d6b0224f233004190264cd9e5ca192 _NewNormal_e9af9405a4a54034863b00f2fbd4c2b6;
            float3 _NewNormal_e9af9405a4a54034863b00f2fbd4c2b6_OutVector3_1;
            SG_NewNormal_c8d6b0224f233004190264cd9e5ca192(_WaveDisplacement_5f54fff0c6334b26bedfff0b63df12b3_OutVector3_2, _WaveDisplacement_2fa36261c2534cbe8639a41fdfe4910a_OutVector3_2, _WaveDisplacement_c9fd16638edd4ccfaf9e3eb2efa55bfa_OutVector3_2, _NewNormal_e9af9405a4a54034863b00f2fbd4c2b6, _NewNormal_e9af9405a4a54034863b00f2fbd4c2b6_OutVector3_1);
            #endif
            description.Position = _Transform_3fce2cebd3054a72a946f448c89c2e3d_Out_1;
            description.Normal = _NewNormal_e9af9405a4a54034863b00f2fbd4c2b6_OutVector3_1;
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
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float4 _Property_1a8602fe3b054c0d851c1d1bb94bfe17_Out_0 = Color_8b23519dc989478fa19369d8efda52f6;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float4 _Property_0ccb5c18d3eb40c5980df5a141a382a5_Out_0 = Color_213be8c364f242cdbabe6ef25c0efbd9;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _SceneDepth_7981f193eac94d87b6f2b2920d4cb528_Out_1;
            Unity_SceneDepth_Linear01_float(float4(IN.ScreenPosition.xy / IN.ScreenPosition.w, 0, 0), _SceneDepth_7981f193eac94d87b6f2b2920d4cb528_Out_1);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Multiply_abe194398c584d8a8934b67cf64c0da1_Out_2;
            Unity_Multiply_float(_SceneDepth_7981f193eac94d87b6f2b2920d4cb528_Out_1, _ProjectionParams.z, _Multiply_abe194398c584d8a8934b67cf64c0da1_Out_2);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float4 _ScreenPosition_68dc8d80a07f4706a6a9a2f0230737ed_Out_0 = IN.ScreenPosition;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Split_7f7e1aadecd54073aff51bd05fa759bc_R_1 = _ScreenPosition_68dc8d80a07f4706a6a9a2f0230737ed_Out_0[0];
            float _Split_7f7e1aadecd54073aff51bd05fa759bc_G_2 = _ScreenPosition_68dc8d80a07f4706a6a9a2f0230737ed_Out_0[1];
            float _Split_7f7e1aadecd54073aff51bd05fa759bc_B_3 = _ScreenPosition_68dc8d80a07f4706a6a9a2f0230737ed_Out_0[2];
            float _Split_7f7e1aadecd54073aff51bd05fa759bc_A_4 = _ScreenPosition_68dc8d80a07f4706a6a9a2f0230737ed_Out_0[3];
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Multiply_7ba2c47eef0f4b89b3b4eb4a8ed31f83_Out_2;
            Unity_Multiply_float(_Split_7f7e1aadecd54073aff51bd05fa759bc_A_4, 1, _Multiply_7ba2c47eef0f4b89b3b4eb4a8ed31f83_Out_2);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Subtract_dc119b36f453437e913cdd2a2623efd2_Out_2;
            Unity_Subtract_float(_Multiply_abe194398c584d8a8934b67cf64c0da1_Out_2, _Multiply_7ba2c47eef0f4b89b3b4eb4a8ed31f83_Out_2, _Subtract_dc119b36f453437e913cdd2a2623efd2_Out_2);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Property_ab48707fe6db419eb51ece316bb6313b_Out_0 = Vector1_cc6e37850778480781084b62719d58c3;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Multiply_3a48e77d57694a868985ec140162c2ef_Out_2;
            Unity_Multiply_float(_Subtract_dc119b36f453437e913cdd2a2623efd2_Out_2, _Property_ab48707fe6db419eb51ece316bb6313b_Out_0, _Multiply_3a48e77d57694a868985ec140162c2ef_Out_2);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Clamp_bb886dbf030149bd8ffa4e979d6e9c46_Out_3;
            Unity_Clamp_float(_Multiply_3a48e77d57694a868985ec140162c2ef_Out_2, 0, 1, _Clamp_bb886dbf030149bd8ffa4e979d6e9c46_Out_3);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float4 _Lerp_5c0550f6966845719d72583200d6ffde_Out_3;
            Unity_Lerp_float4(_Property_1a8602fe3b054c0d851c1d1bb94bfe17_Out_0, _Property_0ccb5c18d3eb40c5980df5a141a382a5_Out_0, (_Clamp_bb886dbf030149bd8ffa4e979d6e9c46_Out_3.xxxx), _Lerp_5c0550f6966845719d72583200d6ffde_Out_3);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            UnityTexture2D _Property_c40253d6917345378f175ecc530ca7ab_Out_0 = UnityBuildTexture2DStructNoScale(Texture2D_c23a14955d09487682029de7801dfc17);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Property_4b555a0d276d4492ae020102ab7d5f6d_Out_0 = Vector1_ba885a3188cc490b9d04d880f79cd1b0;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Multiply_37551db39e924fb4b75b35ca95ad2f80_Out_2;
            Unity_Multiply_float(_Property_4b555a0d276d4492ae020102ab7d5f6d_Out_0, 2, _Multiply_37551db39e924fb4b75b35ca95ad2f80_Out_2);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Property_bdd005a7ca7d46889210f1373e17240d_Out_0 = Vector1_28676355978446f285fbfe6f1a574316;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float2 _CalculateNoiseCustomFunction_1f60b49d6983442f91651f8642c54ee9_Out_0;
            CalculateNoise_float(IN.ObjectSpacePosition, _Multiply_37551db39e924fb4b75b35ca95ad2f80_Out_2, _Property_bdd005a7ca7d46889210f1373e17240d_Out_0, _CalculateNoiseCustomFunction_1f60b49d6983442f91651f8642c54ee9_Out_0);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            #if defined(SHADER_API_GLES) && (SHADER_TARGET < 30)
              float4 _SampleTexture2DLOD_3997426701814db8be9b6de9e67109eb_RGBA_0 = float4(0.0f, 0.0f, 0.0f, 1.0f);
            #else
              float4 _SampleTexture2DLOD_3997426701814db8be9b6de9e67109eb_RGBA_0 = SAMPLE_TEXTURE2D_LOD(_Property_c40253d6917345378f175ecc530ca7ab_Out_0.tex, _Property_c40253d6917345378f175ecc530ca7ab_Out_0.samplerstate, _CalculateNoiseCustomFunction_1f60b49d6983442f91651f8642c54ee9_Out_0, 0);
            #endif
            _SampleTexture2DLOD_3997426701814db8be9b6de9e67109eb_RGBA_0.rgb = UnpackNormal(_SampleTexture2DLOD_3997426701814db8be9b6de9e67109eb_RGBA_0);
            float _SampleTexture2DLOD_3997426701814db8be9b6de9e67109eb_R_5 = _SampleTexture2DLOD_3997426701814db8be9b6de9e67109eb_RGBA_0.r;
            float _SampleTexture2DLOD_3997426701814db8be9b6de9e67109eb_G_6 = _SampleTexture2DLOD_3997426701814db8be9b6de9e67109eb_RGBA_0.g;
            float _SampleTexture2DLOD_3997426701814db8be9b6de9e67109eb_B_7 = _SampleTexture2DLOD_3997426701814db8be9b6de9e67109eb_RGBA_0.b;
            float _SampleTexture2DLOD_3997426701814db8be9b6de9e67109eb_A_8 = _SampleTexture2DLOD_3997426701814db8be9b6de9e67109eb_RGBA_0.a;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Multiply_8d56ca2bac4a4606b07568c5fe27fab5_Out_2;
            Unity_Multiply_float(_Multiply_37551db39e924fb4b75b35ca95ad2f80_Out_2, -0.1, _Multiply_8d56ca2bac4a4606b07568c5fe27fab5_Out_2);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float2 _CalculateNoiseCustomFunction_4767eb28ae364522aa093fefd13ee291_Out_0;
            CalculateNoise_float(IN.ObjectSpacePosition, _Multiply_8d56ca2bac4a4606b07568c5fe27fab5_Out_2, _Property_bdd005a7ca7d46889210f1373e17240d_Out_0, _CalculateNoiseCustomFunction_4767eb28ae364522aa093fefd13ee291_Out_0);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float2 _Multiply_4856504d81624c5183fee40deaf3653d_Out_2;
            Unity_Multiply_float(_CalculateNoiseCustomFunction_4767eb28ae364522aa093fefd13ee291_Out_0, float2(-1, -1), _Multiply_4856504d81624c5183fee40deaf3653d_Out_2);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            #if defined(SHADER_API_GLES) && (SHADER_TARGET < 30)
              float4 _SampleTexture2DLOD_e11df39f24034543be10c1c451ba3abd_RGBA_0 = float4(0.0f, 0.0f, 0.0f, 1.0f);
            #else
              float4 _SampleTexture2DLOD_e11df39f24034543be10c1c451ba3abd_RGBA_0 = SAMPLE_TEXTURE2D_LOD(_Property_c40253d6917345378f175ecc530ca7ab_Out_0.tex, _Property_c40253d6917345378f175ecc530ca7ab_Out_0.samplerstate, _Multiply_4856504d81624c5183fee40deaf3653d_Out_2, 0);
            #endif
            _SampleTexture2DLOD_e11df39f24034543be10c1c451ba3abd_RGBA_0.rgb = UnpackNormal(_SampleTexture2DLOD_e11df39f24034543be10c1c451ba3abd_RGBA_0);
            float _SampleTexture2DLOD_e11df39f24034543be10c1c451ba3abd_R_5 = _SampleTexture2DLOD_e11df39f24034543be10c1c451ba3abd_RGBA_0.r;
            float _SampleTexture2DLOD_e11df39f24034543be10c1c451ba3abd_G_6 = _SampleTexture2DLOD_e11df39f24034543be10c1c451ba3abd_RGBA_0.g;
            float _SampleTexture2DLOD_e11df39f24034543be10c1c451ba3abd_B_7 = _SampleTexture2DLOD_e11df39f24034543be10c1c451ba3abd_RGBA_0.b;
            float _SampleTexture2DLOD_e11df39f24034543be10c1c451ba3abd_A_8 = _SampleTexture2DLOD_e11df39f24034543be10c1c451ba3abd_RGBA_0.a;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float3 _NormalBlend_52c804727a444571ac19c41a6a0d1a36_Out_2;
            Unity_NormalBlend_float((_SampleTexture2DLOD_3997426701814db8be9b6de9e67109eb_RGBA_0.xyz), (_SampleTexture2DLOD_e11df39f24034543be10c1c451ba3abd_RGBA_0.xyz), _NormalBlend_52c804727a444571ac19c41a6a0d1a36_Out_2);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Property_41566b7bdd964ab0a886d5b7c7e06eae_Out_0 = Vector1_bf79c34252fe4364976a552df388436c;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float3 _NormalStrength_07e07b43827048248308d9c7a65f7578_Out_2;
            Unity_NormalStrength_float(_NormalBlend_52c804727a444571ac19c41a6a0d1a36_Out_2, _Property_41566b7bdd964ab0a886d5b7c7e06eae_Out_0, _NormalStrength_07e07b43827048248308d9c7a65f7578_Out_2);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float3 _Normalize_7b05a0adf23947229dbcb1136c5b2557_Out_1;
            Unity_Normalize_float3(_NormalStrength_07e07b43827048248308d9c7a65f7578_Out_2, _Normalize_7b05a0adf23947229dbcb1136c5b2557_Out_1);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Property_9a299c60e1344371ae6f61dcf6f95e4e_Out_0 = Vector1_775ef292c0cf4cda91aecf4845cd67bd;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Property_f3f8f4f602e046b88135452055729933_Out_0 = Vector1_3578eca6d8ac4314939bfb16bfdbde27;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            Bindings_CelShading_802007f66a3e13d42802a3875ed35370 _CelShading_ad7871fb5bf1404e9f1fa53eab938a3a;
            _CelShading_ad7871fb5bf1404e9f1fa53eab938a3a.WorldSpaceNormal = IN.WorldSpaceNormal;
            _CelShading_ad7871fb5bf1404e9f1fa53eab938a3a.WorldSpaceTangent = IN.WorldSpaceTangent;
            _CelShading_ad7871fb5bf1404e9f1fa53eab938a3a.WorldSpaceBiTangent = IN.WorldSpaceBiTangent;
            _CelShading_ad7871fb5bf1404e9f1fa53eab938a3a.WorldSpaceViewDirection = IN.WorldSpaceViewDirection;
            _CelShading_ad7871fb5bf1404e9f1fa53eab938a3a.AbsoluteWorldSpacePosition = IN.AbsoluteWorldSpacePosition;
            _CelShading_ad7871fb5bf1404e9f1fa53eab938a3a.uv0 = IN.uv0;
            half4 _CelShading_ad7871fb5bf1404e9f1fa53eab938a3a_Color_1;
            SG_CelShading_802007f66a3e13d42802a3875ed35370(_Lerp_5c0550f6966845719d72583200d6ffde_Out_3, _Normalize_7b05a0adf23947229dbcb1136c5b2557_Out_1, _Property_9a299c60e1344371ae6f61dcf6f95e4e_Out_0, _Property_f3f8f4f602e046b88135452055729933_Out_0, UnityBuildTexture2DStructNoScale(_CelShading_ad7871fb5bf1404e9f1fa53eab938a3a_Texture2D9666dd2f2f8f4fddb6c2a5c61ddfa9bd_3072536305), _CelShading_ad7871fb5bf1404e9f1fa53eab938a3a, _CelShading_ad7871fb5bf1404e9f1fa53eab938a3a_Color_1);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Power_795157e2d171465e908da5d45e938d28_Out_2;
            Unity_Power_float(_Clamp_bb886dbf030149bd8ffa4e979d6e9c46_Out_3, 0.7, _Power_795157e2d171465e908da5d45e938d28_Out_2);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Property_41daac0ae2c74098ac9111219e1e3b3b_Out_0 = Vector1_5be3734798f64a63b2827794b79391c3;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _FresnelEffect_98bea57f43804c61a33cabcb5a3e5400_Out_3;
            Unity_FresnelEffect_float(IN.WorldSpaceNormal, IN.WorldSpaceViewDirection, _Property_41daac0ae2c74098ac9111219e1e3b3b_Out_0, _FresnelEffect_98bea57f43804c61a33cabcb5a3e5400_Out_3);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Maximum_85ae2b8b22d5495fb3e04a01fa179389_Out_2;
            Unity_Maximum_float(_FresnelEffect_98bea57f43804c61a33cabcb5a3e5400_Out_3, 0.7, _Maximum_85ae2b8b22d5495fb3e04a01fa179389_Out_2);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Multiply_465f2016ad9d499981e3a39ae56b8aef_Out_2;
            Unity_Multiply_float(_Power_795157e2d171465e908da5d45e938d28_Out_2, _Maximum_85ae2b8b22d5495fb3e04a01fa179389_Out_2, _Multiply_465f2016ad9d499981e3a39ae56b8aef_Out_2);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Saturate_00659ca349c74eea975fbb11da46e6bf_Out_1;
            Unity_Saturate_float(_Multiply_465f2016ad9d499981e3a39ae56b8aef_Out_2, _Saturate_00659ca349c74eea975fbb11da46e6bf_Out_1);
            #endif
            surface.BaseColor = (_CelShading_ad7871fb5bf1404e9f1fa53eab938a3a_Color_1.xyz);
            surface.Alpha = _Saturate_00659ca349c74eea975fbb11da46e6bf_Out_1;
            return surface;
        }

            // --------------------------------------------------
            // Build Graph Inputs

            VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
        {
            VertexDescriptionInputs output;
            ZERO_INITIALIZE(VertexDescriptionInputs, output);

        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        output.ObjectSpaceNormal =           input.normalOS;
        #endif

        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        output.WorldSpaceNormal =            TransformObjectToWorldNormal(input.normalOS);
        #endif

        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        output.ObjectSpaceTangent =          input.tangentOS.xyz;
        #endif

        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        output.WorldSpaceTangent =           TransformObjectToWorldDir(input.tangentOS.xyz);
        #endif

        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        output.ObjectSpaceBiTangent =        normalize(cross(input.normalOS, input.tangentOS) * (input.tangentOS.w > 0.0f ? 1.0f : -1.0f) * GetOddNegativeScale());
        #endif

        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        output.WorldSpaceBiTangent =         TransformObjectToWorldDir(output.ObjectSpaceBiTangent);
        #endif

        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        output.ObjectSpacePosition =         input.positionOS;
        #endif

        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        output.AbsoluteWorldSpacePosition =  GetAbsolutePositionWS(TransformObjectToWorld(input.positionOS));
        #endif


            return output;
        }
            SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);

        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        // must use interpolated tangent, bitangent and normal before they are normalized in the pixel shader.
        #endif

        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        float3 unnormalizedNormalWS = input.normalWS;
        #endif

        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        const float renormFactor = 1.0 / length(unnormalizedNormalWS);
        #endif


        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        // use bitangent on the fly like in hdrp
        #endif

        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        // IMPORTANT! If we ever support Flip on double sided materials ensure bitangent and tangent are NOT flipped.
        #endif

        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        float crossSign = (input.tangentWS.w > 0.0 ? 1.0 : -1.0) * GetOddNegativeScale();
        #endif

        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        float3 bitang = crossSign * cross(input.normalWS.xyz, input.tangentWS.xyz);
        #endif


        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        output.WorldSpaceNormal =            renormFactor*input.normalWS.xyz;		// we want a unit length Normal Vector node in shader graph
        #endif


        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        // to preserve mikktspace compliance we use same scale renormFactor as was used on the normal.
        #endif

        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        // This is explained in section 2.2 in "surface gradient based bump mapping framework"
        #endif

        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        output.WorldSpaceTangent =           renormFactor*input.tangentWS.xyz;
        #endif

        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        output.WorldSpaceBiTangent =         renormFactor*bitang;
        #endif


        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        output.WorldSpaceViewDirection =     input.viewDirectionWS; //TODO: by default normalized in HD, but not in universal
        #endif

        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        output.WorldSpacePosition =          input.positionWS;
        #endif

        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        output.ObjectSpacePosition =         TransformWorldToObject(input.positionWS);
        #endif

        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        output.AbsoluteWorldSpacePosition =  GetAbsolutePositionWS(input.positionWS);
        #endif

        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        output.ScreenPosition =              ComputeScreenPos(TransformWorldToHClip(input.positionWS), _ProjectionParams.x);
        #endif

        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        output.uv0 =                         input.texCoord0;
        #endif

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
            #pragma multi_compile_local _ CEL_SPECULAR_ON

        #if defined(CEL_SPECULAR_ON)
            #define KEYWORD_PERMUTATION_0
        #else
            #define KEYWORD_PERMUTATION_1
        #endif


            // Defines
        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        #define _SURFACE_TYPE_TRANSPARENT 1
        #endif

        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        #define ATTRIBUTES_NEED_NORMAL
        #endif

        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        #define ATTRIBUTES_NEED_TANGENT
        #endif

        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        #define VARYINGS_NEED_POSITION_WS
        #endif

        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        #define VARYINGS_NEED_NORMAL_WS
        #endif

        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        #define VARYINGS_NEED_VIEWDIRECTION_WS
        #endif

            #define FEATURES_GRAPH_VERTEX
            /* WARNING: $splice Could not find named fragment 'PassInstancing' */
            #define SHADERPASS SHADERPASS_SHADOWCASTER
        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        #define REQUIRE_DEPTH_TEXTURE
        #endif
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
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float3 positionOS : POSITION;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float3 normalOS : NORMAL;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float4 tangentOS : TANGENT;
            #endif
            #if UNITY_ANY_INSTANCING_ENABLED
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            uint instanceID : INSTANCEID_SEMANTIC;
            #endif
            #endif
        };
        struct Varyings
        {
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float4 positionCS : SV_POSITION;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float3 positionWS;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float3 normalWS;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float3 viewDirectionWS;
            #endif
            #if UNITY_ANY_INSTANCING_ENABLED
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
            #endif
        };
        struct SurfaceDescriptionInputs
        {
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float3 WorldSpaceNormal;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float3 WorldSpaceViewDirection;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float3 WorldSpacePosition;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float4 ScreenPosition;
            #endif
        };
        struct VertexDescriptionInputs
        {
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float3 ObjectSpaceNormal;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float3 WorldSpaceNormal;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float3 ObjectSpaceTangent;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float3 WorldSpaceTangent;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float3 ObjectSpaceBiTangent;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float3 WorldSpaceBiTangent;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float3 ObjectSpacePosition;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float3 AbsoluteWorldSpacePosition;
            #endif
        };
        struct PackedVaryings
        {
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float4 positionCS : SV_POSITION;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float3 interp0 : TEXCOORD0;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float3 interp1 : TEXCOORD1;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float3 interp2 : TEXCOORD2;
            #endif
            #if UNITY_ANY_INSTANCING_ENABLED
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
            #endif
        };

            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        PackedVaryings PackVaryings (Varyings input)
        {
            PackedVaryings output;
            output.positionCS = input.positionCS;
            output.interp0.xyz =  input.positionWS;
            output.interp1.xyz =  input.normalWS;
            output.interp2.xyz =  input.viewDirectionWS;
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
            output.viewDirectionWS = input.interp2.xyz;
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
        #endif

            // --------------------------------------------------
            // Graph

            // Graph Properties
            CBUFFER_START(UnityPerMaterial)
        float4 _CelShading_ad7871fb5bf1404e9f1fa53eab938a3a_Texture2D9666dd2f2f8f4fddb6c2a5c61ddfa9bd_3072536305_TexelSize;
        float Vector1_0e212c6562134ef7add6f6766d6419b4;
        float4 Color_213be8c364f242cdbabe6ef25c0efbd9;
        float4 Color_8b23519dc989478fa19369d8efda52f6;
        float Vector1_40661633426347afb1a705a362429b36;
        float4 Texture2D_c773e99a7970488a9d6bcbd08807a081_TexelSize;
        float Vector1_45a27133fb8b4113a0746c0d1954c57b;
        float Vector1_f133b62c64f14525895d4b7d5afdd79f;
        float Vector1_57cb41fdb69545e8b53a796761acb08d;
        float Vector1_76a8f3bdc13d493dbc42eca7e6de6c4e;
        float4 Texture2D_c23a14955d09487682029de7801dfc17_TexelSize;
        float Vector1_28676355978446f285fbfe6f1a574316;
        float Vector1_ba885a3188cc490b9d04d880f79cd1b0;
        float Vector1_bf79c34252fe4364976a552df388436c;
        float Vector1_f586ab191e3e497491345288306a18c0;
        float Vector1_4da6e98cbfa5408fb6352b485aa76d05;
        float Vector1_b266d7e71de2489ea084cb27060cacca;
        float Vector1_d09cc377b0524610970658ffdacd497f;
        float Vector1_1f93deba5516424f927cee1eb2c93ba3;
        float Vector1_cc6e37850778480781084b62719d58c3;
        float Vector1_5be3734798f64a63b2827794b79391c3;
        float Vector1_3578eca6d8ac4314939bfb16bfdbde27;
        float Vector1_775ef292c0cf4cda91aecf4845cd67bd;
        CBUFFER_END

        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(_CelShading_ad7871fb5bf1404e9f1fa53eab938a3a_Texture2D9666dd2f2f8f4fddb6c2a5c61ddfa9bd_3072536305);
        SAMPLER(sampler_CelShading_ad7871fb5bf1404e9f1fa53eab938a3a_Texture2D9666dd2f2f8f4fddb6c2a5c61ddfa9bd_3072536305);
        TEXTURE2D(Texture2D_c773e99a7970488a9d6bcbd08807a081);
        SAMPLER(samplerTexture2D_c773e99a7970488a9d6bcbd08807a081);
        TEXTURE2D(Texture2D_c23a14955d09487682029de7801dfc17);
        SAMPLER(samplerTexture2D_c23a14955d09487682029de7801dfc17);

            // Graph Functions
            
        // c9d40f89572cca24e7b23cdf7d4f0592
        #include "Assets/Rendering/Shaders/Functions/URPFonctions.hlsl"

        void Unity_Multiply_float(float A, float B, out float Out)
        {
            Out = A * B;
        }

        void Unity_Add_float3(float3 A, float3 B, out float3 Out)
        {
            Out = A + B;
        }

        struct Bindings_WaveDisplacement_81e878f76fb7ea94995742377f80ac36
        {
        };

        void SG_WaveDisplacement_81e878f76fb7ea94995742377f80ac36(float3 Vector3_774baa01c42a49f6abc99dad6b54a583, float Vector1_b53e914fc7984637b80324d57e274b5f, float Vector1_af1a9e8da22b4e08851f991318dec789, float Vector1_e937c75465ce4f99ba008e1a397c452b, Bindings_WaveDisplacement_81e878f76fb7ea94995742377f80ac36 IN, out float3 OutVector3_2)
        {
            float3 _Property_4f1419a54dd7457eb25ffd09e1294716_Out_0 = Vector3_774baa01c42a49f6abc99dad6b54a583;
            float _Property_61f840140291436e8912958fef5953c3_Out_0 = Vector1_b53e914fc7984637b80324d57e274b5f;
            float _Property_aa3d6115a8f7408182a5ad6eaf4dea9c_Out_0 = Vector1_af1a9e8da22b4e08851f991318dec789;
            float _Property_318bfb7c8fcb48359c265b1829bb9333_Out_0 = Vector1_e937c75465ce4f99ba008e1a397c452b;
            float _CalculateWaveCustomFunction_babfa0b8a0ce4e20b113bd73598d4cd6_New_0;
            CalculateWave_float(_Property_4f1419a54dd7457eb25ffd09e1294716_Out_0, _Property_61f840140291436e8912958fef5953c3_Out_0, _Property_aa3d6115a8f7408182a5ad6eaf4dea9c_Out_0, _Property_318bfb7c8fcb48359c265b1829bb9333_Out_0, _CalculateWaveCustomFunction_babfa0b8a0ce4e20b113bd73598d4cd6_New_0);
            float3 _Vector3_fd59b684dbbf45499ba224d658a9f2ec_Out_0 = float3(0, _CalculateWaveCustomFunction_babfa0b8a0ce4e20b113bd73598d4cd6_New_0, 0);
            float3 _Add_bb0b83c602bf41909e47fdeba507406b_Out_2;
            Unity_Add_float3(_Vector3_fd59b684dbbf45499ba224d658a9f2ec_Out_0, _Property_4f1419a54dd7457eb25ffd09e1294716_Out_0, _Add_bb0b83c602bf41909e47fdeba507406b_Out_2);
            OutVector3_2 = _Add_bb0b83c602bf41909e47fdeba507406b_Out_2;
        }

        void Unity_Sign_float(float In, out float Out)
        {
            Out = sign(In);
        }

        void Unity_Multiply_float(float3 A, float3 B, out float3 Out)
        {
            Out = A * B;
        }

        void Unity_Add_float(float A, float B, out float Out)
        {
            Out = A + B;
        }

        void Unity_Subtract_float(float A, float B, out float Out)
        {
            Out = A - B;
        }

        void Unity_Absolute_float(float In, out float Out)
        {
            Out = abs(In);
        }

        void Unity_Saturate_float(float In, out float Out)
        {
            Out = saturate(In);
        }

        void Unity_Divide_float(float A, float B, out float Out)
        {
            Out = A / B;
        }

        void Unity_Combine_float(float R, float G, float B, float A, out float4 RGBA, out float3 RGB, out float2 RG)
        {
            RGBA = float4(R, G, B, A);
            RGB = float3(R, G, B);
            RG = float2(R, G);
        }

        void Unity_Smoothstep_float(float Edge1, float Edge2, float In, out float Out)
        {
            Out = smoothstep(Edge1, Edge2, In);
        }

        void Unity_Lerp_float3(float3 A, float3 B, float3 T, out float3 Out)
        {
            Out = lerp(A, B, T);
        }

        void Unity_Subtract_float3(float3 A, float3 B, out float3 Out)
        {
            Out = A - B;
        }

        void Unity_Normalize_float3(float3 In, out float3 Out)
        {
            Out = normalize(In);
        }

        void Unity_CrossProduct_float(float3 A, float3 B, out float3 Out)
        {
            Out = cross(A, B);
        }

        struct Bindings_Neighbors_df68ff96a41468e448052764fa9bdba1
        {
            float3 ObjectSpaceNormal;
        };

        void SG_Neighbors_df68ff96a41468e448052764fa9bdba1(float3 Vector3_8954E4F1, float Vector1_B5D26639, Bindings_Neighbors_df68ff96a41468e448052764fa9bdba1 IN, out float3 Neighbor1_1, out float3 Neighbor2_2)
        {
            float _Split_609f2b09351ec288b5ed925103f0f9e0_R_1 = IN.ObjectSpaceNormal[0];
            float _Split_609f2b09351ec288b5ed925103f0f9e0_G_2 = IN.ObjectSpaceNormal[1];
            float _Split_609f2b09351ec288b5ed925103f0f9e0_B_3 = IN.ObjectSpaceNormal[2];
            float _Split_609f2b09351ec288b5ed925103f0f9e0_A_4 = 0;
            float _Sign_e3911d5b4df6208882d5722613bfa7df_Out_1;
            Unity_Sign_float(_Split_609f2b09351ec288b5ed925103f0f9e0_R_1, _Sign_e3911d5b4df6208882d5722613bfa7df_Out_1);
            float3 _Property_8a0179cbbf558586ab0af83d3af77169_Out_0 = Vector3_8954E4F1;
            float3 _Multiply_a5a5ef928369cd84997685e6caac87b0_Out_2;
            Unity_Multiply_float(_Property_8a0179cbbf558586ab0af83d3af77169_Out_0, IN.ObjectSpaceNormal, _Multiply_a5a5ef928369cd84997685e6caac87b0_Out_2);
            float _Split_8977fb298fca9e8182c1edaad752997c_R_1 = _Multiply_a5a5ef928369cd84997685e6caac87b0_Out_2[0];
            float _Split_8977fb298fca9e8182c1edaad752997c_G_2 = _Multiply_a5a5ef928369cd84997685e6caac87b0_Out_2[1];
            float _Split_8977fb298fca9e8182c1edaad752997c_B_3 = _Multiply_a5a5ef928369cd84997685e6caac87b0_Out_2[2];
            float _Split_8977fb298fca9e8182c1edaad752997c_A_4 = 0;
            float _Add_eeda377c8b7c038596bc86d83f7f321e_Out_2;
            Unity_Add_float(_Split_8977fb298fca9e8182c1edaad752997c_R_1, _Split_8977fb298fca9e8182c1edaad752997c_G_2, _Add_eeda377c8b7c038596bc86d83f7f321e_Out_2);
            float _Add_61a089896fe3468b82b211a762b6ec21_Out_2;
            Unity_Add_float(_Add_eeda377c8b7c038596bc86d83f7f321e_Out_2, _Split_8977fb298fca9e8182c1edaad752997c_B_3, _Add_61a089896fe3468b82b211a762b6ec21_Out_2);
            float _Split_d79bc2f0568d4b8586a29273c4b01ac3_R_1 = IN.ObjectSpaceNormal[0];
            float _Split_d79bc2f0568d4b8586a29273c4b01ac3_G_2 = IN.ObjectSpaceNormal[1];
            float _Split_d79bc2f0568d4b8586a29273c4b01ac3_B_3 = IN.ObjectSpaceNormal[2];
            float _Split_d79bc2f0568d4b8586a29273c4b01ac3_A_4 = 0;
            float _Float_9d499dd7c2614a8589e8a1cb04daeb5d_Out_0 = 0.05;
            float _Split_802c7c2944faa986ba720f07d8db74ab_R_1 = _Property_8a0179cbbf558586ab0af83d3af77169_Out_0[0];
            float _Split_802c7c2944faa986ba720f07d8db74ab_G_2 = _Property_8a0179cbbf558586ab0af83d3af77169_Out_0[1];
            float _Split_802c7c2944faa986ba720f07d8db74ab_B_3 = _Property_8a0179cbbf558586ab0af83d3af77169_Out_0[2];
            float _Split_802c7c2944faa986ba720f07d8db74ab_A_4 = 0;
            float _Add_4555b76b861a778d97276d765af07483_Out_2;
            Unity_Add_float(_Float_9d499dd7c2614a8589e8a1cb04daeb5d_Out_0, _Split_802c7c2944faa986ba720f07d8db74ab_G_2, _Add_4555b76b861a778d97276d765af07483_Out_2);
            float _Multiply_1e591f42bea8b58c8797ab2c90faa345_Out_2;
            Unity_Multiply_float(_Split_d79bc2f0568d4b8586a29273c4b01ac3_G_2, _Add_4555b76b861a778d97276d765af07483_Out_2, _Multiply_1e591f42bea8b58c8797ab2c90faa345_Out_2);
            float _Subtract_68ba98ef07340b83b46992e63c68d7ca_Out_2;
            Unity_Subtract_float(_Add_61a089896fe3468b82b211a762b6ec21_Out_2, _Multiply_1e591f42bea8b58c8797ab2c90faa345_Out_2, _Subtract_68ba98ef07340b83b46992e63c68d7ca_Out_2);
            float _Multiply_d0572ed2941ee0879549c1da1c572b15_Out_2;
            Unity_Multiply_float(_Split_d79bc2f0568d4b8586a29273c4b01ac3_B_3, _Split_802c7c2944faa986ba720f07d8db74ab_B_3, _Multiply_d0572ed2941ee0879549c1da1c572b15_Out_2);
            float _Subtract_3d9ad256eba2ee8e839158fc0f45ee81_Out_2;
            Unity_Subtract_float(_Subtract_68ba98ef07340b83b46992e63c68d7ca_Out_2, _Multiply_d0572ed2941ee0879549c1da1c572b15_Out_2, _Subtract_3d9ad256eba2ee8e839158fc0f45ee81_Out_2);
            float _Float_654fffd8f366ff808a39bb5a6a1f1fdb_Out_0 = 0.01;
            float _Absolute_9fde5c96b588aa8fbed6af91404da630_Out_1;
            Unity_Absolute_float(_Split_609f2b09351ec288b5ed925103f0f9e0_R_1, _Absolute_9fde5c96b588aa8fbed6af91404da630_Out_1);
            float _Subtract_7173428a9289a58abbac98045c08ceb9_Out_2;
            Unity_Subtract_float(_Absolute_9fde5c96b588aa8fbed6af91404da630_Out_1, _Float_654fffd8f366ff808a39bb5a6a1f1fdb_Out_0, _Subtract_7173428a9289a58abbac98045c08ceb9_Out_2);
            float _Saturate_e8fcca777a01738ab601adda864265d1_Out_1;
            Unity_Saturate_float(_Subtract_7173428a9289a58abbac98045c08ceb9_Out_2, _Saturate_e8fcca777a01738ab601adda864265d1_Out_1);
            float _Add_71e184a7749fc9848483bc81af14e30e_Out_2;
            Unity_Add_float(_Float_654fffd8f366ff808a39bb5a6a1f1fdb_Out_0, _Saturate_e8fcca777a01738ab601adda864265d1_Out_1, _Add_71e184a7749fc9848483bc81af14e30e_Out_2);
            float _Divide_7cc37db0bc8630899cffaa8d565d6d55_Out_2;
            Unity_Divide_float(_Subtract_3d9ad256eba2ee8e839158fc0f45ee81_Out_2, _Add_71e184a7749fc9848483bc81af14e30e_Out_2, _Divide_7cc37db0bc8630899cffaa8d565d6d55_Out_2);
            float _Multiply_9c6b2289e8d2db84a1c0d5ca2a655b93_Out_2;
            Unity_Multiply_float(_Sign_e3911d5b4df6208882d5722613bfa7df_Out_1, _Divide_7cc37db0bc8630899cffaa8d565d6d55_Out_2, _Multiply_9c6b2289e8d2db84a1c0d5ca2a655b93_Out_2);
            float4 _Combine_8fcb17287675378fb024a84210c6677a_RGBA_4;
            float3 _Combine_8fcb17287675378fb024a84210c6677a_RGB_5;
            float2 _Combine_8fcb17287675378fb024a84210c6677a_RG_6;
            Unity_Combine_float(_Multiply_9c6b2289e8d2db84a1c0d5ca2a655b93_Out_2, _Add_4555b76b861a778d97276d765af07483_Out_2, _Split_802c7c2944faa986ba720f07d8db74ab_B_3, 0, _Combine_8fcb17287675378fb024a84210c6677a_RGBA_4, _Combine_8fcb17287675378fb024a84210c6677a_RGB_5, _Combine_8fcb17287675378fb024a84210c6677a_RG_6);
            float _Float_9b0eee4975b51e828a4269a5edcee5b1_Out_0 = 0.05;
            float _Split_f205281c3443d68aa9b0c1a2feace9df_R_1 = _Property_8a0179cbbf558586ab0af83d3af77169_Out_0[0];
            float _Split_f205281c3443d68aa9b0c1a2feace9df_G_2 = _Property_8a0179cbbf558586ab0af83d3af77169_Out_0[1];
            float _Split_f205281c3443d68aa9b0c1a2feace9df_B_3 = _Property_8a0179cbbf558586ab0af83d3af77169_Out_0[2];
            float _Split_f205281c3443d68aa9b0c1a2feace9df_A_4 = 0;
            float _Add_db37e0cfea249d8d9ede45e675501301_Out_2;
            Unity_Add_float(_Float_9b0eee4975b51e828a4269a5edcee5b1_Out_0, _Split_f205281c3443d68aa9b0c1a2feace9df_R_1, _Add_db37e0cfea249d8d9ede45e675501301_Out_2);
            float4 _Combine_9530e51a49a77f849e6eac5cb0ce15ad_RGBA_4;
            float3 _Combine_9530e51a49a77f849e6eac5cb0ce15ad_RGB_5;
            float2 _Combine_9530e51a49a77f849e6eac5cb0ce15ad_RG_6;
            Unity_Combine_float(_Add_db37e0cfea249d8d9ede45e675501301_Out_2, _Split_f205281c3443d68aa9b0c1a2feace9df_G_2, _Split_f205281c3443d68aa9b0c1a2feace9df_B_3, 0, _Combine_9530e51a49a77f849e6eac5cb0ce15ad_RGBA_4, _Combine_9530e51a49a77f849e6eac5cb0ce15ad_RGB_5, _Combine_9530e51a49a77f849e6eac5cb0ce15ad_RG_6);
            float _Multiply_625b1e551cdfd0829aa19290646e6739_Out_2;
            Unity_Multiply_float(-1, _Subtract_7173428a9289a58abbac98045c08ceb9_Out_2, _Multiply_625b1e551cdfd0829aa19290646e6739_Out_2);
            float _Smoothstep_b60ff4818667938f9be3fa9ecf03f163_Out_3;
            Unity_Smoothstep_float(0, 0.01, _Multiply_625b1e551cdfd0829aa19290646e6739_Out_2, _Smoothstep_b60ff4818667938f9be3fa9ecf03f163_Out_3);
            float3 _Lerp_7d115f30f5f0fc8cb46b9ba33440e47a_Out_3;
            Unity_Lerp_float3(_Combine_8fcb17287675378fb024a84210c6677a_RGB_5, _Combine_9530e51a49a77f849e6eac5cb0ce15ad_RGB_5, (_Smoothstep_b60ff4818667938f9be3fa9ecf03f163_Out_3.xxx), _Lerp_7d115f30f5f0fc8cb46b9ba33440e47a_Out_3);
            float3 _Subtract_24a46f67e2876e80b3b01e0b77a50456_Out_2;
            Unity_Subtract_float3(_Lerp_7d115f30f5f0fc8cb46b9ba33440e47a_Out_3, _Property_8a0179cbbf558586ab0af83d3af77169_Out_0, _Subtract_24a46f67e2876e80b3b01e0b77a50456_Out_2);
            float3 _Normalize_f544245d7bd37f8fbaee4a87c2403675_Out_1;
            Unity_Normalize_float3(_Subtract_24a46f67e2876e80b3b01e0b77a50456_Out_2, _Normalize_f544245d7bd37f8fbaee4a87c2403675_Out_1);
            float3 _Normalize_9a359141b951ea84b3220ad5c3123b7c_Out_1;
            Unity_Normalize_float3(IN.ObjectSpaceNormal, _Normalize_9a359141b951ea84b3220ad5c3123b7c_Out_1);
            float3 _CrossProduct_1dbde50ff8c546829c8b59ec1630c252_Out_2;
            Unity_CrossProduct_float(_Normalize_f544245d7bd37f8fbaee4a87c2403675_Out_1, _Normalize_9a359141b951ea84b3220ad5c3123b7c_Out_1, _CrossProduct_1dbde50ff8c546829c8b59ec1630c252_Out_2);
            float3 _Normalize_3eb509f6a593a18d856dabb44191008a_Out_1;
            Unity_Normalize_float3(_CrossProduct_1dbde50ff8c546829c8b59ec1630c252_Out_2, _Normalize_3eb509f6a593a18d856dabb44191008a_Out_1);
            float _Property_4448eb2908e4228e840b80f046abf12e_Out_0 = Vector1_B5D26639;
            float3 _Multiply_37bf5602003a2885bb744b13f93296fe_Out_2;
            Unity_Multiply_float(_Normalize_3eb509f6a593a18d856dabb44191008a_Out_1, (_Property_4448eb2908e4228e840b80f046abf12e_Out_0.xxx), _Multiply_37bf5602003a2885bb744b13f93296fe_Out_2);
            float3 _Add_bd2759426a510983979415cc0e46c2d4_Out_2;
            Unity_Add_float3(_Multiply_37bf5602003a2885bb744b13f93296fe_Out_2, _Property_8a0179cbbf558586ab0af83d3af77169_Out_0, _Add_bd2759426a510983979415cc0e46c2d4_Out_2);
            float3 _Multiply_273e0ecf939b858582031df2660ec861_Out_2;
            Unity_Multiply_float(_Normalize_f544245d7bd37f8fbaee4a87c2403675_Out_1, (_Property_4448eb2908e4228e840b80f046abf12e_Out_0.xxx), _Multiply_273e0ecf939b858582031df2660ec861_Out_2);
            float3 _Add_2259c39580c12b8297f6b81a426bd3c0_Out_2;
            Unity_Add_float3(_Property_8a0179cbbf558586ab0af83d3af77169_Out_0, _Multiply_273e0ecf939b858582031df2660ec861_Out_2, _Add_2259c39580c12b8297f6b81a426bd3c0_Out_2);
            Neighbor1_1 = _Add_bd2759426a510983979415cc0e46c2d4_Out_2;
            Neighbor2_2 = _Add_2259c39580c12b8297f6b81a426bd3c0_Out_2;
        }

        struct Bindings_NewNormal_c8d6b0224f233004190264cd9e5ca192
        {
        };

        void SG_NewNormal_c8d6b0224f233004190264cd9e5ca192(float3 Vector3_988291EB, float3 Vector3_EB4E2DE7, float3 Vector3_1973C67A, Bindings_NewNormal_c8d6b0224f233004190264cd9e5ca192 IN, out float3 OutVector3_1)
        {
            float3 _Property_fd61227e10c3348bb61b5f1d39e68328_Out_0 = Vector3_EB4E2DE7;
            float3 _Property_b8cacddd74e568809402e877976dffa8_Out_0 = Vector3_988291EB;
            float3 _Subtract_1f3c648b5559d28f9acfbe1af362b37b_Out_2;
            Unity_Subtract_float3(_Property_fd61227e10c3348bb61b5f1d39e68328_Out_0, _Property_b8cacddd74e568809402e877976dffa8_Out_0, _Subtract_1f3c648b5559d28f9acfbe1af362b37b_Out_2);
            float3 _Normalize_888167f431fbc788b4ad8e2092e7f28b_Out_1;
            Unity_Normalize_float3(_Subtract_1f3c648b5559d28f9acfbe1af362b37b_Out_2, _Normalize_888167f431fbc788b4ad8e2092e7f28b_Out_1);
            float3 _Property_6008e654ce04488cbe1d5caac8afe08a_Out_0 = Vector3_1973C67A;
            float3 _Subtract_208696e2abb5f18d84a5878fd9a81553_Out_2;
            Unity_Subtract_float3(_Property_6008e654ce04488cbe1d5caac8afe08a_Out_0, _Property_b8cacddd74e568809402e877976dffa8_Out_0, _Subtract_208696e2abb5f18d84a5878fd9a81553_Out_2);
            float3 _Normalize_7250d413c7450d8aaa805c923843f2dd_Out_1;
            Unity_Normalize_float3(_Subtract_208696e2abb5f18d84a5878fd9a81553_Out_2, _Normalize_7250d413c7450d8aaa805c923843f2dd_Out_1);
            float3 _CrossProduct_5566a5ab348aa8809609d834bbe4451f_Out_2;
            Unity_CrossProduct_float(_Normalize_888167f431fbc788b4ad8e2092e7f28b_Out_1, _Normalize_7250d413c7450d8aaa805c923843f2dd_Out_1, _CrossProduct_5566a5ab348aa8809609d834bbe4451f_Out_2);
            float3 _Normalize_a640075232b8c78498848f10ca1e7ba8_Out_1;
            Unity_Normalize_float3(_CrossProduct_5566a5ab348aa8809609d834bbe4451f_Out_2, _Normalize_a640075232b8c78498848f10ca1e7ba8_Out_1);
            OutVector3_1 = _Normalize_a640075232b8c78498848f10ca1e7ba8_Out_1;
        }

        void Unity_SceneDepth_Linear01_float(float4 UV, out float Out)
        {
            Out = Linear01Depth(SHADERGRAPH_SAMPLE_SCENE_DEPTH(UV.xy), _ZBufferParams);
        }

        void Unity_Clamp_float(float In, float Min, float Max, out float Out)
        {
            Out = clamp(In, Min, Max);
        }

        void Unity_Power_float(float A, float B, out float Out)
        {
            Out = pow(A, B);
        }

        void Unity_FresnelEffect_float(float3 Normal, float3 ViewDir, float Power, out float Out)
        {
            Out = pow((1.0 - saturate(dot(normalize(Normal), normalize(ViewDir)))), Power);
        }

        void Unity_Maximum_float(float A, float B, out float Out)
        {
            Out = max(A, B);
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
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            UnityTexture2D _Property_52e3e5c2509c4df984c47cc092c466d8_Out_0 = UnityBuildTexture2DStructNoScale(Texture2D_c773e99a7970488a9d6bcbd08807a081);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Property_736f2734d3e643c398a2586801e4cc17_Out_0 = Vector1_f133b62c64f14525895d4b7d5afdd79f;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Property_7851f92dfd4d4302a7beb8a17dd4bd8b_Out_0 = Vector1_45a27133fb8b4113a0746c0d1954c57b;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float2 _CalculateNoiseCustomFunction_a0c2603a229c4d299ac32a59a8664765_Out_0;
            CalculateNoise_float(IN.AbsoluteWorldSpacePosition, _Property_736f2734d3e643c398a2586801e4cc17_Out_0, _Property_7851f92dfd4d4302a7beb8a17dd4bd8b_Out_0, _CalculateNoiseCustomFunction_a0c2603a229c4d299ac32a59a8664765_Out_0);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            #if defined(SHADER_API_GLES) && (SHADER_TARGET < 30)
              float4 _SampleTexture2DLOD_fe1fede5211b477cbca93fc4c2f76d12_RGBA_0 = float4(0.0f, 0.0f, 0.0f, 1.0f);
            #else
              float4 _SampleTexture2DLOD_fe1fede5211b477cbca93fc4c2f76d12_RGBA_0 = SAMPLE_TEXTURE2D_LOD(_Property_52e3e5c2509c4df984c47cc092c466d8_Out_0.tex, _Property_52e3e5c2509c4df984c47cc092c466d8_Out_0.samplerstate, _CalculateNoiseCustomFunction_a0c2603a229c4d299ac32a59a8664765_Out_0, 0);
            #endif
            float _SampleTexture2DLOD_fe1fede5211b477cbca93fc4c2f76d12_R_5 = _SampleTexture2DLOD_fe1fede5211b477cbca93fc4c2f76d12_RGBA_0.r;
            float _SampleTexture2DLOD_fe1fede5211b477cbca93fc4c2f76d12_G_6 = _SampleTexture2DLOD_fe1fede5211b477cbca93fc4c2f76d12_RGBA_0.g;
            float _SampleTexture2DLOD_fe1fede5211b477cbca93fc4c2f76d12_B_7 = _SampleTexture2DLOD_fe1fede5211b477cbca93fc4c2f76d12_RGBA_0.b;
            float _SampleTexture2DLOD_fe1fede5211b477cbca93fc4c2f76d12_A_8 = _SampleTexture2DLOD_fe1fede5211b477cbca93fc4c2f76d12_RGBA_0.a;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Property_fab5f23c0a9e449bb605bc77e42f5c55_Out_0 = Vector1_57cb41fdb69545e8b53a796761acb08d;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Multiply_be99c4e752404fa4b5a42166d27a7cc0_Out_2;
            Unity_Multiply_float(_SampleTexture2DLOD_fe1fede5211b477cbca93fc4c2f76d12_R_5, _Property_fab5f23c0a9e449bb605bc77e42f5c55_Out_0, _Multiply_be99c4e752404fa4b5a42166d27a7cc0_Out_2);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float3 _Vector3_9e43fda8531d474c890ea7595c453773_Out_0 = float3(0, _Multiply_be99c4e752404fa4b5a42166d27a7cc0_Out_2, 0);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Property_0ea561f00aaf4fbb8fdfeab5a2a0dd08_Out_0 = Vector1_4da6e98cbfa5408fb6352b485aa76d05;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Property_e4937aaae9bc4c098cebea935a7ee1ea_Out_0 = Vector1_b266d7e71de2489ea084cb27060cacca;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Property_2d3470f269ac4477a8dc467d895fe472_Out_0 = Vector1_d09cc377b0524610970658ffdacd497f;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            Bindings_WaveDisplacement_81e878f76fb7ea94995742377f80ac36 _WaveDisplacement_5f54fff0c6334b26bedfff0b63df12b3;
            float3 _WaveDisplacement_5f54fff0c6334b26bedfff0b63df12b3_OutVector3_2;
            SG_WaveDisplacement_81e878f76fb7ea94995742377f80ac36(IN.AbsoluteWorldSpacePosition, _Property_0ea561f00aaf4fbb8fdfeab5a2a0dd08_Out_0, _Property_e4937aaae9bc4c098cebea935a7ee1ea_Out_0, _Property_2d3470f269ac4477a8dc467d895fe472_Out_0, _WaveDisplacement_5f54fff0c6334b26bedfff0b63df12b3, _WaveDisplacement_5f54fff0c6334b26bedfff0b63df12b3_OutVector3_2);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float3 _Add_942b46ad98eb4925b4bfa00a0a5781c7_Out_2;
            Unity_Add_float3(_Vector3_9e43fda8531d474c890ea7595c453773_Out_0, _WaveDisplacement_5f54fff0c6334b26bedfff0b63df12b3_OutVector3_2, _Add_942b46ad98eb4925b4bfa00a0a5781c7_Out_2);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float3 _Transform_3fce2cebd3054a72a946f448c89c2e3d_Out_1 = TransformWorldToObject(GetCameraRelativePositionWS(_Add_942b46ad98eb4925b4bfa00a0a5781c7_Out_2.xyz));
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            Bindings_Neighbors_df68ff96a41468e448052764fa9bdba1 _Neighbors_e648c110f4754cefb6491b8c82fe074f;
            _Neighbors_e648c110f4754cefb6491b8c82fe074f.ObjectSpaceNormal = IN.ObjectSpaceNormal;
            float3 _Neighbors_e648c110f4754cefb6491b8c82fe074f_Neighbor1_1;
            float3 _Neighbors_e648c110f4754cefb6491b8c82fe074f_Neighbor2_2;
            SG_Neighbors_df68ff96a41468e448052764fa9bdba1(IN.AbsoluteWorldSpacePosition, 0.01, _Neighbors_e648c110f4754cefb6491b8c82fe074f, _Neighbors_e648c110f4754cefb6491b8c82fe074f_Neighbor1_1, _Neighbors_e648c110f4754cefb6491b8c82fe074f_Neighbor2_2);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            Bindings_WaveDisplacement_81e878f76fb7ea94995742377f80ac36 _WaveDisplacement_2fa36261c2534cbe8639a41fdfe4910a;
            float3 _WaveDisplacement_2fa36261c2534cbe8639a41fdfe4910a_OutVector3_2;
            SG_WaveDisplacement_81e878f76fb7ea94995742377f80ac36(_Neighbors_e648c110f4754cefb6491b8c82fe074f_Neighbor1_1, _Property_0ea561f00aaf4fbb8fdfeab5a2a0dd08_Out_0, _Property_e4937aaae9bc4c098cebea935a7ee1ea_Out_0, _Property_2d3470f269ac4477a8dc467d895fe472_Out_0, _WaveDisplacement_2fa36261c2534cbe8639a41fdfe4910a, _WaveDisplacement_2fa36261c2534cbe8639a41fdfe4910a_OutVector3_2);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            Bindings_WaveDisplacement_81e878f76fb7ea94995742377f80ac36 _WaveDisplacement_c9fd16638edd4ccfaf9e3eb2efa55bfa;
            float3 _WaveDisplacement_c9fd16638edd4ccfaf9e3eb2efa55bfa_OutVector3_2;
            SG_WaveDisplacement_81e878f76fb7ea94995742377f80ac36(_Neighbors_e648c110f4754cefb6491b8c82fe074f_Neighbor2_2, _Property_0ea561f00aaf4fbb8fdfeab5a2a0dd08_Out_0, _Property_e4937aaae9bc4c098cebea935a7ee1ea_Out_0, _Property_2d3470f269ac4477a8dc467d895fe472_Out_0, _WaveDisplacement_c9fd16638edd4ccfaf9e3eb2efa55bfa, _WaveDisplacement_c9fd16638edd4ccfaf9e3eb2efa55bfa_OutVector3_2);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            Bindings_NewNormal_c8d6b0224f233004190264cd9e5ca192 _NewNormal_e9af9405a4a54034863b00f2fbd4c2b6;
            float3 _NewNormal_e9af9405a4a54034863b00f2fbd4c2b6_OutVector3_1;
            SG_NewNormal_c8d6b0224f233004190264cd9e5ca192(_WaveDisplacement_5f54fff0c6334b26bedfff0b63df12b3_OutVector3_2, _WaveDisplacement_2fa36261c2534cbe8639a41fdfe4910a_OutVector3_2, _WaveDisplacement_c9fd16638edd4ccfaf9e3eb2efa55bfa_OutVector3_2, _NewNormal_e9af9405a4a54034863b00f2fbd4c2b6, _NewNormal_e9af9405a4a54034863b00f2fbd4c2b6_OutVector3_1);
            #endif
            description.Position = _Transform_3fce2cebd3054a72a946f448c89c2e3d_Out_1;
            description.Normal = _NewNormal_e9af9405a4a54034863b00f2fbd4c2b6_OutVector3_1;
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
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _SceneDepth_7981f193eac94d87b6f2b2920d4cb528_Out_1;
            Unity_SceneDepth_Linear01_float(float4(IN.ScreenPosition.xy / IN.ScreenPosition.w, 0, 0), _SceneDepth_7981f193eac94d87b6f2b2920d4cb528_Out_1);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Multiply_abe194398c584d8a8934b67cf64c0da1_Out_2;
            Unity_Multiply_float(_SceneDepth_7981f193eac94d87b6f2b2920d4cb528_Out_1, _ProjectionParams.z, _Multiply_abe194398c584d8a8934b67cf64c0da1_Out_2);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float4 _ScreenPosition_68dc8d80a07f4706a6a9a2f0230737ed_Out_0 = IN.ScreenPosition;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Split_7f7e1aadecd54073aff51bd05fa759bc_R_1 = _ScreenPosition_68dc8d80a07f4706a6a9a2f0230737ed_Out_0[0];
            float _Split_7f7e1aadecd54073aff51bd05fa759bc_G_2 = _ScreenPosition_68dc8d80a07f4706a6a9a2f0230737ed_Out_0[1];
            float _Split_7f7e1aadecd54073aff51bd05fa759bc_B_3 = _ScreenPosition_68dc8d80a07f4706a6a9a2f0230737ed_Out_0[2];
            float _Split_7f7e1aadecd54073aff51bd05fa759bc_A_4 = _ScreenPosition_68dc8d80a07f4706a6a9a2f0230737ed_Out_0[3];
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Multiply_7ba2c47eef0f4b89b3b4eb4a8ed31f83_Out_2;
            Unity_Multiply_float(_Split_7f7e1aadecd54073aff51bd05fa759bc_A_4, 1, _Multiply_7ba2c47eef0f4b89b3b4eb4a8ed31f83_Out_2);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Subtract_dc119b36f453437e913cdd2a2623efd2_Out_2;
            Unity_Subtract_float(_Multiply_abe194398c584d8a8934b67cf64c0da1_Out_2, _Multiply_7ba2c47eef0f4b89b3b4eb4a8ed31f83_Out_2, _Subtract_dc119b36f453437e913cdd2a2623efd2_Out_2);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Property_ab48707fe6db419eb51ece316bb6313b_Out_0 = Vector1_cc6e37850778480781084b62719d58c3;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Multiply_3a48e77d57694a868985ec140162c2ef_Out_2;
            Unity_Multiply_float(_Subtract_dc119b36f453437e913cdd2a2623efd2_Out_2, _Property_ab48707fe6db419eb51ece316bb6313b_Out_0, _Multiply_3a48e77d57694a868985ec140162c2ef_Out_2);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Clamp_bb886dbf030149bd8ffa4e979d6e9c46_Out_3;
            Unity_Clamp_float(_Multiply_3a48e77d57694a868985ec140162c2ef_Out_2, 0, 1, _Clamp_bb886dbf030149bd8ffa4e979d6e9c46_Out_3);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Power_795157e2d171465e908da5d45e938d28_Out_2;
            Unity_Power_float(_Clamp_bb886dbf030149bd8ffa4e979d6e9c46_Out_3, 0.7, _Power_795157e2d171465e908da5d45e938d28_Out_2);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Property_41daac0ae2c74098ac9111219e1e3b3b_Out_0 = Vector1_5be3734798f64a63b2827794b79391c3;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _FresnelEffect_98bea57f43804c61a33cabcb5a3e5400_Out_3;
            Unity_FresnelEffect_float(IN.WorldSpaceNormal, IN.WorldSpaceViewDirection, _Property_41daac0ae2c74098ac9111219e1e3b3b_Out_0, _FresnelEffect_98bea57f43804c61a33cabcb5a3e5400_Out_3);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Maximum_85ae2b8b22d5495fb3e04a01fa179389_Out_2;
            Unity_Maximum_float(_FresnelEffect_98bea57f43804c61a33cabcb5a3e5400_Out_3, 0.7, _Maximum_85ae2b8b22d5495fb3e04a01fa179389_Out_2);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Multiply_465f2016ad9d499981e3a39ae56b8aef_Out_2;
            Unity_Multiply_float(_Power_795157e2d171465e908da5d45e938d28_Out_2, _Maximum_85ae2b8b22d5495fb3e04a01fa179389_Out_2, _Multiply_465f2016ad9d499981e3a39ae56b8aef_Out_2);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Saturate_00659ca349c74eea975fbb11da46e6bf_Out_1;
            Unity_Saturate_float(_Multiply_465f2016ad9d499981e3a39ae56b8aef_Out_2, _Saturate_00659ca349c74eea975fbb11da46e6bf_Out_1);
            #endif
            surface.Alpha = _Saturate_00659ca349c74eea975fbb11da46e6bf_Out_1;
            return surface;
        }

            // --------------------------------------------------
            // Build Graph Inputs

            VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
        {
            VertexDescriptionInputs output;
            ZERO_INITIALIZE(VertexDescriptionInputs, output);

        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        output.ObjectSpaceNormal =           input.normalOS;
        #endif

        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        output.WorldSpaceNormal =            TransformObjectToWorldNormal(input.normalOS);
        #endif

        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        output.ObjectSpaceTangent =          input.tangentOS.xyz;
        #endif

        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        output.WorldSpaceTangent =           TransformObjectToWorldDir(input.tangentOS.xyz);
        #endif

        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        output.ObjectSpaceBiTangent =        normalize(cross(input.normalOS, input.tangentOS) * (input.tangentOS.w > 0.0f ? 1.0f : -1.0f) * GetOddNegativeScale());
        #endif

        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        output.WorldSpaceBiTangent =         TransformObjectToWorldDir(output.ObjectSpaceBiTangent);
        #endif

        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        output.ObjectSpacePosition =         input.positionOS;
        #endif

        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        output.AbsoluteWorldSpacePosition =  GetAbsolutePositionWS(TransformObjectToWorld(input.positionOS));
        #endif


            return output;
        }
            SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);

        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        // must use interpolated tangent, bitangent and normal before they are normalized in the pixel shader.
        #endif

        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        float3 unnormalizedNormalWS = input.normalWS;
        #endif

        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        const float renormFactor = 1.0 / length(unnormalizedNormalWS);
        #endif



        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        output.WorldSpaceNormal =            renormFactor*input.normalWS.xyz;		// we want a unit length Normal Vector node in shader graph
        #endif



        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        output.WorldSpaceViewDirection =     input.viewDirectionWS; //TODO: by default normalized in HD, but not in universal
        #endif

        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        output.WorldSpacePosition =          input.positionWS;
        #endif

        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        output.ScreenPosition =              ComputeScreenPos(TransformWorldToHClip(input.positionWS), _ProjectionParams.x);
        #endif

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
            #pragma multi_compile_local _ CEL_SPECULAR_ON

        #if defined(CEL_SPECULAR_ON)
            #define KEYWORD_PERMUTATION_0
        #else
            #define KEYWORD_PERMUTATION_1
        #endif


            // Defines
        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        #define _SURFACE_TYPE_TRANSPARENT 1
        #endif

        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        #define ATTRIBUTES_NEED_NORMAL
        #endif

        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        #define ATTRIBUTES_NEED_TANGENT
        #endif

        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        #define VARYINGS_NEED_POSITION_WS
        #endif

        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        #define VARYINGS_NEED_NORMAL_WS
        #endif

        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        #define VARYINGS_NEED_VIEWDIRECTION_WS
        #endif

            #define FEATURES_GRAPH_VERTEX
            /* WARNING: $splice Could not find named fragment 'PassInstancing' */
            #define SHADERPASS SHADERPASS_DEPTHONLY
        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        #define REQUIRE_DEPTH_TEXTURE
        #endif
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
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float3 positionOS : POSITION;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float3 normalOS : NORMAL;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float4 tangentOS : TANGENT;
            #endif
            #if UNITY_ANY_INSTANCING_ENABLED
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            uint instanceID : INSTANCEID_SEMANTIC;
            #endif
            #endif
        };
        struct Varyings
        {
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float4 positionCS : SV_POSITION;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float3 positionWS;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float3 normalWS;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float3 viewDirectionWS;
            #endif
            #if UNITY_ANY_INSTANCING_ENABLED
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
            #endif
        };
        struct SurfaceDescriptionInputs
        {
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float3 WorldSpaceNormal;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float3 WorldSpaceViewDirection;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float3 WorldSpacePosition;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float4 ScreenPosition;
            #endif
        };
        struct VertexDescriptionInputs
        {
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float3 ObjectSpaceNormal;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float3 WorldSpaceNormal;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float3 ObjectSpaceTangent;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float3 WorldSpaceTangent;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float3 ObjectSpaceBiTangent;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float3 WorldSpaceBiTangent;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float3 ObjectSpacePosition;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float3 AbsoluteWorldSpacePosition;
            #endif
        };
        struct PackedVaryings
        {
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float4 positionCS : SV_POSITION;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float3 interp0 : TEXCOORD0;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float3 interp1 : TEXCOORD1;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float3 interp2 : TEXCOORD2;
            #endif
            #if UNITY_ANY_INSTANCING_ENABLED
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
            #endif
        };

            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        PackedVaryings PackVaryings (Varyings input)
        {
            PackedVaryings output;
            output.positionCS = input.positionCS;
            output.interp0.xyz =  input.positionWS;
            output.interp1.xyz =  input.normalWS;
            output.interp2.xyz =  input.viewDirectionWS;
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
            output.viewDirectionWS = input.interp2.xyz;
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
        #endif

            // --------------------------------------------------
            // Graph

            // Graph Properties
            CBUFFER_START(UnityPerMaterial)
        float4 _CelShading_ad7871fb5bf1404e9f1fa53eab938a3a_Texture2D9666dd2f2f8f4fddb6c2a5c61ddfa9bd_3072536305_TexelSize;
        float Vector1_0e212c6562134ef7add6f6766d6419b4;
        float4 Color_213be8c364f242cdbabe6ef25c0efbd9;
        float4 Color_8b23519dc989478fa19369d8efda52f6;
        float Vector1_40661633426347afb1a705a362429b36;
        float4 Texture2D_c773e99a7970488a9d6bcbd08807a081_TexelSize;
        float Vector1_45a27133fb8b4113a0746c0d1954c57b;
        float Vector1_f133b62c64f14525895d4b7d5afdd79f;
        float Vector1_57cb41fdb69545e8b53a796761acb08d;
        float Vector1_76a8f3bdc13d493dbc42eca7e6de6c4e;
        float4 Texture2D_c23a14955d09487682029de7801dfc17_TexelSize;
        float Vector1_28676355978446f285fbfe6f1a574316;
        float Vector1_ba885a3188cc490b9d04d880f79cd1b0;
        float Vector1_bf79c34252fe4364976a552df388436c;
        float Vector1_f586ab191e3e497491345288306a18c0;
        float Vector1_4da6e98cbfa5408fb6352b485aa76d05;
        float Vector1_b266d7e71de2489ea084cb27060cacca;
        float Vector1_d09cc377b0524610970658ffdacd497f;
        float Vector1_1f93deba5516424f927cee1eb2c93ba3;
        float Vector1_cc6e37850778480781084b62719d58c3;
        float Vector1_5be3734798f64a63b2827794b79391c3;
        float Vector1_3578eca6d8ac4314939bfb16bfdbde27;
        float Vector1_775ef292c0cf4cda91aecf4845cd67bd;
        CBUFFER_END

        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(_CelShading_ad7871fb5bf1404e9f1fa53eab938a3a_Texture2D9666dd2f2f8f4fddb6c2a5c61ddfa9bd_3072536305);
        SAMPLER(sampler_CelShading_ad7871fb5bf1404e9f1fa53eab938a3a_Texture2D9666dd2f2f8f4fddb6c2a5c61ddfa9bd_3072536305);
        TEXTURE2D(Texture2D_c773e99a7970488a9d6bcbd08807a081);
        SAMPLER(samplerTexture2D_c773e99a7970488a9d6bcbd08807a081);
        TEXTURE2D(Texture2D_c23a14955d09487682029de7801dfc17);
        SAMPLER(samplerTexture2D_c23a14955d09487682029de7801dfc17);

            // Graph Functions
            
        // c9d40f89572cca24e7b23cdf7d4f0592
        #include "Assets/Rendering/Shaders/Functions/URPFonctions.hlsl"

        void Unity_Multiply_float(float A, float B, out float Out)
        {
            Out = A * B;
        }

        void Unity_Add_float3(float3 A, float3 B, out float3 Out)
        {
            Out = A + B;
        }

        struct Bindings_WaveDisplacement_81e878f76fb7ea94995742377f80ac36
        {
        };

        void SG_WaveDisplacement_81e878f76fb7ea94995742377f80ac36(float3 Vector3_774baa01c42a49f6abc99dad6b54a583, float Vector1_b53e914fc7984637b80324d57e274b5f, float Vector1_af1a9e8da22b4e08851f991318dec789, float Vector1_e937c75465ce4f99ba008e1a397c452b, Bindings_WaveDisplacement_81e878f76fb7ea94995742377f80ac36 IN, out float3 OutVector3_2)
        {
            float3 _Property_4f1419a54dd7457eb25ffd09e1294716_Out_0 = Vector3_774baa01c42a49f6abc99dad6b54a583;
            float _Property_61f840140291436e8912958fef5953c3_Out_0 = Vector1_b53e914fc7984637b80324d57e274b5f;
            float _Property_aa3d6115a8f7408182a5ad6eaf4dea9c_Out_0 = Vector1_af1a9e8da22b4e08851f991318dec789;
            float _Property_318bfb7c8fcb48359c265b1829bb9333_Out_0 = Vector1_e937c75465ce4f99ba008e1a397c452b;
            float _CalculateWaveCustomFunction_babfa0b8a0ce4e20b113bd73598d4cd6_New_0;
            CalculateWave_float(_Property_4f1419a54dd7457eb25ffd09e1294716_Out_0, _Property_61f840140291436e8912958fef5953c3_Out_0, _Property_aa3d6115a8f7408182a5ad6eaf4dea9c_Out_0, _Property_318bfb7c8fcb48359c265b1829bb9333_Out_0, _CalculateWaveCustomFunction_babfa0b8a0ce4e20b113bd73598d4cd6_New_0);
            float3 _Vector3_fd59b684dbbf45499ba224d658a9f2ec_Out_0 = float3(0, _CalculateWaveCustomFunction_babfa0b8a0ce4e20b113bd73598d4cd6_New_0, 0);
            float3 _Add_bb0b83c602bf41909e47fdeba507406b_Out_2;
            Unity_Add_float3(_Vector3_fd59b684dbbf45499ba224d658a9f2ec_Out_0, _Property_4f1419a54dd7457eb25ffd09e1294716_Out_0, _Add_bb0b83c602bf41909e47fdeba507406b_Out_2);
            OutVector3_2 = _Add_bb0b83c602bf41909e47fdeba507406b_Out_2;
        }

        void Unity_Sign_float(float In, out float Out)
        {
            Out = sign(In);
        }

        void Unity_Multiply_float(float3 A, float3 B, out float3 Out)
        {
            Out = A * B;
        }

        void Unity_Add_float(float A, float B, out float Out)
        {
            Out = A + B;
        }

        void Unity_Subtract_float(float A, float B, out float Out)
        {
            Out = A - B;
        }

        void Unity_Absolute_float(float In, out float Out)
        {
            Out = abs(In);
        }

        void Unity_Saturate_float(float In, out float Out)
        {
            Out = saturate(In);
        }

        void Unity_Divide_float(float A, float B, out float Out)
        {
            Out = A / B;
        }

        void Unity_Combine_float(float R, float G, float B, float A, out float4 RGBA, out float3 RGB, out float2 RG)
        {
            RGBA = float4(R, G, B, A);
            RGB = float3(R, G, B);
            RG = float2(R, G);
        }

        void Unity_Smoothstep_float(float Edge1, float Edge2, float In, out float Out)
        {
            Out = smoothstep(Edge1, Edge2, In);
        }

        void Unity_Lerp_float3(float3 A, float3 B, float3 T, out float3 Out)
        {
            Out = lerp(A, B, T);
        }

        void Unity_Subtract_float3(float3 A, float3 B, out float3 Out)
        {
            Out = A - B;
        }

        void Unity_Normalize_float3(float3 In, out float3 Out)
        {
            Out = normalize(In);
        }

        void Unity_CrossProduct_float(float3 A, float3 B, out float3 Out)
        {
            Out = cross(A, B);
        }

        struct Bindings_Neighbors_df68ff96a41468e448052764fa9bdba1
        {
            float3 ObjectSpaceNormal;
        };

        void SG_Neighbors_df68ff96a41468e448052764fa9bdba1(float3 Vector3_8954E4F1, float Vector1_B5D26639, Bindings_Neighbors_df68ff96a41468e448052764fa9bdba1 IN, out float3 Neighbor1_1, out float3 Neighbor2_2)
        {
            float _Split_609f2b09351ec288b5ed925103f0f9e0_R_1 = IN.ObjectSpaceNormal[0];
            float _Split_609f2b09351ec288b5ed925103f0f9e0_G_2 = IN.ObjectSpaceNormal[1];
            float _Split_609f2b09351ec288b5ed925103f0f9e0_B_3 = IN.ObjectSpaceNormal[2];
            float _Split_609f2b09351ec288b5ed925103f0f9e0_A_4 = 0;
            float _Sign_e3911d5b4df6208882d5722613bfa7df_Out_1;
            Unity_Sign_float(_Split_609f2b09351ec288b5ed925103f0f9e0_R_1, _Sign_e3911d5b4df6208882d5722613bfa7df_Out_1);
            float3 _Property_8a0179cbbf558586ab0af83d3af77169_Out_0 = Vector3_8954E4F1;
            float3 _Multiply_a5a5ef928369cd84997685e6caac87b0_Out_2;
            Unity_Multiply_float(_Property_8a0179cbbf558586ab0af83d3af77169_Out_0, IN.ObjectSpaceNormal, _Multiply_a5a5ef928369cd84997685e6caac87b0_Out_2);
            float _Split_8977fb298fca9e8182c1edaad752997c_R_1 = _Multiply_a5a5ef928369cd84997685e6caac87b0_Out_2[0];
            float _Split_8977fb298fca9e8182c1edaad752997c_G_2 = _Multiply_a5a5ef928369cd84997685e6caac87b0_Out_2[1];
            float _Split_8977fb298fca9e8182c1edaad752997c_B_3 = _Multiply_a5a5ef928369cd84997685e6caac87b0_Out_2[2];
            float _Split_8977fb298fca9e8182c1edaad752997c_A_4 = 0;
            float _Add_eeda377c8b7c038596bc86d83f7f321e_Out_2;
            Unity_Add_float(_Split_8977fb298fca9e8182c1edaad752997c_R_1, _Split_8977fb298fca9e8182c1edaad752997c_G_2, _Add_eeda377c8b7c038596bc86d83f7f321e_Out_2);
            float _Add_61a089896fe3468b82b211a762b6ec21_Out_2;
            Unity_Add_float(_Add_eeda377c8b7c038596bc86d83f7f321e_Out_2, _Split_8977fb298fca9e8182c1edaad752997c_B_3, _Add_61a089896fe3468b82b211a762b6ec21_Out_2);
            float _Split_d79bc2f0568d4b8586a29273c4b01ac3_R_1 = IN.ObjectSpaceNormal[0];
            float _Split_d79bc2f0568d4b8586a29273c4b01ac3_G_2 = IN.ObjectSpaceNormal[1];
            float _Split_d79bc2f0568d4b8586a29273c4b01ac3_B_3 = IN.ObjectSpaceNormal[2];
            float _Split_d79bc2f0568d4b8586a29273c4b01ac3_A_4 = 0;
            float _Float_9d499dd7c2614a8589e8a1cb04daeb5d_Out_0 = 0.05;
            float _Split_802c7c2944faa986ba720f07d8db74ab_R_1 = _Property_8a0179cbbf558586ab0af83d3af77169_Out_0[0];
            float _Split_802c7c2944faa986ba720f07d8db74ab_G_2 = _Property_8a0179cbbf558586ab0af83d3af77169_Out_0[1];
            float _Split_802c7c2944faa986ba720f07d8db74ab_B_3 = _Property_8a0179cbbf558586ab0af83d3af77169_Out_0[2];
            float _Split_802c7c2944faa986ba720f07d8db74ab_A_4 = 0;
            float _Add_4555b76b861a778d97276d765af07483_Out_2;
            Unity_Add_float(_Float_9d499dd7c2614a8589e8a1cb04daeb5d_Out_0, _Split_802c7c2944faa986ba720f07d8db74ab_G_2, _Add_4555b76b861a778d97276d765af07483_Out_2);
            float _Multiply_1e591f42bea8b58c8797ab2c90faa345_Out_2;
            Unity_Multiply_float(_Split_d79bc2f0568d4b8586a29273c4b01ac3_G_2, _Add_4555b76b861a778d97276d765af07483_Out_2, _Multiply_1e591f42bea8b58c8797ab2c90faa345_Out_2);
            float _Subtract_68ba98ef07340b83b46992e63c68d7ca_Out_2;
            Unity_Subtract_float(_Add_61a089896fe3468b82b211a762b6ec21_Out_2, _Multiply_1e591f42bea8b58c8797ab2c90faa345_Out_2, _Subtract_68ba98ef07340b83b46992e63c68d7ca_Out_2);
            float _Multiply_d0572ed2941ee0879549c1da1c572b15_Out_2;
            Unity_Multiply_float(_Split_d79bc2f0568d4b8586a29273c4b01ac3_B_3, _Split_802c7c2944faa986ba720f07d8db74ab_B_3, _Multiply_d0572ed2941ee0879549c1da1c572b15_Out_2);
            float _Subtract_3d9ad256eba2ee8e839158fc0f45ee81_Out_2;
            Unity_Subtract_float(_Subtract_68ba98ef07340b83b46992e63c68d7ca_Out_2, _Multiply_d0572ed2941ee0879549c1da1c572b15_Out_2, _Subtract_3d9ad256eba2ee8e839158fc0f45ee81_Out_2);
            float _Float_654fffd8f366ff808a39bb5a6a1f1fdb_Out_0 = 0.01;
            float _Absolute_9fde5c96b588aa8fbed6af91404da630_Out_1;
            Unity_Absolute_float(_Split_609f2b09351ec288b5ed925103f0f9e0_R_1, _Absolute_9fde5c96b588aa8fbed6af91404da630_Out_1);
            float _Subtract_7173428a9289a58abbac98045c08ceb9_Out_2;
            Unity_Subtract_float(_Absolute_9fde5c96b588aa8fbed6af91404da630_Out_1, _Float_654fffd8f366ff808a39bb5a6a1f1fdb_Out_0, _Subtract_7173428a9289a58abbac98045c08ceb9_Out_2);
            float _Saturate_e8fcca777a01738ab601adda864265d1_Out_1;
            Unity_Saturate_float(_Subtract_7173428a9289a58abbac98045c08ceb9_Out_2, _Saturate_e8fcca777a01738ab601adda864265d1_Out_1);
            float _Add_71e184a7749fc9848483bc81af14e30e_Out_2;
            Unity_Add_float(_Float_654fffd8f366ff808a39bb5a6a1f1fdb_Out_0, _Saturate_e8fcca777a01738ab601adda864265d1_Out_1, _Add_71e184a7749fc9848483bc81af14e30e_Out_2);
            float _Divide_7cc37db0bc8630899cffaa8d565d6d55_Out_2;
            Unity_Divide_float(_Subtract_3d9ad256eba2ee8e839158fc0f45ee81_Out_2, _Add_71e184a7749fc9848483bc81af14e30e_Out_2, _Divide_7cc37db0bc8630899cffaa8d565d6d55_Out_2);
            float _Multiply_9c6b2289e8d2db84a1c0d5ca2a655b93_Out_2;
            Unity_Multiply_float(_Sign_e3911d5b4df6208882d5722613bfa7df_Out_1, _Divide_7cc37db0bc8630899cffaa8d565d6d55_Out_2, _Multiply_9c6b2289e8d2db84a1c0d5ca2a655b93_Out_2);
            float4 _Combine_8fcb17287675378fb024a84210c6677a_RGBA_4;
            float3 _Combine_8fcb17287675378fb024a84210c6677a_RGB_5;
            float2 _Combine_8fcb17287675378fb024a84210c6677a_RG_6;
            Unity_Combine_float(_Multiply_9c6b2289e8d2db84a1c0d5ca2a655b93_Out_2, _Add_4555b76b861a778d97276d765af07483_Out_2, _Split_802c7c2944faa986ba720f07d8db74ab_B_3, 0, _Combine_8fcb17287675378fb024a84210c6677a_RGBA_4, _Combine_8fcb17287675378fb024a84210c6677a_RGB_5, _Combine_8fcb17287675378fb024a84210c6677a_RG_6);
            float _Float_9b0eee4975b51e828a4269a5edcee5b1_Out_0 = 0.05;
            float _Split_f205281c3443d68aa9b0c1a2feace9df_R_1 = _Property_8a0179cbbf558586ab0af83d3af77169_Out_0[0];
            float _Split_f205281c3443d68aa9b0c1a2feace9df_G_2 = _Property_8a0179cbbf558586ab0af83d3af77169_Out_0[1];
            float _Split_f205281c3443d68aa9b0c1a2feace9df_B_3 = _Property_8a0179cbbf558586ab0af83d3af77169_Out_0[2];
            float _Split_f205281c3443d68aa9b0c1a2feace9df_A_4 = 0;
            float _Add_db37e0cfea249d8d9ede45e675501301_Out_2;
            Unity_Add_float(_Float_9b0eee4975b51e828a4269a5edcee5b1_Out_0, _Split_f205281c3443d68aa9b0c1a2feace9df_R_1, _Add_db37e0cfea249d8d9ede45e675501301_Out_2);
            float4 _Combine_9530e51a49a77f849e6eac5cb0ce15ad_RGBA_4;
            float3 _Combine_9530e51a49a77f849e6eac5cb0ce15ad_RGB_5;
            float2 _Combine_9530e51a49a77f849e6eac5cb0ce15ad_RG_6;
            Unity_Combine_float(_Add_db37e0cfea249d8d9ede45e675501301_Out_2, _Split_f205281c3443d68aa9b0c1a2feace9df_G_2, _Split_f205281c3443d68aa9b0c1a2feace9df_B_3, 0, _Combine_9530e51a49a77f849e6eac5cb0ce15ad_RGBA_4, _Combine_9530e51a49a77f849e6eac5cb0ce15ad_RGB_5, _Combine_9530e51a49a77f849e6eac5cb0ce15ad_RG_6);
            float _Multiply_625b1e551cdfd0829aa19290646e6739_Out_2;
            Unity_Multiply_float(-1, _Subtract_7173428a9289a58abbac98045c08ceb9_Out_2, _Multiply_625b1e551cdfd0829aa19290646e6739_Out_2);
            float _Smoothstep_b60ff4818667938f9be3fa9ecf03f163_Out_3;
            Unity_Smoothstep_float(0, 0.01, _Multiply_625b1e551cdfd0829aa19290646e6739_Out_2, _Smoothstep_b60ff4818667938f9be3fa9ecf03f163_Out_3);
            float3 _Lerp_7d115f30f5f0fc8cb46b9ba33440e47a_Out_3;
            Unity_Lerp_float3(_Combine_8fcb17287675378fb024a84210c6677a_RGB_5, _Combine_9530e51a49a77f849e6eac5cb0ce15ad_RGB_5, (_Smoothstep_b60ff4818667938f9be3fa9ecf03f163_Out_3.xxx), _Lerp_7d115f30f5f0fc8cb46b9ba33440e47a_Out_3);
            float3 _Subtract_24a46f67e2876e80b3b01e0b77a50456_Out_2;
            Unity_Subtract_float3(_Lerp_7d115f30f5f0fc8cb46b9ba33440e47a_Out_3, _Property_8a0179cbbf558586ab0af83d3af77169_Out_0, _Subtract_24a46f67e2876e80b3b01e0b77a50456_Out_2);
            float3 _Normalize_f544245d7bd37f8fbaee4a87c2403675_Out_1;
            Unity_Normalize_float3(_Subtract_24a46f67e2876e80b3b01e0b77a50456_Out_2, _Normalize_f544245d7bd37f8fbaee4a87c2403675_Out_1);
            float3 _Normalize_9a359141b951ea84b3220ad5c3123b7c_Out_1;
            Unity_Normalize_float3(IN.ObjectSpaceNormal, _Normalize_9a359141b951ea84b3220ad5c3123b7c_Out_1);
            float3 _CrossProduct_1dbde50ff8c546829c8b59ec1630c252_Out_2;
            Unity_CrossProduct_float(_Normalize_f544245d7bd37f8fbaee4a87c2403675_Out_1, _Normalize_9a359141b951ea84b3220ad5c3123b7c_Out_1, _CrossProduct_1dbde50ff8c546829c8b59ec1630c252_Out_2);
            float3 _Normalize_3eb509f6a593a18d856dabb44191008a_Out_1;
            Unity_Normalize_float3(_CrossProduct_1dbde50ff8c546829c8b59ec1630c252_Out_2, _Normalize_3eb509f6a593a18d856dabb44191008a_Out_1);
            float _Property_4448eb2908e4228e840b80f046abf12e_Out_0 = Vector1_B5D26639;
            float3 _Multiply_37bf5602003a2885bb744b13f93296fe_Out_2;
            Unity_Multiply_float(_Normalize_3eb509f6a593a18d856dabb44191008a_Out_1, (_Property_4448eb2908e4228e840b80f046abf12e_Out_0.xxx), _Multiply_37bf5602003a2885bb744b13f93296fe_Out_2);
            float3 _Add_bd2759426a510983979415cc0e46c2d4_Out_2;
            Unity_Add_float3(_Multiply_37bf5602003a2885bb744b13f93296fe_Out_2, _Property_8a0179cbbf558586ab0af83d3af77169_Out_0, _Add_bd2759426a510983979415cc0e46c2d4_Out_2);
            float3 _Multiply_273e0ecf939b858582031df2660ec861_Out_2;
            Unity_Multiply_float(_Normalize_f544245d7bd37f8fbaee4a87c2403675_Out_1, (_Property_4448eb2908e4228e840b80f046abf12e_Out_0.xxx), _Multiply_273e0ecf939b858582031df2660ec861_Out_2);
            float3 _Add_2259c39580c12b8297f6b81a426bd3c0_Out_2;
            Unity_Add_float3(_Property_8a0179cbbf558586ab0af83d3af77169_Out_0, _Multiply_273e0ecf939b858582031df2660ec861_Out_2, _Add_2259c39580c12b8297f6b81a426bd3c0_Out_2);
            Neighbor1_1 = _Add_bd2759426a510983979415cc0e46c2d4_Out_2;
            Neighbor2_2 = _Add_2259c39580c12b8297f6b81a426bd3c0_Out_2;
        }

        struct Bindings_NewNormal_c8d6b0224f233004190264cd9e5ca192
        {
        };

        void SG_NewNormal_c8d6b0224f233004190264cd9e5ca192(float3 Vector3_988291EB, float3 Vector3_EB4E2DE7, float3 Vector3_1973C67A, Bindings_NewNormal_c8d6b0224f233004190264cd9e5ca192 IN, out float3 OutVector3_1)
        {
            float3 _Property_fd61227e10c3348bb61b5f1d39e68328_Out_0 = Vector3_EB4E2DE7;
            float3 _Property_b8cacddd74e568809402e877976dffa8_Out_0 = Vector3_988291EB;
            float3 _Subtract_1f3c648b5559d28f9acfbe1af362b37b_Out_2;
            Unity_Subtract_float3(_Property_fd61227e10c3348bb61b5f1d39e68328_Out_0, _Property_b8cacddd74e568809402e877976dffa8_Out_0, _Subtract_1f3c648b5559d28f9acfbe1af362b37b_Out_2);
            float3 _Normalize_888167f431fbc788b4ad8e2092e7f28b_Out_1;
            Unity_Normalize_float3(_Subtract_1f3c648b5559d28f9acfbe1af362b37b_Out_2, _Normalize_888167f431fbc788b4ad8e2092e7f28b_Out_1);
            float3 _Property_6008e654ce04488cbe1d5caac8afe08a_Out_0 = Vector3_1973C67A;
            float3 _Subtract_208696e2abb5f18d84a5878fd9a81553_Out_2;
            Unity_Subtract_float3(_Property_6008e654ce04488cbe1d5caac8afe08a_Out_0, _Property_b8cacddd74e568809402e877976dffa8_Out_0, _Subtract_208696e2abb5f18d84a5878fd9a81553_Out_2);
            float3 _Normalize_7250d413c7450d8aaa805c923843f2dd_Out_1;
            Unity_Normalize_float3(_Subtract_208696e2abb5f18d84a5878fd9a81553_Out_2, _Normalize_7250d413c7450d8aaa805c923843f2dd_Out_1);
            float3 _CrossProduct_5566a5ab348aa8809609d834bbe4451f_Out_2;
            Unity_CrossProduct_float(_Normalize_888167f431fbc788b4ad8e2092e7f28b_Out_1, _Normalize_7250d413c7450d8aaa805c923843f2dd_Out_1, _CrossProduct_5566a5ab348aa8809609d834bbe4451f_Out_2);
            float3 _Normalize_a640075232b8c78498848f10ca1e7ba8_Out_1;
            Unity_Normalize_float3(_CrossProduct_5566a5ab348aa8809609d834bbe4451f_Out_2, _Normalize_a640075232b8c78498848f10ca1e7ba8_Out_1);
            OutVector3_1 = _Normalize_a640075232b8c78498848f10ca1e7ba8_Out_1;
        }

        void Unity_SceneDepth_Linear01_float(float4 UV, out float Out)
        {
            Out = Linear01Depth(SHADERGRAPH_SAMPLE_SCENE_DEPTH(UV.xy), _ZBufferParams);
        }

        void Unity_Clamp_float(float In, float Min, float Max, out float Out)
        {
            Out = clamp(In, Min, Max);
        }

        void Unity_Power_float(float A, float B, out float Out)
        {
            Out = pow(A, B);
        }

        void Unity_FresnelEffect_float(float3 Normal, float3 ViewDir, float Power, out float Out)
        {
            Out = pow((1.0 - saturate(dot(normalize(Normal), normalize(ViewDir)))), Power);
        }

        void Unity_Maximum_float(float A, float B, out float Out)
        {
            Out = max(A, B);
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
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            UnityTexture2D _Property_52e3e5c2509c4df984c47cc092c466d8_Out_0 = UnityBuildTexture2DStructNoScale(Texture2D_c773e99a7970488a9d6bcbd08807a081);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Property_736f2734d3e643c398a2586801e4cc17_Out_0 = Vector1_f133b62c64f14525895d4b7d5afdd79f;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Property_7851f92dfd4d4302a7beb8a17dd4bd8b_Out_0 = Vector1_45a27133fb8b4113a0746c0d1954c57b;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float2 _CalculateNoiseCustomFunction_a0c2603a229c4d299ac32a59a8664765_Out_0;
            CalculateNoise_float(IN.AbsoluteWorldSpacePosition, _Property_736f2734d3e643c398a2586801e4cc17_Out_0, _Property_7851f92dfd4d4302a7beb8a17dd4bd8b_Out_0, _CalculateNoiseCustomFunction_a0c2603a229c4d299ac32a59a8664765_Out_0);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            #if defined(SHADER_API_GLES) && (SHADER_TARGET < 30)
              float4 _SampleTexture2DLOD_fe1fede5211b477cbca93fc4c2f76d12_RGBA_0 = float4(0.0f, 0.0f, 0.0f, 1.0f);
            #else
              float4 _SampleTexture2DLOD_fe1fede5211b477cbca93fc4c2f76d12_RGBA_0 = SAMPLE_TEXTURE2D_LOD(_Property_52e3e5c2509c4df984c47cc092c466d8_Out_0.tex, _Property_52e3e5c2509c4df984c47cc092c466d8_Out_0.samplerstate, _CalculateNoiseCustomFunction_a0c2603a229c4d299ac32a59a8664765_Out_0, 0);
            #endif
            float _SampleTexture2DLOD_fe1fede5211b477cbca93fc4c2f76d12_R_5 = _SampleTexture2DLOD_fe1fede5211b477cbca93fc4c2f76d12_RGBA_0.r;
            float _SampleTexture2DLOD_fe1fede5211b477cbca93fc4c2f76d12_G_6 = _SampleTexture2DLOD_fe1fede5211b477cbca93fc4c2f76d12_RGBA_0.g;
            float _SampleTexture2DLOD_fe1fede5211b477cbca93fc4c2f76d12_B_7 = _SampleTexture2DLOD_fe1fede5211b477cbca93fc4c2f76d12_RGBA_0.b;
            float _SampleTexture2DLOD_fe1fede5211b477cbca93fc4c2f76d12_A_8 = _SampleTexture2DLOD_fe1fede5211b477cbca93fc4c2f76d12_RGBA_0.a;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Property_fab5f23c0a9e449bb605bc77e42f5c55_Out_0 = Vector1_57cb41fdb69545e8b53a796761acb08d;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Multiply_be99c4e752404fa4b5a42166d27a7cc0_Out_2;
            Unity_Multiply_float(_SampleTexture2DLOD_fe1fede5211b477cbca93fc4c2f76d12_R_5, _Property_fab5f23c0a9e449bb605bc77e42f5c55_Out_0, _Multiply_be99c4e752404fa4b5a42166d27a7cc0_Out_2);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float3 _Vector3_9e43fda8531d474c890ea7595c453773_Out_0 = float3(0, _Multiply_be99c4e752404fa4b5a42166d27a7cc0_Out_2, 0);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Property_0ea561f00aaf4fbb8fdfeab5a2a0dd08_Out_0 = Vector1_4da6e98cbfa5408fb6352b485aa76d05;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Property_e4937aaae9bc4c098cebea935a7ee1ea_Out_0 = Vector1_b266d7e71de2489ea084cb27060cacca;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Property_2d3470f269ac4477a8dc467d895fe472_Out_0 = Vector1_d09cc377b0524610970658ffdacd497f;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            Bindings_WaveDisplacement_81e878f76fb7ea94995742377f80ac36 _WaveDisplacement_5f54fff0c6334b26bedfff0b63df12b3;
            float3 _WaveDisplacement_5f54fff0c6334b26bedfff0b63df12b3_OutVector3_2;
            SG_WaveDisplacement_81e878f76fb7ea94995742377f80ac36(IN.AbsoluteWorldSpacePosition, _Property_0ea561f00aaf4fbb8fdfeab5a2a0dd08_Out_0, _Property_e4937aaae9bc4c098cebea935a7ee1ea_Out_0, _Property_2d3470f269ac4477a8dc467d895fe472_Out_0, _WaveDisplacement_5f54fff0c6334b26bedfff0b63df12b3, _WaveDisplacement_5f54fff0c6334b26bedfff0b63df12b3_OutVector3_2);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float3 _Add_942b46ad98eb4925b4bfa00a0a5781c7_Out_2;
            Unity_Add_float3(_Vector3_9e43fda8531d474c890ea7595c453773_Out_0, _WaveDisplacement_5f54fff0c6334b26bedfff0b63df12b3_OutVector3_2, _Add_942b46ad98eb4925b4bfa00a0a5781c7_Out_2);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float3 _Transform_3fce2cebd3054a72a946f448c89c2e3d_Out_1 = TransformWorldToObject(GetCameraRelativePositionWS(_Add_942b46ad98eb4925b4bfa00a0a5781c7_Out_2.xyz));
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            Bindings_Neighbors_df68ff96a41468e448052764fa9bdba1 _Neighbors_e648c110f4754cefb6491b8c82fe074f;
            _Neighbors_e648c110f4754cefb6491b8c82fe074f.ObjectSpaceNormal = IN.ObjectSpaceNormal;
            float3 _Neighbors_e648c110f4754cefb6491b8c82fe074f_Neighbor1_1;
            float3 _Neighbors_e648c110f4754cefb6491b8c82fe074f_Neighbor2_2;
            SG_Neighbors_df68ff96a41468e448052764fa9bdba1(IN.AbsoluteWorldSpacePosition, 0.01, _Neighbors_e648c110f4754cefb6491b8c82fe074f, _Neighbors_e648c110f4754cefb6491b8c82fe074f_Neighbor1_1, _Neighbors_e648c110f4754cefb6491b8c82fe074f_Neighbor2_2);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            Bindings_WaveDisplacement_81e878f76fb7ea94995742377f80ac36 _WaveDisplacement_2fa36261c2534cbe8639a41fdfe4910a;
            float3 _WaveDisplacement_2fa36261c2534cbe8639a41fdfe4910a_OutVector3_2;
            SG_WaveDisplacement_81e878f76fb7ea94995742377f80ac36(_Neighbors_e648c110f4754cefb6491b8c82fe074f_Neighbor1_1, _Property_0ea561f00aaf4fbb8fdfeab5a2a0dd08_Out_0, _Property_e4937aaae9bc4c098cebea935a7ee1ea_Out_0, _Property_2d3470f269ac4477a8dc467d895fe472_Out_0, _WaveDisplacement_2fa36261c2534cbe8639a41fdfe4910a, _WaveDisplacement_2fa36261c2534cbe8639a41fdfe4910a_OutVector3_2);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            Bindings_WaveDisplacement_81e878f76fb7ea94995742377f80ac36 _WaveDisplacement_c9fd16638edd4ccfaf9e3eb2efa55bfa;
            float3 _WaveDisplacement_c9fd16638edd4ccfaf9e3eb2efa55bfa_OutVector3_2;
            SG_WaveDisplacement_81e878f76fb7ea94995742377f80ac36(_Neighbors_e648c110f4754cefb6491b8c82fe074f_Neighbor2_2, _Property_0ea561f00aaf4fbb8fdfeab5a2a0dd08_Out_0, _Property_e4937aaae9bc4c098cebea935a7ee1ea_Out_0, _Property_2d3470f269ac4477a8dc467d895fe472_Out_0, _WaveDisplacement_c9fd16638edd4ccfaf9e3eb2efa55bfa, _WaveDisplacement_c9fd16638edd4ccfaf9e3eb2efa55bfa_OutVector3_2);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            Bindings_NewNormal_c8d6b0224f233004190264cd9e5ca192 _NewNormal_e9af9405a4a54034863b00f2fbd4c2b6;
            float3 _NewNormal_e9af9405a4a54034863b00f2fbd4c2b6_OutVector3_1;
            SG_NewNormal_c8d6b0224f233004190264cd9e5ca192(_WaveDisplacement_5f54fff0c6334b26bedfff0b63df12b3_OutVector3_2, _WaveDisplacement_2fa36261c2534cbe8639a41fdfe4910a_OutVector3_2, _WaveDisplacement_c9fd16638edd4ccfaf9e3eb2efa55bfa_OutVector3_2, _NewNormal_e9af9405a4a54034863b00f2fbd4c2b6, _NewNormal_e9af9405a4a54034863b00f2fbd4c2b6_OutVector3_1);
            #endif
            description.Position = _Transform_3fce2cebd3054a72a946f448c89c2e3d_Out_1;
            description.Normal = _NewNormal_e9af9405a4a54034863b00f2fbd4c2b6_OutVector3_1;
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
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _SceneDepth_7981f193eac94d87b6f2b2920d4cb528_Out_1;
            Unity_SceneDepth_Linear01_float(float4(IN.ScreenPosition.xy / IN.ScreenPosition.w, 0, 0), _SceneDepth_7981f193eac94d87b6f2b2920d4cb528_Out_1);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Multiply_abe194398c584d8a8934b67cf64c0da1_Out_2;
            Unity_Multiply_float(_SceneDepth_7981f193eac94d87b6f2b2920d4cb528_Out_1, _ProjectionParams.z, _Multiply_abe194398c584d8a8934b67cf64c0da1_Out_2);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float4 _ScreenPosition_68dc8d80a07f4706a6a9a2f0230737ed_Out_0 = IN.ScreenPosition;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Split_7f7e1aadecd54073aff51bd05fa759bc_R_1 = _ScreenPosition_68dc8d80a07f4706a6a9a2f0230737ed_Out_0[0];
            float _Split_7f7e1aadecd54073aff51bd05fa759bc_G_2 = _ScreenPosition_68dc8d80a07f4706a6a9a2f0230737ed_Out_0[1];
            float _Split_7f7e1aadecd54073aff51bd05fa759bc_B_3 = _ScreenPosition_68dc8d80a07f4706a6a9a2f0230737ed_Out_0[2];
            float _Split_7f7e1aadecd54073aff51bd05fa759bc_A_4 = _ScreenPosition_68dc8d80a07f4706a6a9a2f0230737ed_Out_0[3];
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Multiply_7ba2c47eef0f4b89b3b4eb4a8ed31f83_Out_2;
            Unity_Multiply_float(_Split_7f7e1aadecd54073aff51bd05fa759bc_A_4, 1, _Multiply_7ba2c47eef0f4b89b3b4eb4a8ed31f83_Out_2);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Subtract_dc119b36f453437e913cdd2a2623efd2_Out_2;
            Unity_Subtract_float(_Multiply_abe194398c584d8a8934b67cf64c0da1_Out_2, _Multiply_7ba2c47eef0f4b89b3b4eb4a8ed31f83_Out_2, _Subtract_dc119b36f453437e913cdd2a2623efd2_Out_2);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Property_ab48707fe6db419eb51ece316bb6313b_Out_0 = Vector1_cc6e37850778480781084b62719d58c3;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Multiply_3a48e77d57694a868985ec140162c2ef_Out_2;
            Unity_Multiply_float(_Subtract_dc119b36f453437e913cdd2a2623efd2_Out_2, _Property_ab48707fe6db419eb51ece316bb6313b_Out_0, _Multiply_3a48e77d57694a868985ec140162c2ef_Out_2);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Clamp_bb886dbf030149bd8ffa4e979d6e9c46_Out_3;
            Unity_Clamp_float(_Multiply_3a48e77d57694a868985ec140162c2ef_Out_2, 0, 1, _Clamp_bb886dbf030149bd8ffa4e979d6e9c46_Out_3);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Power_795157e2d171465e908da5d45e938d28_Out_2;
            Unity_Power_float(_Clamp_bb886dbf030149bd8ffa4e979d6e9c46_Out_3, 0.7, _Power_795157e2d171465e908da5d45e938d28_Out_2);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Property_41daac0ae2c74098ac9111219e1e3b3b_Out_0 = Vector1_5be3734798f64a63b2827794b79391c3;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _FresnelEffect_98bea57f43804c61a33cabcb5a3e5400_Out_3;
            Unity_FresnelEffect_float(IN.WorldSpaceNormal, IN.WorldSpaceViewDirection, _Property_41daac0ae2c74098ac9111219e1e3b3b_Out_0, _FresnelEffect_98bea57f43804c61a33cabcb5a3e5400_Out_3);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Maximum_85ae2b8b22d5495fb3e04a01fa179389_Out_2;
            Unity_Maximum_float(_FresnelEffect_98bea57f43804c61a33cabcb5a3e5400_Out_3, 0.7, _Maximum_85ae2b8b22d5495fb3e04a01fa179389_Out_2);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Multiply_465f2016ad9d499981e3a39ae56b8aef_Out_2;
            Unity_Multiply_float(_Power_795157e2d171465e908da5d45e938d28_Out_2, _Maximum_85ae2b8b22d5495fb3e04a01fa179389_Out_2, _Multiply_465f2016ad9d499981e3a39ae56b8aef_Out_2);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Saturate_00659ca349c74eea975fbb11da46e6bf_Out_1;
            Unity_Saturate_float(_Multiply_465f2016ad9d499981e3a39ae56b8aef_Out_2, _Saturate_00659ca349c74eea975fbb11da46e6bf_Out_1);
            #endif
            surface.Alpha = _Saturate_00659ca349c74eea975fbb11da46e6bf_Out_1;
            return surface;
        }

            // --------------------------------------------------
            // Build Graph Inputs

            VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
        {
            VertexDescriptionInputs output;
            ZERO_INITIALIZE(VertexDescriptionInputs, output);

        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        output.ObjectSpaceNormal =           input.normalOS;
        #endif

        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        output.WorldSpaceNormal =            TransformObjectToWorldNormal(input.normalOS);
        #endif

        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        output.ObjectSpaceTangent =          input.tangentOS.xyz;
        #endif

        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        output.WorldSpaceTangent =           TransformObjectToWorldDir(input.tangentOS.xyz);
        #endif

        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        output.ObjectSpaceBiTangent =        normalize(cross(input.normalOS, input.tangentOS) * (input.tangentOS.w > 0.0f ? 1.0f : -1.0f) * GetOddNegativeScale());
        #endif

        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        output.WorldSpaceBiTangent =         TransformObjectToWorldDir(output.ObjectSpaceBiTangent);
        #endif

        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        output.ObjectSpacePosition =         input.positionOS;
        #endif

        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        output.AbsoluteWorldSpacePosition =  GetAbsolutePositionWS(TransformObjectToWorld(input.positionOS));
        #endif


            return output;
        }
            SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);

        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        // must use interpolated tangent, bitangent and normal before they are normalized in the pixel shader.
        #endif

        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        float3 unnormalizedNormalWS = input.normalWS;
        #endif

        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        const float renormFactor = 1.0 / length(unnormalizedNormalWS);
        #endif



        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        output.WorldSpaceNormal =            renormFactor*input.normalWS.xyz;		// we want a unit length Normal Vector node in shader graph
        #endif



        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        output.WorldSpaceViewDirection =     input.viewDirectionWS; //TODO: by default normalized in HD, but not in universal
        #endif

        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        output.WorldSpacePosition =          input.positionWS;
        #endif

        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        output.ScreenPosition =              ComputeScreenPos(TransformWorldToHClip(input.positionWS), _ProjectionParams.x);
        #endif

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
            #pragma multi_compile_local _ CEL_SPECULAR_ON
        #pragma multi_compile _ _SHADOWS_SOFT
        #pragma multi_compile _ _ADDITIONAL_LIGHTS
        #pragma shader_feature _ CEL_ACCENT_ON

        #if defined(CEL_SPECULAR_ON)
            #define KEYWORD_PERMUTATION_0
        #else
            #define KEYWORD_PERMUTATION_1
        #endif


            // Defines
        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        #define _SURFACE_TYPE_TRANSPARENT 1
        #endif

        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        #define ATTRIBUTES_NEED_NORMAL
        #endif

        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        #define ATTRIBUTES_NEED_TANGENT
        #endif

        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        #define ATTRIBUTES_NEED_TEXCOORD0
        #endif

        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        #define VARYINGS_NEED_POSITION_WS
        #endif

        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        #define VARYINGS_NEED_NORMAL_WS
        #endif

        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        #define VARYINGS_NEED_TANGENT_WS
        #endif

        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        #define VARYINGS_NEED_TEXCOORD0
        #endif

        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        #define VARYINGS_NEED_VIEWDIRECTION_WS
        #endif

            #define FEATURES_GRAPH_VERTEX
            /* WARNING: $splice Could not find named fragment 'PassInstancing' */
            #define SHADERPASS SHADERPASS_UNLIT
        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        #define REQUIRE_DEPTH_TEXTURE
        #endif
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
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float3 positionOS : POSITION;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float3 normalOS : NORMAL;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float4 tangentOS : TANGENT;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float4 uv0 : TEXCOORD0;
            #endif
            #if UNITY_ANY_INSTANCING_ENABLED
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            uint instanceID : INSTANCEID_SEMANTIC;
            #endif
            #endif
        };
        struct Varyings
        {
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float4 positionCS : SV_POSITION;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float3 positionWS;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float3 normalWS;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float4 tangentWS;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float4 texCoord0;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float3 viewDirectionWS;
            #endif
            #if UNITY_ANY_INSTANCING_ENABLED
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
            #endif
        };
        struct SurfaceDescriptionInputs
        {
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float3 WorldSpaceNormal;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float3 WorldSpaceTangent;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float3 WorldSpaceBiTangent;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float3 WorldSpaceViewDirection;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float3 ObjectSpacePosition;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float3 WorldSpacePosition;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float3 AbsoluteWorldSpacePosition;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float4 ScreenPosition;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float4 uv0;
            #endif
        };
        struct VertexDescriptionInputs
        {
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float3 ObjectSpaceNormal;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float3 WorldSpaceNormal;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float3 ObjectSpaceTangent;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float3 WorldSpaceTangent;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float3 ObjectSpaceBiTangent;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float3 WorldSpaceBiTangent;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float3 ObjectSpacePosition;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float3 AbsoluteWorldSpacePosition;
            #endif
        };
        struct PackedVaryings
        {
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float4 positionCS : SV_POSITION;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float3 interp0 : TEXCOORD0;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float3 interp1 : TEXCOORD1;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float4 interp2 : TEXCOORD2;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float4 interp3 : TEXCOORD3;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float3 interp4 : TEXCOORD4;
            #endif
            #if UNITY_ANY_INSTANCING_ENABLED
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
            #endif
        };

            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
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
        #endif

            // --------------------------------------------------
            // Graph

            // Graph Properties
            CBUFFER_START(UnityPerMaterial)
        float4 _CelShading_ad7871fb5bf1404e9f1fa53eab938a3a_Texture2D9666dd2f2f8f4fddb6c2a5c61ddfa9bd_3072536305_TexelSize;
        float Vector1_0e212c6562134ef7add6f6766d6419b4;
        float4 Color_213be8c364f242cdbabe6ef25c0efbd9;
        float4 Color_8b23519dc989478fa19369d8efda52f6;
        float Vector1_40661633426347afb1a705a362429b36;
        float4 Texture2D_c773e99a7970488a9d6bcbd08807a081_TexelSize;
        float Vector1_45a27133fb8b4113a0746c0d1954c57b;
        float Vector1_f133b62c64f14525895d4b7d5afdd79f;
        float Vector1_57cb41fdb69545e8b53a796761acb08d;
        float Vector1_76a8f3bdc13d493dbc42eca7e6de6c4e;
        float4 Texture2D_c23a14955d09487682029de7801dfc17_TexelSize;
        float Vector1_28676355978446f285fbfe6f1a574316;
        float Vector1_ba885a3188cc490b9d04d880f79cd1b0;
        float Vector1_bf79c34252fe4364976a552df388436c;
        float Vector1_f586ab191e3e497491345288306a18c0;
        float Vector1_4da6e98cbfa5408fb6352b485aa76d05;
        float Vector1_b266d7e71de2489ea084cb27060cacca;
        float Vector1_d09cc377b0524610970658ffdacd497f;
        float Vector1_1f93deba5516424f927cee1eb2c93ba3;
        float Vector1_cc6e37850778480781084b62719d58c3;
        float Vector1_5be3734798f64a63b2827794b79391c3;
        float Vector1_3578eca6d8ac4314939bfb16bfdbde27;
        float Vector1_775ef292c0cf4cda91aecf4845cd67bd;
        CBUFFER_END

        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(_CelShading_ad7871fb5bf1404e9f1fa53eab938a3a_Texture2D9666dd2f2f8f4fddb6c2a5c61ddfa9bd_3072536305);
        SAMPLER(sampler_CelShading_ad7871fb5bf1404e9f1fa53eab938a3a_Texture2D9666dd2f2f8f4fddb6c2a5c61ddfa9bd_3072536305);
        TEXTURE2D(Texture2D_c773e99a7970488a9d6bcbd08807a081);
        SAMPLER(samplerTexture2D_c773e99a7970488a9d6bcbd08807a081);
        TEXTURE2D(Texture2D_c23a14955d09487682029de7801dfc17);
        SAMPLER(samplerTexture2D_c23a14955d09487682029de7801dfc17);

            // Graph Functions
            
        // c9d40f89572cca24e7b23cdf7d4f0592
        #include "Assets/Rendering/Shaders/Functions/URPFonctions.hlsl"

        void Unity_Multiply_float(float A, float B, out float Out)
        {
            Out = A * B;
        }

        void Unity_Add_float3(float3 A, float3 B, out float3 Out)
        {
            Out = A + B;
        }

        struct Bindings_WaveDisplacement_81e878f76fb7ea94995742377f80ac36
        {
        };

        void SG_WaveDisplacement_81e878f76fb7ea94995742377f80ac36(float3 Vector3_774baa01c42a49f6abc99dad6b54a583, float Vector1_b53e914fc7984637b80324d57e274b5f, float Vector1_af1a9e8da22b4e08851f991318dec789, float Vector1_e937c75465ce4f99ba008e1a397c452b, Bindings_WaveDisplacement_81e878f76fb7ea94995742377f80ac36 IN, out float3 OutVector3_2)
        {
            float3 _Property_4f1419a54dd7457eb25ffd09e1294716_Out_0 = Vector3_774baa01c42a49f6abc99dad6b54a583;
            float _Property_61f840140291436e8912958fef5953c3_Out_0 = Vector1_b53e914fc7984637b80324d57e274b5f;
            float _Property_aa3d6115a8f7408182a5ad6eaf4dea9c_Out_0 = Vector1_af1a9e8da22b4e08851f991318dec789;
            float _Property_318bfb7c8fcb48359c265b1829bb9333_Out_0 = Vector1_e937c75465ce4f99ba008e1a397c452b;
            float _CalculateWaveCustomFunction_babfa0b8a0ce4e20b113bd73598d4cd6_New_0;
            CalculateWave_float(_Property_4f1419a54dd7457eb25ffd09e1294716_Out_0, _Property_61f840140291436e8912958fef5953c3_Out_0, _Property_aa3d6115a8f7408182a5ad6eaf4dea9c_Out_0, _Property_318bfb7c8fcb48359c265b1829bb9333_Out_0, _CalculateWaveCustomFunction_babfa0b8a0ce4e20b113bd73598d4cd6_New_0);
            float3 _Vector3_fd59b684dbbf45499ba224d658a9f2ec_Out_0 = float3(0, _CalculateWaveCustomFunction_babfa0b8a0ce4e20b113bd73598d4cd6_New_0, 0);
            float3 _Add_bb0b83c602bf41909e47fdeba507406b_Out_2;
            Unity_Add_float3(_Vector3_fd59b684dbbf45499ba224d658a9f2ec_Out_0, _Property_4f1419a54dd7457eb25ffd09e1294716_Out_0, _Add_bb0b83c602bf41909e47fdeba507406b_Out_2);
            OutVector3_2 = _Add_bb0b83c602bf41909e47fdeba507406b_Out_2;
        }

        void Unity_Sign_float(float In, out float Out)
        {
            Out = sign(In);
        }

        void Unity_Multiply_float(float3 A, float3 B, out float3 Out)
        {
            Out = A * B;
        }

        void Unity_Add_float(float A, float B, out float Out)
        {
            Out = A + B;
        }

        void Unity_Subtract_float(float A, float B, out float Out)
        {
            Out = A - B;
        }

        void Unity_Absolute_float(float In, out float Out)
        {
            Out = abs(In);
        }

        void Unity_Saturate_float(float In, out float Out)
        {
            Out = saturate(In);
        }

        void Unity_Divide_float(float A, float B, out float Out)
        {
            Out = A / B;
        }

        void Unity_Combine_float(float R, float G, float B, float A, out float4 RGBA, out float3 RGB, out float2 RG)
        {
            RGBA = float4(R, G, B, A);
            RGB = float3(R, G, B);
            RG = float2(R, G);
        }

        void Unity_Smoothstep_float(float Edge1, float Edge2, float In, out float Out)
        {
            Out = smoothstep(Edge1, Edge2, In);
        }

        void Unity_Lerp_float3(float3 A, float3 B, float3 T, out float3 Out)
        {
            Out = lerp(A, B, T);
        }

        void Unity_Subtract_float3(float3 A, float3 B, out float3 Out)
        {
            Out = A - B;
        }

        void Unity_Normalize_float3(float3 In, out float3 Out)
        {
            Out = normalize(In);
        }

        void Unity_CrossProduct_float(float3 A, float3 B, out float3 Out)
        {
            Out = cross(A, B);
        }

        struct Bindings_Neighbors_df68ff96a41468e448052764fa9bdba1
        {
            float3 ObjectSpaceNormal;
        };

        void SG_Neighbors_df68ff96a41468e448052764fa9bdba1(float3 Vector3_8954E4F1, float Vector1_B5D26639, Bindings_Neighbors_df68ff96a41468e448052764fa9bdba1 IN, out float3 Neighbor1_1, out float3 Neighbor2_2)
        {
            float _Split_609f2b09351ec288b5ed925103f0f9e0_R_1 = IN.ObjectSpaceNormal[0];
            float _Split_609f2b09351ec288b5ed925103f0f9e0_G_2 = IN.ObjectSpaceNormal[1];
            float _Split_609f2b09351ec288b5ed925103f0f9e0_B_3 = IN.ObjectSpaceNormal[2];
            float _Split_609f2b09351ec288b5ed925103f0f9e0_A_4 = 0;
            float _Sign_e3911d5b4df6208882d5722613bfa7df_Out_1;
            Unity_Sign_float(_Split_609f2b09351ec288b5ed925103f0f9e0_R_1, _Sign_e3911d5b4df6208882d5722613bfa7df_Out_1);
            float3 _Property_8a0179cbbf558586ab0af83d3af77169_Out_0 = Vector3_8954E4F1;
            float3 _Multiply_a5a5ef928369cd84997685e6caac87b0_Out_2;
            Unity_Multiply_float(_Property_8a0179cbbf558586ab0af83d3af77169_Out_0, IN.ObjectSpaceNormal, _Multiply_a5a5ef928369cd84997685e6caac87b0_Out_2);
            float _Split_8977fb298fca9e8182c1edaad752997c_R_1 = _Multiply_a5a5ef928369cd84997685e6caac87b0_Out_2[0];
            float _Split_8977fb298fca9e8182c1edaad752997c_G_2 = _Multiply_a5a5ef928369cd84997685e6caac87b0_Out_2[1];
            float _Split_8977fb298fca9e8182c1edaad752997c_B_3 = _Multiply_a5a5ef928369cd84997685e6caac87b0_Out_2[2];
            float _Split_8977fb298fca9e8182c1edaad752997c_A_4 = 0;
            float _Add_eeda377c8b7c038596bc86d83f7f321e_Out_2;
            Unity_Add_float(_Split_8977fb298fca9e8182c1edaad752997c_R_1, _Split_8977fb298fca9e8182c1edaad752997c_G_2, _Add_eeda377c8b7c038596bc86d83f7f321e_Out_2);
            float _Add_61a089896fe3468b82b211a762b6ec21_Out_2;
            Unity_Add_float(_Add_eeda377c8b7c038596bc86d83f7f321e_Out_2, _Split_8977fb298fca9e8182c1edaad752997c_B_3, _Add_61a089896fe3468b82b211a762b6ec21_Out_2);
            float _Split_d79bc2f0568d4b8586a29273c4b01ac3_R_1 = IN.ObjectSpaceNormal[0];
            float _Split_d79bc2f0568d4b8586a29273c4b01ac3_G_2 = IN.ObjectSpaceNormal[1];
            float _Split_d79bc2f0568d4b8586a29273c4b01ac3_B_3 = IN.ObjectSpaceNormal[2];
            float _Split_d79bc2f0568d4b8586a29273c4b01ac3_A_4 = 0;
            float _Float_9d499dd7c2614a8589e8a1cb04daeb5d_Out_0 = 0.05;
            float _Split_802c7c2944faa986ba720f07d8db74ab_R_1 = _Property_8a0179cbbf558586ab0af83d3af77169_Out_0[0];
            float _Split_802c7c2944faa986ba720f07d8db74ab_G_2 = _Property_8a0179cbbf558586ab0af83d3af77169_Out_0[1];
            float _Split_802c7c2944faa986ba720f07d8db74ab_B_3 = _Property_8a0179cbbf558586ab0af83d3af77169_Out_0[2];
            float _Split_802c7c2944faa986ba720f07d8db74ab_A_4 = 0;
            float _Add_4555b76b861a778d97276d765af07483_Out_2;
            Unity_Add_float(_Float_9d499dd7c2614a8589e8a1cb04daeb5d_Out_0, _Split_802c7c2944faa986ba720f07d8db74ab_G_2, _Add_4555b76b861a778d97276d765af07483_Out_2);
            float _Multiply_1e591f42bea8b58c8797ab2c90faa345_Out_2;
            Unity_Multiply_float(_Split_d79bc2f0568d4b8586a29273c4b01ac3_G_2, _Add_4555b76b861a778d97276d765af07483_Out_2, _Multiply_1e591f42bea8b58c8797ab2c90faa345_Out_2);
            float _Subtract_68ba98ef07340b83b46992e63c68d7ca_Out_2;
            Unity_Subtract_float(_Add_61a089896fe3468b82b211a762b6ec21_Out_2, _Multiply_1e591f42bea8b58c8797ab2c90faa345_Out_2, _Subtract_68ba98ef07340b83b46992e63c68d7ca_Out_2);
            float _Multiply_d0572ed2941ee0879549c1da1c572b15_Out_2;
            Unity_Multiply_float(_Split_d79bc2f0568d4b8586a29273c4b01ac3_B_3, _Split_802c7c2944faa986ba720f07d8db74ab_B_3, _Multiply_d0572ed2941ee0879549c1da1c572b15_Out_2);
            float _Subtract_3d9ad256eba2ee8e839158fc0f45ee81_Out_2;
            Unity_Subtract_float(_Subtract_68ba98ef07340b83b46992e63c68d7ca_Out_2, _Multiply_d0572ed2941ee0879549c1da1c572b15_Out_2, _Subtract_3d9ad256eba2ee8e839158fc0f45ee81_Out_2);
            float _Float_654fffd8f366ff808a39bb5a6a1f1fdb_Out_0 = 0.01;
            float _Absolute_9fde5c96b588aa8fbed6af91404da630_Out_1;
            Unity_Absolute_float(_Split_609f2b09351ec288b5ed925103f0f9e0_R_1, _Absolute_9fde5c96b588aa8fbed6af91404da630_Out_1);
            float _Subtract_7173428a9289a58abbac98045c08ceb9_Out_2;
            Unity_Subtract_float(_Absolute_9fde5c96b588aa8fbed6af91404da630_Out_1, _Float_654fffd8f366ff808a39bb5a6a1f1fdb_Out_0, _Subtract_7173428a9289a58abbac98045c08ceb9_Out_2);
            float _Saturate_e8fcca777a01738ab601adda864265d1_Out_1;
            Unity_Saturate_float(_Subtract_7173428a9289a58abbac98045c08ceb9_Out_2, _Saturate_e8fcca777a01738ab601adda864265d1_Out_1);
            float _Add_71e184a7749fc9848483bc81af14e30e_Out_2;
            Unity_Add_float(_Float_654fffd8f366ff808a39bb5a6a1f1fdb_Out_0, _Saturate_e8fcca777a01738ab601adda864265d1_Out_1, _Add_71e184a7749fc9848483bc81af14e30e_Out_2);
            float _Divide_7cc37db0bc8630899cffaa8d565d6d55_Out_2;
            Unity_Divide_float(_Subtract_3d9ad256eba2ee8e839158fc0f45ee81_Out_2, _Add_71e184a7749fc9848483bc81af14e30e_Out_2, _Divide_7cc37db0bc8630899cffaa8d565d6d55_Out_2);
            float _Multiply_9c6b2289e8d2db84a1c0d5ca2a655b93_Out_2;
            Unity_Multiply_float(_Sign_e3911d5b4df6208882d5722613bfa7df_Out_1, _Divide_7cc37db0bc8630899cffaa8d565d6d55_Out_2, _Multiply_9c6b2289e8d2db84a1c0d5ca2a655b93_Out_2);
            float4 _Combine_8fcb17287675378fb024a84210c6677a_RGBA_4;
            float3 _Combine_8fcb17287675378fb024a84210c6677a_RGB_5;
            float2 _Combine_8fcb17287675378fb024a84210c6677a_RG_6;
            Unity_Combine_float(_Multiply_9c6b2289e8d2db84a1c0d5ca2a655b93_Out_2, _Add_4555b76b861a778d97276d765af07483_Out_2, _Split_802c7c2944faa986ba720f07d8db74ab_B_3, 0, _Combine_8fcb17287675378fb024a84210c6677a_RGBA_4, _Combine_8fcb17287675378fb024a84210c6677a_RGB_5, _Combine_8fcb17287675378fb024a84210c6677a_RG_6);
            float _Float_9b0eee4975b51e828a4269a5edcee5b1_Out_0 = 0.05;
            float _Split_f205281c3443d68aa9b0c1a2feace9df_R_1 = _Property_8a0179cbbf558586ab0af83d3af77169_Out_0[0];
            float _Split_f205281c3443d68aa9b0c1a2feace9df_G_2 = _Property_8a0179cbbf558586ab0af83d3af77169_Out_0[1];
            float _Split_f205281c3443d68aa9b0c1a2feace9df_B_3 = _Property_8a0179cbbf558586ab0af83d3af77169_Out_0[2];
            float _Split_f205281c3443d68aa9b0c1a2feace9df_A_4 = 0;
            float _Add_db37e0cfea249d8d9ede45e675501301_Out_2;
            Unity_Add_float(_Float_9b0eee4975b51e828a4269a5edcee5b1_Out_0, _Split_f205281c3443d68aa9b0c1a2feace9df_R_1, _Add_db37e0cfea249d8d9ede45e675501301_Out_2);
            float4 _Combine_9530e51a49a77f849e6eac5cb0ce15ad_RGBA_4;
            float3 _Combine_9530e51a49a77f849e6eac5cb0ce15ad_RGB_5;
            float2 _Combine_9530e51a49a77f849e6eac5cb0ce15ad_RG_6;
            Unity_Combine_float(_Add_db37e0cfea249d8d9ede45e675501301_Out_2, _Split_f205281c3443d68aa9b0c1a2feace9df_G_2, _Split_f205281c3443d68aa9b0c1a2feace9df_B_3, 0, _Combine_9530e51a49a77f849e6eac5cb0ce15ad_RGBA_4, _Combine_9530e51a49a77f849e6eac5cb0ce15ad_RGB_5, _Combine_9530e51a49a77f849e6eac5cb0ce15ad_RG_6);
            float _Multiply_625b1e551cdfd0829aa19290646e6739_Out_2;
            Unity_Multiply_float(-1, _Subtract_7173428a9289a58abbac98045c08ceb9_Out_2, _Multiply_625b1e551cdfd0829aa19290646e6739_Out_2);
            float _Smoothstep_b60ff4818667938f9be3fa9ecf03f163_Out_3;
            Unity_Smoothstep_float(0, 0.01, _Multiply_625b1e551cdfd0829aa19290646e6739_Out_2, _Smoothstep_b60ff4818667938f9be3fa9ecf03f163_Out_3);
            float3 _Lerp_7d115f30f5f0fc8cb46b9ba33440e47a_Out_3;
            Unity_Lerp_float3(_Combine_8fcb17287675378fb024a84210c6677a_RGB_5, _Combine_9530e51a49a77f849e6eac5cb0ce15ad_RGB_5, (_Smoothstep_b60ff4818667938f9be3fa9ecf03f163_Out_3.xxx), _Lerp_7d115f30f5f0fc8cb46b9ba33440e47a_Out_3);
            float3 _Subtract_24a46f67e2876e80b3b01e0b77a50456_Out_2;
            Unity_Subtract_float3(_Lerp_7d115f30f5f0fc8cb46b9ba33440e47a_Out_3, _Property_8a0179cbbf558586ab0af83d3af77169_Out_0, _Subtract_24a46f67e2876e80b3b01e0b77a50456_Out_2);
            float3 _Normalize_f544245d7bd37f8fbaee4a87c2403675_Out_1;
            Unity_Normalize_float3(_Subtract_24a46f67e2876e80b3b01e0b77a50456_Out_2, _Normalize_f544245d7bd37f8fbaee4a87c2403675_Out_1);
            float3 _Normalize_9a359141b951ea84b3220ad5c3123b7c_Out_1;
            Unity_Normalize_float3(IN.ObjectSpaceNormal, _Normalize_9a359141b951ea84b3220ad5c3123b7c_Out_1);
            float3 _CrossProduct_1dbde50ff8c546829c8b59ec1630c252_Out_2;
            Unity_CrossProduct_float(_Normalize_f544245d7bd37f8fbaee4a87c2403675_Out_1, _Normalize_9a359141b951ea84b3220ad5c3123b7c_Out_1, _CrossProduct_1dbde50ff8c546829c8b59ec1630c252_Out_2);
            float3 _Normalize_3eb509f6a593a18d856dabb44191008a_Out_1;
            Unity_Normalize_float3(_CrossProduct_1dbde50ff8c546829c8b59ec1630c252_Out_2, _Normalize_3eb509f6a593a18d856dabb44191008a_Out_1);
            float _Property_4448eb2908e4228e840b80f046abf12e_Out_0 = Vector1_B5D26639;
            float3 _Multiply_37bf5602003a2885bb744b13f93296fe_Out_2;
            Unity_Multiply_float(_Normalize_3eb509f6a593a18d856dabb44191008a_Out_1, (_Property_4448eb2908e4228e840b80f046abf12e_Out_0.xxx), _Multiply_37bf5602003a2885bb744b13f93296fe_Out_2);
            float3 _Add_bd2759426a510983979415cc0e46c2d4_Out_2;
            Unity_Add_float3(_Multiply_37bf5602003a2885bb744b13f93296fe_Out_2, _Property_8a0179cbbf558586ab0af83d3af77169_Out_0, _Add_bd2759426a510983979415cc0e46c2d4_Out_2);
            float3 _Multiply_273e0ecf939b858582031df2660ec861_Out_2;
            Unity_Multiply_float(_Normalize_f544245d7bd37f8fbaee4a87c2403675_Out_1, (_Property_4448eb2908e4228e840b80f046abf12e_Out_0.xxx), _Multiply_273e0ecf939b858582031df2660ec861_Out_2);
            float3 _Add_2259c39580c12b8297f6b81a426bd3c0_Out_2;
            Unity_Add_float3(_Property_8a0179cbbf558586ab0af83d3af77169_Out_0, _Multiply_273e0ecf939b858582031df2660ec861_Out_2, _Add_2259c39580c12b8297f6b81a426bd3c0_Out_2);
            Neighbor1_1 = _Add_bd2759426a510983979415cc0e46c2d4_Out_2;
            Neighbor2_2 = _Add_2259c39580c12b8297f6b81a426bd3c0_Out_2;
        }

        struct Bindings_NewNormal_c8d6b0224f233004190264cd9e5ca192
        {
        };

        void SG_NewNormal_c8d6b0224f233004190264cd9e5ca192(float3 Vector3_988291EB, float3 Vector3_EB4E2DE7, float3 Vector3_1973C67A, Bindings_NewNormal_c8d6b0224f233004190264cd9e5ca192 IN, out float3 OutVector3_1)
        {
            float3 _Property_fd61227e10c3348bb61b5f1d39e68328_Out_0 = Vector3_EB4E2DE7;
            float3 _Property_b8cacddd74e568809402e877976dffa8_Out_0 = Vector3_988291EB;
            float3 _Subtract_1f3c648b5559d28f9acfbe1af362b37b_Out_2;
            Unity_Subtract_float3(_Property_fd61227e10c3348bb61b5f1d39e68328_Out_0, _Property_b8cacddd74e568809402e877976dffa8_Out_0, _Subtract_1f3c648b5559d28f9acfbe1af362b37b_Out_2);
            float3 _Normalize_888167f431fbc788b4ad8e2092e7f28b_Out_1;
            Unity_Normalize_float3(_Subtract_1f3c648b5559d28f9acfbe1af362b37b_Out_2, _Normalize_888167f431fbc788b4ad8e2092e7f28b_Out_1);
            float3 _Property_6008e654ce04488cbe1d5caac8afe08a_Out_0 = Vector3_1973C67A;
            float3 _Subtract_208696e2abb5f18d84a5878fd9a81553_Out_2;
            Unity_Subtract_float3(_Property_6008e654ce04488cbe1d5caac8afe08a_Out_0, _Property_b8cacddd74e568809402e877976dffa8_Out_0, _Subtract_208696e2abb5f18d84a5878fd9a81553_Out_2);
            float3 _Normalize_7250d413c7450d8aaa805c923843f2dd_Out_1;
            Unity_Normalize_float3(_Subtract_208696e2abb5f18d84a5878fd9a81553_Out_2, _Normalize_7250d413c7450d8aaa805c923843f2dd_Out_1);
            float3 _CrossProduct_5566a5ab348aa8809609d834bbe4451f_Out_2;
            Unity_CrossProduct_float(_Normalize_888167f431fbc788b4ad8e2092e7f28b_Out_1, _Normalize_7250d413c7450d8aaa805c923843f2dd_Out_1, _CrossProduct_5566a5ab348aa8809609d834bbe4451f_Out_2);
            float3 _Normalize_a640075232b8c78498848f10ca1e7ba8_Out_1;
            Unity_Normalize_float3(_CrossProduct_5566a5ab348aa8809609d834bbe4451f_Out_2, _Normalize_a640075232b8c78498848f10ca1e7ba8_Out_1);
            OutVector3_1 = _Normalize_a640075232b8c78498848f10ca1e7ba8_Out_1;
        }

        void Unity_SceneDepth_Linear01_float(float4 UV, out float Out)
        {
            Out = Linear01Depth(SHADERGRAPH_SAMPLE_SCENE_DEPTH(UV.xy), _ZBufferParams);
        }

        void Unity_Clamp_float(float In, float Min, float Max, out float Out)
        {
            Out = clamp(In, Min, Max);
        }

        void Unity_Lerp_float4(float4 A, float4 B, float4 T, out float4 Out)
        {
            Out = lerp(A, B, T);
        }

        void Unity_Multiply_float(float2 A, float2 B, out float2 Out)
        {
            Out = A * B;
        }

        void Unity_NormalBlend_float(float3 A, float3 B, out float3 Out)
        {
            Out = SafeNormalize(float3(A.rg + B.rg, A.b * B.b));
        }

        void Unity_NormalStrength_float(float3 In, float Strength, out float3 Out)
        {
            Out = float3(In.rg * Strength, lerp(1, In.b, saturate(Strength)));
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

        void Unity_Power_float(float A, float B, out float Out)
        {
            Out = pow(A, B);
        }

        void Unity_FresnelEffect_float(float3 Normal, float3 ViewDir, float Power, out float Out)
        {
            Out = pow((1.0 - saturate(dot(normalize(Normal), normalize(ViewDir)))), Power);
        }

        void Unity_Maximum_float(float A, float B, out float Out)
        {
            Out = max(A, B);
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
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            UnityTexture2D _Property_52e3e5c2509c4df984c47cc092c466d8_Out_0 = UnityBuildTexture2DStructNoScale(Texture2D_c773e99a7970488a9d6bcbd08807a081);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Property_736f2734d3e643c398a2586801e4cc17_Out_0 = Vector1_f133b62c64f14525895d4b7d5afdd79f;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Property_7851f92dfd4d4302a7beb8a17dd4bd8b_Out_0 = Vector1_45a27133fb8b4113a0746c0d1954c57b;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float2 _CalculateNoiseCustomFunction_a0c2603a229c4d299ac32a59a8664765_Out_0;
            CalculateNoise_float(IN.AbsoluteWorldSpacePosition, _Property_736f2734d3e643c398a2586801e4cc17_Out_0, _Property_7851f92dfd4d4302a7beb8a17dd4bd8b_Out_0, _CalculateNoiseCustomFunction_a0c2603a229c4d299ac32a59a8664765_Out_0);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            #if defined(SHADER_API_GLES) && (SHADER_TARGET < 30)
              float4 _SampleTexture2DLOD_fe1fede5211b477cbca93fc4c2f76d12_RGBA_0 = float4(0.0f, 0.0f, 0.0f, 1.0f);
            #else
              float4 _SampleTexture2DLOD_fe1fede5211b477cbca93fc4c2f76d12_RGBA_0 = SAMPLE_TEXTURE2D_LOD(_Property_52e3e5c2509c4df984c47cc092c466d8_Out_0.tex, _Property_52e3e5c2509c4df984c47cc092c466d8_Out_0.samplerstate, _CalculateNoiseCustomFunction_a0c2603a229c4d299ac32a59a8664765_Out_0, 0);
            #endif
            float _SampleTexture2DLOD_fe1fede5211b477cbca93fc4c2f76d12_R_5 = _SampleTexture2DLOD_fe1fede5211b477cbca93fc4c2f76d12_RGBA_0.r;
            float _SampleTexture2DLOD_fe1fede5211b477cbca93fc4c2f76d12_G_6 = _SampleTexture2DLOD_fe1fede5211b477cbca93fc4c2f76d12_RGBA_0.g;
            float _SampleTexture2DLOD_fe1fede5211b477cbca93fc4c2f76d12_B_7 = _SampleTexture2DLOD_fe1fede5211b477cbca93fc4c2f76d12_RGBA_0.b;
            float _SampleTexture2DLOD_fe1fede5211b477cbca93fc4c2f76d12_A_8 = _SampleTexture2DLOD_fe1fede5211b477cbca93fc4c2f76d12_RGBA_0.a;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Property_fab5f23c0a9e449bb605bc77e42f5c55_Out_0 = Vector1_57cb41fdb69545e8b53a796761acb08d;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Multiply_be99c4e752404fa4b5a42166d27a7cc0_Out_2;
            Unity_Multiply_float(_SampleTexture2DLOD_fe1fede5211b477cbca93fc4c2f76d12_R_5, _Property_fab5f23c0a9e449bb605bc77e42f5c55_Out_0, _Multiply_be99c4e752404fa4b5a42166d27a7cc0_Out_2);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float3 _Vector3_9e43fda8531d474c890ea7595c453773_Out_0 = float3(0, _Multiply_be99c4e752404fa4b5a42166d27a7cc0_Out_2, 0);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Property_0ea561f00aaf4fbb8fdfeab5a2a0dd08_Out_0 = Vector1_4da6e98cbfa5408fb6352b485aa76d05;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Property_e4937aaae9bc4c098cebea935a7ee1ea_Out_0 = Vector1_b266d7e71de2489ea084cb27060cacca;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Property_2d3470f269ac4477a8dc467d895fe472_Out_0 = Vector1_d09cc377b0524610970658ffdacd497f;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            Bindings_WaveDisplacement_81e878f76fb7ea94995742377f80ac36 _WaveDisplacement_5f54fff0c6334b26bedfff0b63df12b3;
            float3 _WaveDisplacement_5f54fff0c6334b26bedfff0b63df12b3_OutVector3_2;
            SG_WaveDisplacement_81e878f76fb7ea94995742377f80ac36(IN.AbsoluteWorldSpacePosition, _Property_0ea561f00aaf4fbb8fdfeab5a2a0dd08_Out_0, _Property_e4937aaae9bc4c098cebea935a7ee1ea_Out_0, _Property_2d3470f269ac4477a8dc467d895fe472_Out_0, _WaveDisplacement_5f54fff0c6334b26bedfff0b63df12b3, _WaveDisplacement_5f54fff0c6334b26bedfff0b63df12b3_OutVector3_2);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float3 _Add_942b46ad98eb4925b4bfa00a0a5781c7_Out_2;
            Unity_Add_float3(_Vector3_9e43fda8531d474c890ea7595c453773_Out_0, _WaveDisplacement_5f54fff0c6334b26bedfff0b63df12b3_OutVector3_2, _Add_942b46ad98eb4925b4bfa00a0a5781c7_Out_2);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float3 _Transform_3fce2cebd3054a72a946f448c89c2e3d_Out_1 = TransformWorldToObject(GetCameraRelativePositionWS(_Add_942b46ad98eb4925b4bfa00a0a5781c7_Out_2.xyz));
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            Bindings_Neighbors_df68ff96a41468e448052764fa9bdba1 _Neighbors_e648c110f4754cefb6491b8c82fe074f;
            _Neighbors_e648c110f4754cefb6491b8c82fe074f.ObjectSpaceNormal = IN.ObjectSpaceNormal;
            float3 _Neighbors_e648c110f4754cefb6491b8c82fe074f_Neighbor1_1;
            float3 _Neighbors_e648c110f4754cefb6491b8c82fe074f_Neighbor2_2;
            SG_Neighbors_df68ff96a41468e448052764fa9bdba1(IN.AbsoluteWorldSpacePosition, 0.01, _Neighbors_e648c110f4754cefb6491b8c82fe074f, _Neighbors_e648c110f4754cefb6491b8c82fe074f_Neighbor1_1, _Neighbors_e648c110f4754cefb6491b8c82fe074f_Neighbor2_2);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            Bindings_WaveDisplacement_81e878f76fb7ea94995742377f80ac36 _WaveDisplacement_2fa36261c2534cbe8639a41fdfe4910a;
            float3 _WaveDisplacement_2fa36261c2534cbe8639a41fdfe4910a_OutVector3_2;
            SG_WaveDisplacement_81e878f76fb7ea94995742377f80ac36(_Neighbors_e648c110f4754cefb6491b8c82fe074f_Neighbor1_1, _Property_0ea561f00aaf4fbb8fdfeab5a2a0dd08_Out_0, _Property_e4937aaae9bc4c098cebea935a7ee1ea_Out_0, _Property_2d3470f269ac4477a8dc467d895fe472_Out_0, _WaveDisplacement_2fa36261c2534cbe8639a41fdfe4910a, _WaveDisplacement_2fa36261c2534cbe8639a41fdfe4910a_OutVector3_2);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            Bindings_WaveDisplacement_81e878f76fb7ea94995742377f80ac36 _WaveDisplacement_c9fd16638edd4ccfaf9e3eb2efa55bfa;
            float3 _WaveDisplacement_c9fd16638edd4ccfaf9e3eb2efa55bfa_OutVector3_2;
            SG_WaveDisplacement_81e878f76fb7ea94995742377f80ac36(_Neighbors_e648c110f4754cefb6491b8c82fe074f_Neighbor2_2, _Property_0ea561f00aaf4fbb8fdfeab5a2a0dd08_Out_0, _Property_e4937aaae9bc4c098cebea935a7ee1ea_Out_0, _Property_2d3470f269ac4477a8dc467d895fe472_Out_0, _WaveDisplacement_c9fd16638edd4ccfaf9e3eb2efa55bfa, _WaveDisplacement_c9fd16638edd4ccfaf9e3eb2efa55bfa_OutVector3_2);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            Bindings_NewNormal_c8d6b0224f233004190264cd9e5ca192 _NewNormal_e9af9405a4a54034863b00f2fbd4c2b6;
            float3 _NewNormal_e9af9405a4a54034863b00f2fbd4c2b6_OutVector3_1;
            SG_NewNormal_c8d6b0224f233004190264cd9e5ca192(_WaveDisplacement_5f54fff0c6334b26bedfff0b63df12b3_OutVector3_2, _WaveDisplacement_2fa36261c2534cbe8639a41fdfe4910a_OutVector3_2, _WaveDisplacement_c9fd16638edd4ccfaf9e3eb2efa55bfa_OutVector3_2, _NewNormal_e9af9405a4a54034863b00f2fbd4c2b6, _NewNormal_e9af9405a4a54034863b00f2fbd4c2b6_OutVector3_1);
            #endif
            description.Position = _Transform_3fce2cebd3054a72a946f448c89c2e3d_Out_1;
            description.Normal = _NewNormal_e9af9405a4a54034863b00f2fbd4c2b6_OutVector3_1;
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
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float4 _Property_1a8602fe3b054c0d851c1d1bb94bfe17_Out_0 = Color_8b23519dc989478fa19369d8efda52f6;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float4 _Property_0ccb5c18d3eb40c5980df5a141a382a5_Out_0 = Color_213be8c364f242cdbabe6ef25c0efbd9;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _SceneDepth_7981f193eac94d87b6f2b2920d4cb528_Out_1;
            Unity_SceneDepth_Linear01_float(float4(IN.ScreenPosition.xy / IN.ScreenPosition.w, 0, 0), _SceneDepth_7981f193eac94d87b6f2b2920d4cb528_Out_1);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Multiply_abe194398c584d8a8934b67cf64c0da1_Out_2;
            Unity_Multiply_float(_SceneDepth_7981f193eac94d87b6f2b2920d4cb528_Out_1, _ProjectionParams.z, _Multiply_abe194398c584d8a8934b67cf64c0da1_Out_2);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float4 _ScreenPosition_68dc8d80a07f4706a6a9a2f0230737ed_Out_0 = IN.ScreenPosition;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Split_7f7e1aadecd54073aff51bd05fa759bc_R_1 = _ScreenPosition_68dc8d80a07f4706a6a9a2f0230737ed_Out_0[0];
            float _Split_7f7e1aadecd54073aff51bd05fa759bc_G_2 = _ScreenPosition_68dc8d80a07f4706a6a9a2f0230737ed_Out_0[1];
            float _Split_7f7e1aadecd54073aff51bd05fa759bc_B_3 = _ScreenPosition_68dc8d80a07f4706a6a9a2f0230737ed_Out_0[2];
            float _Split_7f7e1aadecd54073aff51bd05fa759bc_A_4 = _ScreenPosition_68dc8d80a07f4706a6a9a2f0230737ed_Out_0[3];
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Multiply_7ba2c47eef0f4b89b3b4eb4a8ed31f83_Out_2;
            Unity_Multiply_float(_Split_7f7e1aadecd54073aff51bd05fa759bc_A_4, 1, _Multiply_7ba2c47eef0f4b89b3b4eb4a8ed31f83_Out_2);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Subtract_dc119b36f453437e913cdd2a2623efd2_Out_2;
            Unity_Subtract_float(_Multiply_abe194398c584d8a8934b67cf64c0da1_Out_2, _Multiply_7ba2c47eef0f4b89b3b4eb4a8ed31f83_Out_2, _Subtract_dc119b36f453437e913cdd2a2623efd2_Out_2);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Property_ab48707fe6db419eb51ece316bb6313b_Out_0 = Vector1_cc6e37850778480781084b62719d58c3;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Multiply_3a48e77d57694a868985ec140162c2ef_Out_2;
            Unity_Multiply_float(_Subtract_dc119b36f453437e913cdd2a2623efd2_Out_2, _Property_ab48707fe6db419eb51ece316bb6313b_Out_0, _Multiply_3a48e77d57694a868985ec140162c2ef_Out_2);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Clamp_bb886dbf030149bd8ffa4e979d6e9c46_Out_3;
            Unity_Clamp_float(_Multiply_3a48e77d57694a868985ec140162c2ef_Out_2, 0, 1, _Clamp_bb886dbf030149bd8ffa4e979d6e9c46_Out_3);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float4 _Lerp_5c0550f6966845719d72583200d6ffde_Out_3;
            Unity_Lerp_float4(_Property_1a8602fe3b054c0d851c1d1bb94bfe17_Out_0, _Property_0ccb5c18d3eb40c5980df5a141a382a5_Out_0, (_Clamp_bb886dbf030149bd8ffa4e979d6e9c46_Out_3.xxxx), _Lerp_5c0550f6966845719d72583200d6ffde_Out_3);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            UnityTexture2D _Property_c40253d6917345378f175ecc530ca7ab_Out_0 = UnityBuildTexture2DStructNoScale(Texture2D_c23a14955d09487682029de7801dfc17);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Property_4b555a0d276d4492ae020102ab7d5f6d_Out_0 = Vector1_ba885a3188cc490b9d04d880f79cd1b0;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Multiply_37551db39e924fb4b75b35ca95ad2f80_Out_2;
            Unity_Multiply_float(_Property_4b555a0d276d4492ae020102ab7d5f6d_Out_0, 2, _Multiply_37551db39e924fb4b75b35ca95ad2f80_Out_2);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Property_bdd005a7ca7d46889210f1373e17240d_Out_0 = Vector1_28676355978446f285fbfe6f1a574316;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float2 _CalculateNoiseCustomFunction_1f60b49d6983442f91651f8642c54ee9_Out_0;
            CalculateNoise_float(IN.ObjectSpacePosition, _Multiply_37551db39e924fb4b75b35ca95ad2f80_Out_2, _Property_bdd005a7ca7d46889210f1373e17240d_Out_0, _CalculateNoiseCustomFunction_1f60b49d6983442f91651f8642c54ee9_Out_0);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            #if defined(SHADER_API_GLES) && (SHADER_TARGET < 30)
              float4 _SampleTexture2DLOD_3997426701814db8be9b6de9e67109eb_RGBA_0 = float4(0.0f, 0.0f, 0.0f, 1.0f);
            #else
              float4 _SampleTexture2DLOD_3997426701814db8be9b6de9e67109eb_RGBA_0 = SAMPLE_TEXTURE2D_LOD(_Property_c40253d6917345378f175ecc530ca7ab_Out_0.tex, _Property_c40253d6917345378f175ecc530ca7ab_Out_0.samplerstate, _CalculateNoiseCustomFunction_1f60b49d6983442f91651f8642c54ee9_Out_0, 0);
            #endif
            _SampleTexture2DLOD_3997426701814db8be9b6de9e67109eb_RGBA_0.rgb = UnpackNormal(_SampleTexture2DLOD_3997426701814db8be9b6de9e67109eb_RGBA_0);
            float _SampleTexture2DLOD_3997426701814db8be9b6de9e67109eb_R_5 = _SampleTexture2DLOD_3997426701814db8be9b6de9e67109eb_RGBA_0.r;
            float _SampleTexture2DLOD_3997426701814db8be9b6de9e67109eb_G_6 = _SampleTexture2DLOD_3997426701814db8be9b6de9e67109eb_RGBA_0.g;
            float _SampleTexture2DLOD_3997426701814db8be9b6de9e67109eb_B_7 = _SampleTexture2DLOD_3997426701814db8be9b6de9e67109eb_RGBA_0.b;
            float _SampleTexture2DLOD_3997426701814db8be9b6de9e67109eb_A_8 = _SampleTexture2DLOD_3997426701814db8be9b6de9e67109eb_RGBA_0.a;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Multiply_8d56ca2bac4a4606b07568c5fe27fab5_Out_2;
            Unity_Multiply_float(_Multiply_37551db39e924fb4b75b35ca95ad2f80_Out_2, -0.1, _Multiply_8d56ca2bac4a4606b07568c5fe27fab5_Out_2);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float2 _CalculateNoiseCustomFunction_4767eb28ae364522aa093fefd13ee291_Out_0;
            CalculateNoise_float(IN.ObjectSpacePosition, _Multiply_8d56ca2bac4a4606b07568c5fe27fab5_Out_2, _Property_bdd005a7ca7d46889210f1373e17240d_Out_0, _CalculateNoiseCustomFunction_4767eb28ae364522aa093fefd13ee291_Out_0);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float2 _Multiply_4856504d81624c5183fee40deaf3653d_Out_2;
            Unity_Multiply_float(_CalculateNoiseCustomFunction_4767eb28ae364522aa093fefd13ee291_Out_0, float2(-1, -1), _Multiply_4856504d81624c5183fee40deaf3653d_Out_2);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            #if defined(SHADER_API_GLES) && (SHADER_TARGET < 30)
              float4 _SampleTexture2DLOD_e11df39f24034543be10c1c451ba3abd_RGBA_0 = float4(0.0f, 0.0f, 0.0f, 1.0f);
            #else
              float4 _SampleTexture2DLOD_e11df39f24034543be10c1c451ba3abd_RGBA_0 = SAMPLE_TEXTURE2D_LOD(_Property_c40253d6917345378f175ecc530ca7ab_Out_0.tex, _Property_c40253d6917345378f175ecc530ca7ab_Out_0.samplerstate, _Multiply_4856504d81624c5183fee40deaf3653d_Out_2, 0);
            #endif
            _SampleTexture2DLOD_e11df39f24034543be10c1c451ba3abd_RGBA_0.rgb = UnpackNormal(_SampleTexture2DLOD_e11df39f24034543be10c1c451ba3abd_RGBA_0);
            float _SampleTexture2DLOD_e11df39f24034543be10c1c451ba3abd_R_5 = _SampleTexture2DLOD_e11df39f24034543be10c1c451ba3abd_RGBA_0.r;
            float _SampleTexture2DLOD_e11df39f24034543be10c1c451ba3abd_G_6 = _SampleTexture2DLOD_e11df39f24034543be10c1c451ba3abd_RGBA_0.g;
            float _SampleTexture2DLOD_e11df39f24034543be10c1c451ba3abd_B_7 = _SampleTexture2DLOD_e11df39f24034543be10c1c451ba3abd_RGBA_0.b;
            float _SampleTexture2DLOD_e11df39f24034543be10c1c451ba3abd_A_8 = _SampleTexture2DLOD_e11df39f24034543be10c1c451ba3abd_RGBA_0.a;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float3 _NormalBlend_52c804727a444571ac19c41a6a0d1a36_Out_2;
            Unity_NormalBlend_float((_SampleTexture2DLOD_3997426701814db8be9b6de9e67109eb_RGBA_0.xyz), (_SampleTexture2DLOD_e11df39f24034543be10c1c451ba3abd_RGBA_0.xyz), _NormalBlend_52c804727a444571ac19c41a6a0d1a36_Out_2);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Property_41566b7bdd964ab0a886d5b7c7e06eae_Out_0 = Vector1_bf79c34252fe4364976a552df388436c;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float3 _NormalStrength_07e07b43827048248308d9c7a65f7578_Out_2;
            Unity_NormalStrength_float(_NormalBlend_52c804727a444571ac19c41a6a0d1a36_Out_2, _Property_41566b7bdd964ab0a886d5b7c7e06eae_Out_0, _NormalStrength_07e07b43827048248308d9c7a65f7578_Out_2);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float3 _Normalize_7b05a0adf23947229dbcb1136c5b2557_Out_1;
            Unity_Normalize_float3(_NormalStrength_07e07b43827048248308d9c7a65f7578_Out_2, _Normalize_7b05a0adf23947229dbcb1136c5b2557_Out_1);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Property_9a299c60e1344371ae6f61dcf6f95e4e_Out_0 = Vector1_775ef292c0cf4cda91aecf4845cd67bd;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Property_f3f8f4f602e046b88135452055729933_Out_0 = Vector1_3578eca6d8ac4314939bfb16bfdbde27;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            Bindings_CelShading_802007f66a3e13d42802a3875ed35370 _CelShading_ad7871fb5bf1404e9f1fa53eab938a3a;
            _CelShading_ad7871fb5bf1404e9f1fa53eab938a3a.WorldSpaceNormal = IN.WorldSpaceNormal;
            _CelShading_ad7871fb5bf1404e9f1fa53eab938a3a.WorldSpaceTangent = IN.WorldSpaceTangent;
            _CelShading_ad7871fb5bf1404e9f1fa53eab938a3a.WorldSpaceBiTangent = IN.WorldSpaceBiTangent;
            _CelShading_ad7871fb5bf1404e9f1fa53eab938a3a.WorldSpaceViewDirection = IN.WorldSpaceViewDirection;
            _CelShading_ad7871fb5bf1404e9f1fa53eab938a3a.AbsoluteWorldSpacePosition = IN.AbsoluteWorldSpacePosition;
            _CelShading_ad7871fb5bf1404e9f1fa53eab938a3a.uv0 = IN.uv0;
            half4 _CelShading_ad7871fb5bf1404e9f1fa53eab938a3a_Color_1;
            SG_CelShading_802007f66a3e13d42802a3875ed35370(_Lerp_5c0550f6966845719d72583200d6ffde_Out_3, _Normalize_7b05a0adf23947229dbcb1136c5b2557_Out_1, _Property_9a299c60e1344371ae6f61dcf6f95e4e_Out_0, _Property_f3f8f4f602e046b88135452055729933_Out_0, UnityBuildTexture2DStructNoScale(_CelShading_ad7871fb5bf1404e9f1fa53eab938a3a_Texture2D9666dd2f2f8f4fddb6c2a5c61ddfa9bd_3072536305), _CelShading_ad7871fb5bf1404e9f1fa53eab938a3a, _CelShading_ad7871fb5bf1404e9f1fa53eab938a3a_Color_1);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Power_795157e2d171465e908da5d45e938d28_Out_2;
            Unity_Power_float(_Clamp_bb886dbf030149bd8ffa4e979d6e9c46_Out_3, 0.7, _Power_795157e2d171465e908da5d45e938d28_Out_2);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Property_41daac0ae2c74098ac9111219e1e3b3b_Out_0 = Vector1_5be3734798f64a63b2827794b79391c3;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _FresnelEffect_98bea57f43804c61a33cabcb5a3e5400_Out_3;
            Unity_FresnelEffect_float(IN.WorldSpaceNormal, IN.WorldSpaceViewDirection, _Property_41daac0ae2c74098ac9111219e1e3b3b_Out_0, _FresnelEffect_98bea57f43804c61a33cabcb5a3e5400_Out_3);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Maximum_85ae2b8b22d5495fb3e04a01fa179389_Out_2;
            Unity_Maximum_float(_FresnelEffect_98bea57f43804c61a33cabcb5a3e5400_Out_3, 0.7, _Maximum_85ae2b8b22d5495fb3e04a01fa179389_Out_2);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Multiply_465f2016ad9d499981e3a39ae56b8aef_Out_2;
            Unity_Multiply_float(_Power_795157e2d171465e908da5d45e938d28_Out_2, _Maximum_85ae2b8b22d5495fb3e04a01fa179389_Out_2, _Multiply_465f2016ad9d499981e3a39ae56b8aef_Out_2);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Saturate_00659ca349c74eea975fbb11da46e6bf_Out_1;
            Unity_Saturate_float(_Multiply_465f2016ad9d499981e3a39ae56b8aef_Out_2, _Saturate_00659ca349c74eea975fbb11da46e6bf_Out_1);
            #endif
            surface.BaseColor = (_CelShading_ad7871fb5bf1404e9f1fa53eab938a3a_Color_1.xyz);
            surface.Alpha = _Saturate_00659ca349c74eea975fbb11da46e6bf_Out_1;
            return surface;
        }

            // --------------------------------------------------
            // Build Graph Inputs

            VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
        {
            VertexDescriptionInputs output;
            ZERO_INITIALIZE(VertexDescriptionInputs, output);

        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        output.ObjectSpaceNormal =           input.normalOS;
        #endif

        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        output.WorldSpaceNormal =            TransformObjectToWorldNormal(input.normalOS);
        #endif

        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        output.ObjectSpaceTangent =          input.tangentOS.xyz;
        #endif

        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        output.WorldSpaceTangent =           TransformObjectToWorldDir(input.tangentOS.xyz);
        #endif

        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        output.ObjectSpaceBiTangent =        normalize(cross(input.normalOS, input.tangentOS) * (input.tangentOS.w > 0.0f ? 1.0f : -1.0f) * GetOddNegativeScale());
        #endif

        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        output.WorldSpaceBiTangent =         TransformObjectToWorldDir(output.ObjectSpaceBiTangent);
        #endif

        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        output.ObjectSpacePosition =         input.positionOS;
        #endif

        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        output.AbsoluteWorldSpacePosition =  GetAbsolutePositionWS(TransformObjectToWorld(input.positionOS));
        #endif


            return output;
        }
            SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);

        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        // must use interpolated tangent, bitangent and normal before they are normalized in the pixel shader.
        #endif

        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        float3 unnormalizedNormalWS = input.normalWS;
        #endif

        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        const float renormFactor = 1.0 / length(unnormalizedNormalWS);
        #endif


        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        // use bitangent on the fly like in hdrp
        #endif

        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        // IMPORTANT! If we ever support Flip on double sided materials ensure bitangent and tangent are NOT flipped.
        #endif

        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        float crossSign = (input.tangentWS.w > 0.0 ? 1.0 : -1.0) * GetOddNegativeScale();
        #endif

        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        float3 bitang = crossSign * cross(input.normalWS.xyz, input.tangentWS.xyz);
        #endif


        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        output.WorldSpaceNormal =            renormFactor*input.normalWS.xyz;		// we want a unit length Normal Vector node in shader graph
        #endif


        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        // to preserve mikktspace compliance we use same scale renormFactor as was used on the normal.
        #endif

        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        // This is explained in section 2.2 in "surface gradient based bump mapping framework"
        #endif

        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        output.WorldSpaceTangent =           renormFactor*input.tangentWS.xyz;
        #endif

        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        output.WorldSpaceBiTangent =         renormFactor*bitang;
        #endif


        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        output.WorldSpaceViewDirection =     input.viewDirectionWS; //TODO: by default normalized in HD, but not in universal
        #endif

        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        output.WorldSpacePosition =          input.positionWS;
        #endif

        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        output.ObjectSpacePosition =         TransformWorldToObject(input.positionWS);
        #endif

        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        output.AbsoluteWorldSpacePosition =  GetAbsolutePositionWS(input.positionWS);
        #endif

        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        output.ScreenPosition =              ComputeScreenPos(TransformWorldToHClip(input.positionWS), _ProjectionParams.x);
        #endif

        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        output.uv0 =                         input.texCoord0;
        #endif

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
            #pragma multi_compile_local _ CEL_SPECULAR_ON

        #if defined(CEL_SPECULAR_ON)
            #define KEYWORD_PERMUTATION_0
        #else
            #define KEYWORD_PERMUTATION_1
        #endif


            // Defines
        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        #define _SURFACE_TYPE_TRANSPARENT 1
        #endif

        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        #define ATTRIBUTES_NEED_NORMAL
        #endif

        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        #define ATTRIBUTES_NEED_TANGENT
        #endif

        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        #define VARYINGS_NEED_POSITION_WS
        #endif

        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        #define VARYINGS_NEED_NORMAL_WS
        #endif

        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        #define VARYINGS_NEED_VIEWDIRECTION_WS
        #endif

            #define FEATURES_GRAPH_VERTEX
            /* WARNING: $splice Could not find named fragment 'PassInstancing' */
            #define SHADERPASS SHADERPASS_SHADOWCASTER
        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        #define REQUIRE_DEPTH_TEXTURE
        #endif
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
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float3 positionOS : POSITION;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float3 normalOS : NORMAL;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float4 tangentOS : TANGENT;
            #endif
            #if UNITY_ANY_INSTANCING_ENABLED
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            uint instanceID : INSTANCEID_SEMANTIC;
            #endif
            #endif
        };
        struct Varyings
        {
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float4 positionCS : SV_POSITION;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float3 positionWS;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float3 normalWS;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float3 viewDirectionWS;
            #endif
            #if UNITY_ANY_INSTANCING_ENABLED
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
            #endif
        };
        struct SurfaceDescriptionInputs
        {
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float3 WorldSpaceNormal;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float3 WorldSpaceViewDirection;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float3 WorldSpacePosition;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float4 ScreenPosition;
            #endif
        };
        struct VertexDescriptionInputs
        {
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float3 ObjectSpaceNormal;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float3 WorldSpaceNormal;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float3 ObjectSpaceTangent;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float3 WorldSpaceTangent;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float3 ObjectSpaceBiTangent;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float3 WorldSpaceBiTangent;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float3 ObjectSpacePosition;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float3 AbsoluteWorldSpacePosition;
            #endif
        };
        struct PackedVaryings
        {
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float4 positionCS : SV_POSITION;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float3 interp0 : TEXCOORD0;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float3 interp1 : TEXCOORD1;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float3 interp2 : TEXCOORD2;
            #endif
            #if UNITY_ANY_INSTANCING_ENABLED
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
            #endif
        };

            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        PackedVaryings PackVaryings (Varyings input)
        {
            PackedVaryings output;
            output.positionCS = input.positionCS;
            output.interp0.xyz =  input.positionWS;
            output.interp1.xyz =  input.normalWS;
            output.interp2.xyz =  input.viewDirectionWS;
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
            output.viewDirectionWS = input.interp2.xyz;
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
        #endif

            // --------------------------------------------------
            // Graph

            // Graph Properties
            CBUFFER_START(UnityPerMaterial)
        float4 _CelShading_ad7871fb5bf1404e9f1fa53eab938a3a_Texture2D9666dd2f2f8f4fddb6c2a5c61ddfa9bd_3072536305_TexelSize;
        float Vector1_0e212c6562134ef7add6f6766d6419b4;
        float4 Color_213be8c364f242cdbabe6ef25c0efbd9;
        float4 Color_8b23519dc989478fa19369d8efda52f6;
        float Vector1_40661633426347afb1a705a362429b36;
        float4 Texture2D_c773e99a7970488a9d6bcbd08807a081_TexelSize;
        float Vector1_45a27133fb8b4113a0746c0d1954c57b;
        float Vector1_f133b62c64f14525895d4b7d5afdd79f;
        float Vector1_57cb41fdb69545e8b53a796761acb08d;
        float Vector1_76a8f3bdc13d493dbc42eca7e6de6c4e;
        float4 Texture2D_c23a14955d09487682029de7801dfc17_TexelSize;
        float Vector1_28676355978446f285fbfe6f1a574316;
        float Vector1_ba885a3188cc490b9d04d880f79cd1b0;
        float Vector1_bf79c34252fe4364976a552df388436c;
        float Vector1_f586ab191e3e497491345288306a18c0;
        float Vector1_4da6e98cbfa5408fb6352b485aa76d05;
        float Vector1_b266d7e71de2489ea084cb27060cacca;
        float Vector1_d09cc377b0524610970658ffdacd497f;
        float Vector1_1f93deba5516424f927cee1eb2c93ba3;
        float Vector1_cc6e37850778480781084b62719d58c3;
        float Vector1_5be3734798f64a63b2827794b79391c3;
        float Vector1_3578eca6d8ac4314939bfb16bfdbde27;
        float Vector1_775ef292c0cf4cda91aecf4845cd67bd;
        CBUFFER_END

        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(_CelShading_ad7871fb5bf1404e9f1fa53eab938a3a_Texture2D9666dd2f2f8f4fddb6c2a5c61ddfa9bd_3072536305);
        SAMPLER(sampler_CelShading_ad7871fb5bf1404e9f1fa53eab938a3a_Texture2D9666dd2f2f8f4fddb6c2a5c61ddfa9bd_3072536305);
        TEXTURE2D(Texture2D_c773e99a7970488a9d6bcbd08807a081);
        SAMPLER(samplerTexture2D_c773e99a7970488a9d6bcbd08807a081);
        TEXTURE2D(Texture2D_c23a14955d09487682029de7801dfc17);
        SAMPLER(samplerTexture2D_c23a14955d09487682029de7801dfc17);

            // Graph Functions
            
        // c9d40f89572cca24e7b23cdf7d4f0592
        #include "Assets/Rendering/Shaders/Functions/URPFonctions.hlsl"

        void Unity_Multiply_float(float A, float B, out float Out)
        {
            Out = A * B;
        }

        void Unity_Add_float3(float3 A, float3 B, out float3 Out)
        {
            Out = A + B;
        }

        struct Bindings_WaveDisplacement_81e878f76fb7ea94995742377f80ac36
        {
        };

        void SG_WaveDisplacement_81e878f76fb7ea94995742377f80ac36(float3 Vector3_774baa01c42a49f6abc99dad6b54a583, float Vector1_b53e914fc7984637b80324d57e274b5f, float Vector1_af1a9e8da22b4e08851f991318dec789, float Vector1_e937c75465ce4f99ba008e1a397c452b, Bindings_WaveDisplacement_81e878f76fb7ea94995742377f80ac36 IN, out float3 OutVector3_2)
        {
            float3 _Property_4f1419a54dd7457eb25ffd09e1294716_Out_0 = Vector3_774baa01c42a49f6abc99dad6b54a583;
            float _Property_61f840140291436e8912958fef5953c3_Out_0 = Vector1_b53e914fc7984637b80324d57e274b5f;
            float _Property_aa3d6115a8f7408182a5ad6eaf4dea9c_Out_0 = Vector1_af1a9e8da22b4e08851f991318dec789;
            float _Property_318bfb7c8fcb48359c265b1829bb9333_Out_0 = Vector1_e937c75465ce4f99ba008e1a397c452b;
            float _CalculateWaveCustomFunction_babfa0b8a0ce4e20b113bd73598d4cd6_New_0;
            CalculateWave_float(_Property_4f1419a54dd7457eb25ffd09e1294716_Out_0, _Property_61f840140291436e8912958fef5953c3_Out_0, _Property_aa3d6115a8f7408182a5ad6eaf4dea9c_Out_0, _Property_318bfb7c8fcb48359c265b1829bb9333_Out_0, _CalculateWaveCustomFunction_babfa0b8a0ce4e20b113bd73598d4cd6_New_0);
            float3 _Vector3_fd59b684dbbf45499ba224d658a9f2ec_Out_0 = float3(0, _CalculateWaveCustomFunction_babfa0b8a0ce4e20b113bd73598d4cd6_New_0, 0);
            float3 _Add_bb0b83c602bf41909e47fdeba507406b_Out_2;
            Unity_Add_float3(_Vector3_fd59b684dbbf45499ba224d658a9f2ec_Out_0, _Property_4f1419a54dd7457eb25ffd09e1294716_Out_0, _Add_bb0b83c602bf41909e47fdeba507406b_Out_2);
            OutVector3_2 = _Add_bb0b83c602bf41909e47fdeba507406b_Out_2;
        }

        void Unity_Sign_float(float In, out float Out)
        {
            Out = sign(In);
        }

        void Unity_Multiply_float(float3 A, float3 B, out float3 Out)
        {
            Out = A * B;
        }

        void Unity_Add_float(float A, float B, out float Out)
        {
            Out = A + B;
        }

        void Unity_Subtract_float(float A, float B, out float Out)
        {
            Out = A - B;
        }

        void Unity_Absolute_float(float In, out float Out)
        {
            Out = abs(In);
        }

        void Unity_Saturate_float(float In, out float Out)
        {
            Out = saturate(In);
        }

        void Unity_Divide_float(float A, float B, out float Out)
        {
            Out = A / B;
        }

        void Unity_Combine_float(float R, float G, float B, float A, out float4 RGBA, out float3 RGB, out float2 RG)
        {
            RGBA = float4(R, G, B, A);
            RGB = float3(R, G, B);
            RG = float2(R, G);
        }

        void Unity_Smoothstep_float(float Edge1, float Edge2, float In, out float Out)
        {
            Out = smoothstep(Edge1, Edge2, In);
        }

        void Unity_Lerp_float3(float3 A, float3 B, float3 T, out float3 Out)
        {
            Out = lerp(A, B, T);
        }

        void Unity_Subtract_float3(float3 A, float3 B, out float3 Out)
        {
            Out = A - B;
        }

        void Unity_Normalize_float3(float3 In, out float3 Out)
        {
            Out = normalize(In);
        }

        void Unity_CrossProduct_float(float3 A, float3 B, out float3 Out)
        {
            Out = cross(A, B);
        }

        struct Bindings_Neighbors_df68ff96a41468e448052764fa9bdba1
        {
            float3 ObjectSpaceNormal;
        };

        void SG_Neighbors_df68ff96a41468e448052764fa9bdba1(float3 Vector3_8954E4F1, float Vector1_B5D26639, Bindings_Neighbors_df68ff96a41468e448052764fa9bdba1 IN, out float3 Neighbor1_1, out float3 Neighbor2_2)
        {
            float _Split_609f2b09351ec288b5ed925103f0f9e0_R_1 = IN.ObjectSpaceNormal[0];
            float _Split_609f2b09351ec288b5ed925103f0f9e0_G_2 = IN.ObjectSpaceNormal[1];
            float _Split_609f2b09351ec288b5ed925103f0f9e0_B_3 = IN.ObjectSpaceNormal[2];
            float _Split_609f2b09351ec288b5ed925103f0f9e0_A_4 = 0;
            float _Sign_e3911d5b4df6208882d5722613bfa7df_Out_1;
            Unity_Sign_float(_Split_609f2b09351ec288b5ed925103f0f9e0_R_1, _Sign_e3911d5b4df6208882d5722613bfa7df_Out_1);
            float3 _Property_8a0179cbbf558586ab0af83d3af77169_Out_0 = Vector3_8954E4F1;
            float3 _Multiply_a5a5ef928369cd84997685e6caac87b0_Out_2;
            Unity_Multiply_float(_Property_8a0179cbbf558586ab0af83d3af77169_Out_0, IN.ObjectSpaceNormal, _Multiply_a5a5ef928369cd84997685e6caac87b0_Out_2);
            float _Split_8977fb298fca9e8182c1edaad752997c_R_1 = _Multiply_a5a5ef928369cd84997685e6caac87b0_Out_2[0];
            float _Split_8977fb298fca9e8182c1edaad752997c_G_2 = _Multiply_a5a5ef928369cd84997685e6caac87b0_Out_2[1];
            float _Split_8977fb298fca9e8182c1edaad752997c_B_3 = _Multiply_a5a5ef928369cd84997685e6caac87b0_Out_2[2];
            float _Split_8977fb298fca9e8182c1edaad752997c_A_4 = 0;
            float _Add_eeda377c8b7c038596bc86d83f7f321e_Out_2;
            Unity_Add_float(_Split_8977fb298fca9e8182c1edaad752997c_R_1, _Split_8977fb298fca9e8182c1edaad752997c_G_2, _Add_eeda377c8b7c038596bc86d83f7f321e_Out_2);
            float _Add_61a089896fe3468b82b211a762b6ec21_Out_2;
            Unity_Add_float(_Add_eeda377c8b7c038596bc86d83f7f321e_Out_2, _Split_8977fb298fca9e8182c1edaad752997c_B_3, _Add_61a089896fe3468b82b211a762b6ec21_Out_2);
            float _Split_d79bc2f0568d4b8586a29273c4b01ac3_R_1 = IN.ObjectSpaceNormal[0];
            float _Split_d79bc2f0568d4b8586a29273c4b01ac3_G_2 = IN.ObjectSpaceNormal[1];
            float _Split_d79bc2f0568d4b8586a29273c4b01ac3_B_3 = IN.ObjectSpaceNormal[2];
            float _Split_d79bc2f0568d4b8586a29273c4b01ac3_A_4 = 0;
            float _Float_9d499dd7c2614a8589e8a1cb04daeb5d_Out_0 = 0.05;
            float _Split_802c7c2944faa986ba720f07d8db74ab_R_1 = _Property_8a0179cbbf558586ab0af83d3af77169_Out_0[0];
            float _Split_802c7c2944faa986ba720f07d8db74ab_G_2 = _Property_8a0179cbbf558586ab0af83d3af77169_Out_0[1];
            float _Split_802c7c2944faa986ba720f07d8db74ab_B_3 = _Property_8a0179cbbf558586ab0af83d3af77169_Out_0[2];
            float _Split_802c7c2944faa986ba720f07d8db74ab_A_4 = 0;
            float _Add_4555b76b861a778d97276d765af07483_Out_2;
            Unity_Add_float(_Float_9d499dd7c2614a8589e8a1cb04daeb5d_Out_0, _Split_802c7c2944faa986ba720f07d8db74ab_G_2, _Add_4555b76b861a778d97276d765af07483_Out_2);
            float _Multiply_1e591f42bea8b58c8797ab2c90faa345_Out_2;
            Unity_Multiply_float(_Split_d79bc2f0568d4b8586a29273c4b01ac3_G_2, _Add_4555b76b861a778d97276d765af07483_Out_2, _Multiply_1e591f42bea8b58c8797ab2c90faa345_Out_2);
            float _Subtract_68ba98ef07340b83b46992e63c68d7ca_Out_2;
            Unity_Subtract_float(_Add_61a089896fe3468b82b211a762b6ec21_Out_2, _Multiply_1e591f42bea8b58c8797ab2c90faa345_Out_2, _Subtract_68ba98ef07340b83b46992e63c68d7ca_Out_2);
            float _Multiply_d0572ed2941ee0879549c1da1c572b15_Out_2;
            Unity_Multiply_float(_Split_d79bc2f0568d4b8586a29273c4b01ac3_B_3, _Split_802c7c2944faa986ba720f07d8db74ab_B_3, _Multiply_d0572ed2941ee0879549c1da1c572b15_Out_2);
            float _Subtract_3d9ad256eba2ee8e839158fc0f45ee81_Out_2;
            Unity_Subtract_float(_Subtract_68ba98ef07340b83b46992e63c68d7ca_Out_2, _Multiply_d0572ed2941ee0879549c1da1c572b15_Out_2, _Subtract_3d9ad256eba2ee8e839158fc0f45ee81_Out_2);
            float _Float_654fffd8f366ff808a39bb5a6a1f1fdb_Out_0 = 0.01;
            float _Absolute_9fde5c96b588aa8fbed6af91404da630_Out_1;
            Unity_Absolute_float(_Split_609f2b09351ec288b5ed925103f0f9e0_R_1, _Absolute_9fde5c96b588aa8fbed6af91404da630_Out_1);
            float _Subtract_7173428a9289a58abbac98045c08ceb9_Out_2;
            Unity_Subtract_float(_Absolute_9fde5c96b588aa8fbed6af91404da630_Out_1, _Float_654fffd8f366ff808a39bb5a6a1f1fdb_Out_0, _Subtract_7173428a9289a58abbac98045c08ceb9_Out_2);
            float _Saturate_e8fcca777a01738ab601adda864265d1_Out_1;
            Unity_Saturate_float(_Subtract_7173428a9289a58abbac98045c08ceb9_Out_2, _Saturate_e8fcca777a01738ab601adda864265d1_Out_1);
            float _Add_71e184a7749fc9848483bc81af14e30e_Out_2;
            Unity_Add_float(_Float_654fffd8f366ff808a39bb5a6a1f1fdb_Out_0, _Saturate_e8fcca777a01738ab601adda864265d1_Out_1, _Add_71e184a7749fc9848483bc81af14e30e_Out_2);
            float _Divide_7cc37db0bc8630899cffaa8d565d6d55_Out_2;
            Unity_Divide_float(_Subtract_3d9ad256eba2ee8e839158fc0f45ee81_Out_2, _Add_71e184a7749fc9848483bc81af14e30e_Out_2, _Divide_7cc37db0bc8630899cffaa8d565d6d55_Out_2);
            float _Multiply_9c6b2289e8d2db84a1c0d5ca2a655b93_Out_2;
            Unity_Multiply_float(_Sign_e3911d5b4df6208882d5722613bfa7df_Out_1, _Divide_7cc37db0bc8630899cffaa8d565d6d55_Out_2, _Multiply_9c6b2289e8d2db84a1c0d5ca2a655b93_Out_2);
            float4 _Combine_8fcb17287675378fb024a84210c6677a_RGBA_4;
            float3 _Combine_8fcb17287675378fb024a84210c6677a_RGB_5;
            float2 _Combine_8fcb17287675378fb024a84210c6677a_RG_6;
            Unity_Combine_float(_Multiply_9c6b2289e8d2db84a1c0d5ca2a655b93_Out_2, _Add_4555b76b861a778d97276d765af07483_Out_2, _Split_802c7c2944faa986ba720f07d8db74ab_B_3, 0, _Combine_8fcb17287675378fb024a84210c6677a_RGBA_4, _Combine_8fcb17287675378fb024a84210c6677a_RGB_5, _Combine_8fcb17287675378fb024a84210c6677a_RG_6);
            float _Float_9b0eee4975b51e828a4269a5edcee5b1_Out_0 = 0.05;
            float _Split_f205281c3443d68aa9b0c1a2feace9df_R_1 = _Property_8a0179cbbf558586ab0af83d3af77169_Out_0[0];
            float _Split_f205281c3443d68aa9b0c1a2feace9df_G_2 = _Property_8a0179cbbf558586ab0af83d3af77169_Out_0[1];
            float _Split_f205281c3443d68aa9b0c1a2feace9df_B_3 = _Property_8a0179cbbf558586ab0af83d3af77169_Out_0[2];
            float _Split_f205281c3443d68aa9b0c1a2feace9df_A_4 = 0;
            float _Add_db37e0cfea249d8d9ede45e675501301_Out_2;
            Unity_Add_float(_Float_9b0eee4975b51e828a4269a5edcee5b1_Out_0, _Split_f205281c3443d68aa9b0c1a2feace9df_R_1, _Add_db37e0cfea249d8d9ede45e675501301_Out_2);
            float4 _Combine_9530e51a49a77f849e6eac5cb0ce15ad_RGBA_4;
            float3 _Combine_9530e51a49a77f849e6eac5cb0ce15ad_RGB_5;
            float2 _Combine_9530e51a49a77f849e6eac5cb0ce15ad_RG_6;
            Unity_Combine_float(_Add_db37e0cfea249d8d9ede45e675501301_Out_2, _Split_f205281c3443d68aa9b0c1a2feace9df_G_2, _Split_f205281c3443d68aa9b0c1a2feace9df_B_3, 0, _Combine_9530e51a49a77f849e6eac5cb0ce15ad_RGBA_4, _Combine_9530e51a49a77f849e6eac5cb0ce15ad_RGB_5, _Combine_9530e51a49a77f849e6eac5cb0ce15ad_RG_6);
            float _Multiply_625b1e551cdfd0829aa19290646e6739_Out_2;
            Unity_Multiply_float(-1, _Subtract_7173428a9289a58abbac98045c08ceb9_Out_2, _Multiply_625b1e551cdfd0829aa19290646e6739_Out_2);
            float _Smoothstep_b60ff4818667938f9be3fa9ecf03f163_Out_3;
            Unity_Smoothstep_float(0, 0.01, _Multiply_625b1e551cdfd0829aa19290646e6739_Out_2, _Smoothstep_b60ff4818667938f9be3fa9ecf03f163_Out_3);
            float3 _Lerp_7d115f30f5f0fc8cb46b9ba33440e47a_Out_3;
            Unity_Lerp_float3(_Combine_8fcb17287675378fb024a84210c6677a_RGB_5, _Combine_9530e51a49a77f849e6eac5cb0ce15ad_RGB_5, (_Smoothstep_b60ff4818667938f9be3fa9ecf03f163_Out_3.xxx), _Lerp_7d115f30f5f0fc8cb46b9ba33440e47a_Out_3);
            float3 _Subtract_24a46f67e2876e80b3b01e0b77a50456_Out_2;
            Unity_Subtract_float3(_Lerp_7d115f30f5f0fc8cb46b9ba33440e47a_Out_3, _Property_8a0179cbbf558586ab0af83d3af77169_Out_0, _Subtract_24a46f67e2876e80b3b01e0b77a50456_Out_2);
            float3 _Normalize_f544245d7bd37f8fbaee4a87c2403675_Out_1;
            Unity_Normalize_float3(_Subtract_24a46f67e2876e80b3b01e0b77a50456_Out_2, _Normalize_f544245d7bd37f8fbaee4a87c2403675_Out_1);
            float3 _Normalize_9a359141b951ea84b3220ad5c3123b7c_Out_1;
            Unity_Normalize_float3(IN.ObjectSpaceNormal, _Normalize_9a359141b951ea84b3220ad5c3123b7c_Out_1);
            float3 _CrossProduct_1dbde50ff8c546829c8b59ec1630c252_Out_2;
            Unity_CrossProduct_float(_Normalize_f544245d7bd37f8fbaee4a87c2403675_Out_1, _Normalize_9a359141b951ea84b3220ad5c3123b7c_Out_1, _CrossProduct_1dbde50ff8c546829c8b59ec1630c252_Out_2);
            float3 _Normalize_3eb509f6a593a18d856dabb44191008a_Out_1;
            Unity_Normalize_float3(_CrossProduct_1dbde50ff8c546829c8b59ec1630c252_Out_2, _Normalize_3eb509f6a593a18d856dabb44191008a_Out_1);
            float _Property_4448eb2908e4228e840b80f046abf12e_Out_0 = Vector1_B5D26639;
            float3 _Multiply_37bf5602003a2885bb744b13f93296fe_Out_2;
            Unity_Multiply_float(_Normalize_3eb509f6a593a18d856dabb44191008a_Out_1, (_Property_4448eb2908e4228e840b80f046abf12e_Out_0.xxx), _Multiply_37bf5602003a2885bb744b13f93296fe_Out_2);
            float3 _Add_bd2759426a510983979415cc0e46c2d4_Out_2;
            Unity_Add_float3(_Multiply_37bf5602003a2885bb744b13f93296fe_Out_2, _Property_8a0179cbbf558586ab0af83d3af77169_Out_0, _Add_bd2759426a510983979415cc0e46c2d4_Out_2);
            float3 _Multiply_273e0ecf939b858582031df2660ec861_Out_2;
            Unity_Multiply_float(_Normalize_f544245d7bd37f8fbaee4a87c2403675_Out_1, (_Property_4448eb2908e4228e840b80f046abf12e_Out_0.xxx), _Multiply_273e0ecf939b858582031df2660ec861_Out_2);
            float3 _Add_2259c39580c12b8297f6b81a426bd3c0_Out_2;
            Unity_Add_float3(_Property_8a0179cbbf558586ab0af83d3af77169_Out_0, _Multiply_273e0ecf939b858582031df2660ec861_Out_2, _Add_2259c39580c12b8297f6b81a426bd3c0_Out_2);
            Neighbor1_1 = _Add_bd2759426a510983979415cc0e46c2d4_Out_2;
            Neighbor2_2 = _Add_2259c39580c12b8297f6b81a426bd3c0_Out_2;
        }

        struct Bindings_NewNormal_c8d6b0224f233004190264cd9e5ca192
        {
        };

        void SG_NewNormal_c8d6b0224f233004190264cd9e5ca192(float3 Vector3_988291EB, float3 Vector3_EB4E2DE7, float3 Vector3_1973C67A, Bindings_NewNormal_c8d6b0224f233004190264cd9e5ca192 IN, out float3 OutVector3_1)
        {
            float3 _Property_fd61227e10c3348bb61b5f1d39e68328_Out_0 = Vector3_EB4E2DE7;
            float3 _Property_b8cacddd74e568809402e877976dffa8_Out_0 = Vector3_988291EB;
            float3 _Subtract_1f3c648b5559d28f9acfbe1af362b37b_Out_2;
            Unity_Subtract_float3(_Property_fd61227e10c3348bb61b5f1d39e68328_Out_0, _Property_b8cacddd74e568809402e877976dffa8_Out_0, _Subtract_1f3c648b5559d28f9acfbe1af362b37b_Out_2);
            float3 _Normalize_888167f431fbc788b4ad8e2092e7f28b_Out_1;
            Unity_Normalize_float3(_Subtract_1f3c648b5559d28f9acfbe1af362b37b_Out_2, _Normalize_888167f431fbc788b4ad8e2092e7f28b_Out_1);
            float3 _Property_6008e654ce04488cbe1d5caac8afe08a_Out_0 = Vector3_1973C67A;
            float3 _Subtract_208696e2abb5f18d84a5878fd9a81553_Out_2;
            Unity_Subtract_float3(_Property_6008e654ce04488cbe1d5caac8afe08a_Out_0, _Property_b8cacddd74e568809402e877976dffa8_Out_0, _Subtract_208696e2abb5f18d84a5878fd9a81553_Out_2);
            float3 _Normalize_7250d413c7450d8aaa805c923843f2dd_Out_1;
            Unity_Normalize_float3(_Subtract_208696e2abb5f18d84a5878fd9a81553_Out_2, _Normalize_7250d413c7450d8aaa805c923843f2dd_Out_1);
            float3 _CrossProduct_5566a5ab348aa8809609d834bbe4451f_Out_2;
            Unity_CrossProduct_float(_Normalize_888167f431fbc788b4ad8e2092e7f28b_Out_1, _Normalize_7250d413c7450d8aaa805c923843f2dd_Out_1, _CrossProduct_5566a5ab348aa8809609d834bbe4451f_Out_2);
            float3 _Normalize_a640075232b8c78498848f10ca1e7ba8_Out_1;
            Unity_Normalize_float3(_CrossProduct_5566a5ab348aa8809609d834bbe4451f_Out_2, _Normalize_a640075232b8c78498848f10ca1e7ba8_Out_1);
            OutVector3_1 = _Normalize_a640075232b8c78498848f10ca1e7ba8_Out_1;
        }

        void Unity_SceneDepth_Linear01_float(float4 UV, out float Out)
        {
            Out = Linear01Depth(SHADERGRAPH_SAMPLE_SCENE_DEPTH(UV.xy), _ZBufferParams);
        }

        void Unity_Clamp_float(float In, float Min, float Max, out float Out)
        {
            Out = clamp(In, Min, Max);
        }

        void Unity_Power_float(float A, float B, out float Out)
        {
            Out = pow(A, B);
        }

        void Unity_FresnelEffect_float(float3 Normal, float3 ViewDir, float Power, out float Out)
        {
            Out = pow((1.0 - saturate(dot(normalize(Normal), normalize(ViewDir)))), Power);
        }

        void Unity_Maximum_float(float A, float B, out float Out)
        {
            Out = max(A, B);
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
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            UnityTexture2D _Property_52e3e5c2509c4df984c47cc092c466d8_Out_0 = UnityBuildTexture2DStructNoScale(Texture2D_c773e99a7970488a9d6bcbd08807a081);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Property_736f2734d3e643c398a2586801e4cc17_Out_0 = Vector1_f133b62c64f14525895d4b7d5afdd79f;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Property_7851f92dfd4d4302a7beb8a17dd4bd8b_Out_0 = Vector1_45a27133fb8b4113a0746c0d1954c57b;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float2 _CalculateNoiseCustomFunction_a0c2603a229c4d299ac32a59a8664765_Out_0;
            CalculateNoise_float(IN.AbsoluteWorldSpacePosition, _Property_736f2734d3e643c398a2586801e4cc17_Out_0, _Property_7851f92dfd4d4302a7beb8a17dd4bd8b_Out_0, _CalculateNoiseCustomFunction_a0c2603a229c4d299ac32a59a8664765_Out_0);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            #if defined(SHADER_API_GLES) && (SHADER_TARGET < 30)
              float4 _SampleTexture2DLOD_fe1fede5211b477cbca93fc4c2f76d12_RGBA_0 = float4(0.0f, 0.0f, 0.0f, 1.0f);
            #else
              float4 _SampleTexture2DLOD_fe1fede5211b477cbca93fc4c2f76d12_RGBA_0 = SAMPLE_TEXTURE2D_LOD(_Property_52e3e5c2509c4df984c47cc092c466d8_Out_0.tex, _Property_52e3e5c2509c4df984c47cc092c466d8_Out_0.samplerstate, _CalculateNoiseCustomFunction_a0c2603a229c4d299ac32a59a8664765_Out_0, 0);
            #endif
            float _SampleTexture2DLOD_fe1fede5211b477cbca93fc4c2f76d12_R_5 = _SampleTexture2DLOD_fe1fede5211b477cbca93fc4c2f76d12_RGBA_0.r;
            float _SampleTexture2DLOD_fe1fede5211b477cbca93fc4c2f76d12_G_6 = _SampleTexture2DLOD_fe1fede5211b477cbca93fc4c2f76d12_RGBA_0.g;
            float _SampleTexture2DLOD_fe1fede5211b477cbca93fc4c2f76d12_B_7 = _SampleTexture2DLOD_fe1fede5211b477cbca93fc4c2f76d12_RGBA_0.b;
            float _SampleTexture2DLOD_fe1fede5211b477cbca93fc4c2f76d12_A_8 = _SampleTexture2DLOD_fe1fede5211b477cbca93fc4c2f76d12_RGBA_0.a;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Property_fab5f23c0a9e449bb605bc77e42f5c55_Out_0 = Vector1_57cb41fdb69545e8b53a796761acb08d;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Multiply_be99c4e752404fa4b5a42166d27a7cc0_Out_2;
            Unity_Multiply_float(_SampleTexture2DLOD_fe1fede5211b477cbca93fc4c2f76d12_R_5, _Property_fab5f23c0a9e449bb605bc77e42f5c55_Out_0, _Multiply_be99c4e752404fa4b5a42166d27a7cc0_Out_2);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float3 _Vector3_9e43fda8531d474c890ea7595c453773_Out_0 = float3(0, _Multiply_be99c4e752404fa4b5a42166d27a7cc0_Out_2, 0);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Property_0ea561f00aaf4fbb8fdfeab5a2a0dd08_Out_0 = Vector1_4da6e98cbfa5408fb6352b485aa76d05;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Property_e4937aaae9bc4c098cebea935a7ee1ea_Out_0 = Vector1_b266d7e71de2489ea084cb27060cacca;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Property_2d3470f269ac4477a8dc467d895fe472_Out_0 = Vector1_d09cc377b0524610970658ffdacd497f;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            Bindings_WaveDisplacement_81e878f76fb7ea94995742377f80ac36 _WaveDisplacement_5f54fff0c6334b26bedfff0b63df12b3;
            float3 _WaveDisplacement_5f54fff0c6334b26bedfff0b63df12b3_OutVector3_2;
            SG_WaveDisplacement_81e878f76fb7ea94995742377f80ac36(IN.AbsoluteWorldSpacePosition, _Property_0ea561f00aaf4fbb8fdfeab5a2a0dd08_Out_0, _Property_e4937aaae9bc4c098cebea935a7ee1ea_Out_0, _Property_2d3470f269ac4477a8dc467d895fe472_Out_0, _WaveDisplacement_5f54fff0c6334b26bedfff0b63df12b3, _WaveDisplacement_5f54fff0c6334b26bedfff0b63df12b3_OutVector3_2);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float3 _Add_942b46ad98eb4925b4bfa00a0a5781c7_Out_2;
            Unity_Add_float3(_Vector3_9e43fda8531d474c890ea7595c453773_Out_0, _WaveDisplacement_5f54fff0c6334b26bedfff0b63df12b3_OutVector3_2, _Add_942b46ad98eb4925b4bfa00a0a5781c7_Out_2);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float3 _Transform_3fce2cebd3054a72a946f448c89c2e3d_Out_1 = TransformWorldToObject(GetCameraRelativePositionWS(_Add_942b46ad98eb4925b4bfa00a0a5781c7_Out_2.xyz));
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            Bindings_Neighbors_df68ff96a41468e448052764fa9bdba1 _Neighbors_e648c110f4754cefb6491b8c82fe074f;
            _Neighbors_e648c110f4754cefb6491b8c82fe074f.ObjectSpaceNormal = IN.ObjectSpaceNormal;
            float3 _Neighbors_e648c110f4754cefb6491b8c82fe074f_Neighbor1_1;
            float3 _Neighbors_e648c110f4754cefb6491b8c82fe074f_Neighbor2_2;
            SG_Neighbors_df68ff96a41468e448052764fa9bdba1(IN.AbsoluteWorldSpacePosition, 0.01, _Neighbors_e648c110f4754cefb6491b8c82fe074f, _Neighbors_e648c110f4754cefb6491b8c82fe074f_Neighbor1_1, _Neighbors_e648c110f4754cefb6491b8c82fe074f_Neighbor2_2);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            Bindings_WaveDisplacement_81e878f76fb7ea94995742377f80ac36 _WaveDisplacement_2fa36261c2534cbe8639a41fdfe4910a;
            float3 _WaveDisplacement_2fa36261c2534cbe8639a41fdfe4910a_OutVector3_2;
            SG_WaveDisplacement_81e878f76fb7ea94995742377f80ac36(_Neighbors_e648c110f4754cefb6491b8c82fe074f_Neighbor1_1, _Property_0ea561f00aaf4fbb8fdfeab5a2a0dd08_Out_0, _Property_e4937aaae9bc4c098cebea935a7ee1ea_Out_0, _Property_2d3470f269ac4477a8dc467d895fe472_Out_0, _WaveDisplacement_2fa36261c2534cbe8639a41fdfe4910a, _WaveDisplacement_2fa36261c2534cbe8639a41fdfe4910a_OutVector3_2);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            Bindings_WaveDisplacement_81e878f76fb7ea94995742377f80ac36 _WaveDisplacement_c9fd16638edd4ccfaf9e3eb2efa55bfa;
            float3 _WaveDisplacement_c9fd16638edd4ccfaf9e3eb2efa55bfa_OutVector3_2;
            SG_WaveDisplacement_81e878f76fb7ea94995742377f80ac36(_Neighbors_e648c110f4754cefb6491b8c82fe074f_Neighbor2_2, _Property_0ea561f00aaf4fbb8fdfeab5a2a0dd08_Out_0, _Property_e4937aaae9bc4c098cebea935a7ee1ea_Out_0, _Property_2d3470f269ac4477a8dc467d895fe472_Out_0, _WaveDisplacement_c9fd16638edd4ccfaf9e3eb2efa55bfa, _WaveDisplacement_c9fd16638edd4ccfaf9e3eb2efa55bfa_OutVector3_2);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            Bindings_NewNormal_c8d6b0224f233004190264cd9e5ca192 _NewNormal_e9af9405a4a54034863b00f2fbd4c2b6;
            float3 _NewNormal_e9af9405a4a54034863b00f2fbd4c2b6_OutVector3_1;
            SG_NewNormal_c8d6b0224f233004190264cd9e5ca192(_WaveDisplacement_5f54fff0c6334b26bedfff0b63df12b3_OutVector3_2, _WaveDisplacement_2fa36261c2534cbe8639a41fdfe4910a_OutVector3_2, _WaveDisplacement_c9fd16638edd4ccfaf9e3eb2efa55bfa_OutVector3_2, _NewNormal_e9af9405a4a54034863b00f2fbd4c2b6, _NewNormal_e9af9405a4a54034863b00f2fbd4c2b6_OutVector3_1);
            #endif
            description.Position = _Transform_3fce2cebd3054a72a946f448c89c2e3d_Out_1;
            description.Normal = _NewNormal_e9af9405a4a54034863b00f2fbd4c2b6_OutVector3_1;
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
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _SceneDepth_7981f193eac94d87b6f2b2920d4cb528_Out_1;
            Unity_SceneDepth_Linear01_float(float4(IN.ScreenPosition.xy / IN.ScreenPosition.w, 0, 0), _SceneDepth_7981f193eac94d87b6f2b2920d4cb528_Out_1);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Multiply_abe194398c584d8a8934b67cf64c0da1_Out_2;
            Unity_Multiply_float(_SceneDepth_7981f193eac94d87b6f2b2920d4cb528_Out_1, _ProjectionParams.z, _Multiply_abe194398c584d8a8934b67cf64c0da1_Out_2);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float4 _ScreenPosition_68dc8d80a07f4706a6a9a2f0230737ed_Out_0 = IN.ScreenPosition;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Split_7f7e1aadecd54073aff51bd05fa759bc_R_1 = _ScreenPosition_68dc8d80a07f4706a6a9a2f0230737ed_Out_0[0];
            float _Split_7f7e1aadecd54073aff51bd05fa759bc_G_2 = _ScreenPosition_68dc8d80a07f4706a6a9a2f0230737ed_Out_0[1];
            float _Split_7f7e1aadecd54073aff51bd05fa759bc_B_3 = _ScreenPosition_68dc8d80a07f4706a6a9a2f0230737ed_Out_0[2];
            float _Split_7f7e1aadecd54073aff51bd05fa759bc_A_4 = _ScreenPosition_68dc8d80a07f4706a6a9a2f0230737ed_Out_0[3];
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Multiply_7ba2c47eef0f4b89b3b4eb4a8ed31f83_Out_2;
            Unity_Multiply_float(_Split_7f7e1aadecd54073aff51bd05fa759bc_A_4, 1, _Multiply_7ba2c47eef0f4b89b3b4eb4a8ed31f83_Out_2);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Subtract_dc119b36f453437e913cdd2a2623efd2_Out_2;
            Unity_Subtract_float(_Multiply_abe194398c584d8a8934b67cf64c0da1_Out_2, _Multiply_7ba2c47eef0f4b89b3b4eb4a8ed31f83_Out_2, _Subtract_dc119b36f453437e913cdd2a2623efd2_Out_2);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Property_ab48707fe6db419eb51ece316bb6313b_Out_0 = Vector1_cc6e37850778480781084b62719d58c3;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Multiply_3a48e77d57694a868985ec140162c2ef_Out_2;
            Unity_Multiply_float(_Subtract_dc119b36f453437e913cdd2a2623efd2_Out_2, _Property_ab48707fe6db419eb51ece316bb6313b_Out_0, _Multiply_3a48e77d57694a868985ec140162c2ef_Out_2);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Clamp_bb886dbf030149bd8ffa4e979d6e9c46_Out_3;
            Unity_Clamp_float(_Multiply_3a48e77d57694a868985ec140162c2ef_Out_2, 0, 1, _Clamp_bb886dbf030149bd8ffa4e979d6e9c46_Out_3);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Power_795157e2d171465e908da5d45e938d28_Out_2;
            Unity_Power_float(_Clamp_bb886dbf030149bd8ffa4e979d6e9c46_Out_3, 0.7, _Power_795157e2d171465e908da5d45e938d28_Out_2);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Property_41daac0ae2c74098ac9111219e1e3b3b_Out_0 = Vector1_5be3734798f64a63b2827794b79391c3;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _FresnelEffect_98bea57f43804c61a33cabcb5a3e5400_Out_3;
            Unity_FresnelEffect_float(IN.WorldSpaceNormal, IN.WorldSpaceViewDirection, _Property_41daac0ae2c74098ac9111219e1e3b3b_Out_0, _FresnelEffect_98bea57f43804c61a33cabcb5a3e5400_Out_3);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Maximum_85ae2b8b22d5495fb3e04a01fa179389_Out_2;
            Unity_Maximum_float(_FresnelEffect_98bea57f43804c61a33cabcb5a3e5400_Out_3, 0.7, _Maximum_85ae2b8b22d5495fb3e04a01fa179389_Out_2);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Multiply_465f2016ad9d499981e3a39ae56b8aef_Out_2;
            Unity_Multiply_float(_Power_795157e2d171465e908da5d45e938d28_Out_2, _Maximum_85ae2b8b22d5495fb3e04a01fa179389_Out_2, _Multiply_465f2016ad9d499981e3a39ae56b8aef_Out_2);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Saturate_00659ca349c74eea975fbb11da46e6bf_Out_1;
            Unity_Saturate_float(_Multiply_465f2016ad9d499981e3a39ae56b8aef_Out_2, _Saturate_00659ca349c74eea975fbb11da46e6bf_Out_1);
            #endif
            surface.Alpha = _Saturate_00659ca349c74eea975fbb11da46e6bf_Out_1;
            return surface;
        }

            // --------------------------------------------------
            // Build Graph Inputs

            VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
        {
            VertexDescriptionInputs output;
            ZERO_INITIALIZE(VertexDescriptionInputs, output);

        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        output.ObjectSpaceNormal =           input.normalOS;
        #endif

        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        output.WorldSpaceNormal =            TransformObjectToWorldNormal(input.normalOS);
        #endif

        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        output.ObjectSpaceTangent =          input.tangentOS.xyz;
        #endif

        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        output.WorldSpaceTangent =           TransformObjectToWorldDir(input.tangentOS.xyz);
        #endif

        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        output.ObjectSpaceBiTangent =        normalize(cross(input.normalOS, input.tangentOS) * (input.tangentOS.w > 0.0f ? 1.0f : -1.0f) * GetOddNegativeScale());
        #endif

        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        output.WorldSpaceBiTangent =         TransformObjectToWorldDir(output.ObjectSpaceBiTangent);
        #endif

        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        output.ObjectSpacePosition =         input.positionOS;
        #endif

        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        output.AbsoluteWorldSpacePosition =  GetAbsolutePositionWS(TransformObjectToWorld(input.positionOS));
        #endif


            return output;
        }
            SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);

        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        // must use interpolated tangent, bitangent and normal before they are normalized in the pixel shader.
        #endif

        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        float3 unnormalizedNormalWS = input.normalWS;
        #endif

        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        const float renormFactor = 1.0 / length(unnormalizedNormalWS);
        #endif



        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        output.WorldSpaceNormal =            renormFactor*input.normalWS.xyz;		// we want a unit length Normal Vector node in shader graph
        #endif



        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        output.WorldSpaceViewDirection =     input.viewDirectionWS; //TODO: by default normalized in HD, but not in universal
        #endif

        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        output.WorldSpacePosition =          input.positionWS;
        #endif

        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        output.ScreenPosition =              ComputeScreenPos(TransformWorldToHClip(input.positionWS), _ProjectionParams.x);
        #endif

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
            #pragma multi_compile_local _ CEL_SPECULAR_ON

        #if defined(CEL_SPECULAR_ON)
            #define KEYWORD_PERMUTATION_0
        #else
            #define KEYWORD_PERMUTATION_1
        #endif


            // Defines
        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        #define _SURFACE_TYPE_TRANSPARENT 1
        #endif

        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        #define ATTRIBUTES_NEED_NORMAL
        #endif

        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        #define ATTRIBUTES_NEED_TANGENT
        #endif

        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        #define VARYINGS_NEED_POSITION_WS
        #endif

        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        #define VARYINGS_NEED_NORMAL_WS
        #endif

        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        #define VARYINGS_NEED_VIEWDIRECTION_WS
        #endif

            #define FEATURES_GRAPH_VERTEX
            /* WARNING: $splice Could not find named fragment 'PassInstancing' */
            #define SHADERPASS SHADERPASS_DEPTHONLY
        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        #define REQUIRE_DEPTH_TEXTURE
        #endif
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
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float3 positionOS : POSITION;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float3 normalOS : NORMAL;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float4 tangentOS : TANGENT;
            #endif
            #if UNITY_ANY_INSTANCING_ENABLED
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            uint instanceID : INSTANCEID_SEMANTIC;
            #endif
            #endif
        };
        struct Varyings
        {
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float4 positionCS : SV_POSITION;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float3 positionWS;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float3 normalWS;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float3 viewDirectionWS;
            #endif
            #if UNITY_ANY_INSTANCING_ENABLED
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
            #endif
        };
        struct SurfaceDescriptionInputs
        {
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float3 WorldSpaceNormal;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float3 WorldSpaceViewDirection;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float3 WorldSpacePosition;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float4 ScreenPosition;
            #endif
        };
        struct VertexDescriptionInputs
        {
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float3 ObjectSpaceNormal;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float3 WorldSpaceNormal;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float3 ObjectSpaceTangent;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float3 WorldSpaceTangent;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float3 ObjectSpaceBiTangent;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float3 WorldSpaceBiTangent;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float3 ObjectSpacePosition;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float3 AbsoluteWorldSpacePosition;
            #endif
        };
        struct PackedVaryings
        {
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float4 positionCS : SV_POSITION;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float3 interp0 : TEXCOORD0;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float3 interp1 : TEXCOORD1;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float3 interp2 : TEXCOORD2;
            #endif
            #if UNITY_ANY_INSTANCING_ENABLED
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
            #endif
        };

            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        PackedVaryings PackVaryings (Varyings input)
        {
            PackedVaryings output;
            output.positionCS = input.positionCS;
            output.interp0.xyz =  input.positionWS;
            output.interp1.xyz =  input.normalWS;
            output.interp2.xyz =  input.viewDirectionWS;
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
            output.viewDirectionWS = input.interp2.xyz;
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
        #endif

            // --------------------------------------------------
            // Graph

            // Graph Properties
            CBUFFER_START(UnityPerMaterial)
        float4 _CelShading_ad7871fb5bf1404e9f1fa53eab938a3a_Texture2D9666dd2f2f8f4fddb6c2a5c61ddfa9bd_3072536305_TexelSize;
        float Vector1_0e212c6562134ef7add6f6766d6419b4;
        float4 Color_213be8c364f242cdbabe6ef25c0efbd9;
        float4 Color_8b23519dc989478fa19369d8efda52f6;
        float Vector1_40661633426347afb1a705a362429b36;
        float4 Texture2D_c773e99a7970488a9d6bcbd08807a081_TexelSize;
        float Vector1_45a27133fb8b4113a0746c0d1954c57b;
        float Vector1_f133b62c64f14525895d4b7d5afdd79f;
        float Vector1_57cb41fdb69545e8b53a796761acb08d;
        float Vector1_76a8f3bdc13d493dbc42eca7e6de6c4e;
        float4 Texture2D_c23a14955d09487682029de7801dfc17_TexelSize;
        float Vector1_28676355978446f285fbfe6f1a574316;
        float Vector1_ba885a3188cc490b9d04d880f79cd1b0;
        float Vector1_bf79c34252fe4364976a552df388436c;
        float Vector1_f586ab191e3e497491345288306a18c0;
        float Vector1_4da6e98cbfa5408fb6352b485aa76d05;
        float Vector1_b266d7e71de2489ea084cb27060cacca;
        float Vector1_d09cc377b0524610970658ffdacd497f;
        float Vector1_1f93deba5516424f927cee1eb2c93ba3;
        float Vector1_cc6e37850778480781084b62719d58c3;
        float Vector1_5be3734798f64a63b2827794b79391c3;
        float Vector1_3578eca6d8ac4314939bfb16bfdbde27;
        float Vector1_775ef292c0cf4cda91aecf4845cd67bd;
        CBUFFER_END

        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(_CelShading_ad7871fb5bf1404e9f1fa53eab938a3a_Texture2D9666dd2f2f8f4fddb6c2a5c61ddfa9bd_3072536305);
        SAMPLER(sampler_CelShading_ad7871fb5bf1404e9f1fa53eab938a3a_Texture2D9666dd2f2f8f4fddb6c2a5c61ddfa9bd_3072536305);
        TEXTURE2D(Texture2D_c773e99a7970488a9d6bcbd08807a081);
        SAMPLER(samplerTexture2D_c773e99a7970488a9d6bcbd08807a081);
        TEXTURE2D(Texture2D_c23a14955d09487682029de7801dfc17);
        SAMPLER(samplerTexture2D_c23a14955d09487682029de7801dfc17);

            // Graph Functions
            
        // c9d40f89572cca24e7b23cdf7d4f0592
        #include "Assets/Rendering/Shaders/Functions/URPFonctions.hlsl"

        void Unity_Multiply_float(float A, float B, out float Out)
        {
            Out = A * B;
        }

        void Unity_Add_float3(float3 A, float3 B, out float3 Out)
        {
            Out = A + B;
        }

        struct Bindings_WaveDisplacement_81e878f76fb7ea94995742377f80ac36
        {
        };

        void SG_WaveDisplacement_81e878f76fb7ea94995742377f80ac36(float3 Vector3_774baa01c42a49f6abc99dad6b54a583, float Vector1_b53e914fc7984637b80324d57e274b5f, float Vector1_af1a9e8da22b4e08851f991318dec789, float Vector1_e937c75465ce4f99ba008e1a397c452b, Bindings_WaveDisplacement_81e878f76fb7ea94995742377f80ac36 IN, out float3 OutVector3_2)
        {
            float3 _Property_4f1419a54dd7457eb25ffd09e1294716_Out_0 = Vector3_774baa01c42a49f6abc99dad6b54a583;
            float _Property_61f840140291436e8912958fef5953c3_Out_0 = Vector1_b53e914fc7984637b80324d57e274b5f;
            float _Property_aa3d6115a8f7408182a5ad6eaf4dea9c_Out_0 = Vector1_af1a9e8da22b4e08851f991318dec789;
            float _Property_318bfb7c8fcb48359c265b1829bb9333_Out_0 = Vector1_e937c75465ce4f99ba008e1a397c452b;
            float _CalculateWaveCustomFunction_babfa0b8a0ce4e20b113bd73598d4cd6_New_0;
            CalculateWave_float(_Property_4f1419a54dd7457eb25ffd09e1294716_Out_0, _Property_61f840140291436e8912958fef5953c3_Out_0, _Property_aa3d6115a8f7408182a5ad6eaf4dea9c_Out_0, _Property_318bfb7c8fcb48359c265b1829bb9333_Out_0, _CalculateWaveCustomFunction_babfa0b8a0ce4e20b113bd73598d4cd6_New_0);
            float3 _Vector3_fd59b684dbbf45499ba224d658a9f2ec_Out_0 = float3(0, _CalculateWaveCustomFunction_babfa0b8a0ce4e20b113bd73598d4cd6_New_0, 0);
            float3 _Add_bb0b83c602bf41909e47fdeba507406b_Out_2;
            Unity_Add_float3(_Vector3_fd59b684dbbf45499ba224d658a9f2ec_Out_0, _Property_4f1419a54dd7457eb25ffd09e1294716_Out_0, _Add_bb0b83c602bf41909e47fdeba507406b_Out_2);
            OutVector3_2 = _Add_bb0b83c602bf41909e47fdeba507406b_Out_2;
        }

        void Unity_Sign_float(float In, out float Out)
        {
            Out = sign(In);
        }

        void Unity_Multiply_float(float3 A, float3 B, out float3 Out)
        {
            Out = A * B;
        }

        void Unity_Add_float(float A, float B, out float Out)
        {
            Out = A + B;
        }

        void Unity_Subtract_float(float A, float B, out float Out)
        {
            Out = A - B;
        }

        void Unity_Absolute_float(float In, out float Out)
        {
            Out = abs(In);
        }

        void Unity_Saturate_float(float In, out float Out)
        {
            Out = saturate(In);
        }

        void Unity_Divide_float(float A, float B, out float Out)
        {
            Out = A / B;
        }

        void Unity_Combine_float(float R, float G, float B, float A, out float4 RGBA, out float3 RGB, out float2 RG)
        {
            RGBA = float4(R, G, B, A);
            RGB = float3(R, G, B);
            RG = float2(R, G);
        }

        void Unity_Smoothstep_float(float Edge1, float Edge2, float In, out float Out)
        {
            Out = smoothstep(Edge1, Edge2, In);
        }

        void Unity_Lerp_float3(float3 A, float3 B, float3 T, out float3 Out)
        {
            Out = lerp(A, B, T);
        }

        void Unity_Subtract_float3(float3 A, float3 B, out float3 Out)
        {
            Out = A - B;
        }

        void Unity_Normalize_float3(float3 In, out float3 Out)
        {
            Out = normalize(In);
        }

        void Unity_CrossProduct_float(float3 A, float3 B, out float3 Out)
        {
            Out = cross(A, B);
        }

        struct Bindings_Neighbors_df68ff96a41468e448052764fa9bdba1
        {
            float3 ObjectSpaceNormal;
        };

        void SG_Neighbors_df68ff96a41468e448052764fa9bdba1(float3 Vector3_8954E4F1, float Vector1_B5D26639, Bindings_Neighbors_df68ff96a41468e448052764fa9bdba1 IN, out float3 Neighbor1_1, out float3 Neighbor2_2)
        {
            float _Split_609f2b09351ec288b5ed925103f0f9e0_R_1 = IN.ObjectSpaceNormal[0];
            float _Split_609f2b09351ec288b5ed925103f0f9e0_G_2 = IN.ObjectSpaceNormal[1];
            float _Split_609f2b09351ec288b5ed925103f0f9e0_B_3 = IN.ObjectSpaceNormal[2];
            float _Split_609f2b09351ec288b5ed925103f0f9e0_A_4 = 0;
            float _Sign_e3911d5b4df6208882d5722613bfa7df_Out_1;
            Unity_Sign_float(_Split_609f2b09351ec288b5ed925103f0f9e0_R_1, _Sign_e3911d5b4df6208882d5722613bfa7df_Out_1);
            float3 _Property_8a0179cbbf558586ab0af83d3af77169_Out_0 = Vector3_8954E4F1;
            float3 _Multiply_a5a5ef928369cd84997685e6caac87b0_Out_2;
            Unity_Multiply_float(_Property_8a0179cbbf558586ab0af83d3af77169_Out_0, IN.ObjectSpaceNormal, _Multiply_a5a5ef928369cd84997685e6caac87b0_Out_2);
            float _Split_8977fb298fca9e8182c1edaad752997c_R_1 = _Multiply_a5a5ef928369cd84997685e6caac87b0_Out_2[0];
            float _Split_8977fb298fca9e8182c1edaad752997c_G_2 = _Multiply_a5a5ef928369cd84997685e6caac87b0_Out_2[1];
            float _Split_8977fb298fca9e8182c1edaad752997c_B_3 = _Multiply_a5a5ef928369cd84997685e6caac87b0_Out_2[2];
            float _Split_8977fb298fca9e8182c1edaad752997c_A_4 = 0;
            float _Add_eeda377c8b7c038596bc86d83f7f321e_Out_2;
            Unity_Add_float(_Split_8977fb298fca9e8182c1edaad752997c_R_1, _Split_8977fb298fca9e8182c1edaad752997c_G_2, _Add_eeda377c8b7c038596bc86d83f7f321e_Out_2);
            float _Add_61a089896fe3468b82b211a762b6ec21_Out_2;
            Unity_Add_float(_Add_eeda377c8b7c038596bc86d83f7f321e_Out_2, _Split_8977fb298fca9e8182c1edaad752997c_B_3, _Add_61a089896fe3468b82b211a762b6ec21_Out_2);
            float _Split_d79bc2f0568d4b8586a29273c4b01ac3_R_1 = IN.ObjectSpaceNormal[0];
            float _Split_d79bc2f0568d4b8586a29273c4b01ac3_G_2 = IN.ObjectSpaceNormal[1];
            float _Split_d79bc2f0568d4b8586a29273c4b01ac3_B_3 = IN.ObjectSpaceNormal[2];
            float _Split_d79bc2f0568d4b8586a29273c4b01ac3_A_4 = 0;
            float _Float_9d499dd7c2614a8589e8a1cb04daeb5d_Out_0 = 0.05;
            float _Split_802c7c2944faa986ba720f07d8db74ab_R_1 = _Property_8a0179cbbf558586ab0af83d3af77169_Out_0[0];
            float _Split_802c7c2944faa986ba720f07d8db74ab_G_2 = _Property_8a0179cbbf558586ab0af83d3af77169_Out_0[1];
            float _Split_802c7c2944faa986ba720f07d8db74ab_B_3 = _Property_8a0179cbbf558586ab0af83d3af77169_Out_0[2];
            float _Split_802c7c2944faa986ba720f07d8db74ab_A_4 = 0;
            float _Add_4555b76b861a778d97276d765af07483_Out_2;
            Unity_Add_float(_Float_9d499dd7c2614a8589e8a1cb04daeb5d_Out_0, _Split_802c7c2944faa986ba720f07d8db74ab_G_2, _Add_4555b76b861a778d97276d765af07483_Out_2);
            float _Multiply_1e591f42bea8b58c8797ab2c90faa345_Out_2;
            Unity_Multiply_float(_Split_d79bc2f0568d4b8586a29273c4b01ac3_G_2, _Add_4555b76b861a778d97276d765af07483_Out_2, _Multiply_1e591f42bea8b58c8797ab2c90faa345_Out_2);
            float _Subtract_68ba98ef07340b83b46992e63c68d7ca_Out_2;
            Unity_Subtract_float(_Add_61a089896fe3468b82b211a762b6ec21_Out_2, _Multiply_1e591f42bea8b58c8797ab2c90faa345_Out_2, _Subtract_68ba98ef07340b83b46992e63c68d7ca_Out_2);
            float _Multiply_d0572ed2941ee0879549c1da1c572b15_Out_2;
            Unity_Multiply_float(_Split_d79bc2f0568d4b8586a29273c4b01ac3_B_3, _Split_802c7c2944faa986ba720f07d8db74ab_B_3, _Multiply_d0572ed2941ee0879549c1da1c572b15_Out_2);
            float _Subtract_3d9ad256eba2ee8e839158fc0f45ee81_Out_2;
            Unity_Subtract_float(_Subtract_68ba98ef07340b83b46992e63c68d7ca_Out_2, _Multiply_d0572ed2941ee0879549c1da1c572b15_Out_2, _Subtract_3d9ad256eba2ee8e839158fc0f45ee81_Out_2);
            float _Float_654fffd8f366ff808a39bb5a6a1f1fdb_Out_0 = 0.01;
            float _Absolute_9fde5c96b588aa8fbed6af91404da630_Out_1;
            Unity_Absolute_float(_Split_609f2b09351ec288b5ed925103f0f9e0_R_1, _Absolute_9fde5c96b588aa8fbed6af91404da630_Out_1);
            float _Subtract_7173428a9289a58abbac98045c08ceb9_Out_2;
            Unity_Subtract_float(_Absolute_9fde5c96b588aa8fbed6af91404da630_Out_1, _Float_654fffd8f366ff808a39bb5a6a1f1fdb_Out_0, _Subtract_7173428a9289a58abbac98045c08ceb9_Out_2);
            float _Saturate_e8fcca777a01738ab601adda864265d1_Out_1;
            Unity_Saturate_float(_Subtract_7173428a9289a58abbac98045c08ceb9_Out_2, _Saturate_e8fcca777a01738ab601adda864265d1_Out_1);
            float _Add_71e184a7749fc9848483bc81af14e30e_Out_2;
            Unity_Add_float(_Float_654fffd8f366ff808a39bb5a6a1f1fdb_Out_0, _Saturate_e8fcca777a01738ab601adda864265d1_Out_1, _Add_71e184a7749fc9848483bc81af14e30e_Out_2);
            float _Divide_7cc37db0bc8630899cffaa8d565d6d55_Out_2;
            Unity_Divide_float(_Subtract_3d9ad256eba2ee8e839158fc0f45ee81_Out_2, _Add_71e184a7749fc9848483bc81af14e30e_Out_2, _Divide_7cc37db0bc8630899cffaa8d565d6d55_Out_2);
            float _Multiply_9c6b2289e8d2db84a1c0d5ca2a655b93_Out_2;
            Unity_Multiply_float(_Sign_e3911d5b4df6208882d5722613bfa7df_Out_1, _Divide_7cc37db0bc8630899cffaa8d565d6d55_Out_2, _Multiply_9c6b2289e8d2db84a1c0d5ca2a655b93_Out_2);
            float4 _Combine_8fcb17287675378fb024a84210c6677a_RGBA_4;
            float3 _Combine_8fcb17287675378fb024a84210c6677a_RGB_5;
            float2 _Combine_8fcb17287675378fb024a84210c6677a_RG_6;
            Unity_Combine_float(_Multiply_9c6b2289e8d2db84a1c0d5ca2a655b93_Out_2, _Add_4555b76b861a778d97276d765af07483_Out_2, _Split_802c7c2944faa986ba720f07d8db74ab_B_3, 0, _Combine_8fcb17287675378fb024a84210c6677a_RGBA_4, _Combine_8fcb17287675378fb024a84210c6677a_RGB_5, _Combine_8fcb17287675378fb024a84210c6677a_RG_6);
            float _Float_9b0eee4975b51e828a4269a5edcee5b1_Out_0 = 0.05;
            float _Split_f205281c3443d68aa9b0c1a2feace9df_R_1 = _Property_8a0179cbbf558586ab0af83d3af77169_Out_0[0];
            float _Split_f205281c3443d68aa9b0c1a2feace9df_G_2 = _Property_8a0179cbbf558586ab0af83d3af77169_Out_0[1];
            float _Split_f205281c3443d68aa9b0c1a2feace9df_B_3 = _Property_8a0179cbbf558586ab0af83d3af77169_Out_0[2];
            float _Split_f205281c3443d68aa9b0c1a2feace9df_A_4 = 0;
            float _Add_db37e0cfea249d8d9ede45e675501301_Out_2;
            Unity_Add_float(_Float_9b0eee4975b51e828a4269a5edcee5b1_Out_0, _Split_f205281c3443d68aa9b0c1a2feace9df_R_1, _Add_db37e0cfea249d8d9ede45e675501301_Out_2);
            float4 _Combine_9530e51a49a77f849e6eac5cb0ce15ad_RGBA_4;
            float3 _Combine_9530e51a49a77f849e6eac5cb0ce15ad_RGB_5;
            float2 _Combine_9530e51a49a77f849e6eac5cb0ce15ad_RG_6;
            Unity_Combine_float(_Add_db37e0cfea249d8d9ede45e675501301_Out_2, _Split_f205281c3443d68aa9b0c1a2feace9df_G_2, _Split_f205281c3443d68aa9b0c1a2feace9df_B_3, 0, _Combine_9530e51a49a77f849e6eac5cb0ce15ad_RGBA_4, _Combine_9530e51a49a77f849e6eac5cb0ce15ad_RGB_5, _Combine_9530e51a49a77f849e6eac5cb0ce15ad_RG_6);
            float _Multiply_625b1e551cdfd0829aa19290646e6739_Out_2;
            Unity_Multiply_float(-1, _Subtract_7173428a9289a58abbac98045c08ceb9_Out_2, _Multiply_625b1e551cdfd0829aa19290646e6739_Out_2);
            float _Smoothstep_b60ff4818667938f9be3fa9ecf03f163_Out_3;
            Unity_Smoothstep_float(0, 0.01, _Multiply_625b1e551cdfd0829aa19290646e6739_Out_2, _Smoothstep_b60ff4818667938f9be3fa9ecf03f163_Out_3);
            float3 _Lerp_7d115f30f5f0fc8cb46b9ba33440e47a_Out_3;
            Unity_Lerp_float3(_Combine_8fcb17287675378fb024a84210c6677a_RGB_5, _Combine_9530e51a49a77f849e6eac5cb0ce15ad_RGB_5, (_Smoothstep_b60ff4818667938f9be3fa9ecf03f163_Out_3.xxx), _Lerp_7d115f30f5f0fc8cb46b9ba33440e47a_Out_3);
            float3 _Subtract_24a46f67e2876e80b3b01e0b77a50456_Out_2;
            Unity_Subtract_float3(_Lerp_7d115f30f5f0fc8cb46b9ba33440e47a_Out_3, _Property_8a0179cbbf558586ab0af83d3af77169_Out_0, _Subtract_24a46f67e2876e80b3b01e0b77a50456_Out_2);
            float3 _Normalize_f544245d7bd37f8fbaee4a87c2403675_Out_1;
            Unity_Normalize_float3(_Subtract_24a46f67e2876e80b3b01e0b77a50456_Out_2, _Normalize_f544245d7bd37f8fbaee4a87c2403675_Out_1);
            float3 _Normalize_9a359141b951ea84b3220ad5c3123b7c_Out_1;
            Unity_Normalize_float3(IN.ObjectSpaceNormal, _Normalize_9a359141b951ea84b3220ad5c3123b7c_Out_1);
            float3 _CrossProduct_1dbde50ff8c546829c8b59ec1630c252_Out_2;
            Unity_CrossProduct_float(_Normalize_f544245d7bd37f8fbaee4a87c2403675_Out_1, _Normalize_9a359141b951ea84b3220ad5c3123b7c_Out_1, _CrossProduct_1dbde50ff8c546829c8b59ec1630c252_Out_2);
            float3 _Normalize_3eb509f6a593a18d856dabb44191008a_Out_1;
            Unity_Normalize_float3(_CrossProduct_1dbde50ff8c546829c8b59ec1630c252_Out_2, _Normalize_3eb509f6a593a18d856dabb44191008a_Out_1);
            float _Property_4448eb2908e4228e840b80f046abf12e_Out_0 = Vector1_B5D26639;
            float3 _Multiply_37bf5602003a2885bb744b13f93296fe_Out_2;
            Unity_Multiply_float(_Normalize_3eb509f6a593a18d856dabb44191008a_Out_1, (_Property_4448eb2908e4228e840b80f046abf12e_Out_0.xxx), _Multiply_37bf5602003a2885bb744b13f93296fe_Out_2);
            float3 _Add_bd2759426a510983979415cc0e46c2d4_Out_2;
            Unity_Add_float3(_Multiply_37bf5602003a2885bb744b13f93296fe_Out_2, _Property_8a0179cbbf558586ab0af83d3af77169_Out_0, _Add_bd2759426a510983979415cc0e46c2d4_Out_2);
            float3 _Multiply_273e0ecf939b858582031df2660ec861_Out_2;
            Unity_Multiply_float(_Normalize_f544245d7bd37f8fbaee4a87c2403675_Out_1, (_Property_4448eb2908e4228e840b80f046abf12e_Out_0.xxx), _Multiply_273e0ecf939b858582031df2660ec861_Out_2);
            float3 _Add_2259c39580c12b8297f6b81a426bd3c0_Out_2;
            Unity_Add_float3(_Property_8a0179cbbf558586ab0af83d3af77169_Out_0, _Multiply_273e0ecf939b858582031df2660ec861_Out_2, _Add_2259c39580c12b8297f6b81a426bd3c0_Out_2);
            Neighbor1_1 = _Add_bd2759426a510983979415cc0e46c2d4_Out_2;
            Neighbor2_2 = _Add_2259c39580c12b8297f6b81a426bd3c0_Out_2;
        }

        struct Bindings_NewNormal_c8d6b0224f233004190264cd9e5ca192
        {
        };

        void SG_NewNormal_c8d6b0224f233004190264cd9e5ca192(float3 Vector3_988291EB, float3 Vector3_EB4E2DE7, float3 Vector3_1973C67A, Bindings_NewNormal_c8d6b0224f233004190264cd9e5ca192 IN, out float3 OutVector3_1)
        {
            float3 _Property_fd61227e10c3348bb61b5f1d39e68328_Out_0 = Vector3_EB4E2DE7;
            float3 _Property_b8cacddd74e568809402e877976dffa8_Out_0 = Vector3_988291EB;
            float3 _Subtract_1f3c648b5559d28f9acfbe1af362b37b_Out_2;
            Unity_Subtract_float3(_Property_fd61227e10c3348bb61b5f1d39e68328_Out_0, _Property_b8cacddd74e568809402e877976dffa8_Out_0, _Subtract_1f3c648b5559d28f9acfbe1af362b37b_Out_2);
            float3 _Normalize_888167f431fbc788b4ad8e2092e7f28b_Out_1;
            Unity_Normalize_float3(_Subtract_1f3c648b5559d28f9acfbe1af362b37b_Out_2, _Normalize_888167f431fbc788b4ad8e2092e7f28b_Out_1);
            float3 _Property_6008e654ce04488cbe1d5caac8afe08a_Out_0 = Vector3_1973C67A;
            float3 _Subtract_208696e2abb5f18d84a5878fd9a81553_Out_2;
            Unity_Subtract_float3(_Property_6008e654ce04488cbe1d5caac8afe08a_Out_0, _Property_b8cacddd74e568809402e877976dffa8_Out_0, _Subtract_208696e2abb5f18d84a5878fd9a81553_Out_2);
            float3 _Normalize_7250d413c7450d8aaa805c923843f2dd_Out_1;
            Unity_Normalize_float3(_Subtract_208696e2abb5f18d84a5878fd9a81553_Out_2, _Normalize_7250d413c7450d8aaa805c923843f2dd_Out_1);
            float3 _CrossProduct_5566a5ab348aa8809609d834bbe4451f_Out_2;
            Unity_CrossProduct_float(_Normalize_888167f431fbc788b4ad8e2092e7f28b_Out_1, _Normalize_7250d413c7450d8aaa805c923843f2dd_Out_1, _CrossProduct_5566a5ab348aa8809609d834bbe4451f_Out_2);
            float3 _Normalize_a640075232b8c78498848f10ca1e7ba8_Out_1;
            Unity_Normalize_float3(_CrossProduct_5566a5ab348aa8809609d834bbe4451f_Out_2, _Normalize_a640075232b8c78498848f10ca1e7ba8_Out_1);
            OutVector3_1 = _Normalize_a640075232b8c78498848f10ca1e7ba8_Out_1;
        }

        void Unity_SceneDepth_Linear01_float(float4 UV, out float Out)
        {
            Out = Linear01Depth(SHADERGRAPH_SAMPLE_SCENE_DEPTH(UV.xy), _ZBufferParams);
        }

        void Unity_Clamp_float(float In, float Min, float Max, out float Out)
        {
            Out = clamp(In, Min, Max);
        }

        void Unity_Power_float(float A, float B, out float Out)
        {
            Out = pow(A, B);
        }

        void Unity_FresnelEffect_float(float3 Normal, float3 ViewDir, float Power, out float Out)
        {
            Out = pow((1.0 - saturate(dot(normalize(Normal), normalize(ViewDir)))), Power);
        }

        void Unity_Maximum_float(float A, float B, out float Out)
        {
            Out = max(A, B);
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
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            UnityTexture2D _Property_52e3e5c2509c4df984c47cc092c466d8_Out_0 = UnityBuildTexture2DStructNoScale(Texture2D_c773e99a7970488a9d6bcbd08807a081);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Property_736f2734d3e643c398a2586801e4cc17_Out_0 = Vector1_f133b62c64f14525895d4b7d5afdd79f;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Property_7851f92dfd4d4302a7beb8a17dd4bd8b_Out_0 = Vector1_45a27133fb8b4113a0746c0d1954c57b;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float2 _CalculateNoiseCustomFunction_a0c2603a229c4d299ac32a59a8664765_Out_0;
            CalculateNoise_float(IN.AbsoluteWorldSpacePosition, _Property_736f2734d3e643c398a2586801e4cc17_Out_0, _Property_7851f92dfd4d4302a7beb8a17dd4bd8b_Out_0, _CalculateNoiseCustomFunction_a0c2603a229c4d299ac32a59a8664765_Out_0);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            #if defined(SHADER_API_GLES) && (SHADER_TARGET < 30)
              float4 _SampleTexture2DLOD_fe1fede5211b477cbca93fc4c2f76d12_RGBA_0 = float4(0.0f, 0.0f, 0.0f, 1.0f);
            #else
              float4 _SampleTexture2DLOD_fe1fede5211b477cbca93fc4c2f76d12_RGBA_0 = SAMPLE_TEXTURE2D_LOD(_Property_52e3e5c2509c4df984c47cc092c466d8_Out_0.tex, _Property_52e3e5c2509c4df984c47cc092c466d8_Out_0.samplerstate, _CalculateNoiseCustomFunction_a0c2603a229c4d299ac32a59a8664765_Out_0, 0);
            #endif
            float _SampleTexture2DLOD_fe1fede5211b477cbca93fc4c2f76d12_R_5 = _SampleTexture2DLOD_fe1fede5211b477cbca93fc4c2f76d12_RGBA_0.r;
            float _SampleTexture2DLOD_fe1fede5211b477cbca93fc4c2f76d12_G_6 = _SampleTexture2DLOD_fe1fede5211b477cbca93fc4c2f76d12_RGBA_0.g;
            float _SampleTexture2DLOD_fe1fede5211b477cbca93fc4c2f76d12_B_7 = _SampleTexture2DLOD_fe1fede5211b477cbca93fc4c2f76d12_RGBA_0.b;
            float _SampleTexture2DLOD_fe1fede5211b477cbca93fc4c2f76d12_A_8 = _SampleTexture2DLOD_fe1fede5211b477cbca93fc4c2f76d12_RGBA_0.a;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Property_fab5f23c0a9e449bb605bc77e42f5c55_Out_0 = Vector1_57cb41fdb69545e8b53a796761acb08d;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Multiply_be99c4e752404fa4b5a42166d27a7cc0_Out_2;
            Unity_Multiply_float(_SampleTexture2DLOD_fe1fede5211b477cbca93fc4c2f76d12_R_5, _Property_fab5f23c0a9e449bb605bc77e42f5c55_Out_0, _Multiply_be99c4e752404fa4b5a42166d27a7cc0_Out_2);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float3 _Vector3_9e43fda8531d474c890ea7595c453773_Out_0 = float3(0, _Multiply_be99c4e752404fa4b5a42166d27a7cc0_Out_2, 0);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Property_0ea561f00aaf4fbb8fdfeab5a2a0dd08_Out_0 = Vector1_4da6e98cbfa5408fb6352b485aa76d05;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Property_e4937aaae9bc4c098cebea935a7ee1ea_Out_0 = Vector1_b266d7e71de2489ea084cb27060cacca;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Property_2d3470f269ac4477a8dc467d895fe472_Out_0 = Vector1_d09cc377b0524610970658ffdacd497f;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            Bindings_WaveDisplacement_81e878f76fb7ea94995742377f80ac36 _WaveDisplacement_5f54fff0c6334b26bedfff0b63df12b3;
            float3 _WaveDisplacement_5f54fff0c6334b26bedfff0b63df12b3_OutVector3_2;
            SG_WaveDisplacement_81e878f76fb7ea94995742377f80ac36(IN.AbsoluteWorldSpacePosition, _Property_0ea561f00aaf4fbb8fdfeab5a2a0dd08_Out_0, _Property_e4937aaae9bc4c098cebea935a7ee1ea_Out_0, _Property_2d3470f269ac4477a8dc467d895fe472_Out_0, _WaveDisplacement_5f54fff0c6334b26bedfff0b63df12b3, _WaveDisplacement_5f54fff0c6334b26bedfff0b63df12b3_OutVector3_2);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float3 _Add_942b46ad98eb4925b4bfa00a0a5781c7_Out_2;
            Unity_Add_float3(_Vector3_9e43fda8531d474c890ea7595c453773_Out_0, _WaveDisplacement_5f54fff0c6334b26bedfff0b63df12b3_OutVector3_2, _Add_942b46ad98eb4925b4bfa00a0a5781c7_Out_2);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float3 _Transform_3fce2cebd3054a72a946f448c89c2e3d_Out_1 = TransformWorldToObject(GetCameraRelativePositionWS(_Add_942b46ad98eb4925b4bfa00a0a5781c7_Out_2.xyz));
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            Bindings_Neighbors_df68ff96a41468e448052764fa9bdba1 _Neighbors_e648c110f4754cefb6491b8c82fe074f;
            _Neighbors_e648c110f4754cefb6491b8c82fe074f.ObjectSpaceNormal = IN.ObjectSpaceNormal;
            float3 _Neighbors_e648c110f4754cefb6491b8c82fe074f_Neighbor1_1;
            float3 _Neighbors_e648c110f4754cefb6491b8c82fe074f_Neighbor2_2;
            SG_Neighbors_df68ff96a41468e448052764fa9bdba1(IN.AbsoluteWorldSpacePosition, 0.01, _Neighbors_e648c110f4754cefb6491b8c82fe074f, _Neighbors_e648c110f4754cefb6491b8c82fe074f_Neighbor1_1, _Neighbors_e648c110f4754cefb6491b8c82fe074f_Neighbor2_2);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            Bindings_WaveDisplacement_81e878f76fb7ea94995742377f80ac36 _WaveDisplacement_2fa36261c2534cbe8639a41fdfe4910a;
            float3 _WaveDisplacement_2fa36261c2534cbe8639a41fdfe4910a_OutVector3_2;
            SG_WaveDisplacement_81e878f76fb7ea94995742377f80ac36(_Neighbors_e648c110f4754cefb6491b8c82fe074f_Neighbor1_1, _Property_0ea561f00aaf4fbb8fdfeab5a2a0dd08_Out_0, _Property_e4937aaae9bc4c098cebea935a7ee1ea_Out_0, _Property_2d3470f269ac4477a8dc467d895fe472_Out_0, _WaveDisplacement_2fa36261c2534cbe8639a41fdfe4910a, _WaveDisplacement_2fa36261c2534cbe8639a41fdfe4910a_OutVector3_2);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            Bindings_WaveDisplacement_81e878f76fb7ea94995742377f80ac36 _WaveDisplacement_c9fd16638edd4ccfaf9e3eb2efa55bfa;
            float3 _WaveDisplacement_c9fd16638edd4ccfaf9e3eb2efa55bfa_OutVector3_2;
            SG_WaveDisplacement_81e878f76fb7ea94995742377f80ac36(_Neighbors_e648c110f4754cefb6491b8c82fe074f_Neighbor2_2, _Property_0ea561f00aaf4fbb8fdfeab5a2a0dd08_Out_0, _Property_e4937aaae9bc4c098cebea935a7ee1ea_Out_0, _Property_2d3470f269ac4477a8dc467d895fe472_Out_0, _WaveDisplacement_c9fd16638edd4ccfaf9e3eb2efa55bfa, _WaveDisplacement_c9fd16638edd4ccfaf9e3eb2efa55bfa_OutVector3_2);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            Bindings_NewNormal_c8d6b0224f233004190264cd9e5ca192 _NewNormal_e9af9405a4a54034863b00f2fbd4c2b6;
            float3 _NewNormal_e9af9405a4a54034863b00f2fbd4c2b6_OutVector3_1;
            SG_NewNormal_c8d6b0224f233004190264cd9e5ca192(_WaveDisplacement_5f54fff0c6334b26bedfff0b63df12b3_OutVector3_2, _WaveDisplacement_2fa36261c2534cbe8639a41fdfe4910a_OutVector3_2, _WaveDisplacement_c9fd16638edd4ccfaf9e3eb2efa55bfa_OutVector3_2, _NewNormal_e9af9405a4a54034863b00f2fbd4c2b6, _NewNormal_e9af9405a4a54034863b00f2fbd4c2b6_OutVector3_1);
            #endif
            description.Position = _Transform_3fce2cebd3054a72a946f448c89c2e3d_Out_1;
            description.Normal = _NewNormal_e9af9405a4a54034863b00f2fbd4c2b6_OutVector3_1;
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
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _SceneDepth_7981f193eac94d87b6f2b2920d4cb528_Out_1;
            Unity_SceneDepth_Linear01_float(float4(IN.ScreenPosition.xy / IN.ScreenPosition.w, 0, 0), _SceneDepth_7981f193eac94d87b6f2b2920d4cb528_Out_1);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Multiply_abe194398c584d8a8934b67cf64c0da1_Out_2;
            Unity_Multiply_float(_SceneDepth_7981f193eac94d87b6f2b2920d4cb528_Out_1, _ProjectionParams.z, _Multiply_abe194398c584d8a8934b67cf64c0da1_Out_2);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float4 _ScreenPosition_68dc8d80a07f4706a6a9a2f0230737ed_Out_0 = IN.ScreenPosition;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Split_7f7e1aadecd54073aff51bd05fa759bc_R_1 = _ScreenPosition_68dc8d80a07f4706a6a9a2f0230737ed_Out_0[0];
            float _Split_7f7e1aadecd54073aff51bd05fa759bc_G_2 = _ScreenPosition_68dc8d80a07f4706a6a9a2f0230737ed_Out_0[1];
            float _Split_7f7e1aadecd54073aff51bd05fa759bc_B_3 = _ScreenPosition_68dc8d80a07f4706a6a9a2f0230737ed_Out_0[2];
            float _Split_7f7e1aadecd54073aff51bd05fa759bc_A_4 = _ScreenPosition_68dc8d80a07f4706a6a9a2f0230737ed_Out_0[3];
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Multiply_7ba2c47eef0f4b89b3b4eb4a8ed31f83_Out_2;
            Unity_Multiply_float(_Split_7f7e1aadecd54073aff51bd05fa759bc_A_4, 1, _Multiply_7ba2c47eef0f4b89b3b4eb4a8ed31f83_Out_2);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Subtract_dc119b36f453437e913cdd2a2623efd2_Out_2;
            Unity_Subtract_float(_Multiply_abe194398c584d8a8934b67cf64c0da1_Out_2, _Multiply_7ba2c47eef0f4b89b3b4eb4a8ed31f83_Out_2, _Subtract_dc119b36f453437e913cdd2a2623efd2_Out_2);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Property_ab48707fe6db419eb51ece316bb6313b_Out_0 = Vector1_cc6e37850778480781084b62719d58c3;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Multiply_3a48e77d57694a868985ec140162c2ef_Out_2;
            Unity_Multiply_float(_Subtract_dc119b36f453437e913cdd2a2623efd2_Out_2, _Property_ab48707fe6db419eb51ece316bb6313b_Out_0, _Multiply_3a48e77d57694a868985ec140162c2ef_Out_2);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Clamp_bb886dbf030149bd8ffa4e979d6e9c46_Out_3;
            Unity_Clamp_float(_Multiply_3a48e77d57694a868985ec140162c2ef_Out_2, 0, 1, _Clamp_bb886dbf030149bd8ffa4e979d6e9c46_Out_3);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Power_795157e2d171465e908da5d45e938d28_Out_2;
            Unity_Power_float(_Clamp_bb886dbf030149bd8ffa4e979d6e9c46_Out_3, 0.7, _Power_795157e2d171465e908da5d45e938d28_Out_2);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Property_41daac0ae2c74098ac9111219e1e3b3b_Out_0 = Vector1_5be3734798f64a63b2827794b79391c3;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _FresnelEffect_98bea57f43804c61a33cabcb5a3e5400_Out_3;
            Unity_FresnelEffect_float(IN.WorldSpaceNormal, IN.WorldSpaceViewDirection, _Property_41daac0ae2c74098ac9111219e1e3b3b_Out_0, _FresnelEffect_98bea57f43804c61a33cabcb5a3e5400_Out_3);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Maximum_85ae2b8b22d5495fb3e04a01fa179389_Out_2;
            Unity_Maximum_float(_FresnelEffect_98bea57f43804c61a33cabcb5a3e5400_Out_3, 0.7, _Maximum_85ae2b8b22d5495fb3e04a01fa179389_Out_2);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Multiply_465f2016ad9d499981e3a39ae56b8aef_Out_2;
            Unity_Multiply_float(_Power_795157e2d171465e908da5d45e938d28_Out_2, _Maximum_85ae2b8b22d5495fb3e04a01fa179389_Out_2, _Multiply_465f2016ad9d499981e3a39ae56b8aef_Out_2);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Saturate_00659ca349c74eea975fbb11da46e6bf_Out_1;
            Unity_Saturate_float(_Multiply_465f2016ad9d499981e3a39ae56b8aef_Out_2, _Saturate_00659ca349c74eea975fbb11da46e6bf_Out_1);
            #endif
            surface.Alpha = _Saturate_00659ca349c74eea975fbb11da46e6bf_Out_1;
            return surface;
        }

            // --------------------------------------------------
            // Build Graph Inputs

            VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
        {
            VertexDescriptionInputs output;
            ZERO_INITIALIZE(VertexDescriptionInputs, output);

        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        output.ObjectSpaceNormal =           input.normalOS;
        #endif

        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        output.WorldSpaceNormal =            TransformObjectToWorldNormal(input.normalOS);
        #endif

        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        output.ObjectSpaceTangent =          input.tangentOS.xyz;
        #endif

        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        output.WorldSpaceTangent =           TransformObjectToWorldDir(input.tangentOS.xyz);
        #endif

        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        output.ObjectSpaceBiTangent =        normalize(cross(input.normalOS, input.tangentOS) * (input.tangentOS.w > 0.0f ? 1.0f : -1.0f) * GetOddNegativeScale());
        #endif

        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        output.WorldSpaceBiTangent =         TransformObjectToWorldDir(output.ObjectSpaceBiTangent);
        #endif

        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        output.ObjectSpacePosition =         input.positionOS;
        #endif

        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        output.AbsoluteWorldSpacePosition =  GetAbsolutePositionWS(TransformObjectToWorld(input.positionOS));
        #endif


            return output;
        }
            SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);

        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        // must use interpolated tangent, bitangent and normal before they are normalized in the pixel shader.
        #endif

        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        float3 unnormalizedNormalWS = input.normalWS;
        #endif

        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        const float renormFactor = 1.0 / length(unnormalizedNormalWS);
        #endif



        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        output.WorldSpaceNormal =            renormFactor*input.normalWS.xyz;		// we want a unit length Normal Vector node in shader graph
        #endif



        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        output.WorldSpaceViewDirection =     input.viewDirectionWS; //TODO: by default normalized in HD, but not in universal
        #endif

        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        output.WorldSpacePosition =          input.positionWS;
        #endif

        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        output.ScreenPosition =              ComputeScreenPos(TransformWorldToHClip(input.positionWS), _ProjectionParams.x);
        #endif

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
    CustomEditor "Needle.MarkdownShaderGUI"
    FallBack "Hidden/Shader Graph/FallbackError"
}