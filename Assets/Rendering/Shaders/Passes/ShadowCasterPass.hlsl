#ifndef CEL_SHADOWCASTER_PASS_INCLUDED
#define CEL_SHADOWCASTER_PASS_INCLUDED

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Shadows.hlsl"
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

    // Apply Vertex Displacement / Normal Modifications here --

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

#endif