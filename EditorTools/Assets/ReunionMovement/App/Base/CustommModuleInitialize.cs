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
        /// 获取游戏框架模块优先级。
        /// </summary>
        /// <remarks>优先级较高的模块会优先轮询，并且关闭操作会后进行。</remarks>
        internal virtual int Priority
        {
            get
            {
                return 0;
            }
        }

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
        void Update(float elapseSeconds, float realElapseSeconds);

        /// <summary>
        /// 清除数据
        /// </summary>
        void ClearData();
    }
}