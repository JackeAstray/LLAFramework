Shader "ReunionMovement/UI/Basic Procedural Image"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" { }
        _Color ("Tint", Color) = (1,1,1,1)
        
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
            
            #pragma multi_compile_local _ UNITY_UI_CLIP_RECT
            #pragma multi_compile_local _ UNITY_UI_ALPHACLIP
            
            #pragma multi_compile_local _ CIRCLE TRIANGLE RECTANGLE NSTAR_POLYGON HEART
            #pragma multi_compile_local _ STROKE OUTLINED OUTLINED_STROKE
            
            struct appdata_t
            {
                float4 vertex: POSITION;
                float4 color: COLOR;
                float2 uv0: TEXCOORD0;
                float2 uv1: TEXCOORD1;
                float2 uv2: TEXCOORD2;
                float2 uv3: TEXCOORD3;
                float3 normal: NORMAL;
                float4 tangent: TANGENT;
                
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };
            
            struct v2f
            {
                float4 vertex: SV_POSITION;
                fixed4 color: COLOR;
                float2 uv0: TEXCOORD0;
                float4 sizeData: TEXCOORD1;
                float4 strokeOutlineCornerData: TEXCOORD2;
                fixed4 outlineColor: COLOR1;
                float4 shapeData: TEXCOORD3;
                float4 worldPosition: TEXCOORD4;

                UNITY_VERTEX_OUTPUT_STEREO
            };
            
            sampler2D _MainTex;
            fixed4 _Color;
            fixed4 _TextureSampleAdd;
            float4 _ClipRect;
            float4 _MainTex_ST;
            
            //矩形
            #if RECTANGLE
                half rectangleScene(float4 _sizeData, float4 _shapeData, float _cornerStyle)
                {
                    float2 _texcoord = _sizeData.xy;
                    float2 _size = float2(_sizeData.z, _sizeData.w);
                    float4 radius = _shapeData;
                    half4 c  = half4(_texcoord, _size - _texcoord);
                    half rect = min(min(min(c.x, c.y), c.z), c.w);

                    bool4 cornerRects;
                    cornerRects.x = _texcoord.x < radius.x && _texcoord.y < radius.x;
                    cornerRects.y = _texcoord.x > _size.x - radius.y && _texcoord.y < radius.y;
                    cornerRects.z = _texcoord.x > _size.x - radius.z && _texcoord.y > _size.y - radius.z;
                    cornerRects.w = _texcoord.x < radius.w && _texcoord.y > _size.y - radius.w;

                    half cornerMask = any(cornerRects);

                    half4 cornerCircles;
                    cornerCircles.x = radius.x - length(_texcoord - radius.xx);//circle(_texcoord - radius.xx, radius.x);
                    cornerCircles.y = radius.y - length(_texcoord - half2(_size.x - radius.y, radius.y));//circle(_texcoord - half2(_size.x - radius.y, radius.y), radius.y);
                    cornerCircles.z = radius.z - length(_texcoord - (half2(_size.x, _size.y) - radius.zz));//circle(_texcoord - (half2(_size.x, _size.y) - radius.zz), radius.z);
                    cornerCircles.w = radius.w - length(_texcoord - half2(radius.w, _size.y - radius.w)); //circle(_texcoord - half2(radius.w, _size.y - radius.w), radius.w);

                    cornerCircles = min(max(cornerCircles, 0) * cornerRects, rect);

                    half corners = max(max(max(cornerCircles.x, cornerCircles.y), cornerCircles.z), cornerCircles.w);
                    corners = max(corners, 0.0) * cornerMask;

                    //return rect;
                    return rect*(cornerMask-1) - corners;
                }
            #endif
            
            //圆形
            #if CIRCLE
                float circleScene(float4 _sizeData, float4 _shapeData)
                {
                    float2 _texcoord = _sizeData.xy;
                    float2 _size = _sizeData.zw;
                    float width = _size.x;
                    float height = _size.y;
                    float radius = lerp(_shapeData.x, min(width, height) / 2.0, _shapeData.y);
                    float sdf = circle(_texcoord - float2(width / 2.0, height / 2.0), radius);
                    return sdf;
                }
            #endif
            
            //三角形
            #if TRIANGLE
                half triangleScene(float4 _sizeData, float4 _shapeData)
                {
                    float2 _texcoord = _sizeData.xy;
                    float2 _size = _sizeData.zw;
                    float width = _size.x;//_additionalData.z;
                    float height = _size.y;//_additionalData.w;
                    
                    half sdf = sdTriangleIsosceles(_texcoord - half2(width / 2.0, height), half2(width / 2.0, -height));

                    //return sdf;
                    
                    float3 _TriangleCornerRadius = max(_shapeData.xyz, float3(0.001, 0.001, 0.001));
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
            
            //N星形多边形
            #if NSTAR_POLYGON
                half nStarPolygonScene(float4 _sizeData, float4 _shapeData)
                {
                    float2 _texcoord = _sizeData.xy;
                    float width = _sizeData.z;
                    float height = _sizeData.w;
                    float size = height / 2 - _shapeData.y;
                    half str = sdNStarPolygon(_texcoord - half2(width / 2, height / 2), size, _shapeData.x, _shapeData.z) - _shapeData.y;
                    return str;
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
            
			float4 decode_0_1_16(float2 input){
			    float m = 65535.0;
			    float e = 256.0 / 255.0;
			    float n = 1.0 / m;
			    
			    float4 c = float4(input.x, input.x, input.y, input.y);
			    c.yw *= m;
			    c = frac(c);
			    c -= float4(c.y, 0.0, c.w, 0.0) * n;
			    return clamp(c * e, 0.0, 1.0);
			}
			
            //顶点着色器
            v2f vert(appdata_t v)
            {
                v2f OUT;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
                
                OUT.vertex = UnityObjectToClipPos(v.vertex);
                OUT.worldPosition = v.vertex;
                OUT.color = v.color * _Color;
                OUT.uv0 = v.uv0;
    
                
                float2 size = v.uv1;
                half strokeWidth = v.normal.y;
                half falloff = v.normal.z;
                
                float rotationData = v.uv3.x;
                half cornerStyle = v.uv3.y;
                
                half outlineWidth = v.normal.x;
                half4 outlineColor = v.tangent;

                float4 shapeData;
                #if CIRCLE
                    shapeData.xy = v.uv2.xy;
                #else
                    shapeData = decode_0_1_16(v.uv2) * min(size.x, size.y);
                #endif
                
               
                OUT.strokeOutlineCornerData = float4(strokeWidth, falloff, outlineWidth, cornerStyle);
                OUT.outlineColor = outlineColor;
                OUT.shapeData = shapeData;
                
                // Rotation Values
                half sign = rotationData > 0.0 ? 1 : -1;
                float f = abs(rotationData);
                float shapeRotation = frac(f) * 360.0 * sign;
                
                f = floor(f);
                float p = f / 100.0;
                float z = round(p); 
                p = frac(p) * 10.0;
                float y = round(p);
                p = frac(p) * 10.0;
                float x = round(p);
                
                half constrainRotation = x;
                half flipHorizontal = y;
                half flipVertical = z;
                
                
                shapeRotation = radians(shapeRotation);
                size = constrainRotation > 0.0 && frac(abs(shapeRotation) / 3.14159) > 0.1? float2(size.y, size.x) : size;
                
                float2 shapeUv = constrainRotation > 0 ? v.uv0 : v.uv0 * size;
                shapeUv = rotateUV(shapeUv, shapeRotation, constrainRotation > 0? float2(0.5, 0.5) : size * 0.5);
                shapeUv*= constrainRotation > 0.0? size : 1.0;
                
                shapeUv.x = lerp(shapeUv.x, abs(size.x - shapeUv.x), flipHorizontal);
                shapeUv.y = lerp(shapeUv.y, abs(size.y - shapeUv.y), flipVertical);
                
                OUT.sizeData = float4(shapeUv.x, shapeUv.y, size.x, size.y);
                
                #ifdef UNITY_HALF_TEXEL_OFFSET
                    OUT.vertex.xy += (_ScreenParams.zw - 1.0) * float2(-1.0, 1.0);
                #endif
                
                return OUT;
            }
            
            //片元着色器
            fixed4 frag(v2f IN): SV_Target
            {
                half4 color = IN.color;
                half2 texcoord = IN.uv0;
                color = (tex2D(_MainTex, texcoord) + _TextureSampleAdd) * color;
                
                float4 sizeData = IN.sizeData;
                float strokeWidth = IN.strokeOutlineCornerData.x;
                float falloff = IN.strokeOutlineCornerData.y;
                float outlineWidth = IN.strokeOutlineCornerData.z;
                half4 outlineColor = IN.outlineColor;
                float cornerStyle = IN.strokeOutlineCornerData.w;
                
                float4 shapeData = IN.shapeData;
                half pixelScale = clamp(1.0/falloff, 1.0/2048.0, 2048.0);
                
                float sdfData = 0;
                #if RECTANGLE
                    sdfData = rectangleScene(sizeData, shapeData, cornerStyle);
                #endif
                
                #if CIRCLE
                    sdfData = circleScene(sizeData, shapeData);
                #endif
                
                #if TRIANGLE
                    sdfData = triangleScene(sizeData, shapeData);
                #endif
                
                #if NSTAR_POLYGON
                    sdfData = nStarPolygonScene(sizeData, shapeData);
                #endif
                
                
                #if !OUTLINED && !STROKE && !OUTLINED_STROKE
                    half shape = sampleSdf(sdfData, pixelScale);
                    color.a *= shape;
                #endif
                #if STROKE
                    half shape = sampleSdfStrip(sdfData, strokeWidth, pixelScale);
                    color.a *= shape;
                #endif
                
                #if OUTLINED
                    float alpha = sampleSdf(sdfData, pixelScale);
                    float lerpFac = sampleSdf(sdfData + outlineWidth, pixelScale);
                    color = half4(lerp(outlineColor.rgb, color.rgb, lerpFac), lerp(outlineColor.a * color.a, color.a, lerpFac));
                    color.a *= alpha;
                #endif
                
                #if OUTLINED_STROKE
                    float alpha = sampleSdfStrip(sdfData, outlineWidth + strokeWidth, pixelScale);
                    float lerpFac = sampleSdfStrip(sdfData + outlineWidth, strokeWidth + falloff, pixelScale);
                    lerpFac = clamp(lerpFac, 0, 1);
                    color = half4(lerp(outlineColor.rgb, color.rgb, lerpFac), lerp(outlineColor.a * color.a, color.a, lerpFac));
                    color.a *= alpha;
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