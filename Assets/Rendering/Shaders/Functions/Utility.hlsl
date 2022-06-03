#ifndef HLSL_UTILITY_INCLUDED
#define HLSL_UTILITY_INCLUDED

SamplerState flowmap_point_repeat_sampler;

float remap(float In, float InMin, float InMax, float OutMin, float OutMax) {
    return OutMin + (In - InMin) * (OutMax - OutMin) / (InMax - InMin);
}

float rand(float2 uv){
    return frac(sin(dot(uv, float2(12.9898, 78.233))) * 43758.5453);
}

float2 PerlinDir(float2 uv) {
    uv = uv % 289;
    float x = (34 * uv.x + 1) * uv.x % 289 + uv.y;
    x = (34 * x + 1) * x % 289;
    x = frac(x / 41) * 2 - 1;
    return normalize(float2(x - floor(x + 0.5), abs(x) - 0.5));
}

float Perlin(float2 uv) {
    float2 iuv = floor(uv);
    float2 fuv = frac(uv);
    float d00 = dot(PerlinDir(iuv), fuv);
    float d01 = dot(PerlinDir(iuv + float2(0, 1)), fuv - float2(0, 1));
    float d10 = dot(PerlinDir(iuv + float2(1, 0)), fuv - float2(1, 0));
    float d11 = dot(PerlinDir(iuv + float2(1, 1)), fuv - float2(1, 1));
    fuv = fuv * fuv * fuv * (fuv * (fuv * 6 - 15) + 10);
    return lerp(lerp(d00, d01, fuv.y), lerp(d10, d11, fuv.y), fuv.x);
}

half FractalNoise(half2 uv) {
    half noiseSum = 0;
    half amplitude = 1;
    half frequency = 1;

    for (int i = 0; i < 5; i++) {
        noiseSum += Perlin(uv * frequency) * amplitude;
        frequency *= 2;
        amplitude *= 0.5;
    }

    return noiseSum;
}

half MovingFractalNoise(half2 uv, float Time, half Scale) {

    half noise1 = Perlin( half2(Time,-Time) + uv * Scale );
    half noise2 = Perlin( half2(-Time,Time) + uv * Scale * 2 ) * 0.5;
    half noise3 = Perlin( half2(Time,Time) + uv * Scale * 4 ) * 0.25;
    half noise4 = Perlin( half2(-Time,-Time) + uv * Scale * 8 ) * 0.125;
    half noise5 = Perlin( half2(Time,-Time) + uv * Scale * 16 ) * 0.0625;

    return saturate(noise1 + noise2 + noise3 + noise4 + noise5);
}

half4 FlowMap(Texture2D Texture, Texture2D Flowmap, float2 uv, float time, float strength, bool isNormalMap = false){
    half2 flowDir = Flowmap.Sample(flowmap_point_repeat_sampler, uv).rg;
    flowDir = ( flowDir - half2(0.5, 0.5) ) * -2;

    float fracTime1 = frac(time);
    float fracTime2 = frac(time + 0.5);

    float t = abs((fracTime1 - 0.5) * 2);

    half4 sample1 = Texture.Sample(flowmap_point_repeat_sampler, uv + flowDir * fracTime1 * strength);
    half4 sample2 = Texture.Sample(flowmap_point_repeat_sampler, uv + flowDir * fracTime2 * strength);

    if (isNormalMap) {
        sample1 = half4(UnpackNormal(sample1), 1);
        sample2 = half4(UnpackNormal(sample2), 1);
    }

    return lerp(sample1, sample2, t);
}

half4 FlowMap(sampler2D Texture, sampler2D Flowmap, float2 uv, float time, float strength, bool isNormalMap = false){
    half2 flowDir = tex2D(Flowmap, uv).rg;
    flowDir = ( flowDir - half2(0.5, 0.5) ) * -2;

    float fracTime1 = frac(time);
    float fracTime2 = frac(time + 0.5);

    float t = abs((fracTime1 - 0.5) * 2);

    half4 sample1 = tex2D(Texture, uv + flowDir * fracTime1 * strength);
    half4 sample2 = tex2D(Texture, uv + flowDir * fracTime2 * strength);

    if (isNormalMap) {
        sample1 = half4(UnpackNormal(sample1), 1);
        sample2 = half4(UnpackNormal(sample2), 1);
    }

    return lerp(sample1, sample2, t);
}
#endif