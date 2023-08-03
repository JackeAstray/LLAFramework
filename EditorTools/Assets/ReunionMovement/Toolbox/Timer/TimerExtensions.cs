using System;
using UnityEngine;

/// <summary>
/// 包含与<see cref=“Timer”/>相关的扩展方法.
/// </summary>
namespace GameLogic
{
    public static class TimerExtensions
    {
        /// <summary>
        /// 将计时器连接到行为上。如果行为在计时器完成之前被破坏，例如通过场景更改，则计时器回调将不会执行。
        /// </summary>
        /// <param name="behaviour">将此计时器连接到的行为。</param>
        /// <param name="duration">计时器启动前等待的持续时间。</param>
        /// <param name="onComplete">计时器超时时要运行的操作。</param>
        /// <param name="onUpdate">用于调用计时器的每个刻度的函数。取自当前周期开始以来经过的秒数。</param>
        /// <param name="isLooped">计时器是否应在执行后重新启动。</param>
        /// <param name="useRealTime">计时器是否使用实时（不受慢速移动或暂停的影响）或游戏时间（受时间刻度变化的影响）。</param>
        public static Timer AttachTimer(this MonoBehaviour behaviour,
                                        float duration,
                                        Action onComplete,
                                        Action<float> onUpdate = null,
                                        bool isLooped = false,
                                        bool useRealTime = false
        )
        {
            return Timer.Register(duration, onComplete, onUpdate, isLooped, useRealTime, behaviour);
        }
    }
}
