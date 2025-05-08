const float pi = 3.14159;
const float pi2 = 6.28318; // 2 * pi
const float pi3 = 9.42478; // 3 * pi

//根据屏幕宽高展uv
float2 screenAspectRatio(float2 uv, float ratio)
{
    uv.x -= 0.5;
    uv.x *= ((_ScreenParams.x) / (_ScreenParams.y)) * ratio;
    uv.x += 0.5;

    uv.y *= ratio;

    return uv;
}

// 旋转uv
float2 rotateUV(float2 uv, float rotation, float2 mid)
{
    return float2(
        cos(rotation) * (uv.x - mid.x) + sin(rotation) * (uv.y - mid.y) + mid.x,
        cos(rotation) * (uv.y - mid.y) - sin(rotation) * (uv.x - mid.x) + mid.y
    );
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
float2 unCropUV(float2 uvRelativeToCropped, float4 cropRegion)
{
    return lerp(cropRegion.xy, cropRegion.zw, uvRelativeToCropped);
}

// 从全局UV转换到裁剪区域的相对 UV。
float2 cropUV(float2 uvRelativeToUnCropped, float4 cropRegion)
{
    return (uvRelativeToUnCropped - cropRegion.xy) / (cropRegion.zw - cropRegion.xy);
}

// 将值从输入范围 [inMin, inMax] 映射到输出范围 [outMin, outMax]
float remap(float value, float2 inRange, float2 outRange)
{
    return outRange.x + (value - inRange.x) * (outRange.y - outRange.x) / (inRange.y - inRange.x);
}

// 计算边缘的辅助函数
float getEdge(float dir, float edgeStart, float scale)
{
    return step(dir * scale, edgeStart);
}

// 生成虚线的辅助函数
float makeDashed(float inVal, float edgeStart)
{
    return step(inVal, edgeStart) + step((1.0 - inVal), edgeStart);
}

// 生成虚线图案
float generateDashedPattern(float x, float l, float r)
{
    // 将 x 限制在 [0, l] 范围内
    x = fmod(x, l);

    // 计算虚线的各个阶段
    float a = l * r / 2.0; // 实线开始
    float b = l - a; // 空白开始

    // 根据 x 的位置计算值
    if (x < a)
    {
        return 1.0; // 实线部分
    }
    else if (x < b)
    {
        return 0.0; // 空白部分
    }
    else
    {
        return 1.0; // 实线部分
    }
}