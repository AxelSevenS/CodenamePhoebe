#include "UnityCG.cginc"
#include "Lighting.cginc"
#include "AutoLight.cginc"
#include "Assets/Resources/Materials/Shaders/LightingUtilities.cginc"

#define USE_LIGHTING

struct vertexInput{
    float4 vertex : POSITION;
    float3 normal : NORMAL;
    float4 tangent : TANGENT;
    float2 uv : TEXCOORD0;
};

struct v2f{
    float4 pos : SV_POSITION;
    float2 uv : TEXCOORD0;
    float3 normal : TEXCOORD1;
    float3 tangent : TEXCOORD2;
    float3 bitangent : TEXCOORD3;
    float3 worldPos : TEXCOORD4;
    float4 screenPosition : TEXCOORD5;
    float3 viewVector : TEXCOORD6;
    float3 cameraRay : TEXCOORD7;
    UNITY_LIGHTING_COORDS(8,9)
};

sampler2D _CameraDepthTexture;
sampler2D _CameraDepthNormalsTexture;
sampler2D _WaterDisplacementMap;

uniform sampler2D _WaterNoiseMap;
uniform sampler2D _WaterNormalMap;
uniform float4 _WaterNormalMap_ST;

uniform float4 _ColorShallow;
uniform float4 _ColorDeep;

uniform float _Shininess;
uniform float _Smoothness;

uniform float _NormalIntensity;
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

    return float2(base.x*_NoiseScale + _ElapsedTime*-_WindDirection.x/20*(_WaveSpeed*_NoiseSpeed), base.y*_NoiseScale + _ElapsedTime*-_WindDirection.z/20*_WaveSpeed);
}

float CalculateWave(float3 base){

    return _WaveScale * sin(_ElapsedTime*2*_WaveSpeed + (base.x*_WindDirection.x + base.z*_WindDirection.z) * _WaveFrequency);
}

v2f vert (vertexInput v)
{
    v2f o;
    UNITY_INITIALIZE_OUTPUT(v2f, o);
    
    o.uv = v.vertex.xz;
    
    // Surface Displacement and Update Normals to Fit
    float2 noiseUV = CalculateNoise(o.uv);
    float surfaceNoiseSample = tex2Dlod(_WaterNoiseMap, float4(noiseUV,0,0)).r;

    float3 position = v.vertex;
    position.y += ((surfaceNoiseSample - .5)*_NoiseStrength + CalculateWave(v.vertex));

    float3 modifiedTangent = v.vertex + v.tangent;
    modifiedTangent.y += CalculateWave(v.tangent);

    float3 modifiedBitangent = v.vertex + cross(v.normal, v.tangent);
    modifiedBitangent.y += CalculateWave(cross(v.normal, v.tangent));

    modifiedTangent -= position;
    modifiedBitangent -= position;

    // Pass Data to Fragment shader
    o.pos = UnityObjectToClipPos(position);
    o.tangent = UnityObjectToWorldNormal(modifiedTangent);
    o.bitangent = UnityObjectToWorldDir(modifiedBitangent);
    o.normal = cross(o.tangent, o.bitangent) * (v.tangent.w * unity_WorldTransformParams.w);

    o.worldPos = mul( unity_ObjectToWorld, v.vertex );
    o.screenPosition = ComputeScreenPos(o.pos);
        
    float3 viewVector = mul(unity_CameraInvProjection, float4((o.screenPosition.xy/o.screenPosition.w) * 2 - 1, 0, -1));
    o.viewVector = mul(unity_CameraToWorld, float4(viewVector,0));

    /* float4 cameraRay = float4( v.uv * 2.0 - 1.0, 1.0, 1.0);
    cameraRay = mul( _CameraInverseProjectionMatrix, cameraRay);
    o.cameraRay = cameraRay.xyz / cameraRay.w; */


    TRANSFER_VERTEX_TO_FRAGMENT(o);
    return o;
}


float4 frag (v2f i) : SV_Target{
    // Import Normal Map
    float3 tanSpNormalMap = UnpackNormal(tex2Dlod( _WaterNormalMap, float4(CalculateNoise(i.uv),0,0) ));
    tanSpNormalMap += UnpackNormal(tex2Dlod( _WaterNormalMap, float4(CalculateNoise(i.uv*-0.3),0,0) ));
    tanSpNormalMap /= 2;
    
    tanSpNormalMap = lerp(float3(0,0,1), tanSpNormalMap, _NormalIntensity);

    float3 N = ComputeNormals(i.normal, i.tangent, i.bitangent, tanSpNormalMap);

    // Measure Depth of Water
    float depthLinear = tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.screenPosition)).r;
    float terrainDepth = LinearEyeDepth(depthLinear);
    float waterDepth = i.screenPosition.w; 
    float waterThickness = terrainDepth - waterDepth;
    
    float waterAlpha = 1;
    
    // Calculate Color of the Water
    float3 waterColor = lerp(_ColorShallow, _ColorDeep, 1 - exp(-waterThickness * .08));

    #ifdef USE_LIGHTING
        float3 L = normalize( UnityWorldSpaceLightDir( i.worldPos ) );

        // Calculate Opacity of the Water
        float shoreFade = 1 - exp(-waterThickness * .2);
        float fresnel = pow(1 - saturate(dot(L, N)), .2);
        waterAlpha = saturate(max(0.7,fresnel)*shoreFade);

        float attenuation = LIGHT_ATTENUATION(i);
        
        #ifdef IS_IN_BASE_PASS
            float isBase = 1;
        #else
            float isBase = 0;
        #endif
                
        float3 diffuseLight = ComputeDiffuse(L, N, _AmbientLight, attenuation, isBase);

        float3 specularLight = ComputeSpecular(L, N, i.viewVector, _Smoothness, _Shininess) * attenuation; 


        // Screen Space Reflections
        /* float3 decodedNormal;
        float decodedDepth;
        DecodeDepthNormal( tex2D( _CameraDepthNormalsTexture, i.uv), decodedDepth, decodedNormal);
        float3 pixelPosition = i.cameraRay * decodedDepth; */

        /* float attenuation = LIGHT_ATTENUATION(i);
        attenuation = 1; */

        /* half3 worldViewDir = normalize(UnityWorldSpaceViewDir(i.worldPosition));
        half3 worldRefl = reflect(-worldViewDir, N);

        half4 skyData = UNITY_SAMPLE_TEXCUBE(unity_SpecCube0, worldRefl);
        half3 skyColor = DecodeHDR (skyData, unity_SpecCube0_HDR); */
        return saturate(float4(waterColor * diffuseLight + specularLight, 1/* waterAlpha + (specularLight.x / _LightColor0.x) */));
    #else
        #ifdef IS_IN_BASE_PASS
            return waterColor;
        #else
            return 0;
        #endif
    #endif
}