Shader "Penrose/Water"{
    Properties{	
        [Enum(Off,0,Front,1,Back,2)] _Cull ("Cull", Int) = 0
    }
    SubShader
    {
        Tags{
            "RenderType"="Opaque"
            "Queue"="Geometry"
        }
        Cull [_Cull]

        Pass {
            Tags { 
            "LightMode"="ForwardBase" 
            }
			CGPROGRAM
            #pragma vertex vert alpha
            #pragma fragment frag alpha
            #pragma multi_compile_fwdbase
            #define IS_IN_BASE_PASS
            #include "Passes/Water.cginc"
            ENDCG
        }

        Pass{
            Tags { "LightMode"="ForwardAdd" }
            Blend One One // src*1 + dst*1
			CGPROGRAM
            #pragma vertex vert alpha
            #pragma fragment frag alpha
            #pragma multi_compile_fwdadd
            #include "Passes/Water.cginc"
            ENDCG
        }
        
    }
    //FallBack "Diffuse"
}