Shader "Custom/SegmentedEffect"
{
    Properties
    {
        _SegmentCount("Segment Count", Float) = 5
        _SegmentSpacing("Segment Spacing", Float) = 0.04
        _RemoveSegments("Remove Segments", Float) = 1
        _Rotation("Rotation", Float) = 0
        _Radius("Radius", Float) = 0.35
        _LineWidth("Line Width", Float) = 0.1
        _Color("Color", Color) = (1, 1, 1, 1)
    }
    SubShader
    {
        Tags { "Queue" = "Transparent" "RenderType" = "Transparent" }
        LOD 100
        Cull Off
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct VertexIn
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct VertexOut
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            float _SegmentCount;
            float _SegmentSpacing;
            float _RemoveSegments;
            float _Rotation;
            float _Radius;
            float _LineWidth;
            float4 _Color;

            VertexOut vert(VertexIn v)
            {
                VertexOut o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag(VertexOut i) : SV_Target
            {
                float pi = 3.14159;
                float pi2 = pi * 2;
                float2 center = float2(0.5, 0.5);
                float2 uv = i.uv - center;

                // Rotate UV
                float angle = atan2(uv.y, uv.x) - radians(_Rotation);
                angle = angle < 0 ? angle + pi2 : angle;

                // Calculate segment angle
                float segmentAngle = pi2 / max(_SegmentCount, 1);
                float segmentIndex = floor(angle / segmentAngle);
                float segmentStart = segmentIndex * segmentAngle;
                float segmentEnd = segmentStart + segmentAngle;

                // Apply spacing
                float spacing = smoothstep(0.0, _SegmentSpacing, abs(angle - (segmentStart + segmentEnd) * 0.5));
                float segmentMask = 1.0 - spacing;

                // Ensure at least one segment is visible
                segmentMask = max(segmentMask, 0.01);

                // Remove segments
                float removedSegments = step(_RemoveSegments / _SegmentCount, segmentIndex / _SegmentCount);
                float finalMask = segmentMask * (1.0 - removedSegments);

                // Circle mask
                float radius = length(uv);
                float circleMask = smoothstep(_Radius - _LineWidth, _Radius, radius) * (1.0 - smoothstep(_Radius, _Radius + _LineWidth, radius));

                // Combine masks
                float alpha = finalMask * circleMask;

                // Ensure alpha is not completely zero
                alpha = max(alpha, 0.01);

                return fixed4(_Color.rgb, _Color.a * alpha);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}