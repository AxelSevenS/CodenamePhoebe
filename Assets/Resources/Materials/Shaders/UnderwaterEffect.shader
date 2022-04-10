Shader "Penrose/UnderwaterEffect"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _FogColor ("Fog Color", 2D) = "white"
    }
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 screenPosition : TEXCOORD4;
            };

            sampler2D _MainTex;
            float4 _FogColor;
            sampler2D _CameraDepthTexture;
            sampler2D _CameraDepthNormalsTexture;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv =  v.uv.xy;
                o.screenPosition = ComputeScreenPos(o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float depth = tex2D (_CameraDepthTexture, i.uv).r*100;
                //float depth = i.screenPosition.w*100;
                float3 originalCol = tex2D (_MainTex, i.uv);
                float3 color = lerp(_FogColor.rgb, originalCol, saturate(depth*5));
                //color = lerp(_FogColor.rgb, color, saturate(depth*100));
                int farClip = saturate(depth*200) < 0.1 ? 1 : 0;
                color = lerp(color, originalCol, farClip);

                //float3 color = originalCol;
                return float4(color, 1);
            }
            ENDCG
        }
    }
}
