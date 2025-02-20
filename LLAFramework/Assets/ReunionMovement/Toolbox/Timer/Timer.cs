using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;
using JetBrains.Annotations;
using Object = UnityEngine.Object;

/// <summary>
/// 允许您在不使用<see cref=“Coroutine”/>或<see cref=“MonoBehavior”/>的情况下延迟运行事件。
/// 要创建并启动计时器，请使用<see-cref=“Register”/>方法。
/// </summary>
namespace GameLogic
{
    public class Timer
    {
        #region 公共 属性/字段
        /// <summary>
        /// 计时器从开始到结束需要多长时间。
        /// </summary>
        public float duration { get; private set; }

        /// <summary>
        /// 计时器是否会在完成后再次运行。
        /// </summary>
        public bool isLooped { get; set; }

        /// <summary>
        /// 计时器是否已完成运行。如果计时器被取消，则为false。
        /// </summary>
        public bool isCompleted { get; private set; }

        /// <summary>
        /// 无论计时器使用实时时间还是游戏时间。实时性不受游戏时间尺度变化的影响（例如暂停、慢速移动），而游戏时间则受到影响。
        /// </summary>
        public bool usesRealTime { get; private set; }

        /// <summary>
        /// 计时器当前是否已暂停。
        /// </summary>
        public bool isPaused
        {
            get
            {
                return this.timeElapsedBeforePause.HasValue;
            }
        }

        /// <summary>
        /// 计时器是否已取消。
        /// </summary>
        public bool isCancelled
        {
            get
            {
                return this.timeElapsedBeforeCancel.HasValue;
            }
        }

        /// <summary>
        /// 获取计时器是否由于任何原因已完成运行。
        /// </summary>
        public bool isDone
        {
            get
            {
                return this.isCompleted || this.isCancelled || this.isOwnerDestroyed;
            }
        }
        #endregion

        #region 私有静态 属性/字段

        // 负责更新所有注册的定时器
        private static TimerManager manager;
        #endregion

        #region 私有 属性/字段
        private bool isOwnerDestroyed
        {
            get { return this.hasAutoDestroyOwner && this.autoDestroyOwner == null; }
        }

        private readonly Action onComplete;
        private readonly Action<float> onUpdate;
        private float startTime;
        private float lastUpdateTime;

        // 对于暂停，我们将开始时间提前一段时间。
        // 如果我们只检查开始时间与当前世界时间的对比，
        // 那么当我们被取消或暂停时，
        // 所花费的时间就会变得混乱，
        // 所以我们需要缓存在暂停/取消之前所花费的
        private float? timeElapsedBeforeCancel;
        private float? timeElapsedBeforePause;

        // 在自动销毁所有者被销毁后，
        // 计时器将过期，
        // 这样你就不会遇到任何恼人的错误，
        // 因为计时器在对象被销毁后运行和访问对象
        private readonly MonoBehaviour autoDestroyOwner;
        private readonly bool hasAutoDestroyOwner;
        #endregion

        #region 公共方法

        /// <summary>
        /// 停止正在进行或暂停的计时器。计时器的完成时回调将不会被调用。
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
        /// 暂停正在运行的计时器。暂停的计时器可以从暂停的同一点恢复。
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
        /// 继续暂停计时器。如果计时器未暂停，则不执行任何操作。
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
        /// 获取此计时器当前周期开始后经过的秒数。
        /// </summary>
        /// <returns>
        /// 自该计时器的当前循环开始以来所经过的秒数，即，如果计时器已循环，则为当前循环，如果计时器未循环则为启动。
        ///
        /// 如果计时器已经结束运行，则这等于持续时间。
        ///
        /// 如果计时器被取消/暂停，则等于从计时器启动到取消/暂停之间经过的秒数。
        /// </returns>
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
        /// 获取距离计时器完成还有多少秒。
        /// </summary>
        /// <returns>
        /// 计时器完成之前剩余的秒数。
        /// 计时器只有在未暂停、取消或完成的情况下才会经过时间。
        /// 如果计时器完成，这将等于零。
        /// </returns>
        public float GetTimeRemaining()
        {
            return duration - GetTimeElapsed();
        }

        /// <summary>
        /// 以比率的形式获取计时器从开始到结束的进度。
        /// </summary>
        /// <returns>一个从0到1的值，指示计时器的持续时间已经过去了多少.</returns>
        public float GetRatioComplete()
        {
            return GetTimeElapsed() / duration;
        }

        /// <summary>
        /// 以比率的形式获取计时器剩余的进度。
        /// </summary>
        /// <returns>一个从0到1的值，指示计时器的持续时间还有多长。</returns>
        public float GetRatioRemaining()
        {
            return GetTimeRemaining() / duration;
        }
        #endregion

