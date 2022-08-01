#ifndef LIGHTING_UTILITIES_INCLUDED
#define LIGHTING_UTILITIES_INCLUDED

half PhongReflection( half3 normal, half3 viewDir, half3 lightDir, half smoothness ) {
    half3 V = normalize( -viewDir );
    half3 R = reflect( normalize( lightDir ), normalize( normal ) );
    return pow( saturate( dot( V, R ) ), smoothness );
}

half GaussianReflection( half3 normal, half3 viewDir, half3 lightDir, half smoothness ) {
    half specularAngle = acos( dot( normalize(lightDir + viewDir ), normalize( normal ) ) );
    half specularExponent = specularAngle / smoothness;
    return exp(-specularExponent * specularExponent);
}

float3 ComputeSubScattering(float3 lightDirection, float3 normal, float3 viewDirection, float thickness, float3 color, float distortion, float power, float scale){

    float subScattering = dot(-viewDirection, -(lightDirection + normal * distortion));
    subScattering = pow(subScattering, power);
    subScattering = dot(subScattering, scale);
    return saturate(subScattering * color);
}

#endif