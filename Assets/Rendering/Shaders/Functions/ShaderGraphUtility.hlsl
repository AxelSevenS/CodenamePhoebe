#ifndef SHADERGRAPH_CEL_LIGHTING_INCLUDED
#define SHADERGRAPH_CEL_LIGHTING_INCLUDED

#include "Packages/com.seven.utility/ShaderLibrary/MathUtility.hlsl"

void FlowMap_half( UnityTexture2D sampledTexture, UnityTexture2D flowMap, half2 uv, half scale, half offset, bool isNormalMap, out half4 flowColor ){
    Texture2D convertedTexture = (Texture2D)sampledTexture;
    Texture2D convertedFlowMap = (Texture2D)flowMap;
    flowColor = FlowMap( convertedTexture, convertedFlowMap, uv, scale, offset, isNormalMap );
}

#endif