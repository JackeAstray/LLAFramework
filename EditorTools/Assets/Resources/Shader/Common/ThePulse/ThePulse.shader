Shader "ReunionMovement/ThePulse"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
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
            float4 _MainTex_ST;

            // fixed3 hsv(float h, float s, float v)
            // {
            //     float3 hsv = float3(h, s, v);
            //     float3 p = abs(frac(hsv.xxx + float3(1.0, 2.0 / 3.0, 1.0 / 3.0)) * 6.0 - 3.0);
            //     return v * lerp(float3(1.0, 1.0, 1.0), clamp(p - 1.0, 0.0, 1.0), s);
            // }

            // float circle(float2 p, float r)
            // {
            //     return smoothstep(0.1, 0.0, abs(length(p) - r));
            // }

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float pi = 3.1415;

                float radius = 0.5;
                float lineWidth = 5.0; // in pixels
                float glowSize = 5.0; // in pixels
                
                float pixelSize = 1.0/min(_ScreenParams.x, _ScreenParams.y);
                lineWidth *= pixelSize;
                glowSize *= pixelSize;
                glowSize *= 2.0;
                
                float2 uv = (i.uv.xy / _ScreenParams.xy)-0.5;
                uv.x *= _ScreenParams.x/_ScreenParams.y;
                
                float len = length(uv);
                float angle = atan2(uv.y, uv.x);
                
                float fallOff = frac(-0.5*(angle/pi)-_Time.y*0.5);
                
                lineWidth = (lineWidth-pixelSize)*0.5*fallOff;
                float color = smoothstep(pixelSize, 0.0, abs(radius - len) - lineWidth)*fallOff;
                color += smoothstep(glowSize*fallOff, 0.0, abs(radius - len) - lineWidth)*fallOff*0.5;    
                
                fixed4 fragColor = float4(color, color, color, color);

                return fragColor;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
