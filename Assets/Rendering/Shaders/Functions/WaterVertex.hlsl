#ifndef WATER_VERTEX_INCLUDED
#define WATER_VERTEX_INCLUDED

#include "Functions/WaterWaveDisplacement.hlsl"

void WaterVertexDisplacement( inout VertexOutput output ) {
    float3 tangent;
    float3 binormal;
    float3 normal;
    float3 displacement = WaveDisplacement(output.positionWS, _Time[1], tangent, binormal, normal);
    output.positionWS += displacement;
    output.normalWS = normal;
    output.tangentWS = tangent;
    output.bitangentWS = -binormal;
}

bool WaterClipping( in VertexOutput input, half facing ) {
    return false;
}

#define CustomVertexDisplacement(output) WaterVertexDisplacement(output)
#define CustomClipping(output, facing) WaterClipping(output, facing)

#endif // WATER_VERTEX_INCLUDED