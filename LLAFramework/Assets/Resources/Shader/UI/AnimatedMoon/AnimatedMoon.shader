Shader "ReunionMovement/UI/AnimatedMoon"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _iResolution ("Resolution", Vector) = (1, 1, 1, 1)
        _iTime ("Time",  Range(0, 1)) = 0.0
        _MoonColor ("Moon Color", Color) = (1, 1, 1, 1)
        _BackgroundColor ("Background Color", Color) = (0, 0, 0, 0)
        _Size ("Size", Range(0, 1)) = 0.0
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" }
        LOD 100

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha // 添加透明度混合
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            #include "../../Base/2D_SDF.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _iResolution;
            float _iTime;
            float4 _MoonColor;
            float4 _BackgroundColor;
            float _Size;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 fragCoord = i.uv * _iResolution.xy;
                float2 p = (2.0 * fragCoord - _iResolution.xy) / _iResolution.y;
                p *= 1.3;

                float ra = 1.0;
                float rb = _Size;
                float di = 3 * cos(frac(_iTime) * 1.0 * 3.14159);

                float d = sdMoon(p, di, ra, rb);

                float3 col = lerp(_BackgroundColor.rgb, _MoonColor.rgb, step(d, 0.0));

                return float4(col, lerp(_BackgroundColor.a, _MoonColor.a, step(d, 0.0)));
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
