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

float4 mix(float4 color1, float4 color2, float t)
{
    return lerp(color1, color2, t);
}