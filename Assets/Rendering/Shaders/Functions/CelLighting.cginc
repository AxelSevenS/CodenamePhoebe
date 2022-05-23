#ifndef CEL_LIGHTING_INCLUDED
#define CEL_LIGHTING_INCLUDED

uniform half3 _AmbientLight;

SamplerState gradient_point_clamp_sampler;

float4 SampleGradient(Gradient Gradient, float Time){
    
    float3 color = Gradient.colors[0].rgb;
    [unroll]
    for (int c = 1; c < 8; c++) {
        float colorPos = saturate((Time - Gradient.colors[c-1].w) / (Gradient.colors[c].w - Gradient.colors[c-1].w)) * step(c, Gradient.colorsLength-1);
        color = lerp(color, Gradient.colors[c].rgb, lerp(colorPos, step(0.01, colorPos), Gradient.type));
    }
    #ifndef UNITY_COLORSPACE_GAMMA
        color = SRGBToLinear(color);
    #endif

    float alpha = Gradient.alphas[0].x;
    [unroll]
    for (int a = 1; a < 8; a++) {
        float alphaPos = saturate((Time - Gradient.alphas[a-1].y) / (Gradient.alphas[a].y - Gradient.alphas[a-1].y)) * step(a, Gradient.alphasLength-1);
        alpha = lerp(alpha, Gradient.alphas[a].x, lerp(alphaPos, step(0.01, alphaPos), Gradient.type));
    }
    return float4(color, alpha);
}

float4 SampleGradientTex(UnityTexture2D Gradient, float Time) {
    return Gradient.Sample( gradient_point_clamp_sampler, float2(Time, 0.5) );
}

float2 GetLuminanceSpecular (float3 normalWS, float3 viewDirectionWS, float3 lightDirectionWS, float lightAttenuation, float smoothness = 0) {

    float NdotL = saturate(dot(normalWS, lightDirectionWS));
    float luminance = NdotL * lightAttenuation;

    #ifdef SPECULARGRADIENT_ON
        float specularAngle = acos(dot(normalize(lightDirectionWS - viewDirectionWS), normalize(normalWS)));
        float specularExponent = specularAngle / (1 - smoothness);
        float specular = exp(-specularExponent * specularExponent);
    #else 
        float specular = 0;
    #endif

    return float2(luminance, specular);
}

float3 CelShade (float2 luminanceSpecular, UnityTexture2D shadeGradient, UnityTexture2D specularGradient, UnityTexture2D accentGradient) {

    float attenuation = SampleGradientTex(shadeGradient, luminanceSpecular.x).r;

    #ifdef SPECULARGRADIENT_ON
        float specular = SampleGradientTex(specularGradient, attenuation * luminanceSpecular.y ).r;
    #else 
        float specular = 0;
    #endif

    #ifdef ACCENTGRADIENT_ON
        float accent = SampleGradientTex(accentGradient, luminanceSpecular.x).r;
    #else 
        float accent = 0;
    #endif

    return float3(attenuation, specular, accent);
}

float3 CelColor (float3 lightColor, float attenuation, float specular, float accent, float3 accentColor) {

    float3 radiance = lightColor * _AmbientLight;
    float3 shade = radiance*0.2;

    float3 finalColor = lerp(shade, radiance, attenuation);
    finalColor = lerp(finalColor, finalColor * accentColor, accent);
    
    finalColor += radiance * specular;
    
    return finalColor;
}

