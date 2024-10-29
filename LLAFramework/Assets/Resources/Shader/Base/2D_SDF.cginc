//Signed Distance Function 2D
#ifndef SDF_2D
#define SDF_2D

//形状开始------------------------
//圆形
float circle(float2 _samplePosition, float _radius)
{
    return length(_samplePosition) - _radius;
}

//矩形
float rectanlge(float2 _samplePosition, float _width, float _height)
{
    float2 d = abs(_samplePosition) - float2(_width, _height) / 2.0;
    float sdf = min(max(d.x, d.y), 0.0) + length(max(d, 0.0));
    return sdf;
}

//菱形 Credit: https://www.shadertoy.com/view/XdXcRB | MIT License
float ndot(float2 a, float2 b)
{
    return a.x * b.x - a.y * b.y;
}

float sdRhombus(float2 p, float2 b)
{
    float2 q = abs(p);
    float h = clamp((-2.0 * ndot(q, b) + ndot(b, b)) / dot(b, b), -1.0, 1.0);
    float d = length(q - 0.5 * b * float2(1.0 - h, 1.0 + h));
    return d * sign(q.x * b.y + q.y * b.x - b.x * b.y);
}
//EndCredit

//等边三角形 Credit: https://www.shadertoy.com/view/MldcD7 | MIT License
float sdTriangleIsosceles(float2 p, float2 q)
{
    p.x = abs(p.x);
    float2 a = p - q * clamp(dot(p, q) / dot(q, q), 0.0, 1.0);
    float2 b = p - q * float2(clamp(p.x / q.x, 0.0, 1.0), 1.0);
    float s = -sign(q.y);
    float2 d = min(float2(dot(a, a), s * (p.x * q.y - p.y * q.x)), float2(dot(b, b), s * (p.y - q.y)));
    return -sqrt(d.x) * sign(d.y);
}
//EndCredit

//N星形多边形 Credit: https://www.shadertoy.com/view/3tSGDy
float sdNStarPolygon(in float2 p, in float r, in float n, in float m) // m=[2,n]
{
    float an = 3.141593 / float(n);
    float en = 3.141593 / m;
    float2 acs = float2(cos(an), sin(an));
    float2 ecs = float2(cos(en), sin(en));
    float bn = abs(atan2(p.x, p.y)) % (2.0 * an) - an;
    p = length(p) * float2(cos(bn), abs(sin(bn)));
    p -= r * acs;
    p += ecs * clamp(-dot(p, ecs), 0.0, r * acs.y / ecs.y);
    return length(p) * sign(p.x);
}
//EndCredit

//心 Credit: https://www.shadertoy.com/view/3tyBzV
float dot2(float2 v)
{
    return dot(v, v);
}

float sdHeart(float2 p, float scale)
{
    p.x = abs(p.x) / scale;
    p.y = p.y / scale;

    if (p.y + p.x > 1.0)
    {
        return sqrt(dot2(p - float2(0.25, 0.75))) - sqrt(2.0) / 4.0;
    }
    return sqrt(min(dot2(p - float2(0.00, 1.00)), dot2(p - 0.5 * max(p.x + p.y, 0.0)))) * sign(p.x - p.y);
}
//EndCredit
//https://www.shadertoy.com/view/DtjXDG
//https://www.shadertoy.com/view/7stcR4

//https://www.shadertoy.com/view/4ssyzj
//https://www.shadertoy.com/view/wdlGWn
//https://www.shadertoy.com/view/wdtcDn
//https://www.shadertoy.com/view/ldVXRt
//https://www.shadertoy.com/view/llXfRl
//https://www.shadertoy.com/view/tscSz7
//https://www.shadertoy.com/view/fdSBDD
//https://www.shadertoy.com/view/fd3Szl
//https://www.shadertoy.com/view/4dVXWy
//https://www.shadertoy.com/view/Xd3cR8
//形状结束------------------------

//用于进一步的图形处理，例如渲染或者着色
float sampleSdf(float _sdf, float _offset)
{
    float sdf = saturate(-_sdf * _offset);
    return sdf;
}

// 用于计算一个2D的有向距离场（SDF）条纹样本的函数。
// 这是一个在计算机图形学中常用的技术，用于创建复杂的2D和3D形状。
float sampleSdfStrip(float _sdf, float _stripWidth, float _offset)
{
    float l = (_stripWidth + 1.0 / _offset) / 2.0;
    return saturate((l - distance(-_sdf, l)) * _offset);
}

//（最大值）通常用于合并两个形状，结果是两个形状的并集
float sdfUnion(float _a, float _b)
{
    return max(_a, _b);
}

//（最小值）通常用于找到两个形状的交集
float sdfIntersection(float _a, float _b)
{
    return min(_a, _b);
}

