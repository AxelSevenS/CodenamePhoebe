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
    float4 worldPos : TEXCOORD4;
    float3 viewVector : TEXCOORD5;
    float4 screenPosition : TEXCOORD6;
    /* float3 cameraRay : TEXCOORD3;*/
    UNITY_LIGHTING_COORDS(7,8)
};

v2f vert (vertexInput v)
{
    v2f o;
    UNITY_INITIALIZE_OUTPUT(v2f, o);
    
    o.uv = v.uv;

    // Pass Data to Fragment shader
    o.pos = UnityObjectToClipPos(v.vertex);
    o.tangent = UnityObjectToWorldNormal(v.tangent);
    o.bitangent = UnityObjectToWorldDir(cross(v.normal, v.tangent));
    o.normal = UnityObjectToWorldNormal(v.normal);

    o.worldPos = mul( unity_ObjectToWorld, v.vertex );
    o.screenPosition = ComputeScreenPos(v.vertex);
        

    // float4 cameraRay = float4( v.uv * 2.0 - 1.0, 1.0, 1.0);
    // cameraRay = mul( _CameraInverseProjectionMatrix, cameraRay);
    // o.cameraRay = cameraRay.xyz / cameraRay.w;

    TRANSFER_VERTEX_TO_FRAGMENT(o);
    TRANSFER_SHADOW(o)
    return o;
}

uniform float4 _AmbientLight;
uniform float _Shininess;
uniform float _Smoothness;
uniform float _Distortion;
uniform float _Power;
uniform float _Scale;
uniform float3 _SubScatteringColor;

uniform sampler2D _MainTex;
uniform sampler2D _NormalMap;
uniform float4 _NormalMap_ST;
uniform sampler2D _DetailMap;


float4 frag (v2f i) : SV_Target{
    float3 surfaceColor = tex2Dlod( _MainTex, float4(i.uv,0,0));
    float3 surfaceNormal = UnpackNormal(tex2Dlod( _NormalMap, float4(i.uv,0,0)));
    float surfaceGlossiness = UnpackNormal(tex2Dlod( _DetailMap, float4(i.uv,0,0))).x;
    float surfaceThickness = UnpackNormal(tex2Dlod( _DetailMap, float4(i.uv,0,0))).y;

    float3 viewVector = mul(unity_CameraInvProjection, float4((i.screenPosition.xy/i.screenPosition.w) * 2 - 1, 0, -1));
    viewVector = mul(unity_CameraToWorld, float4(viewVector,0));

    float3 N = ComputeNormals(i.normal, i.tangent, i.bitangent, surfaceNormal);

    #ifdef USE_LIGHTING
        float3 L = normalize( UnityWorldSpaceLightDir( i.worldPos ) );

        // Diffuse Lighting
        float attenuation = LIGHT_ATTENUATION(i);
        
        #ifdef IS_IN_BASE_PASS
            float3 diffuseLight = ComputeDiffuse(L, N, _AmbientLight*.4, attenuation, 1);
        #else
            float3 diffuseLight = ComputeDiffuse(L, N, _AmbientLight*.4, attenuation, 0);
        #endif
                

        float3 specularLight = ComputeSpecular(L, N, viewVector, _Smoothness, _Shininess*surfaceGlossiness) * attenuation; 

        float3 subScattering = ComputeSubScattering(L, N, viewVector, surfaceThickness, _SubScatteringColor*_AmbientLight, _Distortion, _Power, _Scale) * (1 - surfaceThickness);
        
        return saturate(float4(surfaceColor * diffuseLight + specularLight /* + subScattering */, 1));
    #else
        #ifdef IS_IN_BASE_PASS
            return surfaceColor;
        #else
            return 0;
        #endif
    #endif
}