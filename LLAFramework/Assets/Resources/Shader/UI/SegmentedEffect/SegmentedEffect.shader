Shader "Custom/SegmentedEffect"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _CustomTime ("Custom Time", Float) = 0.0
        _Alpha ("Alpha", Range(0, 1)) = 1.0 // 新增透明度属性

        _DashColor ("Dash Color", Color) = (1, 1, 1, 1) // 新增虚线颜色属性

        _LineWidth ("Line Width", Float) = 1.0 // 新增线宽属性
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" } // 修改为透明渲染类型
        Blend SrcAlpha OneMinusSrcAlpha     // 启用混合模式
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

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

            sampler2D _MainTex;
            float _CustomTime;
            float _Alpha; // 新增透明度变量
            float4 _DashColor; // 新增虚线颜色变量
            float _LineWidth; // 新增线宽变量

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float4 frag (v2f i) : SV_Target
            {
                float2 uv = i.uv;

                // 定义形状的 SDF（替换为其他形状）
                float2 center = float2(0.5, 0.5); // 图形中心
                float radius = 0.48;              // 半径（仅对圆形有效）
                // 替换以下 SDF 为其他形状
                float sdf = length(uv - center) - radius;
            
                // 限制虚线效果仅在边界范围内
                if (abs(sdf) > _LineWidth * 0.5)
                {
                    return float4(0, 0, 0, 0); // 超出边界范围，返回透明
                }
            
                // 计算角度（绕图形边界生成虚线）
                float angle = atan2(uv.y - center.y, uv.x - center.x); // 计算当前点的角度
                float normalizedAngle = (angle + 3.1415926) / (2.0 * 3.1415926); // 归一化到 [0, 1]
            
                // 虚线效果
                float dash = step(0.5, frac(normalizedAngle * 10.0 + _CustomTime)); // 基于角度生成虚线
            
                // 结合边界和虚线
                float col = dash;
            
                // 应用透明度
                return float4(_DashColor.rgb * col, col * _Alpha * _DashColor.a);
            }
            ENDCG
        }
    }
}