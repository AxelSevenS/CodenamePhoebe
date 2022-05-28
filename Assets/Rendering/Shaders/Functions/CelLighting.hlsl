#ifndef CEL_LIGHTING_INCLUDED
#define CEL_LIGHTING_INCLUDED

uniform half3 _AmbientLight;

SamplerState gradient_point_clamp_sampler;

// float BlinnPhongReflection( float3 normal, float3 lightDir, float3 viewDir, float smoothness ) {
//     float3 V = normalize( -viewDir );
//     float3 R = reflect( normalize( lightDir ), normalize( normal ) );
//     return pow( saturate( dot( V, R ) ), smoothness );
// }

float PhongReflection( float3 normal, float3 lightDir, float3 viewDir, float smoothness ) {
    float3 V = normalize( -viewDir );
    float3 R = reflect( normalize( lightDir ), normalize( normal ) );
    return pow( saturate( dot( V, R ) ), smoothness );
}

float GaussianReflection( float3 normal, float3 lightDir, float3 viewDir, float smoothness ) {
    float specularAngle = acos( dot( normalize(lightDir + viewDir ), normalize( normal ) ) );
    float specularExponent = specularAngle / smoothness;
    return exp(-specularExponent * specularExponent);
}

float3 PBRSpecular( float3 normal, float3 lightDir, float3 viewDir, float smoothness ) {
    float3 halfVec = SafeNormalize(lightDir + viewDir);
    half NdotH = half(saturate(dot(normal, halfVec)));
    half modifier = pow(NdotH, smoothness);
    half3 specularReflection = /* specularColor */float3(1,1,1) * modifier;
    return /* lightColor */float3(1,1,1) * specularReflection;
}

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

float2 GetLuminance (float3 normalWS, float3 viewDirectionWS, float3 lightDirectionWS, float lightAttenuation) {
    float NdotL = saturate(dot(normalWS, lightDirectionWS));
    return NdotL * lightAttenuation;
}
float2 GetSpecular (float3 normalWS, float3 viewDirectionWS, float3 lightDirectionWS, float lightAttenuation, float smoothness) {
    #ifdef CEL_SPECULAR_ON
        float specular = PhongReflection(normalWS, lightDirectionWS, viewDirectionWS, smoothness*100) * lightAttenuation;
    #else 
        float specular = 0;
    #endif

    return specular;
}

// float3 ApplyGradients(float luminance, UnityTexture2D shadeGradient, UnityTexture2D accentGradient) {

//     float gradedShade = SampleGradientTex(shadeGradient, luminance).r;

//     // #ifdef SPECULARGRADIENT_ON
//     //     float gradedSpecular = specular;
//     //     // float gradedSpecular = SampleGradientTex(specularGradient, specular ).r;
//     // #else 
//     //     float gradedSpecular = 0;
//     // #endif

//     #ifdef CEL_ACCENT_ON
//         float gradedAccent = SampleGradientTex(accentGradient, luminance).r;
//     #else 
//         float gradedAccent = 0;
//     #endif

//     return float2(gradedShade, /* gradedSpecular,  */gradedAccent);
// }

float3 CelColor (float3 lightColor, float attenuation, float specular, float accent, float3 accentColor) {

    float3 radiance = lightColor * _AmbientLight;
    float3 shade = radiance*0.2;

    float3 finalColor = lerp(shade, radiance, attenuation);

    finalColor = lerp(finalColor, finalColor * accentColor, accent);
    
    finalColor += radiance * specular;
    
    return finalColor;
}

float3 CelShade(
            float3 normal, 
            float3 viewDir, 
            float3 lightColor, 
            float3 lightDir, 
            float lightShadowAtten,
            UnityTexture2D shadeGradient, 
            UnityTexture2D specularGradient, 
            float specularIntensity, 
            float smoothness, 
            UnityTexture2D accentGradient, 
            float3 accentColor
        ) {
    
    float luminance = GetLuminance(normal, viewDir, lightDir, lightShadowAtten);

    float shade = SampleGradientTex(shadeGradient, luminance).r;

    #ifdef CEL_SPECULAR_ON
        float specular = GetSpecular(normal, viewDir, lightDir, lightShadowAtten, smoothness);
        specular = SampleGradientTex(specularGradient, saturate(specular)).r * specularIntensity;
    #else
        float specular = 0;
    #endif

    #ifdef CEL_ACCENT_ON
        float accent = SampleGradientTex(accentGradient, luminance).r;
    #else
        float accent = 0;
    #endif

    return CelColor(lightColor, shade, specular, accent, accentColor);
}


void LightingCel_float(float3 baseColor, float3 positionWS, float3 viewDirectionWS, float3 normalWS, UnityTexture2D shadeGradient, UnityTexture2D specularGradient,float specularIntensity, float smoothness, UnityTexture2D accentGradient, float3 accentColor, out float3 finalColor) {

    smoothness = max(smoothness, 0.001);

    #ifdef SHADERGRAPH_PREVIEW

        finalColor = CelShade(
            normalWS, 
            viewDirectionWS, 
            float3(1, 1, 1), 
            float3(0.5, 0.5, 0.5), 
            1,
            shadeGradient,
            specularGradient,
            specularIntensity, 
            smoothness, 
            accentGradient, 
            accentColor
        );

    #else
    
        half cascadeIndex = ComputeCascadeIndex(positionWS);
        float4 shadowCoord = mul(_MainLightWorldToShadow[cascadeIndex], float4(positionWS, 1.0));
        Light light = GetMainLight(shadowCoord);
    
        ShadowSamplingData shadowSamplingData = GetMainLightShadowSamplingData(); 
        float shadowStrength = GetMainLightShadowStrength();
        light.shadowAttenuation = SampleShadowmap(shadowCoord, TEXTURE2D_ARGS(_MainLightShadowmapTexture, sampler_MainLightShadowmapTexture), shadowSamplingData, shadowStrength, false);

        float3 shadeColor = CelShade(
            normalWS, 
            viewDirectionWS, 
            light.color * light.distanceAttenuation, 
            light.direction, 
            light.shadowAttenuation,
            shadeGradient,
            specularGradient,
            specularIntensity, 
            smoothness, 
            accentGradient, 
            accentColor
        );

        #ifdef _ADDITIONAL_LIGHTS
            uint lightsCount = GetAdditionalLightsCount();
            for (uint lightIndex = 0u; lightIndex < lightsCount; ++lightIndex) {
                light = GetAdditionalLight(lightIndex, positionWS);

                shadeColor += CelShade(
                    normalWS, 
                    viewDirectionWS, 
                    light.color * light.distanceAttenuation, 
                    light.direction, 
                    light.shadowAttenuation,
                    shadeGradient,
                    specularGradient,
                    specularIntensity, 
                    smoothness, 
                    accentGradient, 
                    accentColor
                );
            }
        #endif
        
        finalColor = shadeColor * baseColor;

    #endif
}

#endif