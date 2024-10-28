Shader "ReunionMovement/UI/Squircle"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _ShaderTime ("Shader Time", Range(1,3)) = 0.0
        _Resolution ("Resolution", Vector) = (1,1,1,1)
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
            float _ShaderTime;
            float4 _Resolution;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 fragCoord = i.uv * _Resolution.xy;
                float2 p = (2.0 * fragCoord - _Resolution.xy) / _Resolution.y;
                p *= 1.4;

                float n = 3.0 + 2.5 * sin(6.283185 * _ShaderTime / 3.0);

                float d = (p.y < p.x) ? sdSquircle(p, n) : approx_sdSquircle(p, n);

                float3 col = (d > 0.0) ? float3(0.9, 0.6, 0.3) : float3(0.65, 0.85, 1.0);
                // col *= 1.0 - exp(-8.0 * abs(d));
                // col *= 0.6 + 0.4 * smoothstep(-0.5, 0.5, cos(90.0 * abs(d)));
                // col = lerp(col, float3(1.0, 1.0, 1.0), 1.0 - smoothstep(0.0, 0.015, abs(d)));
                // col *= smoothstep(0.005, 0.010, abs(p.y - p.x));

                return float4(col, 1.0);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
