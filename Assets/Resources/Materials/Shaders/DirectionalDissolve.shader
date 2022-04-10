Shader "Penrose/DirectionalDissolve"
{
    Properties
    {
      _MainTex ("Texture", 2D) = "white" {}      
      _BumpMap ("Normal Map", 2D) = "bump" {}
      _BumpPower ("Normal Power", float) = 1
      _DissolveTex ("Dissolve Map", 2D) = "white" {}
      _DissolveEdgeColor ("Dissolve Edge Color", Color) = (1,1,1,0)
      [HDR] _EmissionColor("Emissive Color", Color) = (0,0,0)
      _DissolveIntensity ("Dissolve Intensity", Range(0.0, 1.0)) = 0
      _EmissiveIntensity ("Emission Intensity", float) = 1
      _DissolveEdgeRange ("Dissolve Edge Range", Range(0.0, 1.0)) = 0            
      _DissolveEdgeMultiplier ("Dissolve Edge Multiplier", Float) = 1
    }
   
    SubShader
    {
      Tags { "RenderType" = "Opaque" }
      Cull Off
   
      CGPROGRAM
      #pragma surface surf Lambert addshadow
 
      struct Input
      {
          float2 uv_MainTex;
          float2 uv_BumpMap;
          float2 uv_DissolveTex;
          float3 worldPos;
          float3 viewDir;
          float3 worldNormal;
      };
     
      sampler2D _MainTex;
      sampler2D _BumpMap;
      sampler2D _DissolveTex;
      
      uniform float _BumpPower;      
      uniform float4 _DissolveEdgeColor;  
      uniform float4 _EmissionColor;    
      uniform float _DissolveEdgeRange;
      uniform float _DissolveIntensity;
      uniform float _EmissiveIntensity;
      uniform float _DissolveEdgeMultiplier;
                
      void surf (Input IN, inout SurfaceOutput o)
      {

      	float3 localPos = IN.worldPos - mul(unity_ObjectToWorld, float4(0,0,0,1)).xyz;
      	float4 dissolveColor = tex2D(_DissolveTex, IN.uv_DissolveTex);      	      	
        half dissolveClip = dissolveColor.r - ((_DissolveIntensity-_DissolveEdgeRange) + (_DissolveIntensity*_DissolveEdgeRange));
        clip( dissolveClip );

        float colorClip = dissolveClip < _DissolveEdgeRange ? 1 : 0;
        
        float4 texColor = tex2D(_MainTex, IN.uv_MainTex);
        o.Albedo = lerp(texColor,_DissolveEdgeColor,colorClip);
        o.Emission = colorClip*_EmissionColor*_EmissiveIntensity;
                
        fixed3 normal = UnpackNormal( tex2D (_BumpMap, IN.uv_BumpMap) );
        normal.z /= _BumpPower;
        
        o.Normal = normalize(normal);
                                                             
        return;
      }
      ENDCG
    }
    Fallback "Diffuse"
 }