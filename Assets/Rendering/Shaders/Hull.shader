Shader "CelShaded/Hull"
{
    Properties
    {
        [NoScaleOffset]Texture2D_6081c378ead841c69abfc7b2580254ff("Main Texture", 2D) = "white" {}
        Vector1_70a0f58dca404ff6b453838771b82aef("Height", Float) = 0
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
            "Queue"="AlphaTest"
        }
        Pass
        {
            Name "Pass"
            Tags
            {
                // LightMode: <None>
            }

            // Render State
            Cull Off
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



            // Defines
            #define _AlphaClip 1
            #define ATTRIBUTES_NEED_NORMAL
            #define ATTRIBUTES_NEED_TANGENT
            #define ATTRIBUTES_NEED_TEXCOORD0
            #define VARYINGS_NEED_POSITION_WS
            #define VARYINGS_NEED_NORMAL_WS
            #define VARYINGS_NEED_TANGENT_WS
            #define VARYINGS_NEED_TEXCOORD0
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
            float3 AbsoluteWorldSpacePosition;
            float4 uv0;
            float3 TimeParameters;
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
        float4 Texture2D_6081c378ead841c69abfc7b2580254ff_TexelSize;
        float Vector1_70a0f58dca404ff6b453838771b82aef;
        CBUFFER_END

        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(Texture2D_6081c378ead841c69abfc7b2580254ff);
        SAMPLER(samplerTexture2D_6081c378ead841c69abfc7b2580254ff);

            // Graph Functions
            
        void Unity_Normalize_float3(float3 In, out float3 Out)
        {
            Out = normalize(In);
        }

        void Unity_Multiply_float(float3 A, float3 B, out float3 Out)
        {
            Out = A * B;
        }

        void Unity_Add_float3(float3 A, float3 B, out float3 Out)
        {
            Out = A + B;
        }

        // 9d45c41649cfe89fdaaa688721614302
        #include "Assets/Rendering/Shaders/Functions/ShaderGraphCel.hlsl"

        struct Bindings_SimpleCelShading_2e4e5e58df2ab7843bdc75ee94394506
        {
            float3 WorldSpaceNormal;
            float3 WorldSpaceTangent;
            float3 WorldSpaceBiTangent;
            float3 AbsoluteWorldSpacePosition;
        };

        void SG_SimpleCelShading_2e4e5e58df2ab7843bdc75ee94394506(float4 Color_a950a8e798b94a5aade2710e3fcfde01, float3 Vector3_26519f981d2f4fc0957e66b16a8bc951, Bindings_SimpleCelShading_2e4e5e58df2ab7843bdc75ee94394506 IN, out half4 Color_1)
        {
            float4 _Property_61f5588b2cd94abebf90def44b873f56_Out_0 = Color_a950a8e798b94a5aade2710e3fcfde01;
            float3 _Property_b297331139c44f59939aac58082c32cd_Out_0 = Vector3_26519f981d2f4fc0957e66b16a8bc951;
            float3x3 Transform_1e7abfa1ecca4936a05f1180040b4744_transposeTangent = transpose(float3x3(IN.WorldSpaceTangent, IN.WorldSpaceBiTangent, IN.WorldSpaceNormal));
            float3 _Transform_1e7abfa1ecca4936a05f1180040b4744_Out_1 = normalize(mul(Transform_1e7abfa1ecca4936a05f1180040b4744_transposeTangent, _Property_b297331139c44f59939aac58082c32cd_Out_0.xyz).xyz);
            float3 _Add_29b1cebd4c974883b111230a3e9dae3f_Out_2;
            Unity_Add_float3(IN.WorldSpaceNormal, _Transform_1e7abfa1ecca4936a05f1180040b4744_Out_1, _Add_29b1cebd4c974883b111230a3e9dae3f_Out_2);
            float3 _Normalize_8a60e1ba11d148d0b782a9efe42f72aa_Out_1;
            Unity_Normalize_float3(_Add_29b1cebd4c974883b111230a3e9dae3f_Out_2, _Normalize_8a60e1ba11d148d0b782a9efe42f72aa_Out_1);
            half4 _SimpleCelLightingCustomFunction_2513510943474503a09a9bf6a58817e3_CelShaded_4;
            SimpleCelLighting_half(_Property_61f5588b2cd94abebf90def44b873f56_Out_0, IN.AbsoluteWorldSpacePosition, _Normalize_8a60e1ba11d148d0b782a9efe42f72aa_Out_1, _SimpleCelLightingCustomFunction_2513510943474503a09a9bf6a58817e3_CelShaded_4);
            Color_1 = _SimpleCelLightingCustomFunction_2513510943474503a09a9bf6a58817e3_CelShaded_4;
        }

        void Unity_Saturation_float(float3 In, float Saturation, out float3 Out)
        {
            float luma = dot(In, float3(0.2126729, 0.7151522, 0.0721750));
            Out =  luma.xxx + Saturation.xxx * (In - luma.xxx);
        }

        void Unity_Branch_float3(float Predicate, float3 True, float3 False, out float3 Out)
        {
            Out = Predicate ? True : False;
        }

        struct Bindings_Emission_1c7d3173c45db194bad26a66d6783e8c
        {
        };

        void SG_Emission_1c7d3173c45db194bad26a66d6783e8c(float3 Vector3_4893f82ce4ad4694bf20d3fd06b823a5, float4 Vector4_ec38e529f0df46649fe820c0c5f9bc51, float Vector1_5419bf8509d0445abafd071804949f7a, float Vector1_7d09b510b5864ccd9af8c7022e938a6b, float Boolean_c20b7d2f2312469b9e918a77ac66535b, Bindings_Emission_1c7d3173c45db194bad26a66d6783e8c IN, out float3 FinalColor_1)
        {
            float _Property_a72137ffc2824be6909477c05e3fd560_Out_0 = Boolean_c20b7d2f2312469b9e918a77ac66535b;
            float3 _Property_1eb5e58f96fe45e3b3965b7bd4f7f2ec_Out_0 = Vector3_4893f82ce4ad4694bf20d3fd06b823a5;
            float4 _Property_59cc7de5f6e946b991d0baac2f5639ed_Out_0 = Vector4_ec38e529f0df46649fe820c0c5f9bc51;
            float3 _Multiply_179269d6d4784df7a0c42a3f62f19a9c_Out_2;
            Unity_Multiply_float(_Property_1eb5e58f96fe45e3b3965b7bd4f7f2ec_Out_0, (_Property_59cc7de5f6e946b991d0baac2f5639ed_Out_0.xyz), _Multiply_179269d6d4784df7a0c42a3f62f19a9c_Out_2);
            float _Property_8072e791bf0e459eb93f2c56bca7896a_Out_0 = Vector1_5419bf8509d0445abafd071804949f7a;
            float3 _Multiply_48d1716aae5c45958b7bac29d64fea73_Out_2;
            Unity_Multiply_float(_Multiply_179269d6d4784df7a0c42a3f62f19a9c_Out_2, (_Property_8072e791bf0e459eb93f2c56bca7896a_Out_0.xxx), _Multiply_48d1716aae5c45958b7bac29d64fea73_Out_2);
            float _Property_02d5f7f6da0d4522a3cbad88b0f01592_Out_0 = Vector1_7d09b510b5864ccd9af8c7022e938a6b;
            float3 _Saturation_bf98812e88e24b1caab54dff934c2d56_Out_2;
            Unity_Saturation_float(_Multiply_48d1716aae5c45958b7bac29d64fea73_Out_2, _Property_02d5f7f6da0d4522a3cbad88b0f01592_Out_0, _Saturation_bf98812e88e24b1caab54dff934c2d56_Out_2);
            float3 _Branch_0c1d61324e704f1799e76d84ca1b9dba_Out_3;
            Unity_Branch_float3(_Property_a72137ffc2824be6909477c05e3fd560_Out_0, _Saturation_bf98812e88e24b1caab54dff934c2d56_Out_2, _Property_1eb5e58f96fe45e3b3965b7bd4f7f2ec_Out_0, _Branch_0c1d61324e704f1799e76d84ca1b9dba_Out_3);
            FinalColor_1 = _Branch_0c1d61324e704f1799e76d84ca1b9dba_Out_3;
        }

        void Unity_Multiply_float(float A, float B, out float Out)
        {
            Out = A * B;
        }

        void Unity_Sine_float(float In, out float Out)
        {
            Out = sin(In);
        }

        void Unity_Cosine_float(float In, out float Out)
        {
            Out = cos(In);
        }

        void Unity_Combine_float(float R, float G, float B, float A, out float4 RGBA, out float3 RGB, out float2 RG)
        {
            RGBA = float4(R, G, B, A);
            RGB = float3(R, G, B);
            RG = float2(R, G);
        }

        void Unity_Add_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A + B;
        }


        float2 Unity_GradientNoise_Dir_float(float2 p)
        {
            // Permutation and hashing used in webgl-nosie goo.gl/pX7HtC
            p = p % 289;
            // need full precision, otherwise half overflows when p > 1
            float x = float(34 * p.x + 1) * p.x % 289 + p.y;
            x = (34 * x + 1) * x % 289;
            x = frac(x / 41) * 2 - 1;
            return normalize(float2(x - floor(x + 0.5), abs(x) - 0.5));
        }

        void Unity_GradientNoise_float(float2 UV, float Scale, out float Out)
        { 
            float2 p = UV * Scale;
            float2 ip = floor(p);
            float2 fp = frac(p);
            float d00 = dot(Unity_GradientNoise_Dir_float(ip), fp);
            float d01 = dot(Unity_GradientNoise_Dir_float(ip + float2(0, 1)), fp - float2(0, 1));
            float d10 = dot(Unity_GradientNoise_Dir_float(ip + float2(1, 0)), fp - float2(1, 0));
            float d11 = dot(Unity_GradientNoise_Dir_float(ip + float2(1, 1)), fp - float2(1, 1));
            fp = fp * fp * fp * (fp * (fp * 6 - 15) + 10);
            Out = lerp(lerp(d00, d01, fp.y), lerp(d10, d11, fp.y), fp.x) + 0.5;
        }

        void Unity_Saturate_float(float In, out float Out)
        {
            Out = saturate(In);
        }

        void Unity_Add_float(float A, float B, out float Out)
        {
            Out = A + B;
        }

        void Unity_Remap_float(float In, float2 InMinMax, float2 OutMinMax, out float Out)
        {
            Out = OutMinMax.x + (In - InMinMax.x) * (OutMinMax.y - OutMinMax.x) / (InMinMax.y - InMinMax.x);
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
            float3 _Normalize_3d6977d166e4483cb4ffbefbaf6f182a_Out_1;
            Unity_Normalize_float3(IN.ObjectSpaceNormal, _Normalize_3d6977d166e4483cb4ffbefbaf6f182a_Out_1);
            float _Property_aa9bd97b261642189ab4089656b56996_Out_0 = Vector1_70a0f58dca404ff6b453838771b82aef;
            float3 _Multiply_97e8d083b3b04a15862fb2dec2e8b0bb_Out_2;
            Unity_Multiply_float(_Normalize_3d6977d166e4483cb4ffbefbaf6f182a_Out_1, (_Property_aa9bd97b261642189ab4089656b56996_Out_0.xxx), _Multiply_97e8d083b3b04a15862fb2dec2e8b0bb_Out_2);
            float3 _Add_e4cea26ead64410da28ae26eb24d9c3c_Out_2;
            Unity_Add_float3(_Multiply_97e8d083b3b04a15862fb2dec2e8b0bb_Out_2, IN.ObjectSpacePosition, _Add_e4cea26ead64410da28ae26eb24d9c3c_Out_2);
            description.Position = _Add_e4cea26ead64410da28ae26eb24d9c3c_Out_2;
            description.Normal = IN.ObjectSpaceNormal;
            description.Tangent = IN.ObjectSpaceTangent;
            return description;
        }

            // Graph Pixel
            struct SurfaceDescription
        {
            float3 BaseColor;
            float Alpha;
            float AlphaClipThreshold;
        };

        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            UnityTexture2D _Property_c38376267b5344bc98c0d6506235c886_Out_0 = UnityBuildTexture2DStructNoScale(Texture2D_6081c378ead841c69abfc7b2580254ff);
            float4 _SampleTexture2D_2fa0a4234d564367872a87bbaf90a2d2_RGBA_0 = SAMPLE_TEXTURE2D(_Property_c38376267b5344bc98c0d6506235c886_Out_0.tex, _Property_c38376267b5344bc98c0d6506235c886_Out_0.samplerstate, IN.uv0.xy);
            float _SampleTexture2D_2fa0a4234d564367872a87bbaf90a2d2_R_4 = _SampleTexture2D_2fa0a4234d564367872a87bbaf90a2d2_RGBA_0.r;
            float _SampleTexture2D_2fa0a4234d564367872a87bbaf90a2d2_G_5 = _SampleTexture2D_2fa0a4234d564367872a87bbaf90a2d2_RGBA_0.g;
            float _SampleTexture2D_2fa0a4234d564367872a87bbaf90a2d2_B_6 = _SampleTexture2D_2fa0a4234d564367872a87bbaf90a2d2_RGBA_0.b;
            float _SampleTexture2D_2fa0a4234d564367872a87bbaf90a2d2_A_7 = _SampleTexture2D_2fa0a4234d564367872a87bbaf90a2d2_RGBA_0.a;
            Bindings_SimpleCelShading_2e4e5e58df2ab7843bdc75ee94394506 _SimpleCelShading_edc118fd0c5a49cbb04404ac8b56a35b;
            _SimpleCelShading_edc118fd0c5a49cbb04404ac8b56a35b.WorldSpaceNormal = IN.WorldSpaceNormal;
            _SimpleCelShading_edc118fd0c5a49cbb04404ac8b56a35b.WorldSpaceTangent = IN.WorldSpaceTangent;
            _SimpleCelShading_edc118fd0c5a49cbb04404ac8b56a35b.WorldSpaceBiTangent = IN.WorldSpaceBiTangent;
            _SimpleCelShading_edc118fd0c5a49cbb04404ac8b56a35b.AbsoluteWorldSpacePosition = IN.AbsoluteWorldSpacePosition;
            half4 _SimpleCelShading_edc118fd0c5a49cbb04404ac8b56a35b_Color_1;
            SG_SimpleCelShading_2e4e5e58df2ab7843bdc75ee94394506(_SampleTexture2D_2fa0a4234d564367872a87bbaf90a2d2_RGBA_0, float3 (0, 0, 1), _SimpleCelShading_edc118fd0c5a49cbb04404ac8b56a35b, _SimpleCelShading_edc118fd0c5a49cbb04404ac8b56a35b_Color_1);
            Bindings_Emission_1c7d3173c45db194bad26a66d6783e8c _Emission_d3aedfcd2a82460b815bcf7e42cd7a56;
            float3 _Emission_d3aedfcd2a82460b815bcf7e42cd7a56_FinalColor_1;
            SG_Emission_1c7d3173c45db194bad26a66d6783e8c((_SimpleCelShading_edc118fd0c5a49cbb04404ac8b56a35b_Color_1.xyz), _SimpleCelShading_edc118fd0c5a49cbb04404ac8b56a35b_Color_1, 5, 1, 1, _Emission_d3aedfcd2a82460b815bcf7e42cd7a56, _Emission_d3aedfcd2a82460b815bcf7e42cd7a56_FinalColor_1);
            float4 _UV_0ec55d615f80432f9acd85d11a647175_Out_0 = IN.uv0;
            float _Multiply_6c611459d6994525940bf5f0f92be31e_Out_2;
            Unity_Multiply_float(IN.TimeParameters.x, 0.1, _Multiply_6c611459d6994525940bf5f0f92be31e_Out_2);
            float _Sine_573fae1d0e6c4b2b94775100bc259ce6_Out_1;
            Unity_Sine_float(_Multiply_6c611459d6994525940bf5f0f92be31e_Out_2, _Sine_573fae1d0e6c4b2b94775100bc259ce6_Out_1);
            float _Cosine_edf7bb825cbb4945a0cf937e609c4664_Out_1;
            Unity_Cosine_float(_Multiply_6c611459d6994525940bf5f0f92be31e_Out_2, _Cosine_edf7bb825cbb4945a0cf937e609c4664_Out_1);
            float4 _Combine_31428d23518a41ecac91acbf64be6544_RGBA_4;
            float3 _Combine_31428d23518a41ecac91acbf64be6544_RGB_5;
            float2 _Combine_31428d23518a41ecac91acbf64be6544_RG_6;
            Unity_Combine_float(_Sine_573fae1d0e6c4b2b94775100bc259ce6_Out_1, _Cosine_edf7bb825cbb4945a0cf937e609c4664_Out_1, 0, 0, _Combine_31428d23518a41ecac91acbf64be6544_RGBA_4, _Combine_31428d23518a41ecac91acbf64be6544_RGB_5, _Combine_31428d23518a41ecac91acbf64be6544_RG_6);
            float2 _Add_9a5030da31ca49ebb7ea22a73b786533_Out_2;
            Unity_Add_float2((_UV_0ec55d615f80432f9acd85d11a647175_Out_0.xy), _Combine_31428d23518a41ecac91acbf64be6544_RG_6, _Add_9a5030da31ca49ebb7ea22a73b786533_Out_2);
            float _GradientNoise_11338b0d122b4fc8ae9fa994f4ed5b9c_Out_2;
            Unity_GradientNoise_float(_Add_9a5030da31ca49ebb7ea22a73b786533_Out_2, 4, _GradientNoise_11338b0d122b4fc8ae9fa994f4ed5b9c_Out_2);
            float4 _Combine_66b0057efd814cd69d228d40d50ee8ec_RGBA_4;
            float3 _Combine_66b0057efd814cd69d228d40d50ee8ec_RGB_5;
            float2 _Combine_66b0057efd814cd69d228d40d50ee8ec_RG_6;
            Unity_Combine_float(_Cosine_edf7bb825cbb4945a0cf937e609c4664_Out_1, _Sine_573fae1d0e6c4b2b94775100bc259ce6_Out_1, 0, 0, _Combine_66b0057efd814cd69d228d40d50ee8ec_RGBA_4, _Combine_66b0057efd814cd69d228d40d50ee8ec_RGB_5, _Combine_66b0057efd814cd69d228d40d50ee8ec_RG_6);
            float2 _Add_892d58b07f16404c8968390dd0cbf841_Out_2;
            Unity_Add_float2((_UV_0ec55d615f80432f9acd85d11a647175_Out_0.xy), _Combine_66b0057efd814cd69d228d40d50ee8ec_RG_6, _Add_892d58b07f16404c8968390dd0cbf841_Out_2);
            float _GradientNoise_8134e22a484e442ba95b6ef504ae609e_Out_2;
            Unity_GradientNoise_float(_Add_892d58b07f16404c8968390dd0cbf841_Out_2, 10, _GradientNoise_8134e22a484e442ba95b6ef504ae609e_Out_2);
            float _Multiply_652f527d8cc24dee97852321f23c9d40_Out_2;
            Unity_Multiply_float(_GradientNoise_11338b0d122b4fc8ae9fa994f4ed5b9c_Out_2, _GradientNoise_8134e22a484e442ba95b6ef504ae609e_Out_2, _Multiply_652f527d8cc24dee97852321f23c9d40_Out_2);
            float _GradientNoise_8c21d761530f466fb0b8e4f3e494e745_Out_2;
            Unity_GradientNoise_float(_Add_9a5030da31ca49ebb7ea22a73b786533_Out_2, 30, _GradientNoise_8c21d761530f466fb0b8e4f3e494e745_Out_2);
            float _Multiply_8fc43bbb0d464facafaa453b7b12898c_Out_2;
            Unity_Multiply_float(_Multiply_652f527d8cc24dee97852321f23c9d40_Out_2, _GradientNoise_8c21d761530f466fb0b8e4f3e494e745_Out_2, _Multiply_8fc43bbb0d464facafaa453b7b12898c_Out_2);
            float _Multiply_d8dfe8dac074418a86cd52de04df5b02_Out_2;
            Unity_Multiply_float(_Multiply_8fc43bbb0d464facafaa453b7b12898c_Out_2, 8, _Multiply_d8dfe8dac074418a86cd52de04df5b02_Out_2);
            float _Saturate_870dd07a1aeb4ab082f61cb28ff3aef1_Out_1;
            Unity_Saturate_float(_Multiply_d8dfe8dac074418a86cd52de04df5b02_Out_2, _Saturate_870dd07a1aeb4ab082f61cb28ff3aef1_Out_1);
            float4 _UV_31b0cfe6ed1e467a85fe727f44509f2c_Out_0 = IN.uv0;
            float _Split_21d9ef06c5874708ab568a015b754a62_R_1 = _UV_31b0cfe6ed1e467a85fe727f44509f2c_Out_0[0];
            float _Split_21d9ef06c5874708ab568a015b754a62_G_2 = _UV_31b0cfe6ed1e467a85fe727f44509f2c_Out_0[1];
            float _Split_21d9ef06c5874708ab568a015b754a62_B_3 = _UV_31b0cfe6ed1e467a85fe727f44509f2c_Out_0[2];
            float _Split_21d9ef06c5874708ab568a015b754a62_A_4 = _UV_31b0cfe6ed1e467a85fe727f44509f2c_Out_0[3];
            float _Multiply_2a77d064af994f4bab70efab197c4ca4_Out_2;
            Unity_Multiply_float(_Split_21d9ef06c5874708ab568a015b754a62_G_2, -5, _Multiply_2a77d064af994f4bab70efab197c4ca4_Out_2);
            float _Multiply_35ec5b4f03d142a496a1348ffe30f972_Out_2;
            Unity_Multiply_float(IN.TimeParameters.x, 1.7, _Multiply_35ec5b4f03d142a496a1348ffe30f972_Out_2);
            float _Add_cea8ce815cf84fbfb3ee7ab41f7a0938_Out_2;
            Unity_Add_float(_Multiply_2a77d064af994f4bab70efab197c4ca4_Out_2, _Multiply_35ec5b4f03d142a496a1348ffe30f972_Out_2, _Add_cea8ce815cf84fbfb3ee7ab41f7a0938_Out_2);
            float _Sine_10ccfef68a814cc5bf70eb292f062b85_Out_1;
            Unity_Sine_float(_Add_cea8ce815cf84fbfb3ee7ab41f7a0938_Out_2, _Sine_10ccfef68a814cc5bf70eb292f062b85_Out_1);
            float _Float_d22d473e77ed4b62a6db518cccb31e54_Out_0 = 1.06;
            float _Multiply_bbda3e46d90940d59cb8d6246bbc441c_Out_2;
            Unity_Multiply_float(_Float_d22d473e77ed4b62a6db518cccb31e54_Out_0, -1, _Multiply_bbda3e46d90940d59cb8d6246bbc441c_Out_2);
            float2 _Vector2_870b1d8d641948c197d01588f8741fe8_Out_0 = float2(_Multiply_bbda3e46d90940d59cb8d6246bbc441c_Out_2, 1);
            float _Remap_7d224d24655d4e3f957dcda154226dec_Out_3;
            Unity_Remap_float(_Sine_10ccfef68a814cc5bf70eb292f062b85_Out_1, float2 (0, 1), _Vector2_870b1d8d641948c197d01588f8741fe8_Out_0, _Remap_7d224d24655d4e3f957dcda154226dec_Out_3);
            float _Saturate_56e7d76143aa450cb3a44e3eb05fbfa6_Out_1;
            Unity_Saturate_float(_Remap_7d224d24655d4e3f957dcda154226dec_Out_3, _Saturate_56e7d76143aa450cb3a44e3eb05fbfa6_Out_1);
            float _Multiply_c1a89e2544d24c409bf3b2392fdac326_Out_2;
            Unity_Multiply_float(_Saturate_870dd07a1aeb4ab082f61cb28ff3aef1_Out_1, _Saturate_56e7d76143aa450cb3a44e3eb05fbfa6_Out_1, _Multiply_c1a89e2544d24c409bf3b2392fdac326_Out_2);
            float _Multiply_51cb91320b4c48e49edc1904974a37d2_Out_2;
            Unity_Multiply_float(_SampleTexture2D_2fa0a4234d564367872a87bbaf90a2d2_A_7, _Multiply_c1a89e2544d24c409bf3b2392fdac326_Out_2, _Multiply_51cb91320b4c48e49edc1904974a37d2_Out_2);
            surface.BaseColor = _Emission_d3aedfcd2a82460b815bcf7e42cd7a56_FinalColor_1;
            surface.Alpha = _Multiply_51cb91320b4c48e49edc1904974a37d2_Out_2;
            surface.AlphaClipThreshold = 0.5;
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

            output.AbsoluteWorldSpacePosition =  GetAbsolutePositionWS(input.positionWS);
            output.uv0 =                         input.texCoord0;
            output.TimeParameters =              _TimeParameters.xyz; // This is mainly for LW as HD overwrite this value
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
            Cull Off
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
            #define _AlphaClip 1
            #define ATTRIBUTES_NEED_NORMAL
            #define ATTRIBUTES_NEED_TANGENT
            #define ATTRIBUTES_NEED_TEXCOORD0
            #define VARYINGS_NEED_TEXCOORD0
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
            float4 texCoord0;
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
            float4 uv0;
            float3 TimeParameters;
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
            float4 interp0 : TEXCOORD0;
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
            output.interp0.xyzw =  input.texCoord0;
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
            output.texCoord0 = input.interp0.xyzw;
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
        float4 Texture2D_6081c378ead841c69abfc7b2580254ff_TexelSize;
        float Vector1_70a0f58dca404ff6b453838771b82aef;
        CBUFFER_END

        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(Texture2D_6081c378ead841c69abfc7b2580254ff);
        SAMPLER(samplerTexture2D_6081c378ead841c69abfc7b2580254ff);

            // Graph Functions
            
        void Unity_Normalize_float3(float3 In, out float3 Out)
        {
            Out = normalize(In);
        }

        void Unity_Multiply_float(float3 A, float3 B, out float3 Out)
        {
            Out = A * B;
        }

        void Unity_Add_float3(float3 A, float3 B, out float3 Out)
        {
            Out = A + B;
        }

        void Unity_Multiply_float(float A, float B, out float Out)
        {
            Out = A * B;
        }

        void Unity_Sine_float(float In, out float Out)
        {
            Out = sin(In);
        }

        void Unity_Cosine_float(float In, out float Out)
        {
            Out = cos(In);
        }

        void Unity_Combine_float(float R, float G, float B, float A, out float4 RGBA, out float3 RGB, out float2 RG)
        {
            RGBA = float4(R, G, B, A);
            RGB = float3(R, G, B);
            RG = float2(R, G);
        }

        void Unity_Add_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A + B;
        }


        float2 Unity_GradientNoise_Dir_float(float2 p)
        {
            // Permutation and hashing used in webgl-nosie goo.gl/pX7HtC
            p = p % 289;
            // need full precision, otherwise half overflows when p > 1
            float x = float(34 * p.x + 1) * p.x % 289 + p.y;
            x = (34 * x + 1) * x % 289;
            x = frac(x / 41) * 2 - 1;
            return normalize(float2(x - floor(x + 0.5), abs(x) - 0.5));
        }

        void Unity_GradientNoise_float(float2 UV, float Scale, out float Out)
        { 
            float2 p = UV * Scale;
            float2 ip = floor(p);
            float2 fp = frac(p);
            float d00 = dot(Unity_GradientNoise_Dir_float(ip), fp);
            float d01 = dot(Unity_GradientNoise_Dir_float(ip + float2(0, 1)), fp - float2(0, 1));
            float d10 = dot(Unity_GradientNoise_Dir_float(ip + float2(1, 0)), fp - float2(1, 0));
            float d11 = dot(Unity_GradientNoise_Dir_float(ip + float2(1, 1)), fp - float2(1, 1));
            fp = fp * fp * fp * (fp * (fp * 6 - 15) + 10);
            Out = lerp(lerp(d00, d01, fp.y), lerp(d10, d11, fp.y), fp.x) + 0.5;
        }

        void Unity_Saturate_float(float In, out float Out)
        {
            Out = saturate(In);
        }

        void Unity_Add_float(float A, float B, out float Out)
        {
            Out = A + B;
        }

        void Unity_Remap_float(float In, float2 InMinMax, float2 OutMinMax, out float Out)
        {
            Out = OutMinMax.x + (In - InMinMax.x) * (OutMinMax.y - OutMinMax.x) / (InMinMax.y - InMinMax.x);
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
            float3 _Normalize_3d6977d166e4483cb4ffbefbaf6f182a_Out_1;
            Unity_Normalize_float3(IN.ObjectSpaceNormal, _Normalize_3d6977d166e4483cb4ffbefbaf6f182a_Out_1);
            float _Property_aa9bd97b261642189ab4089656b56996_Out_0 = Vector1_70a0f58dca404ff6b453838771b82aef;
            float3 _Multiply_97e8d083b3b04a15862fb2dec2e8b0bb_Out_2;
            Unity_Multiply_float(_Normalize_3d6977d166e4483cb4ffbefbaf6f182a_Out_1, (_Property_aa9bd97b261642189ab4089656b56996_Out_0.xxx), _Multiply_97e8d083b3b04a15862fb2dec2e8b0bb_Out_2);
            float3 _Add_e4cea26ead64410da28ae26eb24d9c3c_Out_2;
            Unity_Add_float3(_Multiply_97e8d083b3b04a15862fb2dec2e8b0bb_Out_2, IN.ObjectSpacePosition, _Add_e4cea26ead64410da28ae26eb24d9c3c_Out_2);
            description.Position = _Add_e4cea26ead64410da28ae26eb24d9c3c_Out_2;
            description.Normal = IN.ObjectSpaceNormal;
            description.Tangent = IN.ObjectSpaceTangent;
            return description;
        }

            // Graph Pixel
            struct SurfaceDescription
        {
            float Alpha;
            float AlphaClipThreshold;
        };

        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            UnityTexture2D _Property_c38376267b5344bc98c0d6506235c886_Out_0 = UnityBuildTexture2DStructNoScale(Texture2D_6081c378ead841c69abfc7b2580254ff);
            float4 _SampleTexture2D_2fa0a4234d564367872a87bbaf90a2d2_RGBA_0 = SAMPLE_TEXTURE2D(_Property_c38376267b5344bc98c0d6506235c886_Out_0.tex, _Property_c38376267b5344bc98c0d6506235c886_Out_0.samplerstate, IN.uv0.xy);
            float _SampleTexture2D_2fa0a4234d564367872a87bbaf90a2d2_R_4 = _SampleTexture2D_2fa0a4234d564367872a87bbaf90a2d2_RGBA_0.r;
            float _SampleTexture2D_2fa0a4234d564367872a87bbaf90a2d2_G_5 = _SampleTexture2D_2fa0a4234d564367872a87bbaf90a2d2_RGBA_0.g;
            float _SampleTexture2D_2fa0a4234d564367872a87bbaf90a2d2_B_6 = _SampleTexture2D_2fa0a4234d564367872a87bbaf90a2d2_RGBA_0.b;
            float _SampleTexture2D_2fa0a4234d564367872a87bbaf90a2d2_A_7 = _SampleTexture2D_2fa0a4234d564367872a87bbaf90a2d2_RGBA_0.a;
            float4 _UV_0ec55d615f80432f9acd85d11a647175_Out_0 = IN.uv0;
            float _Multiply_6c611459d6994525940bf5f0f92be31e_Out_2;
            Unity_Multiply_float(IN.TimeParameters.x, 0.1, _Multiply_6c611459d6994525940bf5f0f92be31e_Out_2);
            float _Sine_573fae1d0e6c4b2b94775100bc259ce6_Out_1;
            Unity_Sine_float(_Multiply_6c611459d6994525940bf5f0f92be31e_Out_2, _Sine_573fae1d0e6c4b2b94775100bc259ce6_Out_1);
            float _Cosine_edf7bb825cbb4945a0cf937e609c4664_Out_1;
            Unity_Cosine_float(_Multiply_6c611459d6994525940bf5f0f92be31e_Out_2, _Cosine_edf7bb825cbb4945a0cf937e609c4664_Out_1);
            float4 _Combine_31428d23518a41ecac91acbf64be6544_RGBA_4;
            float3 _Combine_31428d23518a41ecac91acbf64be6544_RGB_5;
            float2 _Combine_31428d23518a41ecac91acbf64be6544_RG_6;
            Unity_Combine_float(_Sine_573fae1d0e6c4b2b94775100bc259ce6_Out_1, _Cosine_edf7bb825cbb4945a0cf937e609c4664_Out_1, 0, 0, _Combine_31428d23518a41ecac91acbf64be6544_RGBA_4, _Combine_31428d23518a41ecac91acbf64be6544_RGB_5, _Combine_31428d23518a41ecac91acbf64be6544_RG_6);
            float2 _Add_9a5030da31ca49ebb7ea22a73b786533_Out_2;
            Unity_Add_float2((_UV_0ec55d615f80432f9acd85d11a647175_Out_0.xy), _Combine_31428d23518a41ecac91acbf64be6544_RG_6, _Add_9a5030da31ca49ebb7ea22a73b786533_Out_2);
            float _GradientNoise_11338b0d122b4fc8ae9fa994f4ed5b9c_Out_2;
            Unity_GradientNoise_float(_Add_9a5030da31ca49ebb7ea22a73b786533_Out_2, 4, _GradientNoise_11338b0d122b4fc8ae9fa994f4ed5b9c_Out_2);
            float4 _Combine_66b0057efd814cd69d228d40d50ee8ec_RGBA_4;
            float3 _Combine_66b0057efd814cd69d228d40d50ee8ec_RGB_5;
            float2 _Combine_66b0057efd814cd69d228d40d50ee8ec_RG_6;
            Unity_Combine_float(_Cosine_edf7bb825cbb4945a0cf937e609c4664_Out_1, _Sine_573fae1d0e6c4b2b94775100bc259ce6_Out_1, 0, 0, _Combine_66b0057efd814cd69d228d40d50ee8ec_RGBA_4, _Combine_66b0057efd814cd69d228d40d50ee8ec_RGB_5, _Combine_66b0057efd814cd69d228d40d50ee8ec_RG_6);
            float2 _Add_892d58b07f16404c8968390dd0cbf841_Out_2;
            Unity_Add_float2((_UV_0ec55d615f80432f9acd85d11a647175_Out_0.xy), _Combine_66b0057efd814cd69d228d40d50ee8ec_RG_6, _Add_892d58b07f16404c8968390dd0cbf841_Out_2);
            float _GradientNoise_8134e22a484e442ba95b6ef504ae609e_Out_2;
            Unity_GradientNoise_float(_Add_892d58b07f16404c8968390dd0cbf841_Out_2, 10, _GradientNoise_8134e22a484e442ba95b6ef504ae609e_Out_2);
            float _Multiply_652f527d8cc24dee97852321f23c9d40_Out_2;
            Unity_Multiply_float(_GradientNoise_11338b0d122b4fc8ae9fa994f4ed5b9c_Out_2, _GradientNoise_8134e22a484e442ba95b6ef504ae609e_Out_2, _Multiply_652f527d8cc24dee97852321f23c9d40_Out_2);
            float _GradientNoise_8c21d761530f466fb0b8e4f3e494e745_Out_2;
            Unity_GradientNoise_float(_Add_9a5030da31ca49ebb7ea22a73b786533_Out_2, 30, _GradientNoise_8c21d761530f466fb0b8e4f3e494e745_Out_2);
            float _Multiply_8fc43bbb0d464facafaa453b7b12898c_Out_2;
            Unity_Multiply_float(_Multiply_652f527d8cc24dee97852321f23c9d40_Out_2, _GradientNoise_8c21d761530f466fb0b8e4f3e494e745_Out_2, _Multiply_8fc43bbb0d464facafaa453b7b12898c_Out_2);
            float _Multiply_d8dfe8dac074418a86cd52de04df5b02_Out_2;
            Unity_Multiply_float(_Multiply_8fc43bbb0d464facafaa453b7b12898c_Out_2, 8, _Multiply_d8dfe8dac074418a86cd52de04df5b02_Out_2);
            float _Saturate_870dd07a1aeb4ab082f61cb28ff3aef1_Out_1;
            Unity_Saturate_float(_Multiply_d8dfe8dac074418a86cd52de04df5b02_Out_2, _Saturate_870dd07a1aeb4ab082f61cb28ff3aef1_Out_1);
            float4 _UV_31b0cfe6ed1e467a85fe727f44509f2c_Out_0 = IN.uv0;
            float _Split_21d9ef06c5874708ab568a015b754a62_R_1 = _UV_31b0cfe6ed1e467a85fe727f44509f2c_Out_0[0];
            float _Split_21d9ef06c5874708ab568a015b754a62_G_2 = _UV_31b0cfe6ed1e467a85fe727f44509f2c_Out_0[1];
            float _Split_21d9ef06c5874708ab568a015b754a62_B_3 = _UV_31b0cfe6ed1e467a85fe727f44509f2c_Out_0[2];
            float _Split_21d9ef06c5874708ab568a015b754a62_A_4 = _UV_31b0cfe6ed1e467a85fe727f44509f2c_Out_0[3];
            float _Multiply_2a77d064af994f4bab70efab197c4ca4_Out_2;
            Unity_Multiply_float(_Split_21d9ef06c5874708ab568a015b754a62_G_2, -5, _Multiply_2a77d064af994f4bab70efab197c4ca4_Out_2);
            float _Multiply_35ec5b4f03d142a496a1348ffe30f972_Out_2;
            Unity_Multiply_float(IN.TimeParameters.x, 1.7, _Multiply_35ec5b4f03d142a496a1348ffe30f972_Out_2);
            float _Add_cea8ce815cf84fbfb3ee7ab41f7a0938_Out_2;
            Unity_Add_float(_Multiply_2a77d064af994f4bab70efab197c4ca4_Out_2, _Multiply_35ec5b4f03d142a496a1348ffe30f972_Out_2, _Add_cea8ce815cf84fbfb3ee7ab41f7a0938_Out_2);
            float _Sine_10ccfef68a814cc5bf70eb292f062b85_Out_1;
            Unity_Sine_float(_Add_cea8ce815cf84fbfb3ee7ab41f7a0938_Out_2, _Sine_10ccfef68a814cc5bf70eb292f062b85_Out_1);
            float _Float_d22d473e77ed4b62a6db518cccb31e54_Out_0 = 1.06;
            float _Multiply_bbda3e46d90940d59cb8d6246bbc441c_Out_2;
            Unity_Multiply_float(_Float_d22d473e77ed4b62a6db518cccb31e54_Out_0, -1, _Multiply_bbda3e46d90940d59cb8d6246bbc441c_Out_2);
            float2 _Vector2_870b1d8d641948c197d01588f8741fe8_Out_0 = float2(_Multiply_bbda3e46d90940d59cb8d6246bbc441c_Out_2, 1);
            float _Remap_7d224d24655d4e3f957dcda154226dec_Out_3;
            Unity_Remap_float(_Sine_10ccfef68a814cc5bf70eb292f062b85_Out_1, float2 (0, 1), _Vector2_870b1d8d641948c197d01588f8741fe8_Out_0, _Remap_7d224d24655d4e3f957dcda154226dec_Out_3);
            float _Saturate_56e7d76143aa450cb3a44e3eb05fbfa6_Out_1;
            Unity_Saturate_float(_Remap_7d224d24655d4e3f957dcda154226dec_Out_3, _Saturate_56e7d76143aa450cb3a44e3eb05fbfa6_Out_1);
            float _Multiply_c1a89e2544d24c409bf3b2392fdac326_Out_2;
            Unity_Multiply_float(_Saturate_870dd07a1aeb4ab082f61cb28ff3aef1_Out_1, _Saturate_56e7d76143aa450cb3a44e3eb05fbfa6_Out_1, _Multiply_c1a89e2544d24c409bf3b2392fdac326_Out_2);
            float _Multiply_51cb91320b4c48e49edc1904974a37d2_Out_2;
            Unity_Multiply_float(_SampleTexture2D_2fa0a4234d564367872a87bbaf90a2d2_A_7, _Multiply_c1a89e2544d24c409bf3b2392fdac326_Out_2, _Multiply_51cb91320b4c48e49edc1904974a37d2_Out_2);
            surface.Alpha = _Multiply_51cb91320b4c48e49edc1904974a37d2_Out_2;
            surface.AlphaClipThreshold = 0.5;
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





            output.uv0 =                         input.texCoord0;
            output.TimeParameters =              _TimeParameters.xyz; // This is mainly for LW as HD overwrite this value
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
            Cull Off
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
            #define _AlphaClip 1
            #define ATTRIBUTES_NEED_NORMAL
            #define ATTRIBUTES_NEED_TANGENT
            #define ATTRIBUTES_NEED_TEXCOORD0
            #define VARYINGS_NEED_TEXCOORD0
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
            float4 texCoord0;
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
            float4 uv0;
            float3 TimeParameters;
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
            float4 interp0 : TEXCOORD0;
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
            output.interp0.xyzw =  input.texCoord0;
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
            output.texCoord0 = input.interp0.xyzw;
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
        float4 Texture2D_6081c378ead841c69abfc7b2580254ff_TexelSize;
        float Vector1_70a0f58dca404ff6b453838771b82aef;
        CBUFFER_END

        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(Texture2D_6081c378ead841c69abfc7b2580254ff);
        SAMPLER(samplerTexture2D_6081c378ead841c69abfc7b2580254ff);

            // Graph Functions
            
        void Unity_Normalize_float3(float3 In, out float3 Out)
        {
            Out = normalize(In);
        }

        void Unity_Multiply_float(float3 A, float3 B, out float3 Out)
        {
            Out = A * B;
        }

        void Unity_Add_float3(float3 A, float3 B, out float3 Out)
        {
            Out = A + B;
        }

        void Unity_Multiply_float(float A, float B, out float Out)
        {
            Out = A * B;
        }

        void Unity_Sine_float(float In, out float Out)
        {
            Out = sin(In);
        }

        void Unity_Cosine_float(float In, out float Out)
        {
            Out = cos(In);
        }

        void Unity_Combine_float(float R, float G, float B, float A, out float4 RGBA, out float3 RGB, out float2 RG)
        {
            RGBA = float4(R, G, B, A);
            RGB = float3(R, G, B);
            RG = float2(R, G);
        }

        void Unity_Add_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A + B;
        }


        float2 Unity_GradientNoise_Dir_float(float2 p)
        {
            // Permutation and hashing used in webgl-nosie goo.gl/pX7HtC
            p = p % 289;
            // need full precision, otherwise half overflows when p > 1
            float x = float(34 * p.x + 1) * p.x % 289 + p.y;
            x = (34 * x + 1) * x % 289;
            x = frac(x / 41) * 2 - 1;
            return normalize(float2(x - floor(x + 0.5), abs(x) - 0.5));
        }

        void Unity_GradientNoise_float(float2 UV, float Scale, out float Out)
        { 
            float2 p = UV * Scale;
            float2 ip = floor(p);
            float2 fp = frac(p);
            float d00 = dot(Unity_GradientNoise_Dir_float(ip), fp);
            float d01 = dot(Unity_GradientNoise_Dir_float(ip + float2(0, 1)), fp - float2(0, 1));
            float d10 = dot(Unity_GradientNoise_Dir_float(ip + float2(1, 0)), fp - float2(1, 0));
            float d11 = dot(Unity_GradientNoise_Dir_float(ip + float2(1, 1)), fp - float2(1, 1));
            fp = fp * fp * fp * (fp * (fp * 6 - 15) + 10);
            Out = lerp(lerp(d00, d01, fp.y), lerp(d10, d11, fp.y), fp.x) + 0.5;
        }

        void Unity_Saturate_float(float In, out float Out)
        {
            Out = saturate(In);
        }

        void Unity_Add_float(float A, float B, out float Out)
        {
            Out = A + B;
        }

        void Unity_Remap_float(float In, float2 InMinMax, float2 OutMinMax, out float Out)
        {
            Out = OutMinMax.x + (In - InMinMax.x) * (OutMinMax.y - OutMinMax.x) / (InMinMax.y - InMinMax.x);
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
            float3 _Normalize_3d6977d166e4483cb4ffbefbaf6f182a_Out_1;
            Unity_Normalize_float3(IN.ObjectSpaceNormal, _Normalize_3d6977d166e4483cb4ffbefbaf6f182a_Out_1);
            float _Property_aa9bd97b261642189ab4089656b56996_Out_0 = Vector1_70a0f58dca404ff6b453838771b82aef;
            float3 _Multiply_97e8d083b3b04a15862fb2dec2e8b0bb_Out_2;
            Unity_Multiply_float(_Normalize_3d6977d166e4483cb4ffbefbaf6f182a_Out_1, (_Property_aa9bd97b261642189ab4089656b56996_Out_0.xxx), _Multiply_97e8d083b3b04a15862fb2dec2e8b0bb_Out_2);
            float3 _Add_e4cea26ead64410da28ae26eb24d9c3c_Out_2;
            Unity_Add_float3(_Multiply_97e8d083b3b04a15862fb2dec2e8b0bb_Out_2, IN.ObjectSpacePosition, _Add_e4cea26ead64410da28ae26eb24d9c3c_Out_2);
            description.Position = _Add_e4cea26ead64410da28ae26eb24d9c3c_Out_2;
            description.Normal = IN.ObjectSpaceNormal;
            description.Tangent = IN.ObjectSpaceTangent;
            return description;
        }

            // Graph Pixel
            struct SurfaceDescription
        {
            float Alpha;
            float AlphaClipThreshold;
        };

        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            UnityTexture2D _Property_c38376267b5344bc98c0d6506235c886_Out_0 = UnityBuildTexture2DStructNoScale(Texture2D_6081c378ead841c69abfc7b2580254ff);
            float4 _SampleTexture2D_2fa0a4234d564367872a87bbaf90a2d2_RGBA_0 = SAMPLE_TEXTURE2D(_Property_c38376267b5344bc98c0d6506235c886_Out_0.tex, _Property_c38376267b5344bc98c0d6506235c886_Out_0.samplerstate, IN.uv0.xy);
            float _SampleTexture2D_2fa0a4234d564367872a87bbaf90a2d2_R_4 = _SampleTexture2D_2fa0a4234d564367872a87bbaf90a2d2_RGBA_0.r;
            float _SampleTexture2D_2fa0a4234d564367872a87bbaf90a2d2_G_5 = _SampleTexture2D_2fa0a4234d564367872a87bbaf90a2d2_RGBA_0.g;
            float _SampleTexture2D_2fa0a4234d564367872a87bbaf90a2d2_B_6 = _SampleTexture2D_2fa0a4234d564367872a87bbaf90a2d2_RGBA_0.b;
            float _SampleTexture2D_2fa0a4234d564367872a87bbaf90a2d2_A_7 = _SampleTexture2D_2fa0a4234d564367872a87bbaf90a2d2_RGBA_0.a;
            float4 _UV_0ec55d615f80432f9acd85d11a647175_Out_0 = IN.uv0;
            float _Multiply_6c611459d6994525940bf5f0f92be31e_Out_2;
            Unity_Multiply_float(IN.TimeParameters.x, 0.1, _Multiply_6c611459d6994525940bf5f0f92be31e_Out_2);
            float _Sine_573fae1d0e6c4b2b94775100bc259ce6_Out_1;
            Unity_Sine_float(_Multiply_6c611459d6994525940bf5f0f92be31e_Out_2, _Sine_573fae1d0e6c4b2b94775100bc259ce6_Out_1);
            float _Cosine_edf7bb825cbb4945a0cf937e609c4664_Out_1;
            Unity_Cosine_float(_Multiply_6c611459d6994525940bf5f0f92be31e_Out_2, _Cosine_edf7bb825cbb4945a0cf937e609c4664_Out_1);
            float4 _Combine_31428d23518a41ecac91acbf64be6544_RGBA_4;
            float3 _Combine_31428d23518a41ecac91acbf64be6544_RGB_5;
            float2 _Combine_31428d23518a41ecac91acbf64be6544_RG_6;
            Unity_Combine_float(_Sine_573fae1d0e6c4b2b94775100bc259ce6_Out_1, _Cosine_edf7bb825cbb4945a0cf937e609c4664_Out_1, 0, 0, _Combine_31428d23518a41ecac91acbf64be6544_RGBA_4, _Combine_31428d23518a41ecac91acbf64be6544_RGB_5, _Combine_31428d23518a41ecac91acbf64be6544_RG_6);
            float2 _Add_9a5030da31ca49ebb7ea22a73b786533_Out_2;
            Unity_Add_float2((_UV_0ec55d615f80432f9acd85d11a647175_Out_0.xy), _Combine_31428d23518a41ecac91acbf64be6544_RG_6, _Add_9a5030da31ca49ebb7ea22a73b786533_Out_2);
            float _GradientNoise_11338b0d122b4fc8ae9fa994f4ed5b9c_Out_2;
            Unity_GradientNoise_float(_Add_9a5030da31ca49ebb7ea22a73b786533_Out_2, 4, _GradientNoise_11338b0d122b4fc8ae9fa994f4ed5b9c_Out_2);
            float4 _Combine_66b0057efd814cd69d228d40d50ee8ec_RGBA_4;
            float3 _Combine_66b0057efd814cd69d228d40d50ee8ec_RGB_5;
            float2 _Combine_66b0057efd814cd69d228d40d50ee8ec_RG_6;
            Unity_Combine_float(_Cosine_edf7bb825cbb4945a0cf937e609c4664_Out_1, _Sine_573fae1d0e6c4b2b94775100bc259ce6_Out_1, 0, 0, _Combine_66b0057efd814cd69d228d40d50ee8ec_RGBA_4, _Combine_66b0057efd814cd69d228d40d50ee8ec_RGB_5, _Combine_66b0057efd814cd69d228d40d50ee8ec_RG_6);
            float2 _Add_892d58b07f16404c8968390dd0cbf841_Out_2;
            Unity_Add_float2((_UV_0ec55d615f80432f9acd85d11a647175_Out_0.xy), _Combine_66b0057efd814cd69d228d40d50ee8ec_RG_6, _Add_892d58b07f16404c8968390dd0cbf841_Out_2);
            float _GradientNoise_8134e22a484e442ba95b6ef504ae609e_Out_2;
            Unity_GradientNoise_float(_Add_892d58b07f16404c8968390dd0cbf841_Out_2, 10, _GradientNoise_8134e22a484e442ba95b6ef504ae609e_Out_2);
            float _Multiply_652f527d8cc24dee97852321f23c9d40_Out_2;
            Unity_Multiply_float(_GradientNoise_11338b0d122b4fc8ae9fa994f4ed5b9c_Out_2, _GradientNoise_8134e22a484e442ba95b6ef504ae609e_Out_2, _Multiply_652f527d8cc24dee97852321f23c9d40_Out_2);
            float _GradientNoise_8c21d761530f466fb0b8e4f3e494e745_Out_2;
            Unity_GradientNoise_float(_Add_9a5030da31ca49ebb7ea22a73b786533_Out_2, 30, _GradientNoise_8c21d761530f466fb0b8e4f3e494e745_Out_2);
            float _Multiply_8fc43bbb0d464facafaa453b7b12898c_Out_2;
            Unity_Multiply_float(_Multiply_652f527d8cc24dee97852321f23c9d40_Out_2, _GradientNoise_8c21d761530f466fb0b8e4f3e494e745_Out_2, _Multiply_8fc43bbb0d464facafaa453b7b12898c_Out_2);
            float _Multiply_d8dfe8dac074418a86cd52de04df5b02_Out_2;
            Unity_Multiply_float(_Multiply_8fc43bbb0d464facafaa453b7b12898c_Out_2, 8, _Multiply_d8dfe8dac074418a86cd52de04df5b02_Out_2);
            float _Saturate_870dd07a1aeb4ab082f61cb28ff3aef1_Out_1;
            Unity_Saturate_float(_Multiply_d8dfe8dac074418a86cd52de04df5b02_Out_2, _Saturate_870dd07a1aeb4ab082f61cb28ff3aef1_Out_1);
            float4 _UV_31b0cfe6ed1e467a85fe727f44509f2c_Out_0 = IN.uv0;
            float _Split_21d9ef06c5874708ab568a015b754a62_R_1 = _UV_31b0cfe6ed1e467a85fe727f44509f2c_Out_0[0];
            float _Split_21d9ef06c5874708ab568a015b754a62_G_2 = _UV_31b0cfe6ed1e467a85fe727f44509f2c_Out_0[1];
            float _Split_21d9ef06c5874708ab568a015b754a62_B_3 = _UV_31b0cfe6ed1e467a85fe727f44509f2c_Out_0[2];
            float _Split_21d9ef06c5874708ab568a015b754a62_A_4 = _UV_31b0cfe6ed1e467a85fe727f44509f2c_Out_0[3];
            float _Multiply_2a77d064af994f4bab70efab197c4ca4_Out_2;
            Unity_Multiply_float(_Split_21d9ef06c5874708ab568a015b754a62_G_2, -5, _Multiply_2a77d064af994f4bab70efab197c4ca4_Out_2);
            float _Multiply_35ec5b4f03d142a496a1348ffe30f972_Out_2;
            Unity_Multiply_float(IN.TimeParameters.x, 1.7, _Multiply_35ec5b4f03d142a496a1348ffe30f972_Out_2);
            float _Add_cea8ce815cf84fbfb3ee7ab41f7a0938_Out_2;
            Unity_Add_float(_Multiply_2a77d064af994f4bab70efab197c4ca4_Out_2, _Multiply_35ec5b4f03d142a496a1348ffe30f972_Out_2, _Add_cea8ce815cf84fbfb3ee7ab41f7a0938_Out_2);
            float _Sine_10ccfef68a814cc5bf70eb292f062b85_Out_1;
            Unity_Sine_float(_Add_cea8ce815cf84fbfb3ee7ab41f7a0938_Out_2, _Sine_10ccfef68a814cc5bf70eb292f062b85_Out_1);
            float _Float_d22d473e77ed4b62a6db518cccb31e54_Out_0 = 1.06;
            float _Multiply_bbda3e46d90940d59cb8d6246bbc441c_Out_2;
            Unity_Multiply_float(_Float_d22d473e77ed4b62a6db518cccb31e54_Out_0, -1, _Multiply_bbda3e46d90940d59cb8d6246bbc441c_Out_2);
            float2 _Vector2_870b1d8d641948c197d01588f8741fe8_Out_0 = float2(_Multiply_bbda3e46d90940d59cb8d6246bbc441c_Out_2, 1);
            float _Remap_7d224d24655d4e3f957dcda154226dec_Out_3;
            Unity_Remap_float(_Sine_10ccfef68a814cc5bf70eb292f062b85_Out_1, float2 (0, 1), _Vector2_870b1d8d641948c197d01588f8741fe8_Out_0, _Remap_7d224d24655d4e3f957dcda154226dec_Out_3);
            float _Saturate_56e7d76143aa450cb3a44e3eb05fbfa6_Out_1;
            Unity_Saturate_float(_Remap_7d224d24655d4e3f957dcda154226dec_Out_3, _Saturate_56e7d76143aa450cb3a44e3eb05fbfa6_Out_1);
            float _Multiply_c1a89e2544d24c409bf3b2392fdac326_Out_2;
            Unity_Multiply_float(_Saturate_870dd07a1aeb4ab082f61cb28ff3aef1_Out_1, _Saturate_56e7d76143aa450cb3a44e3eb05fbfa6_Out_1, _Multiply_c1a89e2544d24c409bf3b2392fdac326_Out_2);
            float _Multiply_51cb91320b4c48e49edc1904974a37d2_Out_2;
            Unity_Multiply_float(_SampleTexture2D_2fa0a4234d564367872a87bbaf90a2d2_A_7, _Multiply_c1a89e2544d24c409bf3b2392fdac326_Out_2, _Multiply_51cb91320b4c48e49edc1904974a37d2_Out_2);
            surface.Alpha = _Multiply_51cb91320b4c48e49edc1904974a37d2_Out_2;
            surface.AlphaClipThreshold = 0.5;
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





            output.uv0 =                         input.texCoord0;
            output.TimeParameters =              _TimeParameters.xyz; // This is mainly for LW as HD overwrite this value
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
            "Queue"="AlphaTest"
        }
        Pass
        {
            Name "Pass"
            Tags
            {
                // LightMode: <None>
            }

            // Render State
            Cull Off
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



            // Defines
            #define _AlphaClip 1
            #define ATTRIBUTES_NEED_NORMAL
            #define ATTRIBUTES_NEED_TANGENT
            #define ATTRIBUTES_NEED_TEXCOORD0
            #define VARYINGS_NEED_POSITION_WS
            #define VARYINGS_NEED_NORMAL_WS
            #define VARYINGS_NEED_TANGENT_WS
            #define VARYINGS_NEED_TEXCOORD0
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
            float3 AbsoluteWorldSpacePosition;
            float4 uv0;
            float3 TimeParameters;
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
        float4 Texture2D_6081c378ead841c69abfc7b2580254ff_TexelSize;
        float Vector1_70a0f58dca404ff6b453838771b82aef;
        CBUFFER_END

        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(Texture2D_6081c378ead841c69abfc7b2580254ff);
        SAMPLER(samplerTexture2D_6081c378ead841c69abfc7b2580254ff);

            // Graph Functions
            
        void Unity_Normalize_float3(float3 In, out float3 Out)
        {
            Out = normalize(In);
        }

        void Unity_Multiply_float(float3 A, float3 B, out float3 Out)
        {
            Out = A * B;
        }

        void Unity_Add_float3(float3 A, float3 B, out float3 Out)
        {
            Out = A + B;
        }

        // 9d45c41649cfe89fdaaa688721614302
        #include "Assets/Rendering/Shaders/Functions/ShaderGraphCel.hlsl"

        struct Bindings_SimpleCelShading_2e4e5e58df2ab7843bdc75ee94394506
        {
            float3 WorldSpaceNormal;
            float3 WorldSpaceTangent;
            float3 WorldSpaceBiTangent;
            float3 AbsoluteWorldSpacePosition;
        };

        void SG_SimpleCelShading_2e4e5e58df2ab7843bdc75ee94394506(float4 Color_a950a8e798b94a5aade2710e3fcfde01, float3 Vector3_26519f981d2f4fc0957e66b16a8bc951, Bindings_SimpleCelShading_2e4e5e58df2ab7843bdc75ee94394506 IN, out half4 Color_1)
        {
            float4 _Property_61f5588b2cd94abebf90def44b873f56_Out_0 = Color_a950a8e798b94a5aade2710e3fcfde01;
            float3 _Property_b297331139c44f59939aac58082c32cd_Out_0 = Vector3_26519f981d2f4fc0957e66b16a8bc951;
            float3x3 Transform_1e7abfa1ecca4936a05f1180040b4744_transposeTangent = transpose(float3x3(IN.WorldSpaceTangent, IN.WorldSpaceBiTangent, IN.WorldSpaceNormal));
            float3 _Transform_1e7abfa1ecca4936a05f1180040b4744_Out_1 = normalize(mul(Transform_1e7abfa1ecca4936a05f1180040b4744_transposeTangent, _Property_b297331139c44f59939aac58082c32cd_Out_0.xyz).xyz);
            float3 _Add_29b1cebd4c974883b111230a3e9dae3f_Out_2;
            Unity_Add_float3(IN.WorldSpaceNormal, _Transform_1e7abfa1ecca4936a05f1180040b4744_Out_1, _Add_29b1cebd4c974883b111230a3e9dae3f_Out_2);
            float3 _Normalize_8a60e1ba11d148d0b782a9efe42f72aa_Out_1;
            Unity_Normalize_float3(_Add_29b1cebd4c974883b111230a3e9dae3f_Out_2, _Normalize_8a60e1ba11d148d0b782a9efe42f72aa_Out_1);
            half4 _SimpleCelLightingCustomFunction_2513510943474503a09a9bf6a58817e3_CelShaded_4;
            SimpleCelLighting_half(_Property_61f5588b2cd94abebf90def44b873f56_Out_0, IN.AbsoluteWorldSpacePosition, _Normalize_8a60e1ba11d148d0b782a9efe42f72aa_Out_1, _SimpleCelLightingCustomFunction_2513510943474503a09a9bf6a58817e3_CelShaded_4);
            Color_1 = _SimpleCelLightingCustomFunction_2513510943474503a09a9bf6a58817e3_CelShaded_4;
        }

        void Unity_Saturation_float(float3 In, float Saturation, out float3 Out)
        {
            float luma = dot(In, float3(0.2126729, 0.7151522, 0.0721750));
            Out =  luma.xxx + Saturation.xxx * (In - luma.xxx);
        }

        void Unity_Branch_float3(float Predicate, float3 True, float3 False, out float3 Out)
        {
            Out = Predicate ? True : False;
        }

        struct Bindings_Emission_1c7d3173c45db194bad26a66d6783e8c
        {
        };

        void SG_Emission_1c7d3173c45db194bad26a66d6783e8c(float3 Vector3_4893f82ce4ad4694bf20d3fd06b823a5, float4 Vector4_ec38e529f0df46649fe820c0c5f9bc51, float Vector1_5419bf8509d0445abafd071804949f7a, float Vector1_7d09b510b5864ccd9af8c7022e938a6b, float Boolean_c20b7d2f2312469b9e918a77ac66535b, Bindings_Emission_1c7d3173c45db194bad26a66d6783e8c IN, out float3 FinalColor_1)
        {
            float _Property_a72137ffc2824be6909477c05e3fd560_Out_0 = Boolean_c20b7d2f2312469b9e918a77ac66535b;
            float3 _Property_1eb5e58f96fe45e3b3965b7bd4f7f2ec_Out_0 = Vector3_4893f82ce4ad4694bf20d3fd06b823a5;
            float4 _Property_59cc7de5f6e946b991d0baac2f5639ed_Out_0 = Vector4_ec38e529f0df46649fe820c0c5f9bc51;
            float3 _Multiply_179269d6d4784df7a0c42a3f62f19a9c_Out_2;
            Unity_Multiply_float(_Property_1eb5e58f96fe45e3b3965b7bd4f7f2ec_Out_0, (_Property_59cc7de5f6e946b991d0baac2f5639ed_Out_0.xyz), _Multiply_179269d6d4784df7a0c42a3f62f19a9c_Out_2);
            float _Property_8072e791bf0e459eb93f2c56bca7896a_Out_0 = Vector1_5419bf8509d0445abafd071804949f7a;
            float3 _Multiply_48d1716aae5c45958b7bac29d64fea73_Out_2;
            Unity_Multiply_float(_Multiply_179269d6d4784df7a0c42a3f62f19a9c_Out_2, (_Property_8072e791bf0e459eb93f2c56bca7896a_Out_0.xxx), _Multiply_48d1716aae5c45958b7bac29d64fea73_Out_2);
            float _Property_02d5f7f6da0d4522a3cbad88b0f01592_Out_0 = Vector1_7d09b510b5864ccd9af8c7022e938a6b;
            float3 _Saturation_bf98812e88e24b1caab54dff934c2d56_Out_2;
            Unity_Saturation_float(_Multiply_48d1716aae5c45958b7bac29d64fea73_Out_2, _Property_02d5f7f6da0d4522a3cbad88b0f01592_Out_0, _Saturation_bf98812e88e24b1caab54dff934c2d56_Out_2);
            float3 _Branch_0c1d61324e704f1799e76d84ca1b9dba_Out_3;
            Unity_Branch_float3(_Property_a72137ffc2824be6909477c05e3fd560_Out_0, _Saturation_bf98812e88e24b1caab54dff934c2d56_Out_2, _Property_1eb5e58f96fe45e3b3965b7bd4f7f2ec_Out_0, _Branch_0c1d61324e704f1799e76d84ca1b9dba_Out_3);
            FinalColor_1 = _Branch_0c1d61324e704f1799e76d84ca1b9dba_Out_3;
        }

        void Unity_Multiply_float(float A, float B, out float Out)
        {
            Out = A * B;
        }

        void Unity_Sine_float(float In, out float Out)
        {
            Out = sin(In);
        }

        void Unity_Cosine_float(float In, out float Out)
        {
            Out = cos(In);
        }

        void Unity_Combine_float(float R, float G, float B, float A, out float4 RGBA, out float3 RGB, out float2 RG)
        {
            RGBA = float4(R, G, B, A);
            RGB = float3(R, G, B);
            RG = float2(R, G);
        }

        void Unity_Add_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A + B;
        }


        float2 Unity_GradientNoise_Dir_float(float2 p)
        {
            // Permutation and hashing used in webgl-nosie goo.gl/pX7HtC
            p = p % 289;
            // need full precision, otherwise half overflows when p > 1
            float x = float(34 * p.x + 1) * p.x % 289 + p.y;
            x = (34 * x + 1) * x % 289;
            x = frac(x / 41) * 2 - 1;
            return normalize(float2(x - floor(x + 0.5), abs(x) - 0.5));
        }

        void Unity_GradientNoise_float(float2 UV, float Scale, out float Out)
        { 
            float2 p = UV * Scale;
            float2 ip = floor(p);
            float2 fp = frac(p);
            float d00 = dot(Unity_GradientNoise_Dir_float(ip), fp);
            float d01 = dot(Unity_GradientNoise_Dir_float(ip + float2(0, 1)), fp - float2(0, 1));
            float d10 = dot(Unity_GradientNoise_Dir_float(ip + float2(1, 0)), fp - float2(1, 0));
            float d11 = dot(Unity_GradientNoise_Dir_float(ip + float2(1, 1)), fp - float2(1, 1));
            fp = fp * fp * fp * (fp * (fp * 6 - 15) + 10);
            Out = lerp(lerp(d00, d01, fp.y), lerp(d10, d11, fp.y), fp.x) + 0.5;
        }

        void Unity_Saturate_float(float In, out float Out)
        {
            Out = saturate(In);
        }

        void Unity_Add_float(float A, float B, out float Out)
        {
            Out = A + B;
        }

        void Unity_Remap_float(float In, float2 InMinMax, float2 OutMinMax, out float Out)
        {
            Out = OutMinMax.x + (In - InMinMax.x) * (OutMinMax.y - OutMinMax.x) / (InMinMax.y - InMinMax.x);
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
            float3 _Normalize_3d6977d166e4483cb4ffbefbaf6f182a_Out_1;
            Unity_Normalize_float3(IN.ObjectSpaceNormal, _Normalize_3d6977d166e4483cb4ffbefbaf6f182a_Out_1);
            float _Property_aa9bd97b261642189ab4089656b56996_Out_0 = Vector1_70a0f58dca404ff6b453838771b82aef;
            float3 _Multiply_97e8d083b3b04a15862fb2dec2e8b0bb_Out_2;
            Unity_Multiply_float(_Normalize_3d6977d166e4483cb4ffbefbaf6f182a_Out_1, (_Property_aa9bd97b261642189ab4089656b56996_Out_0.xxx), _Multiply_97e8d083b3b04a15862fb2dec2e8b0bb_Out_2);
            float3 _Add_e4cea26ead64410da28ae26eb24d9c3c_Out_2;
            Unity_Add_float3(_Multiply_97e8d083b3b04a15862fb2dec2e8b0bb_Out_2, IN.ObjectSpacePosition, _Add_e4cea26ead64410da28ae26eb24d9c3c_Out_2);
            description.Position = _Add_e4cea26ead64410da28ae26eb24d9c3c_Out_2;
            description.Normal = IN.ObjectSpaceNormal;
            description.Tangent = IN.ObjectSpaceTangent;
            return description;
        }

            // Graph Pixel
            struct SurfaceDescription
        {
            float3 BaseColor;
            float Alpha;
            float AlphaClipThreshold;
        };

        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            UnityTexture2D _Property_c38376267b5344bc98c0d6506235c886_Out_0 = UnityBuildTexture2DStructNoScale(Texture2D_6081c378ead841c69abfc7b2580254ff);
            float4 _SampleTexture2D_2fa0a4234d564367872a87bbaf90a2d2_RGBA_0 = SAMPLE_TEXTURE2D(_Property_c38376267b5344bc98c0d6506235c886_Out_0.tex, _Property_c38376267b5344bc98c0d6506235c886_Out_0.samplerstate, IN.uv0.xy);
            float _SampleTexture2D_2fa0a4234d564367872a87bbaf90a2d2_R_4 = _SampleTexture2D_2fa0a4234d564367872a87bbaf90a2d2_RGBA_0.r;
            float _SampleTexture2D_2fa0a4234d564367872a87bbaf90a2d2_G_5 = _SampleTexture2D_2fa0a4234d564367872a87bbaf90a2d2_RGBA_0.g;
            float _SampleTexture2D_2fa0a4234d564367872a87bbaf90a2d2_B_6 = _SampleTexture2D_2fa0a4234d564367872a87bbaf90a2d2_RGBA_0.b;
            float _SampleTexture2D_2fa0a4234d564367872a87bbaf90a2d2_A_7 = _SampleTexture2D_2fa0a4234d564367872a87bbaf90a2d2_RGBA_0.a;
            Bindings_SimpleCelShading_2e4e5e58df2ab7843bdc75ee94394506 _SimpleCelShading_edc118fd0c5a49cbb04404ac8b56a35b;
            _SimpleCelShading_edc118fd0c5a49cbb04404ac8b56a35b.WorldSpaceNormal = IN.WorldSpaceNormal;
            _SimpleCelShading_edc118fd0c5a49cbb04404ac8b56a35b.WorldSpaceTangent = IN.WorldSpaceTangent;
            _SimpleCelShading_edc118fd0c5a49cbb04404ac8b56a35b.WorldSpaceBiTangent = IN.WorldSpaceBiTangent;
            _SimpleCelShading_edc118fd0c5a49cbb04404ac8b56a35b.AbsoluteWorldSpacePosition = IN.AbsoluteWorldSpacePosition;
            half4 _SimpleCelShading_edc118fd0c5a49cbb04404ac8b56a35b_Color_1;
            SG_SimpleCelShading_2e4e5e58df2ab7843bdc75ee94394506(_SampleTexture2D_2fa0a4234d564367872a87bbaf90a2d2_RGBA_0, float3 (0, 0, 1), _SimpleCelShading_edc118fd0c5a49cbb04404ac8b56a35b, _SimpleCelShading_edc118fd0c5a49cbb04404ac8b56a35b_Color_1);
            Bindings_Emission_1c7d3173c45db194bad26a66d6783e8c _Emission_d3aedfcd2a82460b815bcf7e42cd7a56;
            float3 _Emission_d3aedfcd2a82460b815bcf7e42cd7a56_FinalColor_1;
            SG_Emission_1c7d3173c45db194bad26a66d6783e8c((_SimpleCelShading_edc118fd0c5a49cbb04404ac8b56a35b_Color_1.xyz), _SimpleCelShading_edc118fd0c5a49cbb04404ac8b56a35b_Color_1, 5, 1, 1, _Emission_d3aedfcd2a82460b815bcf7e42cd7a56, _Emission_d3aedfcd2a82460b815bcf7e42cd7a56_FinalColor_1);
            float4 _UV_0ec55d615f80432f9acd85d11a647175_Out_0 = IN.uv0;
            float _Multiply_6c611459d6994525940bf5f0f92be31e_Out_2;
            Unity_Multiply_float(IN.TimeParameters.x, 0.1, _Multiply_6c611459d6994525940bf5f0f92be31e_Out_2);
            float _Sine_573fae1d0e6c4b2b94775100bc259ce6_Out_1;
            Unity_Sine_float(_Multiply_6c611459d6994525940bf5f0f92be31e_Out_2, _Sine_573fae1d0e6c4b2b94775100bc259ce6_Out_1);
            float _Cosine_edf7bb825cbb4945a0cf937e609c4664_Out_1;
            Unity_Cosine_float(_Multiply_6c611459d6994525940bf5f0f92be31e_Out_2, _Cosine_edf7bb825cbb4945a0cf937e609c4664_Out_1);
            float4 _Combine_31428d23518a41ecac91acbf64be6544_RGBA_4;
            float3 _Combine_31428d23518a41ecac91acbf64be6544_RGB_5;
            float2 _Combine_31428d23518a41ecac91acbf64be6544_RG_6;
            Unity_Combine_float(_Sine_573fae1d0e6c4b2b94775100bc259ce6_Out_1, _Cosine_edf7bb825cbb4945a0cf937e609c4664_Out_1, 0, 0, _Combine_31428d23518a41ecac91acbf64be6544_RGBA_4, _Combine_31428d23518a41ecac91acbf64be6544_RGB_5, _Combine_31428d23518a41ecac91acbf64be6544_RG_6);
            float2 _Add_9a5030da31ca49ebb7ea22a73b786533_Out_2;
            Unity_Add_float2((_UV_0ec55d615f80432f9acd85d11a647175_Out_0.xy), _Combine_31428d23518a41ecac91acbf64be6544_RG_6, _Add_9a5030da31ca49ebb7ea22a73b786533_Out_2);
            float _GradientNoise_11338b0d122b4fc8ae9fa994f4ed5b9c_Out_2;
            Unity_GradientNoise_float(_Add_9a5030da31ca49ebb7ea22a73b786533_Out_2, 4, _GradientNoise_11338b0d122b4fc8ae9fa994f4ed5b9c_Out_2);
            float4 _Combine_66b0057efd814cd69d228d40d50ee8ec_RGBA_4;
            float3 _Combine_66b0057efd814cd69d228d40d50ee8ec_RGB_5;
            float2 _Combine_66b0057efd814cd69d228d40d50ee8ec_RG_6;
            Unity_Combine_float(_Cosine_edf7bb825cbb4945a0cf937e609c4664_Out_1, _Sine_573fae1d0e6c4b2b94775100bc259ce6_Out_1, 0, 0, _Combine_66b0057efd814cd69d228d40d50ee8ec_RGBA_4, _Combine_66b0057efd814cd69d228d40d50ee8ec_RGB_5, _Combine_66b0057efd814cd69d228d40d50ee8ec_RG_6);
            float2 _Add_892d58b07f16404c8968390dd0cbf841_Out_2;
            Unity_Add_float2((_UV_0ec55d615f80432f9acd85d11a647175_Out_0.xy), _Combine_66b0057efd814cd69d228d40d50ee8ec_RG_6, _Add_892d58b07f16404c8968390dd0cbf841_Out_2);
            float _GradientNoise_8134e22a484e442ba95b6ef504ae609e_Out_2;
            Unity_GradientNoise_float(_Add_892d58b07f16404c8968390dd0cbf841_Out_2, 10, _GradientNoise_8134e22a484e442ba95b6ef504ae609e_Out_2);
            float _Multiply_652f527d8cc24dee97852321f23c9d40_Out_2;
            Unity_Multiply_float(_GradientNoise_11338b0d122b4fc8ae9fa994f4ed5b9c_Out_2, _GradientNoise_8134e22a484e442ba95b6ef504ae609e_Out_2, _Multiply_652f527d8cc24dee97852321f23c9d40_Out_2);
            float _GradientNoise_8c21d761530f466fb0b8e4f3e494e745_Out_2;
            Unity_GradientNoise_float(_Add_9a5030da31ca49ebb7ea22a73b786533_Out_2, 30, _GradientNoise_8c21d761530f466fb0b8e4f3e494e745_Out_2);
            float _Multiply_8fc43bbb0d464facafaa453b7b12898c_Out_2;
            Unity_Multiply_float(_Multiply_652f527d8cc24dee97852321f23c9d40_Out_2, _GradientNoise_8c21d761530f466fb0b8e4f3e494e745_Out_2, _Multiply_8fc43bbb0d464facafaa453b7b12898c_Out_2);
            float _Multiply_d8dfe8dac074418a86cd52de04df5b02_Out_2;
            Unity_Multiply_float(_Multiply_8fc43bbb0d464facafaa453b7b12898c_Out_2, 8, _Multiply_d8dfe8dac074418a86cd52de04df5b02_Out_2);
            float _Saturate_870dd07a1aeb4ab082f61cb28ff3aef1_Out_1;
            Unity_Saturate_float(_Multiply_d8dfe8dac074418a86cd52de04df5b02_Out_2, _Saturate_870dd07a1aeb4ab082f61cb28ff3aef1_Out_1);
            float4 _UV_31b0cfe6ed1e467a85fe727f44509f2c_Out_0 = IN.uv0;
            float _Split_21d9ef06c5874708ab568a015b754a62_R_1 = _UV_31b0cfe6ed1e467a85fe727f44509f2c_Out_0[0];
            float _Split_21d9ef06c5874708ab568a015b754a62_G_2 = _UV_31b0cfe6ed1e467a85fe727f44509f2c_Out_0[1];
            float _Split_21d9ef06c5874708ab568a015b754a62_B_3 = _UV_31b0cfe6ed1e467a85fe727f44509f2c_Out_0[2];
            float _Split_21d9ef06c5874708ab568a015b754a62_A_4 = _UV_31b0cfe6ed1e467a85fe727f44509f2c_Out_0[3];
            float _Multiply_2a77d064af994f4bab70efab197c4ca4_Out_2;
            Unity_Multiply_float(_Split_21d9ef06c5874708ab568a015b754a62_G_2, -5, _Multiply_2a77d064af994f4bab70efab197c4ca4_Out_2);
            float _Multiply_35ec5b4f03d142a496a1348ffe30f972_Out_2;
            Unity_Multiply_float(IN.TimeParameters.x, 1.7, _Multiply_35ec5b4f03d142a496a1348ffe30f972_Out_2);
            float _Add_cea8ce815cf84fbfb3ee7ab41f7a0938_Out_2;
            Unity_Add_float(_Multiply_2a77d064af994f4bab70efab197c4ca4_Out_2, _Multiply_35ec5b4f03d142a496a1348ffe30f972_Out_2, _Add_cea8ce815cf84fbfb3ee7ab41f7a0938_Out_2);
            float _Sine_10ccfef68a814cc5bf70eb292f062b85_Out_1;
            Unity_Sine_float(_Add_cea8ce815cf84fbfb3ee7ab41f7a0938_Out_2, _Sine_10ccfef68a814cc5bf70eb292f062b85_Out_1);
            float _Float_d22d473e77ed4b62a6db518cccb31e54_Out_0 = 1.06;
            float _Multiply_bbda3e46d90940d59cb8d6246bbc441c_Out_2;
            Unity_Multiply_float(_Float_d22d473e77ed4b62a6db518cccb31e54_Out_0, -1, _Multiply_bbda3e46d90940d59cb8d6246bbc441c_Out_2);
            float2 _Vector2_870b1d8d641948c197d01588f8741fe8_Out_0 = float2(_Multiply_bbda3e46d90940d59cb8d6246bbc441c_Out_2, 1);
            float _Remap_7d224d24655d4e3f957dcda154226dec_Out_3;
            Unity_Remap_float(_Sine_10ccfef68a814cc5bf70eb292f062b85_Out_1, float2 (0, 1), _Vector2_870b1d8d641948c197d01588f8741fe8_Out_0, _Remap_7d224d24655d4e3f957dcda154226dec_Out_3);
            float _Saturate_56e7d76143aa450cb3a44e3eb05fbfa6_Out_1;
            Unity_Saturate_float(_Remap_7d224d24655d4e3f957dcda154226dec_Out_3, _Saturate_56e7d76143aa450cb3a44e3eb05fbfa6_Out_1);
            float _Multiply_c1a89e2544d24c409bf3b2392fdac326_Out_2;
            Unity_Multiply_float(_Saturate_870dd07a1aeb4ab082f61cb28ff3aef1_Out_1, _Saturate_56e7d76143aa450cb3a44e3eb05fbfa6_Out_1, _Multiply_c1a89e2544d24c409bf3b2392fdac326_Out_2);
            float _Multiply_51cb91320b4c48e49edc1904974a37d2_Out_2;
            Unity_Multiply_float(_SampleTexture2D_2fa0a4234d564367872a87bbaf90a2d2_A_7, _Multiply_c1a89e2544d24c409bf3b2392fdac326_Out_2, _Multiply_51cb91320b4c48e49edc1904974a37d2_Out_2);
            surface.BaseColor = _Emission_d3aedfcd2a82460b815bcf7e42cd7a56_FinalColor_1;
            surface.Alpha = _Multiply_51cb91320b4c48e49edc1904974a37d2_Out_2;
            surface.AlphaClipThreshold = 0.5;
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

            output.AbsoluteWorldSpacePosition =  GetAbsolutePositionWS(input.positionWS);
            output.uv0 =                         input.texCoord0;
            output.TimeParameters =              _TimeParameters.xyz; // This is mainly for LW as HD overwrite this value
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
            Cull Off
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
            #define _AlphaClip 1
            #define ATTRIBUTES_NEED_NORMAL
            #define ATTRIBUTES_NEED_TANGENT
            #define ATTRIBUTES_NEED_TEXCOORD0
            #define VARYINGS_NEED_TEXCOORD0
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
            float4 texCoord0;
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
            float4 uv0;
            float3 TimeParameters;
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
            float4 interp0 : TEXCOORD0;
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
            output.interp0.xyzw =  input.texCoord0;
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
            output.texCoord0 = input.interp0.xyzw;
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
        float4 Texture2D_6081c378ead841c69abfc7b2580254ff_TexelSize;
        float Vector1_70a0f58dca404ff6b453838771b82aef;
        CBUFFER_END

        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(Texture2D_6081c378ead841c69abfc7b2580254ff);
        SAMPLER(samplerTexture2D_6081c378ead841c69abfc7b2580254ff);

            // Graph Functions
            
        void Unity_Normalize_float3(float3 In, out float3 Out)
        {
            Out = normalize(In);
        }

        void Unity_Multiply_float(float3 A, float3 B, out float3 Out)
        {
            Out = A * B;
        }

        void Unity_Add_float3(float3 A, float3 B, out float3 Out)
        {
            Out = A + B;
        }

        void Unity_Multiply_float(float A, float B, out float Out)
        {
            Out = A * B;
        }

        void Unity_Sine_float(float In, out float Out)
        {
            Out = sin(In);
        }

        void Unity_Cosine_float(float In, out float Out)
        {
            Out = cos(In);
        }

        void Unity_Combine_float(float R, float G, float B, float A, out float4 RGBA, out float3 RGB, out float2 RG)
        {
            RGBA = float4(R, G, B, A);
            RGB = float3(R, G, B);
            RG = float2(R, G);
        }

        void Unity_Add_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A + B;
        }


        float2 Unity_GradientNoise_Dir_float(float2 p)
        {
            // Permutation and hashing used in webgl-nosie goo.gl/pX7HtC
            p = p % 289;
            // need full precision, otherwise half overflows when p > 1
            float x = float(34 * p.x + 1) * p.x % 289 + p.y;
            x = (34 * x + 1) * x % 289;
            x = frac(x / 41) * 2 - 1;
            return normalize(float2(x - floor(x + 0.5), abs(x) - 0.5));
        }

        void Unity_GradientNoise_float(float2 UV, float Scale, out float Out)
        { 
            float2 p = UV * Scale;
            float2 ip = floor(p);
            float2 fp = frac(p);
            float d00 = dot(Unity_GradientNoise_Dir_float(ip), fp);
            float d01 = dot(Unity_GradientNoise_Dir_float(ip + float2(0, 1)), fp - float2(0, 1));
            float d10 = dot(Unity_GradientNoise_Dir_float(ip + float2(1, 0)), fp - float2(1, 0));
            float d11 = dot(Unity_GradientNoise_Dir_float(ip + float2(1, 1)), fp - float2(1, 1));
            fp = fp * fp * fp * (fp * (fp * 6 - 15) + 10);
            Out = lerp(lerp(d00, d01, fp.y), lerp(d10, d11, fp.y), fp.x) + 0.5;
        }

        void Unity_Saturate_float(float In, out float Out)
        {
            Out = saturate(In);
        }

        void Unity_Add_float(float A, float B, out float Out)
        {
            Out = A + B;
        }

        void Unity_Remap_float(float In, float2 InMinMax, float2 OutMinMax, out float Out)
        {
            Out = OutMinMax.x + (In - InMinMax.x) * (OutMinMax.y - OutMinMax.x) / (InMinMax.y - InMinMax.x);
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
            float3 _Normalize_3d6977d166e4483cb4ffbefbaf6f182a_Out_1;
            Unity_Normalize_float3(IN.ObjectSpaceNormal, _Normalize_3d6977d166e4483cb4ffbefbaf6f182a_Out_1);
            float _Property_aa9bd97b261642189ab4089656b56996_Out_0 = Vector1_70a0f58dca404ff6b453838771b82aef;
            float3 _Multiply_97e8d083b3b04a15862fb2dec2e8b0bb_Out_2;
            Unity_Multiply_float(_Normalize_3d6977d166e4483cb4ffbefbaf6f182a_Out_1, (_Property_aa9bd97b261642189ab4089656b56996_Out_0.xxx), _Multiply_97e8d083b3b04a15862fb2dec2e8b0bb_Out_2);
            float3 _Add_e4cea26ead64410da28ae26eb24d9c3c_Out_2;
            Unity_Add_float3(_Multiply_97e8d083b3b04a15862fb2dec2e8b0bb_Out_2, IN.ObjectSpacePosition, _Add_e4cea26ead64410da28ae26eb24d9c3c_Out_2);
            description.Position = _Add_e4cea26ead64410da28ae26eb24d9c3c_Out_2;
            description.Normal = IN.ObjectSpaceNormal;
            description.Tangent = IN.ObjectSpaceTangent;
            return description;
        }

            // Graph Pixel
            struct SurfaceDescription
        {
            float Alpha;
            float AlphaClipThreshold;
        };

        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            UnityTexture2D _Property_c38376267b5344bc98c0d6506235c886_Out_0 = UnityBuildTexture2DStructNoScale(Texture2D_6081c378ead841c69abfc7b2580254ff);
            float4 _SampleTexture2D_2fa0a4234d564367872a87bbaf90a2d2_RGBA_0 = SAMPLE_TEXTURE2D(_Property_c38376267b5344bc98c0d6506235c886_Out_0.tex, _Property_c38376267b5344bc98c0d6506235c886_Out_0.samplerstate, IN.uv0.xy);
            float _SampleTexture2D_2fa0a4234d564367872a87bbaf90a2d2_R_4 = _SampleTexture2D_2fa0a4234d564367872a87bbaf90a2d2_RGBA_0.r;
            float _SampleTexture2D_2fa0a4234d564367872a87bbaf90a2d2_G_5 = _SampleTexture2D_2fa0a4234d564367872a87bbaf90a2d2_RGBA_0.g;
            float _SampleTexture2D_2fa0a4234d564367872a87bbaf90a2d2_B_6 = _SampleTexture2D_2fa0a4234d564367872a87bbaf90a2d2_RGBA_0.b;
            float _SampleTexture2D_2fa0a4234d564367872a87bbaf90a2d2_A_7 = _SampleTexture2D_2fa0a4234d564367872a87bbaf90a2d2_RGBA_0.a;
            float4 _UV_0ec55d615f80432f9acd85d11a647175_Out_0 = IN.uv0;
            float _Multiply_6c611459d6994525940bf5f0f92be31e_Out_2;
            Unity_Multiply_float(IN.TimeParameters.x, 0.1, _Multiply_6c611459d6994525940bf5f0f92be31e_Out_2);
            float _Sine_573fae1d0e6c4b2b94775100bc259ce6_Out_1;
            Unity_Sine_float(_Multiply_6c611459d6994525940bf5f0f92be31e_Out_2, _Sine_573fae1d0e6c4b2b94775100bc259ce6_Out_1);
            float _Cosine_edf7bb825cbb4945a0cf937e609c4664_Out_1;
            Unity_Cosine_float(_Multiply_6c611459d6994525940bf5f0f92be31e_Out_2, _Cosine_edf7bb825cbb4945a0cf937e609c4664_Out_1);
            float4 _Combine_31428d23518a41ecac91acbf64be6544_RGBA_4;
            float3 _Combine_31428d23518a41ecac91acbf64be6544_RGB_5;
            float2 _Combine_31428d23518a41ecac91acbf64be6544_RG_6;
            Unity_Combine_float(_Sine_573fae1d0e6c4b2b94775100bc259ce6_Out_1, _Cosine_edf7bb825cbb4945a0cf937e609c4664_Out_1, 0, 0, _Combine_31428d23518a41ecac91acbf64be6544_RGBA_4, _Combine_31428d23518a41ecac91acbf64be6544_RGB_5, _Combine_31428d23518a41ecac91acbf64be6544_RG_6);
            float2 _Add_9a5030da31ca49ebb7ea22a73b786533_Out_2;
            Unity_Add_float2((_UV_0ec55d615f80432f9acd85d11a647175_Out_0.xy), _Combine_31428d23518a41ecac91acbf64be6544_RG_6, _Add_9a5030da31ca49ebb7ea22a73b786533_Out_2);
            float _GradientNoise_11338b0d122b4fc8ae9fa994f4ed5b9c_Out_2;
            Unity_GradientNoise_float(_Add_9a5030da31ca49ebb7ea22a73b786533_Out_2, 4, _GradientNoise_11338b0d122b4fc8ae9fa994f4ed5b9c_Out_2);
            float4 _Combine_66b0057efd814cd69d228d40d50ee8ec_RGBA_4;
            float3 _Combine_66b0057efd814cd69d228d40d50ee8ec_RGB_5;
            float2 _Combine_66b0057efd814cd69d228d40d50ee8ec_RG_6;
            Unity_Combine_float(_Cosine_edf7bb825cbb4945a0cf937e609c4664_Out_1, _Sine_573fae1d0e6c4b2b94775100bc259ce6_Out_1, 0, 0, _Combine_66b0057efd814cd69d228d40d50ee8ec_RGBA_4, _Combine_66b0057efd814cd69d228d40d50ee8ec_RGB_5, _Combine_66b0057efd814cd69d228d40d50ee8ec_RG_6);
            float2 _Add_892d58b07f16404c8968390dd0cbf841_Out_2;
            Unity_Add_float2((_UV_0ec55d615f80432f9acd85d11a647175_Out_0.xy), _Combine_66b0057efd814cd69d228d40d50ee8ec_RG_6, _Add_892d58b07f16404c8968390dd0cbf841_Out_2);
            float _GradientNoise_8134e22a484e442ba95b6ef504ae609e_Out_2;
            Unity_GradientNoise_float(_Add_892d58b07f16404c8968390dd0cbf841_Out_2, 10, _GradientNoise_8134e22a484e442ba95b6ef504ae609e_Out_2);
            float _Multiply_652f527d8cc24dee97852321f23c9d40_Out_2;
            Unity_Multiply_float(_GradientNoise_11338b0d122b4fc8ae9fa994f4ed5b9c_Out_2, _GradientNoise_8134e22a484e442ba95b6ef504ae609e_Out_2, _Multiply_652f527d8cc24dee97852321f23c9d40_Out_2);
            float _GradientNoise_8c21d761530f466fb0b8e4f3e494e745_Out_2;
            Unity_GradientNoise_float(_Add_9a5030da31ca49ebb7ea22a73b786533_Out_2, 30, _GradientNoise_8c21d761530f466fb0b8e4f3e494e745_Out_2);
            float _Multiply_8fc43bbb0d464facafaa453b7b12898c_Out_2;
            Unity_Multiply_float(_Multiply_652f527d8cc24dee97852321f23c9d40_Out_2, _GradientNoise_8c21d761530f466fb0b8e4f3e494e745_Out_2, _Multiply_8fc43bbb0d464facafaa453b7b12898c_Out_2);
            float _Multiply_d8dfe8dac074418a86cd52de04df5b02_Out_2;
            Unity_Multiply_float(_Multiply_8fc43bbb0d464facafaa453b7b12898c_Out_2, 8, _Multiply_d8dfe8dac074418a86cd52de04df5b02_Out_2);
            float _Saturate_870dd07a1aeb4ab082f61cb28ff3aef1_Out_1;
            Unity_Saturate_float(_Multiply_d8dfe8dac074418a86cd52de04df5b02_Out_2, _Saturate_870dd07a1aeb4ab082f61cb28ff3aef1_Out_1);
            float4 _UV_31b0cfe6ed1e467a85fe727f44509f2c_Out_0 = IN.uv0;
            float _Split_21d9ef06c5874708ab568a015b754a62_R_1 = _UV_31b0cfe6ed1e467a85fe727f44509f2c_Out_0[0];
            float _Split_21d9ef06c5874708ab568a015b754a62_G_2 = _UV_31b0cfe6ed1e467a85fe727f44509f2c_Out_0[1];
            float _Split_21d9ef06c5874708ab568a015b754a62_B_3 = _UV_31b0cfe6ed1e467a85fe727f44509f2c_Out_0[2];
            float _Split_21d9ef06c5874708ab568a015b754a62_A_4 = _UV_31b0cfe6ed1e467a85fe727f44509f2c_Out_0[3];
            float _Multiply_2a77d064af994f4bab70efab197c4ca4_Out_2;
            Unity_Multiply_float(_Split_21d9ef06c5874708ab568a015b754a62_G_2, -5, _Multiply_2a77d064af994f4bab70efab197c4ca4_Out_2);
            float _Multiply_35ec5b4f03d142a496a1348ffe30f972_Out_2;
            Unity_Multiply_float(IN.TimeParameters.x, 1.7, _Multiply_35ec5b4f03d142a496a1348ffe30f972_Out_2);
            float _Add_cea8ce815cf84fbfb3ee7ab41f7a0938_Out_2;
            Unity_Add_float(_Multiply_2a77d064af994f4bab70efab197c4ca4_Out_2, _Multiply_35ec5b4f03d142a496a1348ffe30f972_Out_2, _Add_cea8ce815cf84fbfb3ee7ab41f7a0938_Out_2);
            float _Sine_10ccfef68a814cc5bf70eb292f062b85_Out_1;
            Unity_Sine_float(_Add_cea8ce815cf84fbfb3ee7ab41f7a0938_Out_2, _Sine_10ccfef68a814cc5bf70eb292f062b85_Out_1);
            float _Float_d22d473e77ed4b62a6db518cccb31e54_Out_0 = 1.06;
            float _Multiply_bbda3e46d90940d59cb8d6246bbc441c_Out_2;
            Unity_Multiply_float(_Float_d22d473e77ed4b62a6db518cccb31e54_Out_0, -1, _Multiply_bbda3e46d90940d59cb8d6246bbc441c_Out_2);
            float2 _Vector2_870b1d8d641948c197d01588f8741fe8_Out_0 = float2(_Multiply_bbda3e46d90940d59cb8d6246bbc441c_Out_2, 1);
            float _Remap_7d224d24655d4e3f957dcda154226dec_Out_3;
            Unity_Remap_float(_Sine_10ccfef68a814cc5bf70eb292f062b85_Out_1, float2 (0, 1), _Vector2_870b1d8d641948c197d01588f8741fe8_Out_0, _Remap_7d224d24655d4e3f957dcda154226dec_Out_3);
            float _Saturate_56e7d76143aa450cb3a44e3eb05fbfa6_Out_1;
            Unity_Saturate_float(_Remap_7d224d24655d4e3f957dcda154226dec_Out_3, _Saturate_56e7d76143aa450cb3a44e3eb05fbfa6_Out_1);
            float _Multiply_c1a89e2544d24c409bf3b2392fdac326_Out_2;
            Unity_Multiply_float(_Saturate_870dd07a1aeb4ab082f61cb28ff3aef1_Out_1, _Saturate_56e7d76143aa450cb3a44e3eb05fbfa6_Out_1, _Multiply_c1a89e2544d24c409bf3b2392fdac326_Out_2);
            float _Multiply_51cb91320b4c48e49edc1904974a37d2_Out_2;
            Unity_Multiply_float(_SampleTexture2D_2fa0a4234d564367872a87bbaf90a2d2_A_7, _Multiply_c1a89e2544d24c409bf3b2392fdac326_Out_2, _Multiply_51cb91320b4c48e49edc1904974a37d2_Out_2);
            surface.Alpha = _Multiply_51cb91320b4c48e49edc1904974a37d2_Out_2;
            surface.AlphaClipThreshold = 0.5;
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





            output.uv0 =                         input.texCoord0;
            output.TimeParameters =              _TimeParameters.xyz; // This is mainly for LW as HD overwrite this value
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
            Cull Off
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
            #define _AlphaClip 1
            #define ATTRIBUTES_NEED_NORMAL
            #define ATTRIBUTES_NEED_TANGENT
            #define ATTRIBUTES_NEED_TEXCOORD0
            #define VARYINGS_NEED_TEXCOORD0
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
            float4 texCoord0;
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
            float4 uv0;
            float3 TimeParameters;
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
            float4 interp0 : TEXCOORD0;
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
            output.interp0.xyzw =  input.texCoord0;
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
            output.texCoord0 = input.interp0.xyzw;
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
        float4 Texture2D_6081c378ead841c69abfc7b2580254ff_TexelSize;
        float Vector1_70a0f58dca404ff6b453838771b82aef;
        CBUFFER_END

        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(Texture2D_6081c378ead841c69abfc7b2580254ff);
        SAMPLER(samplerTexture2D_6081c378ead841c69abfc7b2580254ff);

            // Graph Functions
            
        void Unity_Normalize_float3(float3 In, out float3 Out)
        {
            Out = normalize(In);
        }

        void Unity_Multiply_float(float3 A, float3 B, out float3 Out)
        {
            Out = A * B;
        }

        void Unity_Add_float3(float3 A, float3 B, out float3 Out)
        {
            Out = A + B;
        }

        void Unity_Multiply_float(float A, float B, out float Out)
        {
            Out = A * B;
        }

        void Unity_Sine_float(float In, out float Out)
        {
            Out = sin(In);
        }

        void Unity_Cosine_float(float In, out float Out)
        {
            Out = cos(In);
        }

        void Unity_Combine_float(float R, float G, float B, float A, out float4 RGBA, out float3 RGB, out float2 RG)
        {
            RGBA = float4(R, G, B, A);
            RGB = float3(R, G, B);
            RG = float2(R, G);
        }

        void Unity_Add_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A + B;
        }


        float2 Unity_GradientNoise_Dir_float(float2 p)
        {
            // Permutation and hashing used in webgl-nosie goo.gl/pX7HtC
            p = p % 289;
            // need full precision, otherwise half overflows when p > 1
            float x = float(34 * p.x + 1) * p.x % 289 + p.y;
            x = (34 * x + 1) * x % 289;
            x = frac(x / 41) * 2 - 1;
            return normalize(float2(x - floor(x + 0.5), abs(x) - 0.5));
        }

        void Unity_GradientNoise_float(float2 UV, float Scale, out float Out)
        { 
            float2 p = UV * Scale;
            float2 ip = floor(p);
            float2 fp = frac(p);
            float d00 = dot(Unity_GradientNoise_Dir_float(ip), fp);
            float d01 = dot(Unity_GradientNoise_Dir_float(ip + float2(0, 1)), fp - float2(0, 1));
            float d10 = dot(Unity_GradientNoise_Dir_float(ip + float2(1, 0)), fp - float2(1, 0));
            float d11 = dot(Unity_GradientNoise_Dir_float(ip + float2(1, 1)), fp - float2(1, 1));
            fp = fp * fp * fp * (fp * (fp * 6 - 15) + 10);
            Out = lerp(lerp(d00, d01, fp.y), lerp(d10, d11, fp.y), fp.x) + 0.5;
        }

        void Unity_Saturate_float(float In, out float Out)
        {
            Out = saturate(In);
        }

        void Unity_Add_float(float A, float B, out float Out)
        {
            Out = A + B;
        }

        void Unity_Remap_float(float In, float2 InMinMax, float2 OutMinMax, out float Out)
        {
            Out = OutMinMax.x + (In - InMinMax.x) * (OutMinMax.y - OutMinMax.x) / (InMinMax.y - InMinMax.x);
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
            float3 _Normalize_3d6977d166e4483cb4ffbefbaf6f182a_Out_1;
            Unity_Normalize_float3(IN.ObjectSpaceNormal, _Normalize_3d6977d166e4483cb4ffbefbaf6f182a_Out_1);
            float _Property_aa9bd97b261642189ab4089656b56996_Out_0 = Vector1_70a0f58dca404ff6b453838771b82aef;
            float3 _Multiply_97e8d083b3b04a15862fb2dec2e8b0bb_Out_2;
            Unity_Multiply_float(_Normalize_3d6977d166e4483cb4ffbefbaf6f182a_Out_1, (_Property_aa9bd97b261642189ab4089656b56996_Out_0.xxx), _Multiply_97e8d083b3b04a15862fb2dec2e8b0bb_Out_2);
            float3 _Add_e4cea26ead64410da28ae26eb24d9c3c_Out_2;
            Unity_Add_float3(_Multiply_97e8d083b3b04a15862fb2dec2e8b0bb_Out_2, IN.ObjectSpacePosition, _Add_e4cea26ead64410da28ae26eb24d9c3c_Out_2);
            description.Position = _Add_e4cea26ead64410da28ae26eb24d9c3c_Out_2;
            description.Normal = IN.ObjectSpaceNormal;
            description.Tangent = IN.ObjectSpaceTangent;
            return description;
        }

            // Graph Pixel
            struct SurfaceDescription
        {
            float Alpha;
            float AlphaClipThreshold;
        };

        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            UnityTexture2D _Property_c38376267b5344bc98c0d6506235c886_Out_0 = UnityBuildTexture2DStructNoScale(Texture2D_6081c378ead841c69abfc7b2580254ff);
            float4 _SampleTexture2D_2fa0a4234d564367872a87bbaf90a2d2_RGBA_0 = SAMPLE_TEXTURE2D(_Property_c38376267b5344bc98c0d6506235c886_Out_0.tex, _Property_c38376267b5344bc98c0d6506235c886_Out_0.samplerstate, IN.uv0.xy);
            float _SampleTexture2D_2fa0a4234d564367872a87bbaf90a2d2_R_4 = _SampleTexture2D_2fa0a4234d564367872a87bbaf90a2d2_RGBA_0.r;
            float _SampleTexture2D_2fa0a4234d564367872a87bbaf90a2d2_G_5 = _SampleTexture2D_2fa0a4234d564367872a87bbaf90a2d2_RGBA_0.g;
            float _SampleTexture2D_2fa0a4234d564367872a87bbaf90a2d2_B_6 = _SampleTexture2D_2fa0a4234d564367872a87bbaf90a2d2_RGBA_0.b;
            float _SampleTexture2D_2fa0a4234d564367872a87bbaf90a2d2_A_7 = _SampleTexture2D_2fa0a4234d564367872a87bbaf90a2d2_RGBA_0.a;
            float4 _UV_0ec55d615f80432f9acd85d11a647175_Out_0 = IN.uv0;
            float _Multiply_6c611459d6994525940bf5f0f92be31e_Out_2;
            Unity_Multiply_float(IN.TimeParameters.x, 0.1, _Multiply_6c611459d6994525940bf5f0f92be31e_Out_2);
            float _Sine_573fae1d0e6c4b2b94775100bc259ce6_Out_1;
            Unity_Sine_float(_Multiply_6c611459d6994525940bf5f0f92be31e_Out_2, _Sine_573fae1d0e6c4b2b94775100bc259ce6_Out_1);
            float _Cosine_edf7bb825cbb4945a0cf937e609c4664_Out_1;
            Unity_Cosine_float(_Multiply_6c611459d6994525940bf5f0f92be31e_Out_2, _Cosine_edf7bb825cbb4945a0cf937e609c4664_Out_1);
            float4 _Combine_31428d23518a41ecac91acbf64be6544_RGBA_4;
            float3 _Combine_31428d23518a41ecac91acbf64be6544_RGB_5;
            float2 _Combine_31428d23518a41ecac91acbf64be6544_RG_6;
            Unity_Combine_float(_Sine_573fae1d0e6c4b2b94775100bc259ce6_Out_1, _Cosine_edf7bb825cbb4945a0cf937e609c4664_Out_1, 0, 0, _Combine_31428d23518a41ecac91acbf64be6544_RGBA_4, _Combine_31428d23518a41ecac91acbf64be6544_RGB_5, _Combine_31428d23518a41ecac91acbf64be6544_RG_6);
            float2 _Add_9a5030da31ca49ebb7ea22a73b786533_Out_2;
            Unity_Add_float2((_UV_0ec55d615f80432f9acd85d11a647175_Out_0.xy), _Combine_31428d23518a41ecac91acbf64be6544_RG_6, _Add_9a5030da31ca49ebb7ea22a73b786533_Out_2);
            float _GradientNoise_11338b0d122b4fc8ae9fa994f4ed5b9c_Out_2;
            Unity_GradientNoise_float(_Add_9a5030da31ca49ebb7ea22a73b786533_Out_2, 4, _GradientNoise_11338b0d122b4fc8ae9fa994f4ed5b9c_Out_2);
            float4 _Combine_66b0057efd814cd69d228d40d50ee8ec_RGBA_4;
            float3 _Combine_66b0057efd814cd69d228d40d50ee8ec_RGB_5;
            float2 _Combine_66b0057efd814cd69d228d40d50ee8ec_RG_6;
            Unity_Combine_float(_Cosine_edf7bb825cbb4945a0cf937e609c4664_Out_1, _Sine_573fae1d0e6c4b2b94775100bc259ce6_Out_1, 0, 0, _Combine_66b0057efd814cd69d228d40d50ee8ec_RGBA_4, _Combine_66b0057efd814cd69d228d40d50ee8ec_RGB_5, _Combine_66b0057efd814cd69d228d40d50ee8ec_RG_6);
            float2 _Add_892d58b07f16404c8968390dd0cbf841_Out_2;
            Unity_Add_float2((_UV_0ec55d615f80432f9acd85d11a647175_Out_0.xy), _Combine_66b0057efd814cd69d228d40d50ee8ec_RG_6, _Add_892d58b07f16404c8968390dd0cbf841_Out_2);
            float _GradientNoise_8134e22a484e442ba95b6ef504ae609e_Out_2;
            Unity_GradientNoise_float(_Add_892d58b07f16404c8968390dd0cbf841_Out_2, 10, _GradientNoise_8134e22a484e442ba95b6ef504ae609e_Out_2);
            float _Multiply_652f527d8cc24dee97852321f23c9d40_Out_2;
            Unity_Multiply_float(_GradientNoise_11338b0d122b4fc8ae9fa994f4ed5b9c_Out_2, _GradientNoise_8134e22a484e442ba95b6ef504ae609e_Out_2, _Multiply_652f527d8cc24dee97852321f23c9d40_Out_2);
            float _GradientNoise_8c21d761530f466fb0b8e4f3e494e745_Out_2;
            Unity_GradientNoise_float(_Add_9a5030da31ca49ebb7ea22a73b786533_Out_2, 30, _GradientNoise_8c21d761530f466fb0b8e4f3e494e745_Out_2);
            float _Multiply_8fc43bbb0d464facafaa453b7b12898c_Out_2;
            Unity_Multiply_float(_Multiply_652f527d8cc24dee97852321f23c9d40_Out_2, _GradientNoise_8c21d761530f466fb0b8e4f3e494e745_Out_2, _Multiply_8fc43bbb0d464facafaa453b7b12898c_Out_2);
            float _Multiply_d8dfe8dac074418a86cd52de04df5b02_Out_2;
            Unity_Multiply_float(_Multiply_8fc43bbb0d464facafaa453b7b12898c_Out_2, 8, _Multiply_d8dfe8dac074418a86cd52de04df5b02_Out_2);
            float _Saturate_870dd07a1aeb4ab082f61cb28ff3aef1_Out_1;
            Unity_Saturate_float(_Multiply_d8dfe8dac074418a86cd52de04df5b02_Out_2, _Saturate_870dd07a1aeb4ab082f61cb28ff3aef1_Out_1);
            float4 _UV_31b0cfe6ed1e467a85fe727f44509f2c_Out_0 = IN.uv0;
            float _Split_21d9ef06c5874708ab568a015b754a62_R_1 = _UV_31b0cfe6ed1e467a85fe727f44509f2c_Out_0[0];
            float _Split_21d9ef06c5874708ab568a015b754a62_G_2 = _UV_31b0cfe6ed1e467a85fe727f44509f2c_Out_0[1];
            float _Split_21d9ef06c5874708ab568a015b754a62_B_3 = _UV_31b0cfe6ed1e467a85fe727f44509f2c_Out_0[2];
            float _Split_21d9ef06c5874708ab568a015b754a62_A_4 = _UV_31b0cfe6ed1e467a85fe727f44509f2c_Out_0[3];
            float _Multiply_2a77d064af994f4bab70efab197c4ca4_Out_2;
            Unity_Multiply_float(_Split_21d9ef06c5874708ab568a015b754a62_G_2, -5, _Multiply_2a77d064af994f4bab70efab197c4ca4_Out_2);
            float _Multiply_35ec5b4f03d142a496a1348ffe30f972_Out_2;
            Unity_Multiply_float(IN.TimeParameters.x, 1.7, _Multiply_35ec5b4f03d142a496a1348ffe30f972_Out_2);
            float _Add_cea8ce815cf84fbfb3ee7ab41f7a0938_Out_2;
            Unity_Add_float(_Multiply_2a77d064af994f4bab70efab197c4ca4_Out_2, _Multiply_35ec5b4f03d142a496a1348ffe30f972_Out_2, _Add_cea8ce815cf84fbfb3ee7ab41f7a0938_Out_2);
            float _Sine_10ccfef68a814cc5bf70eb292f062b85_Out_1;
            Unity_Sine_float(_Add_cea8ce815cf84fbfb3ee7ab41f7a0938_Out_2, _Sine_10ccfef68a814cc5bf70eb292f062b85_Out_1);
            float _Float_d22d473e77ed4b62a6db518cccb31e54_Out_0 = 1.06;
            float _Multiply_bbda3e46d90940d59cb8d6246bbc441c_Out_2;
            Unity_Multiply_float(_Float_d22d473e77ed4b62a6db518cccb31e54_Out_0, -1, _Multiply_bbda3e46d90940d59cb8d6246bbc441c_Out_2);
            float2 _Vector2_870b1d8d641948c197d01588f8741fe8_Out_0 = float2(_Multiply_bbda3e46d90940d59cb8d6246bbc441c_Out_2, 1);
            float _Remap_7d224d24655d4e3f957dcda154226dec_Out_3;
            Unity_Remap_float(_Sine_10ccfef68a814cc5bf70eb292f062b85_Out_1, float2 (0, 1), _Vector2_870b1d8d641948c197d01588f8741fe8_Out_0, _Remap_7d224d24655d4e3f957dcda154226dec_Out_3);
            float _Saturate_56e7d76143aa450cb3a44e3eb05fbfa6_Out_1;
            Unity_Saturate_float(_Remap_7d224d24655d4e3f957dcda154226dec_Out_3, _Saturate_56e7d76143aa450cb3a44e3eb05fbfa6_Out_1);
            float _Multiply_c1a89e2544d24c409bf3b2392fdac326_Out_2;
            Unity_Multiply_float(_Saturate_870dd07a1aeb4ab082f61cb28ff3aef1_Out_1, _Saturate_56e7d76143aa450cb3a44e3eb05fbfa6_Out_1, _Multiply_c1a89e2544d24c409bf3b2392fdac326_Out_2);
            float _Multiply_51cb91320b4c48e49edc1904974a37d2_Out_2;
            Unity_Multiply_float(_SampleTexture2D_2fa0a4234d564367872a87bbaf90a2d2_A_7, _Multiply_c1a89e2544d24c409bf3b2392fdac326_Out_2, _Multiply_51cb91320b4c48e49edc1904974a37d2_Out_2);
            surface.Alpha = _Multiply_51cb91320b4c48e49edc1904974a37d2_Out_2;
            surface.AlphaClipThreshold = 0.5;
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





            output.uv0 =                         input.texCoord0;
            output.TimeParameters =              _TimeParameters.xyz; // This is mainly for LW as HD overwrite this value
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