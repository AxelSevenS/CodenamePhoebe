Shader "Hidden/SeleneGame/Outline" {

    // Properties {
    //     _MainTex ("Texture", 2D) = "white" {}
    //     _OutlineWidth ("Outline Width", Range(0, 1)) = 0.1
    //     _DepthStrength ("Depth Strength", Range(0, 1)) = 0.1
    //     _DepthThickness ("Depth Thickness", Range(0, 1)) = 0.1
    //     _DepthThreshold ("Depth Threshold", Range(0, 1)) = 0.1
    // }

    SubShader {
        
        Cull Off ZWrite Off ZTest Always

        HLSLINCLUDE

        #include "Packages/com.unity.postprocessing/PostProcessing/Shaders/StdLib.hlsl"

        float _OutlineWidth;
        float _DepthStrength;
        float _DepthThickness;
        float _DepthThreshold;

        float4 OutlineFrag(VaryingsDefault input) : SV_TARGET {
            return float4( tex2D(_MainTex, input.texcoord) * half4(1, 1, 0, 1) );
        }

        ENDHLSL

        Pass {

            HLSLINCLUDE

            #pragma vertex VertDefault
            #pragma fragment OutlineFrag

            ENDHLSL
        }
    }
}
