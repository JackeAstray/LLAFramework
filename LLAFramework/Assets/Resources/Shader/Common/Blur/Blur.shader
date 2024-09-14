Shader "ReunionMovement/Blur"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _BlurAmount ("Blur Amount", Range(0, 20)) = 1
        _BlurType ("Blur Type", Range(0, 1)) = 0
    }
    SubShader
    {
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

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
            float _BlurAmount;
            int _BlurType;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col;
                if (_BlurType == 0)
                {
                    // Box blur
                    col = tex2D(_MainTex, i.uv);
                    for (int x = -1; x <= 1; x++)
                    {
                        for (int y = -1; y <= 1; y++)
                        {
                            col += tex2D(_MainTex, i.uv + float2(x, y) * _BlurAmount * 0.001);
                        }
                    }
                    col /= 9.0;
                }
                else if (_BlurType == 1)
                {
                    // Gaussian blur
                    // This is a simplified version, you may want to use a more accurate Gaussian function
                    col = tex2D(_MainTex, i.uv) * 0.4421;
                    col += tex2D(_MainTex, i.uv + float2(0, 1) * _BlurAmount * 0.001) * 0.1659;
                    col += tex2D(_MainTex, i.uv + float2(0, -1) * _BlurAmount * 0.001) * 0.1659;
                    col += tex2D(_MainTex, i.uv + float2(1, 0) * _BlurAmount * 0.001) * 0.1659;
                    col += tex2D(_MainTex, i.uv + float2(-1, 0) * _BlurAmount * 0.001) * 0.1659;
                }
                return col;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
