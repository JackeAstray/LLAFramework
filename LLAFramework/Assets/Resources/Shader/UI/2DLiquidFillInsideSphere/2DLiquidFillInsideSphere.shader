Shader "ReunionMovement/UI/2DLiquidFillInsideSphere"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Resolution ("Resolution", Vector) = (1, 1, 1, 1)
        _Color ("Color", Color) = (1, 1, 1, 1) // 自定义颜色
        _EdgeColor ("Edge Color", Color) = (1, 1, 1, 1) // 自定义边缘颜色
        _FillHeight ("Fill Height", Range(0, 1)) = 0.5 // 自定义液体高度
        _LiquidBackgroundColor ("Liquid Background Color Range", Range(0, 1)) = 0.5 // 自定义液体背景
        _LiquidEdgeColor ("Liquid Edge Color Range", Range(0, 1)) = 0.5 // 自定义液体边缘颜色
        [Enum(Circle, 0, Square, 1, Triangle, 2)]
        _Shape ("Shape", Int) = 0 // 形状选择：0-圆形，1-方形，2-三角形
        _WaveFrequency ("Wave Frequency", Range(0.1, 10.0)) = 1.0 // 波浪频率
        _WaveAmplitude ("Wave Amplitude", Range(0.0, 1.0)) = 0.05 // 波浪幅度
        _WaveCount ("Wave Count", Range(1, 10)) = 3 // 波浪数量
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" }
        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            #define PI 3.14
            #define step(b, a) smoothstep(a, a - (fwidth(b) * 2.0), b)

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
            float4 _Resolution;
            float4 _Color; // 自定义颜色
            float4 _EdgeColor; // 自定义边缘颜色
            float _FillHeight; // 自定义液体高度
            float _LiquidBackgroundColor; // 自定义液体背景
            float _LiquidEdgeColor; // 自定义液体边缘颜色
            int _Shape; // 形状选择
            float _WaveFrequency; // 波浪频率
            float _WaveAmplitude; // 波浪幅度
            int _WaveCount; // 波浪数量

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float shapeSDF(float2 uv, int shape)
            {
                if (shape == 1) // 方形
                {
                    float2 d = abs(uv) - 1;
                    return length(max(d, 0.0)) + min(max(d.x, d.y), 0.0);
                }
                else if (shape == 2) // 三角形
                {
                    // 等边三角形的SDF
                    float k = sqrt(3.0); // 三角形的高度比例
                    uv.y -= -1.5 / k;     // 将三角形的中心向下移动，使底边位于 y = -1
                    uv.x = abs(uv.x) - 1.0;
                    if (uv.x + k * uv.y > 0.0) uv = float2(uv.x - k * uv.y, -k * uv.x - uv.y) / 2.0;
                    uv.x -= clamp(uv.x, -2.0, 0.0);
                    return -length(uv) * sign(uv.y);
                }
                else // 圆形
                {
                    return length(uv) - 1;
                }
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 uv = i.uv;
                float3 r = _Resolution.xyz;
                uv = ((2.0 * uv) - r.xy) / r.y;

                float sdf = shapeSDF(uv, _Shape);
                float c = step(sdf, 0.0);

                float vB = smoothstep(0.1, 0.9, sin(uv.x + (PI * 0.5)) - 0.3);
                float vBA = vB * sin(_Time.y * 4.0 * _WaveFrequency) * 0.1;

                float waveFactor = _WaveCount * 2.0; // 增加波浪数量的因子
                float fW = (sin(((_Time.y * waveFactor * _WaveFrequency) + uv.x) * waveFactor) * _WaveAmplitude) + vBA;
                float bW = (sin(((_Time.y * -waveFactor * _WaveFrequency) + uv.x) * waveFactor + PI) * _WaveAmplitude) - vBA;

                float fA = (sin(_Time.y * 4.0 * _WaveFrequency) * _WaveAmplitude) * vB;

                // 使用 _FillHeight 属性来控制液体高度
                float fF = step(uv.y, (fA + fW) + (_FillHeight * 2.0 - 1.0)) * c;
                float bF = step(uv.y, (-fA + bW) + (_FillHeight * 2.0 - 1.0)) * c;

                fixed4 col = _Color * (
                    (step(sdf, 1.0) - step(sdf, 1)) +
                    (fF + (clamp(bF - fF, _LiquidBackgroundColor, 1.0) * 0.8)) +
                    clamp(pow((sdf + 0.01) * ((1.0 - (fF + bF)) * c), 7.0), 1.0, _LiquidEdgeColor)
                );

                // 添加自定义颜色的边缘
                fixed4 edgeColor = _EdgeColor;
                // 边缘宽度，可以根据需要调整
                float edgeWidth = 0.05; 
                float edgeFactor = smoothstep(0.0 - edgeWidth, 0.0, sdf);
                col = lerp(col, edgeColor, edgeFactor);

                return col;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
