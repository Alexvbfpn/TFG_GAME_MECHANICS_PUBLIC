Shader "Unlit/UIAnimatedGradientURP"
{
   Properties
    {
        _MainTex ("Texture", 2D) = "white" {} // Se agrega una propiedad para la textura
        _Color1 ("Color 1", Color) = (1, 0, 0, 1)
        _Color2 ("Color 2", Color) = (0, 0, 1, 1)
        _Speed ("Speed", Range(0.1, 10.0)) = 1.0
    }
    SubShader
    {
        Tags { "Queue"="Overlay" "IgnoreProjector"="True" "RenderType"="Transparent" }
        Pass
        {
            Name "Pass1"
            Tags { "LightMode"="UniversalForward" }
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Off
            ZWrite Off

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            CBUFFER_START(UnityPerMaterial)
                float4 _Color1;
                float4 _Color2;
                float _Speed;
            CBUFFER_END

            Varyings vert(Attributes input)
            {
                Varyings output;
                output.positionHCS = TransformObjectToHClip(input.positionOS);
                output.uv = input.uv;
                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                float t = frac(_Time.y * _Speed);
                float2 uv = input.uv;
                float gradientFactor = (uv.x + uv.y) / sqrt(2.0); // Calcula la distancia diagonal
                return lerp(_Color1, _Color2, abs(sin(t * 3.14159))); // Interpolación suave entre colores
            }
            ENDHLSL
        }
    }
    FallBack "UI/Default"


}
