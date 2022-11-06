#ifndef SHADERGRAPH_CEL_LIGHTING_INCLUDED
#define SHADERGRAPH_CEL_LIGHTING_INCLUDED

#include "Utility.hlsl"
#include "CelLighting.hlsl"

void CelLighting_half( half4 baseColor, float3 positionOS, half3 normalOS, half specularIntensity, half smoothness, half accentIntensity, out half4 finalColor ){
    CelLightingInput lightingInput = GetCelLightingInput( positionOS, normalOS, specularIntensity, smoothness, accentIntensity );
    finalColor = CelLighting( baseColor, lightingInput );
}

void SimpleCelLighting_half( half4 baseColor, float3 positionOS, half3 normalOS, out half4 finalColor ){
    CelLightingInput lightingInput = GetCelLightingInput( positionOS, normalOS );
    finalColor = SimpleCelLighting( baseColor, lightingInput );
}

void FlowMap_half( UnityTexture2D sampledTexture, UnityTexture2D flowMap, half2 uv, half scale, half offset, bool isNormalMap, out half4 flowColor ){
    Texture2D convertedTexture = (Texture2D)sampledTexture;
    Texture2D convertedFlowMap = (Texture2D)flowMap;
    flowColor = FlowMap( convertedTexture, convertedFlowMap, uv, scale, offset, isNormalMap );
}

#endif