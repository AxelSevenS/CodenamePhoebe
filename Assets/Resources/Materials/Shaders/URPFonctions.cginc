#ifndef MYHLSLINCLUDE_INCLUDED
#define MYHLSLINCLUDE_INCLUDED


// void GetAmbientLight_float(out float4 Out){
//     Out = _AmbientLight;
// }

uniform float3 _AmbientLight;
uniform float3 _WindDirection;

void CalculateNoise_float(float3 Position, float NoiseSpeed, float NoiseScale, out float2 Out){

    Out = float2(Position.x*NoiseScale + _Time[1]*-_WindDirection.x/20 * NoiseSpeed, Position.z*NoiseScale + _Time[1]*-_WindDirection.z/20 * NoiseSpeed);
}

void CalculateWave_float(float3 Position, float WaveStrength, float WaveSpeed, float WaveFrequency, out float Out){

    Out = WaveStrength * sin(_Time[1]*2*WaveSpeed + (Position.x*_WindDirection.x + Position.z*_WindDirection.z) * WaveFrequency);
}

void AmbientLight_float(out float3 Out){

    Out = _AmbientLight;
}

void WindDirection_float(out float3 Out){

    Out = _WindDirection;
}

#endif