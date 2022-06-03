#ifndef CEL_LIGHTING_INCLUDED
#define CEL_LIGHTING_INCLUDED

#include "LightingUtilities.hlsl"
#include "Utility.hlsl"

uniform half3 _AmbientLight;
SamplerState gradient_point_clamp_sampler;

const half3 defaultLightColor = half3(1,1,1);
const half3 defaultLightDir = half3(0.5,0.5,0.5);



half GetLuminance (half3 normalWS, half3 lightDirectionWS, half lightAttenuation) {
    half NdotL = saturate(dot(normalWS, lightDirectionWS));
    return NdotL * lightAttenuation;
}
half GetShade (half luminance) {
    // Color is dark until 15% luminance then it transitions to light until 55% luminance where it plateaus
    // the minimum value is 0.25 and the maximum is 1.0
    half shade = smoothstep(0.15, 0.55, luminance);
    return remap(shade, 0, 1, 0.25, 1);
}
half GetSpecular (half3 normalWS, half3 viewDirectionWS, half3 lightDirectionWS, half lightAttenuation, half smoothness) {
    #ifdef CEL_SPECULAR_ON
        if (smoothness == 0)
            return 0;
        return PhongReflection(normalWS, lightDirectionWS, viewDirectionWS, smoothness*100) * lightAttenuation;
    #else 
        return 0;
    #endif
}

half3 CelColor (half3 lightColor, float attenuation, float specular, float accent, half3 accentColor) {

    half3 radiance = lightColor * _AmbientLight;
    half3 shade = radiance*0.2;

    half3 finalColor = lerp(shade, radiance, attenuation);

    finalColor = lerp(finalColor, finalColor * accentColor, accent);
    
    finalColor += radiance * specular;
    
    return finalColor;
}

half3 CelShade( half3 normal, half3 viewDir, half3 lightColor, half3 lightDir, half lightShadowAtten, half specularIntensity, half smoothness, half3 accentColor ) {
    
    half luminance = GetLuminance(normal, lightDir, lightShadowAtten);
    half shade = GetShade(luminance);

    // half shade = shadeGradient.Sample(gradient_point_clamp_sampler, luminance).r;

    #ifdef CEL_SPECULAR_ON
        half specular = GetSpecular(normal, viewDir, lightDir, lightShadowAtten, smoothness);
        specular = smoothstep(0.15, 1.0, specular) * specularIntensity;

        // specular = SampleGradientTex(specularGradient, saturate(specular)).r * specularIntensity;
    #else
        half specular = 0;
    #endif

    #ifdef CEL_ACCENT_ON
        // accent is how near luminance is to 0.2, it transitions from 1 when the difference is 0 to 0 when the difference is 0.65, then it plateaus
        // sqrt is to make the transition less linear
        half accent = ( 1 - sqrt(abs(0.2 - luminance) ) ) * 0.7;
        
        // half accent = accentGradient.Sample(gradient_point_clamp_sampler, luminance).r;
    #else
        half accent = 0;
    #endif

    return CelColor(lightColor, shade, specular, accent, accentColor);
}

half3 SimpleCelShade( half3 normal, half3 lightColor, half3 lightDir, half lightShadowAtten ) {
    
    half luminance = GetLuminance(normal, lightDir, lightShadowAtten);
    half shade = GetShade(luminance);

    return CelColor(lightColor, shade, 0, 0, half3(0,0,0));
}











half4 CelLighting( half4 baseColor, float3 positionWS, half3 viewDirectionWS, half3 normalWS, half specularIntensity, half smoothness, half3 accentColor ) {

    #ifdef SHADERGRAPH_PREVIEW

        half3 celColor = CelShade(
            normalWS,
            viewDirectionWS,
            defaultLightColor,
            defaultLightDir,
            1,
            specularIntensity,
            smoothness,
            accentColor
        );

        return baseColor * half4(celColor, 1);

    #else
    
        half cascadeIndex = ComputeCascadeIndex(positionWS);
        float4 shadowCoord = mul(_MainLightWorldToShadow[cascadeIndex], float4(positionWS, 1.0));
        Light light = GetMainLight(shadowCoord);
    
        ShadowSamplingData shadowSamplingData = GetMainLightShadowSamplingData(); 
        half shadowStrength = GetMainLightShadowStrength();
        light.shadowAttenuation = SampleShadowmap(shadowCoord, TEXTURE2D_ARGS(_MainLightShadowmapTexture, sampler_MainLightShadowmapTexture), shadowSamplingData, shadowStrength, false);

        half3 celColor = CelShade(
            normalWS, 
            viewDirectionWS, 
            light.color * light.distanceAttenuation, 
            light.direction, 
            light.shadowAttenuation,
            specularIntensity, 
            smoothness, 
            accentColor
        );

        // #ifdef _ADDITIONAL_LIGHTS
            uint lightsCount = GetAdditionalLightsCount();
            for (uint lightIndex = 0u; lightIndex < lightsCount; ++lightIndex) {
                light = GetAdditionalLight(lightIndex, positionWS);

                celColor += CelShade(
                    normalWS, 
                    viewDirectionWS, 
                    light.color * light.distanceAttenuation, 
                    light.direction, 
                    light.shadowAttenuation,
                    specularIntensity, 
                    smoothness, 
                    accentColor
                );
            }
        // #endif
        
        return baseColor * half4(celColor, 1);

    #endif
}

half4 SimpleCelLighting( half4 baseColor, float3 positionWS, half3 normalWS ) {

    #ifdef SHADERGRAPH_PREVIEW

        half3 celColor = SimpleCelShade(
            normalWS,
            defaultLightColor, 
            defaultLightDir, 
            1
        );
        return baseColor * half4(celColor, 1);

    #else
    
        half cascadeIndex = ComputeCascadeIndex(positionWS);

        float4 shadowCoord = mul(_MainLightWorldToShadow[cascadeIndex], float4(positionWS, 1.0));
        Light light = GetMainLight(shadowCoord);
    
        // Get Main Light shadow Attenuation
        ShadowSamplingData shadowSamplingData = GetMainLightShadowSamplingData(); 
        half shadowStrength = GetMainLightShadowStrength();
        light.shadowAttenuation = SampleShadowmap(shadowCoord, TEXTURE2D_ARGS(_MainLightShadowmapTexture, sampler_MainLightShadowmapTexture), shadowSamplingData, shadowStrength, false);

        half3 celColor = SimpleCelShade(
            normalWS,
            light.color * light.distanceAttenuation,
            light.direction,
            light.shadowAttenuation
        );

        // #ifdef _ADDITIONAL_LIGHTS
            uint lightsCount = GetAdditionalLightsCount();
            for (uint lightIndex = 0u; lightIndex < lightsCount; ++lightIndex) {
                light = GetAdditionalLight(lightIndex, positionWS);

                celColor += SimpleCelShade(
                    normalWS,
                    light.color * light.distanceAttenuation,
                    light.direction,
                    light.shadowAttenuation
                );
            }
        // #endif
        
        return baseColor * half4(celColor, 1);

    #endif
}


#endif