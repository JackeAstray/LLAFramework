Shader "ReunionMovement/GalaxyStar"
{
    Properties
    {
        _StarsTexture("星空贴图", CUBE) = "white" {}
        _RotationStars("旋转星空", Float) = 0
        _StarsEmissionPower("星空发射功率", Float) = 4
		_StarsRotationSpeed("星空旋转速度", Float) = 0.1
        _EtaAAEdgesFix("Eta AA边缘修复", Range( 0 , 0.5)) = 0
    }
    SubShader
    {
        Tags { "RenderPipeline"="UniversalPipeline" "RenderType"="Opaque" "Queue"="Background" }
        LOD 0

        HLSLINCLUDE
        #pragma target 3.0
        sampler2D _StarsTexture;

        float3 RotateAroundAxis( float3 center, float3 original, float3 u, float angle )
        {
            original -= center;
            float C = cos( angle );
            float S = sin( angle );
            float t = 1 - C;
            float m00 = t * u.x * u.x + C;
            float m01 = t * u.x * u.y - S * u.z;
            float m02 = t * u.x * u.z + S * u.y;
            float m10 = t * u.x * u.y + S * u.z;
            float m11 = t * u.y * u.y + C;
            float m12 = t * u.y * u.z - S * u.x;
            float m20 = t * u.x * u.z - S * u.y;
            float m21 = t * u.y * u.z + S * u.x;
            float m22 = t * u.z * u.z + C;
            float3x3 finalMatrix = float3x3( m00, m01, m02, m10, m11, m12, m20, m21, m22 );
            return mul( finalMatrix, original ) + center;
        }
        ENDHLSL

        Pass
        {
			Name "DepthOnly"
			Tags { "LightMode"="DepthOnly" }

			ZWrite On
			ColorMask 0
			AlphaToMask Off
            HLSLPROGRAM
            #pragma vertex vert
			#pragma fragment frag

            float _StarsEmissionPower;
            float _StarsRotationSpeed;
            float _RotationStars;

            float _Eta;
            float _EtaAAEdgesFix;

            
            float temp_output_645_0 = ( 1.0 + ( _Eta * clampResult881 ) + ( temp_output_78_0 * -_RimNoiseRefraction ) );
            float Eta797 = temp_output_645_0;
            float In0797 = _EtaAAEdgesFix;
            float3 rotatedValue646 = RotateAroundAxis( float3( 0,0,0 ), -ase_worldViewDir, normalize( _RotationAxis ), temp_output_1023_0 );
            float3 V797 = rotatedValue646;
            float3 localRefractFixed797 = RefractFixed797( V797 , N797 , Eta797 , In0797 );
            float4 texCUBENode640 = texCUBE( _StarsTexture, localRefractFixed797 );

            ENDHLSL
        }
    }
    FallBack "Diffuse"
}
