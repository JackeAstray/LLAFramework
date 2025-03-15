using UnityEngine;
using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Object = UnityEngine.Object;

namespace GameLogic
{
    public class Timer
    {
        #region 公共 属性/字段
        public float duration { get; private set; }                                                 // 计时器持续时间
        public bool isLooped { get; set; }                                                          // 是否循环
        public bool isCompleted { get; private set; }                                               // 是否完成
        public bool usesRealTime { get; private set; }                                              // 是否使用真实时间
        public bool isPaused => this.timeElapsedBeforePause.HasValue;                               // 是否暂停
        public bool isCancelled => this.timeElapsedBeforeCancel.HasValue;                           // 是否取消
        public bool isDone => this.isCompleted || this.isCancelled || this.isOwnerDestroyed;        // 是否结束
        #endregion

        #region 私有静态 属性/字段
        private static TimerManager manager;
        #endregion

        #region 私有 属性/字段
        private bool isOwnerDestroyed => this.hasAutoDestroyOwner && this.autoDestroyOwner == null; // 是否拥有自动销毁的对象

        private readonly Action onComplete;                                                         // 完成时的回调
        private readonly Action<float> onUpdate;                                                    // 更新时的回调
        private float startTime;                                                                    // 开始时间
        private float lastUpdateTime;                                                               // 上次更新时间

        private float? timeElapsedBeforeCancel;                                                     // 取消时的时间
        private float? timeElapsedBeforePause;                                                      // 暂停时的时间

        private readonly MonoBehaviour autoDestroyOwner;                                            // 自动销毁的对象
        private readonly bool hasAutoDestroyOwner;                                                  // 是否有自动销毁的对象

        private bool isStopwatch; // 是否是正计时
        #endregion

        #region 公共方法
        /// <summary>
        /// 取消计时器
        /// </summary>
        public void Cancel()
        {
            if (this.isDone)
            {
                return;
            }

            this.timeElapsedBeforeCancel = this.GetTimeElapsed();
            this.timeElapsedBeforePause = null;
        }

        /// <summary>
        /// 暂停计时器
        /// </summary>
        public void Pause()
        {
            if (this.isPaused || this.isDone)
            {
                return;
            }

            this.timeElapsedBeforePause = this.GetTimeElapsed();
        }

        /// <summary>
        /// 恢复计时器
        /// </summary>
        public void Resume()
        {
            if (!this.isPaused || this.isDone)
            {
                return;
            }

            this.timeElapsedBeforePause = null;
        }

        /// <summary>
        /// 获取已经过去的时间
        /// </summary>
        /// <returns></returns>
        public float GetTimeElapsed()
        {
            if (this.isCompleted || this.GetWorldTime() >= this.GetFireTime())
            {
                return this.duration;
            }

            return this.timeElapsedBeforeCancel ??
                   this.timeElapsedBeforePause ??
                   this.GetWorldTime() - this.startTime;
        }

        /// <summary>
        /// 获取剩余时间
        /// </summary>
        /// <returns></returns>
        public float GetTimeRemaining()
        {
            return duration - GetTimeElapsed();
        }

        /// <summary>
        /// 获取完成比例
        /// </summary>
        /// <returns></returns>
        public float GetRatioComplete()
        {
            return GetTimeElapsed() / duration;
        }

        /// <summary>
        /// 获取剩余比例
        /// </summary>
        /// <returns></returns>
        public float GetRatioRemaining()
        {
            return GetTimeRemaining() / duration;
        }

        /// <summary>
        /// 启动计时器
        /// </summary>
        /// <param name="isStopwatch"> true 正计时 false 倒计时 </param>
        /// <param name="duration"> 倒计时持续时间 </param>
        public void StartTimer(bool isStopwatch, float duration = 0)
        {
            this.isStopwatch = isStopwatch;
            this.duration = duration;
            this.startTime = this.GetWorldTime();
            this.lastUpdateTime = this.startTime;
        }
        #endregion

        #region 公共静态方法
        /// <summary>
        /// 注册计时器
        /// </summary>
        /// <param name="duration"></param>
        /// <param name="onComplete"></param>
        /// <param name="onUpdate"></param>
        /// <param name="isLooped"></param>
        /// <param name="useRealTime"></param>
        /// <param name="autoDestroyOwner"></param>
        /// <returns></returns>
        public static Timer Register(float duration,
                                     Action onComplete,
                                     Action<float> onUpdate = null,
                                     bool isLooped = false,
                                     bool useRealTime = false,
                                     MonoBehaviour autoDestroyOwner = null)
        {
            if (Timer.manager == null)
            {
                TimerManager managerInScene = Object.FindObjectOfType<TimerManager>();
                if (managerInScene != null)
                {
                    Timer.manager = managerInScene;
                }
                else
                {
                    GameObject managerObject = new GameObject { name = "TimerManager" };
                    Timer.manager = managerObject.AddComponent<TimerManager>();
                }
            }

            Timer timer = new Timer(duration, onComplete, onUpdate, isLooped, useRealTime, autoDestroyOwner);
            Timer.manager.RegisterTimer(timer);
            return timer;
        }

