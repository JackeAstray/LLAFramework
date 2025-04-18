//根据屏幕宽高展uv
float2 screen_aspect_ratio(float2 uv, float ratio)
{
    uv.x -= 0.5;
    uv.x *= ((_ScreenParams.x) / (_ScreenParams.y)) * ratio;
    uv.x += 0.5;

    uv.y *= ratio;

    return uv;
}

// 使用frac函数获取uv的小数部分
float2 fract(float2 pos)
{
    return frac(pos);
}

// 线性插值，返回一个介于color1、color2之间的颜色值。
float4 mix(float4 color1, float4 color2, float t)
{
    return lerp(color1, color2, t);
}

// 阴影的模糊或偏移进行额外处理
float2 applyShadowOffset(float2 uv, float2 offset)
{
    return uv + offset;
}

// 计算噪声值
inline float noise(uint2 n)
{
    static const float g = 1.32471795724474602596;
    static const float a1 = 1.0 / g;
    static const float a2 = 1.0 / (g * g);
    return frac(a1 * n.x + a2 * n.y);
}

// 从裁剪区域的相对UV转换到全局UV。
float2 UnCropUV(float2 uvRelativeToCropped, float4 cropRegion)
{
    return lerp(cropRegion.xy, cropRegion.zw, uvRelativeToCropped);
}

// 从全局UV转换到裁剪区域的相对 UV。
float2 CropUV(float2 uvRelativeToUnCropped, float4 cropRegion)
{
    return (uvRelativeToUnCropped - cropRegion.xy) / (cropRegion.zw - cropRegion.xy);
}