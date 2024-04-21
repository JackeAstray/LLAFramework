Shader "ReunionMovement/UI/Procedural Image"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" { }
        _Color ("Tint", Color) = (1, 1, 1, 1)
        _TextureSize ("Texture Size", Vector) = (1, 1, 1, 1)
        
        _DrawShape ("Draw Shape", int) = 2
        
        _StrokeWidth ("Stroke Width", float) = 0
        _FalloffDistance ("Falloff Distance", float) = 0.5
        _PixelWorldScale ("Pixel world scale", Range(0.01, 5)) = 1
        _ShapeRotation ("shape Rotation", float) = 0
        _ConstrainRotation("Constrained Rotation", int) = 0
        _FlipHorizontal ("Flip Horizontal", int) = 0
        _FlipVertical ("Flip Vertical", int) = 0
        
        _RectangleCornerRadius ("Rectangle Corner Radius", Vector) = (0, 0, 0, 0)
        _CircleRadius ("Circle Radius", float) = 0
        _CircleFitRadius ("Fit Circle Radius", float) = 0
        _PentagonCornerRadius ("Pentagon Corner Radius", Vector) = (0, 0, 0, 0)
        _PentagonTipRadius ("Pentagon Triangle Radius", float) = 0
        _PentagonTipSize ("Pentagon Triangle Size", float) = 0
        _TriangleCornerRadius ("Triangle Radius", Vector) = (0, 0, 0, 0)
        _HexagonTipSize ("Hexagon Tip Size", Vector) = (0, 0, 0, 0)
        _HexagonTipRadius ("Hexagon Tip Radius", Vector) = (0, 0, 0, 0)
        _HexagonCornerRadius ("Hexagon Corner Radius", Vector) = (0, 0, 0, 0)
        _NStarPolygonSideCount ("NStar Polygon Side Count", float) = 3
        _NStarPolygonInset ("Nstar Polygon Inset", float) = 2
        _NStarPolygonCornerRadius ("Nstar Polygon Corner Radius", float) = 0
        _NStarPolygonOffset ("Nstar Polygon Offset", Vector) = (0, 0, 0, 0)

        _EnableGradient ("Enable GradientEffect", int) = 0
        _GradientType ("GradientEffect Type", int) = 0
        _GradientInterpolationType ("GradientEffect Interpolation Type", int) = 0
        _GradientRotation ("_GradientRotation", float) = 0
        _GradientColor0 ("GradientColor0", Vector) = (0, 0, 0, 0)
        _GradientColor1 ("GradientColor1", Vector) = (1, 1, 1, 1)
        _GradientColor2 ("GradientColor2", Vector) = (0, 0, 0, 0)
        _GradientColor3 ("GradientColor3", Vector) = (0, 0, 0, 0)
        _GradientColor4 ("GradientColor4", Vector) = (0, 0, 0, 0)
        _GradientColor5 ("GradientColor5", Vector) = (0, 0, 0, 0)
        _GradientColor6 ("GradientColor6", Vector) = (0, 0, 0, 0)
        _GradientColor7 ("GradientColor7", Vector) = (0, 0, 0, 0)
        _GradientColorLength ("GradientColorLength", int) = 0
        _GradientAlpha0 ("GradientAlpha0", Vector) = (1, 0, 0, 0)
        _GradientAlpha1 ("GradientAlpha1", Vector) = (1, 1, 0, 0)
        _GradientAlpha2 ("GradientAlpha2", Vector) = (0, 0, 0, 0)
        _GradientAlpha3 ("GradientAlpha3", Vector) = (0, 0, 0, 0)
        _GradientAlpha4 ("GradientAlpha4", Vector) = (0, 0, 0, 0)
        _GradientAlpha5 ("GradientAlpha5", Vector) = (0, 0, 0, 0)
        _GradientAlpha6 ("GradientAlpha6", Vector) = (0, 0, 0, 0)
        _GradientAlpha7 ("GradientAlpha7", Vector) = (0, 0, 0, 0)
        _GradientAlphaLength ("GradientAlphaLength", int) = 0
        _CornerGradientColor0 ("CornerGradientColor0", Color) = (1, 0, 0, 1)
        _CornerGradientColor1 ("CornerGradientColor1", Color) = (0, 1, 0, 1)
        _CornerGradientColor2 ("CornerGradientColor2", Color) = (0, 0, 1, 1)
        _CornerGradientColor3 ("CornerGradientColor3", Color) = (0, 0, 0, 1)
        
        _OutlineWidth ("Outline Width", float) = 0
        _OutlineColor ("Outline Color", Color) = (0, 0, 0, 1)
        
        _StencilComp ("Stencil Comparison", Float) = 8
        _Stencil ("Stencil ID", Float) = 0
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255
        
        _ColorMask ("Color Mask", Float) = 15
        
        [Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip ("Use Alpha Clip", Float) = 0
    }
    
    SubShader
    {
        Tags { "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" "PreviewType" = "Plane" "CanUseSpriteAtlas" = "True" }
        
        Stencil
        {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }
        
        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask [_ColorMask]
        
        Pass
        {
            Name "Default"
            CGPROGRAM
            
            #pragma vertex vert
            #pragma fragment frag
            
            #include "UnityCG.cginc"
            #include "UnityUI.cginc"
            #include "2D_SDF.cginc"
            
            #pragma multi_compile_local _ UNITY_UI_CLIP_RECT
            #pragma multi_compile_local _ UNITY_UI_ALPHACLIP
            
            #pragma multi_compile_local _ CIRCLE TRIANGLE RECTANGLE PENTAGON HEXAGON NSTAR_POLYGON HEART
            
            #pragma multi_compile_local _ STROKE OUTLINED OUTLINED_STROKE
            #pragma multi_compile_local _ GRADIENT_LINEAR GRADIENT_RADIAL GRADIENT_CORNER

            struct appdata_t
            {
                float4 vertex: POSITION;
                float4 color: COLOR;
                float2 texcoord: TEXCOORD0;
                float2 uv1: TEXCOORD1;
                float2 size: TEXCOORD2;
                
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };
            
            struct v2f
            {
                float4 vertex: SV_POSITION;
                fixed4 color: COLOR;
                float2 texcoord: TEXCOORD0;
                float4 shapeData: TEXCOORD1;
                float2 effectsUv: TEXCOORD2;
                float4 worldPosition : TEXCOORD3;


                /* //SOFTMASK_HANDLE_START
                SOFTMASK_COORDS(4)
                */ //SOFTMASK_HANDLE_END
                
                UNITY_VERTEX_OUTPUT_STEREO
            };
            
            sampler2D _MainTex; float4 _MainTex_ST;
            fixed4 _Color;
            fixed4 _TextureSize;
            fixed4 _TextureSampleAdd;
            float4 _ClipRect;
            half _PixelWorldScale;
            half _StrokeWidth;
            half _OutlineWidth;
            half4 _OutlineColor;
            half _FalloffDistance;
            half _ShapeRotation;
            half _ConstrainRotation;
            half _FlipHorizontal;
            half _FlipVertical;
            
            #if RECTANGLE
                float4 _RectangleCornerRadius;
            #endif
            
            #if CIRCLE
                float _CircleRadius;
                float _CircleFitRadius;
            #endif
            
            #if PENTAGON
                float4 _PentagonCornerRadius;
                float _PentagonTipRadius;
                float _PentagonTipSize;
            #endif
            
            #if TRIANGLE
                float3 _TriangleCornerRadius;
            #endif
            
            #if HEXAGON
                half2 _HexagonTipSize;
                half2 _HexagonTipRadius;
                half4 _HexagonCornerRadius;
            #endif
            
            #if NSTAR_POLYGON
                float _NStarPolygonSideCount;
                float _NStarPolygonCornerRadius;
                float _NStarPolygonInset;
                float2 _NStarPolygonOffset;
            #endif
            
            #if GRADIENT_LINEAR || GRADIENT_RADIAL
                half4 colors[8];
                half4 alphas[8];
                half _GradientInterpolationType;
                half _GradientColorLength;
                half _GradientAlphaLength;
                half _GradientRotation;
                
                half4 _GradientColor0;
                half4 _GradientColor1;
                half4 _GradientColor2;
                half4 _GradientColor3;
                half4 _GradientColor4;
                half4 _GradientColor5;
                half4 _GradientColor6;
                half4 _GradientColor7;
                
                half4 _GradientAlpha0;
                half4 _GradientAlpha1;
                half4 _GradientAlpha2;
                half4 _GradientAlpha3;
                half4 _GradientAlpha4;
                half4 _GradientAlpha5;
                half4 _GradientAlpha6;
                half4 _GradientAlpha7;
            #endif
            
            #if GRADIENT_CORNER
                half4 _CornerGradientColor0;
                half4 _CornerGradientColor1;
                half4 _CornerGradientColor2;
                half4 _CornerGradientColor3;
            #endif
            
            
            
            
            //渐变
            #if GRADIENT_LINEAR || GRADIENT_RADIAL
                float4 SampleGradient(float Time)
                {
                    float3 color = colors[0].rgb;
                    [unroll]
                    for (int c = 1; c < 8; c ++)
                    {
                        float colorPos = saturate((Time - colors[c - 1].w) / (colors[c].w - colors[c - 1].w)) * step(c, _GradientColorLength - 1);
                        color = lerp(color, colors[c].rgb, lerp(colorPos, step(0.01, colorPos), _GradientInterpolationType));
                    }
                    
                    float alpha = alphas[0].x;
                    [unroll]
                    for (int a = 1; a < 8; a ++)
                    {
                        float alphaPos = saturate((Time - alphas[a - 1].y) / (alphas[a].y - alphas[a - 1].y)) * step(a, _GradientAlphaLength - 1);
                        alpha = lerp(alpha, alphas[a].x, lerp(alphaPos, step(0.01, alphaPos), _GradientInterpolationType));
                    }
                    return float4(color, alpha);
                }
            #endif
            
            //矩形
            #if RECTANGLE
                half rectangleScene(float4 _additionalData)
                {

                    float2 _texcoord = _additionalData.xy;
                    float2 _size = float2(_additionalData.z, _additionalData.w);
                    float4 radius = _RectangleCornerRadius;
                    half4 c  = half4(_texcoord, _size - _texcoord);
                    half rect = min(min(min(c.x, c.y), c.z), c.w);

                    bool4 cornerRects;
                    cornerRects.x = _texcoord.x < radius.x && _texcoord.y < radius.x;
                    cornerRects.y = _texcoord.x > _size.x - radius.y && _texcoord.y < radius.y;
                    cornerRects.z = _texcoord.x > _size.x - radius.z && _texcoord.y > _size.y - radius.z;
                    cornerRects.w = _texcoord.x < radius.w && _texcoord.y > _size.y - radius.w;

                    half cornerMask = any(cornerRects);

                    half4 cornerCircles;
                    cornerCircles.x = radius.x - length(_texcoord - radius.xx);
                    cornerCircles.y = radius.y - length(_texcoord - half2(_size.x - radius.y, radius.y));
                    cornerCircles.z = radius.z - length(_texcoord - (half2(_size.x, _size.y) - radius.zz));
                    cornerCircles.w = radius.w - length(_texcoord - half2(radius.w, _size.y - radius.w));

                    cornerCircles = min(max(cornerCircles, 0) * cornerRects, rect);
                    half corners = max(max(max(cornerCircles.x, cornerCircles.y), cornerCircles.z), cornerCircles.w);
                    corners = max(corners, 0.0) * cornerMask;

                    return rect*(cornerMask-1) - corners;
                }
            #endif
            
            //圆
            #if CIRCLE
                float circleScene(float4 _additionalData)
                {
                    float2 _texcoord = _additionalData.xy;
                    float2 _size = float2(_additionalData.z, _additionalData.w);
                    float width = _size.x;
                    float height = _size.y;
                    float radius = lerp(_CircleRadius, min(width, height) / 2.0, _CircleFitRadius);
                    half sdf = circle(_texcoord - float2(width / 2.0, height / 2.0), radius);
                    return sdf;
                }
            #endif
            
            //三角形
            #if TRIANGLE
                half triangleScene(float4 _additionalData)
                {
                    float2 _texcoord = _additionalData.xy;
                    float2 _size = float2(_additionalData.z, _additionalData.w);
                    float width = _size.x;//_additionalData.z;
                    float height = _size.y;//_additionalData.w;
                    
                    half sdf = sdTriangleIsosceles(_texcoord - half2(width / 2.0, height), half2(width / 2.0, -height));
                    //return sdf;
                    
                    _TriangleCornerRadius = max(_TriangleCornerRadius, half3(0.001, 0.001, 0.001));
                    // Left Corner
                    half halfWidth = width / 2.0;
                    half m = height / halfWidth;
                    half d = sqrt(1.0 + m * m);
                    half c = 0.0;
                    half k = -_TriangleCornerRadius.x * d + c;
                    half x = (_TriangleCornerRadius.x - k) / m;
                    half2 circlePivot = half2(x, _TriangleCornerRadius.x);
                    half cornerCircle = circle(_texcoord - circlePivot, _TriangleCornerRadius.x);
                    //sdf = sdfDifference(sdf, cornerCircle);
                    //return sdf;
                    x = (circlePivot.y + circlePivot.x / m - c) / (m + 1.0 / m);
                    half y = m * x + c;
                    half fy = map(_texcoord.x, x, circlePivot.x, y, circlePivot.y);
                    sdf = _texcoord.y < fy && _texcoord.x < circlePivot.x ? cornerCircle: sdf;
                    //return sdf;
                    // Right Corner
                    m = -m; c = 2.0 * height;
                    k = -_TriangleCornerRadius.y * d + c;
                    x = (_TriangleCornerRadius.y - k) / m;
                    circlePivot = half2(x, _TriangleCornerRadius.y);
                    cornerCircle = circle(_texcoord - circlePivot, _TriangleCornerRadius.y);
                    x = (circlePivot.y + circlePivot.x / m - c) / (m + 1.0 / m); y = m * x + c;
                    fy = map(_texcoord.x, circlePivot.x, x, circlePivot.y, y);
                    sdf = _texcoord.x > circlePivot.x && _texcoord.y < fy ? cornerCircle: sdf;
                    
                    //Top Corner
                    k = -_TriangleCornerRadius.z * sqrt(1.0 + m * m) + c;
                    y = m * (width / 2.0) + k;
                    circlePivot = half2(halfWidth, y);
                    cornerCircle = circle(_texcoord - circlePivot, _TriangleCornerRadius.z);
                    x = (circlePivot.y + circlePivot.x / m - c) / (m + 1.0 / m); y = m * x + c;
                    fy = map(_texcoord.x, width - x, x, -1.0, 1.0);
                    fy = lerp(circlePivot.y, y, abs(fy));
                    sdf = _texcoord.y > fy ? cornerCircle: sdf;
                    
                    return sdf;
                }
            #endif
            
            //五边形
            #if PENTAGON
                half pentagonScene(float4 _additionalData)
                {
                    
                    float2 _texcoord = _additionalData.xy;
                    float2 _size = float2(_additionalData.z, _additionalData.w);
                    float width = _size.x;
                    float height = _size.y;
                    
                    // solid pentagon
                    half baseRect = rectanlge(_texcoord - half2(width / 2.0, height / 2.0), width, height);
                    half scale = height / _PentagonTipSize;
                    half rhombus = sdRhombus(_texcoord - float2(width / 2, _PentagonTipSize * scale), float2(width / 2, _PentagonTipSize) * scale);
                    half sdfPentagon = sdfDifference(baseRect, sdfDifference(baseRect, rhombus));
                    
                    // Bottom rounded corner
                    _PentagonTipRadius = max(_PentagonTipRadius, 0.001);
                    float halfWidth = width / 2;
                    float m = -_PentagonTipSize / halfWidth;
                    float d = sqrt(1 + m * m);
                    float c = _PentagonTipSize;
                    float k = _PentagonTipRadius * d + _PentagonTipSize;
                    
                    half2 circlePivot = half2(halfWidth, m * halfWidth + k);
                    half cornerCircle = circle(_texcoord - circlePivot, _PentagonTipRadius);
                    half x = (circlePivot.y + circlePivot.x / m - c) / (m + 1 / m);
                    half y = m * x + c;
                    half fy = map(_texcoord.x, x, width - x, -1, 1);
                    fy = lerp(_PentagonTipRadius, y, abs(fy));
                    sdfPentagon = _texcoord.y < fy ? cornerCircle: sdfPentagon;
                    
                    // Mid Left rounded corner
                    k = _PentagonCornerRadius.w * d + _PentagonTipSize;
                    circlePivot = half2(_PentagonCornerRadius.w, m * _PentagonCornerRadius.w + k);
                    cornerCircle = circle(_texcoord - circlePivot, _PentagonCornerRadius.w);
                    x = (circlePivot.y + circlePivot.x / m - c) / (m + 1 / m); y = m * x + c;
                    fy = map(_texcoord.x, x, circlePivot.x, y, circlePivot.y);
                    sdfPentagon = _texcoord.y > fy && _texcoord.y < circlePivot.y ? cornerCircle: sdfPentagon;
                    
                    // Mid Right rounded corner
                    m = -m; k = _PentagonCornerRadius.z * d - _PentagonTipSize;
                    circlePivot = half2(width - _PentagonCornerRadius.z, m * (width - _PentagonCornerRadius.z) + k);
                    cornerCircle = circle(_texcoord - circlePivot, _PentagonCornerRadius.z);
                    x = (circlePivot.y + circlePivot.x / m - c) / (m + 1 / m); y = m * x + c;
                    fy = map(_texcoord.x, circlePivot.x, x, circlePivot.y, y);
                    sdfPentagon = _texcoord.y > fy && _texcoord.y < circlePivot.y ? cornerCircle: sdfPentagon;
                    
                    // Top rounded corners
                    cornerCircle = circle(_texcoord - half2(_PentagonCornerRadius.x, height - _PentagonCornerRadius.x), _PentagonCornerRadius.x);
                    bool mask = _texcoord.x < _PentagonCornerRadius.x && _texcoord.y > height - _PentagonCornerRadius.x;
                    sdfPentagon = mask ? cornerCircle: sdfPentagon;
                    cornerCircle = circle(_texcoord - half2(width - _PentagonCornerRadius.y, height - _PentagonCornerRadius.y), _PentagonCornerRadius.y);
                    mask = _texcoord.x > width - _PentagonCornerRadius.y && _texcoord.y > height - _PentagonCornerRadius.y;
                    sdfPentagon = mask ? cornerCircle: sdfPentagon;
                    
                    return sdfPentagon;
                }
            #endif
            
            //六边形
            #if HEXAGON
                half hexagonScene(float4 _additionalData)
                {
                    float2 _texcoord = _additionalData.xy;
                    float2 _size = float2(_additionalData.z, _additionalData.w);
                    float width = _size.x;//_additionalData.z;
                    float height = _size.y;//_additionalData.w;
                    
                    half baseRect = rectanlge(_texcoord - half2(width / 2.0, height / 2.0), width, height);
                    half scale = width / _HexagonTipSize.x;
                    half rhombus1 = sdRhombus(_texcoord - float2(_HexagonTipSize.x * scale, height / 2.0), float2(_HexagonTipSize.x, height / 2.0) * scale);
                    scale = width / _HexagonTipSize.y;
                    half rhombus2 = sdRhombus(_texcoord - float2(width - _HexagonTipSize.y * scale, height / 2.0), float2(_HexagonTipSize.y, height / 2.0) * scale);
                    half sdfHexagon = sdfDifference(sdfDifference(baseRect, -rhombus1), -rhombus2);

                    //Left Rounded Corners
                    float halfHeight = height / 2.0;
                    float m = -halfHeight / _HexagonTipSize.x;
                    float c = halfHeight;
                    float d = sqrt(1.0 + m * m);
                    float k = _HexagonTipRadius.x * d + c;
                    //middle
                    half2 circlePivot = half2((halfHeight - k) / m, halfHeight);
                    half cornerCircle = circle(_texcoord - circlePivot, _HexagonTipRadius.x);
                    half x = (circlePivot.y + circlePivot.x / m - c) / (m + 1.0 / m);
                    half y = m * x + c;
                    half fy = map(_texcoord.x, x, circlePivot.x, y, circlePivot.y);
                    sdfHexagon = _texcoord.y > fy && _texcoord.y < height - fy ? cornerCircle: sdfHexagon;
                    //return sdfHexagon;
                    //bottom
                    k = _HexagonCornerRadius.x * d + c;
                    circlePivot = half2((_HexagonCornerRadius.x - k) / m, _HexagonCornerRadius.x);
                    cornerCircle = circle(_texcoord - circlePivot, _HexagonCornerRadius.x);
                    x = (circlePivot.y + circlePivot.x / m - c) / (m + 1.0 / m); y = m * x + c;
                    fy = map(_texcoord.x, x, circlePivot.x, y, circlePivot.y);
                    sdfHexagon = _texcoord.y < fy && _texcoord.x < circlePivot.x ? cornerCircle: sdfHexagon;

                    //return sdfHexagon;
                    //top
                    k = _HexagonCornerRadius.w * d + c;
                    circlePivot = half2((_HexagonCornerRadius.w - k) / m, height - _HexagonCornerRadius.w);
                    cornerCircle = circle(_texcoord - circlePivot, _HexagonCornerRadius.w);
                    x = (_HexagonCornerRadius.w + circlePivot.x / m - c) / (m + 1.0 / m); y = m * x + c;
                    fy = map(_texcoord.x, x, circlePivot.x, height - y, circlePivot.y);
                    sdfHexagon = _texcoord.y > fy && _texcoord.x < circlePivot.x ? cornerCircle: sdfHexagon;
                    //return sdfHexagon;
                    //Right Rounded Corners
                    m = halfHeight / _HexagonTipSize.y;
                    d = sqrt(1.0 + m * m);
                    c = halfHeight - m * width;
                    k = _HexagonTipRadius.y * d + c;
                    
                    //middle
                    circlePivot = half2((halfHeight - k) / m, halfHeight);
                    cornerCircle = circle(_texcoord - circlePivot, _HexagonTipRadius.y);
                    x = (circlePivot.y + circlePivot.x / m - c) / (m + 1.0 / m); y = m * x + c;
                    fy = map(_texcoord.x, circlePivot.x, x, circlePivot.y, y);
                    sdfHexagon = _texcoord.y > fy && _texcoord.y < height - fy ? cornerCircle: sdfHexagon;
                    //return sdfHexagon;
                    //bottom
                    k = _HexagonCornerRadius.y * d + c;
                    circlePivot = half2((_HexagonCornerRadius.y - k) / m, _HexagonCornerRadius.y);
                    cornerCircle = circle(_texcoord - circlePivot, _HexagonCornerRadius.y);
                    x = (circlePivot.y + circlePivot.x / m - c) / (m + 1.0 / m); y = m * x + c;
                    fy = map(_texcoord.x, circlePivot.x, x, circlePivot.y, y);
                    sdfHexagon = _texcoord.y < fy && _texcoord.x > circlePivot.x ? cornerCircle: sdfHexagon;
                    //return sdfHexagon;
                    //top
                    k = _HexagonCornerRadius.z * d + c;
                    circlePivot = half2((_HexagonCornerRadius.z - k) / m, height - _HexagonCornerRadius.z);
                    cornerCircle = circle(_texcoord - circlePivot, _HexagonCornerRadius.z);
                    x = (_HexagonCornerRadius.z + circlePivot.x / m - c) / (m + 1.0 / m); y = m * x + c;
                    fy = map(_texcoord.x, circlePivot.x, x, circlePivot.y, height - y);
                    sdfHexagon = _texcoord.y > fy && _texcoord.x > circlePivot.x ? cornerCircle: sdfHexagon;
                    
                    return sdfHexagon;
                }
                
            #endif
            
            //N星形多边形
            #if NSTAR_POLYGON
                half nStarPolygonScene(float4 _additionalData)
                {
                    float2 _texcoord = _additionalData.xy;
                    float width = _additionalData.z;
                    float height = _additionalData.w;
                    float size = height / 2 - _NStarPolygonCornerRadius;
                    half str = sdNStarPolygon(_texcoord - half2(width / 2, height / 2) - _NStarPolygonOffset, size, _NStarPolygonSideCount, _NStarPolygonInset) - _NStarPolygonCornerRadius;
                    return str;
                }
            #endif
            
            //心形
            #if HEART
                half heartScene(float4 _additionalData)
                {
                    //得到纹理坐标
                    float2 _texcoord = _additionalData.xy;
                    //得到宽
                    float width = _additionalData.z;
                    //得到高
                    float height = _additionalData.w;

                    float radius = min(width, height) * 0.7;

                    float2 value = _texcoord - float2(width * 0.5, height * 0.1);
                    half sdf = sdHeart(value, radius);
                    return sdf;
                }
            #endif

            //旋转uv
            float2 rotateUV(float2 uv, float rotation, float2 mid)
            {
                return float2(
                  cos(rotation) * (uv.x - mid.x) + sin(rotation) * (uv.y - mid.y) + mid.x,
                  cos(rotation) * (uv.y - mid.y) - sin(rotation) * (uv.x - mid.x) + mid.y
                );
            }
            
            //顶点着色器
            v2f vert(appdata_t v)
            {
                v2f OUT;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
                OUT.worldPosition = v.vertex;
                OUT.vertex = UnityObjectToClipPos(v.vertex);
                OUT.texcoord = v.texcoord;
                OUT.effectsUv = v.uv1;
                
                float2 size = float2(v.size.x + _FalloffDistance, v.size.y + _FalloffDistance);
                float shapeRotation = radians(_ShapeRotation);
                size = _ConstrainRotation > 0.0 && frac(abs(shapeRotation) / 3.14159) > 0.1? float2(size.y, size.x) : size;
                
                float2 shapeUv = _ConstrainRotation > 0 ? v.uv1 : v.uv1 * size;
                shapeUv = rotateUV(shapeUv, shapeRotation, _ConstrainRotation > 0? float2(0.5, 0.5) : size * 0.5);
                shapeUv*= _ConstrainRotation > 0.0? size : 1.0;
                
                shapeUv.x = lerp(shapeUv.x, abs(size.x - shapeUv.x), _FlipHorizontal);
                shapeUv.y = lerp(shapeUv.y, abs(size.y - shapeUv.y), _FlipVertical);
                
                OUT.shapeData = float4(shapeUv.x, shapeUv.y, size.x, size.y);
                
                #ifdef UNITY_HALF_TEXEL_OFFSET
                    OUT.vertex.xy += (_ScreenParams.zw - 1.0) * float2(-1.0, 1.0);
                #endif
                OUT.color = v.color * _Color;

                return OUT;
            }
            
            //片元着色器
            fixed4 frag(v2f IN): SV_Target
            {
                half4 color = IN.color;
                half2 texcoord = IN.texcoord;
                color = (tex2D(_MainTex, texcoord) + _TextureSampleAdd) * color;
                
                
                #if GRADIENT_LINEAR || GRADIENT_RADIAL
                    colors[0] = _GradientColor0;
                    colors[1] = _GradientColor1;
                    colors[2] = _GradientColor2;
                    colors[3] = _GradientColor3;
                    colors[4] = _GradientColor4;
                    colors[5] = _GradientColor5;
                    colors[6] = _GradientColor6;
                    colors[7] = _GradientColor7;
                    
                    alphas[0] = _GradientAlpha0;
                    alphas[1] = _GradientAlpha1;
                    alphas[2] = _GradientAlpha2;
                    alphas[3] = _GradientAlpha3;
                    alphas[4] = _GradientAlpha4;
                    alphas[5] = _GradientAlpha5;
                    alphas[6] = _GradientAlpha6;
                    alphas[7] = _GradientAlpha7;
                #endif
                
                #if GRADIENT_LINEAR
                    half gradientRotation = radians(_GradientRotation);
                    half t = cos(gradientRotation) * (IN.effectsUv.x - 0.5) + 
                             sin(gradientRotation) * (IN.effectsUv.y - 0.5) + 0.5;
                    half4 grad = SampleGradient(t);
                    color *= grad;
                #endif
                #if GRADIENT_RADIAL
                    half fac = saturate(length(IN.effectsUv - float2(.5, .5)) * 2);
                    half4 grad = SampleGradient(clamp(fac, 0, 1));
                    color *= grad;
                #endif
                
                #if GRADIENT_CORNER
                    half4 topCol = lerp(_CornerGradientColor2, _CornerGradientColor3, IN.effectsUv.x);
                    half4 bottomCol = lerp(_CornerGradientColor0, _CornerGradientColor1, IN.effectsUv.x);
                    half4 finalCol = lerp(topCol, bottomCol, IN.effectsUv.y);
                    
                    color *= finalCol;
                #endif
                
                #if RECTANGLE || CIRCLE || PENTAGON || TRIANGLE || HEXAGON || NSTAR_POLYGON || HEART
                    float sdfData = 0;
                    float pixelScale = clamp(1.0/_FalloffDistance, 1.0/2048.0, 2048.0);
                    #if RECTANGLE
                        sdfData = rectangleScene(IN.shapeData);
                    #elif CIRCLE
                        sdfData = circleScene(IN.shapeData);
                    #elif PENTAGON
                        sdfData = pentagonScene(IN.shapeData);
                    #elif TRIANGLE
                        sdfData = triangleScene(IN.shapeData);
                    #elif HEXAGON
                        sdfData = hexagonScene(IN.shapeData);
                    #elif NSTAR_POLYGON
                        sdfData = nStarPolygonScene(IN.shapeData);
                    #elif HEART
                        sdfData = heartScene(IN.shapeData);
                    #endif
                
                    #if !OUTLINED && !STROKE && !OUTLINED_STROKE
                        float sdf = sampleSdf(sdfData, pixelScale);
                        color.a *= sdf;
                    #endif

                    #if STROKE
                        float sdf = sampleSdfStrip(sdfData, _StrokeWidth + _OutlineWidth, pixelScale);
                        color.a *= sdf;
                    #endif
                    
                    #if OUTLINED
                        float alpha = sampleSdf(sdfData, pixelScale);
                        float lerpFac = sampleSdf(sdfData + _OutlineWidth, pixelScale);
                        color = half4(lerp(_OutlineColor.rgb, color.rgb, lerpFac), lerp(_OutlineColor.a * color.a, color.a, lerpFac));
                        color.a *= alpha;
                    #endif
                    
                    #if OUTLINED_STROKE
                        float alpha = sampleSdfStrip(sdfData, _OutlineWidth + _StrokeWidth, pixelScale);
                        float lerpFac = sampleSdfStrip(sdfData + _OutlineWidth, _StrokeWidth + _FalloffDistance, pixelScale);
                        lerpFac = clamp(lerpFac, 0, 1);
                        color = half4(lerp(_OutlineColor.rgb, color.rgb, lerpFac), lerp(_OutlineColor.a * color.a, color.a, lerpFac));
                        color.a *= alpha;
                    #endif
                #endif

                #ifdef UNITY_UI_CLIP_RECT
                    color.a *= UnityGet2DClipping(IN.worldPosition.xy, _ClipRect);
                #endif
                
                #ifdef UNITY_UI_ALPHACLIP
                    clip(color.a - 0.001);
                #endif
                
                return fixed4(color);
            }
            ENDCG
            
        }
    }
    CustomEditor "GameLogic.UI.ImageExtensions.Editor.ImageShaderGUI"
}
