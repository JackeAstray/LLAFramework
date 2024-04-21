//根据屏幕宽高展uv
float2 screen_aspect_ratio(float2 uv, float ratio)
{
    uv.x -= 0.5;
    uv.x *= ((_ScreenParams.x) / (_ScreenParams.y)) * ratio;
    uv.x += 0.5;

    uv.y *= ratio;

    return uv;
}