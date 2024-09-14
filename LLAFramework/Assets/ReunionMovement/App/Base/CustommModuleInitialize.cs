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
        /// 游戏框架模块轮询。
        /// </summary>
        /// <param name="elapseSeconds">逻辑流逝时间，以秒为单位。</param>
        /// <param name="realElapseSeconds">真实流逝时间，以秒为单位。(timeScale从上一帧到当前帧的独立间隔（秒）)</param>
        void UpdateTime(float elapseSeconds, float realElapseSeconds);

        /// <summary>
        /// 清除数据
        /// </summary>
        void ClearData();
    }
}