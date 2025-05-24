Shader "ReunionMovement/UI/Procedural Image"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" { }
        _Color ("Tint", Color) = (1, 1, 1, 1)
        _TextureSize ("Texture Size", Vector) = (1, 1, 1, 1)
        
        _DrawShape ("绘制形状", int) = 2
        
        _StrokeWidth ("线条宽度", float) = 0
        _FalloffDistance ("衰减距离", float) = 0.5
        _PixelWorldScale ("像素与世界单位之间的缩放比例", Range(0.01, 5)) = 1
        _ShapeRotation ("形状旋转", float) = 0
        _ConstrainRotation("约束旋转", int) = 0
        _FlipHorizontal ("水平翻转", int) = 0
        _FlipVertical ("垂直翻转", int) = 0
        
        _RectangleCornerRadius ("矩形四个角的圆角半径", Vector) = (0, 0, 0, 0)
        _CircleRadius ("圆半径", float) = 0
        _CircleFitRadius ("拟合圆半径", float) = 0
        _PentagonCornerRadius ("定义五边形的四个角的圆角半径", Vector) = (0, 0, 0, 0)
        _PentagonTipRadius ("五边形顶部尖角的圆角半径", float) = 0
        _PentagonTipSize ("五边形顶部尖角的大小", float) = 0
        _TriangleCornerRadius ("三角形三个角的圆角半径", Vector) = (0, 0, 0, 0)
        _HexagonTipSize ("六边形顶部尖角的大小", Vector) = (0, 0, 0, 0)
        _HexagonTipRadius ("六边形顶部尖角的圆角半径", Vector) = (0, 0, 0, 0)
        _HexagonCornerRadius ("六边形六个角的圆角半径", Vector) = (0, 0, 0, 0)
        _ChamferBoxSize ("倒角盒子尺寸", Vector) = (0.8, 0.4, 0, 0)
        _ChamferBoxRadius ("倒角半径", Float) = 0.15
        _ParallelogramValue ("平行四边形值", Float) = 0
        _NStarPolygonSideCount ("星形多边形的边数", float) = 3
        _NStarPolygonInset ("星形多边形的内凹程度", float) = 2
        _NStarPolygonCornerRadius ("星形多边形角的圆角半径", float) = 0
        _NStarPolygonOffset ("星形多边形的偏移量", Vector) = (0, 0, 0, 0)

        _EnableGradient ("启用渐变效果", int) = 0
        _GradientType ("渐变的类型", int) = 0
        _GradientInterpolationType ("渐变的插值方式", int) = 0
        _GradientRotation ("渐变旋转", float) = 0
        _GradientColor0 ("渐变颜色 0", Vector) = (0, 0, 0, 0)
        _GradientColor1 ("渐变颜色 1", Vector) = (1, 1, 1, 1)
        _GradientColor2 ("渐变颜色 2", Vector) = (0, 0, 0, 0)
        _GradientColor3 ("渐变颜色 3", Vector) = (0, 0, 0, 0)
        _GradientColor4 ("渐变颜色 4", Vector) = (0, 0, 0, 0)
        _GradientColor5 ("渐变颜色 5", Vector) = (0, 0, 0, 0)
        _GradientColor6 ("渐变颜色 6", Vector) = (0, 0, 0, 0)
        _GradientColor7 ("渐变颜色 7", Vector) = (0, 0, 0, 0)
        _GradientColorLength ("渐变颜色的数量", int) = 0
        _GradientAlpha0 ("渐变透明度 0", Vector) = (1, 0, 0, 0)
        _GradientAlpha1 ("渐变透明度 1", Vector) = (1, 1, 0, 0)
        _GradientAlpha2 ("渐变透明度 2", Vector) = (0, 0, 0, 0)
        _GradientAlpha3 ("渐变透明度 3", Vector) = (0, 0, 0, 0)
        _GradientAlpha4 ("渐变透明度 4", Vector) = (0, 0, 0, 0)
        _GradientAlpha5 ("渐变透明度 5", Vector) = (0, 0, 0, 0)
        _GradientAlpha6 ("渐变透明度 6", Vector) = (0, 0, 0, 0)
        _GradientAlpha7 ("渐变透明度 7", Vector) = (0, 0, 0, 0)
        _GradientAlphaLength ("渐变透明度的数量", int) = 0
        _CornerGradientColor0 ("角0渐变效果", Color) = (1, 0, 0, 1)
        _CornerGradientColor1 ("角1渐变效果", Color) = (0, 1, 0, 1)
        _CornerGradientColor2 ("角2渐变效果", Color) = (0, 0, 1, 1)
        _CornerGradientColor3 ("角3渐变效果", Color) = (0, 0, 0, 1)
        
        _OutlineWidth ("轮廓宽", float) = 0
        _OutlineColor ("轮廓颜色", Color) = (0, 0, 0, 1)
        // 虚线开关、自定义时间
        _EnableDashedOutline ("启用虚线轮廓", int) = 0 
        _CustomTime ("自定义时间值", Float) = 0

        _BlobbyCrossTime ("水滴十字形状的动态时间参数", Float) = 0
        _SquircleTime ("方圆形形状的动态时间参数", Float) = 1
        _NTriangleRoundedTime ("N三角形圆角形状的动态时间参数", Float) = 0
        _NTriangleRoundedNumber ("N三角形圆角形状的边数", Float) = 0

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
            #include "../../Base/2D_SDF.cginc"
            #include "../../Base/Common.cginc"
            
            #pragma multi_compile_local _ UNITY_UI_CLIP_RECT
            #pragma multi_compile_local _ UNITY_UI_ALPHACLIP
            
            #pragma multi_compile_local _ CIRCLE TRIANGLE RECTANGLE PENTAGON HEXAGON CHAMFERBOX PARALLELOGRAM NSTAR_POLYGON HEART BLOBBYCROSS SQUIRCLE NTRIANGLE_ROUNDED
            
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
                
                UNITY_VERTEX_OUTPUT_STEREO
            };
            
            int _DrawShape;

            sampler2D _MainTex; float4 _MainTex_ST;
            fixed4 _Color;
            fixed4 _TextureSize;
            fixed4 _TextureSampleAdd;
            float4 _ClipRect;
            half _PixelWorldScale;
            half _StrokeWidth;
            
            half _OutlineWidth;
            half4 _OutlineColor;
            int _EnableDashedOutline;
            float _CustomTime;

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

            #if CHAMFERBOX
                float2 _ChamferBoxSize;
                float _ChamferBoxRadius;
            #endif

            #if PARALLELOGRAM
                float _ParallelogramValue; 
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
            
            #if BLOBBYCROSS
                float _BlobbyCrossTime;
            #endif
            
            #if SQUIRCLE
                float _SquircleTime;
            #endif

            #if NTRIANGLE_ROUNDED
                float _NTriangleRoundedTime;
                float _NTriangleRoundedNumber;
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
                half rectangleScene(float4 additionalData)
                {
                    float2 texcoord = additionalData.xy;
                    float2 size = float2(additionalData.z, additionalData.w);
                    float4 radius = _RectangleCornerRadius;
                    half4 c  = half4(texcoord, size - texcoord);
                    half rect = min(min(min(c.x, c.y), c.z), c.w);

                    bool4 cornerRects;
                    cornerRects.x = texcoord.x < radius.x && texcoord.y < radius.x;
                    cornerRects.y = texcoord.x > size.x - radius.y && texcoord.y < radius.y;
                    cornerRects.z = texcoord.x > size.x - radius.z && texcoord.y > size.y - radius.z;
                    cornerRects.w = texcoord.x < radius.w && texcoord.y > size.y - radius.w;

                    half cornerMask = any(cornerRects);

                    half4 cornerCircles;
                    cornerCircles.x = radius.x - length(texcoord - radius.xx);
                    cornerCircles.y = radius.y - length(texcoord - half2(size.x - radius.y, radius.y));
                    cornerCircles.z = radius.z - length(texcoord - (half2(size.x, size.y) - radius.zz));
                    cornerCircles.w = radius.w - length(texcoord - half2(radius.w, size.y - radius.w));

                    cornerCircles = min(max(cornerCircles, 0) * cornerRects, rect);
                    half corners = max(max(max(cornerCircles.x, cornerCircles.y), cornerCircles.z), cornerCircles.w);
                    corners = max(corners, 0.0) * cornerMask;

                    return rect*(cornerMask-1) - corners;
                }
            #endif
            
            //圆
            #if CIRCLE
                float circleScene(float4 additionalData)
                {
                    float2 texcoord = additionalData.xy;
                    float2 size = float2(additionalData.z, additionalData.w);
                    float width = size.x;
                    float height = size.y;
                    float radius = lerp(_CircleRadius, min(width, height) / 2.0, _CircleFitRadius);
                    half sdf = sdCircle(texcoord - float2(width / 2.0, height / 2.0), radius);
                    return sdf;
                }
            #endif
            
            //三角形
            #if TRIANGLE
                half triangleScene(float4 additionalData)
                {
                    float2 texcoord = additionalData.xy;
                    float2 size = float2(additionalData.z, additionalData.w);
                    float width = size.x;
                    float height = size.y;
                    
                    half sdf = sdTriangleIsosceles(texcoord - half2(width / 2.0, height), half2(width / 2.0, -height));
                    
                    _TriangleCornerRadius = max(_TriangleCornerRadius, half3(0.001, 0.001, 0.001));

                    // 左角
                    half halfWidth = width / 2.0;
                    half m = height / halfWidth;
                    half d = sqrt(1.0 + m * m);
                    half c = 0.0;
                    half k = -_TriangleCornerRadius.x * d + c;
                    half x = (_TriangleCornerRadius.x - k) / m;
                    half2 circlePivot = half2(x, _TriangleCornerRadius.x);
                    half cornerCircle = sdCircle(texcoord - circlePivot, _TriangleCornerRadius.x);

                    x = (circlePivot.y + circlePivot.x / m - c) / (m + 1.0 / m);
                    half y = m * x + c;
                    half fy = map(texcoord.x, x, circlePivot.x, y, circlePivot.y);
                    sdf = texcoord.y < fy && texcoord.x < circlePivot.x ? cornerCircle: sdf;

                    // 右角
                    m = -m; c = 2.0 * height;
                    k = -_TriangleCornerRadius.y * d + c;
                    x = (_TriangleCornerRadius.y - k) / m;
                    circlePivot = half2(x, _TriangleCornerRadius.y);
                    cornerCircle = sdCircle(texcoord - circlePivot, _TriangleCornerRadius.y);
                    x = (circlePivot.y + circlePivot.x / m - c) / (m + 1.0 / m); y = m * x + c;
                    fy = map(texcoord.x, circlePivot.x, x, circlePivot.y, y);
                    sdf = texcoord.x > circlePivot.x && texcoord.y < fy ? cornerCircle: sdf;
                    
                    // 上角
                    k = -_TriangleCornerRadius.z * sqrt(1.0 + m * m) + c;
                    y = m * (width / 2.0) + k;
                    circlePivot = half2(halfWidth, y);
                    cornerCircle = sdCircle(texcoord - circlePivot, _TriangleCornerRadius.z);
                    x = (circlePivot.y + circlePivot.x / m - c) / (m + 1.0 / m); y = m * x + c;
                    fy = map(texcoord.x, width - x, x, -1.0, 1.0);
                    fy = lerp(circlePivot.y, y, abs(fy));
                    sdf = texcoord.y > fy ? cornerCircle: sdf;
                    
                    return sdf;
                }
            #endif
            
            //五边形
            #if PENTAGON
                half pentagonScene(float4 additionalData)
                {
                    
                    float2 texcoord = additionalData.xy;
                    float2 size = float2(additionalData.z, additionalData.w);
                    float width = size.x;
                    float height = size.y;
                    
                    // 实心五边形
                    half baseRect = sdRectanlge(texcoord - half2(width / 2.0, height / 2.0), width, height);
                    half scale = height / _PentagonTipSize;
                    half rhombus = sdRhombus(texcoord - float2(width / 2, _PentagonTipSize * scale), float2(width / 2, _PentagonTipSize) * scale);
                    half sdfPentagon = sdfDifference(baseRect, sdfDifference(baseRect, rhombus));
                    
                    // 底部圆角
                    _PentagonTipRadius = max(_PentagonTipRadius, 0.001);
                    float halfWidth = width / 2;
                    float m = -_PentagonTipSize / halfWidth;
                    float d = sqrt(1 + m * m);
                    float c = _PentagonTipSize;
                    float k = _PentagonTipRadius * d + _PentagonTipSize;
                    
                    half2 circlePivot = half2(halfWidth, m * halfWidth + k);
                    half cornerCircle = sdCircle(texcoord - circlePivot, _PentagonTipRadius);
                    half x = (circlePivot.y + circlePivot.x / m - c) / (m + 1 / m);
                    half y = m * x + c;
                    half fy = map(texcoord.x, x, width - x, -1, 1);
                    fy = lerp(_PentagonTipRadius, y, abs(fy));
                    sdfPentagon = texcoord.y < fy ? cornerCircle: sdfPentagon;
                    
                    // 左中圆角
                    k = _PentagonCornerRadius.w * d + _PentagonTipSize;
                    circlePivot = half2(_PentagonCornerRadius.w, m * _PentagonCornerRadius.w + k);
                    cornerCircle = sdCircle(texcoord - circlePivot, _PentagonCornerRadius.w);
                    x = (circlePivot.y + circlePivot.x / m - c) / (m + 1 / m); y = m * x + c;
                    fy = map(texcoord.x, x, circlePivot.x, y, circlePivot.y);
                    sdfPentagon = texcoord.y > fy && texcoord.y < circlePivot.y ? cornerCircle: sdfPentagon;
                    
                    // 右中圆角
                    m = -m; k = _PentagonCornerRadius.z * d - _PentagonTipSize;
                    circlePivot = half2(width - _PentagonCornerRadius.z, m * (width - _PentagonCornerRadius.z) + k);
                    cornerCircle = sdCircle(texcoord - circlePivot, _PentagonCornerRadius.z);
                    x = (circlePivot.y + circlePivot.x / m - c) / (m + 1 / m); y = m * x + c;
                    fy = map(texcoord.x, circlePivot.x, x, circlePivot.y, y);
                    sdfPentagon = texcoord.y > fy && texcoord.y < circlePivot.y ? cornerCircle: sdfPentagon;
                    
                    // 顶部圆角
                    cornerCircle = sdCircle(texcoord - half2(_PentagonCornerRadius.x, height - _PentagonCornerRadius.x), _PentagonCornerRadius.x);
                    bool mask = texcoord.x < _PentagonCornerRadius.x && texcoord.y > height - _PentagonCornerRadius.x;
                    sdfPentagon = mask ? cornerCircle: sdfPentagon;
                    cornerCircle = sdCircle(texcoord - half2(width - _PentagonCornerRadius.y, height - _PentagonCornerRadius.y), _PentagonCornerRadius.y);
                    mask = texcoord.x > width - _PentagonCornerRadius.y && texcoord.y > height - _PentagonCornerRadius.y;
                    sdfPentagon = mask ? cornerCircle: sdfPentagon;
                    
                    return sdfPentagon;
                }
            #endif
            
            //六边形
            #if HEXAGON
                half hexagonScene(float4 additionalData)
                {
                    float2 texcoord = additionalData.xy;
                    float2 size = float2(additionalData.z, additionalData.w);
                    float width = size.x;
                    float height = size.y;
                    
                    half baseRect = sdRectanlge(texcoord - half2(width / 2.0, height / 2.0), width, height);
                    half scale = width / _HexagonTipSize.x;
                    half rhombus1 = sdRhombus(texcoord - float2(_HexagonTipSize.x * scale, height / 2.0), float2(_HexagonTipSize.x, height / 2.0) * scale);
                    scale = width / _HexagonTipSize.y;
                    half rhombus2 = sdRhombus(texcoord - float2(width - _HexagonTipSize.y * scale, height / 2.0), float2(_HexagonTipSize.y, height / 2.0) * scale);
                    half sdfHexagon = sdfDifference(sdfDifference(baseRect, -rhombus1), -rhombus2);

                    //Left Rounded Corners
                    float halfHeight = height / 2.0;
                    float m = -halfHeight / _HexagonTipSize.x;
                    float c = halfHeight;
                    float d = sqrt(1.0 + m * m);
                    float k = _HexagonTipRadius.x * d + c;
                    //middle
                    half2 circlePivot = half2((halfHeight - k) / m, halfHeight);
                    half cornerCircle = sdCircle(texcoord - circlePivot, _HexagonTipRadius.x);
                    half x = (circlePivot.y + circlePivot.x / m - c) / (m + 1.0 / m);
                    half y = m * x + c;
                    half fy = map(texcoord.x, x, circlePivot.x, y, circlePivot.y);
                    sdfHexagon = texcoord.y > fy && texcoord.y < height - fy ? cornerCircle: sdfHexagon;
                    //return sdfHexagon;
                    //bottom
                    k = _HexagonCornerRadius.x * d + c;
                    circlePivot = half2((_HexagonCornerRadius.x - k) / m, _HexagonCornerRadius.x);
                    cornerCircle = sdCircle(texcoord - circlePivot, _HexagonCornerRadius.x);
                    x = (circlePivot.y + circlePivot.x / m - c) / (m + 1.0 / m); y = m * x + c;
                    fy = map(texcoord.x, x, circlePivot.x, y, circlePivot.y);
                    sdfHexagon = texcoord.y < fy && texcoord.x < circlePivot.x ? cornerCircle: sdfHexagon;

                    //return sdfHexagon;
                    //top
                    k = _HexagonCornerRadius.w * d + c;
                    circlePivot = half2((_HexagonCornerRadius.w - k) / m, height - _HexagonCornerRadius.w);
                    cornerCircle = sdCircle(texcoord - circlePivot, _HexagonCornerRadius.w);
                    x = (_HexagonCornerRadius.w + circlePivot.x / m - c) / (m + 1.0 / m); y = m * x + c;
                    fy = map(texcoord.x, x, circlePivot.x, height - y, circlePivot.y);
                    sdfHexagon = texcoord.y > fy && texcoord.x < circlePivot.x ? cornerCircle: sdfHexagon;
                    //return sdfHexagon;
                    //Right Rounded Corners
                    m = halfHeight / _HexagonTipSize.y;
                    d = sqrt(1.0 + m * m);
                    c = halfHeight - m * width;
                    k = _HexagonTipRadius.y * d + c;
                    
                    //middle
                    circlePivot = half2((halfHeight - k) / m, halfHeight);
                    cornerCircle = sdCircle(texcoord - circlePivot, _HexagonTipRadius.y);
                    x = (circlePivot.y + circlePivot.x / m - c) / (m + 1.0 / m); y = m * x + c;
                    fy = map(texcoord.x, circlePivot.x, x, circlePivot.y, y);
                    sdfHexagon = texcoord.y > fy && texcoord.y < height - fy ? cornerCircle: sdfHexagon;
                    //return sdfHexagon;
                    //bottom
                    k = _HexagonCornerRadius.y * d + c;
                    circlePivot = half2((_HexagonCornerRadius.y - k) / m, _HexagonCornerRadius.y);
                    cornerCircle = sdCircle(texcoord - circlePivot, _HexagonCornerRadius.y);
                    x = (circlePivot.y + circlePivot.x / m - c) / (m + 1.0 / m); y = m * x + c;
                    fy = map(texcoord.x, circlePivot.x, x, circlePivot.y, y);
                    sdfHexagon = texcoord.y < fy && texcoord.x > circlePivot.x ? cornerCircle: sdfHexagon;
                    //return sdfHexagon;
                    //top
                    k = _HexagonCornerRadius.z * d + c;
                    circlePivot = half2((_HexagonCornerRadius.z - k) / m, height - _HexagonCornerRadius.z);
                    cornerCircle = sdCircle(texcoord - circlePivot, _HexagonCornerRadius.z);
                    x = (_HexagonCornerRadius.z + circlePivot.x / m - c) / (m + 1.0 / m); y = m * x + c;
                    fy = map(texcoord.x, circlePivot.x, x, circlePivot.y, height - y);
                    sdfHexagon = texcoord.y > fy && texcoord.x > circlePivot.x ? cornerCircle: sdfHexagon;
                    
                    return sdfHexagon;
                }
            #endif

            #if CHAMFERBOX
                half chamferBoxScene(float4 additionalData)
                {
                    float2 texcoord = additionalData.xy;
                    float2 size = float2(additionalData.z, additionalData.w);

                    // 归一化到中心
                    float2 p = (2.0 * texcoord - size) / size.y;

                    // 动态倒角半径（可选：可加动画/时间参数）
                    float chamfer = _ChamferBoxRadius;

                    // box参数
                    float2 box = _ChamferBoxSize;

                    // 防御性检查，防止倒角过大
                    chamfer = min(chamfer, min(box.x, box.y));

                    float d = sdChamferBox(p, box, chamfer);

                    // 放大SDF以适配像素空间
                    return d * 80.0;
                }
            #endif
            
            #if PARALLELOGRAM
                half parallelogramScene(float4 additionalData)
                {
                    float2 texcoord = additionalData.xy;
                    float2 size = float2(additionalData.z, additionalData.w);

                    // 归一化到中心
                    float2 p = (2.0 * texcoord - size) / size.y;

                    // 动态斜率（可用时间或参数控制，这里用常量）
                    float sk = 0.5 * sin(_ParallelogramValue);

                    // 宽高参数（可根据UI参数调整）
                    float wi = (size.x / size.y) * 0.58;
                    float he = 1;

                    float d = sdParallelogram(p, wi, he, sk);

                    // 放大SDF以适配像素空间
                    return d * 80.0;
                }
            #endif

            //N星形多边形
            #if NSTAR_POLYGON
                half nStarPolygonScene(float4 additionalData)
                {
                    float2 texcoord = additionalData.xy;
                    float width = additionalData.z;
                    float height = additionalData.w;
                    float size = height / 2 - _NStarPolygonCornerRadius;
                    half str = sdNStarPolygon(texcoord - half2(width / 2, height / 2) - _NStarPolygonOffset, size, _NStarPolygonSideCount, _NStarPolygonInset) - _NStarPolygonCornerRadius;
                    return str;
                }
            #endif
            
            //心形
            #if HEART
                half heartScene(float4 additionalData)
                {
                    //得到纹理坐标
                    float2 texcoord = additionalData.xy;
                    //得到宽
                    float width = additionalData.z;
                    //得到高
                    float height = additionalData.w;

                    float radius = min(width, height) * 0.8;

                    float2 value = texcoord - float2(width * 0.5, height * 0.1);
                    half sdf = sdHeart(value, radius) * 110;
                    return sdf;
                }
            #endif

            //水滴十字
            #if BLOBBYCROSS
                half blobbyCrossScene(float4 additionalData)
                {
                     //得到纹理坐标
                    float2 texcoord = additionalData.xy;
                    //得到宽
                    float width = additionalData.z;
                    //得到高
                    float height = additionalData.w;

                    float2 p = (2.0 * texcoord - additionalData.zw) / width;
                    p *= 2.0;

                    float time = _BlobbyCrossTime;
                    float he = sin(time * 0.43 + 4.0);
                    he = (0.001 + abs(he)) * ((he >= 0.0) ? 1.0 : -1.0);
                    float ra = 0.1 + 0.5 * (0.5 + 0.5 * sin(time * 1.7)) + max(0.0, he - 0.7);

                    float d = sdBlobbyCross(p, he) - ra;

                    d = d * 35;

                    return d;
                }
            #endif

            //方圆形 菱形
            #if SQUIRCLE
                half squircleScene(float4 additionalData)
                {
                    float2 texcoord = additionalData.xy;
                    float width = additionalData.z;
                    float height = additionalData.w;

                    float2 p = (2.0 * texcoord - additionalData.zw) / width;

                    // 增加归一化处理，确保中心区域平滑
                    float n = 3.0 + 3 * sin(9.8 * _SquircleTime / 2.0);
                    float d = sdSquircle(p, n);
                    d = d * 80;

                    return d;
                }
            #endif

            //N三角形圆角
            #if NTRIANGLE_ROUNDED

            half nTriangleRoundedScene(float4 additionalData)
            {
                float2 texcoord = additionalData.xy;
                float width = additionalData.z;
                float height = additionalData.w;


                float2 p = (2.0 * texcoord - float2(width,height)) / max(width,height);

                // 动态时间控制
                float time = _NTriangleRoundedTime;
                float number = _NTriangleRoundedNumber;
                float rounding = 0.1 - 0.1 * cos(radians(360.0) * time);
                float n = floor(3.0 + fmod(1.0 * number, 15.0));

                // 应用角度重复
                p = opRepAng(p, radians(360.0) / n, radians(30));

                // 计算三角形的内切圆半径和边长
                float r = 1.0;
                float r_in = r * cos(radians(180.0) / n);
                float side_length = 2.0 * r_in * tan(radians(180.0) / n);

                // 计算带圆角的等腰三角形的 SDF
                float d = sdTriangleIsoscelesRounded(p.yx, float2(0.5 * side_length, r_in), rounding);

                // 返回 SDF 值，放大以适配像素空间
                return d * 80.0;
            }
            #endif

            // --------------------RECTANGLE Start---------------------
            float generateDashedEffect(v2f IN, float time, float aspectRatio, int shapeType)
            {
                // 定义虚线的波长和占空比
                float wavelength = 0.2; // 虚线的周期
                float dashRatio = 0.5;  // 虚线的占空比

                float dashedEffect = 0.0;

                if (shapeType == 1) // CIRCLE
                {
                    #if CIRCLE
                    // 计算图形中心
                    float2 center = float2(IN.shapeData.z * 0.5, IN.shapeData.w * 0.5);
                    // 当前点的角度
                    float angle = atan2(IN.shapeData.y - center.y, IN.shapeData.x - center.x); 
                    float normalizedAngle = (angle + 3.1415926) / (2.0 * 3.1415926); // 归一化到 [0, 1]

                    // 计算当前点到中心的距离
                    float distance = length(float2(IN.shapeData.x - center.x, IN.shapeData.y - center.y));
                    //float radius = min(IN.shapeData.z, IN.shapeData.w) * 0.5; // 圆的半径

                    // 定义内外边界
                    float innerRadius = _CircleRadius * 0.95; // 内边界半径（裁剪中间部分）
                    float outerRadius = _CircleRadius;       // 外边界半径（边缘部分）
                    float edgeMask = smoothstep(innerRadius, innerRadius + 0.01, distance) * 
                                     (1.0 - smoothstep(outerRadius - 0.01, outerRadius, distance));

                    // 基于角度生成虚线
                    float dashPattern = step(0.5, frac(normalizedAngle * 15.0 + _CustomTime));
                    dashedEffect = dashPattern * edgeMask; // 结合边缘遮罩
                    #endif
                }
                else if (shapeType == 3) // RECTANGLE
                {
                    float2 uv = IN.shapeData.xy / float2(IN.shapeData.z, IN.shapeData.w); // 归一化 UV 坐标
                    time = fmod(time, wavelength / 2);

                    // 矩形虚线逻辑
                    float dashedTop = generateDashedPattern(uv.x + time, wavelength, dashRatio);
                    float dashedBottom = generateDashedPattern(uv.x - time, wavelength, dashRatio);
                    float dashedLeft = generateDashedPattern(uv.y + time, wavelength, dashRatio);
                    float dashedRight = generateDashedPattern(uv.y - time, wavelength, dashRatio);

                    float edgeTop = getEdge(1.0 - uv.y, 0.02, 1.0);
                    float edgeBottom = getEdge(uv.y, 0.02, 1.0);
                    float edgeLeft = getEdge(uv.x, 0.02, aspectRatio);
                    float edgeRight = getEdge(1.0 - uv.x, 0.02, aspectRatio);

                    dashedEffect = edgeTop * dashedTop +
                                   edgeBottom * dashedBottom +
                                   edgeLeft * dashedLeft +
                                   edgeRight * dashedRight;
                }
                return saturate(dashedEffect);
            }
            // --------------------RECTANGLE End---------------------
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
                
                #if RECTANGLE || CIRCLE || PENTAGON || TRIANGLE || HEXAGON || CHAMFERBOX || PARALLELOGRAM || NSTAR_POLYGON || HEART || BLOBBYCROSS || SQUIRCLE || NTRIANGLE_ROUNDED
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
                    #elif CHAMFERBOX
                        sdfData = chamferBoxScene(IN.shapeData);
                    #elif PARALLELOGRAM
                        sdfData = parallelogramScene(IN.shapeData);
                    #elif NSTAR_POLYGON
                        sdfData = nStarPolygonScene(IN.shapeData);
                    #elif HEART
                        sdfData = heartScene(IN.shapeData);
                    #elif BLOBBYCROSS
                        sdfData = blobbyCrossScene(IN.shapeData);
                    #elif SQUIRCLE
                        sdfData = squircleScene(IN.shapeData);
                    #elif NTRIANGLE_ROUNDED
                        sdfData = nTriangleRoundedScene(IN.shapeData);
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
            
                        if (_EnableDashedOutline == 1)
                        {
                            float dashedEffect = generateDashedEffect(IN, _CustomTime, IN.shapeData.z / IN.shapeData.w, _DrawShape); // 传递图形类型

                            if (_DrawShape == 1)
                            {
                                color = half4(lerp(color.rgb, _OutlineColor.rgb, dashedEffect), lerp(_OutlineColor.a, color.a, dashedEffect));
                            }
                            else if (_DrawShape == 3)
                            {
                                color = half4(lerp(color.rgb, _OutlineColor.rgb, dashedEffect), lerp(_OutlineColor.a, color.a, dashedEffect));
                            }
                            else
                            {
                                color = half4(lerp(_OutlineColor.rgb, color.rgb, lerpFac), lerp(_OutlineColor.a, color.a, lerpFac));
                            }
                        }
                        else
                        {
                            // 普通描边效果
                            color = half4(lerp(_OutlineColor.rgb, color.rgb, lerpFac), lerp(_OutlineColor.a, color.a, lerpFac));
                        }
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
    CustomEditor "LLAFramework.UI.ImageExtensions.Editor.ImageShaderGUI"
}