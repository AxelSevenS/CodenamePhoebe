#ifndef CEL_LIGHTING_INCLUDED
#define CEL_LIGHTING_INCLUDED
        
#pragma multi_compile _ _MAIN_LIGHT_SHADOWS
#pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
#pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
#pragma multi_compile_fragment _ _ADDITIONAL_LIGHT_SHADOWS
#pragma multi_compile_fragment _ _SHADOWS_SOFT
#pragma multi_compile_fragment _ _SCREEN_SPACE_OCCLUSION
#pragma multi_compile _ LIGHTMAP_SHADOW_MIXING
#pragma multi_compile _ SHADOWS_SHADOWMASK

#include "LightingUtilities.hlsl"
#include "Utility.hlsl"

uniform half3 _AmbientLight;
SamplerState gradient_point_clamp_sampler;

const half3 defaultLightColor = half3(1,1,1);
const half3 defaultLightDir = half3(0.5,0.5,0.5);


struct LightingInput{

    float4 clipPosition;
    float3 worldPosition;
    half3 worldNormal;
    half3 worldViewDirection;
    float4 screenPosition;
    // half4 shadowMask;
    float4 shadowCoord;
};

LightingInput GetLightingInput(half3 ObjectPosition, half3 ObjectNormal) {

    LightingInput lightingInput;
    lightingInput.clipPosition = TransformObjectToHClip(ObjectPosition);
    lightingInput.worldPosition = TransformObjectToWorld(ObjectPosition.xyz);
    lightingInput.worldNormal = normalize(TransformObjectToWorldNormal(ObjectNormal.xyz));
    lightingInput.worldViewDirection = normalize( _WorldSpaceCameraPos.xyz - lightingInput.worldPosition );
    #ifdef SHADERGRAPH_PREVIEW
        lightingInput.screenPosition = float4(0,0,0,0);
        lightingInput.shadowCoord = float4(0,0,0,0);
    #else
        lightingInput.screenPosition = ComputeScreenPos( lightingInput.clipPosition );
        #if SHADOWS_SCREEN
            lightingInput.shadowCoord = lightingInput.screenPosition;
        #else 
            lightingInput.shadowCoord = TransformWorldToShadowCoord(lightingInput.worldPosition);
        #endif
    #endif
    // #if defined(SHADOWS_SHADOWMASK) && defined(LIGHTMAP_ON)
    //     lightingInput.shadowMask = SAMPLE_TEXTURE2D( unity_ShadowMask, samplerunity_ShadowMask, lightMapUV );
    // #elif !defined (LIGHTMAP_ON)
    //     lightingInput.shadowMask = unity_ProbesOcclusion;
    // #else
    //     lightingInput.shadowMask = half4(1,1,1,1);
    // #endif
    return lightingInput;
}


half GetLuminance (LightingInput input, half3 lightDirectionWS, half shadowAttenuation) {
    return saturate(dot(input.worldNormal, lightDirectionWS)) * shadowAttenuation;
    // return NdotL;
}

half GetShade (half luminance) {
    // Color is dark until 15% luminance then it transitions to light until 55% luminance where it plateaus
    // the minimum value is 0.25 and the maximum is 1.0
    half shade = smoothstep(0.15, 0.55, luminance);
    return remap(shade, 0, 1, 0.25, 1);
}

half GetSpecular (LightingInput input, half3 lightDirectionWS, half smoothness) {
    if (smoothness == 0)
        return 0;
    return PhongReflection(input.worldNormal, input.worldViewDirection, lightDirectionWS, smoothness*100);
}


half3 CelColor (half3 lightColor, float attenuation, float specular, float accent, half3 accentColor) {

    half3 radiance = lightColor * _AmbientLight;
    half3 shade = radiance*0.2;

    half3 finalColor = lerp(shade, radiance, attenuation);

    finalColor = lerp(finalColor, finalColor * accentColor, accent);
    
    finalColor += radiance * specular;
    
    return finalColor;
}
half3 CelColor (half3 lightColor, float attenuation) {
    return CelColor(lightColor, attenuation, 0, 0, half3(0,0,0));
}