void LightingCel_float(float3 baseColor, float3 positionWS, float3 viewDirectionWS, float3 normalWS, UnityTexture2D shadeGradient, UnityTexture2D specularGradient, UnityTexture2D accentGradient, float3 accentColor, out float3 finalColor, out float2 mainLuminanceSpecular, out float3 mainShadeSpecularAccent) {

    #ifdef SHADERGRAPH_PREVIEW

        mainLuminanceSpecular = GetLuminanceSpecular(normalWS, viewDirectionWS, float3(0.5, 0.5, 0.5), 0, 0);
        mainShadeSpecularAccent = CelShade(mainLuminanceSpecular, shadeGradient, specularGradient, accentGradient);
        finalColor = CelColor(float3(1, 1, 1), mainShadeSpecularAccent.x, mainShadeSpecularAccent.y, mainShadeSpecularAccent.z, accentColor);

    #else
    
        // half4 shadowCoord = TransformWorldToShadowCoord(positionWS);
        half cascadeIndex = ComputeCascadeIndex(positionWS);
        float4 shadowCoord = mul(_MainLightWorldToShadow[cascadeIndex], float4(positionWS, 1.0));
        Light light = GetMainLight(shadowCoord);
    
        ShadowSamplingData shadowSamplingData = GetMainLightShadowSamplingData();
        float shadowStrength = GetMainLightShadowStrength();
        light.shadowAttenuation = SampleShadowmap(shadowCoord, TEXTURE2D_ARGS(_MainLightShadowmapTexture, sampler_MainLightShadowmapTexture), shadowSamplingData, shadowStrength, false);

        mainLuminanceSpecular = GetLuminanceSpecular(normalWS, viewDirectionWS, light.direction, light.shadowAttenuation, 0);
        mainShadeSpecularAccent = CelShade(mainLuminanceSpecular, shadeGradient, specularGradient, accentGradient);

        float2 luminanceSpecular = mainLuminanceSpecular;
        float3 shadeSpecularAccent = mainShadeSpecularAccent;
        float3 shadeColor = CelColor(light.color * light.distanceAttenuation, mainShadeSpecularAccent.x, mainShadeSpecularAccent.y, mainShadeSpecularAccent.z, accentColor);

        #ifdef _ADDITIONAL_LIGHTS
            uint lightsCount = GetAdditionalLightsCount();
            for (uint lightIndex = 0u; lightIndex < lightsCount; ++lightIndex) {
                light = GetAdditionalLight(lightIndex, positionWS);

                luminanceSpecular = GetLuminanceSpecular(normalWS, viewDirectionWS, light.direction, light.shadowAttenuation, 0);
                shadeSpecularAccent = CelShade(luminanceSpecular, shadeGradient, specularGradient, accentGradient);
                shadeColor += CelColor(light.color * light.distanceAttenuation, shadeSpecularAccent.x, shadeSpecularAccent.y, shadeSpecularAccent.z, accentColor) * shadeSpecularAccent.x;
            }
        #endif
        
        finalColor = shadeColor * baseColor;

    #endif
}

// float3 CelLight
//     (float3 normalWS, float3 viewDirectionWS, float3 lightDirectionWS, float3 lightColor, float lightAttenuation, 
//     UnityTexture2D shadeGradient, UnityTexture2D accentGradient, float3 accentColor) {

//     float NdotL = saturate(dot(normalWS, lightDirectionWS));
//     float lightAmount = NdotL * lightAttenuation;

//     float finalAtten = SampleGradient(shadeGradient, lightAmount).r;
//     float sssRamp = SampleGradient(accentGradient, lightAmount).r;

//     float3 radiance = lightColor * _AmbientLight;
//     float3 shade = _AmbientLight*0.05;

//     float3 shadedColor = lerp(shade, radiance, finalAtten);
//     float3 finalColor = lerp(shadedColor, accentColor * radiance, sssRamp);



//     // Highlights
//     // float3 floatDir = normalize(normalize(viewDirectionWS) + lightDirectionWS);
//     // float NdotH = saturate(dot(normalWS, floatDir));

//     // float highlightSize = 0.1;
//     // float highlightBlend = 0.15;
//     // float highlight = smoothstep(highlightSize, highlightSize + highlightBlend, NdotH * NdotH);

//     // Rimlight
//     // float viewToLight = saturate(dot(lightDirectionWS, -viewDirectionWS));
//     // float viewToSurface = saturate(dot(normalWS, -viewDirectionWS));
//     // float fresnel = (1 - dot(viewDirectionWS, normalWS));
    
//     // float rimLight = fresnel * viewToLight * (1-viewToSurface);
//     // rimLight = smoothstep(shadowSize, shadowSize + shadowBlend, rimLight);

//     return finalColor;
// }

#endif