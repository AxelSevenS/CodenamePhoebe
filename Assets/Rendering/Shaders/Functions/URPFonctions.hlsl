#ifndef URPFUNCS_INCLUDED
#define URPFUNCS_INCLUDED


#include "WaterWaveDisplacement.hlsl"


void CalculateNoise_float(float3 Position, float Time, float NoiseSpeed, float NoiseScale, out float2 Out){

    Out = float2(Position.x*NoiseScale + Time*-_WindDirection.x/20 * NoiseSpeed, Position.z*NoiseScale + Time*-_WindDirection.z/20 * NoiseSpeed);
}

void CalculateWave_float(float3 Position, float Time, out float3 Displacement, out float3 Tangent, out float3 Binormal, out float3 Normal){
    Displacement = WaveDisplacement(Position, Time, Tangent, Binormal, Normal);
}
 
void WindDirection_float(out float3 Out){

    Out = _WindDirection;
}

#endif