#ifndef URPFUNCS_INCLUDED
#define URPFUNCS_INCLUDED


#include "WaterWaveDisplacement.hlsl"


void CalculateNoise_float(float3 Position, float NoiseSpeed, float NoiseScale, out float2 Out){

    Out = float2(Position.x*NoiseScale + _Time[1]*-_WindDirection.x/20 * NoiseSpeed, Position.z*NoiseScale + _Time[1]*-_WindDirection.z/20 * NoiseSpeed);
}

void CalculateWave_float(float3 Position, float WaveStrength, float WaveSpeed, float WaveFrequency, out float Out){

    Out = WaveDisplacement(Position, WaveStrength, WaveSpeed, WaveFrequency, _Time[1]);
}
 
void WindDirection_float(out float3 Out){

    Out = _WindDirection;
}

#endif