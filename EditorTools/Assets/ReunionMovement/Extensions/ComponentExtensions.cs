using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// Component的扩展
/// </summary>
public static class ComponentExtensions
{
    /// <summary>
    /// 将一个组件附加到给定组件的游戏对象
    /// </summary>
    /// <param name="component">Component.</param>
    /// <returns>Newly attached component.</returns>
    public static T AddComponent<T>(this Component component) where T : Component
    {
        return component.gameObject.AddComponent<T>();
    }

    /// <summary>
    /// 获取附加到给定组件的游戏对象的组件
    /// 如果没有找到，则会附加一个新的并返回
    /// </summary>
    /// <param name="component">Component.</param>
    /// <returns>Previously or newly attached component.</returns>
    public static T GetOrAddComponent<T>(this Component component) where T : Component
    {
        return component.GetComponent<T>() ?? component.AddComponent<T>();
    }

    /// <summary>
    /// 检查组件的游戏对象是否附加了类型为T的组件
    /// </summary>
    /// <param name="component">Component.</param>
    /// <returns>True when component is attached.</returns>
    public static bool HasComponent<T>(this Component component) where T : Component
    {
        return component.GetComponent<T>() != null;
    }

    /// <summary>
    /// 搜索子物体组件
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="go"></param>
    /// <param name="subnode"></param>
    /// <returns></returns>
    public static T Get<T>(Component go, string subnode) where T : Component
    {
        return go.transform.Find(subnode).GetComponent<T>();
    }
}