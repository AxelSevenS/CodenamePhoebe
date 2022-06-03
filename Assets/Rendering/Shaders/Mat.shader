Shader "CelShaded/Mat"
{
    Properties
    {
        [NoScaleOffset]Texture2D_9d2dc686cea4411eb4cd42e0ad89db69("Color Map &", 2D) = "white" {}
        Vector1_eba81f4ec9494ffc89d831e650dc2eee("# Style Options", Float) = 0
        [ToggleUI]Boolean_746f00cdd21a463a8f7db0eed7f65639("Proximity Dither", Float) = 0
        [ToggleUI]NORMALMAP_ON("Normal Map", Float) = 0
        [NoScaleOffset]Texture2D_1709bfc569f34021a1205546b253ac2b("- Normal Map & [NORMALMAP_ON]", 2D) = "grey" {}
        Vector1_83717f647fb949a0b96857d7ef1efef2("- Normal Intensity [NORMALMAP_ON]", Range(0, 1)) = 0
        [ToggleUI]FLOWMAP_ON("Flow Map", Float) = 0
        [NoScaleOffset]Texture2D_9e7ce96f70ec4e4fad0aca3a397849ab("- Flow Map & [FLOWMAP_ON]", 2D) = "white" {}
        Vector1_693d76d6835a455fba9b2a18be0e2bec("- Flow Map Speed [FLOWMAP_ON]", Float) = 0.5
        Vector1_da40d7438c114e189f67f755323f75ea("- Flow Map Strength [FLOWMAP_ON]", Float) = 0.5
        [ToggleUI]EMISSIONMAP_ON("Emission", Float) = 0
        [NoScaleOffset]Texture2D_ea1c329133fc4f01a0ae24d3d7ecfc16("- Emission Map & [EMISSIONMAP_ON]", 2D) = "white" {}
        Vector1_a62ab1db54d34856836ce0b6d78fe08f("- Emission Intensity [EMISSIONMAP_ON]", Float) = 0.5
        Vector1_c092b1ab83a845279db3044fddc481d7("- Emission Saturation [EMISSIONMAP_ON]", Float) = 1
        [ToggleUI]Boolean_1("!REF CEL_SPECULAR_ON", Float) = 0
        Vector1_1("- Specular Intensity [CEL_SPECULAR_ON]", Float) = 0.5
        Vector1_38eeab6d9e3146b49202d29537ff230c("- Smoothness [CEL_SPECULAR_ON]", Range(0, 1)) = 0.5
        [ToggleUI]Boolean_101eac5857d84f2b8c80fed3fbaa437b("!REF CEL_ACCENT_ON", Float) = 0
        [NoScaleOffset]Texture2D_408732e7caa34af796ddb77be6aaabea("- Accent Map & [CEL_ACCENT_ON]", 2D) = "white" {}
        [HideInInspector][NoScaleOffset]unity_Lightmaps("unity_Lightmaps", 2DArray) = "" {}
        [HideInInspector][NoScaleOffset]unity_LightmapsInd("unity_LightmapsInd", 2DArray) = "" {}
        [HideInInspector][NoScaleOffset]unity_ShadowMasks("unity_ShadowMasks", 2DArray) = "" {}
        [Toggle]CEL_SPECULAR("Specular", Float) = 0
        [Toggle]CEL_ACCENT("Accent", Float) = 0
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
            #pragma shader_feature_local _ CEL_SPECULAR_ON
        #pragma shader_feature_local _ CEL_ACCENT_ON
        #pragma multi_compile _ _SHADOWS_SOFT
        #pragma multi_compile _ _ADDITIONAL_LIGHTS

        #if defined(CEL_SPECULAR_ON) && defined(CEL_ACCENT_ON)
            #define KEYWORD_PERMUTATION_0
        #elif defined(CEL_SPECULAR_ON)
            #define KEYWORD_PERMUTATION_1
        #elif defined(CEL_ACCENT_ON)
            #define KEYWORD_PERMUTATION_2
        #else
            #define KEYWORD_PERMUTATION_3
        #endif


            // Defines
        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
        #define _AlphaClip 1
        #endif

        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
        #define ATTRIBUTES_NEED_NORMAL
        #endif

        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
        #define ATTRIBUTES_NEED_TANGENT
        #endif

        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
        #define ATTRIBUTES_NEED_TEXCOORD0
        #endif

        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
        #define VARYINGS_NEED_POSITION_WS
        #endif

        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
        #define VARYINGS_NEED_NORMAL_WS
        #endif

        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
        #define VARYINGS_NEED_TANGENT_WS
        #endif

        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
        #define VARYINGS_NEED_TEXCOORD0
        #endif

        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
        #define VARYINGS_NEED_VIEWDIRECTION_WS
        #endif

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
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
            float3 positionOS : POSITION;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
            float3 normalOS : NORMAL;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
            float4 tangentOS : TANGENT;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
            float4 uv0 : TEXCOORD0;
            #endif
            #if UNITY_ANY_INSTANCING_ENABLED
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
            uint instanceID : INSTANCEID_SEMANTIC;
            #endif
            #endif
        };
        struct Varyings
        {
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
            float4 positionCS : SV_POSITION;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
            float3 positionWS;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
            float3 normalWS;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
            float4 tangentWS;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
            float4 texCoord0;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
            float3 viewDirectionWS;
            #endif
            #if UNITY_ANY_INSTANCING_ENABLED
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
            uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
            uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
            uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
            FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
            #endif
        };
        struct SurfaceDescriptionInputs
        {
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
            float3 WorldSpaceNormal;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
            float3 WorldSpaceTangent;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
            float3 WorldSpaceBiTangent;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
            float3 WorldSpaceViewDirection;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
            float3 WorldSpacePosition;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
            float3 AbsoluteWorldSpacePosition;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
            float4 ScreenPosition;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
            float4 uv0;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
            float3 TimeParameters;
            #endif
        };
        struct VertexDescriptionInputs
        {
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
            float3 ObjectSpaceNormal;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
            float3 ObjectSpaceTangent;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
            float3 ObjectSpacePosition;
            #endif
        };
        struct PackedVaryings
        {
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
            float4 positionCS : SV_POSITION;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
            float3 interp0 : TEXCOORD0;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
            float3 interp1 : TEXCOORD1;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
            float4 interp2 : TEXCOORD2;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
            float4 interp3 : TEXCOORD3;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
            float3 interp4 : TEXCOORD4;
            #endif
            #if UNITY_ANY_INSTANCING_ENABLED
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
            uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
            uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
            uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
            FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
            #endif
        };

            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
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
        float4 Texture2D_9d2dc686cea4411eb4cd42e0ad89db69_TexelSize;
        float Vector1_eba81f4ec9494ffc89d831e650dc2eee;
        float Boolean_746f00cdd21a463a8f7db0eed7f65639;
        float NORMALMAP_ON;
        float4 Texture2D_1709bfc569f34021a1205546b253ac2b_TexelSize;
        float Vector1_83717f647fb949a0b96857d7ef1efef2;
        float FLOWMAP_ON;
        float4 Texture2D_9e7ce96f70ec4e4fad0aca3a397849ab_TexelSize;
        float Vector1_693d76d6835a455fba9b2a18be0e2bec;
        float Vector1_da40d7438c114e189f67f755323f75ea;
        float EMISSIONMAP_ON;
        float4 Texture2D_ea1c329133fc4f01a0ae24d3d7ecfc16_TexelSize;
        float Vector1_a62ab1db54d34856836ce0b6d78fe08f;
        float Vector1_c092b1ab83a845279db3044fddc481d7;
        float Boolean_1;
        float Vector1_1;
        float Vector1_38eeab6d9e3146b49202d29537ff230c;
        float Boolean_101eac5857d84f2b8c80fed3fbaa437b;
        float4 Texture2D_408732e7caa34af796ddb77be6aaabea_TexelSize;
        CBUFFER_END

        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(Texture2D_9d2dc686cea4411eb4cd42e0ad89db69);
        SAMPLER(samplerTexture2D_9d2dc686cea4411eb4cd42e0ad89db69);
        TEXTURE2D(Texture2D_1709bfc569f34021a1205546b253ac2b);
        SAMPLER(samplerTexture2D_1709bfc569f34021a1205546b253ac2b);
        TEXTURE2D(Texture2D_9e7ce96f70ec4e4fad0aca3a397849ab);
        SAMPLER(samplerTexture2D_9e7ce96f70ec4e4fad0aca3a397849ab);
        TEXTURE2D(Texture2D_ea1c329133fc4f01a0ae24d3d7ecfc16);
        SAMPLER(samplerTexture2D_ea1c329133fc4f01a0ae24d3d7ecfc16);
        TEXTURE2D(Texture2D_408732e7caa34af796ddb77be6aaabea);
        SAMPLER(samplerTexture2D_408732e7caa34af796ddb77be6aaabea);

            // Graph Functions
            
        void Unity_Multiply_float(float A, float B, out float Out)
        {
            Out = A * B;
        }

        void Unity_Branch_float(float Predicate, float True, float False, out float Out)
        {
            Out = Predicate ? True : False;
        }

        // 14232a670a847cf5d9a67762c8e9c727
        #include "Assets/Rendering/Shaders/Functions/ShaderGraphCel.hlsl"

        struct Bindings_FlowMapAgain_fc942ebb7f073b34c95020e7e9594c1d
        {
        };

        void SG_FlowMapAgain_fc942ebb7f073b34c95020e7e9594c1d(UnityTexture2D Texture2D_438fde66a71545d0881e899d13c392d9, UnityTexture2D Texture2D_e25242356fdd43d0a4c0acd4f27c1b6c, float4 Vector4_45eaa08e83d6442a8c1c17913a80c3f9, float Vector1_bb84133cb040419d98935441d68aa202, float Vector1_6d347278dbed49b99401dc21e9b096d8, float Boolean_51360f658f9b44cbbecfc398bd80b57d, Bindings_FlowMapAgain_fc942ebb7f073b34c95020e7e9594c1d IN, out half4 FlowColor_1)
        {
            UnityTexture2D _Property_31480d3b368d49c4a2881a238a4e2be5_Out_0 = Texture2D_438fde66a71545d0881e899d13c392d9;
            UnityTexture2D _Property_838db75420d843a79a1be455bd2fe17b_Out_0 = Texture2D_e25242356fdd43d0a4c0acd4f27c1b6c;
            float4 _Property_a26b31063dee40f598d76b497a18b741_Out_0 = Vector4_45eaa08e83d6442a8c1c17913a80c3f9;
            float _Property_e9c222f421174ba98ff2b9fe23bfacd7_Out_0 = Vector1_bb84133cb040419d98935441d68aa202;
            float _Property_cc3f06cd8cd840ac93f57a82efb62351_Out_0 = Vector1_6d347278dbed49b99401dc21e9b096d8;
            float _Property_ba22938ef7d24a7e99ac4600f95ce8c4_Out_0 = Boolean_51360f658f9b44cbbecfc398bd80b57d;
            half4 _FlowMapCustomFunction_8a07381e71124a159105c737e309885c_FlowColor_4;
            FlowMap_half(_Property_31480d3b368d49c4a2881a238a4e2be5_Out_0, _Property_838db75420d843a79a1be455bd2fe17b_Out_0, (_Property_a26b31063dee40f598d76b497a18b741_Out_0.xy), _Property_e9c222f421174ba98ff2b9fe23bfacd7_Out_0, _Property_cc3f06cd8cd840ac93f57a82efb62351_Out_0, _Property_ba22938ef7d24a7e99ac4600f95ce8c4_Out_0, _FlowMapCustomFunction_8a07381e71124a159105c737e309885c_FlowColor_4);
            FlowColor_1 = _FlowMapCustomFunction_8a07381e71124a159105c737e309885c_FlowColor_4;
        }

        void Unity_NormalStrength_float(float3 In, float Strength, out float3 Out)
        {
            Out = float3(In.rg * Strength, lerp(1, In.b, saturate(Strength)));
        }

        void Unity_Branch_float3(float Predicate, float3 True, float3 False, out float3 Out)
        {
            Out = Predicate ? True : False;
        }

        void Unity_Add_float3(float3 A, float3 B, out float3 Out)
        {
            Out = A + B;
        }

        void Unity_Normalize_float3(float3 In, out float3 Out)
        {
            Out = normalize(In);
        }

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

        void Unity_Multiply_float(float3 A, float3 B, out float3 Out)
        {
            Out = A * B;
        }

        void Unity_Saturation_float(float3 In, float Saturation, out float3 Out)
        {
            float luma = dot(In, float3(0.2126729, 0.7151522, 0.0721750));
            Out =  luma.xxx + Saturation.xxx * (In - luma.xxx);
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

        void Unity_Distance_float3(float3 A, float3 B, out float Out)
        {
            Out = distance(A, B);
        }

        void Unity_Subtract_float(float A, float B, out float Out)
        {
            Out = A - B;
        }

        void Unity_Dither_float(float In, float4 ScreenPosition, out float Out)
        {
            float2 uv = ScreenPosition.xy * _ScreenParams.xy;
            float DITHER_THRESHOLDS[16] =
            {
                1.0 / 17.0,  9.0 / 17.0,  3.0 / 17.0, 11.0 / 17.0,
                13.0 / 17.0,  5.0 / 17.0, 15.0 / 17.0,  7.0 / 17.0,
                4.0 / 17.0, 12.0 / 17.0,  2.0 / 17.0, 10.0 / 17.0,
                16.0 / 17.0,  8.0 / 17.0, 14.0 / 17.0,  6.0 / 17.0
            };
            uint index = (uint(uv.x) % 4) * 4 + uint(uv.y) % 4;
            Out = In - DITHER_THRESHOLDS[index];
        }

        struct Bindings_Dither_9b41370f0c5105847b2fec9036934da7
        {
            float3 WorldSpacePosition;
            float4 ScreenPosition;
        };

        void SG_Dither_9b41370f0c5105847b2fec9036934da7(float Boolean_75fab38b788b41d18702451bf97d8974, Bindings_Dither_9b41370f0c5105847b2fec9036934da7 IN, out float DistanceAlpha_2, out float DitherPattern_1)
        {
            float _Property_0a971a5d36084b7daed36160bb4ab6eb_Out_0 = Boolean_75fab38b788b41d18702451bf97d8974;
            float _Distance_37c8da6819a54f9b84aad0aa27264adc_Out_2;
            Unity_Distance_float3(_WorldSpaceCameraPos, IN.WorldSpacePosition, _Distance_37c8da6819a54f9b84aad0aa27264adc_Out_2);
            float _Subtract_8cbd49a21ebd4e5ea3c8c363207c9228_Out_2;
            Unity_Subtract_float(_Distance_37c8da6819a54f9b84aad0aa27264adc_Out_2, 0.25, _Subtract_8cbd49a21ebd4e5ea3c8c363207c9228_Out_2);
            float _Multiply_2e703422f1eb441592311108b388c71d_Out_2;
            Unity_Multiply_float(_Subtract_8cbd49a21ebd4e5ea3c8c363207c9228_Out_2, 3, _Multiply_2e703422f1eb441592311108b388c71d_Out_2);
            float _Branch_875139a8f00d481bbd14b6aed3251ccb_Out_3;
            Unity_Branch_float(_Property_0a971a5d36084b7daed36160bb4ab6eb_Out_0, _Multiply_2e703422f1eb441592311108b388c71d_Out_2, 1, _Branch_875139a8f00d481bbd14b6aed3251ccb_Out_3);
            float _Dither_82f502cb016d47018f8c6a7dade171fe_Out_2;
            Unity_Dither_float(1, float4(IN.ScreenPosition.xy / IN.ScreenPosition.w, 0, 0), _Dither_82f502cb016d47018f8c6a7dade171fe_Out_2);
            float _Branch_6efb7f3c563344f9aee2d13c5a52c8e3_Out_3;
            Unity_Branch_float(_Property_0a971a5d36084b7daed36160bb4ab6eb_Out_0, _Dither_82f502cb016d47018f8c6a7dade171fe_Out_2, 0, _Branch_6efb7f3c563344f9aee2d13c5a52c8e3_Out_3);
            DistanceAlpha_2 = _Branch_875139a8f00d481bbd14b6aed3251ccb_Out_3;
            DitherPattern_1 = _Branch_6efb7f3c563344f9aee2d13c5a52c8e3_Out_3;
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
            float AlphaClipThreshold;
        };

        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
            UnityTexture2D _Property_2d650f0660bb4d38bf5f09cedaee98cd_Out_0 = UnityBuildTexture2DStructNoScale(Texture2D_9d2dc686cea4411eb4cd42e0ad89db69);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
            UnityTexture2D _Property_2d76057f23694095af98da3226a7faee_Out_0 = UnityBuildTexture2DStructNoScale(Texture2D_9e7ce96f70ec4e4fad0aca3a397849ab);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
            float4 _UV_c5bca71bad4b49f992c8d5ba39de058c_Out_0 = IN.uv0;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
            float _Property_5912c8450dd24f14a3bb3903d52671cc_Out_0 = Vector1_693d76d6835a455fba9b2a18be0e2bec;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
            float _Multiply_ba2126df2a8c4839bb16eee6ebaf69b2_Out_2;
            Unity_Multiply_float(_Property_5912c8450dd24f14a3bb3903d52671cc_Out_0, IN.TimeParameters.x, _Multiply_ba2126df2a8c4839bb16eee6ebaf69b2_Out_2);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
            float _Property_374ac4d4f39346049a1730f927453584_Out_0 = FLOWMAP_ON;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
            float _Property_f96f66447b4b4d23848d760aea4a6c3b_Out_0 = Vector1_da40d7438c114e189f67f755323f75ea;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
            float _Branch_80de9b6c75ee445f9fa8a1cf23426a99_Out_3;
            Unity_Branch_float(_Property_374ac4d4f39346049a1730f927453584_Out_0, _Property_f96f66447b4b4d23848d760aea4a6c3b_Out_0, 0, _Branch_80de9b6c75ee445f9fa8a1cf23426a99_Out_3);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
            Bindings_FlowMapAgain_fc942ebb7f073b34c95020e7e9594c1d _FlowMapAgain_bcea8020458540d1acec52bd36104aae;
            half4 _FlowMapAgain_bcea8020458540d1acec52bd36104aae_FlowColor_1;
            SG_FlowMapAgain_fc942ebb7f073b34c95020e7e9594c1d(_Property_2d650f0660bb4d38bf5f09cedaee98cd_Out_0, _Property_2d76057f23694095af98da3226a7faee_Out_0, _UV_c5bca71bad4b49f992c8d5ba39de058c_Out_0, _Multiply_ba2126df2a8c4839bb16eee6ebaf69b2_Out_2, _Branch_80de9b6c75ee445f9fa8a1cf23426a99_Out_3, 0, _FlowMapAgain_bcea8020458540d1acec52bd36104aae, _FlowMapAgain_bcea8020458540d1acec52bd36104aae_FlowColor_1);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
            float _Property_f3158fd43f49461c90185566a8219d83_Out_0 = NORMALMAP_ON;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
            UnityTexture2D _Property_7ed04db0e0ac4201bd9dcc36f02604b0_Out_0 = UnityBuildTexture2DStructNoScale(Texture2D_1709bfc569f34021a1205546b253ac2b);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
            Bindings_FlowMapAgain_fc942ebb7f073b34c95020e7e9594c1d _FlowMapAgain_11ae490cfcbe4738b3bd288b3402b87d;
            half4 _FlowMapAgain_11ae490cfcbe4738b3bd288b3402b87d_FlowColor_1;
            SG_FlowMapAgain_fc942ebb7f073b34c95020e7e9594c1d(_Property_7ed04db0e0ac4201bd9dcc36f02604b0_Out_0, _Property_2d76057f23694095af98da3226a7faee_Out_0, _UV_c5bca71bad4b49f992c8d5ba39de058c_Out_0, _Multiply_ba2126df2a8c4839bb16eee6ebaf69b2_Out_2, _Branch_80de9b6c75ee445f9fa8a1cf23426a99_Out_3, 1, _FlowMapAgain_11ae490cfcbe4738b3bd288b3402b87d, _FlowMapAgain_11ae490cfcbe4738b3bd288b3402b87d_FlowColor_1);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
            float _Property_43a5ee13e5da4cfe84bb62af6fdb1d7f_Out_0 = Vector1_83717f647fb949a0b96857d7ef1efef2;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
            float3 _NormalStrength_959d184e9ceb49f1982ad107ee63c1cd_Out_2;
            Unity_NormalStrength_float((_FlowMapAgain_11ae490cfcbe4738b3bd288b3402b87d_FlowColor_1.xyz), _Property_43a5ee13e5da4cfe84bb62af6fdb1d7f_Out_0, _NormalStrength_959d184e9ceb49f1982ad107ee63c1cd_Out_2);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
            float3 _Branch_8cc0465e516546a593af84bd338adc2a_Out_3;
            Unity_Branch_float3(_Property_f3158fd43f49461c90185566a8219d83_Out_0, _NormalStrength_959d184e9ceb49f1982ad107ee63c1cd_Out_2, float3(0, 0, 1), _Branch_8cc0465e516546a593af84bd338adc2a_Out_3);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
            float _Property_e55436ab1ab0470cb5255f52938e4168_Out_0 = Vector1_1;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
            float _Property_0534d185de7d41c1ab3d55da040fffe6_Out_0 = Vector1_38eeab6d9e3146b49202d29537ff230c;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
            UnityTexture2D _Property_dcf9b48cb3b849b2b2b8253150bb5121_Out_0 = UnityBuildTexture2DStructNoScale(Texture2D_408732e7caa34af796ddb77be6aaabea);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
            Bindings_CelShading_802007f66a3e13d42802a3875ed35370 _CelShading_f5c658ba84a743b29c36c275531286d9;
            _CelShading_f5c658ba84a743b29c36c275531286d9.WorldSpaceNormal = IN.WorldSpaceNormal;
            _CelShading_f5c658ba84a743b29c36c275531286d9.WorldSpaceTangent = IN.WorldSpaceTangent;
            _CelShading_f5c658ba84a743b29c36c275531286d9.WorldSpaceBiTangent = IN.WorldSpaceBiTangent;
            _CelShading_f5c658ba84a743b29c36c275531286d9.WorldSpaceViewDirection = IN.WorldSpaceViewDirection;
            _CelShading_f5c658ba84a743b29c36c275531286d9.AbsoluteWorldSpacePosition = IN.AbsoluteWorldSpacePosition;
            _CelShading_f5c658ba84a743b29c36c275531286d9.uv0 = IN.uv0;
            half4 _CelShading_f5c658ba84a743b29c36c275531286d9_Color_1;
            SG_CelShading_802007f66a3e13d42802a3875ed35370(_FlowMapAgain_bcea8020458540d1acec52bd36104aae_FlowColor_1, _Branch_8cc0465e516546a593af84bd338adc2a_Out_3, _Property_e55436ab1ab0470cb5255f52938e4168_Out_0, _Property_0534d185de7d41c1ab3d55da040fffe6_Out_0, _Property_dcf9b48cb3b849b2b2b8253150bb5121_Out_0, _CelShading_f5c658ba84a743b29c36c275531286d9, _CelShading_f5c658ba84a743b29c36c275531286d9_Color_1);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
            UnityTexture2D _Property_d7f0ad396ff04624879f4577d54429bb_Out_0 = UnityBuildTexture2DStructNoScale(Texture2D_ea1c329133fc4f01a0ae24d3d7ecfc16);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
            float4 _SampleTexture2D_8eb61b4203f44272a54e7d80a2514d18_RGBA_0 = SAMPLE_TEXTURE2D(_Property_d7f0ad396ff04624879f4577d54429bb_Out_0.tex, _Property_d7f0ad396ff04624879f4577d54429bb_Out_0.samplerstate, IN.uv0.xy);
            float _SampleTexture2D_8eb61b4203f44272a54e7d80a2514d18_R_4 = _SampleTexture2D_8eb61b4203f44272a54e7d80a2514d18_RGBA_0.r;
            float _SampleTexture2D_8eb61b4203f44272a54e7d80a2514d18_G_5 = _SampleTexture2D_8eb61b4203f44272a54e7d80a2514d18_RGBA_0.g;
            float _SampleTexture2D_8eb61b4203f44272a54e7d80a2514d18_B_6 = _SampleTexture2D_8eb61b4203f44272a54e7d80a2514d18_RGBA_0.b;
            float _SampleTexture2D_8eb61b4203f44272a54e7d80a2514d18_A_7 = _SampleTexture2D_8eb61b4203f44272a54e7d80a2514d18_RGBA_0.a;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
            float _Property_e66795ed672b4c54aebf2255e6f49694_Out_0 = Vector1_a62ab1db54d34856836ce0b6d78fe08f;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
            float _Property_c9ab50e169504622906236aa02a3e4c1_Out_0 = Vector1_c092b1ab83a845279db3044fddc481d7;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
            float _Property_e8b08fad777c4daf8e21fc2e116ec4fc_Out_0 = EMISSIONMAP_ON;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
            Bindings_Emission_1c7d3173c45db194bad26a66d6783e8c _Emission_0c52d2cccf6f4e5bb3d0f691e7e87a81;
            float3 _Emission_0c52d2cccf6f4e5bb3d0f691e7e87a81_FinalColor_1;
            SG_Emission_1c7d3173c45db194bad26a66d6783e8c((_CelShading_f5c658ba84a743b29c36c275531286d9_Color_1.xyz), _SampleTexture2D_8eb61b4203f44272a54e7d80a2514d18_RGBA_0, _Property_e66795ed672b4c54aebf2255e6f49694_Out_0, _Property_c9ab50e169504622906236aa02a3e4c1_Out_0, _Property_e8b08fad777c4daf8e21fc2e116ec4fc_Out_0, _Emission_0c52d2cccf6f4e5bb3d0f691e7e87a81, _Emission_0c52d2cccf6f4e5bb3d0f691e7e87a81_FinalColor_1);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
            float _Property_26183fe36ab7437d89f981a749ccb7d1_Out_0 = Boolean_746f00cdd21a463a8f7db0eed7f65639;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
            Bindings_Dither_9b41370f0c5105847b2fec9036934da7 _Dither_f41700f34e7844b6ba89f6bb33dde6c6;
            _Dither_f41700f34e7844b6ba89f6bb33dde6c6.WorldSpacePosition = IN.WorldSpacePosition;
            _Dither_f41700f34e7844b6ba89f6bb33dde6c6.ScreenPosition = IN.ScreenPosition;
            float _Dither_f41700f34e7844b6ba89f6bb33dde6c6_DistanceAlpha_2;
            float _Dither_f41700f34e7844b6ba89f6bb33dde6c6_DitherPattern_1;
            SG_Dither_9b41370f0c5105847b2fec9036934da7(_Property_26183fe36ab7437d89f981a749ccb7d1_Out_0, _Dither_f41700f34e7844b6ba89f6bb33dde6c6, _Dither_f41700f34e7844b6ba89f6bb33dde6c6_DistanceAlpha_2, _Dither_f41700f34e7844b6ba89f6bb33dde6c6_DitherPattern_1);
            #endif
            surface.BaseColor = _Emission_0c52d2cccf6f4e5bb3d0f691e7e87a81_FinalColor_1;
            surface.Alpha = _Dither_f41700f34e7844b6ba89f6bb33dde6c6_DistanceAlpha_2;
            surface.AlphaClipThreshold = _Dither_f41700f34e7844b6ba89f6bb33dde6c6_DitherPattern_1;
            return surface;
        }

            // --------------------------------------------------
            // Build Graph Inputs

            VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
        {
            VertexDescriptionInputs output;
            ZERO_INITIALIZE(VertexDescriptionInputs, output);

        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
        output.ObjectSpaceNormal =           input.normalOS;
        #endif

        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
        output.ObjectSpaceTangent =          input.tangentOS.xyz;
        #endif

        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
        output.ObjectSpacePosition =         input.positionOS;
        #endif


            return output;
        }
            SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);

        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
        // must use interpolated tangent, bitangent and normal before they are normalized in the pixel shader.
        #endif

        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
        float3 unnormalizedNormalWS = input.normalWS;
        #endif

        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
        const float renormFactor = 1.0 / length(unnormalizedNormalWS);
        #endif


        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
        // use bitangent on the fly like in hdrp
        #endif

        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
        // IMPORTANT! If we ever support Flip on double sided materials ensure bitangent and tangent are NOT flipped.
        #endif

        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
        float crossSign = (input.tangentWS.w > 0.0 ? 1.0 : -1.0) * GetOddNegativeScale();
        #endif

        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
        float3 bitang = crossSign * cross(input.normalWS.xyz, input.tangentWS.xyz);
        #endif


        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
        output.WorldSpaceNormal =            renormFactor*input.normalWS.xyz;		// we want a unit length Normal Vector node in shader graph
        #endif


        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
        // to preserve mikktspace compliance we use same scale renormFactor as was used on the normal.
        #endif

        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
        // This is explained in section 2.2 in "surface gradient based bump mapping framework"
        #endif

        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
        output.WorldSpaceTangent =           renormFactor*input.tangentWS.xyz;
        #endif

        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
        output.WorldSpaceBiTangent =         renormFactor*bitang;
        #endif


        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
        output.WorldSpaceViewDirection =     input.viewDirectionWS; //TODO: by default normalized in HD, but not in universal
        #endif

        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
        output.WorldSpacePosition =          input.positionWS;
        #endif

        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
        output.AbsoluteWorldSpacePosition =  GetAbsolutePositionWS(input.positionWS);
        #endif

        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
        output.ScreenPosition =              ComputeScreenPos(TransformWorldToHClip(input.positionWS), _ProjectionParams.x);
        #endif

        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
        output.uv0 =                         input.texCoord0;
        #endif

        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
        output.TimeParameters =              _TimeParameters.xyz; // This is mainly for LW as HD overwrite this value
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
            #pragma shader_feature_local _ CEL_SPECULAR_ON
        #pragma shader_feature_local _ CEL_ACCENT_ON

        #if defined(CEL_SPECULAR_ON) && defined(CEL_ACCENT_ON)
            #define KEYWORD_PERMUTATION_0
        #elif defined(CEL_SPECULAR_ON)
            #define KEYWORD_PERMUTATION_1
        #elif defined(CEL_ACCENT_ON)
            #define KEYWORD_PERMUTATION_2
        #else
            #define KEYWORD_PERMUTATION_3
        #endif


            // Defines
        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
        #define _AlphaClip 1
        #endif

        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
        #define ATTRIBUTES_NEED_NORMAL
        #endif

        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
        #define ATTRIBUTES_NEED_TANGENT
        #endif

        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
        #define VARYINGS_NEED_POSITION_WS
        #endif

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
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
            float3 positionOS : POSITION;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
            float3 normalOS : NORMAL;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
            float4 tangentOS : TANGENT;
            #endif
            #if UNITY_ANY_INSTANCING_ENABLED
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
            uint instanceID : INSTANCEID_SEMANTIC;
            #endif
            #endif
        };
        struct Varyings
        {
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
            float4 positionCS : SV_POSITION;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
            float3 positionWS;
            #endif
            #if UNITY_ANY_INSTANCING_ENABLED
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
            uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
            uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
            uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
            FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
            #endif
        };
        struct SurfaceDescriptionInputs
        {
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
            float3 WorldSpacePosition;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
            float4 ScreenPosition;
            #endif
        };
        struct VertexDescriptionInputs
        {
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
            float3 ObjectSpaceNormal;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
            float3 ObjectSpaceTangent;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
            float3 ObjectSpacePosition;
            #endif
        };
        struct PackedVaryings
        {
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
            float4 positionCS : SV_POSITION;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
            float3 interp0 : TEXCOORD0;
            #endif
            #if UNITY_ANY_INSTANCING_ENABLED
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
            uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
            uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
            uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
            FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
            #endif
        };

            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
        PackedVaryings PackVaryings (Varyings input)
        {
            PackedVaryings output;
            output.positionCS = input.positionCS;
            output.interp0.xyz =  input.positionWS;
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
        float4 Texture2D_9d2dc686cea4411eb4cd42e0ad89db69_TexelSize;
        float Vector1_eba81f4ec9494ffc89d831e650dc2eee;
        float Boolean_746f00cdd21a463a8f7db0eed7f65639;
        float NORMALMAP_ON;
        float4 Texture2D_1709bfc569f34021a1205546b253ac2b_TexelSize;
        float Vector1_83717f647fb949a0b96857d7ef1efef2;
        float FLOWMAP_ON;
        float4 Texture2D_9e7ce96f70ec4e4fad0aca3a397849ab_TexelSize;
        float Vector1_693d76d6835a455fba9b2a18be0e2bec;
        float Vector1_da40d7438c114e189f67f755323f75ea;
        float EMISSIONMAP_ON;
        float4 Texture2D_ea1c329133fc4f01a0ae24d3d7ecfc16_TexelSize;
        float Vector1_a62ab1db54d34856836ce0b6d78fe08f;
        float Vector1_c092b1ab83a845279db3044fddc481d7;
        float Boolean_1;
        float Vector1_1;
        float Vector1_38eeab6d9e3146b49202d29537ff230c;
        float Boolean_101eac5857d84f2b8c80fed3fbaa437b;
        float4 Texture2D_408732e7caa34af796ddb77be6aaabea_TexelSize;
        CBUFFER_END

        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(Texture2D_9d2dc686cea4411eb4cd42e0ad89db69);
        SAMPLER(samplerTexture2D_9d2dc686cea4411eb4cd42e0ad89db69);
        TEXTURE2D(Texture2D_1709bfc569f34021a1205546b253ac2b);
        SAMPLER(samplerTexture2D_1709bfc569f34021a1205546b253ac2b);
        TEXTURE2D(Texture2D_9e7ce96f70ec4e4fad0aca3a397849ab);
        SAMPLER(samplerTexture2D_9e7ce96f70ec4e4fad0aca3a397849ab);
        TEXTURE2D(Texture2D_ea1c329133fc4f01a0ae24d3d7ecfc16);
        SAMPLER(samplerTexture2D_ea1c329133fc4f01a0ae24d3d7ecfc16);
        TEXTURE2D(Texture2D_408732e7caa34af796ddb77be6aaabea);
        SAMPLER(samplerTexture2D_408732e7caa34af796ddb77be6aaabea);

            // Graph Functions
            
        void Unity_Distance_float3(float3 A, float3 B, out float Out)
        {
            Out = distance(A, B);
        }

        void Unity_Subtract_float(float A, float B, out float Out)
        {
            Out = A - B;
        }

        void Unity_Multiply_float(float A, float B, out float Out)
        {
            Out = A * B;
        }

        void Unity_Branch_float(float Predicate, float True, float False, out float Out)
        {
            Out = Predicate ? True : False;
        }

        void Unity_Dither_float(float In, float4 ScreenPosition, out float Out)
        {
            float2 uv = ScreenPosition.xy * _ScreenParams.xy;
            float DITHER_THRESHOLDS[16] =
            {
                1.0 / 17.0,  9.0 / 17.0,  3.0 / 17.0, 11.0 / 17.0,
                13.0 / 17.0,  5.0 / 17.0, 15.0 / 17.0,  7.0 / 17.0,
                4.0 / 17.0, 12.0 / 17.0,  2.0 / 17.0, 10.0 / 17.0,
                16.0 / 17.0,  8.0 / 17.0, 14.0 / 17.0,  6.0 / 17.0
            };
            uint index = (uint(uv.x) % 4) * 4 + uint(uv.y) % 4;
            Out = In - DITHER_THRESHOLDS[index];
        }

        struct Bindings_Dither_9b41370f0c5105847b2fec9036934da7
        {
            float3 WorldSpacePosition;
            float4 ScreenPosition;
        };

        void SG_Dither_9b41370f0c5105847b2fec9036934da7(float Boolean_75fab38b788b41d18702451bf97d8974, Bindings_Dither_9b41370f0c5105847b2fec9036934da7 IN, out float DistanceAlpha_2, out float DitherPattern_1)
        {
            float _Property_0a971a5d36084b7daed36160bb4ab6eb_Out_0 = Boolean_75fab38b788b41d18702451bf97d8974;
            float _Distance_37c8da6819a54f9b84aad0aa27264adc_Out_2;
            Unity_Distance_float3(_WorldSpaceCameraPos, IN.WorldSpacePosition, _Distance_37c8da6819a54f9b84aad0aa27264adc_Out_2);
            float _Subtract_8cbd49a21ebd4e5ea3c8c363207c9228_Out_2;
            Unity_Subtract_float(_Distance_37c8da6819a54f9b84aad0aa27264adc_Out_2, 0.25, _Subtract_8cbd49a21ebd4e5ea3c8c363207c9228_Out_2);
            float _Multiply_2e703422f1eb441592311108b388c71d_Out_2;
            Unity_Multiply_float(_Subtract_8cbd49a21ebd4e5ea3c8c363207c9228_Out_2, 3, _Multiply_2e703422f1eb441592311108b388c71d_Out_2);
            float _Branch_875139a8f00d481bbd14b6aed3251ccb_Out_3;
            Unity_Branch_float(_Property_0a971a5d36084b7daed36160bb4ab6eb_Out_0, _Multiply_2e703422f1eb441592311108b388c71d_Out_2, 1, _Branch_875139a8f00d481bbd14b6aed3251ccb_Out_3);
            float _Dither_82f502cb016d47018f8c6a7dade171fe_Out_2;
            Unity_Dither_float(1, float4(IN.ScreenPosition.xy / IN.ScreenPosition.w, 0, 0), _Dither_82f502cb016d47018f8c6a7dade171fe_Out_2);
            float _Branch_6efb7f3c563344f9aee2d13c5a52c8e3_Out_3;
            Unity_Branch_float(_Property_0a971a5d36084b7daed36160bb4ab6eb_Out_0, _Dither_82f502cb016d47018f8c6a7dade171fe_Out_2, 0, _Branch_6efb7f3c563344f9aee2d13c5a52c8e3_Out_3);
            DistanceAlpha_2 = _Branch_875139a8f00d481bbd14b6aed3251ccb_Out_3;
            DitherPattern_1 = _Branch_6efb7f3c563344f9aee2d13c5a52c8e3_Out_3;
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
            float Alpha;
            float AlphaClipThreshold;
        };

        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
            float _Property_26183fe36ab7437d89f981a749ccb7d1_Out_0 = Boolean_746f00cdd21a463a8f7db0eed7f65639;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
            Bindings_Dither_9b41370f0c5105847b2fec9036934da7 _Dither_f41700f34e7844b6ba89f6bb33dde6c6;
            _Dither_f41700f34e7844b6ba89f6bb33dde6c6.WorldSpacePosition = IN.WorldSpacePosition;
            _Dither_f41700f34e7844b6ba89f6bb33dde6c6.ScreenPosition = IN.ScreenPosition;
            float _Dither_f41700f34e7844b6ba89f6bb33dde6c6_DistanceAlpha_2;
            float _Dither_f41700f34e7844b6ba89f6bb33dde6c6_DitherPattern_1;
            SG_Dither_9b41370f0c5105847b2fec9036934da7(_Property_26183fe36ab7437d89f981a749ccb7d1_Out_0, _Dither_f41700f34e7844b6ba89f6bb33dde6c6, _Dither_f41700f34e7844b6ba89f6bb33dde6c6_DistanceAlpha_2, _Dither_f41700f34e7844b6ba89f6bb33dde6c6_DitherPattern_1);
            #endif
            surface.Alpha = _Dither_f41700f34e7844b6ba89f6bb33dde6c6_DistanceAlpha_2;
            surface.AlphaClipThreshold = _Dither_f41700f34e7844b6ba89f6bb33dde6c6_DitherPattern_1;
            return surface;
        }

            // --------------------------------------------------
            // Build Graph Inputs

            VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
        {
            VertexDescriptionInputs output;
            ZERO_INITIALIZE(VertexDescriptionInputs, output);

        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
        output.ObjectSpaceNormal =           input.normalOS;
        #endif

        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
        output.ObjectSpaceTangent =          input.tangentOS.xyz;
        #endif

        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
        output.ObjectSpacePosition =         input.positionOS;
        #endif


            return output;
        }
            SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);





        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
        output.WorldSpacePosition =          input.positionWS;
        #endif

        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
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
            #pragma shader_feature_local _ CEL_SPECULAR_ON
        #pragma shader_feature_local _ CEL_ACCENT_ON

        #if defined(CEL_SPECULAR_ON) && defined(CEL_ACCENT_ON)
            #define KEYWORD_PERMUTATION_0
        #elif defined(CEL_SPECULAR_ON)
            #define KEYWORD_PERMUTATION_1
        #elif defined(CEL_ACCENT_ON)
            #define KEYWORD_PERMUTATION_2
        #else
            #define KEYWORD_PERMUTATION_3
        #endif


            // Defines
        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
        #define _AlphaClip 1
        #endif

        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
        #define ATTRIBUTES_NEED_NORMAL
        #endif

        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
        #define ATTRIBUTES_NEED_TANGENT
        #endif

        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
        #define VARYINGS_NEED_POSITION_WS
        #endif

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
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
            float3 positionOS : POSITION;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
            float3 normalOS : NORMAL;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
            float4 tangentOS : TANGENT;
            #endif
            #if UNITY_ANY_INSTANCING_ENABLED
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
            uint instanceID : INSTANCEID_SEMANTIC;
            #endif
            #endif
        };
        struct Varyings
        {
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
            float4 positionCS : SV_POSITION;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
            float3 positionWS;
            #endif
            #if UNITY_ANY_INSTANCING_ENABLED
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
            uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
            uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
            uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
            FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
            #endif
        };
        struct SurfaceDescriptionInputs
        {
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
            float3 WorldSpacePosition;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
            float4 ScreenPosition;
            #endif
        };
        struct VertexDescriptionInputs
        {
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
            float3 ObjectSpaceNormal;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
            float3 ObjectSpaceTangent;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
            float3 ObjectSpacePosition;
            #endif
        };
        struct PackedVaryings
        {
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
            float4 positionCS : SV_POSITION;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
            float3 interp0 : TEXCOORD0;
            #endif
            #if UNITY_ANY_INSTANCING_ENABLED
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
            uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
            uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
            uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
            FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
            #endif
        };

            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
        PackedVaryings PackVaryings (Varyings input)
        {
            PackedVaryings output;
            output.positionCS = input.positionCS;
            output.interp0.xyz =  input.positionWS;
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
        float4 Texture2D_9d2dc686cea4411eb4cd42e0ad89db69_TexelSize;
        float Vector1_eba81f4ec9494ffc89d831e650dc2eee;
        float Boolean_746f00cdd21a463a8f7db0eed7f65639;
        float NORMALMAP_ON;
        float4 Texture2D_1709bfc569f34021a1205546b253ac2b_TexelSize;
        float Vector1_83717f647fb949a0b96857d7ef1efef2;
        float FLOWMAP_ON;
        float4 Texture2D_9e7ce96f70ec4e4fad0aca3a397849ab_TexelSize;
        float Vector1_693d76d6835a455fba9b2a18be0e2bec;
        float Vector1_da40d7438c114e189f67f755323f75ea;
        float EMISSIONMAP_ON;
        float4 Texture2D_ea1c329133fc4f01a0ae24d3d7ecfc16_TexelSize;
        float Vector1_a62ab1db54d34856836ce0b6d78fe08f;
        float Vector1_c092b1ab83a845279db3044fddc481d7;
        float Boolean_1;
        float Vector1_1;
        float Vector1_38eeab6d9e3146b49202d29537ff230c;
        float Boolean_101eac5857d84f2b8c80fed3fbaa437b;
        float4 Texture2D_408732e7caa34af796ddb77be6aaabea_TexelSize;
        CBUFFER_END

        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(Texture2D_9d2dc686cea4411eb4cd42e0ad89db69);
        SAMPLER(samplerTexture2D_9d2dc686cea4411eb4cd42e0ad89db69);
        TEXTURE2D(Texture2D_1709bfc569f34021a1205546b253ac2b);
        SAMPLER(samplerTexture2D_1709bfc569f34021a1205546b253ac2b);
        TEXTURE2D(Texture2D_9e7ce96f70ec4e4fad0aca3a397849ab);
        SAMPLER(samplerTexture2D_9e7ce96f70ec4e4fad0aca3a397849ab);
        TEXTURE2D(Texture2D_ea1c329133fc4f01a0ae24d3d7ecfc16);
        SAMPLER(samplerTexture2D_ea1c329133fc4f01a0ae24d3d7ecfc16);
        TEXTURE2D(Texture2D_408732e7caa34af796ddb77be6aaabea);
        SAMPLER(samplerTexture2D_408732e7caa34af796ddb77be6aaabea);

            // Graph Functions
            
        void Unity_Distance_float3(float3 A, float3 B, out float Out)
        {
            Out = distance(A, B);
        }

        void Unity_Subtract_float(float A, float B, out float Out)
        {
            Out = A - B;
        }

        void Unity_Multiply_float(float A, float B, out float Out)
        {
            Out = A * B;
        }

        void Unity_Branch_float(float Predicate, float True, float False, out float Out)
        {
            Out = Predicate ? True : False;
        }

        void Unity_Dither_float(float In, float4 ScreenPosition, out float Out)
        {
            float2 uv = ScreenPosition.xy * _ScreenParams.xy;
            float DITHER_THRESHOLDS[16] =
            {
                1.0 / 17.0,  9.0 / 17.0,  3.0 / 17.0, 11.0 / 17.0,
                13.0 / 17.0,  5.0 / 17.0, 15.0 / 17.0,  7.0 / 17.0,
                4.0 / 17.0, 12.0 / 17.0,  2.0 / 17.0, 10.0 / 17.0,
                16.0 / 17.0,  8.0 / 17.0, 14.0 / 17.0,  6.0 / 17.0
            };
            uint index = (uint(uv.x) % 4) * 4 + uint(uv.y) % 4;
            Out = In - DITHER_THRESHOLDS[index];
        }

        struct Bindings_Dither_9b41370f0c5105847b2fec9036934da7
        {
            float3 WorldSpacePosition;
            float4 ScreenPosition;
        };

        void SG_Dither_9b41370f0c5105847b2fec9036934da7(float Boolean_75fab38b788b41d18702451bf97d8974, Bindings_Dither_9b41370f0c5105847b2fec9036934da7 IN, out float DistanceAlpha_2, out float DitherPattern_1)
        {
            float _Property_0a971a5d36084b7daed36160bb4ab6eb_Out_0 = Boolean_75fab38b788b41d18702451bf97d8974;
            float _Distance_37c8da6819a54f9b84aad0aa27264adc_Out_2;
            Unity_Distance_float3(_WorldSpaceCameraPos, IN.WorldSpacePosition, _Distance_37c8da6819a54f9b84aad0aa27264adc_Out_2);
            float _Subtract_8cbd49a21ebd4e5ea3c8c363207c9228_Out_2;
            Unity_Subtract_float(_Distance_37c8da6819a54f9b84aad0aa27264adc_Out_2, 0.25, _Subtract_8cbd49a21ebd4e5ea3c8c363207c9228_Out_2);
            float _Multiply_2e703422f1eb441592311108b388c71d_Out_2;
            Unity_Multiply_float(_Subtract_8cbd49a21ebd4e5ea3c8c363207c9228_Out_2, 3, _Multiply_2e703422f1eb441592311108b388c71d_Out_2);
            float _Branch_875139a8f00d481bbd14b6aed3251ccb_Out_3;
            Unity_Branch_float(_Property_0a971a5d36084b7daed36160bb4ab6eb_Out_0, _Multiply_2e703422f1eb441592311108b388c71d_Out_2, 1, _Branch_875139a8f00d481bbd14b6aed3251ccb_Out_3);
            float _Dither_82f502cb016d47018f8c6a7dade171fe_Out_2;
            Unity_Dither_float(1, float4(IN.ScreenPosition.xy / IN.ScreenPosition.w, 0, 0), _Dither_82f502cb016d47018f8c6a7dade171fe_Out_2);
            float _Branch_6efb7f3c563344f9aee2d13c5a52c8e3_Out_3;
            Unity_Branch_float(_Property_0a971a5d36084b7daed36160bb4ab6eb_Out_0, _Dither_82f502cb016d47018f8c6a7dade171fe_Out_2, 0, _Branch_6efb7f3c563344f9aee2d13c5a52c8e3_Out_3);
            DistanceAlpha_2 = _Branch_875139a8f00d481bbd14b6aed3251ccb_Out_3;
            DitherPattern_1 = _Branch_6efb7f3c563344f9aee2d13c5a52c8e3_Out_3;
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
            float Alpha;
            float AlphaClipThreshold;
        };

        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
            float _Property_26183fe36ab7437d89f981a749ccb7d1_Out_0 = Boolean_746f00cdd21a463a8f7db0eed7f65639;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
            Bindings_Dither_9b41370f0c5105847b2fec9036934da7 _Dither_f41700f34e7844b6ba89f6bb33dde6c6;
            _Dither_f41700f34e7844b6ba89f6bb33dde6c6.WorldSpacePosition = IN.WorldSpacePosition;
            _Dither_f41700f34e7844b6ba89f6bb33dde6c6.ScreenPosition = IN.ScreenPosition;
            float _Dither_f41700f34e7844b6ba89f6bb33dde6c6_DistanceAlpha_2;
            float _Dither_f41700f34e7844b6ba89f6bb33dde6c6_DitherPattern_1;
            SG_Dither_9b41370f0c5105847b2fec9036934da7(_Property_26183fe36ab7437d89f981a749ccb7d1_Out_0, _Dither_f41700f34e7844b6ba89f6bb33dde6c6, _Dither_f41700f34e7844b6ba89f6bb33dde6c6_DistanceAlpha_2, _Dither_f41700f34e7844b6ba89f6bb33dde6c6_DitherPattern_1);
            #endif
            surface.Alpha = _Dither_f41700f34e7844b6ba89f6bb33dde6c6_DistanceAlpha_2;
            surface.AlphaClipThreshold = _Dither_f41700f34e7844b6ba89f6bb33dde6c6_DitherPattern_1;
            return surface;
        }

            // --------------------------------------------------
            // Build Graph Inputs

            VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
        {
            VertexDescriptionInputs output;
            ZERO_INITIALIZE(VertexDescriptionInputs, output);

        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
        output.ObjectSpaceNormal =           input.normalOS;
        #endif

        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
        output.ObjectSpaceTangent =          input.tangentOS.xyz;
        #endif

        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
        output.ObjectSpacePosition =         input.positionOS;
        #endif


            return output;
        }
            SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);





        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
        output.WorldSpacePosition =          input.positionWS;
        #endif

        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
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
            #pragma shader_feature_local _ CEL_SPECULAR_ON
        #pragma shader_feature_local _ CEL_ACCENT_ON
        #pragma multi_compile _ _SHADOWS_SOFT
        #pragma multi_compile _ _ADDITIONAL_LIGHTS

        #if defined(CEL_SPECULAR_ON) && defined(CEL_ACCENT_ON)
            #define KEYWORD_PERMUTATION_0
        #elif defined(CEL_SPECULAR_ON)
            #define KEYWORD_PERMUTATION_1
        #elif defined(CEL_ACCENT_ON)
            #define KEYWORD_PERMUTATION_2
        #else
            #define KEYWORD_PERMUTATION_3
        #endif


            // Defines
        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
        #define _AlphaClip 1
        #endif

        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
        #define ATTRIBUTES_NEED_NORMAL
        #endif

        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
        #define ATTRIBUTES_NEED_TANGENT
        #endif

        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
        #define ATTRIBUTES_NEED_TEXCOORD0
        #endif

        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
        #define VARYINGS_NEED_POSITION_WS
        #endif

        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
        #define VARYINGS_NEED_NORMAL_WS
        #endif

        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
        #define VARYINGS_NEED_TANGENT_WS
        #endif

        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
        #define VARYINGS_NEED_TEXCOORD0
        #endif

        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
        #define VARYINGS_NEED_VIEWDIRECTION_WS
        #endif

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
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
            float3 positionOS : POSITION;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
            float3 normalOS : NORMAL;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
            float4 tangentOS : TANGENT;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
            float4 uv0 : TEXCOORD0;
            #endif
            #if UNITY_ANY_INSTANCING_ENABLED
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
            uint instanceID : INSTANCEID_SEMANTIC;
            #endif
            #endif
        };
        struct Varyings
        {
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
            float4 positionCS : SV_POSITION;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
            float3 positionWS;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
            float3 normalWS;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
            float4 tangentWS;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
            float4 texCoord0;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
            float3 viewDirectionWS;
            #endif
            #if UNITY_ANY_INSTANCING_ENABLED
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
            uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
            uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
            uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
            FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
            #endif
        };
        struct SurfaceDescriptionInputs
        {
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
            float3 WorldSpaceNormal;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
            float3 WorldSpaceTangent;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
            float3 WorldSpaceBiTangent;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
            float3 WorldSpaceViewDirection;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
            float3 WorldSpacePosition;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
            float3 AbsoluteWorldSpacePosition;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
            float4 ScreenPosition;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
            float4 uv0;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
            float3 TimeParameters;
            #endif
        };
        struct VertexDescriptionInputs
        {
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
            float3 ObjectSpaceNormal;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
            float3 ObjectSpaceTangent;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
            float3 ObjectSpacePosition;
            #endif
        };
        struct PackedVaryings
        {
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
            float4 positionCS : SV_POSITION;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
            float3 interp0 : TEXCOORD0;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
            float3 interp1 : TEXCOORD1;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
            float4 interp2 : TEXCOORD2;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
            float4 interp3 : TEXCOORD3;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
            float3 interp4 : TEXCOORD4;
            #endif
            #if UNITY_ANY_INSTANCING_ENABLED
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
            uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
            uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
            uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
            FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
            #endif
        };

            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
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
        float4 Texture2D_9d2dc686cea4411eb4cd42e0ad89db69_TexelSize;
        float Vector1_eba81f4ec9494ffc89d831e650dc2eee;
        float Boolean_746f00cdd21a463a8f7db0eed7f65639;
        float NORMALMAP_ON;
        float4 Texture2D_1709bfc569f34021a1205546b253ac2b_TexelSize;
        float Vector1_83717f647fb949a0b96857d7ef1efef2;
        float FLOWMAP_ON;
        float4 Texture2D_9e7ce96f70ec4e4fad0aca3a397849ab_TexelSize;
        float Vector1_693d76d6835a455fba9b2a18be0e2bec;
        float Vector1_da40d7438c114e189f67f755323f75ea;
        float EMISSIONMAP_ON;
        float4 Texture2D_ea1c329133fc4f01a0ae24d3d7ecfc16_TexelSize;
        float Vector1_a62ab1db54d34856836ce0b6d78fe08f;
        float Vector1_c092b1ab83a845279db3044fddc481d7;
        float Boolean_1;
        float Vector1_1;
        float Vector1_38eeab6d9e3146b49202d29537ff230c;
        float Boolean_101eac5857d84f2b8c80fed3fbaa437b;
        float4 Texture2D_408732e7caa34af796ddb77be6aaabea_TexelSize;
        CBUFFER_END

        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(Texture2D_9d2dc686cea4411eb4cd42e0ad89db69);
        SAMPLER(samplerTexture2D_9d2dc686cea4411eb4cd42e0ad89db69);
        TEXTURE2D(Texture2D_1709bfc569f34021a1205546b253ac2b);
        SAMPLER(samplerTexture2D_1709bfc569f34021a1205546b253ac2b);
        TEXTURE2D(Texture2D_9e7ce96f70ec4e4fad0aca3a397849ab);
        SAMPLER(samplerTexture2D_9e7ce96f70ec4e4fad0aca3a397849ab);
        TEXTURE2D(Texture2D_ea1c329133fc4f01a0ae24d3d7ecfc16);
        SAMPLER(samplerTexture2D_ea1c329133fc4f01a0ae24d3d7ecfc16);
        TEXTURE2D(Texture2D_408732e7caa34af796ddb77be6aaabea);
        SAMPLER(samplerTexture2D_408732e7caa34af796ddb77be6aaabea);

            // Graph Functions
            
        void Unity_Multiply_float(float A, float B, out float Out)
        {
            Out = A * B;
        }

        void Unity_Branch_float(float Predicate, float True, float False, out float Out)
        {
            Out = Predicate ? True : False;
        }

        // 14232a670a847cf5d9a67762c8e9c727
        #include "Assets/Rendering/Shaders/Functions/ShaderGraphCel.hlsl"

        struct Bindings_FlowMapAgain_fc942ebb7f073b34c95020e7e9594c1d
        {
        };

        void SG_FlowMapAgain_fc942ebb7f073b34c95020e7e9594c1d(UnityTexture2D Texture2D_438fde66a71545d0881e899d13c392d9, UnityTexture2D Texture2D_e25242356fdd43d0a4c0acd4f27c1b6c, float4 Vector4_45eaa08e83d6442a8c1c17913a80c3f9, float Vector1_bb84133cb040419d98935441d68aa202, float Vector1_6d347278dbed49b99401dc21e9b096d8, float Boolean_51360f658f9b44cbbecfc398bd80b57d, Bindings_FlowMapAgain_fc942ebb7f073b34c95020e7e9594c1d IN, out half4 FlowColor_1)
        {
            UnityTexture2D _Property_31480d3b368d49c4a2881a238a4e2be5_Out_0 = Texture2D_438fde66a71545d0881e899d13c392d9;
            UnityTexture2D _Property_838db75420d843a79a1be455bd2fe17b_Out_0 = Texture2D_e25242356fdd43d0a4c0acd4f27c1b6c;
            float4 _Property_a26b31063dee40f598d76b497a18b741_Out_0 = Vector4_45eaa08e83d6442a8c1c17913a80c3f9;
            float _Property_e9c222f421174ba98ff2b9fe23bfacd7_Out_0 = Vector1_bb84133cb040419d98935441d68aa202;
            float _Property_cc3f06cd8cd840ac93f57a82efb62351_Out_0 = Vector1_6d347278dbed49b99401dc21e9b096d8;
            float _Property_ba22938ef7d24a7e99ac4600f95ce8c4_Out_0 = Boolean_51360f658f9b44cbbecfc398bd80b57d;
            half4 _FlowMapCustomFunction_8a07381e71124a159105c737e309885c_FlowColor_4;
            FlowMap_half(_Property_31480d3b368d49c4a2881a238a4e2be5_Out_0, _Property_838db75420d843a79a1be455bd2fe17b_Out_0, (_Property_a26b31063dee40f598d76b497a18b741_Out_0.xy), _Property_e9c222f421174ba98ff2b9fe23bfacd7_Out_0, _Property_cc3f06cd8cd840ac93f57a82efb62351_Out_0, _Property_ba22938ef7d24a7e99ac4600f95ce8c4_Out_0, _FlowMapCustomFunction_8a07381e71124a159105c737e309885c_FlowColor_4);
            FlowColor_1 = _FlowMapCustomFunction_8a07381e71124a159105c737e309885c_FlowColor_4;
        }

        void Unity_NormalStrength_float(float3 In, float Strength, out float3 Out)
        {
            Out = float3(In.rg * Strength, lerp(1, In.b, saturate(Strength)));
        }

        void Unity_Branch_float3(float Predicate, float3 True, float3 False, out float3 Out)
        {
            Out = Predicate ? True : False;
        }

        void Unity_Add_float3(float3 A, float3 B, out float3 Out)
        {
            Out = A + B;
        }

        void Unity_Normalize_float3(float3 In, out float3 Out)
        {
            Out = normalize(In);
        }

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

        void Unity_Multiply_float(float3 A, float3 B, out float3 Out)
        {
            Out = A * B;
        }

        void Unity_Saturation_float(float3 In, float Saturation, out float3 Out)
        {
            float luma = dot(In, float3(0.2126729, 0.7151522, 0.0721750));
            Out =  luma.xxx + Saturation.xxx * (In - luma.xxx);
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

        void Unity_Distance_float3(float3 A, float3 B, out float Out)
        {
            Out = distance(A, B);
        }

        void Unity_Subtract_float(float A, float B, out float Out)
        {
            Out = A - B;
        }

        void Unity_Dither_float(float In, float4 ScreenPosition, out float Out)
        {
            float2 uv = ScreenPosition.xy * _ScreenParams.xy;
            float DITHER_THRESHOLDS[16] =
            {
                1.0 / 17.0,  9.0 / 17.0,  3.0 / 17.0, 11.0 / 17.0,
                13.0 / 17.0,  5.0 / 17.0, 15.0 / 17.0,  7.0 / 17.0,
                4.0 / 17.0, 12.0 / 17.0,  2.0 / 17.0, 10.0 / 17.0,
                16.0 / 17.0,  8.0 / 17.0, 14.0 / 17.0,  6.0 / 17.0
            };
            uint index = (uint(uv.x) % 4) * 4 + uint(uv.y) % 4;
            Out = In - DITHER_THRESHOLDS[index];
        }

        struct Bindings_Dither_9b41370f0c5105847b2fec9036934da7
        {
            float3 WorldSpacePosition;
            float4 ScreenPosition;
        };

        void SG_Dither_9b41370f0c5105847b2fec9036934da7(float Boolean_75fab38b788b41d18702451bf97d8974, Bindings_Dither_9b41370f0c5105847b2fec9036934da7 IN, out float DistanceAlpha_2, out float DitherPattern_1)
        {
            float _Property_0a971a5d36084b7daed36160bb4ab6eb_Out_0 = Boolean_75fab38b788b41d18702451bf97d8974;
            float _Distance_37c8da6819a54f9b84aad0aa27264adc_Out_2;
            Unity_Distance_float3(_WorldSpaceCameraPos, IN.WorldSpacePosition, _Distance_37c8da6819a54f9b84aad0aa27264adc_Out_2);
            float _Subtract_8cbd49a21ebd4e5ea3c8c363207c9228_Out_2;
            Unity_Subtract_float(_Distance_37c8da6819a54f9b84aad0aa27264adc_Out_2, 0.25, _Subtract_8cbd49a21ebd4e5ea3c8c363207c9228_Out_2);
            float _Multiply_2e703422f1eb441592311108b388c71d_Out_2;
            Unity_Multiply_float(_Subtract_8cbd49a21ebd4e5ea3c8c363207c9228_Out_2, 3, _Multiply_2e703422f1eb441592311108b388c71d_Out_2);
            float _Branch_875139a8f00d481bbd14b6aed3251ccb_Out_3;
            Unity_Branch_float(_Property_0a971a5d36084b7daed36160bb4ab6eb_Out_0, _Multiply_2e703422f1eb441592311108b388c71d_Out_2, 1, _Branch_875139a8f00d481bbd14b6aed3251ccb_Out_3);
            float _Dither_82f502cb016d47018f8c6a7dade171fe_Out_2;
            Unity_Dither_float(1, float4(IN.ScreenPosition.xy / IN.ScreenPosition.w, 0, 0), _Dither_82f502cb016d47018f8c6a7dade171fe_Out_2);
            float _Branch_6efb7f3c563344f9aee2d13c5a52c8e3_Out_3;
            Unity_Branch_float(_Property_0a971a5d36084b7daed36160bb4ab6eb_Out_0, _Dither_82f502cb016d47018f8c6a7dade171fe_Out_2, 0, _Branch_6efb7f3c563344f9aee2d13c5a52c8e3_Out_3);
            DistanceAlpha_2 = _Branch_875139a8f00d481bbd14b6aed3251ccb_Out_3;
            DitherPattern_1 = _Branch_6efb7f3c563344f9aee2d13c5a52c8e3_Out_3;
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
            float AlphaClipThreshold;
        };

        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
            UnityTexture2D _Property_2d650f0660bb4d38bf5f09cedaee98cd_Out_0 = UnityBuildTexture2DStructNoScale(Texture2D_9d2dc686cea4411eb4cd42e0ad89db69);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
            UnityTexture2D _Property_2d76057f23694095af98da3226a7faee_Out_0 = UnityBuildTexture2DStructNoScale(Texture2D_9e7ce96f70ec4e4fad0aca3a397849ab);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
            float4 _UV_c5bca71bad4b49f992c8d5ba39de058c_Out_0 = IN.uv0;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
            float _Property_5912c8450dd24f14a3bb3903d52671cc_Out_0 = Vector1_693d76d6835a455fba9b2a18be0e2bec;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
            float _Multiply_ba2126df2a8c4839bb16eee6ebaf69b2_Out_2;
            Unity_Multiply_float(_Property_5912c8450dd24f14a3bb3903d52671cc_Out_0, IN.TimeParameters.x, _Multiply_ba2126df2a8c4839bb16eee6ebaf69b2_Out_2);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
            float _Property_374ac4d4f39346049a1730f927453584_Out_0 = FLOWMAP_ON;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
            float _Property_f96f66447b4b4d23848d760aea4a6c3b_Out_0 = Vector1_da40d7438c114e189f67f755323f75ea;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
            float _Branch_80de9b6c75ee445f9fa8a1cf23426a99_Out_3;
            Unity_Branch_float(_Property_374ac4d4f39346049a1730f927453584_Out_0, _Property_f96f66447b4b4d23848d760aea4a6c3b_Out_0, 0, _Branch_80de9b6c75ee445f9fa8a1cf23426a99_Out_3);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
            Bindings_FlowMapAgain_fc942ebb7f073b34c95020e7e9594c1d _FlowMapAgain_bcea8020458540d1acec52bd36104aae;
            half4 _FlowMapAgain_bcea8020458540d1acec52bd36104aae_FlowColor_1;
            SG_FlowMapAgain_fc942ebb7f073b34c95020e7e9594c1d(_Property_2d650f0660bb4d38bf5f09cedaee98cd_Out_0, _Property_2d76057f23694095af98da3226a7faee_Out_0, _UV_c5bca71bad4b49f992c8d5ba39de058c_Out_0, _Multiply_ba2126df2a8c4839bb16eee6ebaf69b2_Out_2, _Branch_80de9b6c75ee445f9fa8a1cf23426a99_Out_3, 0, _FlowMapAgain_bcea8020458540d1acec52bd36104aae, _FlowMapAgain_bcea8020458540d1acec52bd36104aae_FlowColor_1);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
            float _Property_f3158fd43f49461c90185566a8219d83_Out_0 = NORMALMAP_ON;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
            UnityTexture2D _Property_7ed04db0e0ac4201bd9dcc36f02604b0_Out_0 = UnityBuildTexture2DStructNoScale(Texture2D_1709bfc569f34021a1205546b253ac2b);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
            Bindings_FlowMapAgain_fc942ebb7f073b34c95020e7e9594c1d _FlowMapAgain_11ae490cfcbe4738b3bd288b3402b87d;
            half4 _FlowMapAgain_11ae490cfcbe4738b3bd288b3402b87d_FlowColor_1;
            SG_FlowMapAgain_fc942ebb7f073b34c95020e7e9594c1d(_Property_7ed04db0e0ac4201bd9dcc36f02604b0_Out_0, _Property_2d76057f23694095af98da3226a7faee_Out_0, _UV_c5bca71bad4b49f992c8d5ba39de058c_Out_0, _Multiply_ba2126df2a8c4839bb16eee6ebaf69b2_Out_2, _Branch_80de9b6c75ee445f9fa8a1cf23426a99_Out_3, 1, _FlowMapAgain_11ae490cfcbe4738b3bd288b3402b87d, _FlowMapAgain_11ae490cfcbe4738b3bd288b3402b87d_FlowColor_1);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
            float _Property_43a5ee13e5da4cfe84bb62af6fdb1d7f_Out_0 = Vector1_83717f647fb949a0b96857d7ef1efef2;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
            float3 _NormalStrength_959d184e9ceb49f1982ad107ee63c1cd_Out_2;
            Unity_NormalStrength_float((_FlowMapAgain_11ae490cfcbe4738b3bd288b3402b87d_FlowColor_1.xyz), _Property_43a5ee13e5da4cfe84bb62af6fdb1d7f_Out_0, _NormalStrength_959d184e9ceb49f1982ad107ee63c1cd_Out_2);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
            float3 _Branch_8cc0465e516546a593af84bd338adc2a_Out_3;
            Unity_Branch_float3(_Property_f3158fd43f49461c90185566a8219d83_Out_0, _NormalStrength_959d184e9ceb49f1982ad107ee63c1cd_Out_2, float3(0, 0, 1), _Branch_8cc0465e516546a593af84bd338adc2a_Out_3);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
            float _Property_e55436ab1ab0470cb5255f52938e4168_Out_0 = Vector1_1;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
            float _Property_0534d185de7d41c1ab3d55da040fffe6_Out_0 = Vector1_38eeab6d9e3146b49202d29537ff230c;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
            UnityTexture2D _Property_dcf9b48cb3b849b2b2b8253150bb5121_Out_0 = UnityBuildTexture2DStructNoScale(Texture2D_408732e7caa34af796ddb77be6aaabea);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
            Bindings_CelShading_802007f66a3e13d42802a3875ed35370 _CelShading_f5c658ba84a743b29c36c275531286d9;
            _CelShading_f5c658ba84a743b29c36c275531286d9.WorldSpaceNormal = IN.WorldSpaceNormal;
            _CelShading_f5c658ba84a743b29c36c275531286d9.WorldSpaceTangent = IN.WorldSpaceTangent;
            _CelShading_f5c658ba84a743b29c36c275531286d9.WorldSpaceBiTangent = IN.WorldSpaceBiTangent;
            _CelShading_f5c658ba84a743b29c36c275531286d9.WorldSpaceViewDirection = IN.WorldSpaceViewDirection;
            _CelShading_f5c658ba84a743b29c36c275531286d9.AbsoluteWorldSpacePosition = IN.AbsoluteWorldSpacePosition;
            _CelShading_f5c658ba84a743b29c36c275531286d9.uv0 = IN.uv0;
            half4 _CelShading_f5c658ba84a743b29c36c275531286d9_Color_1;
            SG_CelShading_802007f66a3e13d42802a3875ed35370(_FlowMapAgain_bcea8020458540d1acec52bd36104aae_FlowColor_1, _Branch_8cc0465e516546a593af84bd338adc2a_Out_3, _Property_e55436ab1ab0470cb5255f52938e4168_Out_0, _Property_0534d185de7d41c1ab3d55da040fffe6_Out_0, _Property_dcf9b48cb3b849b2b2b8253150bb5121_Out_0, _CelShading_f5c658ba84a743b29c36c275531286d9, _CelShading_f5c658ba84a743b29c36c275531286d9_Color_1);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
            UnityTexture2D _Property_d7f0ad396ff04624879f4577d54429bb_Out_0 = UnityBuildTexture2DStructNoScale(Texture2D_ea1c329133fc4f01a0ae24d3d7ecfc16);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
            float4 _SampleTexture2D_8eb61b4203f44272a54e7d80a2514d18_RGBA_0 = SAMPLE_TEXTURE2D(_Property_d7f0ad396ff04624879f4577d54429bb_Out_0.tex, _Property_d7f0ad396ff04624879f4577d54429bb_Out_0.samplerstate, IN.uv0.xy);
            float _SampleTexture2D_8eb61b4203f44272a54e7d80a2514d18_R_4 = _SampleTexture2D_8eb61b4203f44272a54e7d80a2514d18_RGBA_0.r;
            float _SampleTexture2D_8eb61b4203f44272a54e7d80a2514d18_G_5 = _SampleTexture2D_8eb61b4203f44272a54e7d80a2514d18_RGBA_0.g;
            float _SampleTexture2D_8eb61b4203f44272a54e7d80a2514d18_B_6 = _SampleTexture2D_8eb61b4203f44272a54e7d80a2514d18_RGBA_0.b;
            float _SampleTexture2D_8eb61b4203f44272a54e7d80a2514d18_A_7 = _SampleTexture2D_8eb61b4203f44272a54e7d80a2514d18_RGBA_0.a;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
            float _Property_e66795ed672b4c54aebf2255e6f49694_Out_0 = Vector1_a62ab1db54d34856836ce0b6d78fe08f;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
            float _Property_c9ab50e169504622906236aa02a3e4c1_Out_0 = Vector1_c092b1ab83a845279db3044fddc481d7;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
            float _Property_e8b08fad777c4daf8e21fc2e116ec4fc_Out_0 = EMISSIONMAP_ON;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
            Bindings_Emission_1c7d3173c45db194bad26a66d6783e8c _Emission_0c52d2cccf6f4e5bb3d0f691e7e87a81;
            float3 _Emission_0c52d2cccf6f4e5bb3d0f691e7e87a81_FinalColor_1;
            SG_Emission_1c7d3173c45db194bad26a66d6783e8c((_CelShading_f5c658ba84a743b29c36c275531286d9_Color_1.xyz), _SampleTexture2D_8eb61b4203f44272a54e7d80a2514d18_RGBA_0, _Property_e66795ed672b4c54aebf2255e6f49694_Out_0, _Property_c9ab50e169504622906236aa02a3e4c1_Out_0, _Property_e8b08fad777c4daf8e21fc2e116ec4fc_Out_0, _Emission_0c52d2cccf6f4e5bb3d0f691e7e87a81, _Emission_0c52d2cccf6f4e5bb3d0f691e7e87a81_FinalColor_1);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
            float _Property_26183fe36ab7437d89f981a749ccb7d1_Out_0 = Boolean_746f00cdd21a463a8f7db0eed7f65639;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
            Bindings_Dither_9b41370f0c5105847b2fec9036934da7 _Dither_f41700f34e7844b6ba89f6bb33dde6c6;
            _Dither_f41700f34e7844b6ba89f6bb33dde6c6.WorldSpacePosition = IN.WorldSpacePosition;
            _Dither_f41700f34e7844b6ba89f6bb33dde6c6.ScreenPosition = IN.ScreenPosition;
            float _Dither_f41700f34e7844b6ba89f6bb33dde6c6_DistanceAlpha_2;
            float _Dither_f41700f34e7844b6ba89f6bb33dde6c6_DitherPattern_1;
            SG_Dither_9b41370f0c5105847b2fec9036934da7(_Property_26183fe36ab7437d89f981a749ccb7d1_Out_0, _Dither_f41700f34e7844b6ba89f6bb33dde6c6, _Dither_f41700f34e7844b6ba89f6bb33dde6c6_DistanceAlpha_2, _Dither_f41700f34e7844b6ba89f6bb33dde6c6_DitherPattern_1);
            #endif
            surface.BaseColor = _Emission_0c52d2cccf6f4e5bb3d0f691e7e87a81_FinalColor_1;
            surface.Alpha = _Dither_f41700f34e7844b6ba89f6bb33dde6c6_DistanceAlpha_2;
            surface.AlphaClipThreshold = _Dither_f41700f34e7844b6ba89f6bb33dde6c6_DitherPattern_1;
            return surface;
        }

            // --------------------------------------------------
            // Build Graph Inputs

            VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
        {
            VertexDescriptionInputs output;
            ZERO_INITIALIZE(VertexDescriptionInputs, output);

        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
        output.ObjectSpaceNormal =           input.normalOS;
        #endif

        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
        output.ObjectSpaceTangent =          input.tangentOS.xyz;
        #endif

        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
        output.ObjectSpacePosition =         input.positionOS;
        #endif


            return output;
        }
            SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);

        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
        // must use interpolated tangent, bitangent and normal before they are normalized in the pixel shader.
        #endif

        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
        float3 unnormalizedNormalWS = input.normalWS;
        #endif

        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
        const float renormFactor = 1.0 / length(unnormalizedNormalWS);
        #endif


        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
        // use bitangent on the fly like in hdrp
        #endif

        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
        // IMPORTANT! If we ever support Flip on double sided materials ensure bitangent and tangent are NOT flipped.
        #endif

        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
        float crossSign = (input.tangentWS.w > 0.0 ? 1.0 : -1.0) * GetOddNegativeScale();
        #endif

        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
        float3 bitang = crossSign * cross(input.normalWS.xyz, input.tangentWS.xyz);
        #endif


        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
        output.WorldSpaceNormal =            renormFactor*input.normalWS.xyz;		// we want a unit length Normal Vector node in shader graph
        #endif


        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
        // to preserve mikktspace compliance we use same scale renormFactor as was used on the normal.
        #endif

        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
        // This is explained in section 2.2 in "surface gradient based bump mapping framework"
        #endif

        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
        output.WorldSpaceTangent =           renormFactor*input.tangentWS.xyz;
        #endif

        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
        output.WorldSpaceBiTangent =         renormFactor*bitang;
        #endif


        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
        output.WorldSpaceViewDirection =     input.viewDirectionWS; //TODO: by default normalized in HD, but not in universal
        #endif

        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
        output.WorldSpacePosition =          input.positionWS;
        #endif

        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
        output.AbsoluteWorldSpacePosition =  GetAbsolutePositionWS(input.positionWS);
        #endif

        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
        output.ScreenPosition =              ComputeScreenPos(TransformWorldToHClip(input.positionWS), _ProjectionParams.x);
        #endif

        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
        output.uv0 =                         input.texCoord0;
        #endif

        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
        output.TimeParameters =              _TimeParameters.xyz; // This is mainly for LW as HD overwrite this value
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
            #pragma shader_feature_local _ CEL_SPECULAR_ON
        #pragma shader_feature_local _ CEL_ACCENT_ON

        #if defined(CEL_SPECULAR_ON) && defined(CEL_ACCENT_ON)
            #define KEYWORD_PERMUTATION_0
        #elif defined(CEL_SPECULAR_ON)
            #define KEYWORD_PERMUTATION_1
        #elif defined(CEL_ACCENT_ON)
            #define KEYWORD_PERMUTATION_2
        #else
            #define KEYWORD_PERMUTATION_3
        #endif


            // Defines
        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
        #define _AlphaClip 1
        #endif

        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
        #define ATTRIBUTES_NEED_NORMAL
        #endif

        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
        #define ATTRIBUTES_NEED_TANGENT
        #endif

        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
        #define VARYINGS_NEED_POSITION_WS
        #endif

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
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
            float3 positionOS : POSITION;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
            float3 normalOS : NORMAL;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
            float4 tangentOS : TANGENT;
            #endif
            #if UNITY_ANY_INSTANCING_ENABLED
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
            uint instanceID : INSTANCEID_SEMANTIC;
            #endif
            #endif
        };
        struct Varyings
        {
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
            float4 positionCS : SV_POSITION;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
            float3 positionWS;
            #endif
            #if UNITY_ANY_INSTANCING_ENABLED
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
            uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
            uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
            uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
            FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
            #endif
        };
        struct SurfaceDescriptionInputs
        {
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
            float3 WorldSpacePosition;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
            float4 ScreenPosition;
            #endif
        };
        struct VertexDescriptionInputs
        {
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
            float3 ObjectSpaceNormal;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
            float3 ObjectSpaceTangent;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
            float3 ObjectSpacePosition;
            #endif
        };
        struct PackedVaryings
        {
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
            float4 positionCS : SV_POSITION;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
            float3 interp0 : TEXCOORD0;
            #endif
            #if UNITY_ANY_INSTANCING_ENABLED
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
            uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
            uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
            uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
            FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
            #endif
        };

            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
        PackedVaryings PackVaryings (Varyings input)
        {
            PackedVaryings output;
            output.positionCS = input.positionCS;
            output.interp0.xyz =  input.positionWS;
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
        float4 Texture2D_9d2dc686cea4411eb4cd42e0ad89db69_TexelSize;
        float Vector1_eba81f4ec9494ffc89d831e650dc2eee;
        float Boolean_746f00cdd21a463a8f7db0eed7f65639;
        float NORMALMAP_ON;
        float4 Texture2D_1709bfc569f34021a1205546b253ac2b_TexelSize;
        float Vector1_83717f647fb949a0b96857d7ef1efef2;
        float FLOWMAP_ON;
        float4 Texture2D_9e7ce96f70ec4e4fad0aca3a397849ab_TexelSize;
        float Vector1_693d76d6835a455fba9b2a18be0e2bec;
        float Vector1_da40d7438c114e189f67f755323f75ea;
        float EMISSIONMAP_ON;
        float4 Texture2D_ea1c329133fc4f01a0ae24d3d7ecfc16_TexelSize;
        float Vector1_a62ab1db54d34856836ce0b6d78fe08f;
        float Vector1_c092b1ab83a845279db3044fddc481d7;
        float Boolean_1;
        float Vector1_1;
        float Vector1_38eeab6d9e3146b49202d29537ff230c;
        float Boolean_101eac5857d84f2b8c80fed3fbaa437b;
        float4 Texture2D_408732e7caa34af796ddb77be6aaabea_TexelSize;
        CBUFFER_END

        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(Texture2D_9d2dc686cea4411eb4cd42e0ad89db69);
        SAMPLER(samplerTexture2D_9d2dc686cea4411eb4cd42e0ad89db69);
        TEXTURE2D(Texture2D_1709bfc569f34021a1205546b253ac2b);
        SAMPLER(samplerTexture2D_1709bfc569f34021a1205546b253ac2b);
        TEXTURE2D(Texture2D_9e7ce96f70ec4e4fad0aca3a397849ab);
        SAMPLER(samplerTexture2D_9e7ce96f70ec4e4fad0aca3a397849ab);
        TEXTURE2D(Texture2D_ea1c329133fc4f01a0ae24d3d7ecfc16);
        SAMPLER(samplerTexture2D_ea1c329133fc4f01a0ae24d3d7ecfc16);
        TEXTURE2D(Texture2D_408732e7caa34af796ddb77be6aaabea);
        SAMPLER(samplerTexture2D_408732e7caa34af796ddb77be6aaabea);

            // Graph Functions
            
        void Unity_Distance_float3(float3 A, float3 B, out float Out)
        {
            Out = distance(A, B);
        }

        void Unity_Subtract_float(float A, float B, out float Out)
        {
            Out = A - B;
        }

        void Unity_Multiply_float(float A, float B, out float Out)
        {
            Out = A * B;
        }

        void Unity_Branch_float(float Predicate, float True, float False, out float Out)
        {
            Out = Predicate ? True : False;
        }

        void Unity_Dither_float(float In, float4 ScreenPosition, out float Out)
        {
            float2 uv = ScreenPosition.xy * _ScreenParams.xy;
            float DITHER_THRESHOLDS[16] =
            {
                1.0 / 17.0,  9.0 / 17.0,  3.0 / 17.0, 11.0 / 17.0,
                13.0 / 17.0,  5.0 / 17.0, 15.0 / 17.0,  7.0 / 17.0,
                4.0 / 17.0, 12.0 / 17.0,  2.0 / 17.0, 10.0 / 17.0,
                16.0 / 17.0,  8.0 / 17.0, 14.0 / 17.0,  6.0 / 17.0
            };
            uint index = (uint(uv.x) % 4) * 4 + uint(uv.y) % 4;
            Out = In - DITHER_THRESHOLDS[index];
        }

        struct Bindings_Dither_9b41370f0c5105847b2fec9036934da7
        {
            float3 WorldSpacePosition;
            float4 ScreenPosition;
        };

        void SG_Dither_9b41370f0c5105847b2fec9036934da7(float Boolean_75fab38b788b41d18702451bf97d8974, Bindings_Dither_9b41370f0c5105847b2fec9036934da7 IN, out float DistanceAlpha_2, out float DitherPattern_1)
        {
            float _Property_0a971a5d36084b7daed36160bb4ab6eb_Out_0 = Boolean_75fab38b788b41d18702451bf97d8974;
            float _Distance_37c8da6819a54f9b84aad0aa27264adc_Out_2;
            Unity_Distance_float3(_WorldSpaceCameraPos, IN.WorldSpacePosition, _Distance_37c8da6819a54f9b84aad0aa27264adc_Out_2);
            float _Subtract_8cbd49a21ebd4e5ea3c8c363207c9228_Out_2;
            Unity_Subtract_float(_Distance_37c8da6819a54f9b84aad0aa27264adc_Out_2, 0.25, _Subtract_8cbd49a21ebd4e5ea3c8c363207c9228_Out_2);
            float _Multiply_2e703422f1eb441592311108b388c71d_Out_2;
            Unity_Multiply_float(_Subtract_8cbd49a21ebd4e5ea3c8c363207c9228_Out_2, 3, _Multiply_2e703422f1eb441592311108b388c71d_Out_2);
            float _Branch_875139a8f00d481bbd14b6aed3251ccb_Out_3;
            Unity_Branch_float(_Property_0a971a5d36084b7daed36160bb4ab6eb_Out_0, _Multiply_2e703422f1eb441592311108b388c71d_Out_2, 1, _Branch_875139a8f00d481bbd14b6aed3251ccb_Out_3);
            float _Dither_82f502cb016d47018f8c6a7dade171fe_Out_2;
            Unity_Dither_float(1, float4(IN.ScreenPosition.xy / IN.ScreenPosition.w, 0, 0), _Dither_82f502cb016d47018f8c6a7dade171fe_Out_2);
            float _Branch_6efb7f3c563344f9aee2d13c5a52c8e3_Out_3;
            Unity_Branch_float(_Property_0a971a5d36084b7daed36160bb4ab6eb_Out_0, _Dither_82f502cb016d47018f8c6a7dade171fe_Out_2, 0, _Branch_6efb7f3c563344f9aee2d13c5a52c8e3_Out_3);
            DistanceAlpha_2 = _Branch_875139a8f00d481bbd14b6aed3251ccb_Out_3;
            DitherPattern_1 = _Branch_6efb7f3c563344f9aee2d13c5a52c8e3_Out_3;
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
            float Alpha;
            float AlphaClipThreshold;
        };

        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
            float _Property_26183fe36ab7437d89f981a749ccb7d1_Out_0 = Boolean_746f00cdd21a463a8f7db0eed7f65639;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
            Bindings_Dither_9b41370f0c5105847b2fec9036934da7 _Dither_f41700f34e7844b6ba89f6bb33dde6c6;
            _Dither_f41700f34e7844b6ba89f6bb33dde6c6.WorldSpacePosition = IN.WorldSpacePosition;
            _Dither_f41700f34e7844b6ba89f6bb33dde6c6.ScreenPosition = IN.ScreenPosition;
            float _Dither_f41700f34e7844b6ba89f6bb33dde6c6_DistanceAlpha_2;
            float _Dither_f41700f34e7844b6ba89f6bb33dde6c6_DitherPattern_1;
            SG_Dither_9b41370f0c5105847b2fec9036934da7(_Property_26183fe36ab7437d89f981a749ccb7d1_Out_0, _Dither_f41700f34e7844b6ba89f6bb33dde6c6, _Dither_f41700f34e7844b6ba89f6bb33dde6c6_DistanceAlpha_2, _Dither_f41700f34e7844b6ba89f6bb33dde6c6_DitherPattern_1);
            #endif
            surface.Alpha = _Dither_f41700f34e7844b6ba89f6bb33dde6c6_DistanceAlpha_2;
            surface.AlphaClipThreshold = _Dither_f41700f34e7844b6ba89f6bb33dde6c6_DitherPattern_1;
            return surface;
        }

            // --------------------------------------------------
            // Build Graph Inputs

            VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
        {
            VertexDescriptionInputs output;
            ZERO_INITIALIZE(VertexDescriptionInputs, output);

        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
        output.ObjectSpaceNormal =           input.normalOS;
        #endif

        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
        output.ObjectSpaceTangent =          input.tangentOS.xyz;
        #endif

        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
        output.ObjectSpacePosition =         input.positionOS;
        #endif


            return output;
        }
            SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);





        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
        output.WorldSpacePosition =          input.positionWS;
        #endif

        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
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
            #pragma shader_feature_local _ CEL_SPECULAR_ON
        #pragma shader_feature_local _ CEL_ACCENT_ON

        #if defined(CEL_SPECULAR_ON) && defined(CEL_ACCENT_ON)
            #define KEYWORD_PERMUTATION_0
        #elif defined(CEL_SPECULAR_ON)
            #define KEYWORD_PERMUTATION_1
        #elif defined(CEL_ACCENT_ON)
            #define KEYWORD_PERMUTATION_2
        #else
            #define KEYWORD_PERMUTATION_3
        #endif


            // Defines
        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
        #define _AlphaClip 1
        #endif

        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
        #define ATTRIBUTES_NEED_NORMAL
        #endif

        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
        #define ATTRIBUTES_NEED_TANGENT
        #endif

        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
        #define VARYINGS_NEED_POSITION_WS
        #endif

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
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
            float3 positionOS : POSITION;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
            float3 normalOS : NORMAL;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
            float4 tangentOS : TANGENT;
            #endif
            #if UNITY_ANY_INSTANCING_ENABLED
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
            uint instanceID : INSTANCEID_SEMANTIC;
            #endif
            #endif
        };
        struct Varyings
        {
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
            float4 positionCS : SV_POSITION;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
            float3 positionWS;
            #endif
            #if UNITY_ANY_INSTANCING_ENABLED
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
            uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
            uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
            uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
            FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
            #endif
        };
        struct SurfaceDescriptionInputs
        {
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
            float3 WorldSpacePosition;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
            float4 ScreenPosition;
            #endif
        };
        struct VertexDescriptionInputs
        {
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
            float3 ObjectSpaceNormal;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
            float3 ObjectSpaceTangent;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
            float3 ObjectSpacePosition;
            #endif
        };
        struct PackedVaryings
        {
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
            float4 positionCS : SV_POSITION;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
            float3 interp0 : TEXCOORD0;
            #endif
            #if UNITY_ANY_INSTANCING_ENABLED
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
            uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
            uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
            uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
            FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
            #endif
        };

            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
        PackedVaryings PackVaryings (Varyings input)
        {
            PackedVaryings output;
            output.positionCS = input.positionCS;
            output.interp0.xyz =  input.positionWS;
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
        float4 Texture2D_9d2dc686cea4411eb4cd42e0ad89db69_TexelSize;
        float Vector1_eba81f4ec9494ffc89d831e650dc2eee;
        float Boolean_746f00cdd21a463a8f7db0eed7f65639;
        float NORMALMAP_ON;
        float4 Texture2D_1709bfc569f34021a1205546b253ac2b_TexelSize;
        float Vector1_83717f647fb949a0b96857d7ef1efef2;
        float FLOWMAP_ON;
        float4 Texture2D_9e7ce96f70ec4e4fad0aca3a397849ab_TexelSize;
        float Vector1_693d76d6835a455fba9b2a18be0e2bec;
        float Vector1_da40d7438c114e189f67f755323f75ea;
        float EMISSIONMAP_ON;
        float4 Texture2D_ea1c329133fc4f01a0ae24d3d7ecfc16_TexelSize;
        float Vector1_a62ab1db54d34856836ce0b6d78fe08f;
        float Vector1_c092b1ab83a845279db3044fddc481d7;
        float Boolean_1;
        float Vector1_1;
        float Vector1_38eeab6d9e3146b49202d29537ff230c;
        float Boolean_101eac5857d84f2b8c80fed3fbaa437b;
        float4 Texture2D_408732e7caa34af796ddb77be6aaabea_TexelSize;
        CBUFFER_END

        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(Texture2D_9d2dc686cea4411eb4cd42e0ad89db69);
        SAMPLER(samplerTexture2D_9d2dc686cea4411eb4cd42e0ad89db69);
        TEXTURE2D(Texture2D_1709bfc569f34021a1205546b253ac2b);
        SAMPLER(samplerTexture2D_1709bfc569f34021a1205546b253ac2b);
        TEXTURE2D(Texture2D_9e7ce96f70ec4e4fad0aca3a397849ab);
        SAMPLER(samplerTexture2D_9e7ce96f70ec4e4fad0aca3a397849ab);
        TEXTURE2D(Texture2D_ea1c329133fc4f01a0ae24d3d7ecfc16);
        SAMPLER(samplerTexture2D_ea1c329133fc4f01a0ae24d3d7ecfc16);
        TEXTURE2D(Texture2D_408732e7caa34af796ddb77be6aaabea);
        SAMPLER(samplerTexture2D_408732e7caa34af796ddb77be6aaabea);

            // Graph Functions
            
        void Unity_Distance_float3(float3 A, float3 B, out float Out)
        {
            Out = distance(A, B);
        }

        void Unity_Subtract_float(float A, float B, out float Out)
        {
            Out = A - B;
        }

        void Unity_Multiply_float(float A, float B, out float Out)
        {
            Out = A * B;
        }

        void Unity_Branch_float(float Predicate, float True, float False, out float Out)
        {
            Out = Predicate ? True : False;
        }

        void Unity_Dither_float(float In, float4 ScreenPosition, out float Out)
        {
            float2 uv = ScreenPosition.xy * _ScreenParams.xy;
            float DITHER_THRESHOLDS[16] =
            {
                1.0 / 17.0,  9.0 / 17.0,  3.0 / 17.0, 11.0 / 17.0,
                13.0 / 17.0,  5.0 / 17.0, 15.0 / 17.0,  7.0 / 17.0,
                4.0 / 17.0, 12.0 / 17.0,  2.0 / 17.0, 10.0 / 17.0,
                16.0 / 17.0,  8.0 / 17.0, 14.0 / 17.0,  6.0 / 17.0
            };
            uint index = (uint(uv.x) % 4) * 4 + uint(uv.y) % 4;
            Out = In - DITHER_THRESHOLDS[index];
        }

        struct Bindings_Dither_9b41370f0c5105847b2fec9036934da7
        {
            float3 WorldSpacePosition;
            float4 ScreenPosition;
        };

        void SG_Dither_9b41370f0c5105847b2fec9036934da7(float Boolean_75fab38b788b41d18702451bf97d8974, Bindings_Dither_9b41370f0c5105847b2fec9036934da7 IN, out float DistanceAlpha_2, out float DitherPattern_1)
        {
            float _Property_0a971a5d36084b7daed36160bb4ab6eb_Out_0 = Boolean_75fab38b788b41d18702451bf97d8974;
            float _Distance_37c8da6819a54f9b84aad0aa27264adc_Out_2;
            Unity_Distance_float3(_WorldSpaceCameraPos, IN.WorldSpacePosition, _Distance_37c8da6819a54f9b84aad0aa27264adc_Out_2);
            float _Subtract_8cbd49a21ebd4e5ea3c8c363207c9228_Out_2;
            Unity_Subtract_float(_Distance_37c8da6819a54f9b84aad0aa27264adc_Out_2, 0.25, _Subtract_8cbd49a21ebd4e5ea3c8c363207c9228_Out_2);
            float _Multiply_2e703422f1eb441592311108b388c71d_Out_2;
            Unity_Multiply_float(_Subtract_8cbd49a21ebd4e5ea3c8c363207c9228_Out_2, 3, _Multiply_2e703422f1eb441592311108b388c71d_Out_2);
            float _Branch_875139a8f00d481bbd14b6aed3251ccb_Out_3;
            Unity_Branch_float(_Property_0a971a5d36084b7daed36160bb4ab6eb_Out_0, _Multiply_2e703422f1eb441592311108b388c71d_Out_2, 1, _Branch_875139a8f00d481bbd14b6aed3251ccb_Out_3);
            float _Dither_82f502cb016d47018f8c6a7dade171fe_Out_2;
            Unity_Dither_float(1, float4(IN.ScreenPosition.xy / IN.ScreenPosition.w, 0, 0), _Dither_82f502cb016d47018f8c6a7dade171fe_Out_2);
            float _Branch_6efb7f3c563344f9aee2d13c5a52c8e3_Out_3;
            Unity_Branch_float(_Property_0a971a5d36084b7daed36160bb4ab6eb_Out_0, _Dither_82f502cb016d47018f8c6a7dade171fe_Out_2, 0, _Branch_6efb7f3c563344f9aee2d13c5a52c8e3_Out_3);
            DistanceAlpha_2 = _Branch_875139a8f00d481bbd14b6aed3251ccb_Out_3;
            DitherPattern_1 = _Branch_6efb7f3c563344f9aee2d13c5a52c8e3_Out_3;
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
            float Alpha;
            float AlphaClipThreshold;
        };

        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
            float _Property_26183fe36ab7437d89f981a749ccb7d1_Out_0 = Boolean_746f00cdd21a463a8f7db0eed7f65639;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
            Bindings_Dither_9b41370f0c5105847b2fec9036934da7 _Dither_f41700f34e7844b6ba89f6bb33dde6c6;
            _Dither_f41700f34e7844b6ba89f6bb33dde6c6.WorldSpacePosition = IN.WorldSpacePosition;
            _Dither_f41700f34e7844b6ba89f6bb33dde6c6.ScreenPosition = IN.ScreenPosition;
            float _Dither_f41700f34e7844b6ba89f6bb33dde6c6_DistanceAlpha_2;
            float _Dither_f41700f34e7844b6ba89f6bb33dde6c6_DitherPattern_1;
            SG_Dither_9b41370f0c5105847b2fec9036934da7(_Property_26183fe36ab7437d89f981a749ccb7d1_Out_0, _Dither_f41700f34e7844b6ba89f6bb33dde6c6, _Dither_f41700f34e7844b6ba89f6bb33dde6c6_DistanceAlpha_2, _Dither_f41700f34e7844b6ba89f6bb33dde6c6_DitherPattern_1);
            #endif
            surface.Alpha = _Dither_f41700f34e7844b6ba89f6bb33dde6c6_DistanceAlpha_2;
            surface.AlphaClipThreshold = _Dither_f41700f34e7844b6ba89f6bb33dde6c6_DitherPattern_1;
            return surface;
        }

            // --------------------------------------------------
            // Build Graph Inputs

            VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
        {
            VertexDescriptionInputs output;
            ZERO_INITIALIZE(VertexDescriptionInputs, output);

        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
        output.ObjectSpaceNormal =           input.normalOS;
        #endif

        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
        output.ObjectSpaceTangent =          input.tangentOS.xyz;
        #endif

        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
        output.ObjectSpacePosition =         input.positionOS;
        #endif


            return output;
        }
            SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);





        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
        output.WorldSpacePosition =          input.positionWS;
        #endif

        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
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