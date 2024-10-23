Shader "ReunionMovement/UI/AnimatedPie"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _CustomTime ("Custom Time", Range(0, 1)) = 0.0
        _Distance ("Distance", Range(0, 1)) = 0.0
        _Color1 ("Color 1", Color) = (0.9, 0.6, 0.3, 1.0)
        _Color2 ("Color 2", Color) = (0.65, 0.85, 1.0, 1.0)
        _Rotation ("Rotation", Range(0, 1)) = 0.0 // 新增属性
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Overlay" }
        LOD 200

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            Cull Off

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
            float _CustomTime;
            float4 _MainTex_ST;
            float _Distance;
            fixed4 _Color1;
            fixed4 _Color2;
            float _Rotation; // 新增变量

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 fragCoord = i.uv * _ScreenParams.xy;
                float2 iResolution = _ScreenParams.xy;

                // 计算宽高比
                float aspectRatio = iResolution.x / iResolution.y;

                if (iResolution.x < iResolution.y)
                {
                    aspectRatio = iResolution.y / iResolution.x;
                }

                // 归一化像素坐标并保持正比例
                float2 p = (fragCoord * 2.0 - iResolution) / iResolution.y;
                p.x /= aspectRatio;

                // 动画
                float t = 3.14 * _CustomTime;
                float2 w = float2(0.50, 0.25) * (0.5 + 0.5 * cos(_CustomTime * float2(1.1, 1.3) + float2(0.0, 2.0)));

                // 旋转角度
                float angle = _Rotation * 2.0 * 3.14159265359; // 将0-1的值转换为0-2π的角度
                float2x2 rotationMatrix = float2x2(cos(angle), -sin(angle), sin(angle), cos(angle));
                p = mul(rotationMatrix, p);

                // 距离
                float d = sdPie(p, float2(sin(t), cos(t)), _Distance);

                // 采样纹理
                fixed4 texColor = tex2D(_MainTex, i.uv);

                // 着色
                float3 col = (d > 0.0) ? texColor.rgb * _Color1.rgb : texColor.rgb * _Color2.rgb;

                // 返回带有透明度的颜色
                return fixed4(col, texColor.a * (d > 0.0 ? _Color1.a : _Color2.a));
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