        #region 公共静态方法
        /// <summary>
        /// 注册一个新的计时器，该计时器应在经过一定时间后触发事件。
        ///
        /// 场景更改时，已注册的计时器将被销毁。
        /// </summary>
        /// <param name="duration">计时器启动之前等待的时间，以秒为单位。</param>
        /// <param name="onComplete">计时器完成时触发的动作。</param>
        /// <param name="onUpdate">每次更新计时器时都应触发的操作。自计时器的当前循环开始以来经过的时间（以秒为单位）。</param>
        /// <param name="isLooped">计时器是否应在执行后重复。</param>
        /// <param name="useRealTime">
        /// 无论计时器使用实时（即不受暂停、慢/快动作的影响）还是游戏时间（将受暂停和慢/快运动的影响）。
        /// </param>
        /// <param name="autoDestroyOwner">
        /// 要将此计时器附加到的对象。
        /// 对象被销毁后，计时器将过期而不会执行。
        /// 这使您可以通过阻止计时器在父级被销毁后运行和访问其父级组件来避免令人讨厌的<see cref=“NullReferenceException”/>。
        /// </param>
        /// <returns>
        /// 一个计时器对象，允许您检查统计数据并停止/恢复进度。
        /// </returns>
        public static Timer Register(float duration,
                                     Action onComplete,
                                     Action<float> onUpdate = null,
                                     bool isLooped = false,
                                     bool useRealTime = false,
                                     MonoBehaviour autoDestroyOwner = null
        )
        {
            // 创建一个管理器对象来更新所有计时器（如果还不存在）。
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
        /// 取消计时器。与实例上的方法相比，这样做的主要好处是，
        /// 如果计时器为null，则不会得到<see cref=“NullReferenceException”/>。
        /// </summary>
        /// <param name="timer">要取消的计时器。</param>
        public static void Cancel(Timer timer)
        {
            if (timer != null)
            {
                timer.Cancel();
            }
        }

        /// <summary>
        /// 暂停计时器。与实例上的方法相比，这样做的主要好处是，
        /// 如果计时器为null，则不会得到<see cref=“NullReferenceException”/>。
        /// </summary>
        /// <param name="timer">要暂停的计时器。</param>
        public static void Pause(Timer timer)
        {
            if (timer != null)
            {
                timer.Pause();
            }
        }

        /// <summary>
        /// 恢复计时器。与实例上的方法相比，这样做的主要好处是，
        /// 如果计时器为null，则不会得到<see cref=“NullReferenceException”/>。
        /// </summary>
        /// <param name="timer">要恢复的计时器。</param>
        public static void Resume(Timer timer)
        {
            if (timer != null)
            {
                timer.Resume();
            }
        }

        /// <summary>
        /// 取消所有注册计时器
        /// </summary>
        public static void CancelAllRegisteredTimers()
        {
            if (Timer.manager != null)
            {
                Timer.manager.CancelAllTimers();
            }
            // 如果管理员不存在，我们还没有任何注册的计时器，所以在这种情况下不需要做任何事情
        }

        /// <summary>
        /// 暂停所有注册的计时器
        /// </summary>
        public static void PauseAllRegisteredTimers()
        {
            if (Timer.manager != null)
            {
                Timer.manager.PauseAllTimers();
            }
            // 如果管理员不存在，我们还没有任何注册的计时器，所以在这种情况下不需要做任何事情
        }

        /// <summary>
        /// 恢复所有注册的计时器
        /// </summary>
        public static void ResumeAllRegisteredTimers()
        {
            if (Timer.manager != null)
            {
                Timer.manager.ResumeAllTimers();
            }
            // 如果管理员不存在，我们还没有任何注册的计时器，所以在这种情况下不需要做任何事情
        }
        #endregion

        #region 私有构造函数（使用静态Register方法创建新计时器）

        private Timer(float duration,
                      Action onComplete,
                      Action<float> onUpdate,
                      bool isLooped,
                      bool usesRealTime,
                      MonoBehaviour autoDestroyOwner
        )
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
        /// 获取点火时间
        /// </summary>
        /// <returns></returns>
        private float GetFireTime()
        {
            return startTime + duration;
        }

        /// <summary>
        /// 获取时间增量
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
            if (isDone) return;

            if (isPaused)
            {
                startTime += GetTimeDelta();
                lastUpdateTime = GetWorldTime();
                return;
            }

            lastUpdateTime = GetWorldTime();

            onUpdate?.Invoke(GetTimeElapsed());

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
        #endregion

        #region Manager类（实现细节，自动生成并更新所有注册的定时器）
        /// <summary>
        /// 管理更新应用程序中运行的所有<see cref=“Timer”/>。
        /// 这将在您第一次创建计时器时实例化——您不需要手动将其添加到场景中。
        /// </summary>
        private class TimerManager : MonoBehaviour
        {
            private List<Timer> timers = new List<Timer>();

            // 缓冲区添加计时器，这样我们就不会在迭代过程中编辑集合
            private List<Timer> timersToAdd = new List<Timer>();

            /// <summary>
            /// 注册计时器
            /// </summary>
            /// <param name="timer"></param>
            public void RegisterTimer(Timer timer)
            {
                timersToAdd.Add(timer);
            }

            /// <summary>
            /// 取消全部计时器
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
            /// 暂停全部计时器
            /// </summary>
            public void PauseAllTimers()
            {
                foreach (Timer timer in timers)
                {
                    timer.Pause();
                }
            }

            /// <summary>
            /// 恢复全部计时器
            /// </summary>
            public void ResumeAllTimers()
            {
                foreach (Timer timer in timers)
                {
                    timer.Resume();
                }
            }

            /// <summary>
            /// 更新每帧上所有注册的定时器
            /// </summary>
            [UsedImplicitly]
            private void Update()
            {
                UpdateAllTimers();
            }

            /// <summary>
            /// 更新全部计时器
            /// </summary>
            private void UpdateAllTimers()
            {
                if (timersToAdd.Count > 0)
                {
                    timers.AddRange(timersToAdd);
                    timersToAdd.Clear();
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