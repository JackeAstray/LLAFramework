Shader "ReunionMovement/GalaxyStar"
{
   Properties {
        _Color ("Color", Color) = (1, 1, 1, 1)
        _RefractColor ("Refract Color", Color) = (1, 1, 1, 1)
        _RefractAmount ("Refract Amount", range(0, 1)) = 1
        _RefractRatio ("Refract Rastio", range(0.1, 1)) = 0.5
        _Cubemap ("Refract Cubemap", Cube) = "_Skybox" {}
    }
 
    SubShader {
        Pass {
            Tags { "lightMode"="ForwardBase" }
 
            CGPROGRAM
 
            #pragma multi_compile_fwdbase
 
            #pragma vertex vert
            #pragma fragment frag
            #include "Lighting.cginc"
            #include "AutoLight.cginc"
 
            //Properties
            fixed4 _Color;
            fixed4 _RefractColor;
            fixed _RefractAmount;
            fixed _RefractRatio;
            samplerCUBE _Cubemap;
            
            struct a2v {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };
 
            struct v2f {
                float4 pos : SV_POSITION;
                float3 worldPos : TEXCOORD0;
                fixed3 worldNormal : TEXCOORD1;
                fixed3 worldViewDir : TEXCOORD2;
                fixed3 worldLightDir : TEXCOORD3;
                fixed3 worldRefract : TEXCOORD5;
                SHADOW_COORDS(4)
            };
 
            v2f vert(a2v v) {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.worldNormal = UnityObjectToWorldNormal(v.normal).xyz;
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.worldViewDir = UnityWorldSpaceViewDir(o.worldPos).xyz;
                o.worldLightDir = UnityWorldSpaceLightDir(o.worldPos).xyz;
                o.worldRefract = refract(-normalize(o.worldViewDir), normalize(o.worldNormal), _RefractRatio);
 
                TRANSFER_SHADOW(o);
 
                return o;
            }
 
            fixed4 frag(v2f i) : SV_Target {
                fixed3 worldNormal = normalize(i.worldNormal);
                fixed3 worldViewDir = normalize(i.worldViewDir);
                fixed3 worldLightDir = normalize(i.worldLightDir);
 
                fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz;
                fixed3 diffuse = _LightColor0.rgb * _Color.rgb * saturate(dot(worldNormal, worldLightDir));
                //用折射采样cubemap
                fixed3 refract = texCUBE(_Cubemap, i.worldRefract).rgb * _RefractColor.rgb;
 
                // UNITY_LIGHT_ATTENUATION(atten, i, i.worldPos);
 
                // fixed3 color = ambient + lerp(diffuse, refract, _RefractAmount) * atten;
                fixed3 color = lerp(diffuse, refract, _RefractAmount);
                return fixed4(color, 1.0);
 
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}