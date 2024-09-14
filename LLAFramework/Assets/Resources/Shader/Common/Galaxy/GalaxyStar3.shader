// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "ReunionMovement/GalaxyStar3"
{
    Properties 
    {
        _Cube ("Cube Texture", CUBE) = "" {}
        _Refract ("Refract", Range(0, 1)) = 0.5
        _Brightness ("Brightness", Range(0, 5)) = 1
        _Alpha ("Alpha", Range(0,1)) = 1.0
    }
    SubShader 
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 200

        Blend SrcAlpha OneMinusSrcAlpha

        Pass 
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float3 worldPos : TEXCOORD0;
            };

            samplerCUBE _Cube;
            float _Refract;
            float _Brightness;
            float _Alpha;

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float3 uvw = normalize(i.worldPos - _WorldSpaceCameraPos);
                float3 refracted = refract(uvw, normalize(uvw), _Refract);
                fixed4 texcolor = texCUBE(_Cube, refracted);
                texcolor.rgb *= _Brightness;
                texcolor.a = _Alpha; // 设置透明度
                return texcolor;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}