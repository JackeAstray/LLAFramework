using System;
using System.IO;
using UnityEngine;

/// <summary>
/// Transform扩展方法
/// </summary>
public static class TransformExtensions
{
    /// <summary>
    /// 使指定的多个GameObject成为子节点
    /// </summary>
    /// <param name="transform">Parent transform.</param>
    /// <param name="children">Game objects to make children.</param>
    public static void AddChildren(this Transform transform, GameObject[] children) =>
        Array.ForEach(children, child => child.transform.parent = transform);

    /// <summary>
    /// 使指定的多个Component成为子节点
    /// </summary>
    /// <param name="transform">Parent transform.</param>
    /// <param name="children">Components of game objects to make children.</param>
    public static void AddChildren(this Transform transform, Component[] children) =>
        Array.ForEach(children, child => child.transform.parent = transform);

    /// <summary>
    /// 重置子节点位置
    /// </summary>
    /// <param name="transform">Parent transform.</param>
    /// <param name="recursive">Also reset ancestor positions?</param>
    public static void ResetChildPositions(this Transform transform, bool recursive = false)
    {
        foreach (Transform child in transform)
        {
            child.position = Vector3.zero;

            if (recursive) child.ResetChildPositions(recursive);
        }
    }

    /// <summary>
    /// 设置子层级的layer
    /// </summary>
    /// <param name="transform">Parent transform.</param>
    /// <param name="layerName">Name of layer.</param>
    /// <param name="recursive">Also set ancestor layers?</param>
    public static void SetChildLayers(this Transform transform, string layerName, bool recursive = false)
    {
        var layer = LayerMask.NameToLayer(layerName);
        SetChildLayersHelper(transform, layer, recursive);
    }

    static void SetChildLayersHelper(Transform transform, int layer, bool recursive)
    {
        foreach (Transform child in transform)
        {
            child.gameObject.layer = layer;

            if (recursive) SetChildLayersHelper(child, layer, recursive);
        }
    }

    /// <summary>
    /// 设置position的x
    /// </summary>
    /// <param name="x">Value of x.</param>
    public static void SetX(this Transform transform, float x) =>
        transform.position = new Vector3(x, transform.position.y, transform.position.z);

    /// <summary>
    /// 设置position的y
    /// </summary>
    /// <param name="y">Value of y.</param>
    public static void SetY(this Transform transform, float y) =>
        transform.position = new Vector3(transform.position.x, y, transform.position.z);

    /// <summary>
    /// 设置position的z
    /// </summary>
    /// <param name="z">Value of z.</param>
    public static void SetZ(this Transform transform, float z) =>
        transform.position = new Vector3(transform.position.x, transform.position.y, z);

    /// <summary>
    /// 计算该物体的位置。 无论它位于顶部还是底部。 分别为-1和1。
    /// </summary>
    /// <returns></returns>
    public static int CloserEdge(this Transform transform, Camera camera, int width, int height)
    {
        //根据屏幕/相机的边缘点
        var worldPointTop = camera.ScreenToWorldPoint(new Vector3(width / 2, height));
        var worldPointBot = camera.ScreenToWorldPoint(new Vector3(width / 2, 0));

        //轴心点到屏幕边缘的距离
        var deltaTop = Vector2.Distance(worldPointTop, transform.position);
        var deltaBottom = Vector2.Distance(worldPointBot, transform.position);

        return deltaBottom <= deltaTop ? 1 : -1;
    }

    /// <summary>
    /// 搜索子物体组件
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="tf"></param>
    /// <param name="subnode"></param>
    /// <returns></returns>
    public static T Get<T>(this Transform tf, string subnode) where T : Component
    {
        if (tf != null)
        {
            Transform sub = tf.Find(subnode);
            if (sub != null) return sub.GetComponent<T>();
        }
        return null;
    }

    /// <summary>
    /// 清除所有子节点
    /// </summary>
    /// <param name="tf"></param>
    public static void ClearChild(this Transform tf)
    {
        if (tf == null) return;
        for (int i = tf.childCount - 1; i >= 0; i--)
        {
            GameObject.Destroy(tf.GetChild(i).gameObject);
        }
    }

    /// <summary>
    /// 将位置旋转缩放清零
    /// </summary>
    /// <param name="tf"></param>
    public static void ResetLocalTransform(this Transform tf)
    {
        tf.localPosition = Vector3.zero;
        tf.localRotation = Quaternion.identity;
        tf.localScale = Vector3.one;
    }
}