#ifndef CEL_LIGHTING_INCLUDED
#define CEL_LIGHTING_INCLUDED

uniform half3 _AmbientLight;

SamplerState gradient_point_clamp_sampler;

float4 SampleGradient(UnityTexture2D Gradient, float Time) {
    return Gradient.Sample( gradient_point_clamp_sampler, float2(Time, 0.5) );
}

float3 CelLight
    (float3 normalWS, float3 viewDirectionWS, float3 lightDirectionWS, float3 lightColor, float lightAttenuation, 
    UnityTexture2D shadeGradient, UnityTexture2D sssGradient, float3 sssColor) {

    float NdotL = saturate(dot(normalWS, lightDirectionWS));
    float lightAmount = NdotL * lightAttenuation;

    float finalAtten = SampleGradient(shadeGradient, lightAmount).r;
    float sssRamp = SampleGradient(sssGradient, lightAmount).r;

    float3 radiance = lightColor * _AmbientLight;
    float3 shade = _AmbientLight*0.05;

    float3 shadedColor = lerp(shade, radiance, finalAtten);
    float3 finalColor = lerp(shadedColor, sssColor * radiance, sssRamp);



    // Highlights
    // float3 floatDir = normalize(normalize(viewDirectionWS) + lightDirectionWS);
    // float NdotH = saturate(dot(normalWS, floatDir));

    // float highlightSize = 0.1;
    // float highlightBlend = 0.15;
    // float highlight = smoothstep(highlightSize, highlightSize + highlightBlend, NdotH * NdotH);

    // Rimlight
    // float viewToLight = saturate(dot(lightDirectionWS, -viewDirectionWS));
    // float viewToSurface = saturate(dot(normalWS, -viewDirectionWS));
    // float fresnel = (1 - dot(viewDirectionWS, normalWS));
    
    // float rimLight = fresnel * viewToLight * (1-viewToSurface);
    // rimLight = smoothstep(shadowSize, shadowSize + shadowBlend, rimLight);

    return finalColor;
}

void LightingCel_float(float3 positionWS, float3 viewDirectionWS, float3 normalWS, UnityTexture2D shadeGradient, UnityTexture2D sssGradient, float3 sssColor, out float3 finalColor) {

    #ifdef SHADERGRAPH_PREVIEW

        finalColor = CelLight(normalWS, viewDirectionWS, float3(0.5, 0.5, 0.5), float3(1, 1, 1), 0, shadeGradient, sssGradient, sssColor);

    #else
    
        // half4 shadowCoord = TransformWorldToShadowCoord(positionWS);
        half cascadeIndex = ComputeCascadeIndex(positionWS);
        float4 shadowCoord = mul(_MainLightWorldToShadow[cascadeIndex], float4(positionWS, 1.0));
        Light light = GetMainLight(shadowCoord);
    
        ShadowSamplingData shadowSamplingData = GetMainLightShadowSamplingData();
        float shadowStrength = GetMainLightShadowStrength();
        light.shadowAttenuation = SampleShadowmap(shadowCoord, TEXTURE2D_ARGS(_MainLightShadowmapTexture, sampler_MainLightShadowmapTexture), shadowSamplingData, shadowStrength, false);

        finalColor = CelLight(normalWS, viewDirectionWS, light.direction, light.color * light.distanceAttenuation, light.shadowAttenuation, shadeGradient, sssGradient, sssColor);

        #ifdef _ADDITIONAL_LIGHTS
            uint lightsCount = GetAdditionalLightsCount();
            for (uint lightIndex = 0u; lightIndex < lightsCount; ++lightIndex) {
                light = GetAdditionalLight(lightIndex, positionWS);
                finalColor += CelLight(normalWS, viewDirectionWS, light.direction, light.color * light.distanceAttenuation, light.shadowAttenuation, shadeGradient, sssGradient, sssColor).xyz;
            }
        #endif

    #endif
}

#endif