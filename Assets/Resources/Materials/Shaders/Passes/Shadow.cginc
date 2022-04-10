
#include "UnityCG.cginc"
struct v2f {
    V2F_SHADOW_CASTER;
};

/* uniform sampler2D _WaterNoiseMap;

uniform float _WaveScale;
uniform float _WaveSpeed;
uniform float _WaveFrequency;
uniform float _NoiseScale;
uniform float _NoiseSpeed;
uniform float _NoiseStrength;

uniform float4 _AmbientLight;
uniform float3 _WindDirection;
uniform float _ElapsedTime;

float2 CalculateNoise(float2 base){

    return float2(base.x/150*_NoiseScale + _ElapsedTime*2*-_WindDirection.x/40*(_WaveSpeed*_NoiseSpeed), base.y/150*_NoiseScale + _ElapsedTime*2*-_WindDirection.z/40*_WaveSpeed);
}

float CalculateNewPosition(float3 base){

    return _WaveScale * sin(_ElapsedTime*2*_WaveSpeed + (base.x*_WindDirection.x + base.z*_WindDirection.z) * _WaveFrequency);
} */

v2f vertShadowCaster(appdata_base v){
    v2f o;
    
    /* // Surface Displacement and Update Normals to Fit
    float2 noiseUV = CalculateNoise(v.vertex.xz);
    float surfaceNoiseSample = tex2Dlod(_WaterNoiseMap, float4(noiseUV,0,0)).r - .5;

    float3 newPos = v.normal * (surfaceNoiseSample*_NoiseStrength + CalculateNewPosition(v.vertex));
    v.vertex = v.vertex + float4(newPos, v.vertex.w); */

    TRANSFER_SHADOW_CASTER_NORMALOFFSET(o)
    return o;
}

float4 fragShadowCaster(v2f i) : SV_Target{
    SHADOW_CASTER_FRAGMENT(i)
}