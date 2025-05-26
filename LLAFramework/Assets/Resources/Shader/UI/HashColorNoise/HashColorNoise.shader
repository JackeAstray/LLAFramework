Shader "ReunionMovement/HashColorNoise"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        Pass
        {
            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment frag
            #include "UnityCG.cginc"

            // Hugo Elias integer hash, adapted for HLSL
            float hash11(uint n)
            {
                n = (n << 13) ^ n;
                n = n * (n * n * 15731u + 789221u) + 1376312589u;
                return float(n & 0x7fffffffu) / float(0x7fffffffu);
            }

            float3 hash13(uint n)
            {
                n = (n << 13) ^ n;
                n = n * (n * n * 15731u + 789221u) + 1376312589u;
                uint3 k = n * uint3(n, n * 16807u, n * 48271u);
                return float3(k & uint3(0x7fffffffu, 0x7fffffffu, 0x7fffffffu)) / float(0x7fffffffu);
            }

            sampler2D _MainTex;
            float4 _MainTex_TexelSize;

            float4 frag(v2f_img i) : SV_Target
            {
                // 获取像素坐标
                float2 uv = i.uv;
                float2 fragCoord = uv * _ScreenParams.xy;

                // 计算帧数（可用时间代替）
                uint iFrame = (uint)(_Time.y * 60.0);

                uint px = (uint)fragCoord.x;
                uint py = (uint)fragCoord.y;
                uint n = px + (uint)_ScreenParams.x * py + ((uint)_ScreenParams.x * (uint)_ScreenParams.y) * iFrame;

                float3 c = hash13(n);

                return float4(c, 1.0);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
