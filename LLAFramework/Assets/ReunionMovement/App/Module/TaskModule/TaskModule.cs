using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace GameLogic
{
    /// <summary>
    /// 任务模块
    /// </summary>
    public class TaskModule : CustommModuleInitialize
    {
        #region 实例与初始化
        public static TaskModule Instance = new TaskModule();
        public bool IsInited { get; private set; }
        private double initProgress = 0;
        public double InitProgress { get { return initProgress; } }
        #endregion

        #region 初始化
        public IEnumerator Init()
        {
            initProgress = 0;

            // 初始化逻辑（如果有需要）
            yield return null;

            initProgress = 100;
            IsInited = true;
            Log.Debug("TaskModule 初始化完成");
        }

        public void ClearData()
        {
            Log.Debug("TaskModule 清除数据");
        }
        #endregion

        #region 任务管理
        /// <summary>
        /// 开始任务
        /// </summary>
        /// <param name="action"></param>
        /// <param name="callback"></param>
        public async void StartTask(Action action, Action callback, TimeSpan? timeout = null, CancellationToken cancellationToken = default)
        {
            try
            {
                await ExecuteTask(() => { action?.Invoke(); return true; }, timeout, cancellationToken);
                callback?.Invoke();
            }
            catch (Exception ex)
            {
                HandleTaskException(ex);
            }
        }

        /// <summary>
        /// 开始任务
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="func"></param>
        /// <param name="callback"></param>
        public async void StartTask<T>(Func<T> func, Action<T> callback, TimeSpan? timeout = null, CancellationToken cancellationToken = default)
        {
            try
            {
                T result = await ExecuteTask(func, timeout, cancellationToken);
                callback?.Invoke(result);
            }
            catch (Exception ex)
            {
                HandleTaskException(ex);
            }
        }

        /// <summary>
        /// 执行任务
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="func"></param>
        /// <param name="timeout"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task<T> ExecuteTask<T>(Func<T> func, TimeSpan? timeout = null, CancellationToken cancellationToken = default)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                throw new OperationCanceledException(cancellationToken);
            }

            Task<T> task = Task.Run(func, cancellationToken);

            if (timeout.HasValue)
            {
                Task delayTask = Task.Delay(timeout.Value, cancellationToken);
                Task completedTask = await Task.WhenAny(task, delayTask);

                if (completedTask == delayTask)
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        throw new OperationCanceledException(cancellationToken);
                    }
                    else
                    {
                        throw new TimeoutException("任务超时");
                    }
                }
            }

            return await task;
        }

        /// <summary>
        /// 处理任务异常
        /// </summary>
        /// <param name="ex"></param>
        private void HandleTaskException(Exception ex)
        {
            switch (ex)
            {
                case OperationCanceledException:
                    Log.Debug("任务被取消");
                    break;
                case TimeoutException timeoutEx:
                    Log.Debug($"任务超时: {timeoutEx.Message}");
                    break;
                default:
                    Log.Debug($"任务异常: {ex.Message}");
                    break;
            }
        }
        #endregion

        #region 更新
        public void UpdateTime(float elapseSeconds, float realElapseSeconds)
        {
            // 如果需要定时任务管理，可以在这里实现
        }
        #endregion
    }
}
