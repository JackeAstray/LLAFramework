Shader "ReunionMovement/WatchFace"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _LocalTime("Local Time", Vector) = (0,0,0,0)

        [Toggle(ENABLE_SCREEN_ASPECT_RATIO)] _EnableScreenAspectRatio ("启用屏幕纵横比", Float) = 0
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off

            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment frag
            #include "UnityCG.cginc"

            #pragma shader_feature _ ENABLE_SCREEN_ASPECT_RATIO

            float sdLine(float2 p, float2 a, float2 b)
            {
                float2 pa = p - a, ba = b - a;
                float h = clamp(dot(pa, ba) / dot(ba, ba), 0.0, 1.0);
                return length(pa - ba * h);
            }

            float3 hash3(float n)
            {
                return frac(sin(float3(n, n + 1.0, n + 2.0)) * 43758.5453123);
            }

            float3 drawLine(float3 buf, float2 a, float2 b, float2 p, float2 w, float4 col)
            {
                float f = sdLine(p, a, b);
                float g = fwidth(f) * w.y;
                return lerp(buf, col.xyz, col.w * (1.0 - smoothstep(w.x - g, w.x + g, f)));
            }

            sampler2D _MainTex;
            float4 _MainTex_TexelSize;
            float4 _LocalTime;

            float4 frag(v2f_img i) : SV_Target
            {
                float aspect = 1.0;
                // 获取像素坐标
                float2 uv = i.uv;

                //根据屏幕宽高设置UV比率
                #if ENABLE_SCREEN_ASPECT_RATIO
                    aspect = _ScreenParams.y / _ScreenParams.x;
                    uv.x = (uv.x - 0.5) * aspect + 0.5;
                #endif

                float2 fragCoord = uv * _ScreenParams.xy;

                // 归一化到中心
                float2 center = _ScreenParams.xy * 0.5;
                float2 normUV = (fragCoord - center) / min(_ScreenParams.x, _ScreenParams.y) * 2.0;

                float hors = _LocalTime.x;
                float mins = _LocalTime.y;
                float secs = _LocalTime.z;

                float r = length(normUV);
                float a = atan2(normUV.y, normUV.x) + 3.1415926;

                float3 col = float3(1.0, 1.0, 1.0);

                // 表盘外圈
                {
                    float d = r - 0.94;
                    if (d > 0.0) col *= 1.0 - 0.5 / (1.0 + 32.0 * d);
                    col = lerp(col, float3(0.9, 0.9, 0.9), 1.0 - smoothstep(0.0, 0.01, d));
                }

                // 5分钟刻度
                float f = abs(2.0 * frac(0.5 + a * 60.0 / 6.283185) - 1.0);
                float g = 1.0 - smoothstep(0.0, 0.1, abs(2.0 * frac(0.5 + a * 12.0 / 6.283185) - 1.0));
                float w = fwidth(f);
                float ff = 1.0 - smoothstep(0.1 * g + 0.05 - w, 0.1 * g + 0.05 + w, f);
                ff *= smoothstep(0.85, 0.86, r + 0.05 * g) - smoothstep(0.94, 0.95, r);
                col = lerp(col, float3(0.0, 0.0, 0.0), ff);

                // 秒针
                float2 dir = float2(sin(6.283185 * secs / 60.0), cos(6.283185 * secs / 60.0));
                col = drawLine(col, -dir * 0.15, dir * 0.7, normUV + 0.03, float2(0.005, 8.0), float4(0.0, 0.0, 0.0, 0.2));
                col = drawLine(col, -dir * 0.15, dir * 0.7, normUV, float2(0.005, 1.0), float4(0.6, 0.0, 0.0, 1.0));

                // 分针
                dir = float2(sin(6.283185 * mins / 60.0), cos(6.283185 * mins / 60.0));
                col = drawLine(col, -dir * 0.15, dir * 0.7, normUV + 0.03, float2(0.015, 8.0), float4(0.0, 0.0, 0.0, 0.2));
                col = drawLine(col, -dir * 0.15, dir * 0.7, normUV, float2(0.015, 1.0), float4(0.0, 0.0, 0.0, 1.0));

                // 时针
                dir = float2(sin(6.283185 * hors / 12.0), cos(6.283185 * hors / 12.0));
                col = drawLine(col, -dir * 0.15, dir * 0.4, normUV + 0.03, float2(0.015, 8.0), float4(0.0, 0.0, 0.0, 0.2));
                col = drawLine(col, -dir * 0.15, dir * 0.4, normUV, float2(0.015, 1.0), float4(0.0, 0.0, 0.0, 1.0));

                // 中心小圆
                {
                    float d = r - 0.035;
                    if (d > 0.0) col *= 1.0 - 0.5 / (1.0 + 64.0 * d);
                    col = lerp(col, float3(0.9, 0.9, 0.9), 1.0 - smoothstep(0.035, 0.038, r));
                    col = lerp(col, float3(0.0, 0.0, 0.0), 1.0 - smoothstep(0.00, 0.007, abs(r - 0.038)));
                }

                // 抖动
                col += (1.0 / 255.0) * hash3(normUV.x + 13.0 * normUV.y);

                float alpha = 1.0;
                if (r > 0.94) 
                {
                    alpha = 0.0;
                }

                return float4(col, alpha);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
