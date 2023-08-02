using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameLogic
{
    /// <summary>
    /// 日志等级。
    /// </summary>
    public enum GameFrameworkLogLevel : byte
    {
        /// <summary>
        /// 调试。
        /// </summary>
        Debug = 0,

        /// <summary>
        /// 信息。
        /// </summary>
        Info,

        /// <summary>
        /// 警告。
        /// </summary>
        Warning,

        /// <summary>
        /// 错误。
        /// </summary>
        Error,

        /// <summary>
        /// 严重错误。
        /// </summary>
        Fatal
    }

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
        Long,
        //Long数组
        Longs,
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