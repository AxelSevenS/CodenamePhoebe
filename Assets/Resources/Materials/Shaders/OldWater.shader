Shader "LowPoly/OldWater"
{
    Properties
    {	
        _DepthGradientShallow("Depth Gradient Shallow", Color) = (0.325, 0.807, 0.971, 0.725)
        _DepthGradientDeep("Depth Gradient Deep", Color) = (0.086, 0.407, 1, 0.749)
        _SpecularCol("Specular Color", Color) = (0.086, 0.407, 1, 0.749)

        _DepthMultiplier("Depth Multiplier", range(0,4)) = 1.2
        _AlphaMultiplier ("Alpha Multiplier", range(0,0.5)) = 0.47
        _SurfaceNoiseCutoff ("Surface Noise Cutoff", Range(0, 1)) = 0.777
        _WaveCutoff ("Surface Noise Cutoff", Range(0, 1)) = 0.777
        _ShoreFadeStrength ("Shore Fade Strength", Range(0, 1)) = 0.777
        _FoamDistance ("Foam Distance", float) = 0.777
        _WaveSizeAmplifier ("Wave Size Amplifier", range(0,50)) = 0
        _WaveRoughness ("Wave Roughness", range(1,50)) = 0
        _Smoothness ("Smoothness", range(0,1)) = 0.777
        _MainTex ("Color (RGB) Alpha (A)", 2D) = "white"

        _WaterNoiseMap ("Wave Noise Texture (Noise Map Preferred)", 2D) = "white"
        _DisturbanceScale ("Disturbance Wave Scale", float) = 10
        _DisturbanceSpeed ("Disturbance Wave Speed", float) = 10
        _DisturbanceFrequency ("Disturbance Wave Frequency", float) = 10

        _RipleScale ("Riple Wave Scale", float) = 10
        _RipleSpeed ("Riple Wave Speed", float) = 10
        _RipleFrequency ("Riple Wave Frequency", float) = 10
        _WaterNormalMap ("Foam Noise Texture (Water Normal Map Preferred)", 2D) = "bump"

        _SurfaceNoiseScroll("Surface Noise Scroll Amount", Vector) = (0.03, 0.03, 0, 0)
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        ZWrite Off
        Cull Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
			CGPROGRAM
            #pragma vertex vert alpha
            #pragma fragment frag alpha

            #include "UnityLightingCommon.cginc"
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #include "AutoLight.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float3 tangent : TANGENT;
                float2 noiseUV : TEXCOORD0;
                float2 normalUV : TEXCOORD1;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 noiseUV : TEXCOORD0;
                float2 normalUV : TEXCOORD1;
                float3 normal : TEXCOORD2;
                float4 worldPosition : TEXCOORD3;
                float4 screenPosition : TEXCOORD4;
            };

            sampler2D _WaterNoiseMap;
            sampler2D _WaterNormalMap;
            float4 _WaterNoiseMap_ST;
            float4 _WaterNormalMap_ST;
            float _WaveSizeAmplifier;
            float _WaveRoughness;
            float2 _SurfaceNoiseScroll;
            float _DisturbanceFrequency;
            float _DisturbanceSpeed;
            float _DisturbanceScale;
            float _RipleFrequency;
            float _RipleSpeed;
            float _RipleScale;

            float3 CalculateWaveNoise(float3 base, float surfaceNoiseSample){
                float3 offset = float3(0,((surfaceNoiseSample-0.5)*_WaveSizeAmplifier),0);
                float disturbanceWave = _DisturbanceScale * sin(_Time.x * _DisturbanceSpeed + (base.x + base.z) * _DisturbanceFrequency);
                return base + offset + disturbanceWave;
            }
            /* float3 CalculateWaveNormal(float3 base, float3 surfaceNormalSample){
                float3 offset = float3((surfaceNormalSample.x/2*_WaveRoughness),(surfaceNormalSample.y/2*_WaveRoughness),0);
                float disturbanceWave = _DisturbanceScale * sin(_Time.x * _DisturbanceSpeed + (base.x + base.z) * _DisturbanceFrequency);
                return base + offset + disturbanceWave;
            } */
            v2f vert (appdata v)
            {
                v2f o;
                UNITY_INITIALIZE_OUTPUT(v2f, o);
                o.noiseUV = TRANSFORM_TEX(v.noiseUV, _WaterNoiseMap);
                float2 noiseUV = float2(o.noiseUV.x + _Time.y * (_SurfaceNoiseScroll.x/33), o.noiseUV.y + _Time.y * (_SurfaceNoiseScroll.y/33));
                float surfaceNoiseSample = tex2Dlod(_WaterNoiseMap, float4(noiseUV.xy,0,0)).r;

                o.normalUV = TRANSFORM_TEX(v.normalUV, _WaterNormalMap);
                float2 normalUV = float2(o.normalUV.x + _Time.y * (_SurfaceNoiseScroll.x/33), o.normalUV.y + _Time.y * (_SurfaceNoiseScroll.y/33));
                float3 surfaceNormalSample = UnpackNormal(tex2Dlod(_WaterNormalMap, float4(normalUV.xy, 0, 0)));

                float splashRiple = _RipleScale * sin(_Time.x * _RipleSpeed + ((v.vertex.x *v.vertex.x) + (v.vertex.z *v.vertex.z)) * _RipleFrequency);

                float3 position = CalculateWaveNoise( v.vertex, surfaceNoiseSample);

                

                o.worldPosition = mul(unity_ObjectToWorld, v.vertex);
                o.vertex = UnityObjectToClipPos(position);
                //o.vertex = UnityObjectToClipPos(v.vertex);
                //o.normal = normalize(tex2Dlod(_WaterNormalMap, float4(normalUV.xy, 0, 0)).rgb);
                //o.normal = mul(normalize(cross( nb, nt )), (float3x3)unity_WorldToObject);
                //o.normal =  mul(normalize(surfaceNormalSample), (float3x3)unity_WorldToObject);
                o.normal = mul(normalize(float3(0,-1,0) + surfaceNormalSample), (float3x3)unity_WorldToObject);
                //o.normal = v.normal;
                o.screenPosition = ComputeScreenPos(o.vertex);


                return o;
            }


            sampler2D _MainTex;
            sampler2D _CameraDepthTexture;

            float4 _DepthGradientShallow;
            float4 _DepthGradientDeep;
            float4 _SpecularCol;
            float _Smoothness;

            float _DepthMultiplier;
            float _AlphaMultiplier;
            float _ShoreFadeStrength;
            float _FoamDistance;
            float _SurfaceNoiseCutoff;


            float4 frag (v2f i) : SV_Target
            {
                float3 normalDir = normalize(i.normal);
			    float3 lightDir = _WorldSpaceLightPos0.xyz;

                float depthLinear = tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.screenPosition)).r;
                float terrainDepth = LinearEyeDepth(depthLinear);
                float waterDepth = i.screenPosition.w;
                float waterThickness = terrainDepth - waterDepth;

                
                float3 waterColor = lerp(_DepthGradientShallow, _DepthGradientDeep, 1 - exp(-waterThickness * _DepthMultiplier));
                float fresnel = 1 - (saturate(dot(lightDir, normalDir))/5 + (.5-_AlphaMultiplier));
                float shoreFade = 1 - exp(-waterThickness * _ShoreFadeStrength);
                float waterAlpha = shoreFade * fresnel;

                float3 diffuseTerm = saturate(dot(normalDir, -lightDir));
                float3 diffuseLight = diffuseTerm * _LightColor0.xyz+.8;

                float3 V = normalize( _WorldSpaceCameraPos - i.worldPosition);
                float3 H = normalize(lightDir + V);
                float3 specularHighlight = saturate(dot(H, normalDir)) * (diffuseTerm > 0);
                float specularExponent =  exp2(_Smoothness * 11) + 2;
                specularHighlight = pow(specularHighlight, specularExponent) * _Smoothness;

                float foamDepthDifference = saturate(waterThickness / _FoamDistance);
                float surfaceNoiseCutoff = foamDepthDifference * _SurfaceNoiseCutoff;

                float2 noiseUV = float2(i.noiseUV.x + _Time.y * (_SurfaceNoiseScroll.x/66), i.noiseUV.y + _Time.y * (_SurfaceNoiseScroll.y/66));
                float surfaceNoiseSample = tex2D(_WaterNoiseMap, noiseUV).r;
                float surfaceNoise = surfaceNoiseSample > _SurfaceNoiseCutoff ? 1 : 0;
                surfaceNoise += surfaceNoiseSample > foamDepthDifference ? 1 : 0;


                //waterColor *= ((diffuseLight * _DepthGradientShallow ) + (1-diffuseLight * _DepthGradientDeep ))*0.8;
                waterColor *= diffuseLight;
                //waterColor += float4(specularHighlight * _LightColor0.xyz, 1);

                //float surfaceNoiseSample = tex2D(_SurfaceNoise, i.noiseUV).r;
                //float surfaceNoise = surfaceNoiseSample < _SurfaceNoiseCutoff ? 1 : 0;

                return float4(waterColor, waterAlpha) + surfaceNoise;
                //return float4(diffuseLight * _LightColor0 + specularHighlight * _LightColor0.xyz, 1);
                //return float4(i.normal, 0.5);
                //return _DepthGradientShallow;
            }
            ENDCG
        }
    }
}