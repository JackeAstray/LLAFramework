Shader "ReunionMovement/GalaxyStar2"
{
    Properties 
    {
        _MainTex ("Texture", 2D) = "white" {}
        _GradientColor1 ("渐变色1", Color) = (1, 1, 1, 1)
        _GradientColor2 ("渐变色2", Color) = (1, 1, 1, 1)
        _SPEED ("速度", Range(0, 1)) = 0.1
        _STAR_NUMBER ("动态星星数量", Range(1, 500)) = 300
        _ITER ("热量", Range(1, 100)) = 20
    }
    SubShader 
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 100

        Pass 
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #include "../../Base/Common.cginc"
            
            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            float4 _GradientColor1;// 酷炫的星星颜色
            float4 _GradientColor2;// 最热的星星颜色

            float _SPEED;
            float _STAR_NUMBER;
            float _ITER;
            
            //随机
            float rand(float i)
            {
                return frac(sin(dot(float2(i, i) ,float2(32.9898,78.233))) * 43758.5453);
            }

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 uv = i.uv;
                float res = _ScreenParams.y / _ScreenParams.x;
                if (_ScreenParams.x > _ScreenParams.y)
                {
                    res = _ScreenParams.x / _ScreenParams.y;
                }
                float4 fragColor = (0,0,0,0);

                //静态星星
                float4 sStar = float4(rand(uv.x * uv.y),rand(uv.x * uv.y),rand(uv.x * uv.y),rand(uv.x * uv.y));
                sStar *= pow(rand(uv.x * uv.y), 200.);
                sStar.xyz *= lerp(_GradientColor1, _GradientColor2, rand(uv.x + uv.y));
                fragColor += sStar;

                //银河
                float len = length(float2(uv.x, 0.5) - uv);
                float4 col = 0.5 - float4(len, len, len, len);
                col.xyz *= lerp(_GradientColor1, _GradientColor2, 0.75);
                fragColor += col * 2.;
                float c = 0.;
                float c2 = 0.;
                float2 rv = uv;
                rv.x -= _Time.y * _SPEED * 0.25;
                for(int i=0;i<_ITER;i++)
                {
                    c += (tex2D(_MainTex, rv * 0.25 + rand(float(i + 10) + uv.x * uv.y) * (16. / _ScreenParams.y)) / float(_ITER)).x;
                }
                fragColor -= c * 0.5;
                fragColor = clamp(fragColor, 0.0, 1.0);

                //动态星星    
                for (int i = 0; i <= _STAR_NUMBER; i++)
                {
                    float n = float(i);
                    //设置星星的位置
                    float x = rand(n) * res + (_Time.y + 100.0) * _SPEED;
                    float3 pos = float3(x, rand(n + 1.) , rand(n + 2.));
                    //视差效果
                    pos.x = fmod(pos.x * pos.z, res);
                    pos.y = (pos.y + rand(n + 10.)) * 0.5;
                    //绘制星星
                    float powlength = pow(length(pos.xy - uv), -2.) * 0.0001 * pos.z * rand(n + 3.);
                    float4 col2 = float4(powlength, powlength, powlength, powlength);
                    //给星星上色
                    col2.xyz *= lerp(_GradientColor1, _GradientColor2, rand(n + 4.));
                    //星星闪烁
                    col2.xyz *= lerp(rand(n + 5.), 1.0, abs(cos(_Time.y * rand(n + 6.) * 5.)));
                    fragColor += col2;
                }
                return fragColor;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}