// Upgrade NOTE: commented out 'float3 _WorldSpaceCameraPos', a built-in variable

Shader "ReunionMovement/GalaxyStar"
{
    Properties
    {
        _StarsTexture("星空贴图", CUBE) = "white" {}
        
        _Color ("Color Tint", Color) = (1, 1, 1, 1)
        // 用于控制折射颜色
        _RefractColor ("Refraction Color", Color) = (1, 1, 1, 1)
        // 用于控制这个材质的折射程度
        _RefractAmount ("Refraction Amount", Range(0, 1)) = 1
        // 用于控制折射率
        _RefractRatio ("Refraction Ratio", Range(0.1, 1)) = 0.5
        // _RimExp("Rim Exp", Range( 0.2 , 10)) = 4
        // _RimExp2("Rim Exp 2", Range( 0.2 , 10)) = 2
        // _RimNoiseRefraction("Rim Noise Refraction", Range( 0 , 1)) = 0

        // _Eta("Eta", Range( -1 , 0)) = -0.1
        // _EtaAAEdgesFix("Eta AA Edges Fix", Range( 0 , 0.5)) = 0
        // _EtaFresnelExp2("Eta Fresnel Exp 2", Range( 1 , 8)) = 1
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
            #pragma multi_compile_fwdbase

            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #include "AutoLight.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal: NORMAL;
            };

            struct v2f
            {
                float4 pos: SV_POSITION;
                float3 worldPos: TEXCOORD0;
                fixed3 worldNormal: TEXCOORD1;
                fixed3 worldViewDir: TEXCOORD2;
                fixed3 worldRefr: TEXCOORD3;
                // float4 vertex : SV_POSITION;
            };

            samplerCUBE _StarsTexture;
            float4 _StarsTexture_ST;

            fixed4 _Color;
            fixed4 _RefractColor;
            float _RefractAmount;
            fixed _RefractRatio;

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.worldViewDir = UnityWorldSpaceViewDir(o.worldPos);
                
                // 计算世界空间的折射方向（归一化后的矢量，归一化后的表面法线，折射率）
                // 根据入射光线方向I，表面法向量N和折射相对系数eta,计算折射向量。如果对给定的eta,I和N之间的角度太大，返回(0,0,0)。
                o.worldRefr = refract(-normalize(o.worldViewDir), normalize(o.worldNormal), _RefractRatio);
                
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed3 worldNormal = normalize(i.worldNormal);
                fixed3 worldLightDir = normalize(UnityWorldSpaceLightDir(i.worldPos));
                fixed3 worldViewDir = normalize(i.worldViewDir);
                
                fixed3 diffuse = _LightColor0.rgb * _Color.rgb * max(0, dot(worldNormal, worldLightDir));
                // 使用世界空间中的折射方向来访问cubemap
                fixed3 refraction = texCUBE(_StarsTexture, i.worldRefr).rgb * _RefractColor.rgb;

                // 混合漫反射的颜色和折射的颜色 lerp：插值
                fixed3 color = lerp(diffuse, refraction, _RefractAmount);

                return fixed4(color, 1.0);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
