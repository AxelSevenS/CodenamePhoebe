#ifndef WATER_FRAGMENT_INCLUDED
#define WATER_FRAGMENT_INCLUDED

#include "WaterVertex.hlsl"
#include "Packages/com.seven.lunar-render-pipeline/ShaderLibrary/SurfaceData.hlsl"
#include "Packages/com.seven.lunar-render-pipeline/ShaderLibrary/DeclareDepthTexture.hlsl"

float2 NormalUVOffset(float2 position, float2 movement, float noiseSpeed, float noiseScale, float time) {
    return float2(position.x * noiseScale + time * -movement.x/20 * noiseSpeed, position.y * noiseScale + time * -movement.y/20 * noiseSpeed);
}

void WaterFragment( inout SurfaceData surfaceData, inout InputData inputData, VertexOutput input, half facing ) {

    if (facing > 0) {

        float depth = - Linear01Depth(SampleSceneDepth(input.positionSS.xy).r, _ZBufferParams) * _ProjectionParams.z;

        float depthColorFactor = saturate(1 - exp(depth * _DepthStrength));

        surfaceData.albedo = lerp(_ShallowColor, _DeepColor, depthColorFactor);


        float alphaDepth = saturate(1 - exp(depth * _AlphaStrength));
        float fresnel = pow(1 - saturate(dot(inputData.viewDirectionWS, inputData.normalWS)), _AlphaStrength);
        
        surfaceData.alpha = saturate(alphaDepth * fresnel + 0.35);

    } else {

        float fresnel = 1 - saturate(dot(inputData.viewDirectionWS, inputData.normalWS));
        float distance = length(input.positionWS - _WorldSpaceCameraPos);
        surfaceData.albedo = lerp(_ShallowColor, _DeepColor, saturate(/* fresnel *  */log(distance * 0.12)));
        surfaceData.alpha = saturate(distance * _AlphaStrength);
    }

    

    surfaceData.specular = _Specular;
    // surfaceData.metallic = 0;
    surfaceData.smoothness = _Smoothness;
    surfaceData.emission = half3(0, 0, 0);

    if ( _NormalIntensity != 0) {

        float2 positionX = inputData.positionWS.zy;
        float2 windX = _WindDirection.zy;
        float2 positionY = inputData.positionWS.xz;
        float2 windY = _WindDirection.xz;
        float2 positionZ = inputData.positionWS.xy;
        float2 windZ = _WindDirection.xy;

        float2 noiseUVX1 = NormalUVOffset(positionX, windX, _NormalSpeed * 2, _NormalScale, _Time[1]);
        float2 noiseUVX2 = NormalUVOffset(positionX, windX, _NormalSpeed * 2 * -0.1, _NormalScale, _Time[1]) * float2(-1, -1);
        float2 noiseUVY1 = NormalUVOffset(positionY, windY, _NormalSpeed * 2, _NormalScale, _Time[1]);
        float2 noiseUVY2 = NormalUVOffset(positionY, windY, _NormalSpeed * 2 * -0.1, _NormalScale, _Time[1]) * float2(-1, -1);
        float2 noiseUVZ1 = NormalUVOffset(positionZ, windZ, _NormalSpeed * 2, _NormalScale, _Time[1]);
        float2 noiseUVZ2 = NormalUVOffset(positionZ, windZ, _NormalSpeed * 2 * -0.1, _NormalScale, _Time[1]) * float2(-1, -1);

        float3 triW = abs(inputData.normalWS);
        triW / (triW.x + triW.y + triW.z);

        float3 normalX1 = UnpackNormal(tex2D(_NormalMap, noiseUVX1));
        float3 normalX2 = UnpackNormal(tex2D(_NormalMap, noiseUVX2));
        float3 normalX = SafeNormalize( (normalX1 + normalX2) / 2 );

        float3 normalY1 = UnpackNormal(tex2D(_NormalMap, noiseUVY1));
        float3 normalY2 = UnpackNormal(tex2D(_NormalMap, noiseUVY2));
        float3 normalY = SafeNormalize( (normalY1 + normalY2) / 2 );

        float3 normalZ1 = UnpackNormal(tex2D(_NormalMap, noiseUVZ1));
        float3 normalZ2 = UnpackNormal(tex2D(_NormalMap, noiseUVZ2));
        float3 normalZ = SafeNormalize( (normalZ1 + normalZ2) / 2 );


        surfaceData.normalTS = SafeNormalize( (normalX * triW.x + normalY * triW.y + normalZ * triW.z) );
        half3 mapNormal = mul( inputData.tangentToWorld, surfaceData.normalTS);
        inputData.normalWS = lerp(inputData.normalWS, mapNormal, _NormalIntensity);
    }
}

#define CustomFragment(surfaceData, inputData, input, facing) WaterFragment(surfaceData, inputData, input, facing)

#endif // WATER_FRAGMENT_INCLUDED