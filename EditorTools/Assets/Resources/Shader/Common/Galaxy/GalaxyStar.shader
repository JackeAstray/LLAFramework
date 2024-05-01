// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "ReunionMovement/GalaxyStar"
{
    Properties 
    {
        _Cube ("Cube Texture", CUBE) = "" {}
        _Refract ("Refract", Range(0, 1)) = 0.5
        _Brightness ("Brightness", Range(0, 5)) = 1
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
                return texcolor;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}