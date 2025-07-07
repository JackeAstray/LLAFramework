using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Vectors扩展
/// </summary>
public static class VectorExtensions
{
    /// <summary>
    /// 在指定的容器中找到距离最近的位置
    /// </summary>
    /// <param name="position">自己的位置</param>
    /// <param name="otherPositions">其他对象的位置</param>
    /// <returns>最近的</returns>
    public static Vector3 GetClosest(this Vector3 position, IEnumerable<Vector3> otherPositions)
    {
        Vector3 closest = Vector3.zero;
        float shortestDistance = Mathf.Infinity;
        Vector3 difference;

        foreach (var otherPosition in otherPositions)
        {
            difference = position - otherPosition;
            float distance = difference.sqrMagnitude;

            if (distance < shortestDistance)
            {
                closest = otherPosition;
                shortestDistance = distance;
            }
        }

        return closest;
    }

    /// <summary>
    /// vector3转vector2
    /// </summary>
    /// <param name="v"></param>
    /// <returns></returns>
    public static Vector2 XY(this Vector3 v) => new Vector2(v.x, v.y);

    public static void WithX(this Vector3 v, float x) => v.x = x;

    public static void WithY(this Vector3 v, float y) => v.y = y;

    public static void WithZ(this Vector3 v, float z) => v.z = z;

    public static void WithX(this Vector2 v, float x) => v.x = x;

    public static void WithY(this Vector2 v, float y) => v.y = y;

    public static void WithAddX(this Vector3 v, float x) => v.x += x;

    public static void WithAddY(this Vector3 v, float y) => v.y += y;

    public static void WithAddZ(this Vector3 v, float z) => v.z += z;

    public static void WithAddX(this Vector2 v, float x) => v.x += x;

    public static void WithAddY(this Vector2 v, float y) => v.y += y;

    /// <summary>
    /// 将向量旋转指定角度
    /// </summary>
    /// <param name="vector">要旋转的向量</param>
    /// <param name="angleInDeg">角度（度）</param>
    /// <returns>旋转向量</returns>
    public static Vector2 Rotate(this Vector2 vector, float angleInDeg)
    {
        float angleInRad = Mathf.Deg2Rad * angleInDeg;
        float cosAngle = Mathf.Cos(angleInRad);
        float sinAngle = Mathf.Sin(angleInRad);

        float x = vector.x * cosAngle - vector.y * sinAngle;
        float y = vector.x * sinAngle + vector.y * cosAngle;

        return new Vector2(x, y);
    }

    /// <summary>
    /// 将向量围绕目标点旋转指定角度
    /// </summary>
    /// <param name="vector"></param>
    /// <param name="angleInDeg"></param>
    /// <param name="axisPosition"></param>
    /// <returns></returns>
    public static Vector2 RotateAround(this Vector2 vector, float angleInDeg, Vector2 axisPosition)
    {
        return (vector - axisPosition).Rotate(angleInDeg) + axisPosition;
    }


    /// <summary>
    /// 将向量旋转90度
    /// </summary>
    public static Vector2 Rotate90(this Vector2 vector)
    {
        return new Vector2(-vector.y, vector.x);
    }

    /// <summary>
    /// 将向量旋转180度
    /// </summary>
    public static Vector2 Rotate180(this Vector2 vector)
    {
        return new Vector2(-vector.x, -vector.y);
    }

    /// <summary>
    /// 将向量旋转270度
    /// </summary>
    public static Vector2 Rotate270(this Vector2 vector)
    {
        return new Vector2(vector.y, -vector.x);
    }

    /// <summary>
    /// 计算一个点在指定轴上的最近点
    /// </summary>
    /// <param name="axisDirection">轴的方向</param>
    /// <param name="point">要计算的空间点</param>
    /// <returns>点在轴上的最近点</returns>
    public static Vector3 NearestPointOnAxis(this Vector3 axisDirection, Vector3 point)
    {
        // 确保轴的方向是单位向量
        axisDirection.Normalize();

        // 计算点和轴方向的点积，得到点在轴上的投影长度
        var d = Vector3.Dot(point, axisDirection);

        // 将点积乘以轴的方向，得到点在轴上的最近点
        return axisDirection * d;
    }

    /// <summary>
    /// 计算一个点在给定直线上的最近点
    /// </summary>
    /// <param name="lineDirection">直线的方向向量</param>
    /// <param name="point">要计算的空间点</param>
    /// <param name="pointOnLine">用于唯一确定直线的位置，是直线上的一个已知点</param>
    /// <returns>点在直线上的最近点</returns>
    public static Vector3 NearestPointOnLine(this Vector3 lineDirection, Vector3 point, Vector3 pointOnLine)
    {
        // 确保直线的方向是单位向量
        lineDirection.Normalize();

        // 计算点和直线上的点的差，然后和直线方向的点积，得到点在直线上的投影长度
        var d = Vector3.Dot(point - pointOnLine, lineDirection);

        // 将点积乘以直线的方向，然后加上直线上的点，得到点在直线上的最近点
        return pointOnLine + lineDirection * d;
    }

    /// <summary>
    /// 计算一个点在给定平面上的最近点
    /// </summary>
    /// <param name="v"></param>
    /// <returns></returns>
    public static bool IsFinite(this Vector2 v)
    {
        return v.x.IsFinite() && v.y.IsFinite();
    }

    /// <summary>
    /// 计算一个点在给定平面上的最近点
    /// </summary>
    /// <param name="f"></param>
    /// <returns></returns>
    private static bool IsFinite(this float f)
    {
        return !float.IsNaN(f) && !float.IsInfinity(f);
    }
}