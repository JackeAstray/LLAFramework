Shader "ReunionMovement/WaveBackground"
{
    Properties
    {
        _ColorTop ("Top Color", Color) = (0.1, 0.6, 0.24, 1)
        _ColorBottom ("Bottom Color", Color) = (0.3, 0.95, 0.5, 1)
        _WaveColorTop ("Wave Top Color", Color) = (0.45, 1.0, 0.5, 1)
        _WaveColorBottom ("Wave Bottom Color", Color) = (0.3, 0.9, 0.3, 1)
        _TimeSpeed ("Time Speed", Float) = 1.0
        _WaveCount ("Wave Count", Range(1,5)) = 2

         // ===== 波浪参数-1 =====
        _Wave1Amplitude ("Wave1 Amplitude", Float) = 0.04
        _Wave1Frequency ("Wave1 Frequency", Float) = 4.0
        _Wave1Speed ("Wave1 Speed", Float) = 0.0003
        _Wave1Phase ("Wave1 Phase", Float) = 0.0
         // ===== 波浪参数-2 =====
        _Wave2Amplitude ("Wave2 Amplitude", Float) = 0.05
        _Wave2Frequency ("Wave2 Frequency", Float) = 4.2
        _Wave2Speed ("Wave2 Speed", Float) = 0.00032
        _Wave2Phase ("Wave2 Phase", Float) = 1.0
         // ===== 波浪参数-3 =====
        _Wave3Amplitude ("Wave3 Amplitude", Float) = 0.03
        _Wave3Frequency ("Wave3 Frequency", Float) = 3.8
        _Wave3Speed ("Wave3 Speed", Float) = 0.00028
        _Wave3Phase ("Wave3 Phase", Float) = 2.0
         // ===== 波浪参数-4 =====
        _Wave4Amplitude ("Wave4 Amplitude", Float) = 0.06
        _Wave4Frequency ("Wave4 Frequency", Float) = 4.5
        _Wave4Speed ("Wave4 Speed", Float) = 0.00035
        _Wave4Phase ("Wave4 Phase", Float) = 3.0
         // ===== 波浪参数-5 =====
        _Wave5Amplitude ("Wave5 Amplitude", Float) = 0.02
        _Wave5Frequency ("Wave5 Frequency", Float) = 3.5
        _Wave5Speed ("Wave5 Speed", Float) = 0.00025
        _Wave5Phase ("Wave5 Phase", Float) = 4.0

        _BaseY ("Base Y", Float) = 0.35
        _Spacing ("Spacing", Float) = 0.03
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Transparent" }
        LOD 100

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

            fixed4 _ColorTop;
            fixed4 _ColorBottom;
            fixed4 _WaveColorTop;
            fixed4 _WaveColorBottom;
            float _TimeSpeed;
            float _WaveCount;

            float _Wave1Amplitude, _Wave1Frequency, _Wave1Speed, _Wave1Phase;
            float _Wave2Amplitude, _Wave2Frequency, _Wave2Speed, _Wave2Phase;
            float _Wave3Amplitude, _Wave3Frequency, _Wave3Speed, _Wave3Phase;
            float _Wave4Amplitude, _Wave4Frequency, _Wave4Speed, _Wave4Phase;
            float _Wave5Amplitude, _Wave5Frequency, _Wave5Speed, _Wave5Phase;

            float _BaseY;
            float _Spacing;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 uv = i.uv;
                float t = _Time.y * _TimeSpeed;

                // 背景渐变
                float3 color = lerp(_ColorBottom.rgb, _ColorTop.rgb, uv.y);

                // 波浪
                for (float idx = 0.0; idx < _WaveCount; idx += 1.0)
                {
                    float amplitude, frequency, speed, phase;
                    if (idx == 0) {
                        amplitude = _Wave1Amplitude;
                        frequency = _Wave1Frequency;
                        speed = _Wave1Speed;
                        phase = _Wave1Phase;
                    } else if (idx == 1) {
                        amplitude = _Wave2Amplitude;
                        frequency = _Wave2Frequency;
                        speed = _Wave2Speed;
                        phase = _Wave2Phase;
                    } else if (idx == 2) {
                        amplitude = _Wave3Amplitude;
                        frequency = _Wave3Frequency;
                        speed = _Wave3Speed;
                        phase = _Wave3Phase;
                    } else if (idx == 3) {
                        amplitude = _Wave4Amplitude;
                        frequency = _Wave4Frequency;
                        speed = _Wave4Speed;
                        phase = _Wave4Phase;
                    } else if (idx == 4) {
                        amplitude = _Wave5Amplitude;
                        frequency = _Wave5Frequency;
                        speed = _Wave5Speed;
                        phase = _Wave5Phase;
                    }

                    float offset = _Spacing * (idx - 0.5 * (_WaveCount - 1.0));
                    float wave = sin((uv.x - t * speed) * frequency + phase) * amplitude;
                    float waveY = _BaseY + offset + wave;

                    float mask = smoothstep(waveY + 0.001, waveY - 0.001, uv.y);
                    float gradFactor = clamp((waveY - uv.y) / waveY, 0.0, 1.0);
                    float3 waveColor = lerp(_WaveColorTop.rgb, _WaveColorBottom.rgb, gradFactor);
                    color = lerp(color, waveColor, 0.65 * mask);

                    // 高光线
                    float lightY = waveY + 0.001;
                    float lightW = 0.0025;
                    float lightMask = smoothstep(lightY - lightW, lightY, uv.y)
                                    * (1.0 - smoothstep(lightY, lightY + lightW, uv.y));
                    color = lerp(color, float3(1.0,1.0,1.0), lightMask * 0.08);
                }

                // 径向光
                float2 lightCenter = float2(1.0, 0.85);
                float radius = 0.75;
                float intensity = 0.2;
                float dist = distance(uv, lightCenter);
                float glow = 1.0 - smoothstep(0.0, radius, dist);
                glow = pow(glow, 1.5) * intensity;
                color = lerp(color, float3(1.0,1.0,1.0), glow);

                return float4(color, 1.0);
            }
            ENDCG
        }
    }
    FallBack "Unlit/Color"
}