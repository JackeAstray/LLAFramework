Shader "ReunionMovement/InnerOutline"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _InnerOutlineTime ("Time", Float) = 0
        _BackgroundColor ("Background Color", Color) = (0, 0, 0, 1) // 背景颜色
        _TargetColor ("Target Color", Color) = (1, 1, 1, 1)         // 目标颜色
        _ColorIntensity ("Color Intensity", Range(3.9, 4.2)) = 4    // 颜色强度

        _RampColor ("Ramp Color", Color) = (1, 0.2, 1.05, 1)        // 目标颜色
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float _InnerOutlineTime;
            float4 _BackgroundColor; // 背景颜色
            float4 _TargetColor;     // 目标颜色
            float4 _RampColor;       // Ramp颜色
            float _ColorIntensity;   // 颜色强度
            float4 _MainTex_ST;

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            float rand(float2 n)
            {
                return frac(sin(dot(n, float2(12.9898, 12.1414))) * 83758.5453);
            }

            float noise(float2 n)
            {
                const float2 d = float2(0.0, 1.0);
                float2 b = floor(n);
                float2 f = frac(n);
                return lerp(lerp(rand(b), rand(b + d.yx), f.x), lerp(rand(b + d.xy), rand(b + d.yy), f.x), f.y);
            }

            float4 ramp(float t)
            {
                return float4(_RampColor.r, _RampColor.g, _RampColor.b, _RampColor.a) / t;
            }

            float fire(float2 n)
            {
                return noise(n) + noise(n * 2.1) * 0.6 + noise(n * 5.4) * 0.42;
            }

            float4 getLine(float4 bgColor, float2 fc, float2x2 mtx, float shift)
            {
                float t = _Time.y * _InnerOutlineTime;
                float2 uv = mul(mtx, (fc / _ScreenParams.xy));

                uv.x += uv.y < 0.5 ? 23.0 + t * 0.35 : -11.0 + t * 0.3;
                uv.y = abs(uv.y - shift);
                uv *= 5.0;

                float q = fire(uv - t * 0.013) / 2.0;
                float2 r = float2(fire(uv + q / 2.0 + t - uv.x - uv.y), fire(uv + q - t));

                float4 color = float4(_TargetColor.r, _TargetColor.g, _TargetColor.b, _TargetColor.a);

                float grad = pow((r.y + r.y) * max(0.0, uv.y) + 0.1, _ColorIntensity);
                color = ramp(grad);
                color /= (1.5 + max(float4(0, 0, 0, 0), color));

                // 返回颜色和透明度
                return lerp(bgColor, color, color.a);
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 fragCoord = i.uv * _ScreenParams.xy;
                float4 color = _BackgroundColor;
                color = getLine(color, fragCoord, float2x2(1.0, 1.0, 0.0, 1.0), 1.02);
                color = getLine(color, fragCoord, float2x2(1.0, 1.0, 1.0, 0.0), 1.02);
                color = getLine(color, fragCoord, float2x2(1.0, 1.0, 0.0, 1.0), -0.02);
                color = getLine(color, fragCoord, float2x2(1.0, 1.0, 1.0, 0.0), -0.02);
                
                return color;
            }
            ENDCG
        }
    }
}