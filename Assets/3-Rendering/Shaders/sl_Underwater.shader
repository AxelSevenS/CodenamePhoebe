// Shader "Selene/Underwater" {
//     Properties {
        
//         _MainTex ("Main Texture", 2D) = "white" {}
        
//     }
//     SubShader {

//         Tags { "RenderType"="Opaque" }
//         LOD 100
//         Stencil {
//             Ref 1
//             Comp Equal
//         }

//         HLSLINCLUDE

//             sampler2D _MainTex;

//             struct VertexInput {
//                 float2 uv : TEXCOORD0;
//             };

//             struct VertexOutput{
//                 float2 uv : TEXCOORD0;
//             };

//         ENDHLSL
        
//         Pass {

//             Name "Effect"

//             HLSLPROGRAM
//                 #pragma vertex vert
//                 #pragma fragment frag

//                 VertexOutput vert(VertexInput input) {
//                     VertexOutput output;

//                     output.uv = input.uv;

//                     return output;
//                 }


//                 float4 frag(VertexOutput input) : SV_Target {
//                     half4 baseColor = tex2D(_MainTex, input.uv);

//                     return /* baseColor +  */half4(0, 0, 0.25, 0);
//                 }

//             ENDHLSL
//         }

//     }
// }
