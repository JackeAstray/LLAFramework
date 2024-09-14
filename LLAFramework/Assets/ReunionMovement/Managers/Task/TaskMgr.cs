using GameLogic.Base;
using System;
using System.Threading;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

namespace GameLogic
{
    public class TaskMgr : SingletonMgr<TaskMgr>
    {
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
            catch (OperationCanceledException)
            {
                Debug.Log("任务被取消");
            }
            catch (TimeoutException ex)
            {
                Debug.Log($"任务超时: {ex.Message}");
            }
            catch (Exception ex)
            {
                Debug.Log($"任务异常: {ex.Message}");
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
            catch (OperationCanceledException)
            {
                Debug.Log("任务被取消");
            }
            catch (TimeoutException ex)
            {
                Debug.Log($"任务超时: {ex.Message}");
            }
            catch (Exception ex)
            {
                Debug.Log($"任务异常: {ex.Message}");
            }
        }

        /// <summary>
        /// 执行任务
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="func"></param>
        /// <param name="callback"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        private async Task<T> ExecuteTask<T>(Func<T> func, TimeSpan? timeout = null, CancellationToken cancellationToken = default)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                throw new OperationCanceledException(cancellationToken);
            }

            try
            {
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
            catch (OperationCanceledException)
            {
                throw;
            }
        }
    }
}