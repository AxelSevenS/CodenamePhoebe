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



struct CelLightingInput {
    float4 clipPosition;
    float3 worldPosition;
    half3 worldNormal;
    half3 worldViewDirection;
    float4 screenPosition;
    float4 shadowCoord;
    half specularIntensity;
    half smoothness;
    half accentIntensity;
};

CelLightingInput GetCelLightingInput( LightingInput lightingInput, half specularIntensity = 0, half smoothness = 0, half accentIntensity = 0 ) {
    CelLightingInput celLightingInput;
    celLightingInput.clipPosition = lightingInput.clipPosition;
    celLightingInput.worldPosition = lightingInput.worldPosition;
    celLightingInput.worldNormal = lightingInput.worldNormal;
    celLightingInput.worldViewDirection = lightingInput.worldViewDirection;
    celLightingInput.screenPosition = lightingInput.screenPosition;
    celLightingInput.shadowCoord = lightingInput.shadowCoord;
    celLightingInput.specularIntensity = specularIntensity;
    celLightingInput.smoothness = smoothness;
    celLightingInput.accentIntensity = accentIntensity;
    return celLightingInput;
}

CelLightingInput GetCelLightingInput( half3 ObjectPosition, half3 ObjectNormal, half specularIntensity = 0, half smoothness = 0, half accentIntensity = 0 ) {
    return GetCelLightingInput( GetLightingInput(ObjectPosition, ObjectNormal), specularIntensity, smoothness, accentIntensity );
}




half GetLuminance (CelLightingInput input, half3 lightDirectionWS, half shadowAttenuation) {
    return saturate( dot(input.worldNormal, lightDirectionWS) ) * shadowAttenuation;
    // return NdotL;
}

half GetShade (half luminance) {

    // Color is dark until 15% luminance then it transitions to light until 55% luminance where it plateaus
    // the minimum value is 0.25 and the maximum is 1.0
    
    // ||                                   -------- 1 ----------------------------------- || This is a curve graph in case you couldn't tell...
    // ||                          --------                                                ||
    // || ---------- 0.25 --------                                                         ||
    // ||  0  |  0.1  |  0.2  |  0.3  | 0.4  |  0.5  |  0.6  |  0.7  |  0.8  |  0.9  |  1  ||

    half shade = smoothstep(0.15, 0.55, luminance);
    return shade;
}

half GetSpecular (CelLightingInput input, half3 lightDirectionWS, half shade) {
    half phong = PhongReflection(input.worldNormal, input.worldViewDirection, lightDirectionWS, input.smoothness*100);
    return smoothstep(0.15, 1.0, phong * shade) * input.specularIntensity;
}

half GetAccent (half luminance) {

    // accent is how near luminance is to 0.2, it transitions from 1 when the difference is 0 to 0 when the difference is 0.65, then it plateaus
    // sqrt is to make the transition less linear

    // || --------------- 1 ---------------                                                ||
    // ||                                   ----------------                               ||
    // ||                                                    --------------- 0 ----------- ||
    // ||  0  |  0.1  |  0.2  |  0.3  | 0.4  |  0.5  |  0.6  |  0.7  |  0.8  |  0.9  |  1  ||
    
    return ( 1 - sqrt( abs(0.2 - luminance) ) ) * 0.35;
}




half3 CelShade( CelLightingInput input, half3 lightColor, half3 lightDir, half lightShadowAtten, half lightDistanceAtten ) {
    lightDir = normalize(lightDir);
    
    half luminance = GetLuminance(input, lightDir, lightShadowAtten);
    half shade = GetShade(luminance);

    half3 litColor = lightColor * lightDistanceAtten * _AmbientLight;
    half3 shadedColor = litColor*0.2;

    half3 finalColor = lerp(shadedColor, litColor, shade);

    if (input.accentIntensity > 0) {

        half accent = GetAccent(luminance);
        finalColor = lerp(finalColor, ColorSaturation(finalColor, input.accentIntensity + 1).rgb, accent);
    }

    if (input.specularIntensity > 0 && input.smoothness > 0) {

        half specular = GetSpecular(input, lightDir, shade);
        finalColor += litColor * specular;
    }

    return finalColor;

}

half3 SimpleCelShade( CelLightingInput input, half3 lightColor, half3 lightDir, half lightShadowAtten, half lightDistanceAtten ) {
    half luminance = GetLuminance(input, normalize(lightDir), lightShadowAtten);
    half shade = GetShade(luminance);

    half3 litColor = lightColor * lightDistanceAtten * _AmbientLight;
    half3 shadedColor = litColor*0.2;

    return lerp(shadedColor, litColor, shade);
}











half4 CelLighting( half4 baseColor, CelLightingInput input ) {
    input.worldNormal = normalize(input.worldNormal);
    input.worldViewDirection = normalize(input.worldViewDirection);

    #ifdef SHADERGRAPH_PREVIEW

        half3 celColor = CelShade( input, defaultLightColor, defaultLightDir, 1, 1 );

        return baseColor * half4(celColor, 1);

    #else
    
        Light light = GetMainLight(input.shadowCoord, input.worldPosition, 1);

        half3 celColor = CelShade( input, light.color, light.direction, light.shadowAttenuation, light.distanceAttenuation );

        #ifdef _ADDITIONAL_LIGHTS
            uint lightsCount = GetAdditionalLightsCount();
            for (uint lightIndex = 0u; lightIndex < lightsCount; ++lightIndex) {
                light = GetAdditionalLight(lightIndex, input.worldPosition, 1);

                celColor += CelShade( input, light.color, light.direction, light.shadowAttenuation, light.distanceAttenuation );
            }
        #endif
        
        return baseColor * half4(celColor, 1);

    #endif
}

half4 SimpleCelLighting( half4 baseColor, CelLightingInput input ) {
    input.worldViewDirection = normalize(input.worldViewDirection);

    #ifdef SHADERGRAPH_PREVIEW

        half3 celColor = SimpleCelShade( input, defaultLightColor, defaultLightDir, 1, 1 );
        return baseColor * half4(celColor, 1);

    #else

        Light light = GetMainLight(input.shadowCoord, input.worldPosition, 1);
        
        half3 celColor = SimpleCelShade( input, light.color, light.direction, light.shadowAttenuation, light.distanceAttenuation );

        #ifdef _ADDITIONAL_LIGHTS
            uint lightsCount = GetAdditionalLightsCount();
            for (uint lightIndex = 0u; lightIndex < lightsCount; ++lightIndex) {
                light = GetAdditionalLight(lightIndex, input.worldPosition, 1);

                celColor += SimpleCelShade( input, light.color, light.direction, light.shadowAttenuation, light.distanceAttenuation );
            }
        #endif
        
        return baseColor * half4(celColor, 1);

    #endif
}


#endif