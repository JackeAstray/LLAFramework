using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameLogic
{
    /// <summary>
    /// 自定义模块初始化
    /// </summary>
    public interface CustommModuleInitialize
    {
        /// <summary>
        /// 初始化的操作进度
        /// </summary>
        double InitProgress { get; }

        /// <summary>
        /// 异步初始化
        /// </summary>
        /// <returns></returns>
        IEnumerator Init();

        /// <summary>
        /// 清除数据
        /// </summary>
        void ClearData();
    }
}