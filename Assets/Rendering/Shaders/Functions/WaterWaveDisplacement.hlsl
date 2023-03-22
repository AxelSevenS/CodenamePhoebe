#ifndef WATERDISPLACEMENT_INCLUDED
#define WATERDISPLACEMENT_INCLUDED

uniform float3 _WindDirection;

static const half PI_CONST = 3.14159265359;

#pragma target 5.0


uniform StructuredBuffer<float4> _GerstnerWaves;


float3 GerstnerWave(float3 Position, float2 WaveDirection, float WaveStrength, float WaveFrequency, float Time){

    float waveLength = 2 * PI_CONST / WaveFrequency;
    float phaseSpeed = sqrt(9.8 / waveLength);

    float f = waveLength * (dot(WaveDirection, Position.xz) - phaseSpeed * Time);
    float waveHeight = WaveStrength / waveLength;

    return float3(
        WaveDirection.x * (waveHeight * cos(f)), 
        waveHeight * sin(f), 
        WaveDirection.y * (waveHeight * cos(f))
    );

}

float3 GerstnerWave(float3 Position, float4 waveData, float Time) {
    return GerstnerWave(Position, waveData.xy, waveData.z, waveData.w, Time);
}

float3 WaveDisplacement(float3 Position, float Time){

    float3 displacement = float3(0, 0, 0);
    for (int i = 0; i < (int)_GerstnerWaves.Length; i++){
        float4 waveData = _GerstnerWaves[i];
        displacement += GerstnerWave(Position, waveData, Time);
    }

    return displacement;
}



float3 GerstnerWave(float3 Position, float2 WaveDirection, float WaveStrength, float WaveFrequency, float Time, inout float3 tangent, inout float3 binormal){

    float waveLength = 2 * PI_CONST / WaveFrequency;
    float phaseSpeed = sqrt(9.8 / waveLength);

    float f = waveLength * (dot(WaveDirection, Position.xz) - phaseSpeed * Time);
    float waveHeight = WaveStrength / waveLength;


    tangent += float3(
        -WaveDirection.x * WaveDirection.x * (WaveStrength * sin(f)),
        WaveDirection.x * (WaveStrength * cos(f)),
        -WaveDirection.x * WaveDirection.y * (WaveStrength * sin(f))
    );
    binormal += float3(
        -WaveDirection.x * WaveDirection.y * (WaveStrength * sin(f)),
        WaveDirection.y * (WaveStrength * cos(f)),
        -WaveDirection.y * WaveDirection.y * (WaveStrength * sin(f))
    );

    return float3(
        WaveDirection.x * (waveHeight * cos(f)), 
        waveHeight * sin(f), 
        WaveDirection.y * (waveHeight * cos(f))
    );

}

float3 GerstnerWave(float3 Position, float4 waveData, float Time, inout float3 tangent, inout float3 binormal) {
    return GerstnerWave(Position, waveData.xy, waveData.z, waveData.w, Time, tangent, binormal);
}

float3 WaveDisplacement(float3 Position, float Time, out float3 tangent, out float3 binormal, out float3 normal){

    tangent = float3(1, 0, 0);
    binormal = float3(0, 0, 1);

    float3 displacement = float3(0, 0, 0);
    for (int i = 0; i < (int)_GerstnerWaves.Length; i++){
        float4 waveData = _GerstnerWaves[i];
        displacement += GerstnerWave(Position, waveData, Time, tangent, binormal);
    }

    normal = normalize(cross(binormal, tangent));

    return displacement;
}


float WaveHeight(float3 Position, float Time){
    
    float3 samplePosition = Position;
    for (int i = 0; i < 10; i++){
        float3 displacement = WaveDisplacement(samplePosition, Time);
        float2 offset = Position.xz - (samplePosition.xz + displacement.xz);
        samplePosition = float3(samplePosition.x + offset.x, displacement.y, samplePosition.z + offset.y);
    }

    return samplePosition.y;

}




#endif