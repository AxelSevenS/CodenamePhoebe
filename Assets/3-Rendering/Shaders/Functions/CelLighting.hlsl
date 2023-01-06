#ifndef CEL_LIGHTING_INCLUDED
#define CEL_LIGHTING_INCLUDED
        
#pragma multi_compile _MAIN_LIGHT_SHADOWS
#pragma multi_compile _MAIN_LIGHT_SHADOWS_CASCADE
#pragma multi_compile _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
#pragma multi_compile_fragment _ADDITIONAL_LIGHT_SHADOWS
#pragma multi_compile_fragment _SHADOWS_SOFT
#pragma multi_compile_fragment _SCREEN_SPACE_OCCLUSION
#pragma multi_compile LIGHTMAP_SHADOW_MIXING
#pragma multi_compile SHADOWS_SHADOWMASK

#include "LightingUtilities.hlsl"
#include "Utility.hlsl"

uniform half3 _AmbientLight;
uniform half _AmbientStrength;
SamplerState gradient_point_clamp_sampler;

static const half3 defaultLightColor = half3(1,1,1);
static const half3 defaultLightDir = half3(0.5,0.5,0.5);


static const half shadeUpperLimit = 0.15;
static const half lightLowerLimit = 0.55;


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

CelLightingInput GetCelLightingInput( FragLightingInput lightingInput, half specularIntensity = 0, half smoothness = 0, half accentIntensity = 0 ) {
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

CelLightingInput GetCelLightingInput( VertLightingInput lightingInput, half specularIntensity = 0, half smoothness = 0, half accentIntensity = 0 ) {
    return GetCelLightingInput( GetFragLightingInput(lightingInput), specularIntensity, smoothness, accentIntensity );
}

CelLightingInput GetCelLightingInput( half3 ObjectPosition, half3 ObjectNormal, half specularIntensity = 0, half smoothness = 0, half accentIntensity = 0 ) {
    return GetCelLightingInput(GetVertLightingInput( ObjectPosition, ObjectNormal ), specularIntensity, smoothness, accentIntensity);
}




half GetAccent(half luminance) {
    // P-Curve

    half h = 5.5 * (luminance);
    // half h = exp(luminance * -9 + 3);

    return h * exp(1-h);
}

half GetSpecular(CelLightingInput input, half3 lightDirectionWS, half shade) {
    half phong = PhongReflection(input.worldNormal, input.worldViewDirection, lightDirectionWS, input.smoothness*100);
    return smoothstep(0.15, 1.0, phong * shade) * input.specularIntensity;
}

half GetLuminance(CelLightingInput input, half3 lightDirectionWS) {
    return saturate( dot(input.worldNormal, lightDirectionWS) );
}

half GetShade(half luminance, half attenuation) {

    // Color is dark until 15% luminance then it transitions to light until 55% luminance where it plateaus
    // the minimum value is 0.25 and the maximum is 1.0
    
    // ||                                   -------- 1 ----------------------------------- || This is a curve graph in case you couldn't tell...
    // ||                          --------                                                ||
    // || ---------- 0.25 --------                                                         ||
    // ||  0  |  0.1  |  0.2  |  0.3  | 0.4  |  0.5  |  0.6  |  0.7  |  0.8  |  0.9  |  1  ||

    return smoothstep(shadeUpperLimit, lightLowerLimit, luminance) * smoothstep(0, lightLowerLimit - shadeUpperLimit, attenuation);

    // return  luminance < 0.35 ? (luminance * 0.5) + 0.075 : 
    //         luminance < 0.55 ? (luminance * 0.5) + 0.175 : 
    //         (luminance * 0.5) + 0.25;
}




half3 CelShade( CelLightingInput input, half3 lightColor, half3 lightDir, half lightShadowAtten, half lightDistanceAtten ) {
    lightDir = normalize(lightDir);
    
    half luminance = GetLuminance(input, lightDir);
    half shade = GetShade(luminance, lightDistanceAtten * lightShadowAtten);

    half3 litColor = (lightColor * _AmbientLight);
    

    if (input.accentIntensity > 0) {

        half accent = GetAccent(shade);
        litColor = lerp(litColor, ColorSaturation(litColor, input.accentIntensity), accent);
    }

    half3 finalColor = shade * litColor;

    if (input.specularIntensity > 0 && input.smoothness > 0) {

        half specular = GetSpecular(input, lightDir, shade);
        finalColor += litColor * specular;
    }

    return finalColor;

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
        
        return baseColor * half4( saturate(_AmbientLight * _AmbientStrength + celColor), 1);

    #endif
}

half4 SimpleCelLighting( half4 baseColor, CelLightingInput input ) {
    input.accentIntensity = 0;
    input.specularIntensity = 0;
    input.smoothness = 0;
    return CelLighting(baseColor, input);
}


#endif