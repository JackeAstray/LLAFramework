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
    /// 计算一个点在给定轴上的最近点
    /// </summary>
    /// <param name="axisDirection">轴的方向</param>
    /// <param name="point">要计算的点</param>
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
    /// <param name="lineDirection">直线的方向</param>
    /// <param name="point">要计算的点</param>
    /// <param name="pointOnLine">直线上的一个点</param>
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
}