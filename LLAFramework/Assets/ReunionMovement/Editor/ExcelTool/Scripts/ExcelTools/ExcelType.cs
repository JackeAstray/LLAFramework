using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LLAFramework.EditorTools
{
    /// <summary>
    /// 字段类型
    /// </summary>
    public enum FieldTypes
    {
        //未知类型
        Unknown,
        //未知类型表
        UnknownList,
        Bool,
        Int,
        //Int数组
        Ints,

        Float,
        //Float数组
        Floats,

        Double,
        //Double数组
        Doubles,

        Long,
        //Long数组
        Longs,

        //向量
        Vector2,
        Vector3,
        Vector4,
        //矩阵
        Rect,
        //颜色
        Color,
        String,
        Strings,
        //自定义类型
        CustomType,
        //自定义类型数组
        CustomTypeList
    }
}
