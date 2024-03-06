using GameLogic;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

/// <summary>
/// 扩展Unity的FindChild
/// </summary>
public static class FindExtensions
{
    #region GO
    /// <summary>
    /// 查找子项
    /// </summary>
    /// <param name="go"></param>
    /// <param name="id"></param>
    /// <param name="check_visible"></param>
    /// <param name="raise_error"></param>
    /// <returns></returns>
    public static GameObject FindChild(this GameObject go, string id, bool check_visible = false, bool raise_error = true)
    {
        if (!go)
        {
            if (raise_error)
            {
                Log.Error("查找失败，GameObject是空的！");
            }
            return null;
        }

        var t = FindChild(go.transform, id, check_visible, raise_error);
        return t?.gameObject;
    }
    #endregion

    #region GetComponent
    /// <summary>
    /// 查找子项组件
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="go"></param>
    /// <param name="id"></param>
    /// <param name="check_visible"></param>
    /// <param name="raise_error"></param>
    /// <returns></returns>
    public static T FindChild<T>(this GameObject go, string id, bool check_visible = false, bool raise_error = true) where T : Component
    {
        var t = FindChild(go.transform, id, check_visible, raise_error);
        return t?.GetComponent<T>();
    }

    /// <summary>
    /// 查找子项组件
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="go"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public static T FindInChild<T>(this GameObject go, string name = "") where T : Component
    {
        if (!go)
        {
            return null;
        }

        if (!string.IsNullOrEmpty(name) && !go.name.Contains(name))
        {
            return null;
        }

        var comp = go.GetComponent<T>();
        if (comp)
        {
            return comp;
        }

        return go.GetComponentsInChildren<T>().FirstOrDefault();
    }
    #endregion

    #region Transform
    /// <summary>
    /// 查找子项
    /// </summary>
    /// <param name="findTrans"></param>
    /// <param name="id"></param>
    /// <param name="check_visible">检查可见性</param>
    /// <param name="raise_error">抛出错误</param>
    /// <returns></returns>
    public static Transform FindChild(Transform findTrans, string id, bool check_visible = false, bool raise_error = true)
    {
        if (!findTrans)
        {
            if (raise_error)
            {
                Log.Error("查找失败. findTrans是空的!");
            }
            return null;
        }

        if (string.IsNullOrEmpty(id))
        {
            return null;
        }
        // 如果需要检查可见性，但是当前物体不可见
        if (check_visible && !findTrans.gameObject.activeInHierarchy)
        {
            return null;
        }
        if (id == ".")
        {
            return findTrans;
        }

        var ids = id.Split('/');

        foreach (var id1 in ids)
        {
            findTrans = FindChildDirect(findTrans, id1, check_visible);
            if (findTrans == null)
            {
                // 如果需要抛出错误
                if (raise_error)
                {
                    Log.Error($"查找子项失败, id:{id} ,parent={findTrans.name}");
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
    /// <param name="id"></param>
    /// <param name="check_visible"></param>
    /// <param name="raise_error"></param>
    /// <returns></returns>
    public static Transform FindChildTF(this Transform t, string id, bool check_visible = false, bool raise_error = true)
    {
        return FindChild(t, id, check_visible, raise_error);
    }

    /// <summary>
    /// 直接查找子项
    /// </summary>
    /// <param name="trans"></param>
    /// <param name="id"></param>
    /// <param name="check_visible"></param>
    /// <returns></returns>
    private static Transform FindChildDirect(Transform trans, string id, bool check_visible)
    {
        var queue = new Queue<Transform>();
        queue.Enqueue(trans);
        while (queue.Count > 0)
        {
            trans = queue.Dequeue();
            var t1 = trans.Find(id);
            if (t1 != null && (!check_visible || t1.gameObject.activeInHierarchy))
            {
                return t1;
            }

            foreach (Transform child in trans)
            {
                if (!check_visible || child.gameObject.activeInHierarchy)
                    queue.Enqueue(child);
            }
        }
        return null;
    }
    #endregion
}