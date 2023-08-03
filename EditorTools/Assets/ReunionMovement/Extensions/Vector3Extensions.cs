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
    /// <param name="position">World position.</param>
    /// <param name="otherPositions">Other world positions.</param>
    /// <returns>Closest position.</returns>
    public static Vector3 GetClosest(this Vector3 position, IEnumerable<Vector3> otherPositions)
    {
        var closest = Vector3.zero;
        var shortestDistance = Mathf.Infinity;

        foreach (var otherPosition in otherPositions)
        {
            var distance = (position - otherPosition).sqrMagnitude;

            if (distance < shortestDistance)
            {
                closest = otherPosition;
                shortestDistance = distance;
            }
        }

        return closest;
    }

    //v3 -> v2(x,y)
    public static Vector2 XY(this Vector3 v) => new Vector2(v.x, v.y);

    //根据新的x返回新的vector3
    public static Vector3 WithX(this Vector3 v, float x) => new Vector3(x, v.y, v.z);

    //根据新的y返回新的vector3
    public static Vector3 WithY(this Vector3 v, float y) => new Vector3(v.x, y, v.z);

    //根据新的z返回新的vector3
    public static Vector3 WithZ(this Vector3 v, float z) => new Vector3(v.x, v.y, z);

    //根据新的x返回新的vector2
    public static Vector2 WithX(this Vector2 v, float x) => new Vector2(x, v.y);

    //根据新的y返回新的vector2
    public static Vector2 WithY(this Vector2 v, float y) => new Vector2(v.x, y);

    //根据新的z返回新的vector3
    public static Vector3 WithZ(this Vector2 v, float z) => new Vector3(v.x, v.y, z);

    //x相加并返回新的vector3
    public static Vector3 WithAddX(this Vector3 v, float x) => new Vector3(v.x + x, v.y, v.z);

    //y相加并返回新的vector3
    public static Vector3 WithAddY(this Vector3 v, float y) => new Vector3(v.x, v.y + y, v.z);

    //z相加并返回新的vector3
    public static Vector3 WithAddZ(this Vector3 v, float z) => new Vector3(v.x, v.y, v.z + z);

    //x相加并返回新的vector2
    public static Vector2 WithAddX(this Vector2 v, float x) => new Vector2(v.x + x, v.y);

    //y相加并返回新的vector2
    public static Vector2 WithAddY(this Vector2 v, float y) => new Vector2(v.x, v.y + y);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="axisDirection">轴方向上的单位向量（例如，定义穿过零的直线）</param>
    /// <param name="point">在线查找最近的点</param>
    /// <param name="isNormalized">是否归一化</param>
    /// <returns></returns>
    public static Vector3 NearestPointOnAxis(this Vector3 axisDirection, Vector3 point, bool isNormalized = false)
    {
        if (!isNormalized)
            axisDirection.Normalize();
        var d = Vector3.Dot(point, axisDirection);
        return axisDirection * d;
    }

    /// <summary>
    /// 线上最近的点
    /// </summary>
    /// <param name="lineDirection">线方向的单位向量</param>
    /// <param name="point">在线查找最近的点</param>
    /// <param name="pointOnLine">线上的一个点（允许我们定义空间中的实际线）</param>
    /// <param name="isNormalized">是否归一化</param>
    /// <returns></returns>
    public static Vector3 NearestPointOnLine(
        this Vector3 lineDirection, Vector3 point, Vector3 pointOnLine, bool isNormalized = false)
    {
        if (!isNormalized)
            lineDirection.Normalize();
        var d = Vector3.Dot(point - pointOnLine, lineDirection);
        return pointOnLine + lineDirection * d;
    }
}