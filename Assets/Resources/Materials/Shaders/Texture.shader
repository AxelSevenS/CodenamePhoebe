Shader "Penrose/Texture"{
    Properties{

        _MainTex ("Texture", 2D) = "white" {}
        _NormalMap ("Normal Texture", 2D) = "bump" {}
        _DetailMap ("Detail Texture", 2D) = "white" {}

        _Shininess ("Shininess", Range(0.0, 1.0)) = 0.8
        _Smoothness ("Smoothness", Range(0.0, 1.0)) = 0.5
        _SubScatteringColor ("Subsurface Scattering Color", Color) = (1, 0.5, 0.5, 1)
        _Distortion ("SSS Distortion", Range(0.0, 1.0)) = 1
        _Power ("SSS Power", Float) = 1
        _Scale ("SSS Scale", Float) = 1
        
        //_AmbientLight ("Ambient Light", Color) = (0,0,0,0)
    }
    SubShader
    {
        Tags{
            "RenderPipeline" = "UniversalPipeline"
            "RenderType"="Opaque"
            "Queue"="Geometry"
        }
        Blend SrcAlpha OneMinusSrcAlpha

        Pass {
            Tags { "LightMode"="UniversalForward" }
            // Tags { "LightMode"="ForwardBase" }
			CGPROGRAM
            #pragma vertex vert alpha
            #pragma fragment frag alpha
            #pragma multi_compile_fwdbase
            #define IS_IN_BASE_PASS
            #include "Passes/Texture.cginc"
            ENDCG
        }

        // Pass{
        //     Tags { "LightMode"="ForwardAdd" }
        //     Blend One One // src*1 + dst*1
		// 	CGPROGRAM
        //     #pragma vertex vert alpha
        //     #pragma fragment frag alpha
        //     #pragma multi_compile_fwdadd
        //     #include "Passes/Texture.cginc"
        //     ENDCG
        // }

        Pass {
            Name "ShadowCaster"
            Tags { "LightMode" = "ShadowCaster" }

            ZWrite On ZTest LEqual

            CGPROGRAM
            #pragma target 2.0

            #pragma multi_compile_shadowcaster

            #pragma vertex vertShadowCaster
            #pragma fragment fragShadowCaster

            #include "UnityStandardShadow.cginc"

            ENDCG
        }
    }
}