Shader "CelShaded/SnowLayer"
{
    Properties
    {
        [NoScaleOffset]Texture2D_b9baf54b736a49b8931070640840afe0("MainTex", 2D) = "white" {}
        [NoScaleOffset]Texture2D_9689ebada1904c03ba6e4cbb82216610("SnowThickness", 2D) = "black" {}
        Vector1_cf0b9f3c26cf40e09df5caa461ffd8b4("SnowHeight", Range(0, 3)) = 0
        [NoScaleOffset]Texture2D_42a4a1b82a9f4abe96dab75baa1e6491("SnowHeightMap", 2D) = "white" {}
        [NoScaleOffset]Texture2D_748b7f78c75c42a4bdfc5b775def0238("SnowColorMap", 2D) = "white" {}
        [NoScaleOffset]Texture2D_c99ba8a89a0e49c780df6a87923307d3("SnowNormalMap", 2D) = "grey" {}
        [NonModifiableTextureData][NoScaleOffset]_CelShading_abf7811d76a7413ab3f888509118ab1c_Texture2D9666dd2f2f8f4fddb6c2a5c61ddfa9bd_3072536305("Texture2D", 2D) = "white" {}
        [HideInInspector][NoScaleOffset]unity_Lightmaps("unity_Lightmaps", 2DArray) = "" {}
        [HideInInspector][NoScaleOffset]unity_LightmapsInd("unity_LightmapsInd", 2DArray) = "" {}
        [HideInInspector][NoScaleOffset]unity_ShadowMasks("unity_ShadowMasks", 2DArray) = "" {}
    }
    SubShader
    {
        Tags
        {
            "RenderPipeline"="UniversalPipeline"
            "RenderType"="Opaque"
            "UniversalMaterialType" = "Unlit"
            "Queue"="Geometry"
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
        Blend One Zero
        ZTest LEqual
        ZWrite On

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
            float3 AbsoluteWorldSpacePosition;
            float4 uv0;
        };
        struct VertexDescriptionInputs
        {
            float3 ObjectSpaceNormal;
            float3 WorldSpaceNormal;
            float3 ObjectSpaceTangent;
            float3 WorldSpaceTangent;
            float3 ObjectSpaceBiTangent;
            float3 WorldSpaceBiTangent;
            float3 ObjectSpacePosition;
            float3 AbsoluteWorldSpacePosition;
            float4 uv0;
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
        float4 _CelShading_abf7811d76a7413ab3f888509118ab1c_Texture2D9666dd2f2f8f4fddb6c2a5c61ddfa9bd_3072536305_TexelSize;
        float4 Texture2D_b9baf54b736a49b8931070640840afe0_TexelSize;
        float4 Texture2D_9689ebada1904c03ba6e4cbb82216610_TexelSize;
        float Vector1_cf0b9f3c26cf40e09df5caa461ffd8b4;
        float4 Texture2D_42a4a1b82a9f4abe96dab75baa1e6491_TexelSize;
        float4 Texture2D_748b7f78c75c42a4bdfc5b775def0238_TexelSize;
        float4 Texture2D_c99ba8a89a0e49c780df6a87923307d3_TexelSize;
        CBUFFER_END

        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(_CelShading_abf7811d76a7413ab3f888509118ab1c_Texture2D9666dd2f2f8f4fddb6c2a5c61ddfa9bd_3072536305);
        SAMPLER(sampler_CelShading_abf7811d76a7413ab3f888509118ab1c_Texture2D9666dd2f2f8f4fddb6c2a5c61ddfa9bd_3072536305);
        TEXTURE2D(Texture2D_b9baf54b736a49b8931070640840afe0);
        SAMPLER(samplerTexture2D_b9baf54b736a49b8931070640840afe0);
        TEXTURE2D(Texture2D_9689ebada1904c03ba6e4cbb82216610);
        SAMPLER(samplerTexture2D_9689ebada1904c03ba6e4cbb82216610);
        TEXTURE2D(Texture2D_42a4a1b82a9f4abe96dab75baa1e6491);
        SAMPLER(samplerTexture2D_42a4a1b82a9f4abe96dab75baa1e6491);
        TEXTURE2D(Texture2D_748b7f78c75c42a4bdfc5b775def0238);
        SAMPLER(samplerTexture2D_748b7f78c75c42a4bdfc5b775def0238);
        TEXTURE2D(Texture2D_c99ba8a89a0e49c780df6a87923307d3);
        SAMPLER(samplerTexture2D_c99ba8a89a0e49c780df6a87923307d3);

            // Graph Functions
            
        void Unity_OneMinus_float(float In, out float Out)
        {
            Out = 1 - In;
        }

        void Unity_Power_float(float A, float B, out float Out)
        {
            Out = pow(A, B);
        }

        void Unity_Multiply_float(float A, float B, out float Out)
        {
            Out = A * B;
        }

        void Unity_Add_float3(float3 A, float3 B, out float3 Out)
        {
            Out = A + B;
        }

        struct Bindings_HeightDisplacement_b9c98ab237e518945923c754d0bbe831
        {
        };

        void SG_HeightDisplacement_b9c98ab237e518945923c754d0bbe831(float3 Vector3_62bf9535e3ff4df7880687e1587ce215, float Vector1_5c5c0f9b27bb4b6d9ea9f75db55607a7, Bindings_HeightDisplacement_b9c98ab237e518945923c754d0bbe831 IN, out float3 OutVector3_1)
        {
            float3 _Property_0f97590798cf4c53838eab4378e02560_Out_0 = Vector3_62bf9535e3ff4df7880687e1587ce215;
            float _Property_49df1d1a54214d609f7a1472d4f9e35e_Out_0 = Vector1_5c5c0f9b27bb4b6d9ea9f75db55607a7;
            float3 _Vector3_1f29590905e646dfbbe4e52d2fba295d_Out_0 = float3(0, _Property_49df1d1a54214d609f7a1472d4f9e35e_Out_0, 0);
            float3 _Add_7997ee2cc82741668225da98195a9cf7_Out_2;
            Unity_Add_float3(_Property_0f97590798cf4c53838eab4378e02560_Out_0, _Vector3_1f29590905e646dfbbe4e52d2fba295d_Out_0, _Add_7997ee2cc82741668225da98195a9cf7_Out_2);
            OutVector3_1 = _Add_7997ee2cc82741668225da98195a9cf7_Out_2;
        }

        void Unity_Saturate_float(float In, out float Out)
        {
            Out = saturate(In);
        }

        void Unity_Lerp_float4(float4 A, float4 B, float4 T, out float4 Out)
        {
            Out = lerp(A, B, T);
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
            UnityTexture2D _Property_e05227f480e24de0967ad56ec56f0111_Out_0 = UnityBuildTexture2DStructNoScale(Texture2D_42a4a1b82a9f4abe96dab75baa1e6491);
            #if defined(SHADER_API_GLES) && (SHADER_TARGET < 30)
              float4 _SampleTexture2DLOD_aef31cad67014997ba7b0c99b0a60e32_RGBA_0 = float4(0.0f, 0.0f, 0.0f, 1.0f);
            #else
              float4 _SampleTexture2DLOD_aef31cad67014997ba7b0c99b0a60e32_RGBA_0 = SAMPLE_TEXTURE2D_LOD(_Property_e05227f480e24de0967ad56ec56f0111_Out_0.tex, _Property_e05227f480e24de0967ad56ec56f0111_Out_0.samplerstate, IN.uv0.xy, 0);
            #endif
            float _SampleTexture2DLOD_aef31cad67014997ba7b0c99b0a60e32_R_5 = _SampleTexture2DLOD_aef31cad67014997ba7b0c99b0a60e32_RGBA_0.r;
            float _SampleTexture2DLOD_aef31cad67014997ba7b0c99b0a60e32_G_6 = _SampleTexture2DLOD_aef31cad67014997ba7b0c99b0a60e32_RGBA_0.g;
            float _SampleTexture2DLOD_aef31cad67014997ba7b0c99b0a60e32_B_7 = _SampleTexture2DLOD_aef31cad67014997ba7b0c99b0a60e32_RGBA_0.b;
            float _SampleTexture2DLOD_aef31cad67014997ba7b0c99b0a60e32_A_8 = _SampleTexture2DLOD_aef31cad67014997ba7b0c99b0a60e32_RGBA_0.a;
            UnityTexture2D _Property_cebacde6ccf84fd2beb8e37538e75b15_Out_0 = UnityBuildTexture2DStructNoScale(Texture2D_9689ebada1904c03ba6e4cbb82216610);
            #if defined(SHADER_API_GLES) && (SHADER_TARGET < 30)
              float4 _SampleTexture2DLOD_6becb53decd44143a598717795ab9611_RGBA_0 = float4(0.0f, 0.0f, 0.0f, 1.0f);
            #else
              float4 _SampleTexture2DLOD_6becb53decd44143a598717795ab9611_RGBA_0 = SAMPLE_TEXTURE2D_LOD(_Property_cebacde6ccf84fd2beb8e37538e75b15_Out_0.tex, _Property_cebacde6ccf84fd2beb8e37538e75b15_Out_0.samplerstate, IN.uv0.xy, 0);
            #endif
            float _SampleTexture2DLOD_6becb53decd44143a598717795ab9611_R_5 = _SampleTexture2DLOD_6becb53decd44143a598717795ab9611_RGBA_0.r;
            float _SampleTexture2DLOD_6becb53decd44143a598717795ab9611_G_6 = _SampleTexture2DLOD_6becb53decd44143a598717795ab9611_RGBA_0.g;
            float _SampleTexture2DLOD_6becb53decd44143a598717795ab9611_B_7 = _SampleTexture2DLOD_6becb53decd44143a598717795ab9611_RGBA_0.b;
            float _SampleTexture2DLOD_6becb53decd44143a598717795ab9611_A_8 = _SampleTexture2DLOD_6becb53decd44143a598717795ab9611_RGBA_0.a;
            float _OneMinus_d9bc4f62005242809b4528e86e1f3ef6_Out_1;
            Unity_OneMinus_float(_SampleTexture2DLOD_6becb53decd44143a598717795ab9611_R_5, _OneMinus_d9bc4f62005242809b4528e86e1f3ef6_Out_1);
            float _Power_937eb875940743b38b2ed441b457bdea_Out_2;
            Unity_Power_float(_OneMinus_d9bc4f62005242809b4528e86e1f3ef6_Out_1, 2, _Power_937eb875940743b38b2ed441b457bdea_Out_2);
            float _OneMinus_c2d552dce35d42ba835e5ee3ebf2c892_Out_1;
            Unity_OneMinus_float(_Power_937eb875940743b38b2ed441b457bdea_Out_2, _OneMinus_c2d552dce35d42ba835e5ee3ebf2c892_Out_1);
            float _Multiply_30256bfbfe7241d8912131f5e6de9988_Out_2;
            Unity_Multiply_float(_SampleTexture2DLOD_aef31cad67014997ba7b0c99b0a60e32_R_5, _OneMinus_c2d552dce35d42ba835e5ee3ebf2c892_Out_1, _Multiply_30256bfbfe7241d8912131f5e6de9988_Out_2);
            float _Property_d78f7cc8b639449aa812de1783598203_Out_0 = Vector1_cf0b9f3c26cf40e09df5caa461ffd8b4;
            float _Multiply_fa333bf6fddb4079afa8561e7f1ef4b0_Out_2;
            Unity_Multiply_float(_Multiply_30256bfbfe7241d8912131f5e6de9988_Out_2, _Property_d78f7cc8b639449aa812de1783598203_Out_0, _Multiply_fa333bf6fddb4079afa8561e7f1ef4b0_Out_2);
            Bindings_HeightDisplacement_b9c98ab237e518945923c754d0bbe831 _HeightDisplacement_1c8734ff1d9f46e0bb73e04b3d1d2d38;
            float3 _HeightDisplacement_1c8734ff1d9f46e0bb73e04b3d1d2d38_OutVector3_1;
            SG_HeightDisplacement_b9c98ab237e518945923c754d0bbe831(IN.AbsoluteWorldSpacePosition, _Multiply_fa333bf6fddb4079afa8561e7f1ef4b0_Out_2, _HeightDisplacement_1c8734ff1d9f46e0bb73e04b3d1d2d38, _HeightDisplacement_1c8734ff1d9f46e0bb73e04b3d1d2d38_OutVector3_1);
            float3 _Transform_37a6f3b78e9a4eb5898cade582e4ccd0_Out_1 = TransformWorldToObject(GetCameraRelativePositionWS(_HeightDisplacement_1c8734ff1d9f46e0bb73e04b3d1d2d38_OutVector3_1.xyz));
            description.Position = _Transform_37a6f3b78e9a4eb5898cade582e4ccd0_Out_1;
            description.Normal = IN.ObjectSpaceNormal;
            description.Tangent = IN.ObjectSpaceTangent;
            return description;
        }

            // Graph Pixel
            struct SurfaceDescription
        {
            float3 BaseColor;
        };

        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            UnityTexture2D _Property_a12442ff2fa64e91af4282051b30ed63_Out_0 = UnityBuildTexture2DStructNoScale(Texture2D_b9baf54b736a49b8931070640840afe0);
            float4 _SampleTexture2D_1b73574f3dcc4be69bed6da3fb53cdad_RGBA_0 = SAMPLE_TEXTURE2D(_Property_a12442ff2fa64e91af4282051b30ed63_Out_0.tex, _Property_a12442ff2fa64e91af4282051b30ed63_Out_0.samplerstate, IN.uv0.xy);
            float _SampleTexture2D_1b73574f3dcc4be69bed6da3fb53cdad_R_4 = _SampleTexture2D_1b73574f3dcc4be69bed6da3fb53cdad_RGBA_0.r;
            float _SampleTexture2D_1b73574f3dcc4be69bed6da3fb53cdad_G_5 = _SampleTexture2D_1b73574f3dcc4be69bed6da3fb53cdad_RGBA_0.g;
            float _SampleTexture2D_1b73574f3dcc4be69bed6da3fb53cdad_B_6 = _SampleTexture2D_1b73574f3dcc4be69bed6da3fb53cdad_RGBA_0.b;
            float _SampleTexture2D_1b73574f3dcc4be69bed6da3fb53cdad_A_7 = _SampleTexture2D_1b73574f3dcc4be69bed6da3fb53cdad_RGBA_0.a;
            UnityTexture2D _Property_62d4351a3ac24957881d73d316d2df21_Out_0 = UnityBuildTexture2DStructNoScale(Texture2D_748b7f78c75c42a4bdfc5b775def0238);
            float4 _SampleTexture2D_aa4f1ce104ec47e0adf7460035095291_RGBA_0 = SAMPLE_TEXTURE2D(_Property_62d4351a3ac24957881d73d316d2df21_Out_0.tex, _Property_62d4351a3ac24957881d73d316d2df21_Out_0.samplerstate, IN.uv0.xy);
            float _SampleTexture2D_aa4f1ce104ec47e0adf7460035095291_R_4 = _SampleTexture2D_aa4f1ce104ec47e0adf7460035095291_RGBA_0.r;
            float _SampleTexture2D_aa4f1ce104ec47e0adf7460035095291_G_5 = _SampleTexture2D_aa4f1ce104ec47e0adf7460035095291_RGBA_0.g;
            float _SampleTexture2D_aa4f1ce104ec47e0adf7460035095291_B_6 = _SampleTexture2D_aa4f1ce104ec47e0adf7460035095291_RGBA_0.b;
            float _SampleTexture2D_aa4f1ce104ec47e0adf7460035095291_A_7 = _SampleTexture2D_aa4f1ce104ec47e0adf7460035095291_RGBA_0.a;
            UnityTexture2D _Property_c98ec2bed3634840bfb49ad498c99735_Out_0 = UnityBuildTexture2DStructNoScale(Texture2D_9689ebada1904c03ba6e4cbb82216610);
            #if defined(SHADER_API_GLES) && (SHADER_TARGET < 30)
              float4 _SampleTexture2DLOD_c30d430b8173450f9e51e8421529e6e3_RGBA_0 = float4(0.0f, 0.0f, 0.0f, 1.0f);
            #else
              float4 _SampleTexture2DLOD_c30d430b8173450f9e51e8421529e6e3_RGBA_0 = SAMPLE_TEXTURE2D_LOD(_Property_c98ec2bed3634840bfb49ad498c99735_Out_0.tex, _Property_c98ec2bed3634840bfb49ad498c99735_Out_0.samplerstate, IN.uv0.xy, 0);
            #endif
            float _SampleTexture2DLOD_c30d430b8173450f9e51e8421529e6e3_R_5 = _SampleTexture2DLOD_c30d430b8173450f9e51e8421529e6e3_RGBA_0.r;
            float _SampleTexture2DLOD_c30d430b8173450f9e51e8421529e6e3_G_6 = _SampleTexture2DLOD_c30d430b8173450f9e51e8421529e6e3_RGBA_0.g;
            float _SampleTexture2DLOD_c30d430b8173450f9e51e8421529e6e3_B_7 = _SampleTexture2DLOD_c30d430b8173450f9e51e8421529e6e3_RGBA_0.b;
            float _SampleTexture2DLOD_c30d430b8173450f9e51e8421529e6e3_A_8 = _SampleTexture2DLOD_c30d430b8173450f9e51e8421529e6e3_RGBA_0.a;
            float _OneMinus_89dcf2a7834143aba0563ad51e6c15aa_Out_1;
            Unity_OneMinus_float(_SampleTexture2DLOD_c30d430b8173450f9e51e8421529e6e3_R_5, _OneMinus_89dcf2a7834143aba0563ad51e6c15aa_Out_1);
            float _Power_905fe49a0a164ae69012074a1ead4008_Out_2;
            Unity_Power_float(_OneMinus_89dcf2a7834143aba0563ad51e6c15aa_Out_1, 12, _Power_905fe49a0a164ae69012074a1ead4008_Out_2);
            float _OneMinus_3c99932ad6d6471bbdb0cddbc74ac2bf_Out_1;
            Unity_OneMinus_float(_Power_905fe49a0a164ae69012074a1ead4008_Out_2, _OneMinus_3c99932ad6d6471bbdb0cddbc74ac2bf_Out_1);
            float _Multiply_97877bb16ca04ac9ad8cd40aff771d41_Out_2;
            Unity_Multiply_float(_OneMinus_3c99932ad6d6471bbdb0cddbc74ac2bf_Out_1, 2, _Multiply_97877bb16ca04ac9ad8cd40aff771d41_Out_2);
            float _Saturate_41030dee08f34dc3bc99716f940972c0_Out_1;
            Unity_Saturate_float(_Multiply_97877bb16ca04ac9ad8cd40aff771d41_Out_2, _Saturate_41030dee08f34dc3bc99716f940972c0_Out_1);
            float4 _Lerp_72090be370eb47669bdf00c5c8529f7f_Out_3;
            Unity_Lerp_float4(_SampleTexture2D_1b73574f3dcc4be69bed6da3fb53cdad_RGBA_0, _SampleTexture2D_aa4f1ce104ec47e0adf7460035095291_RGBA_0, (_Saturate_41030dee08f34dc3bc99716f940972c0_Out_1.xxxx), _Lerp_72090be370eb47669bdf00c5c8529f7f_Out_3);
            Bindings_CelShading_802007f66a3e13d42802a3875ed35370 _CelShading_abf7811d76a7413ab3f888509118ab1c;
            _CelShading_abf7811d76a7413ab3f888509118ab1c.WorldSpaceNormal = IN.WorldSpaceNormal;
            _CelShading_abf7811d76a7413ab3f888509118ab1c.WorldSpaceTangent = IN.WorldSpaceTangent;
            _CelShading_abf7811d76a7413ab3f888509118ab1c.WorldSpaceBiTangent = IN.WorldSpaceBiTangent;
            _CelShading_abf7811d76a7413ab3f888509118ab1c.WorldSpaceViewDirection = IN.WorldSpaceViewDirection;
            _CelShading_abf7811d76a7413ab3f888509118ab1c.AbsoluteWorldSpacePosition = IN.AbsoluteWorldSpacePosition;
            _CelShading_abf7811d76a7413ab3f888509118ab1c.uv0 = IN.uv0;
            half4 _CelShading_abf7811d76a7413ab3f888509118ab1c_Color_1;
            SG_CelShading_802007f66a3e13d42802a3875ed35370(_Lerp_72090be370eb47669bdf00c5c8529f7f_Out_3, float3 (0, 0, 1), 0, 0, UnityBuildTexture2DStructNoScale(_CelShading_abf7811d76a7413ab3f888509118ab1c_Texture2D9666dd2f2f8f4fddb6c2a5c61ddfa9bd_3072536305), _CelShading_abf7811d76a7413ab3f888509118ab1c, _CelShading_abf7811d76a7413ab3f888509118ab1c_Color_1);
            surface.BaseColor = (_CelShading_abf7811d76a7413ab3f888509118ab1c_Color_1.xyz);
            return surface;
        }

            // --------------------------------------------------
            // Build Graph Inputs

            VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
        {
            VertexDescriptionInputs output;
            ZERO_INITIALIZE(VertexDescriptionInputs, output);

            output.ObjectSpaceNormal =           input.normalOS;
            output.WorldSpaceNormal =            TransformObjectToWorldNormal(input.normalOS);
            output.ObjectSpaceTangent =          input.tangentOS.xyz;
            output.WorldSpaceTangent =           TransformObjectToWorldDir(input.tangentOS.xyz);
            output.ObjectSpaceBiTangent =        normalize(cross(input.normalOS, input.tangentOS) * (input.tangentOS.w > 0.0f ? 1.0f : -1.0f) * GetOddNegativeScale());
            output.WorldSpaceBiTangent =         TransformObjectToWorldDir(output.ObjectSpaceBiTangent);
            output.ObjectSpacePosition =         input.positionOS;
            output.AbsoluteWorldSpacePosition =  GetAbsolutePositionWS(TransformObjectToWorld(input.positionOS));
            output.uv0 =                         input.uv0;

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
            output.AbsoluteWorldSpacePosition =  GetAbsolutePositionWS(input.positionWS);
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
        Blend One Zero
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
            #define ATTRIBUTES_NEED_NORMAL
            #define ATTRIBUTES_NEED_TANGENT
            #define ATTRIBUTES_NEED_TEXCOORD0
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
            float4 uv0 : TEXCOORD0;
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
            float3 WorldSpaceNormal;
            float3 ObjectSpaceTangent;
            float3 WorldSpaceTangent;
            float3 ObjectSpaceBiTangent;
            float3 WorldSpaceBiTangent;
            float3 ObjectSpacePosition;
            float3 AbsoluteWorldSpacePosition;
            float4 uv0;
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
        float4 _CelShading_abf7811d76a7413ab3f888509118ab1c_Texture2D9666dd2f2f8f4fddb6c2a5c61ddfa9bd_3072536305_TexelSize;
        float4 Texture2D_b9baf54b736a49b8931070640840afe0_TexelSize;
        float4 Texture2D_9689ebada1904c03ba6e4cbb82216610_TexelSize;
        float Vector1_cf0b9f3c26cf40e09df5caa461ffd8b4;
        float4 Texture2D_42a4a1b82a9f4abe96dab75baa1e6491_TexelSize;
        float4 Texture2D_748b7f78c75c42a4bdfc5b775def0238_TexelSize;
        float4 Texture2D_c99ba8a89a0e49c780df6a87923307d3_TexelSize;
        CBUFFER_END

        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(_CelShading_abf7811d76a7413ab3f888509118ab1c_Texture2D9666dd2f2f8f4fddb6c2a5c61ddfa9bd_3072536305);
        SAMPLER(sampler_CelShading_abf7811d76a7413ab3f888509118ab1c_Texture2D9666dd2f2f8f4fddb6c2a5c61ddfa9bd_3072536305);
        TEXTURE2D(Texture2D_b9baf54b736a49b8931070640840afe0);
        SAMPLER(samplerTexture2D_b9baf54b736a49b8931070640840afe0);
        TEXTURE2D(Texture2D_9689ebada1904c03ba6e4cbb82216610);
        SAMPLER(samplerTexture2D_9689ebada1904c03ba6e4cbb82216610);
        TEXTURE2D(Texture2D_42a4a1b82a9f4abe96dab75baa1e6491);
        SAMPLER(samplerTexture2D_42a4a1b82a9f4abe96dab75baa1e6491);
        TEXTURE2D(Texture2D_748b7f78c75c42a4bdfc5b775def0238);
        SAMPLER(samplerTexture2D_748b7f78c75c42a4bdfc5b775def0238);
        TEXTURE2D(Texture2D_c99ba8a89a0e49c780df6a87923307d3);
        SAMPLER(samplerTexture2D_c99ba8a89a0e49c780df6a87923307d3);

            // Graph Functions
            
        void Unity_OneMinus_float(float In, out float Out)
        {
            Out = 1 - In;
        }

        void Unity_Power_float(float A, float B, out float Out)
        {
            Out = pow(A, B);
        }

        void Unity_Multiply_float(float A, float B, out float Out)
        {
            Out = A * B;
        }

        void Unity_Add_float3(float3 A, float3 B, out float3 Out)
        {
            Out = A + B;
        }

        struct Bindings_HeightDisplacement_b9c98ab237e518945923c754d0bbe831
        {
        };

        void SG_HeightDisplacement_b9c98ab237e518945923c754d0bbe831(float3 Vector3_62bf9535e3ff4df7880687e1587ce215, float Vector1_5c5c0f9b27bb4b6d9ea9f75db55607a7, Bindings_HeightDisplacement_b9c98ab237e518945923c754d0bbe831 IN, out float3 OutVector3_1)
        {
            float3 _Property_0f97590798cf4c53838eab4378e02560_Out_0 = Vector3_62bf9535e3ff4df7880687e1587ce215;
            float _Property_49df1d1a54214d609f7a1472d4f9e35e_Out_0 = Vector1_5c5c0f9b27bb4b6d9ea9f75db55607a7;
            float3 _Vector3_1f29590905e646dfbbe4e52d2fba295d_Out_0 = float3(0, _Property_49df1d1a54214d609f7a1472d4f9e35e_Out_0, 0);
            float3 _Add_7997ee2cc82741668225da98195a9cf7_Out_2;
            Unity_Add_float3(_Property_0f97590798cf4c53838eab4378e02560_Out_0, _Vector3_1f29590905e646dfbbe4e52d2fba295d_Out_0, _Add_7997ee2cc82741668225da98195a9cf7_Out_2);
            OutVector3_1 = _Add_7997ee2cc82741668225da98195a9cf7_Out_2;
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
            UnityTexture2D _Property_e05227f480e24de0967ad56ec56f0111_Out_0 = UnityBuildTexture2DStructNoScale(Texture2D_42a4a1b82a9f4abe96dab75baa1e6491);
            #if defined(SHADER_API_GLES) && (SHADER_TARGET < 30)
              float4 _SampleTexture2DLOD_aef31cad67014997ba7b0c99b0a60e32_RGBA_0 = float4(0.0f, 0.0f, 0.0f, 1.0f);
            #else
              float4 _SampleTexture2DLOD_aef31cad67014997ba7b0c99b0a60e32_RGBA_0 = SAMPLE_TEXTURE2D_LOD(_Property_e05227f480e24de0967ad56ec56f0111_Out_0.tex, _Property_e05227f480e24de0967ad56ec56f0111_Out_0.samplerstate, IN.uv0.xy, 0);
            #endif
            float _SampleTexture2DLOD_aef31cad67014997ba7b0c99b0a60e32_R_5 = _SampleTexture2DLOD_aef31cad67014997ba7b0c99b0a60e32_RGBA_0.r;
            float _SampleTexture2DLOD_aef31cad67014997ba7b0c99b0a60e32_G_6 = _SampleTexture2DLOD_aef31cad67014997ba7b0c99b0a60e32_RGBA_0.g;
            float _SampleTexture2DLOD_aef31cad67014997ba7b0c99b0a60e32_B_7 = _SampleTexture2DLOD_aef31cad67014997ba7b0c99b0a60e32_RGBA_0.b;
            float _SampleTexture2DLOD_aef31cad67014997ba7b0c99b0a60e32_A_8 = _SampleTexture2DLOD_aef31cad67014997ba7b0c99b0a60e32_RGBA_0.a;
            UnityTexture2D _Property_cebacde6ccf84fd2beb8e37538e75b15_Out_0 = UnityBuildTexture2DStructNoScale(Texture2D_9689ebada1904c03ba6e4cbb82216610);
            #if defined(SHADER_API_GLES) && (SHADER_TARGET < 30)
              float4 _SampleTexture2DLOD_6becb53decd44143a598717795ab9611_RGBA_0 = float4(0.0f, 0.0f, 0.0f, 1.0f);
            #else
              float4 _SampleTexture2DLOD_6becb53decd44143a598717795ab9611_RGBA_0 = SAMPLE_TEXTURE2D_LOD(_Property_cebacde6ccf84fd2beb8e37538e75b15_Out_0.tex, _Property_cebacde6ccf84fd2beb8e37538e75b15_Out_0.samplerstate, IN.uv0.xy, 0);
            #endif
            float _SampleTexture2DLOD_6becb53decd44143a598717795ab9611_R_5 = _SampleTexture2DLOD_6becb53decd44143a598717795ab9611_RGBA_0.r;
            float _SampleTexture2DLOD_6becb53decd44143a598717795ab9611_G_6 = _SampleTexture2DLOD_6becb53decd44143a598717795ab9611_RGBA_0.g;
            float _SampleTexture2DLOD_6becb53decd44143a598717795ab9611_B_7 = _SampleTexture2DLOD_6becb53decd44143a598717795ab9611_RGBA_0.b;
            float _SampleTexture2DLOD_6becb53decd44143a598717795ab9611_A_8 = _SampleTexture2DLOD_6becb53decd44143a598717795ab9611_RGBA_0.a;
            float _OneMinus_d9bc4f62005242809b4528e86e1f3ef6_Out_1;
            Unity_OneMinus_float(_SampleTexture2DLOD_6becb53decd44143a598717795ab9611_R_5, _OneMinus_d9bc4f62005242809b4528e86e1f3ef6_Out_1);
            float _Power_937eb875940743b38b2ed441b457bdea_Out_2;
            Unity_Power_float(_OneMinus_d9bc4f62005242809b4528e86e1f3ef6_Out_1, 2, _Power_937eb875940743b38b2ed441b457bdea_Out_2);
            float _OneMinus_c2d552dce35d42ba835e5ee3ebf2c892_Out_1;
            Unity_OneMinus_float(_Power_937eb875940743b38b2ed441b457bdea_Out_2, _OneMinus_c2d552dce35d42ba835e5ee3ebf2c892_Out_1);
            float _Multiply_30256bfbfe7241d8912131f5e6de9988_Out_2;
            Unity_Multiply_float(_SampleTexture2DLOD_aef31cad67014997ba7b0c99b0a60e32_R_5, _OneMinus_c2d552dce35d42ba835e5ee3ebf2c892_Out_1, _Multiply_30256bfbfe7241d8912131f5e6de9988_Out_2);
            float _Property_d78f7cc8b639449aa812de1783598203_Out_0 = Vector1_cf0b9f3c26cf40e09df5caa461ffd8b4;
            float _Multiply_fa333bf6fddb4079afa8561e7f1ef4b0_Out_2;
            Unity_Multiply_float(_Multiply_30256bfbfe7241d8912131f5e6de9988_Out_2, _Property_d78f7cc8b639449aa812de1783598203_Out_0, _Multiply_fa333bf6fddb4079afa8561e7f1ef4b0_Out_2);
            Bindings_HeightDisplacement_b9c98ab237e518945923c754d0bbe831 _HeightDisplacement_1c8734ff1d9f46e0bb73e04b3d1d2d38;
            float3 _HeightDisplacement_1c8734ff1d9f46e0bb73e04b3d1d2d38_OutVector3_1;
            SG_HeightDisplacement_b9c98ab237e518945923c754d0bbe831(IN.AbsoluteWorldSpacePosition, _Multiply_fa333bf6fddb4079afa8561e7f1ef4b0_Out_2, _HeightDisplacement_1c8734ff1d9f46e0bb73e04b3d1d2d38, _HeightDisplacement_1c8734ff1d9f46e0bb73e04b3d1d2d38_OutVector3_1);
            float3 _Transform_37a6f3b78e9a4eb5898cade582e4ccd0_Out_1 = TransformWorldToObject(GetCameraRelativePositionWS(_HeightDisplacement_1c8734ff1d9f46e0bb73e04b3d1d2d38_OutVector3_1.xyz));
            description.Position = _Transform_37a6f3b78e9a4eb5898cade582e4ccd0_Out_1;
            description.Normal = IN.ObjectSpaceNormal;
            description.Tangent = IN.ObjectSpaceTangent;
            return description;
        }

            // Graph Pixel
            struct SurfaceDescription
        {
        };

        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            return surface;
        }

            // --------------------------------------------------
            // Build Graph Inputs

            VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
        {
            VertexDescriptionInputs output;
            ZERO_INITIALIZE(VertexDescriptionInputs, output);

            output.ObjectSpaceNormal =           input.normalOS;
            output.WorldSpaceNormal =            TransformObjectToWorldNormal(input.normalOS);
            output.ObjectSpaceTangent =          input.tangentOS.xyz;
            output.WorldSpaceTangent =           TransformObjectToWorldDir(input.tangentOS.xyz);
            output.ObjectSpaceBiTangent =        normalize(cross(input.normalOS, input.tangentOS) * (input.tangentOS.w > 0.0f ? 1.0f : -1.0f) * GetOddNegativeScale());
            output.WorldSpaceBiTangent =         TransformObjectToWorldDir(output.ObjectSpaceBiTangent);
            output.ObjectSpacePosition =         input.positionOS;
            output.AbsoluteWorldSpacePosition =  GetAbsolutePositionWS(TransformObjectToWorld(input.positionOS));
            output.uv0 =                         input.uv0;

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
        Blend One Zero
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
            #define ATTRIBUTES_NEED_NORMAL
            #define ATTRIBUTES_NEED_TANGENT
            #define ATTRIBUTES_NEED_TEXCOORD0
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
            float4 uv0 : TEXCOORD0;
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
            float3 WorldSpaceNormal;
            float3 ObjectSpaceTangent;
            float3 WorldSpaceTangent;
            float3 ObjectSpaceBiTangent;
            float3 WorldSpaceBiTangent;
            float3 ObjectSpacePosition;
            float3 AbsoluteWorldSpacePosition;
            float4 uv0;
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
        float4 _CelShading_abf7811d76a7413ab3f888509118ab1c_Texture2D9666dd2f2f8f4fddb6c2a5c61ddfa9bd_3072536305_TexelSize;
        float4 Texture2D_b9baf54b736a49b8931070640840afe0_TexelSize;
        float4 Texture2D_9689ebada1904c03ba6e4cbb82216610_TexelSize;
        float Vector1_cf0b9f3c26cf40e09df5caa461ffd8b4;
        float4 Texture2D_42a4a1b82a9f4abe96dab75baa1e6491_TexelSize;
        float4 Texture2D_748b7f78c75c42a4bdfc5b775def0238_TexelSize;
        float4 Texture2D_c99ba8a89a0e49c780df6a87923307d3_TexelSize;
        CBUFFER_END

        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(_CelShading_abf7811d76a7413ab3f888509118ab1c_Texture2D9666dd2f2f8f4fddb6c2a5c61ddfa9bd_3072536305);
        SAMPLER(sampler_CelShading_abf7811d76a7413ab3f888509118ab1c_Texture2D9666dd2f2f8f4fddb6c2a5c61ddfa9bd_3072536305);
        TEXTURE2D(Texture2D_b9baf54b736a49b8931070640840afe0);
        SAMPLER(samplerTexture2D_b9baf54b736a49b8931070640840afe0);
        TEXTURE2D(Texture2D_9689ebada1904c03ba6e4cbb82216610);
        SAMPLER(samplerTexture2D_9689ebada1904c03ba6e4cbb82216610);
        TEXTURE2D(Texture2D_42a4a1b82a9f4abe96dab75baa1e6491);
        SAMPLER(samplerTexture2D_42a4a1b82a9f4abe96dab75baa1e6491);
        TEXTURE2D(Texture2D_748b7f78c75c42a4bdfc5b775def0238);
        SAMPLER(samplerTexture2D_748b7f78c75c42a4bdfc5b775def0238);
        TEXTURE2D(Texture2D_c99ba8a89a0e49c780df6a87923307d3);
        SAMPLER(samplerTexture2D_c99ba8a89a0e49c780df6a87923307d3);

            // Graph Functions
            
        void Unity_OneMinus_float(float In, out float Out)
        {
            Out = 1 - In;
        }

        void Unity_Power_float(float A, float B, out float Out)
        {
            Out = pow(A, B);
        }

        void Unity_Multiply_float(float A, float B, out float Out)
        {
            Out = A * B;
        }

        void Unity_Add_float3(float3 A, float3 B, out float3 Out)
        {
            Out = A + B;
        }

        struct Bindings_HeightDisplacement_b9c98ab237e518945923c754d0bbe831
        {
        };

        void SG_HeightDisplacement_b9c98ab237e518945923c754d0bbe831(float3 Vector3_62bf9535e3ff4df7880687e1587ce215, float Vector1_5c5c0f9b27bb4b6d9ea9f75db55607a7, Bindings_HeightDisplacement_b9c98ab237e518945923c754d0bbe831 IN, out float3 OutVector3_1)
        {
            float3 _Property_0f97590798cf4c53838eab4378e02560_Out_0 = Vector3_62bf9535e3ff4df7880687e1587ce215;
            float _Property_49df1d1a54214d609f7a1472d4f9e35e_Out_0 = Vector1_5c5c0f9b27bb4b6d9ea9f75db55607a7;
            float3 _Vector3_1f29590905e646dfbbe4e52d2fba295d_Out_0 = float3(0, _Property_49df1d1a54214d609f7a1472d4f9e35e_Out_0, 0);
            float3 _Add_7997ee2cc82741668225da98195a9cf7_Out_2;
            Unity_Add_float3(_Property_0f97590798cf4c53838eab4378e02560_Out_0, _Vector3_1f29590905e646dfbbe4e52d2fba295d_Out_0, _Add_7997ee2cc82741668225da98195a9cf7_Out_2);
            OutVector3_1 = _Add_7997ee2cc82741668225da98195a9cf7_Out_2;
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
            UnityTexture2D _Property_e05227f480e24de0967ad56ec56f0111_Out_0 = UnityBuildTexture2DStructNoScale(Texture2D_42a4a1b82a9f4abe96dab75baa1e6491);
            #if defined(SHADER_API_GLES) && (SHADER_TARGET < 30)
              float4 _SampleTexture2DLOD_aef31cad67014997ba7b0c99b0a60e32_RGBA_0 = float4(0.0f, 0.0f, 0.0f, 1.0f);
            #else
              float4 _SampleTexture2DLOD_aef31cad67014997ba7b0c99b0a60e32_RGBA_0 = SAMPLE_TEXTURE2D_LOD(_Property_e05227f480e24de0967ad56ec56f0111_Out_0.tex, _Property_e05227f480e24de0967ad56ec56f0111_Out_0.samplerstate, IN.uv0.xy, 0);
            #endif
            float _SampleTexture2DLOD_aef31cad67014997ba7b0c99b0a60e32_R_5 = _SampleTexture2DLOD_aef31cad67014997ba7b0c99b0a60e32_RGBA_0.r;
            float _SampleTexture2DLOD_aef31cad67014997ba7b0c99b0a60e32_G_6 = _SampleTexture2DLOD_aef31cad67014997ba7b0c99b0a60e32_RGBA_0.g;
            float _SampleTexture2DLOD_aef31cad67014997ba7b0c99b0a60e32_B_7 = _SampleTexture2DLOD_aef31cad67014997ba7b0c99b0a60e32_RGBA_0.b;
            float _SampleTexture2DLOD_aef31cad67014997ba7b0c99b0a60e32_A_8 = _SampleTexture2DLOD_aef31cad67014997ba7b0c99b0a60e32_RGBA_0.a;
            UnityTexture2D _Property_cebacde6ccf84fd2beb8e37538e75b15_Out_0 = UnityBuildTexture2DStructNoScale(Texture2D_9689ebada1904c03ba6e4cbb82216610);
            #if defined(SHADER_API_GLES) && (SHADER_TARGET < 30)
              float4 _SampleTexture2DLOD_6becb53decd44143a598717795ab9611_RGBA_0 = float4(0.0f, 0.0f, 0.0f, 1.0f);
            #else
              float4 _SampleTexture2DLOD_6becb53decd44143a598717795ab9611_RGBA_0 = SAMPLE_TEXTURE2D_LOD(_Property_cebacde6ccf84fd2beb8e37538e75b15_Out_0.tex, _Property_cebacde6ccf84fd2beb8e37538e75b15_Out_0.samplerstate, IN.uv0.xy, 0);
            #endif
            float _SampleTexture2DLOD_6becb53decd44143a598717795ab9611_R_5 = _SampleTexture2DLOD_6becb53decd44143a598717795ab9611_RGBA_0.r;
            float _SampleTexture2DLOD_6becb53decd44143a598717795ab9611_G_6 = _SampleTexture2DLOD_6becb53decd44143a598717795ab9611_RGBA_0.g;
            float _SampleTexture2DLOD_6becb53decd44143a598717795ab9611_B_7 = _SampleTexture2DLOD_6becb53decd44143a598717795ab9611_RGBA_0.b;
            float _SampleTexture2DLOD_6becb53decd44143a598717795ab9611_A_8 = _SampleTexture2DLOD_6becb53decd44143a598717795ab9611_RGBA_0.a;
            float _OneMinus_d9bc4f62005242809b4528e86e1f3ef6_Out_1;
            Unity_OneMinus_float(_SampleTexture2DLOD_6becb53decd44143a598717795ab9611_R_5, _OneMinus_d9bc4f62005242809b4528e86e1f3ef6_Out_1);
            float _Power_937eb875940743b38b2ed441b457bdea_Out_2;
            Unity_Power_float(_OneMinus_d9bc4f62005242809b4528e86e1f3ef6_Out_1, 2, _Power_937eb875940743b38b2ed441b457bdea_Out_2);
            float _OneMinus_c2d552dce35d42ba835e5ee3ebf2c892_Out_1;
            Unity_OneMinus_float(_Power_937eb875940743b38b2ed441b457bdea_Out_2, _OneMinus_c2d552dce35d42ba835e5ee3ebf2c892_Out_1);
            float _Multiply_30256bfbfe7241d8912131f5e6de9988_Out_2;
            Unity_Multiply_float(_SampleTexture2DLOD_aef31cad67014997ba7b0c99b0a60e32_R_5, _OneMinus_c2d552dce35d42ba835e5ee3ebf2c892_Out_1, _Multiply_30256bfbfe7241d8912131f5e6de9988_Out_2);
            float _Property_d78f7cc8b639449aa812de1783598203_Out_0 = Vector1_cf0b9f3c26cf40e09df5caa461ffd8b4;
            float _Multiply_fa333bf6fddb4079afa8561e7f1ef4b0_Out_2;
            Unity_Multiply_float(_Multiply_30256bfbfe7241d8912131f5e6de9988_Out_2, _Property_d78f7cc8b639449aa812de1783598203_Out_0, _Multiply_fa333bf6fddb4079afa8561e7f1ef4b0_Out_2);
            Bindings_HeightDisplacement_b9c98ab237e518945923c754d0bbe831 _HeightDisplacement_1c8734ff1d9f46e0bb73e04b3d1d2d38;
            float3 _HeightDisplacement_1c8734ff1d9f46e0bb73e04b3d1d2d38_OutVector3_1;
            SG_HeightDisplacement_b9c98ab237e518945923c754d0bbe831(IN.AbsoluteWorldSpacePosition, _Multiply_fa333bf6fddb4079afa8561e7f1ef4b0_Out_2, _HeightDisplacement_1c8734ff1d9f46e0bb73e04b3d1d2d38, _HeightDisplacement_1c8734ff1d9f46e0bb73e04b3d1d2d38_OutVector3_1);
            float3 _Transform_37a6f3b78e9a4eb5898cade582e4ccd0_Out_1 = TransformWorldToObject(GetCameraRelativePositionWS(_HeightDisplacement_1c8734ff1d9f46e0bb73e04b3d1d2d38_OutVector3_1.xyz));
            description.Position = _Transform_37a6f3b78e9a4eb5898cade582e4ccd0_Out_1;
            description.Normal = IN.ObjectSpaceNormal;
            description.Tangent = IN.ObjectSpaceTangent;
            return description;
        }

            // Graph Pixel
            struct SurfaceDescription
        {
        };

        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            return surface;
        }

            // --------------------------------------------------
            // Build Graph Inputs

            VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
        {
            VertexDescriptionInputs output;
            ZERO_INITIALIZE(VertexDescriptionInputs, output);

            output.ObjectSpaceNormal =           input.normalOS;
            output.WorldSpaceNormal =            TransformObjectToWorldNormal(input.normalOS);
            output.ObjectSpaceTangent =          input.tangentOS.xyz;
            output.WorldSpaceTangent =           TransformObjectToWorldDir(input.tangentOS.xyz);
            output.ObjectSpaceBiTangent =        normalize(cross(input.normalOS, input.tangentOS) * (input.tangentOS.w > 0.0f ? 1.0f : -1.0f) * GetOddNegativeScale());
            output.WorldSpaceBiTangent =         TransformObjectToWorldDir(output.ObjectSpaceBiTangent);
            output.ObjectSpacePosition =         input.positionOS;
            output.AbsoluteWorldSpacePosition =  GetAbsolutePositionWS(TransformObjectToWorld(input.positionOS));
            output.uv0 =                         input.uv0;

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
            "RenderType"="Opaque"
            "UniversalMaterialType" = "Unlit"
            "Queue"="Geometry"
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
        Blend One Zero
        ZTest LEqual
        ZWrite On

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
            float3 AbsoluteWorldSpacePosition;
            float4 uv0;
        };
        struct VertexDescriptionInputs
        {
            float3 ObjectSpaceNormal;
            float3 WorldSpaceNormal;
            float3 ObjectSpaceTangent;
            float3 WorldSpaceTangent;
            float3 ObjectSpaceBiTangent;
            float3 WorldSpaceBiTangent;
            float3 ObjectSpacePosition;
            float3 AbsoluteWorldSpacePosition;
            float4 uv0;
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
        float4 _CelShading_abf7811d76a7413ab3f888509118ab1c_Texture2D9666dd2f2f8f4fddb6c2a5c61ddfa9bd_3072536305_TexelSize;
        float4 Texture2D_b9baf54b736a49b8931070640840afe0_TexelSize;
        float4 Texture2D_9689ebada1904c03ba6e4cbb82216610_TexelSize;
        float Vector1_cf0b9f3c26cf40e09df5caa461ffd8b4;
        float4 Texture2D_42a4a1b82a9f4abe96dab75baa1e6491_TexelSize;
        float4 Texture2D_748b7f78c75c42a4bdfc5b775def0238_TexelSize;
        float4 Texture2D_c99ba8a89a0e49c780df6a87923307d3_TexelSize;
        CBUFFER_END

        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(_CelShading_abf7811d76a7413ab3f888509118ab1c_Texture2D9666dd2f2f8f4fddb6c2a5c61ddfa9bd_3072536305);
        SAMPLER(sampler_CelShading_abf7811d76a7413ab3f888509118ab1c_Texture2D9666dd2f2f8f4fddb6c2a5c61ddfa9bd_3072536305);
        TEXTURE2D(Texture2D_b9baf54b736a49b8931070640840afe0);
        SAMPLER(samplerTexture2D_b9baf54b736a49b8931070640840afe0);
        TEXTURE2D(Texture2D_9689ebada1904c03ba6e4cbb82216610);
        SAMPLER(samplerTexture2D_9689ebada1904c03ba6e4cbb82216610);
        TEXTURE2D(Texture2D_42a4a1b82a9f4abe96dab75baa1e6491);
        SAMPLER(samplerTexture2D_42a4a1b82a9f4abe96dab75baa1e6491);
        TEXTURE2D(Texture2D_748b7f78c75c42a4bdfc5b775def0238);
        SAMPLER(samplerTexture2D_748b7f78c75c42a4bdfc5b775def0238);
        TEXTURE2D(Texture2D_c99ba8a89a0e49c780df6a87923307d3);
        SAMPLER(samplerTexture2D_c99ba8a89a0e49c780df6a87923307d3);

            // Graph Functions
            
        void Unity_OneMinus_float(float In, out float Out)
        {
            Out = 1 - In;
        }

        void Unity_Power_float(float A, float B, out float Out)
        {
            Out = pow(A, B);
        }

        void Unity_Multiply_float(float A, float B, out float Out)
        {
            Out = A * B;
        }

        void Unity_Add_float3(float3 A, float3 B, out float3 Out)
        {
            Out = A + B;
        }

        struct Bindings_HeightDisplacement_b9c98ab237e518945923c754d0bbe831
        {
        };

        void SG_HeightDisplacement_b9c98ab237e518945923c754d0bbe831(float3 Vector3_62bf9535e3ff4df7880687e1587ce215, float Vector1_5c5c0f9b27bb4b6d9ea9f75db55607a7, Bindings_HeightDisplacement_b9c98ab237e518945923c754d0bbe831 IN, out float3 OutVector3_1)
        {
            float3 _Property_0f97590798cf4c53838eab4378e02560_Out_0 = Vector3_62bf9535e3ff4df7880687e1587ce215;
            float _Property_49df1d1a54214d609f7a1472d4f9e35e_Out_0 = Vector1_5c5c0f9b27bb4b6d9ea9f75db55607a7;
            float3 _Vector3_1f29590905e646dfbbe4e52d2fba295d_Out_0 = float3(0, _Property_49df1d1a54214d609f7a1472d4f9e35e_Out_0, 0);
            float3 _Add_7997ee2cc82741668225da98195a9cf7_Out_2;
            Unity_Add_float3(_Property_0f97590798cf4c53838eab4378e02560_Out_0, _Vector3_1f29590905e646dfbbe4e52d2fba295d_Out_0, _Add_7997ee2cc82741668225da98195a9cf7_Out_2);
            OutVector3_1 = _Add_7997ee2cc82741668225da98195a9cf7_Out_2;
        }

        void Unity_Saturate_float(float In, out float Out)
        {
            Out = saturate(In);
        }

        void Unity_Lerp_float4(float4 A, float4 B, float4 T, out float4 Out)
        {
            Out = lerp(A, B, T);
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
            UnityTexture2D _Property_e05227f480e24de0967ad56ec56f0111_Out_0 = UnityBuildTexture2DStructNoScale(Texture2D_42a4a1b82a9f4abe96dab75baa1e6491);
            #if defined(SHADER_API_GLES) && (SHADER_TARGET < 30)
              float4 _SampleTexture2DLOD_aef31cad67014997ba7b0c99b0a60e32_RGBA_0 = float4(0.0f, 0.0f, 0.0f, 1.0f);
            #else
              float4 _SampleTexture2DLOD_aef31cad67014997ba7b0c99b0a60e32_RGBA_0 = SAMPLE_TEXTURE2D_LOD(_Property_e05227f480e24de0967ad56ec56f0111_Out_0.tex, _Property_e05227f480e24de0967ad56ec56f0111_Out_0.samplerstate, IN.uv0.xy, 0);
            #endif
            float _SampleTexture2DLOD_aef31cad67014997ba7b0c99b0a60e32_R_5 = _SampleTexture2DLOD_aef31cad67014997ba7b0c99b0a60e32_RGBA_0.r;
            float _SampleTexture2DLOD_aef31cad67014997ba7b0c99b0a60e32_G_6 = _SampleTexture2DLOD_aef31cad67014997ba7b0c99b0a60e32_RGBA_0.g;
            float _SampleTexture2DLOD_aef31cad67014997ba7b0c99b0a60e32_B_7 = _SampleTexture2DLOD_aef31cad67014997ba7b0c99b0a60e32_RGBA_0.b;
            float _SampleTexture2DLOD_aef31cad67014997ba7b0c99b0a60e32_A_8 = _SampleTexture2DLOD_aef31cad67014997ba7b0c99b0a60e32_RGBA_0.a;
            UnityTexture2D _Property_cebacde6ccf84fd2beb8e37538e75b15_Out_0 = UnityBuildTexture2DStructNoScale(Texture2D_9689ebada1904c03ba6e4cbb82216610);
            #if defined(SHADER_API_GLES) && (SHADER_TARGET < 30)
              float4 _SampleTexture2DLOD_6becb53decd44143a598717795ab9611_RGBA_0 = float4(0.0f, 0.0f, 0.0f, 1.0f);
            #else
              float4 _SampleTexture2DLOD_6becb53decd44143a598717795ab9611_RGBA_0 = SAMPLE_TEXTURE2D_LOD(_Property_cebacde6ccf84fd2beb8e37538e75b15_Out_0.tex, _Property_cebacde6ccf84fd2beb8e37538e75b15_Out_0.samplerstate, IN.uv0.xy, 0);
            #endif
            float _SampleTexture2DLOD_6becb53decd44143a598717795ab9611_R_5 = _SampleTexture2DLOD_6becb53decd44143a598717795ab9611_RGBA_0.r;
            float _SampleTexture2DLOD_6becb53decd44143a598717795ab9611_G_6 = _SampleTexture2DLOD_6becb53decd44143a598717795ab9611_RGBA_0.g;
            float _SampleTexture2DLOD_6becb53decd44143a598717795ab9611_B_7 = _SampleTexture2DLOD_6becb53decd44143a598717795ab9611_RGBA_0.b;
            float _SampleTexture2DLOD_6becb53decd44143a598717795ab9611_A_8 = _SampleTexture2DLOD_6becb53decd44143a598717795ab9611_RGBA_0.a;
            float _OneMinus_d9bc4f62005242809b4528e86e1f3ef6_Out_1;
            Unity_OneMinus_float(_SampleTexture2DLOD_6becb53decd44143a598717795ab9611_R_5, _OneMinus_d9bc4f62005242809b4528e86e1f3ef6_Out_1);
            float _Power_937eb875940743b38b2ed441b457bdea_Out_2;
            Unity_Power_float(_OneMinus_d9bc4f62005242809b4528e86e1f3ef6_Out_1, 2, _Power_937eb875940743b38b2ed441b457bdea_Out_2);
            float _OneMinus_c2d552dce35d42ba835e5ee3ebf2c892_Out_1;
            Unity_OneMinus_float(_Power_937eb875940743b38b2ed441b457bdea_Out_2, _OneMinus_c2d552dce35d42ba835e5ee3ebf2c892_Out_1);
            float _Multiply_30256bfbfe7241d8912131f5e6de9988_Out_2;
            Unity_Multiply_float(_SampleTexture2DLOD_aef31cad67014997ba7b0c99b0a60e32_R_5, _OneMinus_c2d552dce35d42ba835e5ee3ebf2c892_Out_1, _Multiply_30256bfbfe7241d8912131f5e6de9988_Out_2);
            float _Property_d78f7cc8b639449aa812de1783598203_Out_0 = Vector1_cf0b9f3c26cf40e09df5caa461ffd8b4;
            float _Multiply_fa333bf6fddb4079afa8561e7f1ef4b0_Out_2;
            Unity_Multiply_float(_Multiply_30256bfbfe7241d8912131f5e6de9988_Out_2, _Property_d78f7cc8b639449aa812de1783598203_Out_0, _Multiply_fa333bf6fddb4079afa8561e7f1ef4b0_Out_2);
            Bindings_HeightDisplacement_b9c98ab237e518945923c754d0bbe831 _HeightDisplacement_1c8734ff1d9f46e0bb73e04b3d1d2d38;
            float3 _HeightDisplacement_1c8734ff1d9f46e0bb73e04b3d1d2d38_OutVector3_1;
            SG_HeightDisplacement_b9c98ab237e518945923c754d0bbe831(IN.AbsoluteWorldSpacePosition, _Multiply_fa333bf6fddb4079afa8561e7f1ef4b0_Out_2, _HeightDisplacement_1c8734ff1d9f46e0bb73e04b3d1d2d38, _HeightDisplacement_1c8734ff1d9f46e0bb73e04b3d1d2d38_OutVector3_1);
            float3 _Transform_37a6f3b78e9a4eb5898cade582e4ccd0_Out_1 = TransformWorldToObject(GetCameraRelativePositionWS(_HeightDisplacement_1c8734ff1d9f46e0bb73e04b3d1d2d38_OutVector3_1.xyz));
            description.Position = _Transform_37a6f3b78e9a4eb5898cade582e4ccd0_Out_1;
            description.Normal = IN.ObjectSpaceNormal;
            description.Tangent = IN.ObjectSpaceTangent;
            return description;
        }

            // Graph Pixel
            struct SurfaceDescription
        {
            float3 BaseColor;
        };

        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            UnityTexture2D _Property_a12442ff2fa64e91af4282051b30ed63_Out_0 = UnityBuildTexture2DStructNoScale(Texture2D_b9baf54b736a49b8931070640840afe0);
            float4 _SampleTexture2D_1b73574f3dcc4be69bed6da3fb53cdad_RGBA_0 = SAMPLE_TEXTURE2D(_Property_a12442ff2fa64e91af4282051b30ed63_Out_0.tex, _Property_a12442ff2fa64e91af4282051b30ed63_Out_0.samplerstate, IN.uv0.xy);
            float _SampleTexture2D_1b73574f3dcc4be69bed6da3fb53cdad_R_4 = _SampleTexture2D_1b73574f3dcc4be69bed6da3fb53cdad_RGBA_0.r;
            float _SampleTexture2D_1b73574f3dcc4be69bed6da3fb53cdad_G_5 = _SampleTexture2D_1b73574f3dcc4be69bed6da3fb53cdad_RGBA_0.g;
            float _SampleTexture2D_1b73574f3dcc4be69bed6da3fb53cdad_B_6 = _SampleTexture2D_1b73574f3dcc4be69bed6da3fb53cdad_RGBA_0.b;
            float _SampleTexture2D_1b73574f3dcc4be69bed6da3fb53cdad_A_7 = _SampleTexture2D_1b73574f3dcc4be69bed6da3fb53cdad_RGBA_0.a;
            UnityTexture2D _Property_62d4351a3ac24957881d73d316d2df21_Out_0 = UnityBuildTexture2DStructNoScale(Texture2D_748b7f78c75c42a4bdfc5b775def0238);
            float4 _SampleTexture2D_aa4f1ce104ec47e0adf7460035095291_RGBA_0 = SAMPLE_TEXTURE2D(_Property_62d4351a3ac24957881d73d316d2df21_Out_0.tex, _Property_62d4351a3ac24957881d73d316d2df21_Out_0.samplerstate, IN.uv0.xy);
            float _SampleTexture2D_aa4f1ce104ec47e0adf7460035095291_R_4 = _SampleTexture2D_aa4f1ce104ec47e0adf7460035095291_RGBA_0.r;
            float _SampleTexture2D_aa4f1ce104ec47e0adf7460035095291_G_5 = _SampleTexture2D_aa4f1ce104ec47e0adf7460035095291_RGBA_0.g;
            float _SampleTexture2D_aa4f1ce104ec47e0adf7460035095291_B_6 = _SampleTexture2D_aa4f1ce104ec47e0adf7460035095291_RGBA_0.b;
            float _SampleTexture2D_aa4f1ce104ec47e0adf7460035095291_A_7 = _SampleTexture2D_aa4f1ce104ec47e0adf7460035095291_RGBA_0.a;
            UnityTexture2D _Property_c98ec2bed3634840bfb49ad498c99735_Out_0 = UnityBuildTexture2DStructNoScale(Texture2D_9689ebada1904c03ba6e4cbb82216610);
            #if defined(SHADER_API_GLES) && (SHADER_TARGET < 30)
              float4 _SampleTexture2DLOD_c30d430b8173450f9e51e8421529e6e3_RGBA_0 = float4(0.0f, 0.0f, 0.0f, 1.0f);
            #else
              float4 _SampleTexture2DLOD_c30d430b8173450f9e51e8421529e6e3_RGBA_0 = SAMPLE_TEXTURE2D_LOD(_Property_c98ec2bed3634840bfb49ad498c99735_Out_0.tex, _Property_c98ec2bed3634840bfb49ad498c99735_Out_0.samplerstate, IN.uv0.xy, 0);
            #endif
            float _SampleTexture2DLOD_c30d430b8173450f9e51e8421529e6e3_R_5 = _SampleTexture2DLOD_c30d430b8173450f9e51e8421529e6e3_RGBA_0.r;
            float _SampleTexture2DLOD_c30d430b8173450f9e51e8421529e6e3_G_6 = _SampleTexture2DLOD_c30d430b8173450f9e51e8421529e6e3_RGBA_0.g;
            float _SampleTexture2DLOD_c30d430b8173450f9e51e8421529e6e3_B_7 = _SampleTexture2DLOD_c30d430b8173450f9e51e8421529e6e3_RGBA_0.b;
            float _SampleTexture2DLOD_c30d430b8173450f9e51e8421529e6e3_A_8 = _SampleTexture2DLOD_c30d430b8173450f9e51e8421529e6e3_RGBA_0.a;
            float _OneMinus_89dcf2a7834143aba0563ad51e6c15aa_Out_1;
            Unity_OneMinus_float(_SampleTexture2DLOD_c30d430b8173450f9e51e8421529e6e3_R_5, _OneMinus_89dcf2a7834143aba0563ad51e6c15aa_Out_1);
            float _Power_905fe49a0a164ae69012074a1ead4008_Out_2;
            Unity_Power_float(_OneMinus_89dcf2a7834143aba0563ad51e6c15aa_Out_1, 12, _Power_905fe49a0a164ae69012074a1ead4008_Out_2);
            float _OneMinus_3c99932ad6d6471bbdb0cddbc74ac2bf_Out_1;
            Unity_OneMinus_float(_Power_905fe49a0a164ae69012074a1ead4008_Out_2, _OneMinus_3c99932ad6d6471bbdb0cddbc74ac2bf_Out_1);
            float _Multiply_97877bb16ca04ac9ad8cd40aff771d41_Out_2;
            Unity_Multiply_float(_OneMinus_3c99932ad6d6471bbdb0cddbc74ac2bf_Out_1, 2, _Multiply_97877bb16ca04ac9ad8cd40aff771d41_Out_2);
            float _Saturate_41030dee08f34dc3bc99716f940972c0_Out_1;
            Unity_Saturate_float(_Multiply_97877bb16ca04ac9ad8cd40aff771d41_Out_2, _Saturate_41030dee08f34dc3bc99716f940972c0_Out_1);
            float4 _Lerp_72090be370eb47669bdf00c5c8529f7f_Out_3;
            Unity_Lerp_float4(_SampleTexture2D_1b73574f3dcc4be69bed6da3fb53cdad_RGBA_0, _SampleTexture2D_aa4f1ce104ec47e0adf7460035095291_RGBA_0, (_Saturate_41030dee08f34dc3bc99716f940972c0_Out_1.xxxx), _Lerp_72090be370eb47669bdf00c5c8529f7f_Out_3);
            Bindings_CelShading_802007f66a3e13d42802a3875ed35370 _CelShading_abf7811d76a7413ab3f888509118ab1c;
            _CelShading_abf7811d76a7413ab3f888509118ab1c.WorldSpaceNormal = IN.WorldSpaceNormal;
            _CelShading_abf7811d76a7413ab3f888509118ab1c.WorldSpaceTangent = IN.WorldSpaceTangent;
            _CelShading_abf7811d76a7413ab3f888509118ab1c.WorldSpaceBiTangent = IN.WorldSpaceBiTangent;
            _CelShading_abf7811d76a7413ab3f888509118ab1c.WorldSpaceViewDirection = IN.WorldSpaceViewDirection;
            _CelShading_abf7811d76a7413ab3f888509118ab1c.AbsoluteWorldSpacePosition = IN.AbsoluteWorldSpacePosition;
            _CelShading_abf7811d76a7413ab3f888509118ab1c.uv0 = IN.uv0;
            half4 _CelShading_abf7811d76a7413ab3f888509118ab1c_Color_1;
            SG_CelShading_802007f66a3e13d42802a3875ed35370(_Lerp_72090be370eb47669bdf00c5c8529f7f_Out_3, float3 (0, 0, 1), 0, 0, UnityBuildTexture2DStructNoScale(_CelShading_abf7811d76a7413ab3f888509118ab1c_Texture2D9666dd2f2f8f4fddb6c2a5c61ddfa9bd_3072536305), _CelShading_abf7811d76a7413ab3f888509118ab1c, _CelShading_abf7811d76a7413ab3f888509118ab1c_Color_1);
            surface.BaseColor = (_CelShading_abf7811d76a7413ab3f888509118ab1c_Color_1.xyz);
            return surface;
        }

            // --------------------------------------------------
            // Build Graph Inputs

            VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
        {
            VertexDescriptionInputs output;
            ZERO_INITIALIZE(VertexDescriptionInputs, output);

            output.ObjectSpaceNormal =           input.normalOS;
            output.WorldSpaceNormal =            TransformObjectToWorldNormal(input.normalOS);
            output.ObjectSpaceTangent =          input.tangentOS.xyz;
            output.WorldSpaceTangent =           TransformObjectToWorldDir(input.tangentOS.xyz);
            output.ObjectSpaceBiTangent =        normalize(cross(input.normalOS, input.tangentOS) * (input.tangentOS.w > 0.0f ? 1.0f : -1.0f) * GetOddNegativeScale());
            output.WorldSpaceBiTangent =         TransformObjectToWorldDir(output.ObjectSpaceBiTangent);
            output.ObjectSpacePosition =         input.positionOS;
            output.AbsoluteWorldSpacePosition =  GetAbsolutePositionWS(TransformObjectToWorld(input.positionOS));
            output.uv0 =                         input.uv0;

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
            output.AbsoluteWorldSpacePosition =  GetAbsolutePositionWS(input.positionWS);
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
        Blend One Zero
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
            #define ATTRIBUTES_NEED_NORMAL
            #define ATTRIBUTES_NEED_TANGENT
            #define ATTRIBUTES_NEED_TEXCOORD0
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
            float4 uv0 : TEXCOORD0;
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
            float3 WorldSpaceNormal;
            float3 ObjectSpaceTangent;
            float3 WorldSpaceTangent;
            float3 ObjectSpaceBiTangent;
            float3 WorldSpaceBiTangent;
            float3 ObjectSpacePosition;
            float3 AbsoluteWorldSpacePosition;
            float4 uv0;
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
        float4 _CelShading_abf7811d76a7413ab3f888509118ab1c_Texture2D9666dd2f2f8f4fddb6c2a5c61ddfa9bd_3072536305_TexelSize;
        float4 Texture2D_b9baf54b736a49b8931070640840afe0_TexelSize;
        float4 Texture2D_9689ebada1904c03ba6e4cbb82216610_TexelSize;
        float Vector1_cf0b9f3c26cf40e09df5caa461ffd8b4;
        float4 Texture2D_42a4a1b82a9f4abe96dab75baa1e6491_TexelSize;
        float4 Texture2D_748b7f78c75c42a4bdfc5b775def0238_TexelSize;
        float4 Texture2D_c99ba8a89a0e49c780df6a87923307d3_TexelSize;
        CBUFFER_END

        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(_CelShading_abf7811d76a7413ab3f888509118ab1c_Texture2D9666dd2f2f8f4fddb6c2a5c61ddfa9bd_3072536305);
        SAMPLER(sampler_CelShading_abf7811d76a7413ab3f888509118ab1c_Texture2D9666dd2f2f8f4fddb6c2a5c61ddfa9bd_3072536305);
        TEXTURE2D(Texture2D_b9baf54b736a49b8931070640840afe0);
        SAMPLER(samplerTexture2D_b9baf54b736a49b8931070640840afe0);
        TEXTURE2D(Texture2D_9689ebada1904c03ba6e4cbb82216610);
        SAMPLER(samplerTexture2D_9689ebada1904c03ba6e4cbb82216610);
        TEXTURE2D(Texture2D_42a4a1b82a9f4abe96dab75baa1e6491);
        SAMPLER(samplerTexture2D_42a4a1b82a9f4abe96dab75baa1e6491);
        TEXTURE2D(Texture2D_748b7f78c75c42a4bdfc5b775def0238);
        SAMPLER(samplerTexture2D_748b7f78c75c42a4bdfc5b775def0238);
        TEXTURE2D(Texture2D_c99ba8a89a0e49c780df6a87923307d3);
        SAMPLER(samplerTexture2D_c99ba8a89a0e49c780df6a87923307d3);

            // Graph Functions
            
        void Unity_OneMinus_float(float In, out float Out)
        {
            Out = 1 - In;
        }

        void Unity_Power_float(float A, float B, out float Out)
        {
            Out = pow(A, B);
        }

        void Unity_Multiply_float(float A, float B, out float Out)
        {
            Out = A * B;
        }

        void Unity_Add_float3(float3 A, float3 B, out float3 Out)
        {
            Out = A + B;
        }

        struct Bindings_HeightDisplacement_b9c98ab237e518945923c754d0bbe831
        {
        };

        void SG_HeightDisplacement_b9c98ab237e518945923c754d0bbe831(float3 Vector3_62bf9535e3ff4df7880687e1587ce215, float Vector1_5c5c0f9b27bb4b6d9ea9f75db55607a7, Bindings_HeightDisplacement_b9c98ab237e518945923c754d0bbe831 IN, out float3 OutVector3_1)
        {
            float3 _Property_0f97590798cf4c53838eab4378e02560_Out_0 = Vector3_62bf9535e3ff4df7880687e1587ce215;
            float _Property_49df1d1a54214d609f7a1472d4f9e35e_Out_0 = Vector1_5c5c0f9b27bb4b6d9ea9f75db55607a7;
            float3 _Vector3_1f29590905e646dfbbe4e52d2fba295d_Out_0 = float3(0, _Property_49df1d1a54214d609f7a1472d4f9e35e_Out_0, 0);
            float3 _Add_7997ee2cc82741668225da98195a9cf7_Out_2;
            Unity_Add_float3(_Property_0f97590798cf4c53838eab4378e02560_Out_0, _Vector3_1f29590905e646dfbbe4e52d2fba295d_Out_0, _Add_7997ee2cc82741668225da98195a9cf7_Out_2);
            OutVector3_1 = _Add_7997ee2cc82741668225da98195a9cf7_Out_2;
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
            UnityTexture2D _Property_e05227f480e24de0967ad56ec56f0111_Out_0 = UnityBuildTexture2DStructNoScale(Texture2D_42a4a1b82a9f4abe96dab75baa1e6491);
            #if defined(SHADER_API_GLES) && (SHADER_TARGET < 30)
              float4 _SampleTexture2DLOD_aef31cad67014997ba7b0c99b0a60e32_RGBA_0 = float4(0.0f, 0.0f, 0.0f, 1.0f);
            #else
              float4 _SampleTexture2DLOD_aef31cad67014997ba7b0c99b0a60e32_RGBA_0 = SAMPLE_TEXTURE2D_LOD(_Property_e05227f480e24de0967ad56ec56f0111_Out_0.tex, _Property_e05227f480e24de0967ad56ec56f0111_Out_0.samplerstate, IN.uv0.xy, 0);
            #endif
            float _SampleTexture2DLOD_aef31cad67014997ba7b0c99b0a60e32_R_5 = _SampleTexture2DLOD_aef31cad67014997ba7b0c99b0a60e32_RGBA_0.r;
            float _SampleTexture2DLOD_aef31cad67014997ba7b0c99b0a60e32_G_6 = _SampleTexture2DLOD_aef31cad67014997ba7b0c99b0a60e32_RGBA_0.g;
            float _SampleTexture2DLOD_aef31cad67014997ba7b0c99b0a60e32_B_7 = _SampleTexture2DLOD_aef31cad67014997ba7b0c99b0a60e32_RGBA_0.b;
            float _SampleTexture2DLOD_aef31cad67014997ba7b0c99b0a60e32_A_8 = _SampleTexture2DLOD_aef31cad67014997ba7b0c99b0a60e32_RGBA_0.a;
            UnityTexture2D _Property_cebacde6ccf84fd2beb8e37538e75b15_Out_0 = UnityBuildTexture2DStructNoScale(Texture2D_9689ebada1904c03ba6e4cbb82216610);
            #if defined(SHADER_API_GLES) && (SHADER_TARGET < 30)
              float4 _SampleTexture2DLOD_6becb53decd44143a598717795ab9611_RGBA_0 = float4(0.0f, 0.0f, 0.0f, 1.0f);
            #else
              float4 _SampleTexture2DLOD_6becb53decd44143a598717795ab9611_RGBA_0 = SAMPLE_TEXTURE2D_LOD(_Property_cebacde6ccf84fd2beb8e37538e75b15_Out_0.tex, _Property_cebacde6ccf84fd2beb8e37538e75b15_Out_0.samplerstate, IN.uv0.xy, 0);
            #endif
            float _SampleTexture2DLOD_6becb53decd44143a598717795ab9611_R_5 = _SampleTexture2DLOD_6becb53decd44143a598717795ab9611_RGBA_0.r;
            float _SampleTexture2DLOD_6becb53decd44143a598717795ab9611_G_6 = _SampleTexture2DLOD_6becb53decd44143a598717795ab9611_RGBA_0.g;
            float _SampleTexture2DLOD_6becb53decd44143a598717795ab9611_B_7 = _SampleTexture2DLOD_6becb53decd44143a598717795ab9611_RGBA_0.b;
            float _SampleTexture2DLOD_6becb53decd44143a598717795ab9611_A_8 = _SampleTexture2DLOD_6becb53decd44143a598717795ab9611_RGBA_0.a;
            float _OneMinus_d9bc4f62005242809b4528e86e1f3ef6_Out_1;
            Unity_OneMinus_float(_SampleTexture2DLOD_6becb53decd44143a598717795ab9611_R_5, _OneMinus_d9bc4f62005242809b4528e86e1f3ef6_Out_1);
            float _Power_937eb875940743b38b2ed441b457bdea_Out_2;
            Unity_Power_float(_OneMinus_d9bc4f62005242809b4528e86e1f3ef6_Out_1, 2, _Power_937eb875940743b38b2ed441b457bdea_Out_2);
            float _OneMinus_c2d552dce35d42ba835e5ee3ebf2c892_Out_1;
            Unity_OneMinus_float(_Power_937eb875940743b38b2ed441b457bdea_Out_2, _OneMinus_c2d552dce35d42ba835e5ee3ebf2c892_Out_1);
            float _Multiply_30256bfbfe7241d8912131f5e6de9988_Out_2;
            Unity_Multiply_float(_SampleTexture2DLOD_aef31cad67014997ba7b0c99b0a60e32_R_5, _OneMinus_c2d552dce35d42ba835e5ee3ebf2c892_Out_1, _Multiply_30256bfbfe7241d8912131f5e6de9988_Out_2);
            float _Property_d78f7cc8b639449aa812de1783598203_Out_0 = Vector1_cf0b9f3c26cf40e09df5caa461ffd8b4;
            float _Multiply_fa333bf6fddb4079afa8561e7f1ef4b0_Out_2;
            Unity_Multiply_float(_Multiply_30256bfbfe7241d8912131f5e6de9988_Out_2, _Property_d78f7cc8b639449aa812de1783598203_Out_0, _Multiply_fa333bf6fddb4079afa8561e7f1ef4b0_Out_2);
            Bindings_HeightDisplacement_b9c98ab237e518945923c754d0bbe831 _HeightDisplacement_1c8734ff1d9f46e0bb73e04b3d1d2d38;
            float3 _HeightDisplacement_1c8734ff1d9f46e0bb73e04b3d1d2d38_OutVector3_1;
            SG_HeightDisplacement_b9c98ab237e518945923c754d0bbe831(IN.AbsoluteWorldSpacePosition, _Multiply_fa333bf6fddb4079afa8561e7f1ef4b0_Out_2, _HeightDisplacement_1c8734ff1d9f46e0bb73e04b3d1d2d38, _HeightDisplacement_1c8734ff1d9f46e0bb73e04b3d1d2d38_OutVector3_1);
            float3 _Transform_37a6f3b78e9a4eb5898cade582e4ccd0_Out_1 = TransformWorldToObject(GetCameraRelativePositionWS(_HeightDisplacement_1c8734ff1d9f46e0bb73e04b3d1d2d38_OutVector3_1.xyz));
            description.Position = _Transform_37a6f3b78e9a4eb5898cade582e4ccd0_Out_1;
            description.Normal = IN.ObjectSpaceNormal;
            description.Tangent = IN.ObjectSpaceTangent;
            return description;
        }

            // Graph Pixel
            struct SurfaceDescription
        {
        };

        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            return surface;
        }

            // --------------------------------------------------
            // Build Graph Inputs

            VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
        {
            VertexDescriptionInputs output;
            ZERO_INITIALIZE(VertexDescriptionInputs, output);

            output.ObjectSpaceNormal =           input.normalOS;
            output.WorldSpaceNormal =            TransformObjectToWorldNormal(input.normalOS);
            output.ObjectSpaceTangent =          input.tangentOS.xyz;
            output.WorldSpaceTangent =           TransformObjectToWorldDir(input.tangentOS.xyz);
            output.ObjectSpaceBiTangent =        normalize(cross(input.normalOS, input.tangentOS) * (input.tangentOS.w > 0.0f ? 1.0f : -1.0f) * GetOddNegativeScale());
            output.WorldSpaceBiTangent =         TransformObjectToWorldDir(output.ObjectSpaceBiTangent);
            output.ObjectSpacePosition =         input.positionOS;
            output.AbsoluteWorldSpacePosition =  GetAbsolutePositionWS(TransformObjectToWorld(input.positionOS));
            output.uv0 =                         input.uv0;

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
        Blend One Zero
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
            #define ATTRIBUTES_NEED_NORMAL
            #define ATTRIBUTES_NEED_TANGENT
            #define ATTRIBUTES_NEED_TEXCOORD0
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
            float4 uv0 : TEXCOORD0;
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
            float3 WorldSpaceNormal;
            float3 ObjectSpaceTangent;
            float3 WorldSpaceTangent;
            float3 ObjectSpaceBiTangent;
            float3 WorldSpaceBiTangent;
            float3 ObjectSpacePosition;
            float3 AbsoluteWorldSpacePosition;
            float4 uv0;
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
        float4 _CelShading_abf7811d76a7413ab3f888509118ab1c_Texture2D9666dd2f2f8f4fddb6c2a5c61ddfa9bd_3072536305_TexelSize;
        float4 Texture2D_b9baf54b736a49b8931070640840afe0_TexelSize;
        float4 Texture2D_9689ebada1904c03ba6e4cbb82216610_TexelSize;
        float Vector1_cf0b9f3c26cf40e09df5caa461ffd8b4;
        float4 Texture2D_42a4a1b82a9f4abe96dab75baa1e6491_TexelSize;
        float4 Texture2D_748b7f78c75c42a4bdfc5b775def0238_TexelSize;
        float4 Texture2D_c99ba8a89a0e49c780df6a87923307d3_TexelSize;
        CBUFFER_END

        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(_CelShading_abf7811d76a7413ab3f888509118ab1c_Texture2D9666dd2f2f8f4fddb6c2a5c61ddfa9bd_3072536305);
        SAMPLER(sampler_CelShading_abf7811d76a7413ab3f888509118ab1c_Texture2D9666dd2f2f8f4fddb6c2a5c61ddfa9bd_3072536305);
        TEXTURE2D(Texture2D_b9baf54b736a49b8931070640840afe0);
        SAMPLER(samplerTexture2D_b9baf54b736a49b8931070640840afe0);
        TEXTURE2D(Texture2D_9689ebada1904c03ba6e4cbb82216610);
        SAMPLER(samplerTexture2D_9689ebada1904c03ba6e4cbb82216610);
        TEXTURE2D(Texture2D_42a4a1b82a9f4abe96dab75baa1e6491);
        SAMPLER(samplerTexture2D_42a4a1b82a9f4abe96dab75baa1e6491);
        TEXTURE2D(Texture2D_748b7f78c75c42a4bdfc5b775def0238);
        SAMPLER(samplerTexture2D_748b7f78c75c42a4bdfc5b775def0238);
        TEXTURE2D(Texture2D_c99ba8a89a0e49c780df6a87923307d3);
        SAMPLER(samplerTexture2D_c99ba8a89a0e49c780df6a87923307d3);

            // Graph Functions
            
        void Unity_OneMinus_float(float In, out float Out)
        {
            Out = 1 - In;
        }

        void Unity_Power_float(float A, float B, out float Out)
        {
            Out = pow(A, B);
        }

        void Unity_Multiply_float(float A, float B, out float Out)
        {
            Out = A * B;
        }

        void Unity_Add_float3(float3 A, float3 B, out float3 Out)
        {
            Out = A + B;
        }

        struct Bindings_HeightDisplacement_b9c98ab237e518945923c754d0bbe831
        {
        };

        void SG_HeightDisplacement_b9c98ab237e518945923c754d0bbe831(float3 Vector3_62bf9535e3ff4df7880687e1587ce215, float Vector1_5c5c0f9b27bb4b6d9ea9f75db55607a7, Bindings_HeightDisplacement_b9c98ab237e518945923c754d0bbe831 IN, out float3 OutVector3_1)
        {
            float3 _Property_0f97590798cf4c53838eab4378e02560_Out_0 = Vector3_62bf9535e3ff4df7880687e1587ce215;
            float _Property_49df1d1a54214d609f7a1472d4f9e35e_Out_0 = Vector1_5c5c0f9b27bb4b6d9ea9f75db55607a7;
            float3 _Vector3_1f29590905e646dfbbe4e52d2fba295d_Out_0 = float3(0, _Property_49df1d1a54214d609f7a1472d4f9e35e_Out_0, 0);
            float3 _Add_7997ee2cc82741668225da98195a9cf7_Out_2;
            Unity_Add_float3(_Property_0f97590798cf4c53838eab4378e02560_Out_0, _Vector3_1f29590905e646dfbbe4e52d2fba295d_Out_0, _Add_7997ee2cc82741668225da98195a9cf7_Out_2);
            OutVector3_1 = _Add_7997ee2cc82741668225da98195a9cf7_Out_2;
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
            UnityTexture2D _Property_e05227f480e24de0967ad56ec56f0111_Out_0 = UnityBuildTexture2DStructNoScale(Texture2D_42a4a1b82a9f4abe96dab75baa1e6491);
            #if defined(SHADER_API_GLES) && (SHADER_TARGET < 30)
              float4 _SampleTexture2DLOD_aef31cad67014997ba7b0c99b0a60e32_RGBA_0 = float4(0.0f, 0.0f, 0.0f, 1.0f);
            #else
              float4 _SampleTexture2DLOD_aef31cad67014997ba7b0c99b0a60e32_RGBA_0 = SAMPLE_TEXTURE2D_LOD(_Property_e05227f480e24de0967ad56ec56f0111_Out_0.tex, _Property_e05227f480e24de0967ad56ec56f0111_Out_0.samplerstate, IN.uv0.xy, 0);
            #endif
            float _SampleTexture2DLOD_aef31cad67014997ba7b0c99b0a60e32_R_5 = _SampleTexture2DLOD_aef31cad67014997ba7b0c99b0a60e32_RGBA_0.r;
            float _SampleTexture2DLOD_aef31cad67014997ba7b0c99b0a60e32_G_6 = _SampleTexture2DLOD_aef31cad67014997ba7b0c99b0a60e32_RGBA_0.g;
            float _SampleTexture2DLOD_aef31cad67014997ba7b0c99b0a60e32_B_7 = _SampleTexture2DLOD_aef31cad67014997ba7b0c99b0a60e32_RGBA_0.b;
            float _SampleTexture2DLOD_aef31cad67014997ba7b0c99b0a60e32_A_8 = _SampleTexture2DLOD_aef31cad67014997ba7b0c99b0a60e32_RGBA_0.a;
            UnityTexture2D _Property_cebacde6ccf84fd2beb8e37538e75b15_Out_0 = UnityBuildTexture2DStructNoScale(Texture2D_9689ebada1904c03ba6e4cbb82216610);
            #if defined(SHADER_API_GLES) && (SHADER_TARGET < 30)
              float4 _SampleTexture2DLOD_6becb53decd44143a598717795ab9611_RGBA_0 = float4(0.0f, 0.0f, 0.0f, 1.0f);
            #else
              float4 _SampleTexture2DLOD_6becb53decd44143a598717795ab9611_RGBA_0 = SAMPLE_TEXTURE2D_LOD(_Property_cebacde6ccf84fd2beb8e37538e75b15_Out_0.tex, _Property_cebacde6ccf84fd2beb8e37538e75b15_Out_0.samplerstate, IN.uv0.xy, 0);
            #endif
            float _SampleTexture2DLOD_6becb53decd44143a598717795ab9611_R_5 = _SampleTexture2DLOD_6becb53decd44143a598717795ab9611_RGBA_0.r;
            float _SampleTexture2DLOD_6becb53decd44143a598717795ab9611_G_6 = _SampleTexture2DLOD_6becb53decd44143a598717795ab9611_RGBA_0.g;
            float _SampleTexture2DLOD_6becb53decd44143a598717795ab9611_B_7 = _SampleTexture2DLOD_6becb53decd44143a598717795ab9611_RGBA_0.b;
            float _SampleTexture2DLOD_6becb53decd44143a598717795ab9611_A_8 = _SampleTexture2DLOD_6becb53decd44143a598717795ab9611_RGBA_0.a;
            float _OneMinus_d9bc4f62005242809b4528e86e1f3ef6_Out_1;
            Unity_OneMinus_float(_SampleTexture2DLOD_6becb53decd44143a598717795ab9611_R_5, _OneMinus_d9bc4f62005242809b4528e86e1f3ef6_Out_1);
            float _Power_937eb875940743b38b2ed441b457bdea_Out_2;
            Unity_Power_float(_OneMinus_d9bc4f62005242809b4528e86e1f3ef6_Out_1, 2, _Power_937eb875940743b38b2ed441b457bdea_Out_2);
            float _OneMinus_c2d552dce35d42ba835e5ee3ebf2c892_Out_1;
            Unity_OneMinus_float(_Power_937eb875940743b38b2ed441b457bdea_Out_2, _OneMinus_c2d552dce35d42ba835e5ee3ebf2c892_Out_1);
            float _Multiply_30256bfbfe7241d8912131f5e6de9988_Out_2;
            Unity_Multiply_float(_SampleTexture2DLOD_aef31cad67014997ba7b0c99b0a60e32_R_5, _OneMinus_c2d552dce35d42ba835e5ee3ebf2c892_Out_1, _Multiply_30256bfbfe7241d8912131f5e6de9988_Out_2);
            float _Property_d78f7cc8b639449aa812de1783598203_Out_0 = Vector1_cf0b9f3c26cf40e09df5caa461ffd8b4;
            float _Multiply_fa333bf6fddb4079afa8561e7f1ef4b0_Out_2;
            Unity_Multiply_float(_Multiply_30256bfbfe7241d8912131f5e6de9988_Out_2, _Property_d78f7cc8b639449aa812de1783598203_Out_0, _Multiply_fa333bf6fddb4079afa8561e7f1ef4b0_Out_2);
            Bindings_HeightDisplacement_b9c98ab237e518945923c754d0bbe831 _HeightDisplacement_1c8734ff1d9f46e0bb73e04b3d1d2d38;
            float3 _HeightDisplacement_1c8734ff1d9f46e0bb73e04b3d1d2d38_OutVector3_1;
            SG_HeightDisplacement_b9c98ab237e518945923c754d0bbe831(IN.AbsoluteWorldSpacePosition, _Multiply_fa333bf6fddb4079afa8561e7f1ef4b0_Out_2, _HeightDisplacement_1c8734ff1d9f46e0bb73e04b3d1d2d38, _HeightDisplacement_1c8734ff1d9f46e0bb73e04b3d1d2d38_OutVector3_1);
            float3 _Transform_37a6f3b78e9a4eb5898cade582e4ccd0_Out_1 = TransformWorldToObject(GetCameraRelativePositionWS(_HeightDisplacement_1c8734ff1d9f46e0bb73e04b3d1d2d38_OutVector3_1.xyz));
            description.Position = _Transform_37a6f3b78e9a4eb5898cade582e4ccd0_Out_1;
            description.Normal = IN.ObjectSpaceNormal;
            description.Tangent = IN.ObjectSpaceTangent;
            return description;
        }

            // Graph Pixel
            struct SurfaceDescription
        {
        };

        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            return surface;
        }

            // --------------------------------------------------
            // Build Graph Inputs

            VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
        {
            VertexDescriptionInputs output;
            ZERO_INITIALIZE(VertexDescriptionInputs, output);

            output.ObjectSpaceNormal =           input.normalOS;
            output.WorldSpaceNormal =            TransformObjectToWorldNormal(input.normalOS);
            output.ObjectSpaceTangent =          input.tangentOS.xyz;
            output.WorldSpaceTangent =           TransformObjectToWorldDir(input.tangentOS.xyz);
            output.ObjectSpaceBiTangent =        normalize(cross(input.normalOS, input.tangentOS) * (input.tangentOS.w > 0.0f ? 1.0f : -1.0f) * GetOddNegativeScale());
            output.WorldSpaceBiTangent =         TransformObjectToWorldDir(output.ObjectSpaceBiTangent);
            output.ObjectSpacePosition =         input.positionOS;
            output.AbsoluteWorldSpacePosition =  GetAbsolutePositionWS(TransformObjectToWorld(input.positionOS));
            output.uv0 =                         input.uv0;

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