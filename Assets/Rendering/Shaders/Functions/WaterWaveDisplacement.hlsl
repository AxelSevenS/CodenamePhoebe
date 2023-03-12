#ifndef WATERDISPLACEMENT_INCLUDED
#define WATERDISPLACEMENT_INCLUDED

uniform float3 _WindDirection;

float WaveDisplacement(float3 Position, float WaveStrength, float WaveSpeed, float WaveFrequency, float Time){

    return WaveStrength * sin(Time*WaveSpeed + (Position.x*_WindDirection.x + Position.z*_WindDirection.z) * WaveFrequency);
}


#endif