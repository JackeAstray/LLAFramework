using GameLogic;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

/// <summary>
/// Transform扩展方法
/// </summary>
public static class TransformExtensions
{
    /// <summary>
    /// 使指定的多个GameObject成为子节点
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="children"></param>
    public static void AddChildren(this Transform transform, GameObject[] children)
    {
        foreach (var child in children)
        {
            child.transform.parent = transform;
        }
    }

    /// <summary>
    /// 使指定的多个Component成为子节点
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="children"></param>
    public static void AddChildren(this Transform transform, Component[] children)
    {
        foreach (var child in children)
        {
            child.transform.parent = transform;
        }
    }

    /// <summary>
    /// 重置子节点位置
    /// </summary>
    /// <param name="transform">Parent transform.</param>
    /// <param name="recursive">Also reset ancestor positions?</param>
    public static void ResetChildPositions(this Transform transform, bool recursive = false)
    {
        foreach (Transform child in transform)
        {
            child.localPosition = Vector3.zero;

            if (recursive)
            {
                child.ResetChildPositions(true);
            }
        }
    }

    /// <summary>
    /// 设置子层级的layer
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="layerName"></param>
    /// <param name="recursive"></param>
    public static void SetChildLayers(this Transform transform, string layerName, bool recursive = false)
    {
        var layer = LayerMask.NameToLayer(layerName);

        foreach (Transform child in transform)
        {
            child.gameObject.layer = layer;

            if (recursive)
            {
                child.SetChildLayers(layerName, true);
            }
        }
    }

    /// <summary>
    /// 设置X轴位置
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="x"></param>
    public static void SetX(this Transform transform, float x)
    {
        transform.position = new Vector3(x, transform.position.y, transform.position.z);
    }

    /// <summary>
    /// 设置Y轴位置
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="y"></param>
    public static void SetY(this Transform transform, float y)
    {
        transform.position = new Vector3(transform.position.x, y, transform.position.z);
    }

    /// <summary>
    /// 设置Z轴位置
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="z"></param>
    public static void SetZ(this Transform transform, float z)
    {
        transform.position = new Vector3(transform.position.x, transform.position.y, z);
    }

    /// <summary>
    /// 计算该物体的位置。无论它位于顶部还是底部。分别为-1和1。
    /// </summary>
    /// <returns></returns>
    public static int CloserEdge(this Transform transform, Camera camera, int width, int height)
    {
        // 世界坐标转换为屏幕坐标
        var worldPointTop = camera.ScreenToWorldPoint(new Vector3(width / 2, height));
        var worldPointBot = camera.ScreenToWorldPoint(new Vector3(width / 2, 0));
        // 计算距离
        var deltaTop = Vector2.Distance(worldPointTop, transform.position);
        var deltaBottom = Vector2.Distance(worldPointBot, transform.position);

        return deltaBottom <= deltaTop ? 1 : -1;
    }

    /// <summary>
    /// 搜索子物体组件
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="tf"></param>
    /// <param name="objName"></param>
    /// <returns></returns>
    public static T Get<T>(this Transform tf, string objName) where T : Component
    {
        var sub = tf?.Find(objName);
        return sub?.GetComponent<T>();
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

    /// <summary>
    /// 查找子项
    /// </summary>
    /// <param name="findTrans"></param>
    /// <param name="objName"></param>
    /// <param name="check_visible">检查可见性</param>
    /// <param name="raise_error">抛出错误</param>
    /// <returns></returns>
    public static Transform Child(this Transform findTrans, string objName, bool check_visible = false, bool raise_error = true)
    {
        if (!findTrans)
        {
            if (raise_error)
            {
                Log.Error("查找失败. findTrans是空的!");
            }
            return null;
        }

        if (string.IsNullOrEmpty(objName))
        {
            return null;
        }
        // 如果需要检查可见性，但是当前物体不可见
        if (check_visible && !findTrans.gameObject.activeInHierarchy)
        {
            return null;
        }
        if (objName == ".")
        {
            return findTrans;
        }

        var ids = objName.Split('/');

        foreach (var id1 in ids)
        {
            findTrans = ChildDirect(findTrans, id1, check_visible);
            if (findTrans == null)
            {
                // 如果需要抛出错误
                if (raise_error)
                {
                    Log.Error($"查找子项失败, id:{objName} ,parent={findTrans.name}");
                }
                break;
            }
        }

        return findTrans;
    }

    /// <summary>
    /// 查找子项
    /// </summary>
    /// <param name="t"></param>
    /// <param name="objName"></param>
    /// <param name="check_visible"></param>
    /// <param name="raise_error"></param>
    /// <returns></returns>
    public static Transform ChildTF(this Transform t, string objName, bool check_visible = false, bool raise_error = true)
    {
        return Child(t, objName, check_visible, raise_error);
    }

    /// <summary>
    /// 查找子项
    /// </summary>
    /// <param name="trans"></param>
    /// <param name="objName"></param>
    /// <param name="check_visible"></param>
    /// <returns></returns>
    private static Transform ChildDirect(this Transform trans, string objName, bool check_visible)
    {
        if (trans == null || string.IsNullOrEmpty(objName))
        {
            return null;
        }

        var child = trans.Find(objName);
        if (child != null && (!check_visible || child.gameObject.activeInHierarchy))
        {
            return child;
        }

        if (!check_visible)
        {
            // 如果不检查可见性且未找到匹配项，直接返回null
            return null; 
        }

        foreach (Transform t in trans)
        {
            if (t.gameObject.activeInHierarchy)
            {
                var found = ChildDirect(t, objName, true);
                if (found != null)
                {
                    return found;
                }
            }
        }

        return null;
    }

    /// <summary>
    /// 获取从父节点到自己的完整路径
    /// </summary>
    /// <param name="transform"></param>
    /// <returns></returns>
    public static string GetRootPathName(this Transform transform)
    {
        if (transform == null)
        {
            return string.Empty;
        }

        StringBuilder path = new StringBuilder();
        BuildPath(transform, ref path);
        return path.ToString();
    }

    private static void BuildPath(Transform current, ref StringBuilder path)
    {
        if (current.parent != null)
        {
            BuildPath(current.parent, ref path);
            path.Append("/");
        }
        path.Append(current.name);
    }
}