half3 CelShade( LightingInput input, half3 lightColor, half3 lightDir, half lightShadowAtten, half lightDistanceAtten, half specularIntensity, half smoothness, half3 accentColor ) {
    lightDir = normalize(lightDir);
    
    half luminance = GetLuminance(input, lightDir, lightShadowAtten);
    half shade = GetShade(luminance);

    half specular = GetSpecular(input, lightDir, smoothness) * shade;
    specular = smoothstep(0.15, 1.0, specular) * specularIntensity;

    // accent is how near luminance is to 0.2, it transitions from 1 when the difference is 0 to 0 when the difference is 0.65, then it plateaus
    // sqrt is to make the transition less linear

    // ||      ---------- 1 ----------                                                     ||
    // || 0.5                          ----------                                          ||
    // ||                                         ---------- 0 --------------------------- ||
    // ||  0  |  0.1  |  0.2  |  0.3  | 0.4  |  0.5  |  0.6  |  0.7  |  0.8  |  0.9  |  1  ||

    half accent = ( 1 - sqrt(abs(0.2 - luminance) ) ) * 0.7;

    return CelColor(lightColor * lightDistanceAtten, shade, specular, accent, accentColor);
}

half3 SimpleCelShade( LightingInput input, half3 lightColor, half3 lightDir, half lightShadowAtten, half lightDistanceAtten ) {
    half luminance = GetLuminance(input, normalize(lightDir), lightShadowAtten);
    half shade = GetShade(luminance);

    return CelColor(lightColor * lightDistanceAtten, shade);
}











half4 CelLighting( half4 baseColor, LightingInput input, half specularIntensity, half smoothness, half3 accentColor ) {
    input.worldNormal = normalize(input.worldNormal);
    input.worldViewDirection = normalize(input.worldViewDirection);

    #ifdef SHADERGRAPH_PREVIEW

        half3 celColor = CelShade(
            input,
            defaultLightColor,
            defaultLightDir,
            1,
            1,
            specularIntensity,
            smoothness,
            accentColor
        );

        return baseColor * half4(celColor, 1);

    #else
    
        Light light = GetMainLight(input.shadowCoord, input.worldPosition, 1);

        half3 celColor = CelShade(
            input,
            light.color, 
            light.direction, 
            light.shadowAttenuation,
            light.distanceAttenuation,
            specularIntensity, 
            smoothness, 
            accentColor
        );

        #ifdef _ADDITIONAL_LIGHTS
            uint lightsCount = GetAdditionalLightsCount();
            for (uint lightIndex = 0u; lightIndex < lightsCount; ++lightIndex) {
                // float4 shadowMask = SAMPLE_SHADOWMASK(input.uv2);
                light = GetAdditionalLight(lightIndex, input.worldPosition, 1);

                celColor += CelShade(
                    input,
                    light.color, 
                    light.direction, 
                    light.shadowAttenuation,
                    light.distanceAttenuation,
                    specularIntensity, 
                    smoothness, 
                    accentColor
                );
            }
        #endif
        
        return baseColor * half4(celColor, 1);

    #endif
}

half4 SimpleCelLighting( half4 baseColor, LightingInput input ) {
    input.worldViewDirection = normalize(input.worldViewDirection);

    #ifdef SHADERGRAPH_PREVIEW

        half3 celColor = SimpleCelShade(
            input,
            defaultLightColor, 
            defaultLightDir,
            1,
            1
        );
        return baseColor * half4(celColor, 1);

    #else
    
        // half cascadeIndex = ComputeCascadeIndex(input.worldPosition);

        // float4 shadowCoord = mul(_MainLightWorldToShadow[cascadeIndex], float4(input.worldPosition, 1.0));
        // Light light = GetMainLight(shadowCoord);
    
        // // Get Main Light shadow Attenuation
        // ShadowSamplingData shadowSamplingData = GetMainLightShadowSamplingData(); 
        // half shadowStrength = GetMainLightShadowStrength();
        // light.shadowAttenuation = SampleShadowmap(shadowCoord, TEXTURE2D_ARGS(_MainLightShadowmapTexture, sampler_MainLightShadowmapTexture), shadowSamplingData, shadowStrength, false);

        Light light = GetMainLight(input.shadowCoord, input.worldPosition, 1);
        
        half3 celColor = SimpleCelShade(
            input,
            light.color,
            light.direction,
            light.shadowAttenuation,
            light.distanceAttenuation
        );

        #ifdef _ADDITIONAL_LIGHTS
            uint lightsCount = GetAdditionalLightsCount();
            for (uint lightIndex = 0u; lightIndex < lightsCount; ++lightIndex) {
                // float4 shadowMask = SAMPLE_SHADOWMASK(input.uv2);
                light = GetAdditionalLight(lightIndex, input.worldPosition, 1);

                celColor += SimpleCelShade(
                    input,
                    light.color,
                    light.direction,
                    light.shadowAttenuation,
                    light.distanceAttenuation
                );
            }
        #endif
        
        return baseColor * half4(celColor, 1);

    #endif
}


#endif