        /// <summary>
        /// 取消计时器
        /// </summary>
        /// <param name="timer"></param>
        public static void Cancel(Timer timer)
        {
            timer?.Cancel();
        }

        /// <summary>
        /// 暂停计时器
        /// </summary>
        /// <param name="timer"></param>
        public static void Pause(Timer timer)
        {
            timer?.Pause();
        }

        /// <summary>
        /// 恢复计时器
        /// </summary>
        /// <param name="timer"></param>
        public static void Resume(Timer timer)
        {
            timer?.Resume();
        }

        /// <summary>
        /// 获取已经过去的时间
        /// </summary>
        public static void CancelAllRegisteredTimers()
        {
            Timer.manager?.CancelAllTimers();
        }

        /// <summary>
        /// 暂停所有计时器
        /// </summary>
        public static void PauseAllRegisteredTimers()
        {
            Timer.manager?.PauseAllTimers();
        }

        /// <summary>
        /// 恢复所有计时器
        /// </summary>
        public static void ResumeAllRegisteredTimers()
        {
            Timer.manager?.ResumeAllTimers();
        }
        #endregion

        #region 私有构造函数（使用静态Register方法创建新计时器）
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="duration"></param>
        /// <param name="onComplete"></param>
        /// <param name="onUpdate"></param>
        /// <param name="isLooped"></param>
        /// <param name="usesRealTime"></param>
        /// <param name="autoDestroyOwner"></param>
        private Timer(float duration,
                      Action onComplete,
                      Action<float> onUpdate,
                      bool isLooped,
                      bool usesRealTime,
                      MonoBehaviour autoDestroyOwner)
        {
            this.duration = duration;
            this.onComplete = onComplete;
            this.onUpdate = onUpdate;

            this.isLooped = isLooped;
            this.usesRealTime = usesRealTime;

            this.autoDestroyOwner = autoDestroyOwner;
            this.hasAutoDestroyOwner = autoDestroyOwner != null;

            this.startTime = this.GetWorldTime();
            this.lastUpdateTime = this.startTime;
        }

        #endregion

        #region 私有方法
        /// <summary>
        /// 获取世界时间
        /// </summary>
        /// <returns></returns>
        private float GetWorldTime()
        {
            return usesRealTime ? Time.realtimeSinceStartup : Time.time;
        }
        /// <summary>
        /// 获取触发时间
        /// </summary>
        /// <returns></returns>
        private float GetFireTime()
        {
            return startTime + duration;
        }
        /// <summary>
        /// 获取时间差
        /// </summary>
        /// <returns></returns>
        private float GetTimeDelta()
        {
            return GetWorldTime() - lastUpdateTime;
        }
        /// <summary>
        /// 更新
        /// </summary>
        private void Update()
        {
            if (isDone || isPaused) return;

            lastUpdateTime = GetWorldTime();
            onUpdate?.Invoke(GetTimeElapsed());

            if (isStopwatch)
            {
                if (GetWorldTime() >= GetFireTime())
                {
                    onComplete?.Invoke();
                    if (isLooped)
                    {
                        startTime = GetWorldTime();
                    }
                    else
                    {
                        isCompleted = true;
                    }
                }
            }
            else
            {
                if (GetTimeRemaining() <= 0)
                {
                    onComplete?.Invoke();
                    if (isLooped)
                    {
                        startTime = GetWorldTime();
                    }
                    else
                    {
                        isCompleted = true;
                    }
                }
            }
        }
        #endregion

        #region Manager类
        /// <summary>
        /// 计时器管理器
        /// </summary>
        private class TimerManager : MonoBehaviour
        {
            private readonly List<Timer> timers = new List<Timer>();
            private readonly Queue<Timer> timersToAdd = new Queue<Timer>();

            /// <summary>
            /// 注册计时器
            /// </summary>
            /// <param name="timer"></param>
            public void RegisterTimer(Timer timer)
            {
                timersToAdd.Enqueue(timer);
            }
            /// <summary>
            /// 取消所有计时器
            /// </summary>
            public void CancelAllTimers()
            {
                foreach (Timer timer in timers)
                {
                    timer.Cancel();
                }

                timers.Clear();
                timersToAdd.Clear();
            }
            /// <summary>
            /// 暂停所有计时器
            /// </summary>
            public void PauseAllTimers()
            {
                foreach (Timer timer in timers)
                {
                    timer.Pause();
                }
            }
            /// <summary>
            /// 恢复所有计时器
            /// </summary>
            public void ResumeAllTimers()
            {
                foreach (Timer timer in timers)
                {
                    timer.Resume();
                }
            }

            [UsedImplicitly]
            private void Update()
            {
                UpdateAllTimers();
            }
            /// <summary>
            /// 更新所有计时器
            /// </summary>
            private void UpdateAllTimers()
            {
                while (timersToAdd.Count > 0)
                {
                    timers.Add(timersToAdd.Dequeue());
                }

                foreach (Timer timer in timers)
                {
                    timer.Update();
                }

                timers.RemoveAll(t => t.isDone);
            }
        }
        #endregion
    }
}