#ifndef LIGHTING_UTILITIES_INCLUDED
#define LIGHTING_UTILITIES_INCLUDED

half PhongReflection( half3 normal, half3 lightDir, half3 viewDir, half smoothness ) {
    half3 V = normalize( -viewDir );
    half3 R = reflect( normalize( lightDir ), normalize( normal ) );
    return pow( saturate( dot( V, R ) ), smoothness );
}

half GaussianReflection( half3 normal, half3 lightDir, half3 viewDir, half smoothness ) {
    half specularAngle = acos( dot( normalize(lightDir + viewDir ), normalize( normal ) ) );
    half specularExponent = specularAngle / smoothness;
    return exp(-specularExponent * specularExponent);
}

// float3 ComputeSpecular(float3 L, float3 N, float3 viewVector, float smoothness, float shininess){
//     if (shininess > 0){
//         float specularAngle = acos(dot(normalize(L - viewVector), N));
//         float specularExponent = specularAngle / (1 - smoothness);
//         float specular = exp(-specularExponent * specularExponent)*shininess;
//         return specular *_LightColor0.xyz;
//     }
//     return float3(0, 0, 0);
// }

// float3 ComputeDiffuse(float3 L, float3 N, float3 ambientLight, float attenuation, float isBase){
//     float lambert = saturate( dot( N, L ) );
//     float3 diffuseLight = lambert * attenuation * _LightColor0.xyz;

//     if (isBase == 1){
//         diffuseLight = diffuseLight + ambientLight; // adds the indirect diffuse lighting
//     }
//     return diffuseLight;
// }

float3 ComputeSubScattering(float3 lightDirection, float3 normal, float3 viewDirection, float thickness, float3 color, float distortion, float power, float scale){

    float subScattering = dot(-viewDirection, -(lightDirection + normal * distortion));
    subScattering = pow(subScattering, power);
    subScattering = dot(subScattering, scale);
    return saturate(subScattering * color);
}

// float3 ComputeNormals(float3 nor, float3 tan, float3 bit, float3 normals){
//     float3x3 mtxTangentToWorld = {
//         bit.x, tan.x, nor.x, 
//         bit.y, tan.y, nor.y, 
//         bit.z, tan.z, nor.z
//     };
//     return normalize( mul(mtxTangentToWorld, normals) );
// }

#endif