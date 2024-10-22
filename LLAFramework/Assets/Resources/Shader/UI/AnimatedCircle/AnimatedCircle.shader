Shader "ReunionMovement/UI/AnimatedCircle"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _CustomTime ("Custom Time", Float) = 0.0
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Overlay" }
        LOD 100

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
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
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float _CustomTime;
            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            float sdRing(float2 p, float2 n, float r, float th)
            {
                p.x = abs(p.x);
                p = mul(float2x2(n.x, n.y, -n.y, n.x), p);
                return max(abs(length(p) - r) - th * 0.5, length(float2(p.x, max(0.0, abs(r - p.y) - th * 0.5))) * sign(p.x));
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 p = (2.0 * i.uv - 1.0) * float2(_ScreenParams.x / _ScreenParams.y, 1.0);
                float2 m = (2.0 * float2(_CustomTime, _CustomTime) - 1.0) * float2(_ScreenParams.x / _ScreenParams.y, 1.0);

                float t = 3.14159 * (0.5 + 0.5 * cos(_CustomTime * 0.52));
                float2 cs = float2(cos(t), sin(t));
                const float ra = 0.5;
                const float th = 0.2;

                float d = sdRing(p, cs, ra, th);

                float3 col = (d > 0.0) ? float3(0.9, 0.6, 0.3) : float3(0.65, 0.85, 1.0);
                col *= 1.0 - exp2(-25.0 * abs(d));
                col *= 0.8 + 0.2 * cos(120.0 * abs(d));
                col = lerp(col, float3(1.0, 1.0, 1.0), 1.0 - smoothstep(0.0, 0.01, abs(d)));

                if (_CustomTime > 0.001)
                {
                    d = sdRing(m, cs, ra, th);
                    col = lerp(col, float3(1.0, 1.0, 0.0), 1.0 - smoothstep(0.0, 0.005, abs(length(p - m) - abs(d)) - 0.0025));
                    col = lerp(col, float3(1.0, 1.0, 0.0), 1.0 - smoothstep(0.0, 0.005, length(p - m) - 0.015));
                }

                return float4(col, 1.0);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}