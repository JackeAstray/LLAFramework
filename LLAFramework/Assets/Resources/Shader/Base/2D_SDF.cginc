#ifndef SDF_2D
#define SDF_2D

// 计算向量的平方模长
float dot2(float2 v)
{
    return dot(v, v);
}

// 模
float mod(float x, float y)
{
    return x - y * floor(x / y);
}

// 用于计算两个二维向量的“伪点积”
float ndot(float2 a, float2 b)
{
    return a.x * b.x - a.y * b.y;
}

// 将一个范围内的值映射到另一个范围
float map(float value, float start1, float stop1, float start2, float stop2)
{
    return start2 + (stop2 - start2) * ((value - start1) / (stop1 - start1));
}

// 用于进一步的图形处理，例如渲染或者着色
float sampleSdf(float sdf, float offset)
{
    float tempSdf = saturate(-sdf * offset);
    return tempSdf;
}

// 用于计算一个2D的有向距离场（SDF）条纹样本的函数。
// 这是一个在计算机图形学中常用的技术，用于创建复杂的2D和3D形状。
float sampleSdfStrip(float sdf, float stripWidth, float offset)
{
    float l = (stripWidth + 1.0 / offset) / 2.0;
    return saturate((l - distance(-sdf, l)) * offset);
}

// （最大值）通常用于合并两个形状，结果是两个形状的并集
float sdfUnion(float a, float b)
{
    return max(a, b);
}

// （最小值）通常用于找到两个形状的交集
float sdfIntersection(float a, float b)
{
    return min(a, b);
}

//（最大值）通常从一个形状中减去另一个形状，结果是两个形状的差集
float sdfDifference(float a, float b)
{
    return max(a, -b);
}

// 圆形 （位置、半径）
float sdCircle(float2 p, float r)
{
    return length(p) - r;
}

// 矩形 （位置、宽、高）
float sdRectanlge(float2 p, float w, float h)
{
    float2 d = abs(p) - float2(w, h) / 2.0;
    float sdf = min(max(d.x, d.y), 0.0) + length(max(d, 0.0));
    return sdf;
}

// 倒角长方形
float sdChamferBox(float2 p, float2 b, float chamfer)
{
    p = abs(p) - b;

    p = (p.y > p.x) ? p.yx : p.xy;
    p.y += chamfer;
    
    const float k = 1.0 - sqrt(2.0);
    if (p.y < 0.0 && p.y + p.x * k < 0.0)
        return p.x;
    
    if (p.x < p.y)
        return (p.x + p.y) * sqrt(0.5);
    
    return length(p);
}

// 平行四边形
float sdParallelogram(float2 p, float wi, float he, float sk)
{
    float2 e = float2(sk, he);
    float e2 = sk * sk + he * he;

    float da = abs(p.x * e.y - p.y * e.x) - wi * he;
    float db = abs(p.y) - e.y;
    if (max(da, db) < 0.0) // 内部
    {
        return max(da * rsqrt(e2), db);
    }
    else // 外部
    {
        float f = clamp(p.y / e.y, -1.0, 1.0);
        float g = clamp(p.x - e.x * f, -wi, wi);
        float h = clamp(((p.x - g) * e.x + p.y * e.y) / e2, -1.0, 1.0);
        return length(p - float2(g + e.x * h, e.y * h));
    }
}

// 菱形
float sdRhombus(float2 p, float2 b)
{
    float2 q = abs(p);
    float h = clamp((-2.0 * ndot(q, b) + ndot(b, b)) / dot(b, b), -1.0, 1.0);
    float d = length(q - 0.5 * b * float2(1.0 - h, 1.0 + h));
    return d * sign(q.x * b.y + q.y * b.x - b.x * b.y);
}

//等边三角形
float sdTriangleIsosceles(float2 p, float2 q)
{
    p.x = abs(p.x);
    float2 a = p - q * clamp(dot(p, q) / dot(q, q), 0.0, 1.0);
    float2 b = p - q * float2(clamp(p.x / q.x, 0.0, 1.0), 1.0);
    float s = -sign(q.y);
    float2 d = min(float2(dot(a, a), s * (p.x * q.y - p.y * q.x)), float2(dot(b, b), s * (p.y - q.y)));
    return -sqrt(d.x) * sign(d.y);
}

// 引入圆角等边三角形 SDF 与角度重复函数
float sdTriangleIsoscelesRounded(in float2 p, in float2 q, float rounding)
{
    float W = 2.0 * q.x;
    float H = q.y;
    float r = W * H / (W + 2.0 * sqrt(0.25 * W * W + H * H));
    float s = 1.0 - rounding / r;
    float Wp = W * s;
    float Hp = H * s;
    float y_off = (H - Hp) - rounding;
    if (rounding >= r)
    {
        return length(p - float2(0, H - r)) - r;
    }
    return sdTriangleIsosceles(p - float2(0, y_off), float2(0.5 * Wp, Hp)) - rounding;
}

// N星形多边形
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

// 心 (位置、比例)
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

// 方圆形
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

// 线
float sdLine(float2 p, float2 a, float2 b)
{
    float2 pa = p - a, ba = b - a;
    float h = clamp(dot(pa, ba) / dot(ba, ba), 0.0, 1.0);
    return length(pa - ba * h);
}

float2 opRepAng(float2 p, float theta, float offset)
{
    float a = atan2(p.y, p.x) - offset;
    a = mod(a + 0.5 * theta, theta) - 0.5 * theta;
    return length(p) * float2(cos(a), sin(a));
}
#endif