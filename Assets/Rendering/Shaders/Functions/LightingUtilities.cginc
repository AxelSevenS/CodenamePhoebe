#ifndef LIGHTING_UTILITIES
#define LIGHTING_UTILITIES


float3 ComputeSpecular(float3 L, float3 N, float3 viewVector, float smoothness, float shininess){
    if (shininess > 0){
        float specularAngle = acos(dot(normalize(L - viewVector), N));
        float specularExponent = specularAngle / (1 - smoothness);
        float specular = exp(-specularExponent * specularExponent)*shininess;
        return specular *_LightColor0.xyz;
    }
    return float3(0, 0, 0);
}

float3 ComputeDiffuse(float3 L, float3 N, float3 ambientLight, float attenuation, float isBase){
    float lambert = saturate( dot( N, L ) );
    float3 diffuseLight = lambert * attenuation * _LightColor0.xyz;

    if (isBase == 1){
        diffuseLight = diffuseLight + ambientLight; // adds the indirect diffuse lighting
    }
    return diffuseLight;
}

float3 ComputeSubScattering(float3 L, float3 N, float3 viewVector, float thickness, float3 color, float distortion, float power, float scale){

    float subScattering = dot(-viewVector, -(L + N * distortion));
    subScattering = pow(subScattering, power);
    subScattering = dot(subScattering, scale);
    return saturate(subScattering * color);
}

float3 ComputeNormals(float3 nor, float3 tan, float3 bit, float3 normals){
    float3x3 mtxTangentToWorld = {
        bit.x, tan.x, nor.x, 
        bit.y, tan.y, nor.y, 
        bit.z, tan.z, nor.z
    };
    return normalize( mul(mtxTangentToWorld, normals) );
}

#endif