//（最大值）通常从一个形状中减去另一个形状，结果是两个形状的差集
float sdfDifference(float _a, float _b)
{
    return max(_a, -_b);
}

// 将一个范围内的值映射到另一个范围
float map(float value, float start1, float stop1, float start2, float stop2)
{
    return start2 + (stop2 - start2) * ((value - start1) / (stop1 - start1));
}

// 十字
float sdCross(float2 p, float2 b, float r)
{
    p = abs(p);
    p = (p.y > p.x) ? p.yx : p.xy;
    
    float2 q = p - b;
    float k = max(q.y, q.x);
    float2 w = (k > 0.0) ? q : float2(b.y - p.x, -k);
    float d = length(max(w, 0.0));
    return ((k > 0.0) ? d : -d) + r;
}

// 馅饼
float sdPie(float2 p, float2 c, in float r)
{
    p.x = abs(p.x);
    float l = length(p) - r;
    float m = length(p - c * clamp(dot(p, c), 0.0, r));
    return max(l, m * sign(c.y * p.x - c.x * p.y));
}

// 环形
float sdRing(float2 p, float2 n, float r, float th)
{
    p.x = abs(p.x);
    
    float2x2 rotationMatrix = float2x2(n.x, n.y, -n.y, n.x);
    p = mul(rotationMatrix, p);

    return max(abs(length(p) - r) - th * 0.5, length(float2(p.x, max(0.0, abs(r - p.y) - th * 0.5))) * sign(p.x));
}

// 月亮
float sdMoon(float2 p, float d, float ra, float rb)
{
    p.y = abs(p.y);

    float a = (ra * ra - rb * rb + d * d) / (2.0 * d);
    float b = sqrt(max(ra * ra - a * a, 0.0));
    if (d * (p.x * b - p.y * a) > d * d * max(b - p.y, 0.0))
    {
        return length(p - float2(a, b));
    }

    return max((length(p) - ra), -(length(p - float2(d, 0)) - rb));
}

// 胆囊
float sdVesica(float2 p, float r, float d)
{
    p = abs(p);

    float b = sqrt(r * r - d * d); // can delay this sqrt by rewriting the comparison
    return ((p.y - b) * d > p.x * b) ? length(p - float2(0.0, b)) * sign(d) : length(p - float2(-d, 0.0)) - r;
}

// 水滴十字
float sdBlobbyCross(float2 pos, float he)
{
    pos = abs(pos);
    pos = float2(abs(pos.x - pos.y), 1.0 - pos.x - pos.y) / sqrt(2.0);
    
    float p = (he - pos.y - 0.25 / he) / (6.0 * he);
    float q = pos.x / (he * he * 16.0);
    float h = q * q - p * p * p;
    
    float x;
    if (h > 0.0)
    {
        float r = sqrt(h);
        x = pow(q + r, 1.0 / 3.0) - pow(abs(q - r), 1.0 / 3.0) * sign(r - q);
    }
    else
    {
        float r = sqrt(p);
        x = 2.0 * r * cos(acos(q / (p * r)) / 3.0);
    }
    x = min(x, sqrt(2.0) / 2.0);
    
    float2 z = float2(x, he * (1.0 - 2.0 * x * x)) - pos;
    return length(z) * sign(z.y);
}

//方圆
float sdSquircle(float2 p, float n)
{
    p = abs(p);
    if (p.y > p.x)
        p = p.yx;
    n = 2.0 / n;

    float xa = 0.0, xb = 6.283185 / 8.0;
    for (int i = 0; i < 6; i++)
    {
        float x = 0.5 * (xa + xb);
        float c = cos(x);
        float s = sin(x);
        float cn = pow(c, n);
        float sn = pow(s, n);
        float y = (p.x - cn) * cn * s * s - (p.y - sn) * sn * c * c;

        if (y < 0.0)
            xa = x;
        else
            xb = x;
    }

    float2 qa = pow(float2(cos(xa), sin(xa)), n);
    float2 qb = pow(float2(cos(xb), sin(xb)), n);
    float2 pa = p - qa, ba = qb - qa;
    float h = clamp(dot(pa, ba) / dot(ba, ba), 0.0, 1.0);
    return length(pa - ba * h) * sign(pa.x * ba.y - pa.y * ba.x);
}
//方圆
float approx_sdSquircle(float2 p, float n)
{
    p = abs(p);
    if (p.y > p.x)
        p = p.yx;
    float w = pow(p.x, n) + pow(p.y, n);
    float b = 2.0 * n - 2.0;
    float a = 1.0 - 1.0 / n;
    return (w - pow(w, a)) * rsqrt(pow(p.x, b) + pow(p.y, b));
}
#endif