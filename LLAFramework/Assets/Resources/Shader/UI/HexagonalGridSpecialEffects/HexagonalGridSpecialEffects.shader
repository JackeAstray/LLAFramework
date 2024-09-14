Shader "ReunionMovement/HexagonalGridSpecialEffects"
{
    Properties
    {
        _MainTex ("贴图", 2D) = "white"{}
        _GradientColor1 ("渐变色1", Color) = (1, 1, 1, 1)
        _GradientColor2 ("渐变色2", Color) = (1, 1, 1, 1)
        [MainColor] _BaseColor("基础颜色", Color) = (1, 1, 1, 1)
        _Ratio("缩放比率", Range(1, 5)) = 2

        _NoiseDistance("噪波距离", Range(1, 10)) = 5
        _NoiseDistanceRatio("噪声距离比", Range(0, 1)) = 0.335

        [Toggle(ENABLE_SCREEN_ASPECT_RATIO)] _EnableScreenAspectRatio ("启用屏幕纵横比", Float) = 0
    }

    CGINCLUDE
    #include "UnityCG.cginc"
    #include "../../Base/Common.cginc"

    #pragma shader_feature _ ENABLE_SCREEN_ASPECT_RATIO
    
    float4 _GradientColor1;
    float4 _GradientColor2;
    float4 _BaseColor;
    float _Ratio;
    float _Value;
    float _NoiseDistance;
    float _NoiseDistanceRatio;

    //随机数
    float2 random2(float2 st)
    {
        st = float2(dot(st, float2(127.1, 311.7)),
                    dot(st, float2(269.5, 183.3)));
        
        return -1.0 + 2.0 * frac(sin(st) * 43758.5453123);
    }

    //噪波
    float cell_noise(float2 st)
    {
        st *= 3;

        float2 ist = floor(st);
        float2 fst = frac(st);

        float distance = _NoiseDistance;

        for (int y = -1; y <= 1; y++)
        for (int x = -1; x <= 1; x++)
        {
            float2 neighbor = float2(x, y);
            float2 p = 0.5 + 0.5 * sin(_Time.y + 6.2831 * random2(ist + neighbor));

            float2 diff = neighbor + p - fst;
            distance = min(distance, length(diff));
        }
        return distance * _NoiseDistanceRatio;
    }

    //模
    float2 mod(float2 a, float2 b) 
    { 
        return a - b * floor(a / b);
    }

    //六边形
    float hex(float2 st, float2 r)
    {
        st = abs(st);
        return max(st.x - r.y, max((st.x + st.y * 0.6), (st.y * 1.2) - r.x));
    }

    //六边形网格
    float hex_grid(float2 st)
    {
        st.x += 0.1;
        
        float2 g = float2(0.7, 0.4) * 0.4;
        float r = 0.00005;
        
        float2 p1 = mod(st, g) - g * 0.5;
        float2 p2 = mod(st + g * 0.5, g) - g * 0.5;
        
        return min(hex(p1, r), hex(p2, r));
    }

    //波浪
    float wave(float2 st)
    {
        float pos = st.y + st.x;

        float t = (sin(_Time.y) + 2.5) * 0.5;
        float temp = lerp(0, 0.8, t);

        float value = (1.8 + sin(0 * 1 + pos * 1)) * temp;

        return value * cell_noise(st);
    }

    //片元函数
    float4 frag(v2f_img i) : SV_Target
    {
        float4 gradientColor1 = _GradientColor1;
        float4 gradientColor2 = _GradientColor2;
        float4 baseColor = _BaseColor;
        float ratio = _Ratio;

        //根据屏幕宽高设置UV比率
        #if ENABLE_SCREEN_ASPECT_RATIO
        i.uv = screen_aspect_ratio(i.uv, ratio);
        #endif

        float4 color = baseColor;

        float w =  wave(i.uv);

        i.uv.xy += _Time.x;

        float h1 = abs(0.4 + sin(hex_grid(i.uv) * 40) * w);
        h1 = step(h1, 0.1);
        color = lerp(color, gradientColor1, h1);

        float h2 = abs(0.3 + sin(hex_grid(i.uv) * 40) * w);
        h2 = step(h2, 0.1);
        color = lerp(color, gradientColor2, h2);

        return color;
    }
    ENDCG

    SubShader
    {
        Cull Off
        Lighting Off
        ZWrite Off
        ZTest Always
        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask RGB
    
        Pass
        {
            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment frag
            ENDCG
        }
    }
}