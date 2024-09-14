Shader "ReunionMovement/PrettyHip2"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _GradientColor1 ("渐变色1", Color) = (1, 1, 1, 1)
        _GradientColor2 ("渐变色2", Color) = (1, 1, 1, 1)
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"
            #include "../../Base/Common.cginc"

            float4 _GradientColor1;
            float4 _GradientColor2;
            
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float aspect =  _ScreenParams.y / _ScreenParams.x;
                // 将UV坐标中心移到(0.5, 0.5)
                float2 uv = i.uv;
                uv -= 0.5;
                // 调整UV坐标
                uv.y *= aspect;
                // 旋转角度，以弧度为单位
                float angle = radians(30 + _Time.y);
                // 创建旋转矩阵
                float2x2 rotationMatrix = float2x2(cos(angle), -sin(angle), sin(angle), cos(angle));  
	            // 旋转UV坐标
                uv = mul(rotationMatrix, uv);
                // 将UV坐标中心移回(0.5, 0.5)
                uv /= aspect;
                uv += 0.5;

                float2 pos = 20.0*uv;
                float2 rep = fract(pos);
                float dist = min(min(rep.x, 1.0-rep.x), min(rep.y, 1.0-rep.y));
                float squareDist = length((floor(pos) + float2(0.5,0.5)) - float2(10.0, 10.0));
                float edge = sin(_Time.y - squareDist * 20.);
                edge = fmod(edge * edge, edge / edge);

                float value = fract(dist*2.0);
                value = mix(value, 1.0-value, step(1.0, edge));
                edge = pow(abs(1.0-edge), 2.2) * 0.5;
                value = smoothstep( edge-0.05, edge, 0.95*value);

                //设置渐变色
                float4 gradientColor1 = _GradientColor1;
                float4 gradientColor2 = _GradientColor2;
                fixed4 col = mix(gradientColor1, gradientColor2, float4(pow(value, 2), pow(value, 1.5), pow(value, 1.2), 1));
   
                return col;
            }
            ENDCG
        }
    }
}
