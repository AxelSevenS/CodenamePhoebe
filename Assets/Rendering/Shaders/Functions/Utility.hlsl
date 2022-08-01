#ifndef HLSL_UTILITY_INCLUDED
#define HLSL_UTILITY_INCLUDED

SamplerState flowmap_point_repeat_sampler;
float Epsilon = 1e-10;


struct LightingInput{
    float4 clipPosition;
    float3 worldPosition;
    half3 worldNormal;
    half3 worldViewDirection;
    float4 screenPosition;
    float4 shadowCoord;
};

LightingInput GetLightingInput(half3 ObjectPosition, half3 ObjectNormal) {

    LightingInput lightingInput;
    lightingInput.clipPosition = TransformObjectToHClip(ObjectPosition);
    
    lightingInput.worldPosition = TransformObjectToWorld(ObjectPosition.xyz);
    lightingInput.worldNormal = normalize(TransformObjectToWorldNormal(ObjectNormal.xyz));
    lightingInput.worldViewDirection = normalize( _WorldSpaceCameraPos.xyz - lightingInput.worldPosition );
    #ifdef SHADERGRAPH_PREVIEW
        lightingInput.screenPosition = float4(0,0,0,0);
        lightingInput.shadowCoord = float4(0,0,0,0);
    #else
        lightingInput.screenPosition = ComputeScreenPos( lightingInput.clipPosition );
        #if SHADOWS_SCREEN
            lightingInput.shadowCoord = lightingInput.screenPosition;
        #else 
            lightingInput.shadowCoord = TransformWorldToShadowCoord(lightingInput.worldPosition);
        #endif
    #endif
    return lightingInput;
}


float3 RGBtoHSV(float3 rgb)
{
    float3 hsv;
    float minVal, maxVal, delta;

    minVal = min(rgb.x, min(rgb.y, rgb.z));
    maxVal = max(rgb.x, max(rgb.y, rgb.z));

    hsv.z = maxVal;				// v
    delta = maxVal - minVal;

    if (maxVal != 0)
        hsv.y = delta / maxVal;		// s
    else {
        hsv.y = 0;
        hsv.x = -1;
        return hsv;
    }

    if (rgb.x == maxVal)
        hsv.x = (rgb.y - rgb.z) / delta;		// between yellow & magenta
    else if (rgb.y == maxVal)
        hsv.x = 2 + (rgb.z - rgb.x) / delta;	// between cyan & yellow
    else
        hsv.x = 4 + (rgb.x - rgb.y) / delta;	// between magenta & cyan

    hsv.x *= 60;				// degrees
    if (hsv.x < 0)
        hsv.x += 360;

    return hsv;
}

float3 HSVtoRGB(float3 HSV) {
    float3 RGB = HSV.z;

    float var_h = HSV.x * 6;
    float var_i = floor(var_h);   // Or ... var_i = floor( var_h )
    float var_1 = HSV.z * (1.0 - HSV.y);
    float var_2 = HSV.z * (1.0 - HSV.y * (var_h-var_i));
    float var_3 = HSV.z * (1.0 - HSV.y * (1-(var_h-var_i)));
    if (var_i == 0) 
        RGB = float3(HSV.z, var_3, var_1);
    else if (var_i == 1) 
        RGB = float3(var_2, HSV.z, var_1);
    else if (var_i == 2) 
        RGB = float3(var_1, HSV.z, var_3);
    else if (var_i == 3) 
        RGB = float3(var_1, var_2, HSV.z);
    else if (var_i == 4) 
        RGB = float3(var_3, var_1, HSV.z);
    else                 
        RGB = float3(HSV.z, var_1, var_2);
    
    return (RGB);
}


float4 ColorSaturation(float4 color, float saturation) {
    float3 hsl = RGBtoHSV(color);
    hsl.y = hsl.y * saturation;
    return float4( HSVtoRGB(hsl), color.a );
}
float3 ColorSaturation(float3 color, float saturation) {
    float3 hsl = RGBtoHSV(color);
    hsl.y = hsl.y * saturation;
    return HSVtoRGB(hsl);
}
    // float4 saturatedColor = mix(
    //     float3( dot(color, float3(0.299, 0.587, 0.114)) ),
    //     color,
    //     saturation
    // )
    // saturatedColor.a = color.a;
    // return saturatedColor;

float remap(float In, float InMin, float InMax, float OutMin, float OutMax) {
    return OutMin + (In - InMin) * (OutMax - OutMin) / (InMax - InMin);
}

float3 NormalBlend( float3 A, float3 B) {
    return normalize(float3(A.rg + B.rg, A.b * B.b));
}

float3 NormalStrength(float3 normal, float Strength) {
    return float3(normal.rg * Strength, lerp(1, normal.b, saturate(Strength)));
}

float Saturation(float In, float Saturation) {
    float luma = dot(In, float3(0.2126729, 0.7151522, 0.0721750));
    return luma.xxx + Saturation.xxx * (In - luma.xxx);
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

float Dither(float2 ScreenPosition) {
    float2 uv = ScreenPosition * _ScreenParams.xy;
    float DITHER_THRESHOLDS[16] = {
        1.0 / 17.0,  9.0 / 17.0,  3.0 / 17.0, 11.0 / 17.0,
        13.0 / 17.0,  5.0 / 17.0, 15.0 / 17.0,  7.0 / 17.0,
        4.0 / 17.0, 12.0 / 17.0,  2.0 / 17.0, 10.0 / 17.0,
        16.0 / 17.0,  8.0 / 17.0, 14.0 / 17.0,  6.0 / 17.0
    };
    uint index = (uint(uv.x) % 4) * 4 + uint(uv.y) % 4;
    return 1 - DITHER_THRESHOLDS[index];
}

bool ProximityDither(float3 worldPosition, float4 screenPosition) {
    float proximityAlphaMultiplier = (distance(_WorldSpaceCameraPos, worldPosition) - 0.25) * 3;

    float ditherMask = Dither( screenPosition.xy / screenPosition.w );
    bool shouldClip = ditherMask <= 1 - proximityAlphaMultiplier;
    clip ( shouldClip ? -1 : 0 );
    return shouldClip;
}

float3 ComputeNormals(float3 normalWS, float3 tangentWS, float3 bitangentWS, float3 normalTS){
    float3x3 mtxTangentToWorld = {
        bitangentWS.x, tangentWS.x, normalWS.x, 
        bitangentWS.y, tangentWS.y, normalWS.y, 
        bitangentWS.z, tangentWS.z, normalWS.z
    };
    return normalize( mul(mtxTangentToWorld, normalTS) );
}

#endif